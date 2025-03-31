using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;


public class MapLoader : MonoBehaviour
{
    /*    [SerializeField]
        private TextAsset wallDataFile;  // Load the map file here*/
    [SerializeField]
    private List<TextAsset> wallFiles = new List<TextAsset>();  // Load multiple map files here

    [SerializeField]
    private GameObject _wallPrefab;
    [SerializeField]
    private GameObject _CheeseTripWire;
    [SerializeField]
    private GameObject _WallTripWire;
    [SerializeField]
    public ObjectSpawner spawner;

    [SerializeField] private Material rockTexture;
    [SerializeField] private Material cheeseTexture;
    [SerializeField] private Material CheeseWire;
    [SerializeField] private Material WallWire;

    private HashSet<int> playedMazes = new HashSet<int>();  // To keep track of played mazes
    private Dictionary<int, Wall> wallDictionary = new Dictionary<int, Wall>();
    private int currentMazeIndex = -1;  // To track the current maze index

    // Wall class to hold wall data
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
    private int currentMazeIndex = -1;  // Start with no maze selected initially
    private HashSet<int> playedMazes = new HashSet<int>();  // To keep track of mazes that have been played
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
                    Debug.Log("Peripheral:");
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
                                Debug.LogWarning("objArray was null. Initializing now.");
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
        Debug.Log(objFile);
        Debug.Log(modelPrefabs.Length);

        for (int i=0;i<modelPrefabs.Length;i++)
        {
            String target = modelPrefabs[i].name;

            if ( (target+".obj") == objFile)
            {
                Debug.Log("FOUND");
                Vector3 pos= new Vector3(x, 0f, y);

                GameObject ObjectPrefab=Instantiate(modelPrefabs[i], pos, Quaternion.identity);
                ObjectPrefab.transform.localScale = new Vector3(scale, scale, scale);
                ObjectPrefab.tag = "Object";
                return;
            }
        }
        Debug.Log("NOT FOUND");

    }

    void LoadWallData(TextAsset mapData)
    {
        LoadNewMaze();  // Load the first maze at the start
    }

    // Load wall data from the selected maze map file
    private void LoadWallData(TextAsset mapData)
    {
        wallDictionary.Clear();  // Clear any existing walls

        using (StringReader reader = new StringReader(mapData.text))
        {
            string line;
            int wallCounter = 0;

            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line)) continue;  // Skip comments or empty lines

                string[] values = line.Split('\t');
                
                try
                {
                    // Ensure the format is correct: x1, y1, x2, y2, texture
                    if (values.Length == 5)
                    {
                        float x1 = float.Parse(values[0]);
                        float y1 = float.Parse(values[1]);
                        float x2 = float.Parse(values[2]);
                        float y2 = float.Parse(values[3]);
                        string textureName = values[4];

                        Material textureMaterial = GetMaterialByName(textureName);  // Get material based on the texture name

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

    // Helper function to map texture names to materials
    private Material GetMaterialByName(string textureName)
    {
        switch (textureName.ToLower())
        {
            case "rock.bmp": return rockTexture;
            case "cheese.bmp": return cheeseTexture;
            case "cheesewire.bmp": return CheeseWire;
            case "wallwire.bmp": return WallWire;
            default:
                Debug.LogWarning("Unknown texture: " + textureName);
                return null;
        }
    }
    public void Start()
    {
        LoadNewMaze();
    }
    public void LoadNewMaze()
    {
        DestroyExistingWalls();  // Destroy any existing walls in the scene
        wallDictionary.Clear();

        if (wallFiles.Count == 0)
        {
            Debug.LogError("No maze files available.");
            return;
        }

        int nextMazeIndex = GetRandomUnplayedMazeIndex();  // Get a random unplayed maze index
        TextAsset wallDataFile = wallFiles[nextMazeIndex];
        LoadWallData(wallDataFile);
        LoadObjectData(wallDataFile);

        Debug.Log($"Loaded {wallDictionary.Count} walls into the dictionary.");

        // Place player at the spawn point (or origin if no spawn is set)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = new Vector3(1f, 0.7f, 2f);
        }
        else
        {
            Debug.LogError("Player not found!");
        }

        // Build the walls in the scene
        Debug.Log($"Building maze number: {nextMazeIndex}");
        foreach (var wall in wallDictionary.Values)
        {
            BuildWall(wall.start.x * 2, wall.start.y * 2, wall.end.x * 2, wall.end.y * 2, wall.texture);
        }
    }

    // Selects a random unplayed maze index
    private int GetRandomUnplayedMazeIndex()
    {
        List<int> availableMazes = new List<int>();

        for (int i = 0; i < wallFiles.Count; i++)
        {
            if (!playedMazes.Contains(i)) availableMazes.Add(i);
        }

        if (availableMazes.Count == 0)
        {
            // If all mazes have been played, reset
            playedMazes.Clear();
            availableMazes.AddRange(Enumerable.Range(0, wallFiles.Count));
        }

        // Randomly select a maze from the available ones
        int randomIndex = UnityEngine.Random.Range(0, availableMazes.Count);
        currentMazeIndex = availableMazes[randomIndex];

        playedMazes.Add(currentMazeIndex);  // Mark the selected maze as played
        return currentMazeIndex;
    }

    // Build a wall based on the start and end points
    public void BuildWall(float x1, float y1, float x2, float y2, Material texture)
    {

        // 1. Calculate midpoint in 3D
        Vector3 startPos = new Vector3(x1, 0f, y1);
        Vector3 endPos = new Vector3(x2, 0f, y2);
        Vector3 midpoint = (startPos + endPos) / 2f;

        float length = Vector3.Distance(startPos, endPos);
        float angleRad = Mathf.Atan2(y2 - y1, x2 - x1);
        float angleDeg = angleRad * Mathf.Rad2Deg;

        GameObject newWall = Instantiate(_wallPrefab, midpoint, Quaternion.Euler(0f, angleDeg, 0f));

        // Scale the wall based on length
        Vector3 scale = newWall.transform.localScale;
        scale.x = length;
        newWall.transform.localScale = scale;

        if (texture == cheeseTexture)
        {
            GameObject finishline = Instantiate(_CheeseTripWire, midpoint, Quaternion.Euler(0f, angleDeg, 0f));
            Vector3 scaleCheese = finishline.transform.localScale;
            scaleCheese.x = length;
            finishline.transform.localScale = scaleCheese;
        }

        if (texture == WallWire)
        {
            GameObject trigger = Instantiate(_WallTripWire, midpoint, Quaternion.Euler(0f, angleDeg, 0f));
            Vector3 scaleTrigger = trigger.transform.localScale;
            scaleTrigger.x = length;
            trigger.transform.localScale = scaleTrigger;
        }

        // Apply texture to the wall
        Renderer wallRenderer = newWall.GetComponentInChildren<MeshRenderer>();
        if (wallRenderer != null)
        {
            wallRenderer.material = texture;
        }
    }
    public void buildObjects(float x1, float y1, float x2, float y2, Material TexMaterial)
    {

    }
    void DestroyExistingWalls()
    {
        // Delete all GameObjects with the tags "Wall", "Cheese", or "WallTripWire"
        string[] tagsToDelete = new string[] { "Wall", "Cheese", "WallTripWire" ,"Object"};

        foreach (string tag in tagsToDelete)
        {
            GameObject[] objectsToDestroy = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in objectsToDestroy)
            {
                Destroy(obj);  // Destroy each object found with the tag
            }
        }
    }

    // Load the next maze or loop back to the first maze if all have been completed
    public void LoadNextMaze()
    {

        // Debugging current maze index before incrementing
        Debug.Log("Current Maze Index: " + currentMazeIndex);
        Debug.Log($"Current Maze Index: {currentMazeIndex}");

        if (currentMazeIndex + 1 < wallFiles.Count)
        {
            currentMazeIndex++;  // Move to the next maze
        }
        else
        {
            currentMazeIndex = 0;  // Reset to the first maze
            Debug.Log("Looping back to the first maze.");
        }

        DestroyExistingWalls();
        wallDictionary.Clear();
        LoadNewMaze();

    }

    void Update() { }
}
