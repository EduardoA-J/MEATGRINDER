using System.Collections;
using System.Collections.Generic;
using Meatgrinder.Core;
using Meatgrinder.Presentation;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Meatgrinder.Tests
{
    public sealed class MenuPresentationPlayModeTests
    {
        static readonly string[] OptionKeys =
        {
            "Meatgrinder.Volume", "Meatgrinder.Sensitivity", "Meatgrinder.Fov",
            "Meatgrinder.ReduceMotion", "Meatgrinder.ReduceFlashes"
        };
        readonly bool[] hadOption = new bool[OptionKeys.Length];
        LocalGameSession session;
        float originalVolume, originalSensitivity, originalFov;
        bool originalMotion, originalFlashes;

        [UnityTearDown]
        public IEnumerator Cleanup()
        {
            Time.timeScale = 1;
            if (session != null)
            {
                session.SetOptions(originalVolume, originalSensitivity, originalFov, originalMotion, originalFlashes);
                for (int index = 0; index < OptionKeys.Length; index++)
                    if (!hadOption[index]) PlayerPrefs.DeleteKey(OptionKeys[index]);
                Object.Destroy(session.gameObject);
            }
            yield return null;
        }

        [UnityTest]
        public IEnumerator PauseMenuSupportsRaycastsNavigationAndReducedMotion()
        {
            yield return SceneManager.LoadSceneAsync("Boot");
            yield return null;
            session = Object.FindFirstObjectByType<LocalGameSession>();
            Assert.NotNull(session);
            originalVolume = session.Volume;
            originalSensitivity = session.Sensitivity;
            originalFov = session.FieldOfView;
            originalMotion = session.ReduceMotion;
            originalFlashes = session.ReduceFlashes;
            for (int index = 0; index < OptionKeys.Length; index++)
                hadOption[index] = PlayerPrefs.HasKey(OptionKeys[index]);

            session.SetOptions(originalVolume, originalSensitivity, originalFov, false, false);
            Assert.True(session.StartMap(session.Maps[0]));
            float deadline = Time.realtimeSinceStartup + 30;
            while (session.Phase == LocalSessionPhase.Loading && Time.realtimeSinceStartup < deadline)
                yield return null;
            Assert.AreEqual(LocalSessionPhase.Playing, session.Phase);

            session.Pause();
            Assert.AreEqual(0, Time.timeScale);
            var events = EventSystem.current;
            var resume = FindButton("Resume");
            var options = FindButton("Options");
            Assert.AreEqual(resume.gameObject, events.currentSelectedGameObject);
            var pausePage = (RectTransform)resume.transform.parent;
            Assert.AreEqual(0, pausePage.GetComponent<CanvasGroup>().alpha);
            yield return new WaitForSecondsRealtime(.25f);
            Assert.AreEqual(1, pausePage.GetComponent<CanvasGroup>().alpha);

            ExecuteEvents.Execute(resume.gameObject, new AxisEventData(events)
            {
                moveVector = Vector2.down,
                moveDir = MoveDirection.Down
            }, ExecuteEvents.moveHandler);
            Assert.AreEqual(options.gameObject, events.currentSelectedGameObject);
            events.SetSelectedGameObject(resume.gameObject);

            Canvas.ForceUpdateCanvases();
            var rect = (RectTransform)options.transform;
            var pointer = new PointerEventData(events)
            {
                position = RectTransformUtility.WorldToScreenPoint(null, rect.TransformPoint(rect.rect.center)),
                button = PointerEventData.InputButton.Left
            };
            var hits = new List<RaycastResult>();
            events.RaycastAll(pointer, hits);
            Assert.IsNotEmpty(hits);
            Assert.AreEqual(options.gameObject, hits[0].gameObject);
            ExecuteEvents.Execute(options.gameObject, pointer, ExecuteEvents.pointerEnterHandler);
            yield return null;
            var face = (RectTransform)options.transform.Find("Face");
            Assert.Greater(face.anchoredPosition.x, 0);
            ExecuteEvents.Execute(options.gameObject, pointer, ExecuteEvents.pointerDownHandler);
            Assert.Less(face.anchoredPosition.x, 0);
            ExecuteEvents.Execute(options.gameObject, pointer, ExecuteEvents.pointerUpHandler);
            ExecuteEvents.Execute(options.gameObject, pointer, ExecuteEvents.pointerClickHandler);
            var back = FindButton("Back");
            Assert.False(resume.IsInteractable());
            Assert.AreEqual("VOLUMENSlider", events.currentSelectedGameObject.name);

            session.SetOptions(originalVolume, originalSensitivity, originalFov, true, false);
            events.SetSelectedGameObject(back.gameObject);
            ExecuteEvents.Execute(back.gameObject, new BaseEventData(events), ExecuteEvents.submitHandler);
            Assert.AreEqual(resume.gameObject, events.currentSelectedGameObject);
            Assert.AreEqual(1, pausePage.GetComponent<CanvasGroup>().alpha);
            Assert.False(back.gameObject.activeInHierarchy);
            ExecuteEvents.Execute(options.gameObject, pointer, ExecuteEvents.pointerEnterHandler);
            yield return null;
            Assert.AreEqual(0, face.anchoredPosition.x);
        }

        [Test]
        public void FontRolesResolveProjectAssetsWithSpanishUiGlyphs()
        {
            Assert.AreEqual(Resources.Load<Font>("MeatgrinderFonts/MetalMania-Regular"), GameUi.FontFor(UiFontRole.Display));
            Assert.AreEqual(Resources.Load<Font>("MeatgrinderFonts/BarlowSemiCondensed-SemiBold"), GameUi.FontFor(UiFontRole.Industrial));
            Assert.AreEqual(Resources.Load<Font>("MeatgrinderFonts/IBMPlexMono-Medium"), GameUi.FontFor(UiFontRole.System));
            Assert.AreEqual(Resources.Load<Font>("MeatgrinderFonts/BarlowSemiCondensed-Regular"), GameUi.FontFor(UiFontRole.Body));

            foreach (var role in new[] { UiFontRole.Industrial, UiFontRole.System, UiFontRole.Body })
                foreach (char glyph in "áéíóúÁÉÍÓÚñÑ¿¡×—·")
                    Assert.True(GameUi.FontFor(role).HasCharacter(glyph), role + " missing " + glyph);
        }

        static MenuButton FindButton(string name)
        {
            foreach (var button in Object.FindObjectsByType<MenuButton>(FindObjectsSortMode.None))
                if (button.name == name && button.IsInteractable()) return button;
            Assert.Fail("Missing interactive menu button " + name);
            return null;
        }
    }
}
