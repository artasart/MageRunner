using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelElement : MonoBehaviour
{
	public PropsType type = new PropsType();
}

public enum PropsType
{
	None,
	Coin,
	Monster,
}
