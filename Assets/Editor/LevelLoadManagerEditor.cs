using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelLoadManager))]
public class LevelLoadManagerEditor : Editor
{
	private string levelName = "levelData";

	public override void OnInspectorGUI()
	{
		LevelLoadManager manager = (LevelLoadManager)target;

		DrawDefaultInspector();

		GUILayout.Space(10);
		GUILayout.Label("Load Level", EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("File Name:", GUILayout.Width(70));
		levelName = EditorGUILayout.TextField(levelName);
		EditorGUILayout.EndHorizontal();



		GUILayout.Space(10);
		GUILayout.Label("Save Level", EditorStyles.boldLabel);

		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Save", GUILayout.Height(30)))
		{
			manager.SaveLevel();
		}

		if (GUILayout.Button("Load", GUILayout.Height(30)))
		{
			manager.LoadLevel(levelName + ".json");
		}

		if (GUILayout.Button("Clear", GUILayout.Height(30)))
		{
			manager.ClearLevel();
		}
		GUILayout.EndHorizontal();
	}
}
