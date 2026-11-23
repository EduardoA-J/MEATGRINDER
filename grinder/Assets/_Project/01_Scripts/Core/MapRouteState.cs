using System;
using System.Collections.Generic;
using UnityEngine;
namespace Meatgrinder.Core
{
    public sealed class MapRouteState : MonoBehaviour
    {
        readonly HashSet<string> keys = new HashSet<string>();
        public event Action<KeyDefinition> Unlocked;
        public bool Has(KeyDefinition key) => key == null || keys.Contains(key.KeyId);
        public bool Unlock(KeyDefinition key)
        {
            if (key == null || !keys.Add(key.KeyId)) return false;
            Unlocked?.Invoke(key); return true;
        }
    }
}
