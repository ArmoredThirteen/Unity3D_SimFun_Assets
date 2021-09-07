using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ATE.TerrainGen
{
	public static class WaterDropEroder
	{
		public static float[,] MakeEroded(float[,] startMap, LiquidSettings liquidSettings, int dropCount)
        {
            float[,] map = (float[,])startMap.Clone();

            for (int i = 0; i < dropCount; i++)
            {
                Vector2 newPos = ArrayHelpers.GetRandomIndex(map);
                Droplet drop = new Droplet(liquidSettings, newPos.x, newPos.y, 1, 0);
                ErodeDroplet(map, drop);
            }

            return map;
        }

        public static void ErodeDroplet(float[,] map, Droplet drop)
        {
            int xLen = map.GetLength(1);
            int yLen = map.GetLength(0);

            float[,] brush = drop.settings.GetErosionBrush();

            while (CanDropletRun(drop))
            {
                drop.lifetime++;
                
                int xInd = (int)drop.xPos;
                int yInd = (int)drop.yPos;

                // If in a pit, fast track evaporation
                // Deposit dirt up to height of lowest neighbor, reduce liquid volume proportionally
                if (!IsMovementPossible(map, xInd, yInd))
                {
                    float neighborHeight = GetHeightOfLowestNeighbor(map, xInd, yInd);
                    float depositAmount = Mathf.Min(drop.dirt, neighborHeight - map[yInd, xInd]);

                    // Lower liquid volume proportionally and deposit dirt
                    drop.volume *= 1 - (drop.dirt / depositAmount);
                    map[yInd, xInd] += depositAmount;

                    //continue;
                    break;
                }

                Vector3 downDir = GetFlowDir(map, drop).Zof(0).normalized;
                drop.xPos = drop.xPos + downDir.x;
                drop.yPos = drop.yPos + downDir.y;

                int xIndNew = (int)drop.xPos;
                int yIndNew = (int)drop.yPos;

                // Verify new location is in bounds
                if (!IsInBounds(xLen, yLen, xIndNew, yIndNew))
                    break;
                // Hasn't moved enough to do a calculation this iteration
                if (!HasIndChanged(xInd, yInd, xIndNew, yIndNew))
                    continue;

                float dHeight = GetHeightDif(map, xInd, yInd, xIndNew, yIndNew);

                //TODO: What exactly is this doing?
                float totalCapacity = Mathf.Max(
                    drop.settings.minDirt,
                    drop.settings.dirtCapacity * -dHeight/* * drop.volume*/);

                // Deposit dirt
                if (drop.dirt > totalCapacity)
                {
                    float dirtAmount = (drop.dirt - totalCapacity) * drop.settings.depositSpeed;
                    DepositDirt(map, xInd, yInd, drop, dirtAmount);
                }
                // Erode dirt
                else
                {
                    float dirtAmount = Mathf.Min((totalCapacity - drop.dirt) * drop.settings.erodeSpeed, -dHeight);
                    ErodeDirt(map, brush, xInd, yInd, drop, dirtAmount);
                }

                // Slowly reduce amount of water
                drop.volume *= (1 - drop.settings.evapSpeed);
            }
        }


        public static bool CanDropletRun(Droplet droplet)
        {
            if (droplet.lifetime >= droplet.settings.maxLifetime)
                return false;
            if (droplet.volume <= 0)
                return false;

            return true;
        }

        public static bool IsInBounds(int xLen, int yLen, int xInd, int yInd)
        {
            if (xInd < 0 || xInd >= xLen)
                return false;
            if (yInd < 0 || yInd >= yLen)
                return false;

            return true;
        }

        public static bool HasIndChanged(int xInd, int yInd, int xIndNew, int yIndNew)
        {
            if (xInd == xIndNew && yInd == yIndNew)
                return false;
            return true;
        }

        public static float GetHeightDif(float[,] map, int xInd, int yInd, int xIndNew, int yIndNew)
        {
            return map[yIndNew, xIndNew] - map[yInd, xInd];
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

        public static bool IsMovementPossible(float[,] map, int x, int y)
        {
            return map[y, x] > GetHeightOfLowestNeighbor(map, x, y);
        }

        public static float GetHeightOfLowestNeighbor(float[,] map, int x, int y)
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


        private static void DepositDirt(float[,] map, int xInd, int yInd, Droplet drop, float depositAmount)
        {
            drop.dirt -= depositAmount;
            map[yInd, xInd] += depositAmount;
        }

        private static void ErodeDirt(float[,] map, float[,] brush, int xInd, int yInd, Droplet drop, float erodeAmount)
        {
            //float[,] brush = GetErosionBrush(drop.settings, erodeAmount);

            float lowestNeighborHeight = GetHeightOfLowestNeighbor(map, xInd, yInd);

            int xLen_map = map.GetLength(1);
            int yLen_map = map.GetLength(0);
            int xLen_brush = brush.GetLength(1);
            int yLen_brush = brush.GetLength(0);

            int xOffset = -(xLen_brush / 2);
            int yOffset = -(yLen_brush / 2);

            for (int y = 0; y < yLen_brush; y++)
                for (int x = 0; x < xLen_brush; x++)
                {
                    int xInd_map = xInd + xOffset + x;
                    int yInd_map = yInd + yOffset + y;

                    // Out of bounds
                    if (!IsInBounds(xLen_map, yLen_map, xInd_map, yInd_map))
                        continue;
                    // Too low to erode
                    float heightDif = map[yInd_map, xInd_map] - lowestNeighborHeight;
                    if (heightDif <= 0)
                        continue;

                    // Only erode down to lowest neighbor at most
                    float actualErodeAmount = Mathf.Min(brush[y, x] * erodeAmount, heightDif);

                    drop.dirt += actualErodeAmount;
                    map[yInd_map, xInd_map] -= actualErodeAmount;
                }
        }


        private static float[,] GetErosionBrush(LiquidSettings settings, float amount)
        {
            float[,] brush = settings.GetErosionBrush ();
            int xLen = brush.GetLength(1);
            int yLen = brush.GetLength(0);
            
            for (int y = 0; y < yLen; y++)
                for (int x = 0; x < xLen; x++)
                    brush[y, x] *= amount;

            return brush;
        }

    }
}
