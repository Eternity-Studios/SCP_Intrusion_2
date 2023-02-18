using UnityEngine;

namespace UI
{
    public class Crosshair : MonoBehaviour
    {
        public static Crosshair Singleton;

        RectTransform rect;

        Vector2 vel;

        private void Awake()
        {
            if (Singleton == null) Singleton = this;
            else Destroy(gameObject);

            rect = GetComponent<RectTransform>();
        }

        public void SetSize(float s)
        {
            rect.sizeDelta = Vector2.SmoothDamp(rect.sizeDelta, new Vector2(s * 2f, s * 2f), ref vel, 0.1f);
        }
    }
}
