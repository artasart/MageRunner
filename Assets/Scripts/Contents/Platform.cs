using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : LevelElement
{
	protected override void Awake()
	{
		base.Awake();

		type = PropsType.Platform;
	}
}
