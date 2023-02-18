using UnityEngine;

namespace Utilities.Audio
{
    public class AudioLoader : MonoBehaviour
    {
        public static AudioLoader Singleton;

        private void Awake()
        {
            if (Singleton == null) Singleton = this;
            else Destroy(gameObject);

            if (NetworkAudioManager.Sounds.Count < 1)
            {
                AudioClip[] a = Resources.LoadAll<AudioClip>("");

                if (a.Length < 1)
                    return;

                for (uint i = 0; i < a.Length; i++)
                {
                    NetworkAudioManager.Sounds.Add(i, a[i]);
                    NetworkAudioManager.SoundToID.Add(a[i], i);
                }
            }
        }
    }
}
