using UnityEngine;

namespace Meatgrinder.Networking
{
    [CreateAssetMenu(menuName = "MEATGRINDER/Versus rules")]
    public sealed class VersusRules : ScriptableObject
    {
        // PENDIENTE (D-05 / D-14): values for product iteration.
        [Min(1)] public int RoundSeconds = 300;
        [Min(1)] public int ContinuousKillLimit = 20;
        [Range(1, 4)] public int PlayersPerTeam = 2;
        [Min(5)] public int SceneTimeoutSeconds = 45;
    }
}
