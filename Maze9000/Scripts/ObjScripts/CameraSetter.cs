using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CameraSetter : MonoBehaviour
{
    public Camera centeredCam;
    public MazeGen.MazeGeneratorObject mazeGenObj;

    public Vector3 camOffset;
    public float orthoSizeScale = 0.5f;
    public float orthoSizeAdd = 1;


    void Update()
    {
        Vector3 camCenter = mazeGenObj.transform.position + new Vector3 ((mazeGenObj.xSize * mazeGenObj.cellSpacing), (mazeGenObj.ySize * mazeGenObj.cellSpacing));
        float orthoSize = Mathf.Max ((mazeGenObj.xSize * mazeGenObj.cellSpacing * 2) + mazeGenObj.cellSpacing, (mazeGenObj.ySize * mazeGenObj.cellSpacing * 2) + mazeGenObj.cellSpacing);

        centeredCam.transform.position = camCenter + camOffset;
        centeredCam.orthographicSize = (orthoSize * orthoSizeScale) + orthoSizeAdd;
    }
}
