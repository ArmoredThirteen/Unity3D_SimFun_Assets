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
                UpdateDropCounts(drops, map.GetLength(1) - 1, map.GetLength(0) - 1, dropsPerIteration, liquidSettings);
                Iterate(map, drops);
            }

            Debug.Log(drops.Count);

            return map;
        }

        public static void UpdateDropCounts(List<Droplet> drops, int maxX, int maxY, int dropsToAdd, LiquidSettings liquidSettings)
        {
            // Clear old drops
            for (int k = drops.Count - 1; k >= 0; k--)
            {
                if (drops[k].lifetime >= drops[k].settings.maxLifetime
                    || drops[k].xPos < 0 || drops[k].xPos > maxX
                    || drops[k].yPos < 0 || drops[k].yPos > maxY)
                    drops.RemoveAt(k);
            }

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

                int xInd = (int)drop.xPos;
                int yInd = (int)drop.yPos;

                // Find direction of flow and get new drop location
                Vector3 downDir = GetFlowDir(map, drop).Zof(0).normalized;
                //float lowestHeight = GetLowestHeight(map, xInt, yInt);
                drop.xPos = drop.xPos + downDir.x;
                drop.yPos = drop.yPos + downDir.y;

                if (xInd != (int)drop.xPos && yInd != (int)drop.yPos)
                    map[yInd, xInd] += 0.02f;
            }
        }

        // Return true if x2/y2 is lower than x1/y1
        // Return false if higher, same height, or out of bounds
        // https://stackoverflow.com/questions/49640250/calculate-normals-from-heightmap
        public static Vector3 GetFlowDir(float[,] map, Droplet drop)
        {
            int xInd = (int)drop.xPos;
            int yInd = (int)drop.yPos;

            // Get the heights of the 4 bordering cells
            // If out of bounds, use height of map[y,x]
            float negX = GetHeightNegX(map, xInd, yInd);
            float negY = GetHeightNegY(map, xInd, yInd);
            float posX = GetHeightPosX(map, xInd, yInd);
            float posY = GetHeightPosY(map, xInd, yInd);

            Vector3 normal = new Vector3(negX - posX, negY - posY, -4);
            return normal.normalized;
        }

        public static float GetLowestHeight(float[,] map, int x, int y)
        {
            return Mathf.Min(
                GetHeightNegX(map, x, y),
                GetHeightPosX(map, x, y),
                GetHeightNegY(map, x, y),
                GetHeightPosY(map, x, y)
                );
        }


        // Gets the height of [x-1,y] but default to [x,y] if out of bounds
        private static float GetHeightNegX(float[,] map, int x, int y)
        {
            return x - 1 < 0 ? map[y, x] : map[y, x - 1];
        }

        // Gets the height of [x-1,y] but default to [x,y] if out of bounds
        private static float GetHeightNegY(float[,] map, int x, int y)
        {
            return y - 1 < 0 ? map[y, x] : map[y - 1, x];
        }

        // Gets the height of [x-1,y] but default to [x,y] if out of bounds
        private static float GetHeightPosX(float[,] map, int x, int y)
        {
            return x + 1 >= map.GetLength(1) ? map[y, x] : map[y, x + 1];
        }

        // Gets the height of [x-1,y] but default to [x,y] if out of bounds
        private static float GetHeightPosY(float[,] map, int x, int y)
        {
            return y + 1 >= map.GetLength(0) ? map[y, x] : map[y + 1, x];
        }

    }
}
