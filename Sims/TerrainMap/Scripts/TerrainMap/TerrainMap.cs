using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;


namespace ATE.TerrainGen
{
    public class TerrainMap : MonoBehaviour
    {
        public int resolution = 512;
        public Terrain terrain;

        public int seed = 0;

        public SimplexOctavesSettings simplexGroundSettings;

        public bool useWaterErosion = true;
        public LiquidSettings waterSettings;
        public int waterDropsToErode = 1000;

        public ColorizeTypes colorizeType = ColorizeTypes.Steepness;
        public ColorizeSettings colorizeSettings;


        public Flat2dAra<TerrainCell> map;


        public float[,] GetAraOfType(TerrainValTypes valType)
        {
            float[,] ara = new float[resolution, resolution];

            for (int y = 0; y < resolution; y++)
                for (int x = 0; x < resolution; x++)
                    ara[y, x] = map[y, x][valType];

            return ara;
        }

        public void SetValsOfType(TerrainValTypes valType, float[,] ara)
        {
            for (int y = 0; y < resolution; y++)
                for (int x = 0; x < resolution; x++)
                    map[y, x][valType] = ara[y, x];
        }


        [ContextMenu("Generate")]
        public void Generate()
        {
            // Find and set seeds
            int randSeed = seed > 0 ? seed : Random.Range(int.MinValue, int.MaxValue);
            Random.InitState(randSeed);
            SimplexNoise.Seed = randSeed;

            GroundGen_SimplexOctaves groundGen = new GroundGen_SimplexOctaves(this);
            WaterDropEroder eroder = new WaterDropEroder(this);

            groundGen.RegenerateGround(simplexGroundSettings);

            if (useWaterErosion)
                eroder.MakeEroded(waterSettings, waterDropsToErode);

            float[,] heightmap = GetAraOfType(TerrainValTypes.Rock);
            heightmap.Zip(GetAraOfType(TerrainValTypes.Dirt), (x, y) => x + y);
            terrain.terrainData.SetHeights(0, 0, heightmap);

            //float[,] debugmap = GetAraOfType(TerrainValTypes.Dirt);
            //float highest = ArrayHelpers.GetHighest(debugmap);
            //float lowest = ArrayHelpers.GetLowest(debugmap);
            //float average = ArrayHelpers.GetAverageOfNonZero(debugmap);
            //Debug.Log("Highest: " + highest + ", Lowest: " + lowest + ", Average: " + average);

            // Build and apply height texture
            GenAndApplyTexture();

            // Set scene dirty for saving
            UnityEditor.EditorUtility.SetDirty(this.gameObject);
            UnityEngine.SceneManagement.Scene currScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(currScene);
        }


        public void ResetMap()
        {
            map = new Flat2dAra<TerrainCell>(resolution, resolution);
            for (int y = 0; y < resolution; y++)
                for (int x = 0; x < resolution; x++)
                    map[y, x] = new TerrainCell(0);

            //resolution = terrain.terrainData.heightmapResolution;
            terrain.terrainData.heightmapResolution = resolution;
        }


        [ContextMenu("Generate Texture")]
        public void GenAndApplyTexture()
        {
            Colorizer colorizer = new Colorizer(colorizeSettings, this, terrain);
            Texture2D texture = colorizer.GenerateTexture(colorizeType);
            terrain.terrainData.terrainLayers[0].diffuseTexture = texture;
            
            // Save changes to disk
            string texturePath = UnityEditor.AssetDatabase.GetAssetPath(terrain.terrainData.terrainLayers[0].diffuseTexture);
            System.IO.File.WriteAllBytes(texturePath, texture.EncodeToPNG());
        }


        public float GetSteepness(int x, int y)
        {
            int resolution = terrain.terrainData.heightmapResolution;
            float xPos = (float)x / (resolution - 1);
            float yPos = (float)y / (resolution - 1);

            return terrain.terrainData.GetSteepness(xPos, yPos) / 90;
        }


        public TerrainCell this[int y, int x]
        {
            get => map[y, x];
        }

    }
}
