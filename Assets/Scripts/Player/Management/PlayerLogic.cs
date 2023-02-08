using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Player.Management
{
    [RequireComponent(typeof(NetworkObject))]
    [DisallowMultipleComponent]
    public class PlayerLogic : NetworkBehaviour
    {
        [SerializeField]
        GameObject playerObject;

        public override void OnNetworkSpawn()
        {
            Invoke(nameof(SpawnPlayer), 0.5f);
        }

        public Transform GetAvailableSpawnPoint()
        {
            return SpawnPoint.Singleton.Spawns[Random.Range(0, SpawnPoint.Singleton.Spawns.Length)];
        }

        [ServerRpc]
        public void SpawnPlayerServerRpc()
        {
            Transform sp = GetAvailableSpawnPoint();

            Debug.Log("Spawn Player! Client ID: " + OwnerClientId);

            GameObject p = Instantiate(playerObject, sp.position, Quaternion.identity);
            p.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId, true);
        }

        private void SpawnPlayer()
        {
            if (IsOwner)
            {
                SpawnPlayerServerRpc();
            }
        }
    }
}
