// This code is not being used. It is from a tutorial by Sebastian Lague

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;

        DrawDefaultInspector();

        if(GUILayout.Button("Generate"))
        {
            mapGen.GenerateMap();
        }
    }
}
