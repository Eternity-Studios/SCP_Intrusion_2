using Player.Movement;
using Entities;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Random = UnityEngine.Random;

namespace Guns 
{
    [RequireComponent(typeof(NetworkObject))]
    [DisallowMultipleComponent]
    public class Gun : NetworkBehaviour
    {
        public GunStats gun;

        readonly NetworkVariable<int> currentAmmo = new(0);

        Game inputActions;

        Transform shootPoint;

        Entity owner;

        bool IsShooting;

        float timer;
        float shootFactor;

        private void Awake()
        {
            TryGetComponent(out owner);
        }

        private void Update()
        {
            if (!IsOwner) return;

            timer -= Time.deltaTime;

            if (shootFactor > 0f && timer <= -gun.CooldownDelay)
                shootFactor -= gun.RecoilCooldown * Time.deltaTime;
            else if (shootFactor < 0f)
                shootFactor = 0f;

            if (IsShooting)
                Shoot();
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
                currentAmmo.Value = gun.Ammo;

            if (!IsOwner) return;

            shootPoint = PlayerLook.OwnedInstance.camTransform.Find("FirePoint");

            inputActions = new Game();

            inputActions.Player.Shoot.performed += ShootInput;
            inputActions.Player.Shoot.canceled += ShootInput;

            inputActions.Player.Shoot.Enable();

            timer = 1f / gun.RPS;
        }

        public void Shoot()
        {
            if (timer > 0f)
                return;

            timer = 1f / gun.RPS;

            Spread();

            OnShoot();

            ShootServerRpc(shootPoint.position, shootPoint.forward);

            Recoil();

            shootFactor = Mathf.Clamp(shootFactor + 1f, 0f, gun.Ammo);

            if (shootFactor == gun.Ammo)
                shootFactor = 0f;
        }

        public void Spread()
        {
            shootPoint.localRotation = Quaternion.Euler(Random.Range(-gun.Spread, gun.Spread), Random.Range(-gun.Spread, gun.Spread), 0f);
        }

        public void Recoil()
        {
            PlayerLook.OwnedInstance.CamAdd(gun.Recoil, gun.RecoilPattern.Evaluate(shootFactor) * gun.Recoil);
        }

        [ServerRpc]
        public void ShootServerRpc(Vector3 position, Vector3 direction)
        {
            if (Physics.Raycast(position, direction, out RaycastHit _hit, Mathf.Infinity, gun.HitMask, QueryTriggerInteraction.Ignore))
            {
                foreach (NetworkObject go in gun.HitObjects)
                {
                    NetworkObject spawn = Instantiate(go, _hit.point, shootPoint.rotation);
                    spawn.Spawn(true);
                }

                Entity ent = _hit.transform.GetComponentInParent<Entity>();
                IHealth hit = _hit.transform.GetComponent<IHealth>();

                if (ent != null)
                {
                    if (ent.entity.Faction == owner.entity.Faction)
                        return;

                    hit.TakeDamage(gun.Damage, OwnerClientId);
                }
            }
        }

        public void ShootInput(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.performed)
                IsShooting = true;
            else if (callbackContext.canceled)
                IsShooting = false;
        }

        public event Action onShoot;
        public void OnShoot() { onShoot?.Invoke(); }
    }
}
