using System.Collections;
using System.Text;


namespace MazeGen.CellStructures
{
    public class CellArray2D_BitArray : CellArray2D<BitArray, bool>
    {
        public CellArray2D_BitArray(int theXSize, int theYSize, int theValuesPerCell) : base (theXSize, theYSize, theValuesPerCell)
        {

        }


        protected override void InitializeMap(int arraySize)
        {
            map = new BitArray (arraySize);
        }

        protected override bool GetCellValue(int cellValIndex)
        {
            return map[cellValIndex];
        }

        protected override void SetCellValue(int cellValIndex, bool cellVal)
        {
            map[cellValIndex] = cellVal;
        }


        public override string ToString()
        {
            StringBuilder theStr = new StringBuilder ();

            for (int i = 0; i < map.Count; i++)
            {
                theStr.Append (map[i] ? "1" : "0");
                if (i % valuesPerCell == 1)
                    theStr.Append (" ");
            }

            return theStr.ToString ();
        }

    }
}
