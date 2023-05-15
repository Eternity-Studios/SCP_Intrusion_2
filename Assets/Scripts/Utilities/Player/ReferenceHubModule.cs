namespace Utilities.Player
{
    using Unity.Netcode;
    using UnityEngine;
    using Utilities.Networking;

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
