using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ATE.TerrainGen
{
	public class WaterDropEroder
	{
        private TerrainMap map;


        public WaterDropEroder (TerrainMap theMap)
        {
            map = theMap;
        }


		public void MakeEroded(LiquidSettings liquidSettings, int dropCount)
        {
            for (int i = 0; i < dropCount; i++)
            {
                int xInd = Random.Range(0, map.resolution);
                int yInd = Random.Range(0, map.resolution);

                Droplet drop = new Droplet(liquidSettings, xInd, yInd, 1, 0);
                ErodeDroplet(drop);
            }
        }

        public void ErodeDroplet(Droplet drop)
        {
            int xLen = map.resolution;
            int yLen = map.resolution;

            float[,] brush = drop.settings.GetErosionBrush();

            while (CanDropletRun(drop))
            {
                drop.lifetime++;
                
                int xInd = (int)drop.xPos;
                int yInd = (int)drop.yPos;

                // If in a pit, fast track evaporation
                // Deposit dirt up to height of lowest neighbor, reduce liquid volume proportionally
                if (!IsMovementPossible(xInd, yInd))
                {
                    float neighborHeight = GetHeightOfLowestNeighbor(xInd, yInd);
                    float depositAmount = Mathf.Min(drop.dirt, neighborHeight - map[yInd, xInd].GroundHeight);

                    // Lower liquid volume proportionally and deposit dirt
                    drop.volume *= 1 - (drop.dirt / depositAmount);
                    //map[yInd, xInd] += depositAmount;

                    continue;
                    //break;
                }

                Vector3 downDir = GetFlowDir(drop).Zof(0).normalized;
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

                float dHeight = GetHeightDif(xInd, yInd, xIndNew, yIndNew);

                //TODO: What exactly is this doing?
                float totalCapacity = Mathf.Max(
                    drop.settings.minDirt,
                    drop.settings.dirtCapacity * -dHeight/* * drop.volume*/);

                // Deposit dirt
                if (drop.dirt > totalCapacity)
                {
                    float dirtAmount = (drop.dirt - totalCapacity) * drop.settings.depositSpeed;
                    DepositDirt(xInd, yInd, drop, dirtAmount);
                }
                // Erode dirt
                else
                {
                    float dirtAmount = Mathf.Min((totalCapacity - drop.dirt) * drop.settings.erodeSpeed, -dHeight);
                    ErodeDirt(brush, xInd, yInd, drop, dirtAmount);
                }

                // Slowly reduce amount of water
                drop.volume *= (1 - drop.settings.evapSpeed);
            }
        }


        public bool CanDropletRun(Droplet droplet)
        {
            if (droplet.lifetime >= droplet.settings.maxLifetime)
                return false;
            if (droplet.volume <= 0)
                return false;

            return true;
        }

        public bool IsInBounds(int xLen, int yLen, int xInd, int yInd)
        {
            if (xInd < 0 || xInd >= xLen)
                return false;
            if (yInd < 0 || yInd >= yLen)
                return false;

            return true;
        }

        public bool HasIndChanged(int xInd, int yInd, int xIndNew, int yIndNew)
        {
            if (xInd == xIndNew && yInd == yIndNew)
                return false;
            return true;
        }

        public float GetHeightDif(int xInd, int yInd, int xIndNew, int yIndNew)
        {
            return map[yIndNew, xIndNew].GroundHeight - map[yInd, xInd].GroundHeight;
        }


        // Return true if x2/y2 is lower than x1/y1
        // Return false if higher, same height, or out of bounds
        // https://stackoverflow.com/questions/49640250/calculate-normals-from-heightmap
        public Vector3 GetFlowDir(Droplet drop)
        {
            int xInd = (int)drop.xPos;
            int yInd = (int)drop.yPos;

            // Get the heights of the 4 bordering cells
            // If out of bounds, use height of map[y,x]
            float negX = GetHeightNegX(xInd, yInd);
            float negY = GetHeightNegY(xInd, yInd);
            float posX = GetHeightPosX(xInd, yInd);
            float posY = GetHeightPosY(xInd, yInd);

            Vector3 normal = new Vector3(negX - posX, negY - posY, -4);
            return normal.normalized;
        }

        public bool IsMovementPossible(int x, int y)
        {
            return map[y, x].GroundHeight > GetHeightOfLowestNeighbor(x, y);
        }

        public float GetHeightOfLowestNeighbor(int x, int y)
        {
            return Mathf.Min(
                GetHeightNegX(x, y),
                GetHeightPosX(x, y),
                GetHeightNegY(x, y),
                GetHeightPosY(x, y)
                );
        }


        // Gets the height of [x-1,y] but default to [x,y] if out of bounds
        private float GetHeightNegX(int x, int y)
        {
            return x - 1 < 0 ? map[y, x].GroundHeight : map[y, x - 1].GroundHeight;
        }

        // Gets the height of [x-1,y] but default to [x,y] if out of bounds
        private float GetHeightNegY(int x, int y)
        {
            return y - 1 < 0 ? map[y, x].GroundHeight : map[y - 1, x].GroundHeight;
        }

        // Gets the height of [x-1,y] but default to [x,y] if out of bounds
        private float GetHeightPosX(int x, int y)
        {
            return x + 1 >= map.resolution ? map[y, x].GroundHeight : map[y, x + 1].GroundHeight;
        }

        // Gets the height of [x-1,y] but default to [x,y] if out of bounds
        private float GetHeightPosY(int x, int y)
        {
            return y + 1 >= map.resolution ? map[y, x].GroundHeight : map[y + 1, x].GroundHeight;
        }


        private void DepositDirt(int xInd, int yInd, Droplet drop, float depositAmount)
        {
            drop.dirt -= depositAmount;
            //map[yInd, xInd][TerrainValTypes.Rock] += depositAmount;
            map[yInd, xInd].AddGround(depositAmount);
        }

        private void ErodeDirt(float[,] brush, int xInd, int yInd, Droplet drop, float erodeAmount)
        {
            //float[,] brush = GetErosionBrush(drop.settings, erodeAmount);

            float lowestNeighborHeight = GetHeightOfLowestNeighbor(xInd, yInd);

            int xLen_map = map.resolution;
            int yLen_map = map.resolution;
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
                    float heightDif = map[yInd_map, xInd_map].GroundHeight - lowestNeighborHeight;
                    if (heightDif <= 0)
                        continue;

                    // Only erode down to lowest neighbor at most
                    float actualErodeAmount = Mathf.Min(brush[y, x] * erodeAmount, heightDif);

                    drop.dirt += actualErodeAmount;
                    //map[yInd_map, xInd_map][TerrainValTypes.Rock] -= actualErodeAmount;
                    map[yInd_map, xInd_map].RemoveGround(actualErodeAmount);
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
