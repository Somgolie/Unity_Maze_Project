using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine;
public class UIText : MonoBehaviour
{


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void changeTexture(Texture Utexture)
    {
        RawImage rawImage = GetComponent<RawImage>();
        rawImage.texture = Utexture;
    }
}
