using Meatgrinder.Players;
using Meatgrinder.Presentation;
using NUnit.Framework;
using UnityEngine;

namespace Meatgrinder.Tests
{
    public sealed class PresentationEditModeTests
    {
        [Test]
        public void ViewmodelKickDoesNotRotateTheCamera()
        {
            GameObject cameraObject = new GameObject("presentation-camera");
            cameraObject.AddComponent<Camera>();
            Quaternion before = cameraObject.transform.rotation;
            Vector3 position = cameraObject.transform.position;
            GameObject view = new GameObject("ShotgunView");
            view.transform.SetParent(cameraObject.transform, false);
            GameObject model = new GameObject("Model");
            model.transform.SetParent(view.transform, false);
            ShotgunView component = view.AddComponent<ShotgunView>();
            PlayerImpactFeedback feedback = cameraObject.AddComponent<PlayerImpactFeedback>();
            component.Bind(model.transform, null, feedback, null);
            component.enabled = false;
            component.enabled = true;
            feedback.Kick(2f);
            component.PlayShotPose();
            component.ApplyVisual(0.05f);
            Assert.Less(Quaternion.Angle(before, cameraObject.transform.rotation), 0.01f);
            Assert.AreEqual(position, cameraObject.transform.position);
            Assert.Less(model.transform.localPosition.z, 0.45f);
            Object.DestroyImmediate(cameraObject);
        }
    }
}
