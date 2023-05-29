namespace Weapon
{
    using System;
    using System.Collections;
    using EntitySystem;
    using Player.Movement;
    using UI;
    using Unity.Netcode;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using Utilities.Audio;
    using Utilities.Gameplay;
    using Utilities.Networking;
    using Utilities.Player;
    using Random = UnityEngine.Random;

    [RequireComponent(typeof(NetworkObject))]
    [DisallowMultipleComponent]
    public class Gun : ReferenceHubModule
    {
        public GunStats gun;

        public Transform ViewModelTransform;

        readonly NetworkVariable<int> currentAmmo = new();

        Game inputActions;

        Transform shootPoint;

        Entity owner;

        Coroutine reloadOperation;

        bool IsShooting;

        float timer;
        float shootFactor;
        float shootFactorClamped;
        float sp;

        private void Update()
        {
            if (!IsOwner) return;

            timer -= Time.deltaTime;

            if (shootFactor > 0f && timer <= -gun.CooldownDelay)
            {
                shootFactor -= gun.RecoilCooldown * Time.deltaTime;
                shootFactorClamped -= gun.RecoilCooldown * Time.deltaTime;
            }
            else if (shootFactor < 0f)
            {
                shootFactor = 0f;
                shootFactorClamped = 0f;
            }

            sp = gun.Spread.Evaluate(Mathf.InverseLerp(0f, gun.Ammo, shootFactorClamped));
            Crosshair.Singleton.SetSize(sp * 10f);

            if (IsShooting)
                Shoot();
        }

        public void ServerSwitchWeapon(GunStats g)
        {
            if (!IsServer)
                return;

            gun = g;

            SwitchWeaponClientRpc(NetworkWeaponLoader.WeaponToID[g]);

            ServerResetWeapon();
        }

        public void ServerResetWeapon()
        {
            if (!NetworkObject.IsSpawned || !IsServer)
                return;

            if (reloadOperation != null)
                StopCoroutine(reloadOperation);

            foreach (Transform go in ViewModelTransform)
            {
                if (go != ViewModelTransform)
                    Destroy(go.gameObject);
            }

            Instantiate(gun.ViewModel, ViewModelTransform);

            currentAmmo.Value = gun.Ammo;

            ResetWeaponClientRpc();
        }

        [ClientRpc]
        public void SwitchWeaponClientRpc(uint gunID)
        {
            gun = NetworkWeaponLoader.IDToWeapon[gunID];
        }

        [ClientRpc]
        public void ResetWeaponClientRpc()
        {
            timer = 0f;
            shootFactor = 0f;
            shootFactorClamped = 0f;
            sp = 0f;

            IsShooting = false;

            foreach (Transform go in ViewModelTransform)
            {
                if (go != ViewModelTransform)
                    Destroy(go.gameObject);
            }

            Instantiate(gun.ViewModel, ViewModelTransform);
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
                currentAmmo.Value = gun.Ammo;

            if (!IsOwner) return;

            TryGetComponent(out owner);

            shootPoint = PlayerLook.OwnedInstance.camTransform.Find("FirePoint");

            inputActions = new Game();

            inputActions.Player.Shoot.performed += ShootInput;
            inputActions.Player.Reload.performed += ReloadInput;
            inputActions.Player.Shoot.canceled += ShootInput;

            inputActions.Player.Enable();

            timer = 1f / gun.RPS;
        }

        public void Shoot()
        {
            if (!NetworkObject.IsSpawned)
                return;

            if (timer > 0f || currentAmmo.Value <= 0)
                return;

            timer = 1f / gun.RPS;

            Spread();

            ShootServerRpc(shootPoint.position, shootPoint.forward, owner.entity.Faction);

            OnShootLocal();

            Recoil();

            shootFactor = Mathf.Clamp(shootFactor + 1f, 0f, gun.Ammo);
            shootFactorClamped = Mathf.Clamp(shootFactorClamped + 1f, 0f, gun.Ammo);

            if (shootFactor == gun.Ammo)
            {
                shootFactor = 0f;
                shootFactorClamped = gun.Ammo;
            }
        }

        public void Spread()
        {
            sp = gun.Spread.Evaluate(Mathf.InverseLerp(0f, gun.Ammo, shootFactorClamped));

            shootPoint.localRotation = Quaternion.Euler(Random.Range(-sp, sp), Random.Range(-sp, sp), 0f);
        }

        public void Recoil()
        {
            PlayerLook.OwnedInstance.CamAdd(gun.Recoil, gun.RecoilPattern.Evaluate(Mathf.InverseLerp(0f, gun.Ammo, shootFactor)) * gun.Recoil);
        }

        [ServerRpc]
        public virtual void ShootServerRpc(Vector3 position, Vector3 direction, Factions faction)
        {
            if (!NetworkObject.IsSpawned || currentAmmo.Value <= 0)
                return;

            ServerShoot(position, direction, faction);
        }

        public void ServerShoot(Vector3 position, Vector3 direction, Factions faction)
        {
            if (!IsServer)
                return;

            if (Physics.Raycast(position, direction, out RaycastHit _hit, Mathf.Infinity, gun.HitMask, QueryTriggerInteraction.Ignore))
            {
                ServerEffects(_hit);

                ServerAmmo();

                OnShoot();

                OnShootClientRpc();

                Entity ent = _hit.transform.GetComponentInParent<Entity>();
                IDamageable hit = _hit.transform.GetComponent<IDamageable>();

                if (ent != null)
                {
                    if (ent.entity.Faction == faction)
                        return;

                    hit.TakeDamage(gun.Damage, OwnerClientId);
                }
            }
        }

        [ServerRpc]
        public void ReloadServerRpc()
        {
            if (!NetworkObject.IsSpawned)
                return;

            if (currentAmmo.Value < gun.Ammo)
            {
                OnReloadPerformed(true);

                OnReloadPerformedClientRpc(true);

                reloadOperation = StartCoroutine(ServerReload());
            }

            OnReloadPerformed(false);

            OnReloadPerformedClientRpc(false);
        }

        [ServerRpc]
        public void ReloadInterruptServerRpc()
        {
            if (reloadOperation != null)
            {
                StopCoroutine(reloadOperation);

                OnReloadFinished(false);

                OnReloadFinishedClientRpc(false);
            }
        }

        public void ServerAmmo(int amount = 1)
        {
            if (!NetworkObject.IsSpawned)
                return;

            if (!IsServer)
                return;

            currentAmmo.Value -= amount;

            if (currentAmmo.Value < 0)
                currentAmmo.Value = 0;
        }

        public void ServerEffects(RaycastHit hit)
        {
            if (!IsServer)
                return;

            if (gun.HitObjects.Length > 0)
                foreach (DestroyAfter go in gun.HitObjects)
                    NetworkSpawnEffectObject.Singleton.Spawn(go, hit.point, Quaternion.FromToRotation(Vector3.zero, hit.normal));

            if (gun.ShootSounds.Length > 0)
                if (NetworkAudioManager.SoundToID.TryGetValue(gun.ShootSounds[Random.Range(0, gun.ShootSounds.Length)], out uint id))
                {
                    NetworkAudioManager.Singleton.PlaySoundClientRpc(id, transform.position, gun.Volume, gun.Priority, IsOwner);
                }
        }

        public IEnumerator ServerReload()
        {
            if (NetworkObject.IsSpawned)
            {
                if (IsServer)
                {
                    currentAmmo.Value = 0;

                    yield return new WaitForSeconds(gun.ReloadTime);

                    currentAmmo.Value = gun.Ammo;

                    OnReloadFinished(true);

                    OnReloadFinishedClientRpc(true);
                }
            }
        }

        public void ShootInput(InputAction.CallbackContext callbackContext)
        {
            if (!IsClient)
                return;

            if (callbackContext.performed)
                IsShooting = true;
            else if (callbackContext.canceled)
                IsShooting = false;
        }

        public void ReloadInput(InputAction.CallbackContext callbackContext)
        {
            if (!IsClient)
                return;

            ReloadServerRpc();
        }

        public override void AssignController(PlayerController controller)
        {
            base.AssignController(controller);
            ReferenceHub.weapon = this;
        }

        public event Action onShootLocal;
        public void OnShootLocal() { onShootLocal?.Invoke(); }

        public event Action onShoot;
        public void OnShoot() { onShoot?.Invoke(); }

        [ClientRpc]
        public void OnShootClientRpc()
        {
            if (!IsClient)
                return;

            OnShoot();
        }

        public event Action<bool> onReloadPerformed;
        public void OnReloadPerformed(bool performed) { onReloadPerformed?.Invoke(performed); }

        [ClientRpc]
        public void OnReloadPerformedClientRpc(bool performed)
        {
            if (!IsClient)
                return;

            OnReloadPerformed(performed);
        }

        public event Action<bool> onReloadFinished;
        public void OnReloadFinished(bool completed) { onReloadFinished?.Invoke(completed); }

        [ClientRpc]
        public void OnReloadFinishedClientRpc(bool completed)
        {
            if (!IsClient)
                return;

            OnReloadFinished(completed);
        }
    }
}
