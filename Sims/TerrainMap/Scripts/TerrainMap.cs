using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TerrainMap : MonoBehaviour
{
    public Terrain terrain;
    
    public int seed = 0;
    public float frequency = 1;
    public int octaves = 1;
    public float heightExponent = 1;

    // Min/Max height values for normalizing between 0 and 1
    private float minHeight;
    private float maxHeight;


    [ContextMenu ("Generate")]
    public void Generate()
    {
        // Get the heightmap and new texture
        int resolution = terrain.terrainData.heightmapResolution;
        int randSeed = seed > 0 ? seed : Random.Range(int.MinValue, int.MaxValue);
        float[,] heightMap = GenerateHeight(resolution, resolution, randSeed);
        Texture2D texture = GenerateTexture(heightMap);

        // Apply heightmap and texture
        terrain.terrainData.SetHeights(0, 0, heightMap);
        terrain.terrainData.terrainLayers[0].diffuseTexture = texture;

        // Set scene dirty for saving
        UnityEditor.EditorUtility.SetDirty (this.gameObject);
        UnityEngine.SceneManagement.Scene currScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene ();
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(currScene);
    }

    public float[,] GenerateHeight(int xSize, int ySize, int randSeed)
    {
        SimplexNoise.Seed = randSeed;
        float[,] heightMap = new float[ySize, xSize];
        
        minHeight = float.MaxValue;
        maxHeight = float.MinValue;

        // Generate noise per pixel
        for (int y = 0; y < ySize; y++)
            for (int x = 0; x < xSize; x++)
            {
                float nX = ((float)x / xSize);
                float nY = ((float)y / ySize);

                float height = 0;
                float amplitudeSum = 0;
                
                // For each octave
                for (int oct = 0; oct < octaves; oct++)
                {
                    float frequencyMult = Mathf.Pow(2, oct);
                    float amplitude = 1.0f / frequencyMult;
                    amplitudeSum += amplitude;

                    height += SimplexNoise.CalcPixel2D(nX, nY, frequency * frequencyMult) * amplitude;
                }

                height = Mathf.Pow(height / amplitudeSum, heightExponent);

                minHeight = Mathf.Min(minHeight, height);
                maxHeight = Mathf.Max(maxHeight, height);

                heightMap[y, x] = height;
            }

        return heightMap;
    }

    public Texture2D GenerateTexture(float[,] heightMap)
    {
        // Set terrain layer width/length to make texture size fill terrain completely instead of tiling
        int width = (int)terrain.terrainData.size.x;
        int length = (int)terrain.terrainData.size.z;
        terrain.terrainData.terrainLayers[0].tileSize = new Vector2(width, length);
        
        // Set texture size to match terrain resolution / heightmap size
        int resolution = terrain.terrainData.heightmapResolution;
        Texture2D texture = terrain.terrainData.terrainLayers[0].diffuseTexture;
        texture.Resize(resolution, resolution);
        
        // Color each pixel
        for (int y = 0; y < resolution; y++)
            for (int x = 0; x < resolution; x++)
            {
                // Get scaled height
                float height = (heightMap[y, x] - minHeight) / (maxHeight - minHeight);

                // Determine color
                Color color = Color.white;

                if (height < 0.05)
                    color = Color.blue;
                else if (height < 0.1)
                    color = Color.yellow;
                else if (height < 0.2)
                    color = new Color(0.65f, 0.15f, 0.15f);
                else if (height < 0.6)
                    color = Color.green;

                texture.SetPixel(x, y, color);
            }

        texture.Apply();
        return texture;
    }
	
}
