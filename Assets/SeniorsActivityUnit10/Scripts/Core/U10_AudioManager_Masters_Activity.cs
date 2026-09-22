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

#if UNITY_EDITOR
            if (allAudioClips == null || allAudioClips.Count == 0 || clipMap.Count == 0)
            {
                AutoPopulateClips();
            }
            else
            {
                RegisterClips();
            }
#else
            RegisterClips();
#endif
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (allAudioClips == null || allAudioClips.Count == 0)
            {
                AutoPopulateClips();
            }
        }

        public void AutoPopulateClips()
        {
            if (allAudioClips == null) allAudioClips = new List<AudioClip>();
            
            string[] searchFolders = new[] { "Assets/SeniorsActivityUnit10/SFX", "Assets/SeniorsActivityUnit10/Audio" };
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:AudioClip", searchFolders);
            foreach (var guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                AudioClip clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                if (clip != null && !allAudioClips.Contains(clip))
                {
                    allAudioClips.Add(clip);
                }
            }
            RegisterClips();
        }
#endif

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

        public void RegisterClips()
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

            if (allAudioClips != null)
            {
                foreach (var clip in allAudioClips)
                {
                    if (clip != null)
                    {
                        AddClip(clip.name, clip);
                    }
                }
            }
        }

        private void AddClip(string name, AudioClip clip)
        {
            if (clip != null && !string.IsNullOrEmpty(name))
            {
                clipMap[name] = clip;
            }
        }

        public AudioClip GetClip(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            if (clipMap.TryGetValue(name, out AudioClip clip) && clip != null)
            {
                return clip;
            }

#if UNITY_EDITOR
            // Dynamic fallback search in project
            string[] searchFolders = new[] { "Assets/SeniorsActivityUnit10/SFX", "Assets/SeniorsActivityUnit10/Audio" };
            string[] guids = UnityEditor.AssetDatabase.FindAssets($"{name} t:AudioClip", searchFolders);
            if (guids != null && guids.Length > 0)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                AudioClip found = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                if (found != null)
                {
                    AddClip(name, found);
                    AddClip(found.name, found);
                    if (!allAudioClips.Contains(found)) allAudioClips.Add(found);
                    return found;
                }
            }
#endif
            return null;
        }

        public void PlaySFX(string clipName)
        {
            AudioClip clip = GetClip(clipName);
            if (clip != null)
            {
                sfxSource.PlayOneShot(clip);
            }
            else
            {
                Debug.LogWarning($"[U10_AudioManager] SFX clip not found: {clipName}");
            }
        }

        public float PlayVO(string voName)
        {
            AudioClip clip = GetClip(voName);
            return PlayVOClip(clip);
        }

        public float PlayVOClip(AudioClip clip)
        {
            if (clip != null)
            {
                if (voSource != null)
                {
                    voSource.Stop();
                    voSource.clip = clip;
                    voSource.Play();
                }
                return clip.length;
            }
            return 0f;
        }

        public float GetClipDuration(string clipName)
        {
            AudioClip clip = GetClip(clipName);
            return clip != null ? clip.length : 0f;
        }

        public bool IsVOPlaying => voSource != null && voSource.isPlaying;

        public void StopVO()
        {
            if (voSource != null) voSource.Stop();
        }

        public void PlayMusic(string clipName, bool loop = true)
        {
            AudioClip clip = GetClip(clipName);
            if (clip != null)
            {
                if (musicSource.clip == clip && musicSource.isPlaying) return;
                musicSource.clip = clip;
                musicSource.loop = loop;
                musicSource.Play();
            }
        }

        public void PlayAmbience()
        {
            AudioClip clip = ambGarden != null ? ambGarden : GetClip("AMB_Garden");
            if (clip != null && ambienceSource != null)
            {
                ambienceSource.clip = clip;
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
