using System.Collections;
using Meatgrinder.Movement;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Meatgrinder.Tests
{
    public sealed class MapLayoutPlayModeTests
    {
        [UnityTest]
        public IEnumerator SlaughterhouseKeepsTheRampAndTheFloor()
        {
            yield return Load("M01_TheSlaughterhouse");
            Assert.IsNotNull(GameObject.Find("PB_Ramp"));
            Assert.IsNotNull(GameObject.Find("PB_KillFloor"));
            Assert.GreaterOrEqual(Object.FindObjectsByType<ProBuilderMesh>(FindObjectsSortMode.None).Length, 100);
            Assert.IsNotNull(GameObject.Find("Spawn_Human_A"));
            Assert.IsNotNull(GameObject.Find("Spawn_Monster_A"));
            Assert.IsTrue(SpawnRayHitsGeometry("Spawn_Human_A", "Spawn_Monster_A"));
            yield return WalkStaysAbove(-0.5f);
        }

        [UnityTest]
        public IEnumerator CastleFloorHoldsThePlayer()
        {
            yield return Load("M02_CastleOfTheDamned");
            Assert.IsNotNull(GameObject.Find("PB_GreatHall"));
            Assert.IsNotNull(GameObject.Find("PB_TowerNW_Crown"));
            Assert.IsNotNull(GameObject.Find("PB_ArchNW_L"));
            Assert.IsNotNull(GameObject.Find("PB_WardNorth"));
            Assert.IsNotNull(GameObject.Find("PB_Keep_Crown"));
            Assert.IsNotNull(GameObject.Find("PB_RuinSpan"));
            Assert.GreaterOrEqual(Object.FindObjectsByType<ProBuilderMesh>(FindObjectsSortMode.None).Length, 350);
            Assert.IsTrue(SpawnRayHitsGeometry("Spawn_Human_A", "Spawn_Monster_A"));
            yield return WalkStaysAbove(-0.5f);
        }

        [UnityTest]
        public IEnumerator RuinsFloorHoldsThePlayer()
        {
            yield return Load("M03_RuinsOfTheForgotten");
            Assert.IsNotNull(GameObject.Find("PB_TempleFloor"));
            Assert.GreaterOrEqual(Object.FindObjectsByType<ProBuilderMesh>(FindObjectsSortMode.None).Length, 90);
            Assert.IsTrue(SpawnRayHitsGeometry("Spawn_Human_A", "Spawn_Monster_A"));
            yield return WalkStaysAbove(1f);
        }

        [UnityTest]
        public IEnumerator VoidHasNoGroundPlaneAndHoldsTheCore()
        {
            yield return Load("M04_TheLongestVoid");
            Assert.IsNull(GameObject.Find("PB_Ground"));
            Assert.IsNotNull(GameObject.Find("PB_Core"));
            Assert.GreaterOrEqual(Object.FindObjectsByType<ProBuilderMesh>(FindObjectsSortMode.None).Length, 60);
            Assert.IsTrue(SpawnRayHitsGeometry("Spawn_Human_A", "Spawn_Monster_A"));
            yield return WalkStaysAbove(2f);
        }

        static IEnumerator Load(string sceneName)
        {
            AsyncOperation load = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            Assert.IsNotNull(load);
            while (load.isDone == false)
            {
                yield return null;
            }
        }

        static IEnumerator WalkStaysAbove(float minimumY)
        {
            GameObject player = GameObject.Find("LocalPlayer");
            Assert.IsNotNull(player);
            PlayerMovement movement = player.GetComponent<PlayerMovement>();
            Assert.IsNotNull(movement);
            ScriptedPlanarInput input = player.AddComponent<ScriptedPlanarInput>();
            movement.SetPlanarInput(input);
            input.Move = new Vector2(0f, 1f);
            for (int i = 0; i < 40; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            Assert.Greater(player.transform.position.y, minimumY);
            ProBuilderMesh[] meshes = Object.FindObjectsByType<ProBuilderMesh>(FindObjectsSortMode.None);
            for (int i = 0; i < meshes.Length; i++)
            {
                Assert.IsNotNull(meshes[i].GetComponent<Collider>(), meshes[i].name);
                Assert.IsNotNull(meshes[i].GetComponent<MeshRenderer>().sharedMaterial);
                Assert.AreNotEqual("Hidden/InternalErrorShader", meshes[i].GetComponent<MeshRenderer>().sharedMaterial.shader.name);
            }
        }

        static bool SpawnRayHitsGeometry(string fromName, string toName)
        {
            GameObject from = GameObject.Find(fromName);
            GameObject to = GameObject.Find(toName);
            Assert.IsNotNull(from);
            Assert.IsNotNull(to);
            Vector3 origin = from.transform.position + (Vector3.up * 1.2f);
            Vector3 target = to.transform.position + (Vector3.up * 1.2f);
            Vector3 delta = target - origin;
            return Physics.Raycast(origin, delta.normalized, delta.magnitude - 0.2f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        }

        sealed class ScriptedPlanarInput : MonoBehaviour, IPlanarInput
        {
            public Vector2 Move { get; set; }
            public bool JumpHeld { get; set; }
            public Vector2 LookDelta => Vector2.zero;
        }
    }
}
