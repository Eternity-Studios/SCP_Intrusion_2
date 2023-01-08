using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Movement
{
    public class PlayerLook : MonoBehaviour
    {
        public static PlayerLook Instance;

        public float Sensitivity = 0.45f;
        public float ClampAngle = 90f;

        public Transform camTransform;

        [HideInInspector]
        public Vector2 LookInput;

        Vector2 rot;

        Game inputActions;

        InputAction look;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            inputActions = new Game();

            look = inputActions.Player.Look;

            look.Enable();

            rot = camTransform.eulerAngles;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnDestroy()
        {
            look.Disable();
        }

        private void Update()
        {
            LookInput = look.ReadValue<Vector2>();

            Vector2 vec = rot;

            vec.y += LookInput.x * Sensitivity * 0.05f;
            vec.x += LookInput.y * Sensitivity * 0.05f;

            vec.x = Mathf.Clamp(vec.x, -ClampAngle, ClampAngle);

            rot = vec;

            camTransform.localRotation = Quaternion.Euler(-rot.x, 0f, 0f);
            transform.rotation = Quaternion.Euler(0f, rot.y, 0f);
        }

        public void CamAdd(float x)
        {
            rot.x += x;
        }
    }
}
