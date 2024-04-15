using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelElement : MonoBehaviour
{
	public PropsType type = new PropsType();
	public float x;
	public float y;

	protected virtual void Awake()
	{
		x = this.transform.position.x;
		y = this.transform.position.y;
	}
}

public enum PropsType
{
	None,
	Coin,
}
