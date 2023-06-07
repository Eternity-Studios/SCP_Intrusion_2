using System;
using UI;
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
        [Header("Crouching")]
        public float StandingHeight = 2f;
        public float StandingCameraHeight = 0.7f;
        public float CrouchingHeight = 1f;
        public float CrouchingCameraHeight = 0.1f;
        public float CrouchSpeed = 3f;
        [Header("Gravity")]
        public float Gravity = 9.81f;
        public float StoppingForce = 10f;
        [Header("Stamina")]
        public float MaxStamina = 100f;
        public float StaminaRecovery = 25f;
        public float DashStaminaCost = 50f;

        readonly NetworkVariable<float> CurrentStamina = new(0);
        readonly NetworkVariable<bool> Crouching = new(false);

        public float Stamina
        {
            get
            {
                return CurrentStamina.Value;
            }
            set
            {
                if (!IsServer)
                    return;

                float temp = CurrentStamina.Value;

                CurrentStamina.Value = value;

                OnStaminaChange(temp, value);

                OnStaminaChangeClientRpc(temp, value);
            }
        }

        bool hasDoubleJumped = false;

        float dashTimer = 0f;

        [HideInInspector]
        public Vector2 MovementInput;

        CharacterController controller;

        Game inputs;

        InputAction movement;

        float grav;
        float jumpBuffer;
        float currentSpeed;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                Stamina = MaxStamina;
            }

            if (!IsOwner) return;

            inputs = new Game();
            movement = inputs.Player.Movement;

            inputs.Player.Jump.performed += JumpInput;
            inputs.Player.Dash.performed += DashInput;
            inputs.Player.Crouch.performed += CrouchInput;
            inputs.Player.Crouch.canceled += CrouchInput;
            inputs.Player.Enable();

            if (IsClient)
                onStaminaChange += StaminaUI;
        }

        public override void OnDestroy()
        {
            if (!IsOwner) return;

            inputs.Player.Jump.performed -= JumpInput;
            inputs.Player.Dash.performed -= DashInput;
            inputs.Player.Crouch.performed -= CrouchInput;
            inputs.Player.Crouch.canceled -= CrouchInput;
            inputs.Player.Disable();

            if (IsClient)
                onStaminaChange -= StaminaUI;
        }

        public void StaminaUI(float prevStam, float currStam)
        {
            AliveUI.Instance.UpdateStamina(currStam, MaxStamina);

            Debug.Log("Updating Stamina UI");
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
            controller.height = Crouching.Value ? CrouchingHeight : StandingHeight;
            Vector3 vec = ReferenceHub.look.camTransform.localPosition;
            vec.y = Crouching.Value ? CrouchingCameraHeight : StandingCameraHeight;

            if (!IsOwner) return;

            if (controller.isGrounded && grav <= 0f)
                grav = Mathf.MoveTowards(grav, -0.3f, StoppingForce * Time.fixedDeltaTime);
            else if (!controller.isGrounded)
                grav -= Gravity * Time.fixedDeltaTime;

            currentSpeed = Crouching.Value ? CrouchSpeed : Speed;

            Vector3 motion;

            if (dashTimer <= 0f)
                motion = MovementInput.x * currentSpeed * Time.fixedDeltaTime * transform.right + MovementInput.y * currentSpeed * Time.fixedDeltaTime * transform.forward + grav * Time.fixedDeltaTime * Vector3.up;
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
                if (Stamina < MaxStamina)
                {
                    Stamina = Mathf.Clamp(CurrentStamina.Value + StaminaRecovery * Time.fixedDeltaTime, 0f, MaxStamina);
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
            if (Stamina - 50f < 0f)
                return;

            Dash();
        }

        public void CrouchInput(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.performed)
            {
                CrouchServerRpc(true);
            }
            else if (callbackContext.canceled)
            {
                CrouchServerRpc(false);
            }
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
        public void CrouchServerRpc(bool pressed)
        {
            Crouching.Value = pressed;
        }

        [ServerRpc]
        public void SubtractStaminaServerRpc(float amount)
        {
            Stamina -= amount;
        }

        public override void AssignController(PlayerController controller)
        {
            base.AssignController(controller);
            ReferenceHub.movement = this;
        }

        [ClientRpc]
        public void OnStaminaChangeClientRpc(float prevStam, float currStam)
        {
            OnStaminaChange(prevStam, currStam);
        }

        public event Action<float, float> onStaminaChange;
        public void OnStaminaChange(float prevStam, float currStam) { onStaminaChange?.Invoke(prevStam, currStam); }
    }
}
