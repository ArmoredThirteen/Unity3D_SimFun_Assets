//#define LOG_DEBUG
#define STOPWATCH


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MazeGen.Processing;
using MazeGen.CellStructures;
using System.Diagnostics;
using Process = MazeGen.Processing.Process;
using Debug = UnityEngine.Debug;


namespace MazeGen
{
    public class MazeGeneratorObject : MonoBehaviour
    {
        public bool shouldUpdate = true;
        public bool printMaze = true;
        public bool generateOnStartup = true;
        public int stepsPerUpdate = 1;
        public float timeBetweenUpdates = 0;

        public int xSize;
        public int ySize;

        public float cellSpacing;
        public GameObject cellValParentObj;
        public GameObject currentLocationParentObj;
        public GameObject cellValPrintPrefab;
        public GameObject currentLocationPrefab;


#if STOPWATCH
        private Stopwatch watch = new Stopwatch();
#endif
        private float timer_nextUpdate;

        private CellArray2D_BitArray mazeMap;

        private Process generateProcess;
        private Process_2DPrintByCurrCell printProcess;
        private bool generateCouldStep;


        // Start is called before the first frame update
        void Start()
        {
            timer_nextUpdate = 0;

            mazeMap = new CellArray2D_BitArray (xSize, ySize, 2);
            generateProcess = new Process_2DGenerateBacktrack (mazeMap, 0, 0);
            printProcess = new Process_2DPrintByCurrCell (mazeMap, (Process_2DGenerateBacktrack)generateProcess, cellSpacing, cellValParentObj, currentLocationParentObj, cellValPrintPrefab, currentLocationPrefab);

            generateCouldStep = !generateOnStartup;

            if (generateOnStartup)
            {
                generateProcess.RunWhileHasSteps ();
                RePrintMapObjects ();
#if LOG_DEBUG
                Debug.Log (mazeMap.ToString ());
#endif
            }
            else
            {
                if (printMaze)
                    printProcess.FirstPrint ();
            }
        }

        private void Update()
        {
            if (!shouldUpdate)
                return;
            if (!generateCouldStep)
                return;

#if STOPWATCH
            if (!watch.IsRunning)
                watch.Start ();
#endif

            timer_nextUpdate += Time.deltaTime;
            if (timer_nextUpdate < timeBetweenUpdates)
                return;
            timer_nextUpdate -= timeBetweenUpdates;

            for (int i = 0; i < stepsPerUpdate; i++)
            {
                if (printMaze)
                    printProcess.RunProcess ();

                generateCouldStep = generateCouldStep && generateProcess.RunProcess ();
                
                if (!generateCouldStep)
                {
#if LOG_DEBUG
                    Debug.Log (mazeMap.ToString ());
#endif
#if STOPWATCH
                    Debug.Log ($"Execution Time: {watch.ElapsedMilliseconds} ms");
#endif
                    mazeMap = null;
                    break;
                }
            }
        }


        private void RePrintMapObjects()
        {
            foreach (Transform child in this.gameObject.transform)
            {
                GameObject.Destroy (child.gameObject);
            }

            Process_2DPrintWithObjects printWithObjects = new Process_2DPrintWithObjects (mazeMap, this.gameObject, cellValPrintPrefab, cellSpacing);
            printWithObjects.RunProcess ();
        }

    }
}
