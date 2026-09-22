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
            ("Water", "Drink Water", "Who drank their water most days this week?", "Drink your water -- inside and outside."),
            ("Sleep", "Sleep Early", "Who went to bed early most days?", "Sleep early, wake up fresh and ready."),
            ("Outside", "Play Outside", "Who played outside this week?", "Play outside -- run under the open sky."),
            ("Read", "Read a Book", "Who read a book this week?", "Read a book -- every page holds a wonder."),
            ("Quiet", "Sit Quietly", "Who sat quietly for ten minutes?", "Sit quietly for ten minutes. Listen to the calm."),
            ("Walk", "Walk and Smile", "Who went for a walk?", "Walk with good posture, and greet others with a smile."),
            ("Give", "Give with Heart", "Who gave something to somebody?", "Give something to somebody with a happy heart."),
            ("Family", "Family Time", "Who spent time with their family?", "Spend time with your family every single day.")
        };

        public static readonly string[] GOLDEN_LINES_BY_WEEK = new[]
        {
            "Every day is a new chance.",
            "If today was not good, tomorrow can still be better.",
            "You do not have to be like anybody else.",
            "Say something kind about somebody today.",
            "Give something to somebody. It can be very small.",
            "Some days are not fair. It is still a good life.",
            "Spend some time with your family today.",
            "Nothing stays the same forever.",
            "If somebody is sad, sit with them.",
            "Do the thing you have been putting off.",
            "Be excited about something small today.",
            "Look at our garden. You grew all of that."
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
