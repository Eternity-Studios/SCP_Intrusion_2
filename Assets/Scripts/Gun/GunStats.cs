using UnityEngine;

namespace Guns
{
    public class GunStats : ScriptableObject
    {
        public int Damage = 25;

        public float Recoil = 1f;
        public float Spread = 0.25f;
    }
}
