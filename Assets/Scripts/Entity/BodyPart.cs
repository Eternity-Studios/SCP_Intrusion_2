using UnityEngine;

namespace Entities
{
    public class BodyPart : MonoBehaviour, IHealth
    {
        public float Multiplier;

        Entity owner;

        private void Awake()
        {
            owner = GetComponentInParent<Entity>();
        }

        public void TakeDamage(int dmg, ulong attackerId)
        {
            owner.TakeDamage(Mathf.RoundToInt(dmg * Multiplier), attackerId);
        }
    }
}
