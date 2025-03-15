using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    [SerializeField]
    private GameObject _wallPrefab;
    [SerializeField]
    private GameObject _CheeseTripWire;
    [SerializeField]
    private GameObject _WallTripWire;
    [SerializeField]
    private TextAsset wallDataFile;  // Load the map file here

    public class Wall
    {
        public Vector2 start;
        public Vector2 end;
        public Material texture;
    }

    [SerializeField]
    public Material rockTexture;
    [SerializeField]
    public Material cheeseTexture;
    [SerializeField]
    public Material CheeseWire;
    [SerializeField]
    public Material WallWire;

    public Dictionary<int, Wall> wallDictionary = new Dictionary<int, Wall>();

    void Awake()
    {
        LoadWallData(wallDataFile);  // Load walls from file
        Debug.Log("Loaded " + wallDictionary.Count + " walls into the dictionary.");
    }

    void LoadWallData(TextAsset mapData)
    {
        wallDictionary.Clear(); // Clear any existing walls

        using (StringReader reader = new StringReader(mapData.text))
        {
            string line;
            int wallCounter = 0;

            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line)) continue; // Skip comments or empty lines

                string[] values = line.Split('\t');

                // Debugging print to check the values before parsing
                Debug.Log($"Processing line: {line}");

                try
                {
                    // Check for wall format: x1, y1, x2, y2, texture
                    if (values.Length == 5)
                    {
                        float x1 = float.Parse(values[0]);
                        float y1 = float.Parse(values[1]);
                        float x2 = float.Parse(values[2]);
                        float y2 = float.Parse(values[3]);
                        string texture = values[4];

                        Material textureMaterial = GetMaterialByName(texture); // Get material

                        // Store wall data in dictionary
                        wallDictionary.Add(wallCounter, new Wall
                        {
                            start = new Vector2(x1, y1),
                            end = new Vector2(x2, y2),
                            texture = textureMaterial
                        });

                        wallCounter++;
                    }
                }
                catch (FormatException ex)
                {
                    Debug.LogError($"Failed to parse line: {line} Error: {ex.Message}");
                }
            }
        }
    }

    Material GetMaterialByName(string textureName)
    {
        switch (textureName.ToLower())
        {
            case "rock.bmp":
                return rockTexture;
            case "cheese.bmp":
                return cheeseTexture;
            case "cheesewire.bmp":
                return CheeseWire;
            case "wallwire.bmp":
                return WallWire;
            default:
                Debug.LogWarning("Unknown texture: " + textureName);
                return null;  // Or return a default material
        }
    }

    void Start()
    {
        for (int i = 0; i < wallDictionary.Count; i++)
        {
            Wall currentWall = wallDictionary[i];
            BuildWall(currentWall.start.x * 2, currentWall.start.y * 2, currentWall.end.x * 2, currentWall.end.y * 2, currentWall.texture);
        }
    }

    public void BuildWall(float x1, float y1, float x2, float y2, Material TexMaterial)
    {
        // 1. Calculate midpoint in 3D
        Vector3 startPos = new Vector3(x1, 0f, y1);
        Vector3 endPos = new Vector3(x2, 0f, y2);
        Vector3 midpoint = (startPos + endPos) / 2f;

        // 2. Calculate length and angle
        float length = Vector3.Distance(startPos, endPos);
        float angleRad = Mathf.Atan2((y2 - y1), (x2 - x1));
        float angleDeg = angleRad * Mathf.Rad2Deg;

        // 3. Instantiate the wall
        GameObject newWall = Instantiate(_wallPrefab, midpoint, Quaternion.Euler(0f, angleDeg, 0f));

        // 4. Adjust the wall・s length
        Vector3 scale = newWall.transform.localScale;
        scale.x = length;  // Adjust the wall's scale
        newWall.transform.localScale = scale;

        if (TexMaterial == cheeseTexture)
        {
            GameObject Finishline = Instantiate(_CheeseTripWire, midpoint, Quaternion.Euler(0f, angleDeg, 0f));
            Vector3 scale_cheese = Finishline.transform.localScale;
            scale_cheese.x = length;
            Finishline.transform.localScale = scale_cheese;
        }
        if (TexMaterial == WallWire)
        {
            GameObject Trigger = Instantiate(_WallTripWire, midpoint, Quaternion.Euler(0f, angleDeg, 0f));
            Vector3 scale_trip = Trigger.transform.localScale;
            scale_trip.x = length;
            Trigger.transform.localScale = scale_trip;
        }
        // Apply the material to the wall
        Renderer wallRenderer = newWall.GetComponentInChildren<MeshRenderer>();
        if (wallRenderer != null)
        {
            wallRenderer.material = TexMaterial;
            Debug.Log("Renderer is " + wallRenderer.material);
        }
        else
        {
            Debug.LogError("Renderer not found on Wall(Clone)!");
        }
    }

    void Update()
    {

    }
}