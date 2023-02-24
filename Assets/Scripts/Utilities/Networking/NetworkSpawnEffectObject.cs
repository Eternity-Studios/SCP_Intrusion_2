using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Utilities.Gameplay;

namespace Utilities.Networking
{
    public class NetworkSpawnEffectObject : NetworkBehaviour
    {
        public static NetworkSpawnEffectObject Singleton;

        public static Dictionary<uint, DestroyAfter> RegisteredPrefabs = new();
        public static Dictionary<DestroyAfter, uint> RegisteredPrefabsToID = new();

        private void Awake()
        {
            if (Singleton == null) Singleton = this;
            else Destroy(gameObject);
        }

        public void Spawn(DestroyAfter obj, Vector3 pos, Quaternion rot)
        {
            if (obj == null)
                return;

            if (RegisteredPrefabsToID.TryGetValue(obj, out uint id))
                Spawn(id, pos, rot);
            else
                Debug.LogError("Cannot find ID of effect object: " + obj.name);
        }

        public void Spawn(uint id, Vector3 pos, Quaternion rot)
        {
            if (!IsServer)
            {
                Debug.LogError("You are trying to spawn an object from a client! ");
                return;
            }

            SpawnClientRpc(id, pos, rot);
        }

        [ClientRpc]
        public void SpawnClientRpc(uint id, Vector3 pos, Quaternion rot)
        {
            if (RegisteredPrefabs.TryGetValue(id, out DestroyAfter obj))
                Instantiate(obj, pos, rot);
            else
                Debug.LogError("Cannot find effect object with ID: " + id);
        }
    }
}
