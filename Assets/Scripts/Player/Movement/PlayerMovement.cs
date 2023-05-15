using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Movement
{
    using Utilities.Networking;
    using Utilities.Player;

    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(NetworkObject))]
    [DisallowMultipleComponent]
    public class PlayerMovement : ReferenceHubModule
    {
        public float Speed = 5f;
        public float JumpSpeed = 1f;
        public float JumpBufferTime = 0.2f;
        public float Gravity = 9.81f;
        public float StoppingForce = 10f;

        [HideInInspector]
        public Vector2 MovementInput;

        CharacterController controller;

        Game inputs;

        InputAction movement;

        float grav;
        float jumpBuffer;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;

            inputs = new Game();
            movement = inputs.Player.Movement;

            inputs.Player.Jump.performed += JumpInput;
            inputs.Player.Enable();
        }

        public override void OnDestroy()
        {
            if (!IsOwner) return;

            inputs.Player.Jump.performed -= JumpInput;
            inputs.Player.Disable();
        }

        private void Update()
        {
            if (!IsOwner) return;

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
                grav = Mathf.MoveTowards(grav, -0.3f, StoppingForce * Time.deltaTime);
            else if (!controller.isGrounded)
                grav -= Gravity * Time.deltaTime;

            Vector3 motion = MovementInput.x * Speed * Time.deltaTime * transform.right + MovementInput.y * Speed * Time.deltaTime * transform.forward + grav * Time.deltaTime * Vector3.up;

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

        public override void AssignReferenceHub(ReferenceHub hub)
        {
            base.AssignReferenceHub(hub);
            ReferenceHub.movement = this;
        }
    }
}
