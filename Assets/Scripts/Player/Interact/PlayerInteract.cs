using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Interact
{
    [RequireComponent(typeof(NetworkObject))]
    [DisallowMultipleComponent]
    public class PlayerInteract : NetworkBehaviour, IReferenceHub
    {
        public static PlayerInteract OwnedInstance;

        public float Distance;

        public LayerMask PickupLayer;

        private ReferenceHub referenceHub;

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
            if (Physics.Raycast(referenceHub.look.camTransform.position, referenceHub.look.camTransform.forward, out RaycastHit _hit, Distance, PickupLayer))
            {
                if (_hit.transform.TryGetComponent(out IInteractable interactable))
                {
                    interactable.Interact(referenceHub.logic);
                }
            }
        }

        public void AssignReferenceHub(ReferenceHub hub)
        {
            referenceHub = hub;

            referenceHub.interact = this;
        }
    }
}
