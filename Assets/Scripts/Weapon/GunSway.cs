namespace Weapon.Visuals
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class GunSway : MonoBehaviour
    {
        public float Amount;
        public float MaxAmount;
        public float Smooth;

        Vector2 mouseInput;

        Game inputs;

        InputAction mouse;

        Quaternion originalRotation;

        private void Awake()
        {
            inputs = new Game();

            mouse = inputs.Player.Look;

            inputs.Player.Enable();

            originalRotation = transform.localRotation;
        }

        private void Update()
        {
            mouseInput = mouse.ReadValue<Vector2>();

            float factorX = Mathf.Clamp(-mouseInput.y * Amount, -MaxAmount, MaxAmount);
            float factorY = Mathf.Clamp(mouseInput.x * Amount, -MaxAmount, MaxAmount);

            Quaternion Final = Quaternion.Euler(originalRotation.x + factorX, originalRotation.y + factorY, originalRotation.z);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Final, Time.deltaTime * Smooth);
        }
    }
}