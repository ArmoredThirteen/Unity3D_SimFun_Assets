using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ATE.TerrainGen
{
	public class GroundGen_SimplexOctaves
	{
        private TerrainMap map;


        public GroundGen_SimplexOctaves(TerrainMap theMap)
        {
            map = theMap;
        }


        public void RegenerateGround (SimplexOctavesSettings simplexSettings)
        {
            map.ResetMap();

            float[,] noiseMap = SimplexNoise.CalcOctaved2D(map.resolution, map.resolution,
                                simplexSettings.octaves, simplexSettings.frequency, simplexSettings.heightExponent);
            ArrayHelpers.NormalizeArray2D(noiseMap, 0, 1);

            map.SetValsOfType(TerrainValTypes.Rock, noiseMap);
        }

    }
}
