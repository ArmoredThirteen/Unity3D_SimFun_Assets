using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ATE
{
	public class Cell3D : ATEObject
	{
		[System.Serializable]
		public class CellWall
        {
			public bool walled;
			public GameObject wall;

			public void SetWall(bool setToWalled)
            {
				walled = setToWalled;
				wall.gameObject.SetActive(walled);
            }
        }


		public CellWall xWall;
		public CellWall yWall;
		public CellWall zWall;


		public Vector3Int GetGraphLocation(Vector3 graphToWorldScale)
        {
			float xPos = transform.position.x / graphToWorldScale.x;
			float yPos = transform.position.y / graphToWorldScale.y;
			float zPos = transform.position.z / graphToWorldScale.z;

			return Vector3Int.RoundToInt(new Vector3(xPos, yPos, zPos));
		}


		public void SetWalls(bool setToWalled)
        {
			xWall.SetWall(setToWalled);
			yWall.SetWall(setToWalled);
			zWall.SetWall(setToWalled);
		}


	}
}
