using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise {

    public enum NormalizeMode {Local, Global};

    public static float[,] GenerateNoiseMap(int mapwidth, int mapheight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset,NormalizeMode normalizeMode ) {

        float[,] noisemap = new float[mapwidth, mapheight];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        float maxPossibleHeight = 0;

        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;

            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistance; 

        }
 
        if (scale <= 0)
        {
            scale = 0.000001f;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfwidth = mapwidth / 2f;
        //float halfheight = mapheight / 2f; 

        for(int y = 0; y < mapheight; y++)
        {
            for(int x = 0; x < mapwidth; x++)
            {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for(int i = 0; i < octaves; i++)
                {
                    float samplex = (x-halfwidth + octaveOffsets[i].x) / scale * frequency;
                    float sampley = (y-halfwidth + octaveOffsets[i].y) / scale * frequency;

                    float perlinvalue = Mathf.PerlinNoise(samplex, sampley)*2 - 1;

                    noisemap[x, y] = perlinvalue;
                    noiseHeight += perlinvalue * amplitude;

                    amplitude *= persistance;

                    frequency *= lacunarity;

                }

                if (noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }

                else if (noiseHeight < minLocalNoiseHeight) {
                    minLocalNoiseHeight = noiseHeight;
                }

                noisemap[x, y] = noiseHeight;
                
            }
        }

        for (int y = 0; y < mapheight; y++)
        {
            for (int x = 0; x < mapwidth; x++)
            {
                if(normalizeMode == NormalizeMode.Local)
                {
                    noisemap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noisemap[x, y]);
                }
                else
                {
                    float normalizedHeight = (noisemap[x, y] + 1) / 2f*maxPossibleHeight/2f;
                    noisemap[x, y] = Mathf.Clamp(normalizedHeight,0,int.MaxValue);
                }
                
            }
        }

                return noisemap;
    }
}

