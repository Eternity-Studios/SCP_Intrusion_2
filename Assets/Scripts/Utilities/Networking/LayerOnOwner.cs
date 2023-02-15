using Unity.Netcode;
using UnityEngine;

namespace Utilities.Networking
{
    public class LayerOnOwner : NetworkBehaviour
    {
        public LayerMask Layer;

        public GameObject[] ObjectsToSet;

        public bool Reverse;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsOwner && !Reverse || !IsOwner && Reverse) foreach (GameObject g in ObjectsToSet) g.layer = Layer;
        }
    }
}
