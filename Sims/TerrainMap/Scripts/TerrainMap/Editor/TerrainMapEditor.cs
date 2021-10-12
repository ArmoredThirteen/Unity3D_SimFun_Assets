using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace ATE.TerrainGen
{
    [CustomEditor(typeof(TerrainMap))]
    public class TerrainMapEditor : Editor
	{
        private TerrainMap targ;


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            targ = (TerrainMap)target;

            EditorGUILayout.Space(20);


            EditorGUILayout.LabelField("Generate:");

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Full"))
                targ.GenerateFull();

            if (GUILayout.Button("Texture"))
                targ.GenTexture();

            EditorGUILayout.EndHorizontal();
        }

    }
}
