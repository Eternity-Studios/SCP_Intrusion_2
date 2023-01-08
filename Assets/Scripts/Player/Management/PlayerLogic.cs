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
            Transform p = Instantiate(playerObject);
            p.GetComponent<NetworkObject>().Spawn();
        }
    }
}
