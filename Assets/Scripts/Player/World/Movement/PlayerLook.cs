using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities.Networking;
using Utilities.Player;


namespace Player.Movement
{
    [RequireComponent(typeof(NetworkObject))]
    [DisallowMultipleComponent]
    public class PlayerLook : ReferenceHubModule
    {
        public float Sensitivity = 0.45f;
        public float ClampAngle = 90f;

        public Transform camTransform;

        [HideInInspector]
        public Vector2 LookInput;

        Vector2 rot;

        Game inputActions;

        InputAction look;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                camTransform.GetComponentInChildren<Camera>().gameObject.SetActive(false);
                return;
            }

            inputActions = new Game();

            look = inputActions.Player.Look;
            look.Enable();

            rot = camTransform.eulerAngles;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public override void OnDestroy()
        {
            if (!IsOwner)
                return;

            look.Disable();
        }

        private void Update()
        {
            if (!IsOwner) return;

            LookInput = look.ReadValue<Vector2>();

            Vector2 vec = rot;

            vec.y += LookInput.x * Sensitivity * 0.05f;
            vec.x += LookInput.y * Sensitivity * 0.05f;

            vec.x = Mathf.Clamp(vec.x, -ClampAngle, ClampAngle);

            rot = vec;

            camTransform.localRotation = Quaternion.Euler(-rot.x, 0f, 0f);
            transform.rotation = Quaternion.Euler(0f, rot.y, 0f);
        }

        public void CamAdd(float x, float y)
        {
            rot.x += x;
            rot.y += y;
        }

        public void SetState(bool state)
        {
            if (!IsOwner) return;

            if (state)
            {
                look.Enable();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                look.Disable();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public override void AssignController(PlayerController controller)
        {
            base.AssignController(controller);
            ReferenceHub.look = this;
        }
    }
}
