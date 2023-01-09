using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Player.Management
{
    public class SpawnPoint : MonoBehaviour
    {
        public static List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

        private void Awake()
        {
            spawnPoints.Add(this);
        }
    }
}
