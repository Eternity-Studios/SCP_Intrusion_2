using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AliveUI : MonoBehaviour
    {
        public static AliveUI Instance;

        [SerializeField]
        Image HP;
        [SerializeField]
        Image Stamina;

        private void Awake()
        {
            if (Instance != null) Destroy(gameObject);
            else Instance = this;
        }

        public void UpdateHP(float hp, float maxHp)
        {
            HP.fillAmount = Mathf.InverseLerp(0f, maxHp, hp);
        }

        public void UpdateStamina(float stam, float maxStam)
        {
            Stamina.fillAmount = Mathf.InverseLerp(0f, maxStam, stam);
        }
    }
}