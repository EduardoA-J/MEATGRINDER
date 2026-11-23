using UnityEngine;
namespace Meatgrinder.Core
{
    [CreateAssetMenu(menuName = "MEATGRINDER/Route Key")]
    public sealed class KeyDefinition : ScriptableObject
    {
        [SerializeField] private string keyId = "route_key";
        [SerializeField] private string displayName = "Llave de ruta";
        public string KeyId => keyId;
        public string DisplayName => displayName;
    }
}
