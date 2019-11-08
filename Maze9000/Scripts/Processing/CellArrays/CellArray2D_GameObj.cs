using System.Collections;
using System.Text;
using UnityEngine;


namespace MazeGen.CellStructures
{
    public class CellArray2D_GameObj : CellArray2D<GameObject[], GameObject>
    {
        public CellArray2D_GameObj(int theXSize, int theYSize, int theValuesPerCell) : base (theXSize, theYSize, theValuesPerCell)
        {

        }


        protected override void InitializeMap(int arraySize)
        {
            this.map = new GameObject[this.fullMapSize];
        }

        protected override GameObject GetCellValue(int cellValIndex)
        {
            return this.map[cellValIndex];
        }

        protected override void SetCellValue(int cellValIndex, GameObject cellVal)
        {
            this.map[cellValIndex] = cellVal;
        }

    }
}
