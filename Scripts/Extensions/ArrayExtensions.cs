using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ATE
{
	public static class ArrayExtensions
	{
        public static void Zip(this float[,] ara, float[,] toZip, Func<float, float, float> action)
        {
            for (int y = 0; y < ara.GetLength(0); y++)
                for (int x = 0; x < ara.GetLength(1); x++)
                    ara[y, x] = action(ara[y, x], toZip[y, x]);
        }

    }
}
