namespace Utilities.Player
{
    using Unity.Netcode;
    using Utilities.Networking;

    public class PlayerController : NetworkBehaviour
    {
        public ReferenceHub ReferenceHub { get; private set; }

        public void InitWithReferenceHub(ReferenceHub referenceHub)
        {
            ReferenceHub = referenceHub;
            referenceHub.controller = this;
            foreach (var module in GetComponentsInChildren<ReferenceHubModule>())
            {
                module.AssignController(referenceHub.controller);
            }
        }
    }
}