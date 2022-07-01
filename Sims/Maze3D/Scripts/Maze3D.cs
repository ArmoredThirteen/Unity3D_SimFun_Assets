using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace ATE
{
	public class Maze3D : ATEObject
	{
		public Vector3 graphToWorldScale = Vector3.one;


		private Dictionary<Vector3Int, Cell3D> cells;


		public Cell3D GetCellAt(Vector3Int loc)
        {
			return cells[loc];
        }

		public Cell3D GetCellAt(int x, int y, int z)
        {
			return GetCellAt(new Vector3Int(x, y, z));
        }



		public List<Vector3Int> GetNeighborLocs(Vector3Int loc)
		{
			List<Vector3Int> neighbors = new List<Vector3Int>();

			List<Vector3Int> mods = new List<Vector3Int>();
			mods.Add(new Vector3Int(1, 0, 0));
			mods.Add(new Vector3Int(-1, 0, 0));
			mods.Add(new Vector3Int(0, 1, 0));
			mods.Add(new Vector3Int(0, -1, 0));
			mods.Add(new Vector3Int(0, 0, 1));
			mods.Add(new Vector3Int(0, 0, -1));

			for (int i = 0; i < mods.Count; i++)
			{
				Vector3Int neighborLoc = loc + mods[i];
				if (cells.ContainsKey(neighborLoc))
					neighbors.Add(neighborLoc);
			}

			return neighbors;
		}


		public void GatherChildrenAsCells()
        {
			cells = new Dictionary<Vector3Int, Cell3D>();

			//TODO: This 'world position as dictionary key and graph position' is making things hacky
			Cell3D[] childrenCells = gameObject.GetComponentsInChildren<Cell3D>();
			for (int i = 0; i < childrenCells.Length; i++)
				cells.Add(childrenCells[i].GetGraphLocation(graphToWorldScale), childrenCells[i]);
        }

		public void ResetCellWalls()
		{
			List<Cell3D> cellValues = cells.Values.ToList();
			for (int i = 0; i < cellValues.Count; i++)
				cellValues[i].SetWalls(true);
		}

		public void ResetCellSolveIndicators()
		{
			List<Cell3D> cellValues = cells.Values.ToList();
			for (int i = 0; i < cellValues.Count; i++)
				cellValues[i].SetSolveIndicator(false);
		}

	}
}
