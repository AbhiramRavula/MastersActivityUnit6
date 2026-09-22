using System;
using System.Collections.Generic;
using UnityEngine;

namespace Googolplex.Unit10
{
    [Serializable]
    public class U10_HabitData
    {
        public string habitKey;        // "Water", "Sleep", "Outside", "Read", "Quiet", "Walk", "Give", "Family"
        public string habitTitle;      // e.g. "Drink Water"
        public string habitPrompt;     // e.g. "Who drank fresh water every day this week?"
        public string habitBookQuote;  // e.g. "Drink your water -- inside and outside."
        public int growthStage;        // 1 to 6
        public int totalCheckins;      // Number of weeks completed

        public U10_HabitData() { }

        public U10_HabitData(string key, string title, string prompt, string quote)
        {
            habitKey = key;
            habitTitle = title;
            habitPrompt = prompt;
            habitBookQuote = quote;
            growthStage = 1;
            totalCheckins = 0;
        }
    }

    [Serializable]
    public class U10_ClassGardenSaveData
    {
        public string className = "Class 3 and 4";
        public int currentWeek = 1;          // 1 to 12
        public int totalMarbles = 0;         // Kindness Jar count
        public bool isSetupCompleted = false;
        public List<U10_HabitData> chosenHabits = new List<U10_HabitData>(); // Exactly 4 habits
    }

    public static class U10_SaveSystem
    {
        private const string SAVE_KEY = "U10_GoldenGarden_SaveData_v1";

        public static readonly (string key, string title, string prompt, string quote)[] ALL_AVAILABLE_HABITS = new[]
        {
            ("Water", "Drink Water", "Who drank fresh water every day this week?", "Drink your water -- inside and outside."),
            ("Sleep", "Sleep Early", "Who went to sleep on time and rested well?", "Sleep early, wake up fresh and ready."),
            ("Outside", "Play Outside", "Who ran and played in the fresh open air?", "Play outside -- run under the open sky."),
            ("Read", "Read a Book", "Who enjoyed reading a story or book?", "Read a book -- every page holds a wonder."),
            ("Quiet", "Sit Quietly", "Who took ten quiet minutes to sit and relax?", "Sit quietly for ten minutes. Listen to the calm."),
            ("Walk", "Walk and Smile", "Who walked politely and shared warm smiles?", "Walk with good posture, and greet others with a smile."),
            ("Give", "Give with Heart", "Who shared or gave something to someone?", "Give something to somebody with a happy heart."),
            ("Family", "Family Time", "Who spent loving quality time with family?", "Spend time with your family every single day.")
        };

        public static readonly string[] GOLDEN_LINES_BY_WEEK = new[]
        {
            "The Golden Life begins with the simple things you do every day.",
            "Drink your water -- inside and outside.",
            "Sleep early, rise strong and joyful.",
            "Play outside -- the world is wide and green.",
            "A good book is a doorway to a thousand adventures.",
            "Sit quietly for ten minutes. Listen to the calm world.",
            "Walk with confidence, and greet others with a smile.",
            "Giving from the heart makes both people glow.",
            "Family time is the warmest sunshine of life.",
            "Every small good habit grows into a giant strong tree.",
            "Every act of kindness is a shining marble in your jar.",
            "The Three Es of Golden Life: Energy, Empathy, Excellence!"
        };

        public static void Save(U10_ClassGardenSaveData data)
        {
            try
            {
                string json = JsonUtility.ToJson(data, true);
                PlayerPrefs.SetString(SAVE_KEY, json);
                PlayerPrefs.Save();
                Debug.Log("[U10_SaveSystem] Saved class garden data successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[U10_SaveSystem] Error saving data: {ex.Message}");
            }
        }

        public static U10_ClassGardenSaveData Load()
        {
            if (PlayerPrefs.HasKey(SAVE_KEY))
            {
                try
                {
                    string json = PlayerPrefs.GetString(SAVE_KEY);
                    var data = JsonUtility.FromJson<U10_ClassGardenSaveData>(json);
                    if (data != null && data.chosenHabits != null && data.chosenHabits.Count == 4)
                    {
                        return data;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[U10_SaveSystem] Failed to load existing save: {ex.Message}");
                }
            }

            return CreateDefaultData();
        }

        public static U10_ClassGardenSaveData CreateDefaultData()
        {
            var data = new U10_ClassGardenSaveData
            {
                className = "Class 3 and 4",
                currentWeek = 1,
                totalMarbles = 0,
                isSetupCompleted = false,
                chosenHabits = new List<U10_HabitData>()
            };
            return data;
        }

        public static void ClearSave()
        {
            PlayerPrefs.DeleteKey(SAVE_KEY);
            PlayerPrefs.Save();
            Debug.Log("[U10_SaveSystem] Save data cleared.");
        }
    }
}
