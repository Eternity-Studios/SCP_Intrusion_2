using Unity.Netcode;
using UnityEngine;
using Utilities.Networking;

namespace Utilities.Player
{
    [RequireComponent(typeof(ReferenceHub))]
    public abstract class ReferenceHubModule : NetworkBehaviour
    {
        public ReferenceHub ReferenceHub { get; private set; }

        public virtual void AssignReferenceHub(ReferenceHub hub)
        {
            ReferenceHub = hub;
        }
    }
}
