using Unity.Netcode;
using UnityEngine;

namespace Utilities.Networking
{
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkSpawner : NetworkBehaviour
    {
        public NetworkObject Spawnee;

        public float MaxTimer = 1f;

        public bool OneTime = false;

        float timer;

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                enabled = false;
                return;
            }

            timer = MaxTimer;
        }

        private void Update()
        {
            if (!IsServer)
                return;

            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                timer = MaxTimer;

                NetworkObject no = Instantiate(Spawnee, transform.position, transform.rotation);
                no.Spawn();

                if (OneTime)
                    enabled = false;
            }
        }
    }
}
