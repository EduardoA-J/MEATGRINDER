using System.Collections;
using System.IO;
using System.Linq;
using Meatgrinder.Combat;
using Meatgrinder.Core;
using Meatgrinder.Pickups;
using Meatgrinder.Players;
using Meatgrinder.Presentation;
using Meatgrinder.Weapons;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Meatgrinder.Tests
{
    public sealed class UiIdentityPlayModeTests
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
        public IEnumerator EveryMenuStateAndEveryArenaUseTheSharedIdentity()
        {
            yield return SceneManager.LoadSceneAsync("Boot");
            yield return null;
            session = Object.FindFirstObjectByType<LocalGameSession>();
            yield return Capture("01-main");
            var play = Find("Play");
            var events = EventSystem.current;
            var pointer = new PointerEventData(events) { button = PointerEventData.InputButton.Left };
            events.SetSelectedGameObject(null);
            ExecuteEvents.Execute(play.gameObject, pointer, ExecuteEvents.pointerExitHandler);
            yield return Capture("02-button-normal");
            ExecuteEvents.Execute(play.gameObject, pointer, ExecuteEvents.pointerEnterHandler);
            yield return Capture("03-button-hover");
            ExecuteEvents.Execute(play.gameObject, pointer, ExecuteEvents.pointerDownHandler);
            yield return Capture("04-button-pressed");
            ExecuteEvents.Execute(play.gameObject, pointer, ExecuteEvents.pointerUpHandler);
            events.SetSelectedGameObject(play.gameObject);
            yield return Capture("05-button-selected");
            play.interactable = false;
            yield return Capture("06-button-disabled");
            Assert.AreEqual(GameUi.Muted, play.GetComponentInChildren<UnityEngine.UI.Text>().color);
            play.interactable = true;

            Find("Options").onClick.Invoke();
            yield return Capture("07-options");
            var slider = session.GetComponentsInChildren<UnityEngine.UI.Slider>().First();
            events.SetSelectedGameObject(slider.gameObject);
            yield return Capture("08-slider-selected");
            ExecuteEvents.Execute(slider.gameObject, pointer, ExecuteEvents.pointerEnterHandler);
            yield return Capture("09-slider-hover");
            slider.interactable = false;
            yield return Capture("10-slider-disabled");
            slider.interactable = true;
            Find("Back").onClick.Invoke();
            yield return null;
            Find("Credits").onClick.Invoke();
            yield return Capture("11-credits");
            Find("Back").onClick.Invoke();
            yield return null;
            Find("Create").onClick.Invoke();
            yield return Capture("12-maps");
            Find("M01").onClick.Invoke();
            yield return Capture("13-create");
            var invalid = ScriptableObject.CreateInstance<MapDefinition>();
            Assert.False(session.StartMap(invalid));
            yield return Capture("14-load-error");
            Object.Destroy(invalid);

            for (int index = 0; index < session.Maps.Length; index++)
            {
                float loadStarted = Time.realtimeSinceStartup;
                Assert.True(session.StartMap(session.Maps[index]));
                yield return Capture("15-loading-M0" + (index + 1), settle: false);
                yield return AwaitLoad();
                Assert.GreaterOrEqual(Time.realtimeSinceStartup - loadStarted, 2.95f, "Loading screen minimum duration");
                AssertIdentity(session.Player.transform);
                foreach (var text in Object.FindObjectsByType<TextMesh>(FindObjectsSortMode.None))
                {
                    Assert.AreSame(GameUi.FontFor(UiFontRole.Industrial), text.font, text.name);
                    Assert.AreEqual((Color32)GameUi.Paper, (Color32)text.color, text.name);
                }
                Assert.False(session.Player.GetComponent<CombatReadout>().enabled);
                Assert.False(session.Player.GetComponent<MovementProbeReadout>().enabled);
                yield return Capture("16-hud-M0" + (index + 1));
            }

            var health = session.Player.GetComponent<HealthComponent>();
            var weapons = session.Player.GetComponent<WeaponController>();
            foreach (var pickup in Object.FindObjectsByType<PickupController>(FindObjectsSortMode.None))
                if (pickup.Definition.Type == PickupType.PowerUp || pickup.Definition.PickupId == "shotgun_weapon")
                    pickup.Collect(health);
            yield return Capture("17-powers-ammo-message");
            session.Pause();
            yield return Capture("18-pause");
            Find("Options").onClick.Invoke();
            yield return Capture("19-pause-options");
            Find("Back").onClick.Invoke();
            yield return null;
            Find("Resume").onClick.Invoke();
            health.ReceiveConfirmed(10000);
            yield return Capture("20-death");
            session.Finish();
            yield return Capture("21-results");
            session.ReturnToMenu();
            yield return AwaitLoad();
            yield return Capture("22-return-menu");
        }

        static void AssertIdentity(Transform root)
        {
            foreach (var text in root.GetComponentsInChildren<UnityEngine.UI.Text>(true))
            {
                Assert.That(text.font, Is.EqualTo(GameUi.FontFor(UiFontRole.Industrial))
                    .Or.EqualTo(GameUi.FontFor(UiFontRole.System)).Or.EqualTo(GameUi.FontFor(UiFontRole.Display)), text.name);
            }
            foreach (var graphic in root.GetComponentsInChildren<UnityEngine.UI.Graphic>(true))
            {
                var color = graphic.color;
                if (color.a <= .001f) continue;
                Assert.LessOrEqual(color.b, color.r + .025f, "Blue UI: " + graphic.name);
                bool orange = color.r > color.g + .12f && color.g > color.b + .08f;
                Assert.False(orange, "Orange UI: " + graphic.name);
                if (graphic is UnityEngine.UI.RawImage raw && raw.texture != null)
                    Assert.AreEqual("UI/Default", raw.material.shader.name, "Keep original background colors: " + raw.name);
            }
        }
        IEnumerator Capture(string name, bool settle = true)
        {
            if (settle) yield return new WaitForSecondsRealtime(.3f);
            AssertIdentity(session.transform);
            Canvas.ForceUpdateCanvases();
            yield return new WaitForEndOfFrame();
            string folder = Path.GetFullPath(Path.Combine(Application.dataPath, "../../Documentation/test-results/ui-redesign"));
            Directory.CreateDirectory(folder);
            var texture = ScreenCapture.CaptureScreenshotAsTexture();
            File.WriteAllBytes(Path.Combine(folder, name + ".png"), texture.EncodeToPNG());
            Object.Destroy(texture);
        }
        IEnumerator AwaitLoad()
        {
            float deadline = Time.realtimeSinceStartup + 45;
            while (session.Phase == LocalSessionPhase.Loading && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.AreNotEqual(LocalSessionPhase.Loading, session.Phase);
            yield return null;
        }
        static MenuButton Find(string name) => Object.FindObjectsByType<MenuButton>(FindObjectsSortMode.None)
            .First(button => button.name == name && button.IsInteractable());
    }
}
