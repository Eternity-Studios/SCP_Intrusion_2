namespace EntitySystem
{
    using UnityEngine;

    public class BodyPart : MonoBehaviour, IDamageable
    {
        public float Multiplier;

        Entity owner;

        private void Awake()
        {
            owner = GetComponentInParent<Entity>();
        }

        public void TakeDamage(float dmg, ulong attackerId)
        {
            owner.TakeDamage(Mathf.RoundToInt(dmg * Multiplier), attackerId);
        }
    }
}
