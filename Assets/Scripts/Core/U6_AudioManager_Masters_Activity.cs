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
                    sfxSource.PlayOneShot(entry.clip, entry.volume > 0 ? entry.volume : 1f);
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
                    voSource.Play();
                }
            }
            else
            {
                Debug.Log($"[U6_AudioManager] VO requested: {voId}");
            }
        }

        public void PlayAmbience(string ambId, bool loop = true)
        {
            if (soundDict.TryGetValue(ambId, out U6_SoundEntry_Masters_Activity entry))
            {
                if (entry.clip != null && ambSource != null)
                {
                    ambSource.clip = entry.clip;
                    ambSource.loop = loop;
                    ambSource.volume = entry.volume > 0 ? entry.volume : 0.5f;
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
    }
}
