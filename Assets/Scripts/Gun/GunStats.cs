using UnityEngine;

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
        public float Spread = 0.25f;
        public float RPS = 4f;

        public AnimationCurve RecoilPattern;

        public LayerMask HitMask;
        public GameObject[] HitObjects;
    }
}
