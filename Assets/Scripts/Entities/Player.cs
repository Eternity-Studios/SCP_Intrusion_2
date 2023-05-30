namespace EntitySystem
{
    using UnityEngine;
    using Utilities.Gameplay;
    using Utilities.Networking;

    public class Player : Entity
    {
        public override void Death(ulong attackerId)
        {
            if (!IsServer)
                return;

            if (entity.DeathObjects.Length > 0)
                foreach (DestroyAfter go in entity.DeathObjects)
                    NetworkSpawnEffectObject.Singleton.Spawn(go, transform.position, transform.rotation);

            OnDeath(attackerId);

            Debug.Log("Player " + OwnerClientId + "Has Died; IsServer: " + IsServer);

            
        }
    }
}
