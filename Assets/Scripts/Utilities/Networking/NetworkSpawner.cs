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

        private void FixedUpdate()
        {
            if (!IsServer)
                return;

            timer -= Time.fixedDeltaTime;

            if (timer <= 0f)
            {
                timer = MaxTimer;

                var transform1 = transform;
                NetworkObject no = Instantiate(Spawnee, transform1.position, transform1.rotation);
                no.Spawn(true);

                if (OneTime)
                    enabled = false;
            }
        }
    }
}
