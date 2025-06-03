using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Varjo.XR;
using System.Linq;

public class EyeTracking : MonoBehaviour
{
    private Dictionary<string, float> gazeTimePerTag = new Dictionary<string, float>();
    private Dictionary<string, int> gazeCountPerTag = new Dictionary<string, int>();
    private string lastGazeTag = "";
    private float totalSessionTime = 0f;
    private string userName = "DefaultUser";

    [Header("Gaze Dot UI")]
    public RectTransform gazeDot; // assign in inspector
    private Camera mainCamera;

    void Start()
    {
        InitializeUserName();
        if (mainCamera == null)
    {
        mainCamera = Camera.main;
    }
    }

    void Update()
    {
        totalSessionTime += Time.deltaTime;
        TrackGaze();
    }

    void TrackGaze()
    {
        if (!VarjoEyeTracking.IsGazeAllowed() || !VarjoEyeTracking.IsGazeCalibrated())
        {
            if (gazeDot != null)
                gazeDot.gameObject.SetActive(false);
            return;
        }

        VarjoEyeTracking.GazeData gazeData = VarjoEyeTracking.GetGaze();
        if (gazeData.status != VarjoEyeTracking.GazeStatus.Valid)
        {
            if (gazeDot != null)
                gazeDot.gameObject.SetActive(false);
            return;
        }

        Transform hmdTransform = Camera.main.transform;
        Vector3 gazeOrigin = hmdTransform.TransformPoint(gazeData.gaze.origin);
        Vector3 gazeDirection = hmdTransform.TransformDirection(gazeData.gaze.forward);

        Debug.DrawRay(gazeOrigin, gazeDirection * 10f, Color.green, 1f);

        if (Physics.Raycast(gazeOrigin, gazeDirection, out RaycastHit hit))
        {
            string tag = hit.collider.tag;
            if (!string.IsNullOrEmpty(tag))
            {
                UpdateGazeTime(tag);
                TrackGazeTransition(tag);
                lastGazeTag = tag;
            }
            if (gazeDot != null)
            {
                Vector3 screenPos = mainCamera.WorldToScreenPoint(hit.point);
                if (screenPos.z > 0)
                {
                    gazeDot.gameObject.SetActive(true);
                    gazeDot.position = screenPos;
                }
                else
                {
                    gazeDot.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            if (gazeDot != null)
                gazeDot.gameObject.SetActive(false);
        }
    }

    void UpdateGazeTime(string tag)
    {
        if (!gazeTimePerTag.ContainsKey(tag)) gazeTimePerTag[tag] = 0f;
        gazeTimePerTag[tag] += Time.deltaTime;
    }

    void TrackGazeTransition(string tag)
    {
        if (tag != lastGazeTag)
        {
            if (!gazeCountPerTag.ContainsKey(tag)) gazeCountPerTag[tag] = 0;
            gazeCountPerTag[tag]++;
        }
    }

    public void ResetGazeTracking()
    {
        gazeTimePerTag.Clear();
        gazeCountPerTag.Clear();
        lastGazeTag = "";
        totalSessionTime = 0f;
        if (gazeDot != null)
            gazeDot.gameObject.SetActive(false);
    }

    void MergeCSVFiles()
    {
        string mergedFilePath = Path.Combine(Application.persistentDataPath, $"{userName}_Merged_EyeTrackingData.csv");
        string[] csvFiles = Directory.GetFiles(Application.persistentDataPath, $"{userName}_*_EyeTrackingData.csv");

        HashSet<string> uniqueLines = new HashSet<string>();
        using (StreamWriter writer = new StreamWriter(mergedFilePath, false))
        {
            foreach (string file in csvFiles)
            {
                using (StreamReader reader = new StreamReader(file))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("File:"))
                        {
                            uniqueLines.Add(line);
                        }
                    }
                }
            }

            foreach (string line in uniqueLines)
            {
                writer.WriteLine(line);
            }
        }

        Debug.Log($"Merged gaze data saved to {mergedFilePath}.");
    }

    // Modified to save gaze data and ensure all tags are included
    public void SaveMazeData(string mazeFileName)
    {
        SaveGazeDataToCSV(mazeFileName);
        ResetGazeTracking();
    }

    private void SaveGazeDataToCSV(string mazeFileName)
{
    string csvFilePath = Path.Combine(Application.persistentDataPath, $"{userName}_{mazeFileName}_EyeTrackingData.csv");

    // Collect all unique tags in the scene (including unused tags)
    HashSet<string> allTags = new HashSet<string>();
    foreach (GameObject obj in FindObjectsOfType<GameObject>())
    {
        if (!string.IsNullOrEmpty(obj.tag) && obj.tag != "Untagged")
        {
            allTags.Add(obj.tag);
        }
    }

    // Check if the file exists to avoid writing header again
    bool fileExists = File.Exists(csvFilePath);

    using (StreamWriter writer = new StreamWriter(csvFilePath, true)) // Append mode
    {
        // Write the header only if the file is being created for the first time
        if (!fileExists)
        {
            writer.WriteLine("FileName,Tag,TotalTime(s),Percentage(%),GazeCount");
        }

        // Write data for all possible tags, even if their values are 0
        foreach (var tag in allTags)
        {
            if (tag != "MainCamera" && tag != "Player") // Exclude MainCamera and Player tags
            {
                float totalTime = gazeTimePerTag.ContainsKey(tag) ? gazeTimePerTag[tag] : 0f;
                int gazeCount = gazeCountPerTag.ContainsKey(tag) ? gazeCountPerTag[tag] : 0;
                float percentage = (totalSessionTime > 0) ? (totalTime / totalSessionTime) * 100 : 0;

                // Add the maze file name to the data row
                writer.WriteLine($"{mazeFileName},{tag},{totalTime:F2},{percentage:F2},{gazeCount}");
            }
        }

        writer.WriteLine($"Total Session Time,{totalSessionTime:F2}");
    }
}

    void OnApplicationQuit()
    {
        MapLoader mapLoader = FindObjectOfType<MapLoader>();
        if (mapLoader != null && mapLoader.currentMazeIndex != -1)
        {
            string mazeFileName = mapLoader.wallFiles[mapLoader.currentMazeIndex].name;
            SaveMazeData(mazeFileName);
        }
        MergeCSVFiles();
    }

    void InitializeUserName()
    {
        if (EyeTrackingDataManager.Instance != null && !string.IsNullOrEmpty(EyeTrackingDataManager.Instance.UserName))
        {
            userName = EyeTrackingDataManager.Instance.UserName;
        }
    }
}
