using Microsoft.Xna.Framework;

namespace Defenders;

public class Asset
{
	public LevelData levelData;

	public string name;

	public string desc;

	public Vector2 position;

	public float angle;

	public float size;

	public int type;

	public uint frame;

	public string text;

	public Color color;

	public Color color2;

	public Color color3;

	public int numSec;

	public int numPri;

	public static LevelData Parse(string d)
	{
		LevelData levelData = LevelData.orb;
		return d switch
		{
			"colony" => LevelData.colony, 
			"playerSpawn" => LevelData.playerSpawn, 
			"enemy" => LevelData.enemy, 
			"enemyBase" => LevelData.enemyBase, 
			"asteroid" => LevelData.asteroid, 
			"block" => LevelData.block, 
			"pathNode" => LevelData.pathNode, 
			"lensFlare" => LevelData.lensFlare, 
			"orb" => LevelData.orb, 
			"blueMatter" => LevelData.blueMatter, 
			"relic" => LevelData.relic, 
			"message" => LevelData.message, 
			_ => LevelData.orb, 
		};
	}

	public void Reset()
	{
		numPri = numSec;
	}

	public Asset()
	{
	}

	public Asset(LevelData levelData)
		: this(levelData, new Vector2(640f, 360f), "", "", 0f, 1f, 0, 0u, "", Color.White, Color.White, Color.White, 1, 1)
	{
	}

	public Asset(LevelData levelData, int type)
		: this(levelData, new Vector2(640f, 360f), "", "", 0f, 1f, type, 0u, "", Color.White, Color.White, Color.White, 1, 1)
	{
	}

	public Asset(LevelData levelData, Vector2 position)
		: this(levelData, position, "", "", 0f, 1f, 0, 0u, "", Color.White, Color.White, Color.White, 1, 1)
	{
	}

	public Asset(LevelData levelData, Vector2 position, string name, string desc, float angle, float size, int type, uint frame, string text, Color color, Color color2, Color color3, int numSec, int numPri)
	{
		this.levelData = levelData;
		this.name = "name: " + name;
		this.desc = "desc: " + desc;
		this.position = position;
		this.angle = angle;
		this.size = size;
		this.type = type;
		this.frame = frame;
		this.text = "text: " + text;
		this.color = color;
		this.color2 = color2;
		this.color3 = color3;
		this.numSec = numSec;
		this.numPri = numPri;
	}
}
