using System.Collections.Generic;
using UnityEngine;

namespace Googolplex.Unit6
{
    [System.Serializable]
    public struct U6_SoundEntry_Masters_Activity
    {
        public string id;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume;
    }

    public class U6_AudioManager_Masters_Activity : MonoBehaviour
    {
        public static U6_AudioManager_Masters_Activity Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource ambSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource voSource;

        [Header("Audio Clips Library")]
        [SerializeField] private List<U6_SoundEntry_Masters_Activity> sounds = new List<U6_SoundEntry_Masters_Activity>();

        private Dictionary<string, U6_SoundEntry_Masters_Activity> soundDict = new Dictionary<string, U6_SoundEntry_Masters_Activity>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (bgmSource != null) { bgmSource.playOnAwake = false; bgmSource.loop = true; bgmSource.volume = 0.20f; }
            if (ambSource != null) { ambSource.playOnAwake = false; ambSource.loop = true; ambSource.volume = 0.15f; }
            if (sfxSource != null) { sfxSource.playOnAwake = false; sfxSource.loop = false; sfxSource.volume = 0.85f; }
            if (voSource != null) { voSource.playOnAwake = false; voSource.loop = false; voSource.volume = 1f; }

            foreach (var entry in sounds)
            {
                if (!string.IsNullOrEmpty(entry.id) && !soundDict.ContainsKey(entry.id))
                {
                    soundDict.Add(entry.id, entry);
                }
            }
        }

        public void PlaySFX(string soundId)
        {
            if (soundDict.TryGetValue(soundId, out U6_SoundEntry_Masters_Activity entry))
            {
                if (entry.clip != null && sfxSource != null)
                {
                    sfxSource.PlayOneShot(entry.clip, entry.volume > 0 ? entry.volume * 0.85f : 0.85f);
                }
            }
            else
            {
                Debug.Log($"[U6_AudioManager] SFX requested: {soundId}");
            }
        }

        public void PlayVO(string voId)
        {
            if (soundDict.TryGetValue(voId, out U6_SoundEntry_Masters_Activity entry))
            {
                if (entry.clip != null && voSource != null)
                {
                    voSource.Stop();
                    voSource.clip = entry.clip;
                    voSource.volume = entry.volume > 0 ? entry.volume : 1f;
                    voSource.loop = false;
                    voSource.Play();
                }
            }
            else
            {
                Debug.Log($"[U6_AudioManager] VO requested: {voId}");
            }
        }

        public void StopVO()
        {
            if (voSource != null) voSource.Stop();
        }

        public void PlayAmbience(string ambId, bool loop = true)
        {
            if (soundDict.TryGetValue(ambId, out U6_SoundEntry_Masters_Activity entry))
            {
                if (entry.clip != null && ambSource != null)
                {
                    if (ambSource.clip == entry.clip && ambSource.isPlaying) return;

                    ambSource.Stop();
                    ambSource.clip = entry.clip;
                    ambSource.loop = loop;
                    ambSource.volume = entry.volume > 0 ? entry.volume * 0.15f : 0.15f;
                    ambSource.Play();
                }
            }
            else
            {
                Debug.Log($"[U6_AudioManager] Ambience requested: {ambId}");
            }
        }

        public void StopAmbience()
        {
            if (ambSource != null) ambSource.Stop();
        }

        public void PlayBGM(string bgmId)
        {
            if (soundDict.TryGetValue(bgmId, out U6_SoundEntry_Masters_Activity entry))
            {
                if (entry.clip != null && bgmSource != null)
                {
                    if (bgmSource.clip == entry.clip && bgmSource.isPlaying) return;

                    bgmSource.Stop();
                    bgmSource.clip = entry.clip;
                    bgmSource.loop = true;
                    bgmSource.volume = entry.volume > 0 ? entry.volume * 0.20f : 0.20f;
                    bgmSource.Play();
                }
            }
        }

        public void StopBGM()
        {
            if (bgmSource != null) bgmSource.Stop();
        }
    }
}
