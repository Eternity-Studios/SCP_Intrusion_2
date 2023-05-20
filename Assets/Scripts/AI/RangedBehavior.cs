namespace AI
{
    using EntitySystem;
    using UnityEngine;
    using Weapon;

    public class RangedBehavior : AIBehavior
    {
        public GunStats weapon;
        private float cooldown = float.NaN;
        private float cooldownCurrent = float.NaN;
        [SerializeField]
        private float range = -1f;
        protected override float GetRange => range;

        protected override void OnInsideRange(Entity target)
        {
            cooldownCurrent -= Time.fixedDeltaTime;
            if (!(cooldownCurrent <= 0f)) return;
            cooldownCurrent = cooldown;
            target.TakeDamage(weapon.Damage, AI.OwnerClientId);
        }

        protected override void Awake()
        {
            cooldown = 1f / weapon.RPS;
            cooldownCurrent = cooldown;
            base.Awake();
        }
    }
}