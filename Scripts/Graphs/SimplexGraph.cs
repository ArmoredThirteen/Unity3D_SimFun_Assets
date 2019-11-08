using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SimplexGraph : MonoBehaviour
{
    public GameObject objPrefab;

    public float objectSpacing = 1.0f;

    public int   width  = 50;
    public int   height = 50;
    public float noiseScale  = 500.0f;
    public float heightScale = 1.0f;


    [ContextMenu ("Re-Create Graph")]
    public void ReCreateGraph()
    {
        Debug.Log ("Clear Graph");
        this.ClearGraph ();

        Debug.Log ("Create Graph");
        CreateGraph ();
    }

    [ContextMenu ("Clear Object Graph")]
    public void ClearGraph()
    {
        while (transform.childCount > 0)
        {
            GameObject.DestroyImmediate (transform.GetChild (0).gameObject);
        }
    }

    [ContextMenu ("Create Graph")]
    public void CreateGraph()
    {
        float[,] simplexGraph = SimplexNoise.Noise.Calc2D (this.width, this.height, this.noiseScale);

        for (int x = 0; x < this.width; x++)
        {
            for (int z = 0; z < this.height; z++)
            {
                float  y          = simplexGraph[x, z] * this.heightScale;
                string prefabName = "x" + x + ", z" + z + ", y" + y;
                
                GameObject newObj = InstantiatePrefab (prefabName, objPrefab, this.gameObject, new Vector3 (x, y, z));
                //newObj.transform.localScale = new Vector3 (1, y*this.heightScale, 1);
            }
        }
    }

    private GameObject InstantiatePrefab(string name, GameObject prefab, GameObject parentObj, Vector3 pos)
    {
        GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab (prefab);

        newObj.name = name;
        newObj.transform.parent = parentObj.transform;
        newObj.transform.position = new Vector3 (pos.x, pos.y, pos.z);

        return newObj;
    }

}
