using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ATE
{
	public static class ArrayHelpers
	{
		public static void NormalizeArray2D(float[,] theAra, float floor, float ceiling)
        {
            float minVal = float.MaxValue;
            float maxVal = float.MinValue;

            // Get the min and max values
            for (int y = 0; y < theAra.GetLength(0); y++)
                for (int x = 0; x < theAra.GetLength(1); x++)
                {
                    minVal = Mathf.Min(minVal, theAra[y, x]);
                    maxVal = Mathf.Max(maxVal, theAra[y, x]);
                }

            // Normalize everything
            float mult = 1 / (maxVal - minVal);
            for (int y = 0; y < theAra.GetLength(0); y++)
                for (int x = 0; x < theAra.GetLength(1); x++)
                {
                    theAra[y, x] = (theAra[y, x] - minVal) * mult;
                }
        }
		
	}
}
