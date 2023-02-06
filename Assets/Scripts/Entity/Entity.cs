using Unity.Netcode;
using UnityEngine;

namespace Entities
{
    [RequireComponent(typeof(NetworkObject))]
    [DisallowMultipleComponent]
    public class Entity : NetworkBehaviour
    {
        public EntityStats entity;

        readonly NetworkVariable<int> currentHealth = new(0);

        public override void OnNetworkSpawn()
        {
            if (IsServer)
                currentHealth.Value = entity.Health;

            if (IsClient)
                enabled = false;
        }

        public void TakeDamage(int dmg, ulong attackerId)
        {
            if (!IsServer)
                return;

            Debug.Log(gameObject.name + " Took Damage: " + dmg + "; IsServer: " + IsServer);

            currentHealth.Value -= dmg;

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

            Debug.Log("Destroying " + gameObject.name + "; IsServer: " + IsServer);

            NetworkObject.Despawn(true);
        }
    }
}
