using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Movement
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        public float Speed = 5f;
        public float JumpSpeed = 1f;
        public float JumpBufferTime = 0.2f;
        public float Gravity = 9.81f;

        [HideInInspector]
        public Vector2 MovementInput;

        CharacterController controller;

        Game inputs;

        InputAction movement;

        float grav;
        float jumpBuffer;

        private void Awake()
        {
            inputs = new Game();
            movement = inputs.Player.Movement;

            controller = GetComponent<CharacterController>();

            inputs.Player.Jump.performed += JumpInput;

            inputs.Player.Enable();
        }

        private void OnDestroy()
        {
            inputs.Player.Jump.performed -= JumpInput;

            inputs.Player.Disable();
        }

        private void Update()
        {
            MovementInput = movement.ReadValue<Vector2>();

            if (jumpBuffer > 0f)
            {
                if (controller.isGrounded)
                {
                    Jump();
                    jumpBuffer = 0f;
                }

                jumpBuffer -= Time.deltaTime;
            }
            else
                jumpBuffer = 0f;

            if (controller.isGrounded && grav <= 0f)
                grav = -0.3f;
            else if (!controller.isGrounded)
                grav -= Gravity * Time.deltaTime;

            Vector3 motion = transform.right * MovementInput.x * Speed * Time.deltaTime + transform.forward * MovementInput.y * Speed * Time.deltaTime + Vector3.up * grav * Time.deltaTime;

            controller.Move(motion);
        }

        public void JumpInput(InputAction.CallbackContext callbackContext)
        {
            if (!controller.isGrounded)
            {
                jumpBuffer = JumpBufferTime;
                return;
            }

            Jump();
        }

        public void Jump()
        {
            grav = JumpSpeed;
        }
    }
}
