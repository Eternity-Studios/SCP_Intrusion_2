using System.Collections.Generic;
using UnityEngine;

namespace Player.Management
{
    public class SpawnPoint : MonoBehaviour
    {
        public static List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

        [HideInInspector]
        public bool Used;

        private void Awake()
        {
            spawnPoints.Add(this);
        }

        public void ResetSpawn()
        {
            Used = false;
        }
    }
}
