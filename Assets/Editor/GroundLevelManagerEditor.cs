using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GroundLevelManager))]
public class GroundLevelManagerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		GroundLevelManager manager = (GroundLevelManager)target;

		DrawDefaultInspector();

		GUILayout.Space(10);
		GUILayout.Label("Load Level", EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("File Name:", GUILayout.Width(70));
		EditorGUILayout.EndHorizontal();



		GUILayout.Space(10);
		GUILayout.Label("Save Level", EditorStyles.boldLabel);

		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Randomize", GUILayout.Height(30)))
		{
			manager.GenerateGround();
		}

		if (GUILayout.Button("Load", GUILayout.Height(30)))
		{

		}

		if (GUILayout.Button("Clear", GUILayout.Height(30)))
		{

		}
		GUILayout.EndHorizontal();
	}
}
