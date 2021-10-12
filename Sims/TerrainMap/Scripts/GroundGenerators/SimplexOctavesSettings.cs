using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ATE.TerrainGen
{
    [CreateAssetMenu(fileName = "SimplexOctaves_", menuName = "ScriptableObjects/SimplexOctavesSettings", order = 1)]
    public class SimplexOctavesSettings : ScriptableObject
    {
        public int octaves = 5;
        public float frequency = 2;
        public float heightExponent = 3;

    }
}
