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

        ReferenceHub referenceHub;

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
            
        }

        public void AssignReferenceHub(ReferenceHub hub)
        {
            referenceHub = hub;

            referenceHub.interact = this;
        }
    }
}
