using Player.Movement;
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

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
                return;

            if (OwnedInstance == null) OwnedInstance = this;
            else Destroy(gameObject);

            inputActions = new Game();

            inputActions.Player.Interact.performed += ClientInteract;

            inputActions.Enable();
        }

        public void ClientInteract(InputAction.CallbackContext callbackContext)
        {
            InteractServerRpc(PlayerLook.OwnedInstance.camTransform.position, PlayerLook.OwnedInstance.camTransform.forward);
        }

        [ServerRpc]
        public void InteractServerRpc(Vector3 position, Vector3 direction)
        {
            ServerInteract(position, direction);
        }

        public void ServerInteract(Vector3 position, Vector3 direction)
        {
            if (!IsServer)
                return;

            if (Physics.Raycast(position, direction, out RaycastHit _hit, Distance, PickupLayer))
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
