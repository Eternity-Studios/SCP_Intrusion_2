using Unity.Netcode;
using UnityEngine;

namespace Player.Management
{
    public class PlayerLogic : NetworkBehaviour
    {
        [SerializeField]
        Transform playerObject;

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
                SpawnPlayerServerRpc();
        }

        public SpawnPoint GetAvailableSpawnPoint()
        {
            foreach (SpawnPoint sp in SpawnPoint.spawnPoints)
            {
                if (!sp.Used)
                    return sp;
            }

            return null;
        }

        [ServerRpc(RequireOwnership = false)]
        public void SpawnPlayerServerRpc()
        {
            Debug.Log("Spawn Player! Client ID: " + OwnerClientId);
            Transform p = Instantiate(playerObject, GetAvailableSpawnPoint().transform.position, Quaternion.identity);
            p.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
        }
    }
}
