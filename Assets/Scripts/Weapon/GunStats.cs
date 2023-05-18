namespace Weapon
{
    using UnityEngine;
    using Utilities.Gameplay;

    [CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]
    public class GunStats : ScriptableObject
    {
        public float Damage = 25;
        public int Ammo = 15;

        public float Recoil = 1f;
        public float RecoilCooldown = 1f;
        public float CooldownDelay = 0.2f;
        public float RPS = 4f;
        public float ReloadTime = 1.5f;

        public float AIRange = 10f;

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
