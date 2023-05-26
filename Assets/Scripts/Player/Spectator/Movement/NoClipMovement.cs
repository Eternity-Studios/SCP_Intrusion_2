using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities.Player;

namespace Player.Movement
{
    public class NoClipMovement : ReferenceHubModule
    {
        public float Speed;

        [HideInInspector]
        public Vector2 MovementInput;

        [HideInInspector]
        public float AscensionInput;

        Game inputs;

        InputAction movement;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;

            inputs = new Game();

            movement = inputs.Player.Movement;
            inputs.Player.Crouch.performed += Descend;
            inputs.Player.Jump.performed += Ascend;
            inputs.Player.Crouch.canceled += Descend;
            inputs.Player.Jump.canceled += Ascend;

            inputs.Player.Enable();
        }

        public override void OnDestroy()
        {
            if (!IsOwner) return;

            inputs.Player.Crouch.performed -= Descend;
            inputs.Player.Jump.performed -= Ascend;
            inputs.Player.Crouch.canceled -= Descend;
            inputs.Player.Jump.canceled -= Ascend;

            inputs.Player.Disable();
        }

        private void Update()
        {
            if (!IsOwner) return;

            MovementInput = movement.ReadValue<Vector2>();
        }

        private void FixedUpdate()
        {
            transform.Translate(Speed * Time.fixedDeltaTime * MovementInput.y * transform.forward + Speed * Time.fixedDeltaTime * MovementInput.x * transform.right + Speed * Time.fixedDeltaTime * AscensionInput * Vector3.up);
        }

        public void Descend(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.performed)
                AscensionInput = -1f;
            else if (callbackContext.canceled)
                AscensionInput = 0f;
        }

        public void Ascend(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.performed)
                AscensionInput = 1f;
            else if (callbackContext.canceled)
                AscensionInput = 0f;
        }
    }
}
