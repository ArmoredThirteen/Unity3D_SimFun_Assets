using System;
using UnityEngine;


namespace ATE
{
	static class Vector3Extensions
	{
        public static Vector3 Xof(this Vector3 vect, int val)
        {
            return new Vector3(val, vect.y, vect.z);
        }

        public static Vector3 Yof(this Vector3 vect, int val)
        {
            return new Vector3(vect.x, val, vect.z);
        }

        public static Vector3 Zof(this Vector3 vect, int val)
        {
            return new Vector3(vect.x, vect.y, val);
        }

    }
}
