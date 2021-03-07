using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TerrainMap : MonoBehaviour
{
    public Terrain terrain;
    
    public int heightSeed = 0;
    public float heightScale = 1;
    public int heightOctaves = 1;
    public float heightExponent = 1;


    [ContextMenu ("Generate")]
    public void Generate()
    {
        GenerateHeight ();

        Debug.Log ("Complete");

        UnityEditor.EditorUtility.SetDirty (this.gameObject);
        UnityEngine.SceneManagement.Scene currScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene ();
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(currScene);
    }

    public void GenerateHeight()
    {
        int resolution = terrain.terrainData.heightmapResolution;

        SimplexNoise.Seed = heightSeed > 0 ? heightSeed : Random.Range (int.MinValue, int.MaxValue);
        //float[,] heightNoise = SimplexNoise.Noise.Calc2D (resolution, resolution, heightScale);

        float[,] heightNoise = new float[resolution, resolution];

        float lowest = int.MaxValue;
        float highest = int.MinValue;

        for (int y = 0; y < resolution; y++)
            for (int x = 0; x < resolution; x++)
            {
                float nX = ((float)x / resolution);
                float nY = ((float)y / resolution);

                float noise = 0;

                for (int i = 0; i < heightOctaves; i++)
                {
                    float frequency = Mathf.Pow (2, i + 1);
                    float amplitude = 1.0f / frequency;
                    noise += SimplexNoise.CalcPixel2D (nX + (i * 100), nY + (i * 100), heightScale * frequency) * amplitude;
                }

                noise = noise / heightOctaves;
                lowest = Mathf.Min (noise, lowest);
                highest = Mathf.Max (noise, highest);

                noise = Mathf.Pow (noise, heightExponent);

                heightNoise[y, x] = noise;
            }

        Debug.Log ($"L: {lowest}, H: {highest}");
        terrain.terrainData.SetHeights (0, 0, heightNoise);
    }
	
}
