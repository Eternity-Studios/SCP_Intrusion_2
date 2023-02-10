using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Utilities.Audio
{
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkAudioManager : NetworkBehaviour
    {
        public static NetworkAudioManager Singleton;

        public static Dictionary<string, AudioClip> Sounds;

        private void Awake()
        {
            if (Singleton == null) Singleton = this;
            else Destroy(gameObject);
        }

        [ClientRpc]
        public void PlaySoundClientRpc(FixedString64Bytes id, Vector3 position, float volume, int priority) // 30 characters
        {
            if (!Sounds.TryGetValue(id.ToString(), out AudioClip sound))
                return;

            AudioSystem.PlaySound(sound, position, volume, priority);
        }
    }
}
