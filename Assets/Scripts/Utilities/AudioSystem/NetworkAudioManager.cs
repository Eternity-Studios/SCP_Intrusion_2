using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Utilities.Audio
{
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkAudioManager : NetworkBehaviour
    {
        public static NetworkAudioManager Singleton;

        public static Dictionary<uint, AudioClip> Sounds = new();
        public static Dictionary<AudioClip, uint> SoundToID = new();

        private void Awake()
        {
            if (Singleton == null) Singleton = this;
            else Destroy(gameObject);
        }

        [ClientRpc]
        public void PlaySoundClientRpc(uint id, Vector3 position, float volume, int priority, bool isOwner)
        {
            if (!Sounds.TryGetValue(id, out AudioClip sound))
                return;

            if (!isOwner)
                AudioSystem.PlaySound(sound, position, volume, priority);
        }
    }
}
