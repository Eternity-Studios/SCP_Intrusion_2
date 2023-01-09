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
            return SpawnPoint.spawnPoints[Random.Range(0, SpawnPoint.spawnPoints.Count)];
        }

        [ServerRpc(RequireOwnership = false)]
        public void SpawnPlayerServerRpc()
        {
            SpawnPoint sp = GetAvailableSpawnPoint();

            Debug.Log("Spawn Player! Client ID: " + OwnerClientId);

            Transform p = Instantiate(playerObject, sp.transform.position, Quaternion.identity);
            p.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId, true);
        }
    }
}
