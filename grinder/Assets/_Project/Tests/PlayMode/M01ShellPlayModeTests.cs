using System.Collections;
using Meatgrinder.Movement;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Meatgrinder.Tests
{
    public sealed class M01ShellPlayModeTests
    {
        [UnityTest]
        public IEnumerator PlayerMovesJumpsAndClimbsProBuilderGeometry()
        {
            AsyncOperation load = SceneManager.LoadSceneAsync("M01_TheSlaughterhouse", LoadSceneMode.Single);
            Assert.IsNotNull(load);
            while (load.isDone == false)
            {
                yield return null;
            }

            ProBuilderMesh[] meshes = Object.FindObjectsByType<ProBuilderMesh>(FindObjectsSortMode.None);
            Assert.GreaterOrEqual(meshes.Length, 4);
            for (int i = 0; i < meshes.Length; i++)
            {
                MeshRenderer renderer = meshes[i].GetComponent<MeshRenderer>();
                Assert.IsNotNull(renderer);
                Assert.IsNotNull(renderer.sharedMaterial);
                string shaderName = renderer.sharedMaterial.shader.name;
                Assert.AreNotEqual("Hidden/InternalErrorShader", shaderName);
                Assert.IsTrue(renderer.sharedMaterial.shader.isSupported, shaderName);
                Assert.IsNotNull(meshes[i].GetComponent<Collider>());
                Assert.Greater(meshes[i].vertexCount, 0);
            }

            GameObject player = GameObject.Find("LocalPlayer");
            Assert.IsNotNull(player);
            PlayerMovement movement = player.GetComponent<PlayerMovement>();
            Assert.IsNotNull(movement);
            ScriptedPlanarInput input = player.AddComponent<ScriptedPlanarInput>();
            movement.SetPlanarInput(input);
            input.Move = new Vector2(0f, 1f);

            Vector3 start = player.transform.position;
            for (int i = 0; i < 45; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            Assert.Greater(player.transform.position.z, start.z + 0.5f);
            Assert.Greater(player.transform.position.y, -0.2f);

            input.JumpHeld = true;
            yield return new WaitForFixedUpdate();
            input.JumpHeld = false;
            float peak = player.transform.position.y;
            for (int i = 0; i < 25; i++)
            {
                yield return new WaitForFixedUpdate();
                if (player.transform.position.y > peak)
                {
                    peak = player.transform.position.y;
                }
            }

            Assert.Greater(peak, start.y + 0.35f);

            GameObject ramp = GameObject.Find("PB_Ramp");
            Assert.IsNotNull(ramp);
            Collider rampCollider = ramp.GetComponent<Collider>();
            Debug.Log(
                "[M01Test] rampPos=" + ramp.transform.position
                + " col=" + (rampCollider != null ? rampCollider.GetType().Name : "none")
                + " bounds=" + (rampCollider != null ? rampCollider.bounds.ToString() : "none"));
            player.transform.SetPositionAndRotation(
                new Vector3(ramp.transform.position.x, 0.05f, ramp.transform.position.z - 5.5f),
                Quaternion.identity);
            input.Move = new Vector2(0f, 1f);
            float beforeRamp = player.transform.position.y;
            for (int i = 0; i < 120; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            Debug.Log("[M01Test] rampDelta y0=" + beforeRamp + " y1=" + player.transform.position.y + " pos=" + player.transform.position);
            Assert.Greater(player.transform.position.y, beforeRamp + 0.35f);
        }

        sealed class ScriptedPlanarInput : MonoBehaviour, IPlanarInput
        {
            public Vector2 Move { get; set; }
            public bool JumpHeld { get; set; }
            public Vector2 LookDelta => Vector2.zero;
        }
    }
}
