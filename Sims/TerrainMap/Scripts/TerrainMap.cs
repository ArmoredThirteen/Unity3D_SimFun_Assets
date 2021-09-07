using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;


namespace ATE.TerrainGen
{
    public class TerrainMap : MonoBehaviour
    {
        public enum TexturingTypes
        {
            White,
            AltitudeCutoff,
            Steepness,
            AltitudeWithSteepness,
        }


        public Terrain terrain;

        public int seed = 0;
        public int octaves = 1;
        public float frequency = 1;
        public float heightExponent = 1;

        public bool useWaterErosion = true;
        public LiquidSettings waterSettings;
        public int waterDropsToErode = 1000;

        public TexturingTypes texturingType = TexturingTypes.Steepness;


        private float[,] heightMap;


        [ContextMenu("Generate")]
        public void Generate()
        {
            int resolution = terrain.terrainData.heightmapResolution;

            // Find and set seeds
            int randSeed = seed > 0 ? seed : Random.Range(int.MinValue, int.MaxValue);
            Random.InitState(randSeed);
            SimplexNoise.Seed = randSeed;

            // Build and apply height map
            heightMap = GenerateHeightmap(resolution, resolution, randSeed);
            terrain.terrainData.SetHeights(0, 0, heightMap);

            // Build and apply height texture
            GenAndApplyTexture();

            // Set scene dirty for saving
            UnityEditor.EditorUtility.SetDirty(this.gameObject);
            UnityEngine.SceneManagement.Scene currScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(currScene);
        }

        public float[,] GenerateHeightmap(int xSize, int ySize, int randSeed)
        {
            float[,] map = SimplexNoise.CalcOctaved2D(xSize, ySize, octaves, frequency, heightExponent);
            ArrayHelpers.NormalizeArray2D(map, 0, 1);

            if (useWaterErosion)
                map = WaterDropEroder.MakeEroded(map, waterSettings, waterDropsToErode);

            //ArrayHelpers.NormalizeArray2D(map, 0, 1);

            Debug.Log("Completed heightmap");
            return map;
        }


        [ContextMenu("Generate Texture")]
        public void GenAndApplyTexture()
        {
            Texture2D texture = GenerateTexture();
            terrain.terrainData.terrainLayers[0].diffuseTexture = texture;
        }

        public Texture2D GenerateTexture(/*float[,] heightMap*/)
        {
            Func<float[,], int, int, Color> TexturingMethod = GetColor_White;
            switch (texturingType)
            {
                case TexturingTypes.White:
                    TexturingMethod = GetColor_White;
                    break;
                case TexturingTypes.AltitudeCutoff:
                    TexturingMethod = GetColor_AltitudeCutoff;
                    break;
                case TexturingTypes.Steepness:
                    TexturingMethod = GetColor_Steepness;
                    break;
                case TexturingTypes.AltitudeWithSteepness:
                    TexturingMethod = GetColor_AltWithSteep;
                    break;
            }

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
                    texture.SetPixel(x, y, TexturingMethod(heightMap, x, y));

            texture.Apply();

            Debug.Log("Completed texture");
            return texture;
        }


        private Color GetColor_White(float[,] map, int x, int y)
        {
            return Color.white;
        }

        private Color GetColor_AltitudeCutoff(float[,] map, int x, int y)
        {
            // Get scaled height
            float height = map[y, x];

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

            return color;
        }

        private Color GetColor_Steepness(float[,] map, int x, int y)
        {
            float steepness = GetSteepness(x, y);
            steepness = Mathf.Pow(steepness, 2);
            return (Color.white * (1 - steepness)) + (Color.red * steepness);
        }

        private Color GetColor_AltWithSteep(float[,] map, int x, int y)
        {
            Color altColor = GetColor_AltitudeCutoff(map, x, y);

            float steepness = GetSteepness(x, y);
            //steepness = Mathf.Pow(steepness, 2);
            //return (altColor * (1 - steepness)) + (Color.gray * steepness);

            return steepness > 0.65f ? Color.gray : altColor;
        }


        private float GetSteepness (int x, int y)
        {
            int resolution = terrain.terrainData.heightmapResolution;
            float xPos = (float)x / (resolution - 1);
            float yPos = (float)y / (resolution - 1);

            return terrain.terrainData.GetSteepness(xPos, yPos) / 90;
        }

    }
}
