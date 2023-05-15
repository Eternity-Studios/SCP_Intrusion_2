using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities.Networking;
using Utilities.Player;

namespace Player.Interact
{
    [RequireComponent(typeof(NetworkObject))]
    [DisallowMultipleComponent]
    public class PlayerInteract : ReferenceHubModule
    {
        public static PlayerInteract OwnedInstance;

        public float Distance;

        public LayerMask PickupLayer;

        Game inputActions;

        private void Awake()
        {
            inputActions = new Game();

            inputActions.Player.Interact.performed += ClientInteract;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
                return;

            if (OwnedInstance == null) OwnedInstance = this;
            else Destroy(gameObject);
        }

        public void ClientInteract(InputAction.CallbackContext callbackContext)
        {
            InteractServerRpc();
        }

        [ServerRpc]
        public void InteractServerRpc()
        {
            if (Physics.Raycast(ReferenceHub.look.camTransform.position, ReferenceHub.look.camTransform.forward, out RaycastHit _hit, Distance, PickupLayer))
            {
                if (_hit.transform.TryGetComponent(out IInteractable interactable))
                {
                    interactable.Interact(ReferenceHub.logic);
                }
            }
        }

        public void AssignReferenceHub(ReferenceHub hub)
        {
            base.AssignReferenceHub(hub);
            ReferenceHub.interact = this;
        }
    }
}
