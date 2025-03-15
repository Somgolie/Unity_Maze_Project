using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WallGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject _wallPrefab;
    [SerializeField]
    private GameObject _CheeseTripWire;
    [SerializeField]
    private GameObject _WallTripWire;

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
        // Manually add each wall to the dictionary.

        // Wall 0: 0.5,0.0 -> 6.0,0.0 Rock.bmp
        wallDictionary.Add(0, new Wall
        {
            start = new Vector2(0.5f, 0.0f),
            end = new Vector2(6.0f, 0.0f),
            texture = rockTexture

        }) ;

        // Wall 1: 6.0,0.0 -> 6.0,6.0 Rock.bmp
        wallDictionary.Add(1, new Wall
        {
            start = new Vector2(6.0f, 0.0f),
            end = new Vector2(6.0f, 6.0f),
            texture = rockTexture

        });

        // Wall 2: 6.0,6.0 -> 0.5,6.0 Rock.bmp
        wallDictionary.Add(2, new Wall
        {
            start = new Vector2(6.0f, 6.0f),
            end = new Vector2(0.5f, 6.0f),
            texture = rockTexture

        });

        // Wall 3: 0.0,5.5 -> 0.0,0.5 Rock.bmp
        wallDictionary.Add(3, new Wall
        {
            start = new Vector2(0.0f, 5.5f),
            end = new Vector2(0.0f, 0.5f),
            texture = rockTexture

        });


        // Wall 4: 0.0,-0.5 -> 0.5,0.0 Rock.bmp
        wallDictionary.Add(4, new Wall
        {
            start = new Vector2(0.0f, 0.0f),
            end = new Vector2(0.5f, 0.5f),
            texture = rockTexture
        });

        // Wall 5: 0.0,5.5 -> -0.5,6.0 Cheese.bmp
        wallDictionary.Add(5, new Wall
        {
            start = new Vector2(0.5f, 5.5f),
            end = new Vector2(0.0f, 6.0f),
            texture = cheeseTexture
        });


        // Wall 6: 6.0,5.0 -> 4.0,5.0 Rock.bmp
        wallDictionary.Add(6, new Wall
        {
            start = new Vector2(6.0f, 5.0f),
            end = new Vector2(4.0f, 5.0f),
            texture = rockTexture
        });

        // Wall 7: 6.0,3.0 -> 4.0,3.0 Rock.bmp
        wallDictionary.Add(7, new Wall
        {
            start = new Vector2(6.0f, 3.0f),
            end = new Vector2(4.0f, 3.0f),
            texture = rockTexture
        });

        // Wall 8: 0.0,4.0 -> 3.0,4.0 Rock.bmp
        wallDictionary.Add(8, new Wall
        {
            start = new Vector2(0.0f, 4.0f),
            end = new Vector2(3.0f, 4.0f),
            texture = rockTexture
        });

        // Wall 9: 3.0,4.0 -> 3.0,2.0 Rock.bmp
        wallDictionary.Add(9, new Wall
        {
            start = new Vector2(3.0f, 4.0f),
            end = new Vector2(3.0f, 2.0f),
            texture = rockTexture
        });

        // Wall 10: 2.0,6.0 -> 2.0,5.0 Rock.bmp
        wallDictionary.Add(10, new Wall
        {
            start = new Vector2(2.0f, 6.0f),
            end = new Vector2(2.0f, 5.0f),
            texture = rockTexture
        });

        // Wall 11: 1.0,2.0 -> 1.0,3.0 Rock.bmp
        wallDictionary.Add(11, new Wall
        {
            start = new Vector2(1.0f, 2.0f),
            end = new Vector2(1.0f, 3.0f),
            texture = rockTexture
        });

        // Wall 12: 2.0,1.0 -> 2.0,3.0 Rock.bmp
        wallDictionary.Add(12, new Wall
        {
            start = new Vector2(2.0f, 1.0f),
            end = new Vector2(2.0f, 3.0f),
            texture = rockTexture
        });

        // Wall 13: 3.0,5.0 -> 4.0,5.0 Rock.bmp
        wallDictionary.Add(13, new Wall
        {
            start = new Vector2(3.0f, 5.0f),
            end = new Vector2(4.0f, 5.0f),
            texture = rockTexture
        });

        // Wall 14: 4.0,3.0 -> 4.0,2.0 Rock.bmp
        wallDictionary.Add(14, new Wall
        {
            start = new Vector2(4.0f, 3.0f),
            end = new Vector2(4.0f, 2.0f),
            texture = rockTexture
        });

        // Wall 15: 4.0,2.0 -> 5.0,2.0 Rock.bmp
        wallDictionary.Add(15, new Wall
        {
            start = new Vector2(4.0f, 2.0f),
            end = new Vector2(5.0f, 2.0f),
            texture = rockTexture
        });

        // Wall 16: 5.0,5.0 -> 5.0,4.0 Rock.bmp
        wallDictionary.Add(16, new Wall
        {
            start = new Vector2(5.0f, 5.0f),
            end = new Vector2(5.0f, 4.0f),
            texture = rockTexture
        });

        // TRIP WIRES//
        wallDictionary.Add(17, new Wall
        {
            start = new Vector2(0.0f, 2.0f),
            end = new Vector2(3.0f, 2.0f),
            texture = WallWire
        });

        wallDictionary.Add(18, new Wall
        {
            start = new Vector2(0.0f, 3.0f),
            end = new Vector2(3.0f, 3.0f),
            texture = WallWire
        });

        wallDictionary.Add(19, new Wall
        {
            start = new Vector2(3.0f, 6.0f),
            end = new Vector2(3.0f, 5.0f),
            texture = WallWire
        });

        wallDictionary.Add(20, new Wall
        {
            start = new Vector2(4.0f, 5.0f),
            end = new Vector2(4.0f, 3.0f),
            texture = WallWire
        });

        wallDictionary.Add(21, new Wall
        {
            start = new Vector2(5.0f, 4.0f),
            end = new Vector2(5.0f, 3.0f),
            texture = WallWire
        });

        wallDictionary.Add(22, new Wall
        {
            start = new Vector2(4.0f, 2.0f),
            end = new Vector2(4.0f, 0.0f),
            texture = WallWire
        });


        Debug.Log("Loaded " + wallDictionary.Count + " walls into the dictionary.");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int x = 0; x < wallDictionary.Count; x += 1)
        {
            BuildWall(wallDictionary[x].start.x * 2, wallDictionary[x].start.y * 2, wallDictionary[x].end.x * 2, wallDictionary[x].end.y * 2, wallDictionary[x].texture);
        }

        }
    public void BuildWall(float x1, float y1, float x2, float y2,Material TexMaterial)
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
        
        
        // 4. Adjust the wall’s length
        Vector3 scale = newWall.transform.localScale;
        scale.x = length;   // If the wall prefab is 1 unit long in local X
        newWall.transform.localScale = scale;

        if (TexMaterial == cheeseTexture)
        {
            GameObject Finishline = Instantiate(_CheeseTripWire, midpoint, Quaternion.Euler(0f, angleDeg, 0f));
            Vector3 scale_cheese= Finishline.transform.localScale;
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
            Debug.Log("Renderer is "+ wallRenderer.material);

        }
        else
        {
            Debug.LogError("Renderer not found on Wall(Clone)!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
