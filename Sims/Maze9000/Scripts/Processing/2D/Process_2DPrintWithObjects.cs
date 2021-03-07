using MazeGen.CellStructures;
using UnityEngine;


namespace MazeGen.Processing
{
    public class Process_2DPrintWithObjects : Process
    {
        CellArray2D_BitArray map;

        GameObject parentObj;
        GameObject printObj;

        float spacing;


        public Process_2DPrintWithObjects(CellArray2D_BitArray theMap, GameObject theParentObj, GameObject thePrintObj, float theSpacing)
        {
            map = theMap;

            parentObj = theParentObj;
            printObj = thePrintObj;

            spacing = theSpacing;
        }


        public override bool RunProcess()
        {
            PrintBorders (map);

            for (int x = 0; x < map.sizeX; x++)
            {
                for (int y = 0; y < map.sizeY; y++)
                {
                    PrintCell (map, x, y);
                }
            }

            return true;
        }

        private void PrintBorders(CellArray2D_BitArray theMap)
        {
            int xCount = theMap.sizeX * 2;
            int yCount = theMap.sizeY * 2;

            // UpX Border, +1 to cover corner in UpX,UpY (upper right, last cell)
            for (int x = 0; x < xCount + 1; x++)
            {
                GameObject xBorder = GameObject.Instantiate (printObj, parentObj.transform);
                xBorder.transform.localPosition = new Vector3 (x * spacing, yCount * spacing);
            }

            // UpY Border
            for (int y = 0; y < yCount; y++)
            {
                GameObject xBorder = GameObject.Instantiate (printObj, parentObj.transform);
                xBorder.transform.localPosition = new Vector3 (xCount * spacing, y * spacing);
            }
        }

        private void PrintCell(CellArray2D_BitArray theMap, int x, int y)
        {
            float cellLocX = x * spacing * 2;
            float cellLocY = y * spacing * 2;

            // Instantiate cell's game objects
            GameObject solidCorner = GameObject.Instantiate (printObj, parentObj.transform);
            solidCorner.transform.localPosition = new Vector3 (cellLocX, cellLocY);

            if (!theMap[x, y, 0])
            {
                GameObject firstCorner = GameObject.Instantiate (printObj, parentObj.transform);
                firstCorner.transform.localPosition = new Vector3 (cellLocX, cellLocY + spacing);
            }

            if (!theMap[x, y, 1])
            {
                GameObject secondCorner = GameObject.Instantiate (printObj, parentObj.transform);
                secondCorner.transform.localPosition = new Vector3 (cellLocX + spacing, cellLocY);
            }
        }

    }
}
