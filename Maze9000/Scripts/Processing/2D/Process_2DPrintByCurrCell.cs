//#define USE_DESTROY


using MazeGen.CellStructures;
using UnityEngine;


namespace MazeGen.Processing
{
    public class Process_2DPrintByCurrCell : Process
    {
        CellArray2D_BitArray inGenMap;
        CellArray2D_GameObj objMap;
        Process_2DGenerateBacktrack currCellGen;

        float spacing;

        GameObject wallParentObj;
        GameObject currLocParentObj;
        GameObject printPrefab;
        GameObject locPrefab;

        GameObject currLocObj;


        public Process_2DPrintByCurrCell(CellArray2D_BitArray theInGenMap, Process_2DGenerateBacktrack theCurrCellGen, float theSpacing, GameObject theWallParentObj, GameObject theCurrLocParentObj, GameObject thePrintPrefab, GameObject theLocPrefab)
        {
            inGenMap = theInGenMap;
            objMap = new CellArray2D_GameObj (theInGenMap.sizeX, theInGenMap.sizeY, 2);
            currCellGen = theCurrCellGen;

            spacing = theSpacing;

            wallParentObj = theWallParentObj;
            currLocParentObj = theCurrLocParentObj;
            printPrefab = thePrintPrefab;
            locPrefab = theLocPrefab;
        }


        int currX;
        int currY;

        public override bool RunProcess()
        {
            // Print the now previous cell
            // Without this, walls don't get deleted correctly when moving negatively along an axis
            PrintCellWalls (currX, currY);
            
            currX = currCellGen.GetCurrCellX ();
            currY = currCellGen.GetCurrCellY ();

            // Print the current cell
            PrintCellWalls (currX, currY);
            PlaceLocationPrefab (currX, currY);

            return true;
        }


        // Prints the borders and the cell starting states
        public void FirstPrint()
        {
            PrintBorders ();

            for (int x = 0; x < inGenMap.sizeX; x++)
            {
                for (int y = 0; y < inGenMap.sizeY; y++)
                {
                    // Print cell center block
                    GameObject solidCorner = GameObject.Instantiate (printPrefab, wallParentObj.transform);
                    solidCorner.transform.localPosition = new Vector3 (x * 2 * spacing, y * 2 * spacing);

                    // Instantiate wall object map
                    objMap[x, y, 0] = GameObject.Instantiate (printPrefab, wallParentObj.transform);
                    objMap[x, y, 1] = GameObject.Instantiate (printPrefab, wallParentObj.transform);
                }
            }

            currLocObj = GameObject.Instantiate (locPrefab, currLocParentObj.transform);
            currLocObj.transform.localPosition = new Vector3 (0, 0);
            PlaceLocationPrefab (0, 0);
        }


        private void PrintBorders()
        {
            int xCount = inGenMap.sizeX * 2;
            int yCount = inGenMap.sizeY * 2;

            // UpX Border, +1 to cover corner in UpX,UpY (upper right, last cell)
            for (int x = 0; x < xCount + 1; x++)
            {
                GameObject xBorder = GameObject.Instantiate (printPrefab, wallParentObj.transform);
                xBorder.transform.localPosition = new Vector3 (x * spacing, yCount * spacing);
            }

            // UpY Border
            for (int y = 0; y < yCount; y++)
            {
                GameObject xBorder = GameObject.Instantiate (printPrefab, wallParentObj.transform);
                xBorder.transform.localPosition = new Vector3 (xCount * spacing, y * spacing);
            }
        }


        private void PlaceLocationPrefab(int x, int y)
        {
            currLocObj.transform.localPosition = new Vector3 ((x * 2 * spacing) + spacing, (y * 2 * spacing) + spacing);
        }
        

        private void PrintCellWalls(int x, int y)
        {
            float wallLocX = x * spacing * 2;
            float wallLocY = y * spacing * 2;

            PrintWall (x, y, 0, wallLocX, wallLocY + spacing);
            PrintWall (x, y, 1, wallLocX + spacing, wallLocY);
        }

        private void PrintWall(int x, int y, int wallI, float wallLocX, float wallLocY)
        {
#if USE_DESTROY
            GameObject.Destroy (objMap[x, y, wallI]);
#else
            //Vector3 newPos = new Vector3 (10000, 10000, 10000);
#endif
            if (!inGenMap[x, y, wallI])
            {
#if USE_DESTROY
                GameObject newObj = GameObject.Instantiate (printPrefab, wallParentObj.transform);
                newObj.transform.localPosition = new Vector3 (wallLocX, wallLocY);
                objMap[x, y, wallI] = newObj;
#else
                //newPos = new Vector3 (wallLocX, wallLocY);
                objMap[x, y, wallI].transform.localPosition = new Vector3 (wallLocX, wallLocY);
#endif
            }
            else
            {
                objMap[x, y, wallI].transform.localPosition = new Vector3 (10000, 10000, 10000);
            }

            //objMap[x, y, wallI].transform.position = newPos;
        }

    }
}
