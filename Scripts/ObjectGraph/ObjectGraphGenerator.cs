using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectGraphGenerator : MonoBehaviour
{
    public enum Dimensions
    {
        x,
        y,
        z,
    }

    [System.Serializable]
    public class DimensionData
    {
        public Dimensions dimension   = Dimensions.x;
        public int        children    = 1;
        public float      posChange   = 1;

        public DimensionData(Dimensions theDimension, int theChildren, float thePosChange)
        {
            this.dimension = theDimension;
            this.children = theChildren;
            this.posChange = thePosChange;
        }
    }


    public GameObject leafObject;
    public GameObject parentObject;

    public List<DimensionData> dimensionDatas = new List<DimensionData> {  };
    

    [ContextMenu ("Re-Create Graph")]
    public void ReCreateGraph()
    {
        Debug.Log ("Clear Graph");
        this.ClearGraph ();

        Debug.Log ("Create Graph");
        CreateGraph (0, this.gameObject, this.transform.position);
    }

    [ContextMenu ("Clear Object Graph")]
    public void ClearGraph()
    {
        while (transform.childCount > 0)
        {
            GameObject.DestroyImmediate (transform.GetChild (0).gameObject);
        }
    }


    public void CreateGraph(int indexDimDats, GameObject currParent, Vector3 currPos)
    {
        // Leaf node: Create object then exit
        if (indexDimDats >= dimensionDatas.Count)
        {
            InstantiatePrefab (leafObject.name, leafObject, currParent, currPos);
            return;
        }

        // Get current data
        DimensionData curData = this.dimensionDatas[indexDimDats];

        // Get position change vector
        Vector3 posChange = Vector3.zero;
        switch (curData.dimension)
        {
            case Dimensions.x:
                posChange = new Vector3 (curData.posChange, 0, 0);
                break;
            case Dimensions.y:
                posChange = new Vector3 (0, curData.posChange, 0);
                break;
            case Dimensions.z:
                posChange = new Vector3 (0, 0, curData.posChange);
                break;
        }

        // Each new object in that dimension
        for (int i = 0; i < curData.children; i++)
        {
            // Create new parent for next dimension's objects
            GameObject dimensionParent = InstantiatePrefab (curData.dimension.ToString (), parentObject, currParent, currPos);

            // Continue down the next dimension
            CreateGraph (indexDimDats + 1, dimensionParent, currPos + (posChange * i));
        }
        
    }

    private GameObject InstantiatePrefab (string name, GameObject prefab, GameObject parentObj, Vector3 pos)
    {
        GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab (prefab);

        newObj.name = name;
        newObj.transform.parent = parentObj.transform;
        newObj.transform.position = new Vector3 (pos.x, pos.y, pos.z);
        
        return newObj;
    }

}
