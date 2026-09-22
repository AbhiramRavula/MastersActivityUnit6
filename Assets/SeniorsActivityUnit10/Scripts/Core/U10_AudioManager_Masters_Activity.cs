using System;
using System.Collections.Generic;
using UnityEngine;

namespace Googolplex.Unit10
{
    public class U10_AudioManager_Masters_Activity : MonoBehaviour
    {
        public static U10_AudioManager_Masters_Activity Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource ambienceSource;
        [SerializeField] private AudioSource voSource;

        [Header("Audio Clips")]
        [SerializeField] private AudioClip sfxPlantGrow;
        [SerializeField] private AudioClip sfxFlower;
        [SerializeField] private AudioClip sfxMarble;
        [SerializeField] private AudioClip sfxJarFull;
        [SerializeField] private AudioClip sfxGoldenLine;
        [SerializeField] private AudioClip sfxTally;
        [SerializeField] private AudioClip sfxTap;
        [SerializeField] private AudioClip ambGarden;
        [SerializeField] private AudioClip musGarden;
        [SerializeField] private AudioClip musEndTerm;
        [SerializeField] private List<AudioClip> allAudioClips = new List<AudioClip>();

        private Dictionary<string, AudioClip> clipMap = new Dictionary<string, AudioClip>(StringComparer.OrdinalIgnoreCase);

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitAudioSources();
            RegisterClips();
        }

        private void Start()
        {
            PlayAmbience();
            PlayMusic("MUS_Garden", true);
        }

        private void InitAudioSources()
        {
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
            }
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
                musicSource.volume = 0.35f;
            }
            if (ambienceSource == null)
            {
                ambienceSource = gameObject.AddComponent<AudioSource>();
                ambienceSource.loop = true;
                ambienceSource.playOnAwake = false;
                ambienceSource.volume = 0.2f;
            }
            if (voSource == null)
            {
                voSource = gameObject.AddComponent<AudioSource>();
                voSource.playOnAwake = false;
                voSource.volume = 1.0f;
            }
        }

        private void RegisterClips()
        {
            clipMap.Clear();
            AddClip("SFX_PlantGrow", sfxPlantGrow);
            AddClip("SFX_Flower", sfxFlower);
            AddClip("SFX_Marble", sfxMarble);
            AddClip("SFX_JarFull", sfxJarFull);
            AddClip("SFX_GoldenLine", sfxGoldenLine);
            AddClip("SFX_Tally", sfxTally);
            AddClip("SFX_Tap", sfxTap);
            AddClip("AMB_Garden", ambGarden);
            AddClip("MUS_Garden", musGarden);
            AddClip("MUS_EndTerm", musEndTerm);

            foreach (var clip in allAudioClips)
            {
                if (clip != null)
                {
                    AddClip(clip.name, clip);
                }
            }
        }

        private void AddClip(string name, AudioClip clip)
        {
            if (clip != null)
            {
                clipMap[name] = clip;
            }
        }

        public void PlaySFX(string clipName)
        {
            if (clipMap.TryGetValue(clipName, out AudioClip clip))
            {
                sfxSource.PlayOneShot(clip);
            }
            else
            {
                Debug.LogWarning($"[U10_AudioManager] SFX clip not found: {clipName}");
            }
        }

        public void PlayVO(string voName)
        {
            if (clipMap.TryGetValue(voName, out AudioClip clip))
            {
                if (voSource != null)
                {
                    voSource.Stop();
                    voSource.clip = clip;
                    voSource.Play();
                }
            }
            else
            {
                Debug.LogWarning($"[U10_AudioManager] VO clip not found: {voName}");
            }
        }

        public void StopVO()
        {
            if (voSource != null) voSource.Stop();
        }

        public void PlayMusic(string clipName, bool loop = true)
        {
            if (clipMap.TryGetValue(clipName, out AudioClip clip))
            {
                if (musicSource.clip == clip && musicSource.isPlaying) return;
                musicSource.clip = clip;
                musicSource.loop = loop;
                musicSource.Play();
            }
        }

        public void PlayAmbience()
        {
            if (ambGarden != null && ambienceSource != null)
            {
                ambienceSource.clip = ambGarden;
                ambienceSource.loop = true;
                ambienceSource.Play();
            }
        }

        public void StopMusic()
        {
            if (musicSource != null) musicSource.Stop();
        }

        public void StopAmbience()
        {
            if (ambienceSource != null) ambienceSource.Stop();
        }
    }
}
