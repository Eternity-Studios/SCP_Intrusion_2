using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilities.Player;

namespace Player.Movement
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(NetworkObject))]
    [DisallowMultipleComponent]
    public class PlayerMovement : ReferenceHubModule
    {
        [Header("Basic Stats")]
        public float Speed = 5f;
        [Header("Dashing")]
        public float DashSpeed = 15f;
        public float DashDuration = 0.2f;
        [Header("Jumping")]
        public float JumpSpeed = 1f;
        public float JumpBufferTime = 0.2f;
        [Header("Gravity")]
        public float Gravity = 9.81f;
        public float StoppingForce = 10f;
        [Header("Stamina")]
        public float MaxStamina = 100f;
        public float StaminaRecovery = 25f;
        public float DashStaminaCost = 50f;

        public NetworkVariable<float> CurrentStamina = new(0);
        
        bool hasDoubleJumped = false;

        float dashTimer = 0f;

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
            if (IsServer)
            {
                CurrentStamina.Value = MaxStamina;
            }

            if (!IsOwner) return;

            inputs = new Game();
            movement = inputs.Player.Movement;

            inputs.Player.Jump.performed += JumpInput;
            inputs.Player.Dash.performed += DashInput;
            inputs.Player.Enable();
        }

        public override void OnDestroy()
        {
            if (!IsOwner) return;

            inputs.Player.Jump.performed -= JumpInput;
            inputs.Player.Dash.performed -= DashInput;
            inputs.Player.Disable();
        }

        private void Update()
        {
            if (!IsOwner) return;

            bool grounded = controller.isGrounded;

            if (dashTimer <= 0f)
                MovementInput = movement.ReadValue<Vector2>();

            if (grounded)
                hasDoubleJumped = false;

            if (jumpBuffer > 0f)
            {
                if (grounded)
                {
                    Jump();
                    jumpBuffer = 0f;
                }

                jumpBuffer -= Time.deltaTime;
            }
            else
                jumpBuffer = 0f;
        }

        private void FixedUpdate()
        {
            if (!IsOwner) return;

            if (controller.isGrounded && grav <= 0f)
                grav = Mathf.MoveTowards(grav, -0.3f, StoppingForce * Time.fixedDeltaTime);
            else if (!controller.isGrounded)
                grav -= Gravity * Time.fixedDeltaTime;

            Vector3 motion;

            if (dashTimer <= 0f)
                motion = MovementInput.x * Speed * Time.fixedDeltaTime * transform.right + MovementInput.y * Speed * Time.fixedDeltaTime * transform.forward + grav * Time.fixedDeltaTime * Vector3.up;
            else
            {
                if (MovementInput == Vector2.zero)
                    MovementInput = Vector2.up;
                motion = MovementInput.x * DashSpeed * Time.fixedDeltaTime * transform.right + MovementInput.y * DashSpeed * Time.fixedDeltaTime * transform.forward + grav * Time.fixedDeltaTime * Vector3.up;
            }

            controller.Move(motion);

            if (dashTimer > 0f)
                dashTimer -= Time.fixedDeltaTime;
            else if (dashTimer < 0f)
                dashTimer = 0f;

            if (IsServer)
            {
                if (CurrentStamina.Value < MaxStamina)
                {
                    CurrentStamina.Value = Mathf.Clamp(CurrentStamina.Value + StaminaRecovery * Time.fixedDeltaTime, 0f, MaxStamina);
                }
            }
        }

        public void JumpInput(InputAction.CallbackContext callbackContext)
        {
            if (!controller.isGrounded)
            {
                if (!hasDoubleJumped)
                {
                    Jump();
                    hasDoubleJumped = true;
                    return;
                }

                jumpBuffer = JumpBufferTime;
                return;
            }

            Jump();
        }

        public void DashInput(InputAction.CallbackContext callbackContext)
        {
            if (CurrentStamina.Value - 50f < 0f)
                return;

            Dash();
        }

        public void Jump()
        {
            grav = JumpSpeed;
        }

        public void Dash()
        {
            dashTimer = DashDuration;
            SubtractStaminaServerRpc(DashStaminaCost);
        }

        [ServerRpc]
        public void SubtractStaminaServerRpc(float amount)
        {
            CurrentStamina.Value -= amount;
        }

        public override void AssignController(PlayerController controller)
        {
            base.AssignController(controller);
            ReferenceHub.movement = this;
        }
    }
}
