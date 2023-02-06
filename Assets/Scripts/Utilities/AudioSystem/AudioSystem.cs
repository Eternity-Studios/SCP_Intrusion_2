using UnityEngine;
using UnityEngine.Audio;
using Utilities.Gameplay;

namespace Utilities.Audio
{
    public static class AudioSystem
    {
        public static AudioMixerGroup audioMixer;

        #region Methods

        public static void PlaySound(AudioClip _sound, Vector2 _position, float _volume, int _priority)
        {
            GameObject soundObj = new("Sound", typeof(AudioSource), typeof(DestroyAfter));
            AudioSource au = soundObj.GetComponent<AudioSource>();
            DestroyAfter des = soundObj.GetComponent<DestroyAfter>();
            soundObj.transform.position = _position;
            au.playOnAwake = false;
            au.clip = _sound;
            au.volume = _volume;
            au.pitch = Time.timeScale;
            au.priority = _priority;
            des.Timer = _sound.length + 0.1f;
            au.minDistance = 1.5f;

            if (audioMixer == null)
                audioMixer = Resources.Load<AudioMixer>("AudioMixer").FindMatchingGroups("Master/Effects")[0];

            au.outputAudioMixerGroup = audioMixer;

            au.Play();
        }

        #endregion
    }
}

