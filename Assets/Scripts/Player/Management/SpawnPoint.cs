using UnityEngine;

namespace Player.Management
{
    [DisallowMultipleComponent]
    public class SpawnPoint : MonoBehaviour
    {
        public static SpawnPoint Singleton;

        public Transform[] Spawns;

        private void Awake()
        {
            if (Singleton == null) Singleton = this;
            else Destroy(gameObject);
        }
    }
}
