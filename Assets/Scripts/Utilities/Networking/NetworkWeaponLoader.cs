using Guns;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Networking
{
    public class NetworkWeaponLoader : MonoBehaviour
    {
        public static NetworkWeaponLoader Singleton;

        public static Dictionary<uint, GunStats> IDToWeapon = new();
        public static Dictionary<GunStats, uint> WeaponToID = new();

        private void Awake()
        {
            if (Singleton == null) Singleton = this;
            else Destroy(gameObject);

            if (IDToWeapon.Count < 1)
            {
                GunStats[] a = Resources.LoadAll<GunStats>("");

                if (a.Length < 1)
                    return;

                for (uint i = 0; i < a.Length; i++)
                {
                    IDToWeapon.Add(i, a[i]);
                    WeaponToID.Add(a[i], i);
                }
            }
        }
    }
}
