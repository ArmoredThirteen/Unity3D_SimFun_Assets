using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ATE.TerrainGen
{
	public static class WaterDropEroder
	{
		public static float[,] MakeEroded(float[,] startMap, LiquidSettings liquidSettings, int iterations, int dropsPerIteration)
        {
            float[,] map = (float[,])startMap.Clone();

            List<Droplet> drops = new List<Droplet>();

            for (int i = 0; i < iterations; i++)
            {
                UpdateDropCounts(drops, map.GetLength(1), map.GetLength(0), dropsPerIteration, liquidSettings);
                Iterate(map, drops);
            }

            Debug.Log(drops.Count);

            return map;
        }
		
        public static void UpdateDropCounts(List<Droplet> drops, int maxX, int maxY, int dropsToAdd, LiquidSettings liquidSettings)
        {
            // Clear old drops
            for (int k = drops.Count - 1; k >= 0; k--)
                if (drops[k].lifetime >= drops[k].settings.maxLifetime)
                    drops.RemoveAt(k);

            // Add new drops
            for (int k = 0; k < dropsToAdd; k++)
            {
                int xPos = Random.Range(0, maxX);
                int yPos = Random.Range(0, maxY);
                drops.Add(new Droplet(liquidSettings, xPos, yPos, 1, 0));
            }
        }

        public static void Iterate (float[,] map, List<Droplet> drops)
        {
            int xLen = map.GetLength(1);
            int yLen = map.GetLength(0);

            for (int i = 0; i < drops.Count; i++)
            {
                Droplet drop = drops[i];
                drop.lifetime++;

                int xPos = (int)drop.xPos;
                int yPos = (int)drop.yPos;
                int lowX = xPos;
                int lowY = yPos;

                // Negative X
                if (IsLower(map, xPos, yPos, xPos - 1, yPos))
                {
                    lowX = xPos - 1;
                    lowY = yPos;
                }
                // Positive X
                // Negative Y
                // Positive Y
            }
        }

        // Return true if x2/y2 is lower than x1/y1
        // Return false if higher, same height, or out of bounds
        public static bool IsLower(float[,] map, int x1, int y1, int x2, int y2)
        {
            //TODO: Actually do this
            return false;
        }

	}
}
