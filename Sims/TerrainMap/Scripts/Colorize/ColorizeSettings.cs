using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ATE.TerrainGen
{
    [CreateAssetMenu(fileName = "Colorize_", menuName = "ScriptableObjects/ColorizeSettings", order = 1)]
    public class ColorizeSettings : ScriptableObject
	{
        public float steepnessPower = 2;
        public float steepnessCutoff = 0.65f;
        public float erodedMultiplier = 15;
        public float dirtMultiplier = 100;
        public float altCutoff_blue = 0.05f;
        public float altCutoff_yellow = 0.1f;
        public float altCutoff_red = 0.2f;
        public float altCutoff_green = 0.6f;

    }
}
