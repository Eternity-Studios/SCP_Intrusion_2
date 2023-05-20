namespace Items
{
    using Player.Management;
    using Unity.Netcode;
    using UnityEngine;
    using Weapon;

    [RequireComponent(typeof(NetworkObject))]
    [DisallowMultipleComponent]
    public class GunPickup : NetworkBehaviour, IInteractable
    {
        public GunStats gun;

        public void Interact(PlayerLogic player)
        {
            if (!IsServer)
                return;

            player.referenceHub.weapon.ServerSwitchWeapon(gun);

            NetworkObject.Despawn(true);
        }
    }
}
