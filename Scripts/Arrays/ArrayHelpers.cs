﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ATE
{
	public static class ArrayHelpers
	{
        public static float GetHighest(float[,] theAra)
        {
            float returnVal = float.MinValue;

            for (int y = 0; y < theAra.GetLength(0); y++)
                for (int x = 0; x < theAra.GetLength(1); x++)
                    if (theAra[y, x] > returnVal)
                        returnVal = theAra[y, x];

            return returnVal;
        }

        public static float GetLowest(float[,] theAra)
        {
            float returnVal = float.MaxValue;

            for (int y = 0; y < theAra.GetLength(0); y++)
                for (int x = 0; x < theAra.GetLength(1); x++)
                    if (theAra[y, x] < returnVal)
                        returnVal = theAra[y, x];

            return returnVal;
        }

        public static float GetAverageOfNonZero(float[,] theAra)
        {
            float returnVal = 0;
            int valsFound = 0;

            for (int y = 0; y < theAra.GetLength(0); y++)
                for (int x = 0; x < theAra.GetLength(1); x++)
                    if (theAra[y, x] > 0)
                    {
                        returnVal += theAra[y, x];
                        valsFound++;
                    }


            return returnVal / valsFound;
        }


        public static Vector2 GetRandomIndex(float[,] theAra)
        {
            int xPos = Random.Range(0, theAra.GetLength(1) - 1);
            int yPos = Random.Range(0, theAra.GetLength(0) - 1);
            return new Vector2(xPos, yPos);
        }

		public static void NormalizeArray2D(float[,] theAra, float floor, float ceiling)
        {
            float minVal = float.MaxValue;
            float maxVal = float.MinValue;

            int xLen = theAra.GetLength(1);
            int yLen = theAra.GetLength(0);

            // Get the min and max values
            for (int y = 0; y < yLen; y++)
                for (int x = 0; x < xLen; x++)
                {
                    minVal = Mathf.Min(minVal, theAra[y, x]);
                    maxVal = Mathf.Max(maxVal, theAra[y, x]);
                }

            // Normalize everything
            float mult = 1 / (maxVal - minVal);
            for (int y = 0; y < yLen; y++)
                for (int x = 0; x < xLen; x++)
                {
                    theAra[y, x] = (theAra[y, x] - minVal) * mult;
                }
        }
		
	}
}
