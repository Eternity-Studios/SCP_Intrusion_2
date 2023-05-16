using Unity.Netcode;
using UnityEngine;
using Utilities.Gameplay;

namespace Utilities.Networking
{
    public class NetworkPrefabLoader : MonoBehaviour
    {
        public static NetworkPrefabLoader Singleton;

        private void Awake()
        {
            if (Singleton == null) Singleton = this;
            else Destroy(gameObject);

            if (NetworkSpawnEffectObject.RegisteredPrefabs.Count < 1)
            {
                DestroyAfter[] a = Resources.LoadAll<DestroyAfter>("");

                if (a.Length < 1)
                    return;

                for (uint i = 0; i < a.Length; i++)
                {
                    NetworkSpawnEffectObject.RegisteredPrefabs.Add(i, a[i]);
                    NetworkSpawnEffectObject.RegisteredPrefabsToID.Add(a[i], i);
                }
            }
        }

        private void Start()
        {
            NetworkObject[] a = Resources.LoadAll<NetworkObject>(""); // TODO:

            if (a.Length < 1)
                return;

            foreach (NetworkObject no in a)
                NetworkManager.Singleton.AddNetworkPrefab(no.gameObject);
        }
    }
}
