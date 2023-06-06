namespace Entities
{
    using EntitySystem;
    using global::Player.Management;
    using UI;
    using Unity.Netcode;
    using UnityEngine;
    using Utilities.Gameplay;
    using Utilities.Networking;

    public class Player : Entity
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (this.IsOwner && IsClient)
            {
                onHealthChange += UpdateHPUI;
            }
        }

        public override void OnDestroy()
        {
            if (IsOwner && IsClient)
            {
                onHealthChange -= UpdateHPUI;
            }
        }

        public void UpdateHPUI(float prevHP, float currHP)
        {
            AliveUI.Instance.UpdateHP(currHP, entity.Health);

            Debug.Log("Updating HP UI");
        }

        public override void Death(ulong attackerId)
        {
            if (!IsServer)
                return;

            if (entity.DeathObjects.Length > 0)
                foreach (DestroyAfter go in entity.DeathObjects)
                {
                    var transform1 = transform;
                    NetworkSpawnEffectObject.Singleton.Spawn(go, transform1.position, transform1.rotation);
                }

            OnDeath(attackerId);

            Debug.Log("Player " + OwnerClientId + " Has Died; IsServer: " + IsServer);

            PlayerLogic.OwnedInstance.SpawnSpectatorServerRpc(transform.position);

            NetworkObject.Despawn(true);
        }
    }
}
