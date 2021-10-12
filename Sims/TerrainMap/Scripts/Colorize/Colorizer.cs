using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ATE.TerrainGen
{
	public class Colorizer
	{
        private ColorizeSettings settings;
        private TerrainMap map;
        private Terrain terrain;


        public Colorizer(ColorizeSettings theSettings, TerrainMap theMap, Terrain theTerrain)
        {
            settings = theSettings;
            map = theMap;
            terrain = theTerrain;
        }


        public Texture2D GenerateTexture(ColorizeTypes colorizeType)
        {
            Func<float[,], int, int, Color> TexturingMethod = GetColor_White;
            switch (colorizeType)
            {
                case ColorizeTypes.White:
                    TexturingMethod = GetColor_White;
                    break;
                case ColorizeTypes.AltitudeCutoff:
                    TexturingMethod = GetColor_AltitudeCutoff;
                    break;
                case ColorizeTypes.Steepness:
                    TexturingMethod = GetColor_Steepness;
                    break;
                case ColorizeTypes.AltitudeWithSteepness:
                    TexturingMethod = GetColor_AltWithSteep;
                    break;
                case ColorizeTypes.Eroded:
                    TexturingMethod = GetColor_Eroded;
                    break;
                case ColorizeTypes.DirtDeposited:
                    TexturingMethod = GetColor_DirtDeposited;
                    break;
            }

            // Set terrain layer width/length to make texture size fill terrain completely instead of tiling
            int width = (int)terrain.terrainData.size.x;
            int length = (int)terrain.terrainData.size.z;
            terrain.terrainData.terrainLayers[0].tileSize = new Vector2(width, length);

            // Set texture size to match terrain resolution / heightmap size
            Texture2D texture = terrain.terrainData.terrainLayers[0].diffuseTexture;
            texture.Resize(map.resolution, map.resolution);

            // Color each pixel
            float[,] heightmap = map.GetAraOfType(TerrainValTypes.Rock);
            for (int y = 0; y < map.resolution; y++)
                for (int x = 0; x < map.resolution; x++)
                    texture.SetPixel(x, y, TexturingMethod(heightmap, x, y));

            texture.Apply();
            return texture;
        }


        private Color GetColor_White(float[,] heightmap, int x, int y)
        {
            return Color.white;
        }

        private Color GetColor_AltitudeCutoff(float[,] heightmap, int x, int y)
        {
            float height = heightmap[y, x];

            // Determine color
            Color color = Color.white;

            if (height < settings.altCutoff_blue)
                color = Color.blue;
            else if (height < settings.altCutoff_yellow)
                color = Color.yellow;
            else if (height < settings.altCutoff_red)
                color = new Color(0.65f, 0.15f, 0.15f);
            else if (height < settings.altCutoff_green)
                color = Color.green;

            return color;
        }

        private Color GetColor_Steepness(float[,] heightmap, int x, int y)
        {
            float steepness = map.GetSteepness(x, y);
            steepness = Mathf.Pow(steepness, settings.steepnessPower);
            return (Color.white * (1 - steepness)) + (Color.red * steepness);
        }

        private Color GetColor_AltWithSteep(float[,] heightmap, int x, int y)
        {
            Color altColor = GetColor_AltitudeCutoff(heightmap, x, y);

            float steepness = map.GetSteepness(x, y);

            return steepness > settings.steepnessCutoff ? Color.gray : altColor;
        }

        private Color GetColor_Eroded(float[,] heightmap, int x, int y)
        {
            float eroded = Mathf.Min(1, (map[y, x].Eroded * settings.erodedMultiplier));
            return (Color.white * (1 - eroded)) + (Color.red * eroded);
        }

        private Color GetColor_DirtDeposited(float[,] heightmap, int x, int y)
        {
            float dirt = Mathf.Min(1, (map[y, x].Dirt * settings.dirtMultiplier));
            return (Color.white * (1 - dirt)) + (Color.green * dirt);
        }

    }
}
