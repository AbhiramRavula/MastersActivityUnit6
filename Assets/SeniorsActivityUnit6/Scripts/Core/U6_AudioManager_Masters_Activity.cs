using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

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

        private Dictionary<string, U6_SoundEntry_Masters_Activity> soundDict = new Dictionary<string, U6_SoundEntry_Masters_Activity>(System.StringComparer.OrdinalIgnoreCase);

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            AudioListener.volume = 1.0f;
            AudioListener.pause = false;

            EnsureAudioSources();
            EnsureAudioListener();

#if UNITY_EDITOR
            AutoPopulateClipsInEditor();
#endif

            BuildSoundDictionary();
            Debug.Log($"[U6_AudioManager] Initialized successfully with {soundDict.Count} audio entries registered in dictionary.");
        }

        private GameObject GetAudioHostGameObject()
        {
            if (Camera.main != null) return Camera.main.gameObject;
            Camera anyCam = FindFirstObjectByType<Camera>();
            if (anyCam == null) anyCam = FindObjectOfType<Camera>();
            if (anyCam != null) return anyCam.gameObject;
            return gameObject;
        }

        private void EnsureAudioSources()
        {
            GameObject host = GetAudioHostGameObject();
            if (!host.activeSelf) host.SetActive(true);

            bgmSource = ValidateOrAttachSource(bgmSource, host, 0.25f, true);
            ambSource = ValidateOrAttachSource(ambSource, host, 0.35f, true);
            sfxSource = ValidateOrAttachSource(sfxSource, host, 1.0f, false);
            voSource = ValidateOrAttachSource(voSource, host, 1.0f, false);
        }

        private AudioSource ValidateOrAttachSource(AudioSource source, GameObject host, float defaultVol, bool loop)
        {
            if (source == null || !source.gameObject.activeInHierarchy)
            {
                GameObject target = host != null ? host : GetAudioHostGameObject();
                source = target.AddComponent<AudioSource>();
            }

            source.enabled = true;
            source.playOnAwake = false;
            source.loop = loop;
            source.volume = defaultVol;
            source.mute = false;
            source.spatialBlend = 0f; // 100% 2D Sound - prevents 3D distance attenuation
            source.bypassEffects = false;
            source.bypassListenerEffects = false;
            return source;
        }

        private void EnsureAudioListener()
        {
            GameObject host = GetAudioHostGameObject();
            AudioListener listener = host.GetComponent<AudioListener>();
            if (listener == null)
            {
                listener = FindFirstObjectByType<AudioListener>();
                if (listener == null) listener = FindObjectOfType<AudioListener>();
            }

            if (listener == null)
            {
                listener = host.AddComponent<AudioListener>();
                Debug.Log($"[U6_AudioManager] Attached AudioListener to {host.name}.");
            }

            if (listener != null)
            {
                listener.enabled = true;
            }
        }

        private void BuildSoundDictionary()
        {
            soundDict.Clear();
            if (sounds != null)
            {
                foreach (var entry in sounds)
                {
                    if (!string.IsNullOrEmpty(entry.id) && entry.clip != null)
                    {
                        soundDict[entry.id] = entry;
                    }
                    if (entry.clip != null && !soundDict.ContainsKey(entry.clip.name))
                    {
                        soundDict[entry.clip.name] = entry;
                    }
                }
            }
        }

#if UNITY_EDITOR
        private void AutoPopulateClipsInEditor()
        {
            if (sounds == null) sounds = new List<U6_SoundEntry_Masters_Activity>();
            HashSet<string> registered = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);

            foreach (var s in sounds)
            {
                if (!string.IsNullOrEmpty(s.id) && s.clip != null)
                    registered.Add(s.id);
            }

            // 1. Genuine SFX & Ambience
            string sfxPath = "Assets/SeniorsActivityUnit6/SFX";
            if (System.IO.Directory.Exists(sfxPath))
            {
                string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { sfxPath });
                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                    if (clip == null) continue;

                    string id = clip.name;
                    if (!registered.Contains(id))
                    {
                        registered.Add(id);
                        sounds.Add(new U6_SoundEntry_Masters_Activity { id = id, clip = clip, volume = 1f });
                    }
                }
            }

            // 2. VO & Additional Audios
            string audioPath = "Assets/SeniorsActivityUnit6/Audio";
            if (System.IO.Directory.Exists(audioPath))
            {
                string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { audioPath });
                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                    if (clip == null) continue;

                    string rawName = clip.name;
                    if (!registered.Contains(rawName))
                    {
                        registered.Add(rawName);
                        sounds.Add(new U6_SoundEntry_Masters_Activity { id = rawName, clip = clip, volume = 1f });
                    }

                    string id = MapFileNameToSoundId(rawName);
                    if (!string.IsNullOrEmpty(id) && !registered.Contains(id))
                    {
                        registered.Add(id);
                        sounds.Add(new U6_SoundEntry_Masters_Activity { id = id, clip = clip, volume = 1f });
                    }
                }
            }

            Debug.Log($"[U6_AudioManager] Scanned project audio: total {sounds.Count} clips loaded.");
        }

        private static string ResolveAlias(string id)
        {
            if (string.IsNullOrEmpty(id)) return id;
            if (string.Equals(id, "MUS_Loop", System.StringComparison.OrdinalIgnoreCase)) return "MUS_Restaurant";
            if (string.Equals(id, "SFX_DoorBell", System.StringComparison.OrdinalIgnoreCase)) return "SFX_DoorChime";
            if (string.Equals(id, "SFX_SliderZone", System.StringComparison.OrdinalIgnoreCase)) return "SFX_Bubble";
            return id;
        }

        private AudioClip LoadClipOnDemand(string soundId)
        {
            string canonicalId = ResolveAlias(soundId);
            string[] searchFolders = new[] { "Assets/SeniorsActivityUnit6/SFX", "Assets/SeniorsActivityUnit6/Audio" };
            string[] guids = AssetDatabase.FindAssets("t:AudioClip", searchFolders);
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
                if (string.Equals(fileName, soundId, System.StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(fileName, canonicalId, System.StringComparison.OrdinalIgnoreCase))
                {
                    return AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                }
                string mappedId = MapFileNameToSoundId(fileName);
                if (!string.IsNullOrEmpty(mappedId) &&
                    (string.Equals(mappedId, soundId, System.StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(mappedId, canonicalId, System.StringComparison.OrdinalIgnoreCase)))
                {
                    return AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                }
            }
            return null;
        }

        private static string MapFileNameToSoundId(string fileName)
        {
            if (string.Equals(fileName, "MUS_Loop", System.StringComparison.OrdinalIgnoreCase)) return "MUS_Restaurant";
            if (string.Equals(fileName, "MUS_Restaurant", System.StringComparison.OrdinalIgnoreCase)) return "MUS_Restaurant";
            if (string.Equals(fileName, "SFX_DoorBell", System.StringComparison.OrdinalIgnoreCase)) return "SFX_DoorChime";
            if (string.Equals(fileName, "SFX_DoorChime", System.StringComparison.OrdinalIgnoreCase)) return "SFX_DoorChime";
            if (string.Equals(fileName, "SFX_SliderZone", System.StringComparison.OrdinalIgnoreCase)) return "SFX_Bubble";
            if (string.Equals(fileName, "SFX_Bubble", System.StringComparison.OrdinalIgnoreCase)) return "SFX_Bubble";

            if (fileName.StartsWith("SFX_") || fileName.StartsWith("AMB_") || fileName.StartsWith("MUS_") || fileName.StartsWith("VO_") || fileName.StartsWith("BGM_"))
                return fileName;

            string lower = fileName.ToLower();

            // Teacher / Narrator VO
            if (lower.Contains("today anus family")) return "VO_U6_01";
            if (lower.Contains("food is not here yet")) return "VO_U6_02";
            if (lower.Contains("quick tap the button")) return "VO_U6_03";
            if (lower.Contains("everybody is happy")) return "VO_U6_04";
            if (lower.Contains("now choose what will anu eat")) return "VO_U6_05";
            if (lower.Contains("read first")) return "VO_U6_06";
            if (lower.Contains("how should anu ask")) return "VO_U6_07";
            if (lower.Contains("this is ravi")) return "VO_U6_08";
            if (lower.Contains("wrong dish")) return "VO_U6_09";
            if (lower.Contains("how loud should anu talk")) return "VO_U6_10";
            if (lower.Contains("restaurant is full now")) return "VO_U6_11";
            if (lower.Contains("three stars thank you")) return "VO_U6_12";
            if (lower.Contains("what will you say to the waiter")) return "VO_U6_13";

            // Anu (Child) VO
            if (lower.Contains("could i have the dosa")) return "VO_U6_ANU_1";
            if (lower.Contains("i want dosa")) return "VO_U6_ANU_2";
            if (lower.Contains("ummm ummm")) return "VO_U6_ANU_3";
            if (lower.Contains("thank you") && !lower.Contains("do come again")) return "VO_U6_ANU_4";
            if (lower.Contains("sorry i think i ordered dosa")) return "VO_U6_ANU_5";
            if (lower.Contains("this is wrong")) return "VO_U6_ANU_6";
            if (lower.Contains("i am so hungry")) return "VO_U6_ANU_7";
            if (lower.Contains("excuse me could i have another fork")) return "VO_U6_ANU_8";
            if (lower.Contains("i dropped my fork")) return "VO_U6_ANU_9";
            if (lower.Contains("daddy guess what")) return "VO_U6_ANU_10";

            // Waiter Ravi VO
            if (lower.Contains("certainly")) return "VO_U6_WAIT_1";
            if (lower.Contains("of course one moment")) return "VO_U6_WAIT_2";
            if (lower.Contains("i am so sorry i will fix")) return "VO_U6_WAIT_3";
            if (lower.Contains("thank you do come again")) return "VO_U6_WAIT_4";

            // Parents VO
            if (lower.Contains("sorry i cannot hear you")) return "VO_U6_DAD_1";
            if (lower.Contains("anu quiet")) return "VO_U6_MUM_1";

            // SFX / Music mappings for descriptive audio files
            if (lower.Contains("laminated menu")) return "SFX_MenuOpen";
            if (lower.Contains("hot plate")) return "SFX_PlateDown";
            if (lower.Contains("metal fork falling")) return "SFX_ForkDrop";
            if (lower.Contains("metal spoon tapping")) return "SFX_GlassTing";
            if (lower.Contains("wooden chair tipping")) return "SFX_ChairWobble";
            if (lower.Contains("baby starting to cry")) return "SFX_BabyCry";
            if (lower.Contains("magical sparkle bloom")) return "SFX_Sparkle";
            if (lower.Contains("bright star chime")) return "SFX_Star";
            if (lower.Contains("party popper")) return "SFX_Confetti";
            if (lower.Contains("children clapping")) return "SFX_Clap";
            if (lower.Contains("pencil scribble")) return "SFX_PadWrite";
            if (lower.Contains("small soft bubble pop")) return "SFX_Bubble";
            if (lower.Contains("ukulele and marimba")) return "MUS_Restaurant";
            if (lower.Contains("busy but pleasant restaurant")) return "AMB_Restaurant";
            if (lower.Contains("restaurant going suddenly quieter")) return "AMB_RestaurantHush";

            return null;
        }
#endif

        public void PlaySFX(string soundId)
        {
            if (string.IsNullOrEmpty(soundId)) return;

            sfxSource = ValidateOrAttachSource(sfxSource, GetAudioHostGameObject(), 1.0f, false);

            if (!soundDict.TryGetValue(soundId, out U6_SoundEntry_Masters_Activity entry))
            {
#if UNITY_EDITOR
                AudioClip onDemand = LoadClipOnDemand(soundId);
                if (onDemand != null)
                {
                    entry = new U6_SoundEntry_Masters_Activity { id = soundId, clip = onDemand, volume = 1f };
                    soundDict[soundId] = entry;
                }
#endif
            }

            if (entry.clip != null && sfxSource != null)
            {
                float vol = entry.volume > 0 ? entry.volume * 1.0f : 1.0f;
                sfxSource.PlayOneShot(entry.clip, vol);
                Debug.Log($"[U6_AudioManager] >>> PLAYING SFX: {soundId} (Clip: '{entry.clip.name}') <<<");
            }
            else
            {
                Debug.LogWarning($"[U6_AudioManager] SFX '{soundId}' clip could not be located.");
            }
        }

        public void PlayVO(string voId)
        {
            if (string.IsNullOrEmpty(voId)) return;

            voSource = ValidateOrAttachSource(voSource, GetAudioHostGameObject(), 1.0f, false);

            if (!soundDict.TryGetValue(voId, out U6_SoundEntry_Masters_Activity entry))
            {
#if UNITY_EDITOR
                AudioClip onDemand = LoadClipOnDemand(voId);
                if (onDemand != null)
                {
                    entry = new U6_SoundEntry_Masters_Activity { id = voId, clip = onDemand, volume = 1f };
                    soundDict[voId] = entry;
                }
#endif
            }

            if (entry.clip != null && voSource != null)
            {
                voSource.Stop();
                voSource.clip = entry.clip;
                voSource.volume = entry.volume > 0 ? entry.volume : 1f;
                voSource.loop = false;
                voSource.Play();
                Debug.Log($"[U6_AudioManager] >>> PLAYING VO: {voId} (Clip: '{entry.clip.name}') <<<");
            }
            else
            {
                Debug.LogWarning($"[U6_AudioManager] VO '{voId}' clip could not be located.");
            }
        }

        public void StopVO()
        {
            if (voSource != null) voSource.Stop();
        }

        public void PlayAmbience(string ambId, bool loop = true)
        {
            if (string.IsNullOrEmpty(ambId)) return;

            ambSource = ValidateOrAttachSource(ambSource, GetAudioHostGameObject(), 0.35f, loop);

            if (!soundDict.TryGetValue(ambId, out U6_SoundEntry_Masters_Activity entry))
            {
#if UNITY_EDITOR
                AudioClip onDemand = LoadClipOnDemand(ambId);
                if (onDemand != null)
                {
                    entry = new U6_SoundEntry_Masters_Activity { id = ambId, clip = onDemand, volume = 1f };
                    soundDict[ambId] = entry;
                }
#endif
            }

            if (entry.clip != null && ambSource != null)
            {
                if (ambSource.clip == entry.clip && ambSource.isPlaying) return;

                ambSource.Stop();
                ambSource.clip = entry.clip;
                ambSource.loop = loop;
                ambSource.volume = entry.volume > 0 ? entry.volume * 0.35f : 0.35f;
                ambSource.Play();
                Debug.Log($"[U6_AudioManager] >>> PLAYING AMBIENCE: {ambId} (Clip: '{entry.clip.name}') <<<");
            }
            else
            {
                Debug.LogWarning($"[U6_AudioManager] Ambience '{ambId}' clip could not be located.");
            }
        }

        public void StopAmbience()
        {
            if (ambSource != null) ambSource.Stop();
        }

        public void PlayBGM(string bgmId)
        {
            if (string.IsNullOrEmpty(bgmId)) return;

            bgmSource = ValidateOrAttachSource(bgmSource, GetAudioHostGameObject(), 0.25f, true);

            if (!soundDict.TryGetValue(bgmId, out U6_SoundEntry_Masters_Activity entry))
            {
#if UNITY_EDITOR
                AudioClip onDemand = LoadClipOnDemand(bgmId);
                if (onDemand != null)
                {
                    entry = new U6_SoundEntry_Masters_Activity { id = bgmId, clip = onDemand, volume = 1f };
                    soundDict[bgmId] = entry;
                }
#endif
            }

            if (entry.clip != null && bgmSource != null)
            {
                if (bgmSource.clip == entry.clip && bgmSource.isPlaying) return;

                bgmSource.Stop();
                bgmSource.clip = entry.clip;
                bgmSource.loop = true;
                bgmSource.volume = entry.volume > 0 ? entry.volume * 0.25f : 0.25f;
                bgmSource.Play();
                Debug.Log($"[U6_AudioManager] >>> PLAYING BGM: {bgmId} (Clip: '{entry.clip.name}') <<<");
            }
        }

        public void StopBGM()
        {
            if (bgmSource != null) bgmSource.Stop();
        }
    }
}
