using Unity.Netcode;
using UnityEngine;

namespace Utilities.Networking
{
    public class DisableOnOwner : NetworkBehaviour
    {
        public bool ActiveState;

        public GameObject[] ObjectsToSet;

        public bool Reverse;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsOwner && !Reverse || !IsOwner && Reverse) foreach (GameObject g in ObjectsToSet) g.SetActive(ActiveState);
        }
    }
}
