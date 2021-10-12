using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ATE
{
    [System.Serializable]
	public class Flat2dAra<T>
	{
        public int xLen;
        public int yLen;

        public T[] ara;

        
        public Flat2dAra(int theXLen, int theYLen)
        {
            xLen = theXLen;
            yLen = theYLen;

            ara = new T[xLen * yLen];
        }


        public T this[int y, int x]
        {
            get => ara[(y * xLen) + x];
            set => ara[(y * xLen) + x] = value;
        }

    }
}
