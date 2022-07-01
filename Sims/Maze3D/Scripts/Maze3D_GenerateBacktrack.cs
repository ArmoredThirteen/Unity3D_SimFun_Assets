using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace ATE
{
	public class Maze3D_GenerateBacktrack : ATEObject
	{
		public Maze3D maze;
		public Cell3D startCell;
		public Cell3D endCell;

		public int seed = 0;


		[ContextMenu("Reset Maze")]
		public void ResetMaze()
        {
			maze.GatherChildrenAsCells();
			maze.ResetCellWalls();
			maze.ResetCellSolveIndicators();

			EditorUtility.SetDirty(maze);
        }

		[ContextMenu("Generate and Solve Maze")]
		public void GenerateAndSolveMaze()
        {
			GenerateMaze();
			PrintSolveInfo();
        }

		[ContextMenu("Generate Maze")]
		public void GenerateMaze()
        {
			int realSeed = seed != 0 ? seed : System.DateTime.Now.Second;
			Debug.Log("Generating with seed: " + realSeed);
			Random.InitState(realSeed);
			
			ResetMaze();

			Stack<Vector3Int> crawler = new Stack<Vector3Int>();
			List<Vector3Int> visited = new List<Vector3Int>();

			Vector3Int startLoc = startCell.GetGraphLocation(maze.graphToWorldScale);
			crawler.Push(startLoc);
			visited.Add(startLoc);
			
			while (crawler.Count > 0)
            {
				Vector3Int currLoc = crawler.Pop();

				List<Vector3Int> neighbors = GetUnvisitedNeighbors(visited, currLoc);

				if (neighbors.Count <= 0)
					continue;

				Vector3Int nextLoc = neighbors[Random.Range(0, neighbors.Count)];

				crawler.Push(currLoc);
				crawler.Push(nextLoc);
				visited.Add(nextLoc);

				Cell3D currCell = maze.GetCellAt(currLoc);
				Cell3D nextCell = maze.GetCellAt(nextLoc);
				BreakWallBetween(currCell, nextCell);
			}

			EditorUtility.SetDirty(maze);
		}

		[ContextMenu("Print Solve Info")]
		public void PrintSolveInfo()
        {
			maze.GatherChildrenAsCells();
			maze.ResetCellSolveIndicators();

			Stack<Vector3Int> crawler = new Stack<Vector3Int>();
			List<Vector3Int> visited = new List<Vector3Int>();

			Vector3Int startLoc = startCell.GetGraphLocation(maze.graphToWorldScale);
			Vector3Int endLoc = endCell.GetGraphLocation(maze.graphToWorldScale);

			crawler.Push(startLoc);
			visited.Add(startLoc);

			while (crawler.Count > 0)
			{
				if (crawler.Peek() == endLoc)
					break;

				Vector3Int currLoc = crawler.Pop();

				List<Vector3Int> neighbors = GetUnvisitedPathableToNeighbors(visited, currLoc);

				if (neighbors.Count <= 0)
					continue;

				Vector3Int nextLoc = neighbors[Random.Range(0, neighbors.Count)];

				crawler.Push(currLoc);
				crawler.Push(nextLoc);
				visited.Add(nextLoc);

				Cell3D currCell = maze.GetCellAt(currLoc);
				Cell3D nextCell = maze.GetCellAt(nextLoc);
			}

			if (crawler.Count <= 0)
            {
				Debug.Log("No solution found");
				return;
            }

			foreach(Vector3Int solveLoc in crawler)
				maze.GetCellAt(solveLoc).SetSolveIndicator(true);

			Debug.Log("Steps to end: " + crawler.Count);

			EditorUtility.SetDirty(maze);
		}


		private List<Vector3Int> GetUnvisitedNeighbors(List<Vector3Int> visited, Vector3Int cellLoc)
        {
			List<Vector3Int> cells = maze.GetNeighborLocs(cellLoc);
			cells.RemoveAll(c => visited.Contains(c));
			return cells;
		}

		private List<Vector3Int> GetUnvisitedPathableToNeighbors(List<Vector3Int> visited, Vector3Int cellLoc)
		{
			List<Vector3Int> cells = GetUnvisitedNeighbors(visited, cellLoc);
			cells.RemoveAll(c => !HasPathBetween(maze.GetCellAt(cellLoc), maze.GetCellAt(c)));
			return cells;
        }


		private void BreakWallBetween(Cell3D cellA, Cell3D cellB)
        {
			BreakWallOneWay(cellA, cellB);
			BreakWallOneWay(cellB, cellA);
		}

		// Potentially breaks a wall on cellA depending on where cellB is relatively
		private void BreakWallOneWay(Cell3D cellA, Cell3D cellB)
        {
			Vector3 posA = cellA.transform.position;
			Vector3 posB = cellB.transform.position;

			// Break wall on cellA
			if (posA.x > posB.x)
				cellA.xWall.SetWall(false);
			if (posA.y > posB.y)
				cellA.yWall.SetWall(false);
			if (posA.z > posB.z)
				cellA.zWall.SetWall(false);
		}


		private bool HasPathBetween (Cell3D cellA, Cell3D cellB)
        {
			return HasPathOneWay(cellA, cellB) || HasPathOneWay(cellB, cellA);
        }

		private bool HasPathOneWay(Cell3D cellA, Cell3D cellB)
        {
			Vector3 posA = cellA.transform.position;
			Vector3 posB = cellB.transform.position;

			// Check wall on cellA
			if (posA.x > posB.x)
				return !cellA.xWall.walled;
			if (posA.y > posB.y)
				return !cellA.yWall.walled;
			if (posA.z > posB.z)
				return !cellA.zWall.walled;

			return false;
        }
		
	}
}
