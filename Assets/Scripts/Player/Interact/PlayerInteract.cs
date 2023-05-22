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

            inputActions.Enable();
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
            Debug.DrawRay(ReferenceHub.look.camTransform.position, ReferenceHub.look.camTransform.forward, Color.blue, Distance);
        }

        [ServerRpc]
        public void InteractServerRpc()
        {
            ServerInteract();
        }

        public void ServerInteract()
        {
            if (!IsServer)
                return;

            if (Physics.Raycast(ReferenceHub.look.camTransform.position, ReferenceHub.look.camTransform.forward, out RaycastHit _hit, Distance, PickupLayer))
            {
                if (_hit.transform.TryGetComponent(out IInteractable interactable))
                {
                    interactable.Interact(ReferenceHub.logic);
                }
            }
        }

        public override void AssignController(PlayerController controller)
        {
            base.AssignController(controller);
            ReferenceHub.interact = this;
        }
    }
}
