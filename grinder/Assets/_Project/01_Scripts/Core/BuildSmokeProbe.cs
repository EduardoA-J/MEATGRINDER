#if DEVELOPMENT_BUILD || UNITY_EDITOR
using System;
using System.Collections;
using Meatgrinder.Weapons;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Meatgrinder.Core
{
    public sealed class BuildSmokeProbe : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Begin()
        {
            if (System.Array.IndexOf(Environment.GetCommandLineArgs(), "--meatgrinder-smoke") < 0) return;
            DontDestroyOnLoad(new GameObject("BuildSmokeProbe", typeof(BuildSmokeProbe)));
        }
        IEnumerator Start()
        {
            bool fromStartup = SceneManager.GetActiveScene().name == "Startup";
            var session = FindFirstObjectByType<LocalGameSession>();
            if (session == null || (fromStartup && session.Phase != LocalSessionPhase.Loading)) { Fail("Startup loading"); yield break; }
            yield return WaitForLoad(session);
            if (session.Phase != LocalSessionPhase.Menu || (fromStartup && session.LoadingProgress != 1)) { Fail("Boot menu"); yield break; }
            if (!session.StartMap(session.Maps[0])) { Fail("Load request"); yield break; }
            yield return WaitForLoad(session);
            if (session.Player == null || session.Phase != LocalSessionPhase.Playing || session.LoadingProgress != 1 || session.Player.GetComponent<WeaponController>().SlotCount != 0)
            { Fail("Map and melee spawn"); yield break; }
            session.Pause(); yield return null;
            if (Time.timeScale != 0) { Fail("Pause"); yield break; }
            session.Resume(); session.Finish(); yield return null;
            if (session.Phase != LocalSessionPhase.Results) { Fail("Results"); yield break; }
            session.ReturnToMenu();
            yield return WaitForLoad(session);
            if (session.Phase != LocalSessionPhase.Menu || session.LoadingProgress != 1) { Fail("Return to menu"); yield break; }
            Debug.Log("[ProductSmoke] PASS " + (fromStartup ? "Startup -> " : "") + "Boot -> M01 melee-only -> pause -> results -> Boot");
            Application.Quit(0);
        }

        static IEnumerator WaitForLoad(LocalGameSession session)
        {
            float deadline = Time.realtimeSinceStartup + 40;
            while (session.Phase == LocalSessionPhase.Loading && Time.realtimeSinceStartup < deadline) yield return null;
        }
        static void Fail(string reason) { Debug.LogError("[ProductSmoke] FAIL " + reason); Application.Quit(1); }
    }
}
#endif
