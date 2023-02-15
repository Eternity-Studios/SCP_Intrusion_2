using System;
using Unity.Netcode;
using UnityEngine;

namespace Entities
{
    [RequireComponent(typeof(NetworkObject))]
    [DisallowMultipleComponent]
    public class Entity : NetworkBehaviour, IHealth
    {
        public EntityStats entity;

        readonly NetworkVariable<int> currentHealth = new(0);
        readonly NetworkVariable<bool> isDead = new(false);

        public override void OnNetworkSpawn()
        {
            if (IsClient)
                isDead.OnValueChanged += CheckDeath;

            if (IsServer)
                currentHealth.Value = entity.Health;
        }

        public void TakeDamage(int dmg, ulong attackerId)
        {
            if (!IsServer)
                return;

            Debug.Log(gameObject.name + " Took Damage: " + dmg + "; IsServer: " + IsServer);

            currentHealth.Value -= dmg;

            OnDamage(attackerId, dmg);

            if (currentHealth.Value <= 0)
                Death(attackerId);
        }

        public void Death(ulong attackerId)
        {
            if (!IsServer)
                return;

            foreach (GameObject go in entity.DeathObjects)
            {
                NetworkObject spawn = Instantiate(go, transform.position, transform.rotation).GetComponent<NetworkObject>();
                spawn.Spawn(true);
            }

            OnDeath(attackerId);

            Debug.Log("Destroying " + gameObject.name + "; IsServer: " + IsServer);
            isDead.Value = true;

            NetworkObject.Despawn(true);
        }

        private void CheckDeath(bool prev, bool curr)
        {
            if (curr)
                Destroy(gameObject);
        }

        public event Action<ulong, int> onDamage;
        public void OnDamage(ulong attackerId, int dmg) { onDamage?.Invoke(attackerId, dmg); }

        public event Action<ulong> onDeath;
        public void OnDeath(ulong attackerId) { onDeath?.Invoke(attackerId); }
    }

    public interface IHealth
    {
        public void TakeDamage(int dmg, ulong attackerId);
    }
}
