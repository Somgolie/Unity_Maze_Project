using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] modelPrefabs; // Array to hold references to the prefabs



    // Method to spawn a model by index (for models from wall data)

    public static void SpawnPeripheralObject(float x1, float y1, float x2, float y2, float scale, float rotationAngle, string objFile)
    {
        Debug.Log(objFile+" Object found");
        /*      // Use the static version of FindPrefabByName
              GameObject Prefab = FindPrefabByName(objFile);
              Vector3 startPos = new Vector3(x1, 0f, y1);
              Vector3 endPos = new Vector3(x2, 0f, y2);
              Vector3 midpoint = (startPos + endPos) / 2f;
              if (Prefab != null)
              {
                  GameObject ObjectPrefab = Instantiate(Prefab, midpoint, Quaternion.identity);
                  // Apply a constant scale
                  ObjectPrefab.transform.localScale = new Vector3(scale, scale, scale);
              }
              else
              {
                  Debug.LogError($"Prefab "+objFile+"not found.");
              }*/
    }

    // Make this method static as well to be used in the static SpawnPeripheralObject

}
