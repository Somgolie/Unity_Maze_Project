using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WallScript : MonoBehaviour
{
    [SerializeField]
    public Vector2 start;

    [SerializeField]
    public Vector2 end;


    [SerializeField]
    public Material Mat;
    public void ChangeTexture(Material image)
    {
        Mat = image;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
