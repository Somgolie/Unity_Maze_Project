using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    /*    [SerializeField]
        private TextAsset wallDataFile;  // Load the map file here*/
    [SerializeField]
    public List<TextAsset> wallFiles = new List<TextAsset>();  // Load multiple map files here

    [SerializeField]
    private GameObject _wallPrefab;
    [SerializeField]
    private GameObject _CheeseTripWire;
    [SerializeField]
    private GameObject _WallTripWire;
    [SerializeField]
    public ObjectSpawner spawner;

    [SerializeField]
    public TextAsset PracticeMaze;
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
    [SerializeField]
    public GameObject[] modelPrefabs; // Array to hold references to the prefabs
    public int currentMazeIndex = -1;  // Start with no maze selected initially  //<-

    List<int> availableMazes = new List<int>();
    List<int> playedMazes = new List<int>();
    private bool readnext;

    public Dictionary<int, Wall> wallDictionary = new Dictionary<int, Wall>();
    public Dictionary<int, Wall> ObjDictionary = new Dictionary<int, Wall>();
    private int objCount;


    void LoadObjectData(TextAsset mapData)
    {
        Debug.Log("Load more objects  " + (modelPrefabs.Length));
        float[] objArray = null;

        using (StringReader reader = new StringReader(mapData.text))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("#Peripheral"))
                {

                    readnext = true;
                    objCount = 0;


                    // Initialize objArray here to store 4 float values for the peripheral data
                    objArray = new float[4];
                    continue;  // Skip to the next line after initializing objArray
                }

                string[] values = line.Split(new char[] { '\t', ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    foreach (string v in values)
                    {
                        if (readnext == true)
                        {
                            if (objArray == null)
                            {

                                objArray = new float[4];
                            }
                            if (objCount < 4)
                            {
                                objArray[objCount] = float.Parse(v);  // Parse string value into float and store in objArray

                                objCount++;
                            }
                            else
                            {
                                if (objCount < 5)
                                {
                                    SpawnPeripheralObject(objArray[0], objArray[1], objArray[2], objArray[3], v);
                                    objCount++;
                                }
                                readnext = false;  // Stop reading after 4 values
                            }
                        }
                    }
                }
                catch (FormatException ex)
                {
                    Debug.LogError($"Failed to parse line: {line} Error: {ex.Message}");
                }
            }
        }
    }

    void SpawnPeripheralObject(float x, float y, float scale, float rotationAngle, string objFile)
    {


        for (int i = 0; i < modelPrefabs.Length; i++)
        {
            String target = modelPrefabs[i].name;

            if ((target + ".obj") == objFile)
            {

                Vector3 pos = new Vector3(x, 0f, y);

                GameObject ObjectPrefab = Instantiate(modelPrefabs[i], pos, Quaternion.identity);
                ObjectPrefab.transform.localScale = new Vector3(scale, scale, scale);
                ObjectPrefab.tag = "Object";
                return;
            }
        }


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

                string[] values = line.Split(new char[] { '\t', ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

                // Debugging print to check the values before parsing
                //Debug.Log($"Processing line: {line}");

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
                    // Check for wall format: x1, y1, x2, y2
                    if (values.Length == 4)
                    {
                        float x1 = float.Parse(values[0]);
                        float y1 = float.Parse(values[1]);
                        float x2 = float.Parse(values[2]);
                        float y2 = float.Parse(values[3]);




                        // Store wall data in dictionary
                        wallDictionary.Add(wallCounter, new Wall
                        {
                            start = new Vector2(x1, y1),
                            end = new Vector2(x2, y2),
                            texture = WallWire
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

            default:
                Debug.LogWarning("Unknown texture: " + textureName);
                return null;  // Or return a default material
        }
    }
    public void Start()
    {


    }
    public void LoadPracticeMaze()
    {
        DestroyExistingWalls();
        wallDictionary.Clear();
        TextAsset wallDataFile = PracticeMaze;
        LoadWallData(wallDataFile);
        LoadObjectData(wallDataFile);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = new Vector3(0.7f, 0.7f, 0.7f);
        }
        else
        {
            Debug.LogError("Player not found!");
        }

        // Build the walls for the maze

        for (int i = 0; i < wallDictionary.Count; i++)
        {
            Wall currentWall = wallDictionary[i];

            BuildWall(currentWall.start.x * 2, currentWall.start.y * 2, currentWall.end.x * 2, currentWall.end.y * 2, currentWall.texture);


        }
    }
    public void LoadNewMaze()
    {

        // Save the previous maze's data before loading a new one
        if (currentMazeIndex != -1 && currentMazeIndex < wallFiles.Count)
        {
            string previousMazeFileName = wallFiles[currentMazeIndex].name;
            EyeTracking eyeTracking = FindObjectOfType<EyeTracking>();
            if (eyeTracking != null)
            {
                eyeTracking.SaveMazeData(previousMazeFileName);
            }
        }

        DestroyExistingWalls();
        wallDictionary.Clear();

        if (wallFiles.Count == 0)
        {
            Debug.LogError("No maze files available.");
            return;
        }

        int previousMazeIndex = currentMazeIndex; // Store the current maze index before getting the new one
        int nextMazeIndex = GetRandomUnplayedMazeIndex();
        if (nextMazeIndex != -1)
        {
            // Load the wall data for the selected maze
            TextAsset wallDataFile = wallFiles[nextMazeIndex];
            LoadWallData(wallDataFile);
            LoadObjectData(wallDataFile);



            // Place player at the origin (or desired spawn point)
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = new Vector3(0.7f, 0.7f, 0.7f);
            }
            else
            {
                Debug.LogError("Player not found!");
            }

            // Build the walls for the maze

            for (int i = 0; i < wallDictionary.Count; i++)
            {
                Wall currentWall = wallDictionary[i];

                BuildWall(currentWall.start.x * 2, currentWall.start.y * 2, currentWall.end.x * 2, currentWall.end.y * 2, currentWall.texture);


            }
        }
    }
    int GetRandomUnplayedMazeIndex()
    {
        List<int> availableMazes = new List<int>();

        for (int i = 0; i < wallFiles.Count; i++)
        {
            if (!playedMazes.Contains(i))
            {
                availableMazes.Add(i);
            }
        }

        if (availableMazes.Count == 0)
        {
            // All mazes have been played
            Debug.Log("ALL MAZES COMPLETE!");
            return -1;
        }

        // Select a random index from the available ones
        int randomIndex = UnityEngine.Random.Range(0, availableMazes.Count);
        currentMazeIndex = availableMazes[randomIndex];

        // Mark the current maze as played
        playedMazes.Add(currentMazeIndex);
        int A = availableMazes.Count - 1;

        return currentMazeIndex;
    }

    /*    void LoadCurrentMaze()
        {
            if (currentMazeIndex >= wallFiles.Count)
            {
                Debug.Log("All mazes have been completed!");
                return;
            }

            wallDictionary.Clear(); // Clear previous maze walls
            //DestroyExistingWalls(); // Remove old walls from the scene

            TextAsset currentMazeFile = wallFiles[currentMazeIndex];
            LoadWallData(currentMazeFile);
        }*/

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
        if (TexMaterial != WallWire)
        {
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
            // Apply the material to the wall
            Renderer wallRenderer = newWall.GetComponentInChildren<MeshRenderer>();
            if (wallRenderer != null)
            {
                wallRenderer.material = TexMaterial;

            }
            else
            {
                //Debug.LogError("Renderer not found on Wall(Clone)!");
            }
        }



        if (TexMaterial == WallWire)
        {
            GameObject Trigger = Instantiate(_WallTripWire, midpoint, Quaternion.Euler(0f, angleDeg, 0f));
            Vector3 scale_trip = Trigger.transform.localScale;
            scale_trip.x = length;
            Trigger.transform.localScale = scale_trip;
        }

    }
    public void buildObjects(float x1, float y1, float x2, float y2, Material TexMaterial)
    {

    }
    void DestroyExistingWalls()
    {
        // Delete all GameObjects with the tags "Wall", "Cheese", or "WallTripWire"
        string[] tagsToDelete = new string[] { "Wall", "Cheese", "WallTripWire", "Object" };

        foreach (string tag in tagsToDelete)
        {
            GameObject[] objectsToDestroy = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in objectsToDestroy)
            {
                Destroy(obj); // Destroy each object found with the tag
            }
        }

    }

    /*  public void LoadNextMaze()
      {

          // Debugging current maze index before incrementing
          Debug.Log("Current Maze Index: " + currentMazeIndex);

          // Check if we have more mazes left to load
          if (currentMazeIndex + 1 < wallFiles.Count)
          {
              currentMazeIndex++; // Move to the next maze
          }
          else
          {
              // If no more mazes, loop back to the first maze
              currentMazeIndex = 0;  // Reset to first maze
              Debug.Log("Looping back to the first maze.");
          }

          // Now clear existing walls and load the new maze
          DestroyExistingWalls();
          wallDictionary.Clear();

          // Load the new maze's data and build the maze
          LoadNewMaze();

      }*/

    void Update()
    {

    }
}