using System.Collections;
using System.Text;


namespace MazeGen.CellStructures
{
    public abstract class CellArray2D<MapDataType, CellValType>
    {
        protected int valuesPerCell;

        protected int fullMapSize;
        protected MapDataType map;


        public CellArray2D(int theXSize, int theYSize, int theValuesPerCell)
        {
            sizeX = theXSize;
            sizeY = theYSize;
            valuesPerCell = theValuesPerCell;

            fullMapSize = sizeX * sizeY * theValuesPerCell;
            InitializeMap (fullMapSize);
        }


        public int sizeX
        {
            get;
            private set;
        }

        public int sizeY
        {
            get;
            private set;
        }


        protected abstract void InitializeMap(int arraySize);

        protected abstract CellValType GetCellValue(int cellValIndex);
        protected abstract void SetCellValue(int cellValIndex, CellValType cellVal);


        // Allows this[x,y] to get a cell's starting bit index
        private int this[int xIndex, int yIndex]
        {
            get
            {
                return (yIndex * sizeX * valuesPerCell) + (xIndex * valuesPerCell);
            }
        }

        public CellValType this[int xIndex, int yIndex, int valIndex]
        {
            get
            {
                int index = this[xIndex, yIndex] + valIndex;
                return GetCellValue (index);
            }
            set
            {
                int index = this[xIndex, yIndex] + valIndex;
                SetCellValue(index, value);
                //UnityEngine.Debug.Log ("Set Cell Val: " + xIndex + "," + yIndex + "," + valIndex + " -> " + value);
                //UnityEngine.Debug.Log (index);
            }
        }

    }
}
