using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelElement : MonoBehaviour
{
	public PropsType type = new PropsType();

	protected virtual void Awake()
	{
		if(type == PropsType.None)
		{
			DebugManager.Log($"Warning!! This object type is None. {this.gameObject.name}");
		}
	}
}

public enum PropsType
{
	None,
	Coin,
	Monster,
	Trigger,
	Platform,
	Obstacle,
	Ground,
	Portal,
}
