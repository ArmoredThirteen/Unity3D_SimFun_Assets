using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ATE.TerrainGen
{
    public class TerrainMap : MonoBehaviour
    {
        public Terrain terrain;

        public int seed = 0;
        public int octaves = 1;
        public float frequency = 1;
        public float heightExponent = 1;

        public bool useWaterErosion = true;
        public LiquidSettings waterSettings;
        public int waterErosionIterations = 1000;
        public int waterDropsPerIteration = 10000;


        [ContextMenu("Generate")]
        public void Generate()
        {
            int resolution = terrain.terrainData.heightmapResolution;

            // Find and set seeds
            int randSeed = seed > 0 ? seed : Random.Range(int.MinValue, int.MaxValue);
            Random.InitState(randSeed);
            SimplexNoise.Seed = randSeed;

            // Build and apply height map
            float[,] heightMap = GenerateHeightmap(resolution, resolution, randSeed);
            
            terrain.terrainData.SetHeights(0, 0, heightMap);

            // Build and apply height texture
            Texture2D texture = GenerateTexture(heightMap);
            terrain.terrainData.terrainLayers[0].diffuseTexture = texture;

            Debug.Log("Complete!");

            // Set scene dirty for saving
            UnityEditor.EditorUtility.SetDirty(this.gameObject);
            UnityEngine.SceneManagement.Scene currScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(currScene);
        }

        public float[,] GenerateHeightmap(int xSize, int ySize, int randSeed)
        {
            float[,] map = SimplexNoise.CalcOctaved2D(xSize, ySize, octaves, frequency, heightExponent);

            if (useWaterErosion)
                map = WaterDropEroder.MakeEroded(map, waterSettings, waterErosionIterations, waterDropsPerIteration);

            ArrayHelpers.NormalizeArray2D(map, 0, 1);
            return map;
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
                    float height = heightMap[y, x];

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
}
