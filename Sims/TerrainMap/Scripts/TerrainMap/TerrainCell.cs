using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ATE.TerrainGen
{
    [System.Serializable]
	public class TerrainCell
	{
        //private Dictionary<TerrainValTypes, float> values;

        public float rock;
        public float dirt;
        public float eroded;

        
        public TerrainCell(float theRock)
        {
            /*values = new Dictionary<TerrainValTypes, float>();
            this[TerrainValTypes.Rock] = rock;
            this[TerrainValTypes.Dirt] = 0;
            this[TerrainValTypes.Eroded] = 0;*/

            this.rock = theRock;
            this.dirt = 0;
            this.eroded = 0;
        }


        public float GroundHeight
        {
            get { return Rock + Dirt; }
        }


        public float Rock
        {
            //get { return this[TerrainValTypes.Rock]; }
            //set { this[TerrainValTypes.Rock] = value; }
            get { return rock; }
            set { rock = value; }
        }

        public float Dirt
        {
            //get { return this[TerrainValTypes.Dirt]; }
            //set { this[TerrainValTypes.Dirt] = value; }
            get { return dirt; }
            set { dirt = value; }

        }

        public float Eroded
        {
            //get { return this[TerrainValTypes.Eroded]; }
            //set { this[TerrainValTypes.Eroded] = value; }
            get { return eroded; }
            set { eroded = value; }
        }


        /*public float GetValue(TerrainValTypes key)
        {
            return values[key];
        }

        public void SetValue(TerrainValTypes key, float value)
        {
            if (!values.ContainsKey(key))
                values.Add(key, value);
            else
                values[key] = value;
        }*/


        /*
         * Remove amount from dirt.
         * If not enough dirt, start removing from rock.
         */
        public void RemoveGround(float amount)
        {
            if (amount <= Dirt)
            {
                Dirt -= amount;
                return;
            }

            amount -= Dirt;
            Dirt = 0;

            Rock -= amount;
            Eroded += amount;
        }

        public void AddGround(float amount)
        {
            Dirt += amount;
            Eroded -= amount;
        }


        public float this[TerrainValTypes key]
        {
            get
            {
                if (key == TerrainValTypes.Rock)
                    return rock;
                else if (key == TerrainValTypes.Dirt)
                    return dirt;
                else
                    return eroded;
            }

            set
            {
                if (key == TerrainValTypes.Rock)
                    rock = value;
                else if (key == TerrainValTypes.Dirt)
                    dirt = value;
                else
                    eroded = value;
            }
        }

    }
}
