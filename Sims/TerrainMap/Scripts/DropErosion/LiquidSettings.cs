using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ATE.TerrainGen
{
    [CreateAssetMenu(fileName = "Liquid_", menuName = "ScriptableObjects/SpawnLiquidSettings", order = 1)]
    public class LiquidSettings : ScriptableObject
	{
        public int maxLifetime = 25;
        public float dirtCapacity = 4;
        public float minDirt = 0.01f;

        [Range(0,1)]
        public float erodeSpeed = 0.25f;
        [Range(0, 1)]
        public float depositSpeed = 0.25f;
        [Range(0, 1)]
        public float evapSpeed = 0.01f;

        public float[,] erosionBrush =
        {
            {0.0f, 0.2f, 0.3f, 0.2f, 0.0f},
            {0.2f, 0.4f, 0.7f, 0.4f, 0.2f},
            {0.3f, 0.7f, 1.0f, 0.7f, 0.3f},
            {0.2f, 0.4f, 0.7f, 0.4f, 0.2f},
            {0.0f, 0.2f, 0.3f, 0.2f, 0.0f}
        };

	}
}
