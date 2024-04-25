using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup_Skill : Popup_Base
{
	protected override void Awake()
	{
		isDefault = false;

		base.Awake();
	}
}

public enum Skills
{	
	Gold,
	Exp,
	Damage,
	Mana,
	CoolTime,
	Critical,
	Speed,

	PowerOverWhelming,
	Thunder,
	ShockWave,
	Electricute,
	Execution,
}

[Serializable]
public class PlayerSkill
{
	public string name;
	public string description;
	public string thumbnailPath;

	public PlayerSkill(string name, string description, string thumbnailPath)
	{
		this.name = name;
		this.description = description;
		this.thumbnailPath = thumbnailPath;
	}
}

[Serializable]
public class PlayerPassiveSkill : PlayerSkill
{
	public int level;

	public PlayerPassiveSkill(string name, string description, string thumbnailPath, int level) : base(name, description, thumbnailPath)
	{
		this.level = level;
	}
}

[Serializable]
public class PlayerActiveSkill : PlayerSkill
{
	public PlayerActiveSkill(string name, string description, string thumbnailPath) : base(name, description, thumbnailPath)
	{

	}
}