using Guns;
using Player.Movement;
using Unity.Netcode;
using UnityEngine;

namespace Player.Management
{
    [RequireComponent(typeof(NetworkObject))]
    [DisallowMultipleComponent]
    public class PlayerLogic : NetworkBehaviour
    {
        [HideInInspector]
        public NetworkObject WorldPlayer;

        [HideInInspector]
        public ReferenceHub referenceHub;

        [SerializeField]
        GameObject playerObject;

        public override void OnNetworkSpawn()
        {
            referenceHub = GetComponent<ReferenceHub>();

            referenceHub.logic = this;

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
            NetworkObject n = p.GetComponent<NetworkObject>();
            n.SpawnWithOwnership(OwnerClientId, true);
            WorldPlayer = n;

            referenceHub.look = p.GetComponent<PlayerLook>();
            referenceHub.movement = p.GetComponent<PlayerMovement>();
            referenceHub.weapon = p.GetComponent<Gun>();
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
