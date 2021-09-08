using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ATE.TerrainGen
{
	public class TerrainCell
	{
        private Dictionary<TerrainValTypes, float> values;

        
        public TerrainCell(float rock)
        {
            values = new Dictionary<TerrainValTypes, float>();
            this[TerrainValTypes.Rock] = rock;
            this[TerrainValTypes.Dirt] = 0;
        }


        public float Rock
        {
            get { return this[TerrainValTypes.Rock]; }
            set { this[TerrainValTypes.Rock] = value; }
        }

        public float Dirt
        {
            get { return this[TerrainValTypes.Dirt]; }
            set { this[TerrainValTypes.Dirt] = value; }
        }


        public float GetValue(TerrainValTypes key)
        {
            return values[key];
        }

        public void SetValue(TerrainValTypes key, float value)
        {
            if (!values.ContainsKey(key))
                values.Add(key, value);
            else
                values[key] = value;
        }


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
        }

        public void AddGround(float amount)
        {
            Dirt += amount;
        }


        public float this[TerrainValTypes key]
        {
            get => GetValue(key);
            set => SetValue(key, value);
        }

    }
}
