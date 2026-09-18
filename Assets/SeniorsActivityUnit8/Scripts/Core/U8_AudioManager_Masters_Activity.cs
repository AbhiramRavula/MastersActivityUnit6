using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Googolplex.Unit8
{
    [System.Serializable]
    public struct U8_SoundEntry_Masters_Activity
    {
        public string id;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume;
    }

    public class U8_AudioManager_Masters_Activity : MonoBehaviour
    {
        public static U8_AudioManager_Masters_Activity Instance { get; private set; }

        [Header("Audio Sources (Hosted on Main Camera)")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource ambSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource voSource;

        [Header("Audio Clips Library")]
        [SerializeField] private List<U8_SoundEntry_Masters_Activity> sounds = new List<U8_SoundEntry_Masters_Activity>();

        private Dictionary<string, U8_SoundEntry_Masters_Activity> soundDict = new Dictionary<string, U8_SoundEntry_Masters_Activity>(System.StringComparer.OrdinalIgnoreCase);

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

            EnsureAudioSourcesOnMainCamera();

#if UNITY_EDITOR
            AutoPopulateClipsInEditor();
#endif

            BuildSoundDictionary();
            Debug.Log($"[U8_AudioManager] Initialized successfully with {soundDict.Count} registered audio entries. Audio Sources hosted on Main Camera.");
        }

        public GameObject GetMainCameraHost()
        {
            if (Camera.main != null) return Camera.main.gameObject;
            Camera anyCam = FindFirstObjectByType<Camera>();
            if (anyCam == null) anyCam = FindObjectOfType<Camera>();
            if (anyCam != null) return anyCam.gameObject;
            return gameObject;
        }

        public void EnsureAudioSourcesOnMainCamera()
        {
            GameObject camHost = GetMainCameraHost();
            if (!camHost.activeSelf) camHost.SetActive(true);

            // Ensure AudioListener
            AudioListener listener = camHost.GetComponent<AudioListener>();
            if (listener == null) listener = camHost.AddComponent<AudioListener>();
            listener.enabled = true;

            // Clean up any extra listeners in scene
            AudioListener[] listeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
            foreach (var l in listeners)
            {
                if (l != listener) l.enabled = false;
            }

            bgmSource = ValidateOrAttachSource(bgmSource, camHost, "BGM_Source", 0.45f, true);
            ambSource = ValidateOrAttachSource(ambSource, camHost, "AMB_Source", 0.35f, true);
            sfxSource = ValidateOrAttachSource(sfxSource, camHost, "SFX_Source", 1.0f, false);
            voSource = ValidateOrAttachSource(voSource, camHost, "VO_Source", 1.0f, false);
        }

        private AudioSource ValidateOrAttachSource(AudioSource source, GameObject host, string sourceTag, float defaultVol, bool loop)
        {
            if (source == null || source.gameObject != host)
            {
                // Find existing source with tag or on host
                AudioSource[] sources = host.GetComponents<AudioSource>();
                if (sources != null && sources.Length > 0)
                {
                    // Match by loop setting or tag if possible
                    foreach (var s in sources)
                    {
                        if (s.loop == loop && (source == null || s != bgmSource && s != ambSource && s != sfxSource && s != voSource))
                        {
                            source = s;
                            break;
                        }
                    }
                }

                if (source == null)
                {
                    source = host.AddComponent<AudioSource>();
                }
            }

            source.enabled = true;
            source.playOnAwake = false;
            source.loop = loop;
            source.volume = defaultVol;
            source.mute = false;
            source.spatialBlend = 0f; // 2D Full stereo everywhere
            return source;
        }

        public void BuildSoundDictionary()
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
        public void AutoPopulateClipsInEditor()
        {
            if (sounds == null) sounds = new List<U8_SoundEntry_Masters_Activity>();
            HashSet<string> registered = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);

            foreach (var s in sounds)
            {
                if (!string.IsNullOrEmpty(s.id) && s.clip != null)
                    registered.Add(s.id);
            }

            string[] searchFolders = new[] { 
                "Assets/SeniorsActivityUnit8/SFX", 
                "Assets/SeniorsActivityUnit8/Audio/U8_MastersActivity_audios",
                "Assets/SeniorsActivityUnit8/Audio", 
                "Assets/SeniorsActivityUnit6/SFX" 
            };

            foreach (string folder in searchFolders)
            {
                if (!System.IO.Directory.Exists(folder)) continue;

                string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { folder });
                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                    if (clip == null) continue;

                    string rawName = clip.name;
                    if (!registered.Contains(rawName))
                    {
                        registered.Add(rawName);
                        sounds.Add(new U8_SoundEntry_Masters_Activity { id = rawName, clip = clip, volume = 1f });
                    }

                    string id = MapFileNameToSoundId(rawName);
                    if (!string.IsNullOrEmpty(id) && !registered.Contains(id))
                    {
                        registered.Add(id);
                        sounds.Add(new U8_SoundEntry_Masters_Activity { id = id, clip = clip, volume = 1f });
                    }
                }
            }

            BuildSoundDictionary();
            Debug.Log($"[U8_AudioManager] Auto-populated audio clips: total {sounds.Count} clips loaded.");
        }

        private AudioClip LoadClipOnDemand(string soundId)
        {
            List<string> validFolders = new List<string>();
            string[] searchFolders = new[] { 
                "Assets/SeniorsActivityUnit8/SFX", 
                "Assets/SeniorsActivityUnit8/Audio/U8_MastersActivity_audios",
                "Assets/SeniorsActivityUnit8/Audio", 
                "Assets/SeniorsActivityUnit6/SFX" 
            };
            foreach (var f in searchFolders)
            {
                if (System.IO.Directory.Exists(f)) validFolders.Add(f);
            }

            if (validFolders.Count == 0) return null;

            string[] guids = AssetDatabase.FindAssets("t:AudioClip", validFolders.ToArray());
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
                if (string.Equals(fileName, soundId, System.StringComparison.OrdinalIgnoreCase))
                {
                    return AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                }
                string mappedId = MapFileNameToSoundId(fileName);
                if (!string.IsNullOrEmpty(mappedId) && string.Equals(mappedId, soundId, System.StringComparison.OrdinalIgnoreCase))
                {
                    return AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                }
            }
            return null;
        }

        public static string MapFileNameToSoundId(string fileName)
        {
            if (fileName.StartsWith("SFX_") || fileName.StartsWith("AMB_") || fileName.StartsWith("MUS_") || fileName.StartsWith("VO_"))
                return fileName;

            string lower = fileName.ToLower().Trim();

            // Teacher / Narrator VO
            if (lower.Contains("anu needs the washroom")) return "VO_U8_01";
            if (lower.Contains("the door is closed what should") || lower.Contains("door is closed")) return "VO_U8_02";
            if (lower.Contains("knock and wait")) return "VO_U8_03";
            if (lower.Contains("now what next")) return "VO_U8_04";
            if (lower.Contains("do not forget to flush") || lower.Contains("forget to flush")) return "VO_U8_05";
            if (lower.Contains("now wash your hands") || lower.Contains("keep tapping")) return "VO_U8_06";
            if (lower.Contains("keep going")) return "VO_U8_07";
            if (lower.Contains("all clean one star") || lower.Contains("all clean")) return "VO_U8_08";
            if (lower.Contains("turn the tap off")) return "VO_U8_09";
            if (lower.Contains("towel in the bin")) return "VO_U8_10";
            if (lower.Contains("wipe the sink")) return "VO_U8_11";
            if (lower.Contains("here comes meera")) return "VO_U8_12";
            if (lower.Contains("shall we try again") || lower.Contains("oh dear")) return "VO_U8_13";
            if (lower.Contains("the soap is finished what should") || lower.Contains("soap is finished what")) return "VO_U8_14";
            if (lower.Contains("three stars ready for the next") || lower.Contains("ready for the next person")) return "VO_U8_15";
            if (lower.Contains("would the next person be happy")) return "VO_U8_16";

            // Character Voices
            if (lower.Contains("sorry")) return "VO_U8_ANU_1";
            if (lower.Contains("maam the soap is finished") || lower.Contains("ma'am the soap is finished")) return "VO_U8_MEE_1";
            if (lower.Contains("meera slip") || lower.Contains("meera startle") || lower.Contains("gasp")) return "VO_U8_MEE_2";
            if (lower.Contains("just a minute")) return "VO_U8_VOICE_1";

            // Music
            if (lower.Contains("handwash song") && lower.Contains("inst")) return "MUS_HandwashSong_Inst";
            if (lower.Contains("handwash song") || lower.Contains("handwashing")) return "MUS_HandwashSong";
            if (lower.Contains("win") || lower.Contains("celebration")) return "MUS_Win";
            if (lower.Contains("washroom") && lower.Contains("amb")) return "AMB_Washroom";

            // SFX
            if (lower.Contains("two polite knocks") || lower.Contains("knock")) return "SFX_Knock";
            if (lower.Contains("heavy bangs") || lower.Contains("bang")) return "SFX_BangDoor";
            if (lower.Contains("bolted door rattling") || lower.Contains("rattle")) return "SFX_BoltRattle";
            if (lower.Contains("bolt sliding shut") || lower.Contains("bolt click")) return "SFX_BoltClick";
            if (lower.Contains("toilet flushing") || lower.Contains("flush")) return "SFX_Flush";
            if (lower.Contains("tap turning on") || lower.Contains("tap on")) return "SFX_TapOn";
            if (lower.Contains("tap squeaking shut") || lower.Contains("tap off")) return "SFX_TapOff";
            if (lower.Contains("soap dispenser pumping once") || lower.Contains("soap pump")) return "SFX_SoapPump";
            if (lower.Contains("nothing in it") || lower.Contains("soap empty")) return "SFX_SoapEmpty";
            if (lower.Contains("wet hand-rubbing") || lower.Contains("scrub")) return "SFX_Scrub";
            if (lower.Contains("bubble pop") || lower.Contains("bubble")) return "SFX_Bubble";
            if (lower.Contains("germ") || lower.Contains("squeak")) return "SFX_GermOff";
            if (lower.Contains("paper towel pulled") || lower.Contains("towel pull")) return "SFX_TowelPull";
            if (lower.Contains("paper landing in") || lower.Contains("bin drop")) return "SFX_BinDrop";
            if (lower.Contains("quick wipe") || lower.Contains("wipe")) return "SFX_Wipe";
            if (lower.Contains("foot slipping") || lower.Contains("slip")) return "SFX_Slip";
            if (lower.Contains("sparkle")) return "SFX_Sparkle";
            if (lower.Contains("star chime") || lower.Contains("star")) return "SFX_Star";
            if (lower.Contains("clapping") || lower.Contains("clap")) return "SFX_Clap";
            if (lower.Contains("party popper") || lower.Contains("confetti")) return "SFX_Confetti";

            return null;
        }
#endif

        public void PlaySFX(string soundId)
        {
            if (string.IsNullOrEmpty(soundId)) return;

            EnsureAudioSourcesOnMainCamera();

            if (!soundDict.TryGetValue(soundId, out U8_SoundEntry_Masters_Activity entry))
            {
#if UNITY_EDITOR
                AudioClip onDemand = LoadClipOnDemand(soundId);
                if (onDemand != null)
                {
                    entry = new U8_SoundEntry_Masters_Activity { id = soundId, clip = onDemand, volume = 1f };
                    soundDict[soundId] = entry;
                }
#endif
            }

            if (entry.clip != null && sfxSource != null)
            {
                float vol = entry.volume > 0 ? entry.volume : 1.0f;
                sfxSource.PlayOneShot(entry.clip, vol);
                Debug.Log($"[U8_AudioManager] >>> PLAYING SFX on Main Camera: '{soundId}' (Clip: '{entry.clip.name}') <<<");
            }
            else
            {
                Debug.LogWarning($"[U8_AudioManager] SFX '{soundId}' clip could not be located.");
            }
        }

        public void PlayVO(string voId)
        {
            if (string.IsNullOrEmpty(voId)) return;

            EnsureAudioSourcesOnMainCamera();

            if (!soundDict.TryGetValue(voId, out U8_SoundEntry_Masters_Activity entry))
            {
#if UNITY_EDITOR
                AudioClip onDemand = LoadClipOnDemand(voId);
                if (onDemand != null)
                {
                    entry = new U8_SoundEntry_Masters_Activity { id = voId, clip = onDemand, volume = 1f };
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
                Debug.Log($"[U8_AudioManager] >>> PLAYING VO on Main Camera: '{voId}' (Clip: '{entry.clip.name}') <<<");
            }
            else
            {
                Debug.LogWarning($"[U8_AudioManager] VO '{voId}' clip could not be located.");
            }
        }

        public void StopVO()
        {
            if (voSource != null) voSource.Stop();
        }

        public void PlayBGM(string bgmId, bool loop = true)
        {
            if (string.IsNullOrEmpty(bgmId)) return;

            EnsureAudioSourcesOnMainCamera();

            if (!soundDict.TryGetValue(bgmId, out U8_SoundEntry_Masters_Activity entry))
            {
#if UNITY_EDITOR
                AudioClip onDemand = LoadClipOnDemand(bgmId);
                if (onDemand != null)
                {
                    entry = new U8_SoundEntry_Masters_Activity { id = bgmId, clip = onDemand, volume = 0.45f };
                    soundDict[bgmId] = entry;
                }
#endif
            }

            if (entry.clip != null && bgmSource != null)
            {
                if (bgmSource.clip == entry.clip && bgmSource.isPlaying) return;

                bgmSource.Stop();
                bgmSource.clip = entry.clip;
                bgmSource.loop = loop;
                bgmSource.volume = entry.volume > 0 ? entry.volume : 0.45f;
                bgmSource.Play();
                Debug.Log($"[U8_AudioManager] >>> PLAYING BGM on Main Camera: '{bgmId}' (Clip: '{entry.clip.name}') <<<");
            }
        }

        public void StopBGM()
        {
            if (bgmSource != null) bgmSource.Stop();
        }

        public void PlayAmbience(string ambId, bool loop = true)
        {
            if (string.IsNullOrEmpty(ambId)) return;

            EnsureAudioSourcesOnMainCamera();

            if (!soundDict.TryGetValue(ambId, out U8_SoundEntry_Masters_Activity entry))
            {
#if UNITY_EDITOR
                AudioClip onDemand = LoadClipOnDemand(ambId);
                if (onDemand != null)
                {
                    entry = new U8_SoundEntry_Masters_Activity { id = ambId, clip = onDemand, volume = 0.35f };
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
                ambSource.volume = entry.volume > 0 ? entry.volume : 0.35f;
                ambSource.Play();
                Debug.Log($"[U8_AudioManager] >>> PLAYING AMBIENCE on Main Camera: '{ambId}' (Clip: '{entry.clip.name}') <<<");
            }
        }

        public void StopAmbience()
        {
            if (ambSource != null) ambSource.Stop();
        }
    }
}
