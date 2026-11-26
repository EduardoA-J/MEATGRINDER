using Meatgrinder.Combat;
using Meatgrinder.Movement;
using UnityEngine;

namespace Meatgrinder.Players
{
    public enum MonsterKind { None, Light, Heavy }

    [CreateAssetMenu(menuName = "MEATGRINDER/Monster")]
    public sealed class MonsterDefinition : ScriptableObject
    {
        [SerializeField] MonsterKind kind;
        [SerializeField] string displayName;
        [SerializeField] HealthProfile health;
        [SerializeField] MovementTuning movement;
        [SerializeField] MeleeTuning melee;
        [SerializeField] GameObject visualPrefab;

        public string DisplayName => displayName;
        public HealthProfile Health => health;
        public MovementTuning Movement => movement;
        public MeleeTuning Melee => melee;
        public GameObject VisualPrefab => visualPrefab;

        static MonsterDefinition light, heavy;

        public static MonsterDefinition Get(MonsterKind value)
        {
            var definition = value switch
            {
                MonsterKind.Light => light != null ? light : light = Resources.Load<MonsterDefinition>("DemonLight"),
                MonsterKind.Heavy => heavy != null ? heavy : heavy = Resources.Load<MonsterDefinition>("DemonHeavy"),
                _ => null
            };
            return definition != null && definition.kind == value ? definition : null;
        }

        public void Apply(GameObject body)
        {
            body.GetComponent<HealthComponent>().SetProfile(health);
            body.GetComponent<PlayerMovement>().SetTuning(movement);
            body.GetComponent<PlayerLook>().SetTuning(movement);
            body.GetComponent<PlayerMelee>().SetTuning(melee);
        }
    }
}
