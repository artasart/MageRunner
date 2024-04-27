using UnityEngine;

public static class Define
{
	public const string BGM = "BGM";
	public const string SOUNDEFFECT = "SoundEffect";

	public const string PATH_PREFAB = "Prefabs/";
	public const string PATH_SOUND = "Sound/";
	public const string PATH_PLAYER = PATH_PREFAB + "Player/";
	public const string PATH_CORE = PATH_PREFAB + "Manager/";
	public const string PATH_UI = PATH_PREFAB + "UI/";
	public const string PATH_VFX = PATH_PREFAB + "VFx/";
	public const string PATH_Actor = PATH_PREFAB + "Actor/";
	public const string PATH_SPRITE = "Sprites/";
	public const string PATH_MONSTERS = PATH_CONTENTS + "Monsters/";
	public const string PATH_CONTENTS = PATH_PREFAB + "Contents/";
	public const string PATH_UNITS = PATH_CONTENTS + "Units/";
	public const string PATH_ACTOR = PATH_CONTENTS + "Actor/";
	public const string PATH_ICON = "Icons/";
	public const string PATH_HAND_SKILLS = PATH_ICON + "Hand/Skills/";
	public const string PATH_ITEMS = PATH_ICON + "Hand/Skills/";

	public const string PATH_ITEM = PATH_PREFAB + "Item/";

	public const string KEY_FIRST = "First";
	public const string KEY_BGM = "BGMVolume";
	public const string KEY_SOUNDEFFECT = "EffectVolume";
	public const string KEY_NICKNAME = "Nickname";

	public const string SPAWN = "Spawn";
	public const string POOL = "Pool";

	public const string SERVERURL = "http://localhost:3000"; // LOCALHOST

	public const string PLAYER = "Player";
	public const string GROUND = "Ground";
	public const string COIN = "Coin";
	public const string BIGCOIN = "BigCoin";
	public const string MONSTER = "Monster";
	public const string EXECUTE = "Execute";
	public const string PORTAL = "Portal";
	public const string OBSTACLE = "Obstacle";
	public const string COLLECTOR = "Collector";


	public const string RUN = "Run";
	public const string RUNSTATE = "RunState";
	public const string SLIDE = "Slide";
	public const string DIE = "Die";
	public const string EDITCHK = "Editchk";

	public const string SOUND_CLOSE = "Warnning";
	public const string SOUND_OPEN = "Warnning";

	public const string	JSON_MASTERDATA = "MasterData.json";
	public const string	JSON_LEVELDATA = "LevelData.json";
	public const string	JSON_GAMEDATA = "GameData.json";
	public const string	JSON_INVENDATA = "InvenData.json";

	[Header("Pool")]
	public const string MONSTER_ACTOR = "MonsterActor";
	public const string VFX_THUNDER = "Thunder";
	public const string VFX_BIG_THUNDER_EXPLOSION = "Thunder_Explosion";
	public const string VFX_SMALL_THUNDER_EXPLOSION = "Thunder_ExplosionSmall";
	public const string VFX_UI_ELECTRIC_MESH = "ElectricMesh";

	public const string ITEM_COIN_SPAWNER = "CoinSpawner";
	public const string ITEM_COIN = "Coin";
	public const string ITEM_SKILL_CARD = "SkillCard";
	public const string VFX_SKULL = "Skull";

	public const string SOUND_THUNDER = "Spawn";
	public const string SOUND_DAWN = "Dawn";
	public const string SOUND_DIE = "BodyFall_1";
}
