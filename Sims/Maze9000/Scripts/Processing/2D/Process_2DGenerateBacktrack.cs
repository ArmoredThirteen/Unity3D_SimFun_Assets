//#define LOG_DEBUG


using MazeGen.CellStructures;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;


namespace MazeGen.Processing
{
    public class Process_2DGenerateBacktrack : Process
    {
        int currCellX;
        int currCellY;

        CellArray2D_BitArray map;
        Stack<MoveDirs2D> previousPath;
        CellArray2D_BitArray isVisited;
        Random rand;
        MoveDirs2D[] dirs;


        public int GetCurrCellX()
        {
            return currCellX;
        }

        public int GetCurrCellY()
        {
            return currCellY;
        }


        public Process_2DGenerateBacktrack(CellArray2D_BitArray theMap, int startCellX, int startCellY)
        {
            currCellX = startCellX;
            currCellY = startCellY;

            map = theMap;
            previousPath = new Stack<MoveDirs2D> ();
            isVisited = new CellArray2D_BitArray (theMap.sizeX, theMap.sizeY, 1);
            rand = new Random (0);
            dirs = new MoveDirs2D[] { MoveDirs2D.UpX, MoveDirs2D.DownX, MoveDirs2D.UpY, MoveDirs2D.DownY };
        }


        public override bool RunProcess()
        {
            bool moved = false;
            isVisited[currCellX, currCellY, 0] = true;

            // Set the order to check for moving forward
            //TODO: Maybe have different ways to randomize the dirs
            //      Such as weighting toward straight movement, left turns, etc
            rand.Shuffle (dirs);

            // Attempt to move forward
            for (int i = 0; i < dirs.Length; i++)
            {
                // If valid forward move
                if (CanMoveInDir (dirs[i], map, isVisited))
                {
                    // Move forward
#if LOG_DEBUG
                    Debug.Log ("Moving forward with dir: " + dirs[i].ToString ());
#endif
                    MoveInDir (dirs[i], map);
                    previousPath.Push (dirs[i]);
                    moved = true;
                    break;
                }
            }

            // Could not move forward, attempt to move backward
            if (!moved && previousPath.Count > 0)
            {
#if LOG_DEBUG
                Debug.Log ("Moving backward with dir opposite of: " + previousPath.Peek ().ToString ());
#endif
                MoveInReverseDir (previousPath.Pop ());
                moved = true;
            }

            return moved;
        }


        // Returns false if moveDir goes out of bounds or toward a visited cell
        private bool CanMoveInDir(MoveDirs2D moveDir, CellArray2D_BitArray theMap, CellArray2D_BitArray isVisited)
        {
            switch (moveDir)
            {
                case MoveDirs2D.UpX:
                    return !(currCellX + 1 >= theMap.sizeX)
                        && !(isVisited[currCellX + 1, currCellY, 0]);
                case MoveDirs2D.DownX:
                    return !(currCellX - 1 < 0)
                        && !(isVisited[currCellX - 1, currCellY, 0]);
                case MoveDirs2D.UpY:
                    return !(currCellY + 1 >= theMap.sizeY)
                        && !(isVisited[currCellX, currCellY + 1, 0]);
                case MoveDirs2D.DownY:
                    return !(currCellY - 1 < 0)
                        && !(isVisited[currCellX, currCellY - 1, 0]);
            }

            return false;
        }

        // Move in the specified direction
        private void MoveInDir(MoveDirs2D moveDir, CellArray2D_BitArray theMap)
        {
            // Setting theMap[] vals to true when wall is broken down
            switch (moveDir)
            {
                case MoveDirs2D.UpX:
                    currCellX += 1;
                    theMap[currCellX, currCellY, 0] = true;
                    break;
                case MoveDirs2D.DownX:
                    theMap[currCellX, currCellY, 0] = true;
                    currCellX -= 1;
                    break;
                case MoveDirs2D.UpY:
                    currCellY += 1;
                    theMap[currCellX, currCellY, 1] = true;
                    break;
                case MoveDirs2D.DownY:
                    theMap[currCellX, currCellY, 1] = true;
                    currCellY -= 1;
                    break;
            }
        }


        // Move in the opposite direction as the previous direction of movement
        private void MoveInReverseDir(MoveDirs2D previousDir)
        {
            switch (previousDir)
            {
                case MoveDirs2D.UpX:
                    currCellX--;
                    break;
                case MoveDirs2D.DownX:
                    currCellX++;
                    break;
                case MoveDirs2D.UpY:
                    currCellY--;
                    break;
                case MoveDirs2D.DownY:
                    currCellY++;
                    break;
            }
        }

    }
}
