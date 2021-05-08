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
        public float grabSpeed = 0.25f;
        [Range(0, 1)]
        public float dropSpeed = 0.25f;
        [Range(0, 1)]
        public float evapSpeed = 0.01f;
		
	}
}
