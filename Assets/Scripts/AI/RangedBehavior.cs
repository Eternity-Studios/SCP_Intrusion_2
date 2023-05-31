namespace AI
{
    using EntitySystem;
    using UnityEngine;
    using Utilities.Audio;
    using Utilities.Gameplay;
    using Utilities.Networking;
    using Utilities.Player;
    using Weapon;

    public class RangedBehavior : AIBehavior
    {
        public GunStats weapon;
        public Transform shootPoint;
        private float cooldown = float.NaN;
        private float cooldownCurrent = float.NaN;
        private int ammo;
        [SerializeField]
        private float range = -1f;
        protected override float GetRange => range;
        private Entity _lastTarget = null;

        protected override void OnInsideRange(Entity target)
        {
            if (_lastTarget != target)
            {
                _lastTarget = target;
                cooldownCurrent = FirstAttackDelay;
            }
            cooldownCurrent -= Time.fixedDeltaTime;
            if (!(cooldownCurrent <= 0f)) return;
            if (ammo <= 0)
            {
                ammo = weapon.Ammo;
                cooldownCurrent = weapon.ReloadTime;
                return;
            }
            cooldownCurrent = cooldown;
            var sp = weapon.Spread.Evaluate(Mathf.InverseLerp(0f, weapon.Ammo, 0));
            shootPoint.localRotation = Quaternion.Euler(Random.Range(-sp, sp), Random.Range(-sp, sp), 0f);
            if (Physics.Raycast(shootPoint.position, shootPoint.forward, out RaycastHit hit, weapon.AIRange, weapon.HitMask, QueryTriggerInteraction.Ignore))
            {
                if (weapon.HitObjects.Length > 0)
                    foreach (DestroyAfter go in weapon.HitObjects)
                        NetworkSpawnEffectObject.Singleton.Spawn(go, hit.point, Quaternion.FromToRotation(Vector3.zero, hit.normal));

                if (weapon.ShootSounds.Length > 0)
                    if (NetworkAudioManager.SoundToID.TryGetValue(weapon.ShootSounds[Random.Range(0, weapon.ShootSounds.Length)], out uint id))
                    {
                        NetworkAudioManager.Singleton.PlaySoundClientRpc(id, transform.position, weapon.Volume, weapon.Priority);
                    }

                if (hit.collider.gameObject.GetComponent<Entity>())
                    target.TakeDamage(weapon.Damage, AI.OwnerClientId);
            }
            ammo--;
        }

        protected override void Awake()
        {
            cooldown = 1f / weapon.RPS;
            cooldownCurrent = cooldown;
            ammo = weapon.Ammo;
            base.Awake();
        }
    }
}