namespace EntitySystem
{
    using System;
    using Unity.Netcode;
    using UnityEngine;
    using Utilities.Gameplay;
    using Utilities.Networking;

    [RequireComponent(typeof(NetworkObject))]
    [DisallowMultipleComponent]
    public class Entity : NetworkBehaviour, IDamageable
    {
        public EntityStats entity;

        readonly NetworkVariable<float> currentHealth = new();

        public float Health
        {
            get
            {
                return currentHealth.Value;
            }
            set
            {
                if (!IsServer)
                    return;

                float temp = currentHealth.Value;

                currentHealth.Value = value;

                OnHealthChange(temp, value);

                OnHealthChangeClientRpc(temp, value);
            }
        }

        public override void OnNetworkSpawn()
        {
            Debug.Log("Spawned Object: " + gameObject.name + ", currentHealth: " + Health);

            if (IsServer)
                Health = entity.Health;
        }

        public virtual void TakeDamage(float dmg, ulong attackerId)
        {
            if (!IsServer)
                return;

            Debug.Log(gameObject.name + " Took Damage: " + dmg + "; IsServer: " + IsServer);

            Health -= dmg;

            OnDamage(attackerId, dmg);

            if (Health <= 0)
                Death(attackerId);
        }

        public virtual void Death(ulong attackerId)
        {
            if (!IsServer)
                return;

            if (entity.DeathObjects.Length > 0)
                foreach (DestroyAfter go in entity.DeathObjects)
                    NetworkSpawnEffectObject.Singleton.Spawn(go, transform.position, transform.rotation);

            OnDeath(attackerId);

            Debug.Log("Destroying " + gameObject.name + "; IsServer: " + IsServer);

            NetworkObject.Despawn(true);
        }

        [ClientRpc]
        public void OnHealthChangeClientRpc(float prevHealth, float currHealth)
        {
            OnHealthChange(prevHealth, currHealth);
        }

        public event Action<ulong, float> onDamage;
        public void OnDamage(ulong attackerId, float dmg) { onDamage?.Invoke(attackerId, dmg); }

        public event Action<float, float> onHealthChange;
        public void OnHealthChange(float prevHealth, float currHealth) { onHealthChange?.Invoke(prevHealth, currHealth); }

        public event Action<ulong> onDeath;
        public void OnDeath(ulong attackerId) { onDeath?.Invoke(attackerId); }
    }

    public interface IDamageable
    {
        public void TakeDamage(float dmg, ulong attackerId);
    }
}
