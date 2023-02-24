using Unity.Netcode;
using UnityEngine;
using Utilities.Gameplay;

namespace Guns
{
    [CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]
    public class GunStats : ScriptableObject
    {
        public int Damage = 25;
        public int Ammo = 15;

        public float Recoil = 1f;
        public float RecoilCooldown = 1f;
        public float CooldownDelay = 0.2f;
        public float RPS = 4f;
        public float ReloadTime = 1.5f;

        public AnimationCurve RecoilPattern;
        public AnimationCurve Spread;

        public LayerMask HitMask;
        public DestroyAfter[] HitObjects;

        public GameObject ViewModel;

        public AudioClip[] ShootSounds;

        public float Volume;

        public int Priority;
    }
}
