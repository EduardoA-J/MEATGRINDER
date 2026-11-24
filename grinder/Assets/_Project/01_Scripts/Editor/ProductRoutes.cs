using System.Linq;
using Meatgrinder.Core;
using Meatgrinder.Pickups;
using UnityEditor;
using UnityEngine;
using static Meatgrinder.Editor.ProductAssets;

namespace Meatgrinder.Editor
{
    public static class ProductRoutes
    {
        public static void Install(int index, Transform parent, Material dark, Material frame, Material glow)
        {
            if (parent.Find("KeyRoutes") != null) return;
            var root = new GameObject("KeyRoutes"); root.transform.SetParent(parent, false);
            var state = root.AddComponent<MapRouteState>();
            var definition = Data<KeyDefinition>(Root + "02_ScriptableObjects/Maps/M0" + (index + 1) + "_Key.asset");
            Set(definition, "keyId", "m0" + (index + 1) + "_route"); Set(definition, "displayName", "Llave M0" + (index + 1));
            var pickups = UnityEngine.Object.FindObjectsByType<PickupController>(FindObjectsSortMode.None);
            var health = pickups.First(p => p.Definition.Type == PickupType.Health);
            var key = new GameObject("RouteKey"); key.transform.SetParent(root.transform, false); key.transform.position = health.transform.position + Vector3.right * 2;
            var trigger = key.AddComponent<SphereCollider>(); trigger.radius = .8f; trigger.isTrigger = true;
            var visual = new GameObject("Visual"); visual.transform.SetParent(key.transform, false);
            Part(visual.transform, "KeyBow", PrimitiveType.Sphere, new Vector3(0, .7f, 0), Vector3.one * .35f, glow);
            Part(visual.transform, "KeyShaft", PrimitiveType.Cube, new Vector3(0, .35f, 0), new Vector3(.1f, .5f, .12f), glow);
            Part(visual.transform, "KeyTooth", PrimitiveType.Cube, new Vector3(.1f, .14f, 0), new Vector3(.25f, .08f, .12f), glow);
            var collectible = key.AddComponent<RouteKey>(); Set(collectible, "definition", definition); Set(collectible, "state", state); Set(collectible, "visual", visual.transform);
            var power = pickups.First(p => p.Definition.Type == PickupType.PowerUp && p.Definition.PowerUpType == PowerUpType.QuadDamage);
            Vector3 optional = power.transform.position + new Vector3(0, -.45f, -3);
            var optionalGate = Gate(root.transform, "PowerGate", optional, state, definition, dark, frame, glow);
            for (int side = -1; side <= 1; side += 2) Part(optionalGate.transform, "VaultSide", PrimitiveType.Cube, new Vector3(side * 3, 2, 3), new Vector3(.3f, 4, 6), dark, true);
            Part(optionalGate.transform, "VaultBack", PrimitiveType.Cube, new Vector3(0, 2, 6), new Vector3(6, 4, .3f), dark, true);
            Part(optionalGate.transform, "VaultRoof", PrimitiveType.Cube, new Vector3(0, 4.2f, 3), new Vector3(6, .3f, 6), dark, true);
            Vector3[] exits = { new Vector3(0, 0, 75), new Vector3(0, 0, 84), new Vector3(0, 3.6f, 78), new Vector3(0, 10.5f, 42) };
            var exit = Gate(root.transform, "AdvanceGate", exits[index], state, definition, dark, frame, glow);
            var gate = exit.GetComponent<KeyGate>(); Set(gate, "advancesMap", true);
            if (index < 3) Set(gate, "nextMap", Asset<MapDefinition>(Root + "02_ScriptableObjects/Maps/M0" + (index + 2) + ".asset"));
        }
        static GameObject Gate(Transform parent, string name, Vector3 position, MapRouteState state, KeyDefinition definition, Material dark, Material frame, Material glow)
        {
            var root = new GameObject(name); root.transform.SetParent(parent, false); root.transform.position = position;
            for (int side = -1; side <= 1; side += 2) Part(root.transform, "Jamb", PrimitiveType.Cube, new Vector3(side * 2.6f, 2.2f, 0), new Vector3(.5f, 4.4f, 1), frame, true);
            Part(root.transform, "Lintel", PrimitiveType.Cube, new Vector3(0, 4.4f, 0), new Vector3(5.7f, .6f, 1), frame, true);
            var door = Part(root.transform, "Door", PrimitiveType.Cube, new Vector3(0, 2, 0), new Vector3(4.7f, 4, .3f), dark, true);
            Part(door.transform, "LockMark", PrimitiveType.Cube, new Vector3(0, 0, -.6f), new Vector3(.1f, .6f, .1f), glow);
            var trigger = root.AddComponent<BoxCollider>(); trigger.isTrigger = true; trigger.center = new Vector3(0, 1.8f, .7f); trigger.size = new Vector3(4.5f, 3.6f, 1);
            var gate = root.AddComponent<KeyGate>(); Set(gate, "state", state); Set(gate, "requiredKey", definition); Set(gate, "door", door.transform);
            var sign = new GameObject("GateLabel"); sign.transform.SetParent(root.transform, false); sign.transform.localPosition = new Vector3(0, 4.8f, -.6f);
            var text = sign.AddComponent<TextMesh>(); text.text = name == "AdvanceGate" ? "SALIDA / LLAVE" : "QUAD / LLAVE"; text.fontSize = 48; text.characterSize = .045f; text.anchor = TextAnchor.MiddleCenter; text.color = new Color(1, .65f, .2f);
            ApplySignFont(text);
            return root;
        }
    }
}
