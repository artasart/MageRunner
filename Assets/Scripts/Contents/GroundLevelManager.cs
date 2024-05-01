using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundLevelManager : MonoBehaviour
{
	public void GenerateGround()
	{
		var grounds = FindObjectsOfType<Ground>();

		for(int i = 0; i < grounds.Length; i++)
		{
			grounds[i].Generate();
		}
	}
}
