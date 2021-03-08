﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ATE.SortBots
{
	public class ArenaBuilder : ATEObject
	{
        public GameObject floor;
        public GameObject wallXPos;
        public GameObject wallXNeg;
        public GameObject wallZPos;
        public GameObject wallZNeg;

        public SortBot botPrefab;
        public List<BotBox> boxPrefabs;

        public float cellSpacing = 1;
        public int cellsX = 20;
        public int cellsZ = 20;

        public int botsCount = 2;
        public int boxesCount = 15;
        

        private void Awake()
        {
            GenerateArena();
        }

        void OnDrawGizmos()
        {
            float startX = transform.position.x - ((cellsX - 1) * cellSpacing / 2.0f);
            float startZ = transform.position.z - ((cellsZ - 1) * cellSpacing / 2.0f);

            Gizmos.color = Color.yellow;
            for (int x = 0; x < cellsX; x++)
                for(int z = 0; z < cellsZ; z++)
                {
                    float xPos = startX + (x * cellSpacing);
                    float zPos = startZ + (z * cellSpacing);
                    Gizmos.DrawSphere(new Vector3(xPos, 0, zPos), 0.5f);
                }
        }


        public void GenerateArena()
        {
            float startX = transform.position.x - ((cellsX - 1) * cellSpacing / 2.0f);
            float startZ = transform.position.z - ((cellsZ - 1) * cellSpacing / 2.0f);

            bool[,] hasObj = new bool[cellsX, cellsZ];

            // Place bots
            for (int i = 0; i < botsCount; i++)
            {
                // Find open space
                Vector2Int cellPos = GetNextEmpty(hasObj);
                hasObj[cellPos.x, cellPos.y] = true;

                // Translate open space location into world space
                float xPos = startX + (cellPos.x * cellSpacing);
                float zPos = startZ + (cellPos.y * cellSpacing);

                // Instantiate and place object
                SortBot newBot = GameObject.Instantiate<SortBot>(botPrefab, transform);
                newBot.transform.position = new Vector3(xPos, 0, zPos);
                newBot.transform.Rotate(new Vector3(0, Random.Range(0, 360), 0));
            }

            // Place boxes
            for (int i = 0; i < boxesCount; i++)
            {
                // Find open space
                Vector2Int cellPos = GetNextEmpty(hasObj);
                hasObj[cellPos.x, cellPos.y] = true;

                // Translate open space location into world space
                float xPos = startX + (cellPos.x * cellSpacing);
                float zPos = startZ + (cellPos.y * cellSpacing);

                // Instantiate and place object
                BotBox boxPrefab = boxPrefabs[Random.Range(0, boxPrefabs.Count)];
                BotBox newBox = GameObject.Instantiate<BotBox>(boxPrefab, transform);
                newBox.transform.position = new Vector3(xPos, 0.25f, zPos);
            }
        }

        private void InstantiateAt(GameObject prefab, float xPos, float yPos, float zPos)
        {
            
        }

        private Vector2Int GetNextEmpty(bool[,] hasObj)
        {
            int x = Random.Range(0, cellsX);
            int z = Random.Range(0, cellsZ);
            while (hasObj[x, z])
            {
                x = Random.Range(0, cellsX);
                z = Random.Range(0, cellsZ);
            }

            return new Vector2Int(x, z);
        }

    }
}
