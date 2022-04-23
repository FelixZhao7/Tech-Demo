using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTextureColor : MonoBehaviour
{
    public int width = 256;

    public int height = 256;

    void Start()
    {
        GeneratePerlinTexture();
    }

    // Generate Perlin noise texture
    private void GeneratePerlinTexture()
    {
        Texture2D texture = new Texture2D(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color cr =  GetColor(x, y);

                texture.SetPixel(x, y, cr);
            }
        }

        int z = 16;
        float k = (float)z/256;

        texture.Apply();
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = texture;
    }

    private Color GetColor(int x, int y)
    {
        float r = (float)y / 256f * 0.5f;
        float g = (float)y / 256f;
        float b = (float)x / 256f * 0.5f;

        return new Color(r, g, b);
    }
}
