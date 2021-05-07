using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ATE.TerrainGen
{
    [CreateAssetMenu(fileName = "Liquid_", menuName = "ScriptableObjects/SpawnLiquidSettings", order = 1)]
    public class LiquidSettings : ScriptableObject
	{
        public int maxLifetime = 10;
        public float dirtCapacity = 1;
        public float minDirt = 0.1f;

        [Range(0,1)]
        public float grabSpeed;
        [Range(0, 1)]
        public float dropSpeed;
        [Range(0, 1)]
        public float evapSpeed;
		
	}
}
