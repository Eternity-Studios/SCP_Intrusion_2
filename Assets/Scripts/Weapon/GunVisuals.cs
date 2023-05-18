namespace Weapon
{
    using UnityEngine;

    public class GunVisuals : MonoBehaviour
    {
        [HideInInspector]
        public Gun gun;

        ParticleSystem particle;

        Animator anim;

        private void Awake()
        {
            particle = GetComponentInChildren<ParticleSystem>();
            anim = GetComponentInChildren<Animator>();

            gun = GetComponentInParent<Gun>();
        }

        private void Start()
        {
            if (!gun.IsOwner)
                gun.onShoot += OnShoot;
            else
                gun.onShootLocal += OnShoot;

            gun.onReloadPerformed += OnReloadPerformed;
            gun.onReloadFinished += OnReloadFinished;
        }

        public void OnShoot()
        {
            particle.Play();
        }

        public void OnReloadPerformed(bool performed)
        {
            if (!performed)
                return;

            anim.Play("StartReload");
        }

        public void OnReloadFinished(bool completed)
        {
            anim.Play("EndReload");
        }
    }
}
