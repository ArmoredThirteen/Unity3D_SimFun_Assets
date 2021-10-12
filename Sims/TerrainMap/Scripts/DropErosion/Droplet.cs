using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ATE.TerrainGen
{
	public class Droplet
	{
        public LiquidSettings settings;

        public float xPos;
        public float yPos;

        public float volume;
        public float dirt;

        public int lifetime;


        public Droplet(LiquidSettings theSettings, float startXPos, float startYPos, float startVolume, float startDirt)
        {
            xPos = startXPos;
            yPos = startYPos;
            settings = theSettings;
            volume = startVolume;
            dirt = startDirt;
        }

    }
}
