namespace EntitySystem
{
    using UI;
    using UnityEngine;
    using Utilities.Gameplay;
    using Utilities.Networking;

    public class Player : Entity
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsOwner && IsClient)
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
                    NetworkSpawnEffectObject.Singleton.Spawn(go, transform.position, transform.rotation);

            OnDeath(attackerId);

            Debug.Log("Player " + OwnerClientId + " Has Died; IsServer: " + IsServer);
        }
    }
}
