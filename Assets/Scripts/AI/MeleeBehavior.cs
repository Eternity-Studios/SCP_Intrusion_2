namespace AI
{
    using EntitySystem;
    using UnityEngine;
    using Weapon;

    public class MeleeBehavior : AIBehavior
    {
        public KnifeStats weapon;
        private float cooldown = float.NaN;
        private float cooldownCurrent = float.NaN;
        protected override float GetRange => weapon.Range;

        protected override void OnInsideRange(Entity target)
        {
            cooldownCurrent -= Time.fixedDeltaTime;
            if (!(cooldownCurrent <= 0f)) return;
            cooldownCurrent = cooldown;
            target.TakeDamage(weapon.Damage, AI.OwnerClientId);
        }

        protected override void Awake()
        {
            cooldown = 1f / weapon.AttackSpeed;
            cooldownCurrent = cooldown;
            base.Awake();
        }
    }
}
