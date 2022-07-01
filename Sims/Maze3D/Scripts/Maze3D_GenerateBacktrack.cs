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


		[ContextMenu("Reset Maze")]
		public void ResetMaze()
        {
			maze.GatherChildrenAsCells();
			maze.ResetCellWalls();

			EditorUtility.SetDirty(maze);
        }

		[ContextMenu("Generate Maze")]
		public void GenerateMaze()
        {
			ResetMaze();

			Stack<Vector3Int> crawler = new Stack<Vector3Int>();
			List<Vector3Int> visited = new List<Vector3Int>();

			Vector3Int startLoc = startCell.GetGraphLocation(maze.graphToWorldScale);
			crawler.Push(startLoc);
			visited.Add(startLoc);

			
			while (crawler.Count > 0)
            {
				Vector3Int currLoc = crawler.Pop();
				
				// All unvisited neighbors
				List<Vector3Int> neighbors = maze.GetNeighborLocs(currLoc);
				neighbors.RemoveAll(c => visited.Contains(c));

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
		
	}
}
