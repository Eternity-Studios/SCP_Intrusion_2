using Unity.Netcode;
using UnityEngine;
using Utilities.Networking;

namespace Utilities.Player
{
    [RequireComponent(typeof(PlayerController))]
    public abstract class ReferenceHubModule : NetworkBehaviour
    {
        public ReferenceHub ReferenceHub => PlayerController.ReferenceHub;
        public PlayerController PlayerController { get; private set; }

        public virtual void AssignController(PlayerController controller)
        {
            PlayerController = controller;
        }
    }
}
