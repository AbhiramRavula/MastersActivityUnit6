using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;
using System.Text;

public class EngSnap_VoiceGeneratorWindow : EditorWindow
{
    private string inputText = "";
    private int speedPercent = 0;
    private int pitchHz = 0;
    private string outputDirectory = "Assets/Audio";
    private bool isGenerating = false;

    // Async process tracking
    private Process _runningProcess;
    private StringBuilder _outputLog = new StringBuilder();
    private StringBuilder _errorLog = new StringBuilder();

    [MenuItem("EngSnap/Voice Generator")]
    public static void ShowWindow()
    {
        GetWindow<EngSnap_VoiceGeneratorWindow>("Voice Generator");
    }

    private int totalCount = 0;
    private int currentProgressCount = 0;
    private string currentItemStatus = "";

    private void OnGUI()
    {
        GUILayout.Label("Batch Voice Generator (edge-tts)", EditorStyles.boldLabel);

        GUILayout.Space(10);
        GUILayout.Label("Input Sentences (One per line):");
        inputText = EditorGUILayout.TextArea(inputText, GUILayout.Height(150));

        GUILayout.Space(10);
        speedPercent = EditorGUILayout.IntSlider("Speed (%)", speedPercent, -100, 100);
        pitchHz = EditorGUILayout.IntSlider("Pitch (Hz)", pitchHz, -100, 100);

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        outputDirectory = EditorGUILayout.TextField("Output Directory", outputDirectory);
        if (GUILayout.Button("Select Folder", GUILayout.Width(100)))
        {
            string path = EditorUtility.OpenFolderPanel("Select Output Directory", outputDirectory, "");
            if (!string.IsNullOrEmpty(path))
            {
                outputDirectory = path.StartsWith(Application.dataPath)
                    ? "Assets" + path.Substring(Application.dataPath.Length)
                    : path;
            }
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Set to Selected Folder in Project Window"))
        {
            if (Selection.activeObject != null)
            {
                string path = AssetDatabase.GetAssetPath(Selection.activeObject);
                outputDirectory = AssetDatabase.IsValidFolder(path)
                    ? path
                    : Path.GetDirectoryName(path);
            }
        }

        GUILayout.Space(20);

        if (isGenerating)
        {
            float progressVal = totalCount > 0 ? (float)currentProgressCount / totalCount : 0f;
            string progressLabel = totalCount > 0 
                ? $"Generating {currentProgressCount}/{totalCount} ({Mathf.RoundToInt(progressVal * 100)}%): {currentItemStatus}" 
                : "Starting generation...";

            EditorGUI.ProgressBar(GUILayoutUtility.GetRect(18, 24, "TextField"), progressVal, progressLabel);
            GUILayout.Space(8);

            if (GUILayout.Button("Cancel Generation", GUILayout.Height(30)))
            {
                CancelGeneration();
            }
        }
        else
        {
            EditorGUI.BeginDisabledGroup(string.IsNullOrWhiteSpace(inputText));
            if (GUILayout.Button("Generate Audio", GUILayout.Height(40)))
            {
                GenerateAudio();
            }
            EditorGUI.EndDisabledGroup();
        }
    }

    private void GenerateAudio()
    {
        isGenerating = true;
        _outputLog.Clear();
        _errorLog.Clear();

        string[] lines = inputText.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        totalCount = lines.Length;
        currentProgressCount = 0;
        currentItemStatus = "";

        string speedStr = (speedPercent >= 0 ? "+" : "") + speedPercent + "%";
        string pitchStr = (pitchHz >= 0 ? "+" : "") + pitchHz + "Hz";

        string absOutputDir = outputDirectory.StartsWith("Assets")
            ? Application.dataPath + outputDirectory.Substring(6)
            : outputDirectory;

        var sb = new StringBuilder();
        sb.Append("{");
        sb.AppendFormat("\"speed\": \"{0}\",", speedStr);
        sb.AppendFormat("\"pitch\": \"{0}\",", pitchStr);
        sb.AppendFormat("\"output_dir\": \"{0}\",", absOutputDir.Replace("\\", "/"));
        sb.Append("\"sentences\": [");
        for (int i = 0; i < lines.Length; i++)
        {
            sb.AppendFormat("\"{0}\"", lines[i].Replace("\\", "\\\\").Replace("\"", "\\\""));
            if (i < lines.Length - 1) sb.Append(",");
        }
        sb.Append("]}");

        string jsonPath = Path.Combine(Application.temporaryCachePath, "voice_gen_data.json");
        File.WriteAllText(jsonPath, sb.ToString());

        string pyScriptPath = Path.Combine(Application.dataPath, "Editor", "VoiceGen", "unity_voice_gen.py");

        var startInfo = new ProcessStartInfo
        {
            FileName = "py",
            Arguments = $"\"{pyScriptPath}\" \"{jsonPath}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        _runningProcess = new Process { StartInfo = startInfo, EnableRaisingEvents = true };

        _runningProcess.OutputDataReceived += (sender, e) => {
            if (e.Data != null)
            {
                _outputLog.AppendLine(e.Data);
                if (e.Data.StartsWith("PROGRESS:"))
                {
                    // Format: PROGRESS:curr/total:filename
                    string[] parts = e.Data.Split(':');
                    if (parts.Length >= 3)
                    {
                        string[] countParts = parts[1].Split('/');
                        if (countParts.Length == 2 && int.TryParse(countParts[0], out int curr))
                        {
                            currentProgressCount = curr;
                            currentItemStatus = parts[2];
                        }
                    }
                }
            }
        };

        _runningProcess.ErrorDataReceived += (sender, e) => {
            if (e.Data != null) _errorLog.AppendLine(e.Data);
        };
        _runningProcess.Exited += OnProcessExited;

        _runningProcess.Start();
        _runningProcess.BeginOutputReadLine();
        _runningProcess.BeginErrorReadLine();

        EditorApplication.update += PollProcess;

        UnityEngine.Debug.Log($"<color=cyan>Voice generation started for {lines.Length} items.</color>");
    }

    private void PollProcess()
    {
        if (_runningProcess == null)
        {
            EditorApplication.update -= PollProcess;
            return;
        }

        Repaint();
    }

    private void OnProcessExited(object sender, System.EventArgs e)
    {
        EditorApplication.delayCall += FinishGeneration;
    }

    private void FinishGeneration()
    {
        EditorApplication.update -= PollProcess;

        if (_runningProcess == null) return;

        int exitCode = _runningProcess.ExitCode;
        _runningProcess.Dispose();
        _runningProcess = null;
        isGenerating = false;

        if (exitCode != 0)
        {
            UnityEngine.Debug.LogError("Voice Generation Failed:\n" + _errorLog);
        }
        else
        {
            UnityEngine.Debug.Log("<color=green><b>Voice Generation Complete!</b></color>\n" + _outputLog);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        }

        Repaint();
    }

    private void CancelGeneration()
    {
        EditorApplication.update -= PollProcess;
        if (_runningProcess != null && !_runningProcess.HasExited)
        {
            _runningProcess.Kill();
            _runningProcess.Dispose();
            _runningProcess = null;
        }
        isGenerating = false;
        UnityEngine.Debug.LogWarning("Voice generation cancelled by user.");
        Repaint();
    }

    private void OnDestroy()
    {
        // Clean up if window is closed mid-generation
        CancelGeneration();
    }
}
