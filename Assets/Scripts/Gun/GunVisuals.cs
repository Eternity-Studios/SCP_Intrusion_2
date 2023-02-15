using Unity.Netcode;
using UnityEngine;

namespace Guns
{
    public class GunVisuals : MonoBehaviour
    {
        [HideInInspector]
        public Gun gun;

        ParticleSystem particle;

        private void Awake()
        {
            particle = GetComponentInChildren<ParticleSystem>();

            gun = GetComponentInParent<Gun>();
        }

        private void Start()
        {
            gun.onShoot += OnShoot;
        }

        public void OnShoot()
        {
            particle.Play();
        }
    }
}
