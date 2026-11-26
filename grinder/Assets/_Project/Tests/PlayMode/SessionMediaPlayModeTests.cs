using System.Collections;
using System.IO;
using System.Linq;
using Meatgrinder.Combat;
using Meatgrinder.Core;
using Meatgrinder.Presentation;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Meatgrinder.Tests
{
    public sealed class SessionMediaPlayModeTests
    {
        LocalGameSession session;

        [UnityTearDown]
        public IEnumerator Cleanup()
        {
            Time.timeScale = 1;
            if (session != null) Object.Destroy(session.gameObject);
            yield return null;
        }

        [UnityTest]
        public IEnumerator AllFourMapsUseTheirArtworkMusicFollowUpAndSingleDeathSound()
        {
            yield return SceneManager.LoadSceneAsync("Boot");
            yield return null;
            session = Object.FindFirstObjectByType<LocalGameSession>();
            var music = session.GetComponent<SessionMusic>();
            Assert.NotNull(music);
            yield return new WaitForSecondsRealtime(.25f);
            Assert.AreEqual("PrincipalMenu", music.CurrentSong.name);
            var menuImage = session.transform.Find("SessionScreens/Artwork").GetComponent<UnityEngine.UI.RawImage>();
            Assert.AreEqual("PrincipalMenu", menuImage.texture.name);
            Assert.False(menuImage.raycastTarget);
            yield return Capture("menu");

            for (int i = 0; i < 4; i++)
            {
                var map = session.Maps[i];
                Assert.NotNull(map.LoadingBackground);
                Assert.NotNull(map.FirstSong);
                Assert.True(session.StartMap(map));
                Assert.AreEqual(map, session.LoadingMap);
                var loading = session.transform.Find("LoadingScreen");
                Assert.True(loading.GetComponent<Canvas>().enabled);
                var image = loading.Find("Artwork").GetComponent<UnityEngine.UI.RawImage>();
                Assert.AreSame(map.LoadingBackground, image.texture);
                Assert.AreEqual("level" + (i + 1), image.texture.name);
                yield return Capture("loading-M0" + (i + 1));
                yield return AwaitLoad();
                Assert.AreEqual(map, session.CurrentMap);
                Assert.AreEqual(map.FirstSong, music.CurrentSong);
                Assert.AreEqual(1, Object.FindObjectsByType<SessionMusic>(FindObjectsSortMode.None).Length);
                var source = session.GetComponents<AudioSource>().Single(s => s.clip == music.CurrentSong);
                yield return WaitForAudio(source);
                yield return new WaitForSecondsRealtime(.25f);
                Assert.False(source.loop, "The level's first song must finish once.");
                float beforePause = source.time;
                session.Pause();
                yield return new WaitForSecondsRealtime(.15f);
                Assert.GreaterOrEqual(source.time, beforePause, "Pause must not restart music.");
                session.Resume();

                // Seek the real imported clip near its end; let the audio device complete it naturally.
                source.time = Mathf.Max(0, source.clip.length - .2f);
                float deadline = Time.realtimeSinceStartup + 5;
                while (music.CurrentSong == map.FirstSong && Time.realtimeSinceStartup < deadline) yield return null;
                Assert.AreEqual("afterFirstSong", music.CurrentSong.name, map.MapId);
                Assert.True(source.loop);
                yield return WaitForAudio(source);

                var health = session.Player.GetComponent<HealthComponent>();
                int deaths = 0;
                health.Died += () => deaths++;
                health.ReceiveConfirmed(10000);
                var deathSource = session.Player.GetComponents<AudioSource>().Single(s => s.clip != null && s.clip.name == "DeathSfx");
                yield return WaitForAudio(deathSource);
                yield return new WaitForSecondsRealtime(.15f);
                float deathPosition = deathSource.time;
                health.ReceiveConfirmed(10000);
                Assert.AreEqual(1, deaths);
                Assert.GreaterOrEqual(deathSource.time, deathPosition, "Repeated damage must not replay the death sound.");
                deadline = Time.realtimeSinceStartup + 6;
                while (!health.IsAlive && Time.realtimeSinceStartup < deadline) yield return null;
                Assert.True(health.IsAlive);
                health.ReceiveConfirmed(10000);
                Assert.AreEqual(2, deaths);
                Assert.Less(deathSource.time, .1f, "A new death must start the clip again.");

                var followUp = music.CurrentSong;
                session.Restart();
                yield return AwaitLoad();
                Assert.AreSame(followUp, music.CurrentSong, "Reloading the same map must not reset its playlist.");
                session.Finish();
                Assert.AreSame(followUp, music.CurrentSong);
            }
            session.ReturnToMenu();
            yield return AwaitLoad();
            Assert.AreEqual(LocalSessionPhase.Menu, session.Phase);
            Assert.AreEqual("PrincipalMenu", music.CurrentSong.name);
            Assert.AreEqual(1, session.GetComponents<AudioSource>().Count(s => s.isPlaying && s.loop));
        }

        [UnityTest]
        public IEnumerator BackgroundPreservesAspectForWideAndTallScreens()
        {
            var root = new GameObject("BackgroundTest", typeof(RectTransform));
            var texture = new Texture2D(160, 90);
            try
            {
                var background = ScreenBackground.Create(root.transform, texture);
                var image = background.GetComponent<UnityEngine.UI.RawImage>();
                foreach (var size in new[] { new Vector2(1366, 768), new Vector2(1920, 1080), new Vector2(1024, 768), new Vector2(2560, 1080), new Vector2(600, 900) })
                {
                    ((RectTransform)root.transform).sizeDelta = size;
                    yield return null;
                    var uv = image.uvRect;
                    Assert.That((texture.width * uv.width) / (texture.height * uv.height), Is.EqualTo(size.x / size.y).Within(.001f));
                    Assert.That(uv.xMin, Is.GreaterThanOrEqualTo(0));
                    Assert.That(uv.yMin, Is.GreaterThanOrEqualTo(0));
                    Assert.That(uv.xMax, Is.LessThanOrEqualTo(1));
                    Assert.That(uv.yMax, Is.LessThanOrEqualTo(1));
                }
            }
            finally { Object.Destroy(root); Object.Destroy(texture); }
        }

        IEnumerator AwaitLoad()
        {
            float deadline = Time.realtimeSinceStartup + 45;
            while (session.Phase == LocalSessionPhase.Loading && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.AreNotEqual(LocalSessionPhase.Loading, session.Phase);
            Assert.IsNull(session.LoadError);
            yield return null;
        }
        static IEnumerator WaitForAudio(AudioSource source)
        {
            float deadline = Time.realtimeSinceStartup + 5;
            while (!source.isPlaying && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.True(source.isPlaying, "Audio must actually be playing.");
        }
        static IEnumerator Capture(string name)
        {
            yield return new WaitForEndOfFrame();
            string folder = Path.GetFullPath(Path.Combine(Application.dataPath, "../../Documentation/test-results/session-media"));
            Directory.CreateDirectory(folder);
            var texture = ScreenCapture.CaptureScreenshotAsTexture();
            File.WriteAllBytes(Path.Combine(folder, name + ".png"), texture.EncodeToPNG());
            Object.Destroy(texture);
        }
    }
}
