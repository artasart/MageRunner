using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelLoadManager))]
public class LevelLoadManagerEditor : Editor
{
	private string levelName = "levelData";
	public int groundCount = 2;
	public int coinCount = 1;

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

		if (GUILayout.Button("Destroy", GUILayout.Height(30)))
		{
			manager.DestroyLevel();
		}
		GUILayout.EndHorizontal();

		// Ground ------------------------------------------------------------

		GUILayout.Space(40);
		GUILayout.Label("Ground", EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Count:", GUILayout.Width(70));
		groundCount = EditorGUILayout.IntField(groundCount);
		EditorGUILayout.EndHorizontal();


		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Geneate", GUILayout.Height(30)))
		{
			manager.GenerateGround(groundCount);
		}
		if (GUILayout.Button("Randomize", GUILayout.Height(30)))
		{
			manager.RandomizeGround();
		}
		if (GUILayout.Button("Destroy", GUILayout.Height(30)))
		{
			manager.DestroyGround();
		}
		EditorGUILayout.EndHorizontal();

		// Coin --------------------------------------------------------------

		GUILayout.Space(40);
		GUILayout.Label("Coin", EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Count:", GUILayout.Width(70));
		coinCount = EditorGUILayout.IntField(coinCount);
		EditorGUILayout.EndHorizontal();


		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Geneate", GUILayout.Height(30)))
		{
			manager.GenerateCoin(coinCount);
		}
		if (GUILayout.Button("Destroy", GUILayout.Height(30)))
		{
			manager.DestroyCoin();
		}
		EditorGUILayout.EndHorizontal();
	}
}