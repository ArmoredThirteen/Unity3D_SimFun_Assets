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
                Vector3 downDir = GetFlowDir(map, xInd, yInd).Zof(0).normalized;
                //float lowestHeight = GetLowestHeight(map, xInt, yInt);
                drop.xPos = drop.xPos + downDir.x;
                drop.yPos = drop.yPos + downDir.y;

                if (xInd != (int)drop.xPos && yInd != (int)drop.yPos)
                    map[xInd, yInd] += 0.02f;
            }
        }

        // Return true if x2/y2 is lower than x1/y1
        // Return false if higher, same height, or out of bounds
        // https://stackoverflow.com/questions/49640250/calculate-normals-from-heightmap
        public static Vector3 GetFlowDir(float[,] map, int x, int y)
        {
            // Get the heights of the 4 bordering cells
            // If out of bounds, use height of map[y,x]
            float negX = GetHeightNegX(map, x, y);
            float negY = GetHeightNegY(map, x, y);
            float posX = GetHeightPosX(map, x, y);
            float posY = GetHeightPosY(map, x, y);

            //Vector3 normal = new Vector3(2 * (posY - negY), 2 * (posX - negX), -4);
            //return normal.normalized;

            /*int coordX = (int)posX;
            int coordY = (int)posY;

            // Calculate droplet's offset inside the cell (0,0) = at NW node, (1,1) = at SE node
            float x = posX - coordX;
            float y = posY - coordY;

            // Calculate heights of the four nodes of the droplet's cell
            int nodeIndexNW = coordY * mapSize + coordX;
            float heightNW = nodes[nodeIndexNW];
            float heightNE = nodes[nodeIndexNW + 1];
            float heightSW = nodes[nodeIndexNW + mapSize];
            float heightSE = nodes[nodeIndexNW + mapSize + 1];

            // Calculate droplet's direction of flow with bilinear interpolation of height difference along the edges
            float gradientX = (heightNE - heightNW) * (1 - y) + (heightSE - heightSW) * y;
            float gradientY = (heightSW - heightNW) * (1 - x) + (heightSE - heightNE) * x;*/
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
