using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Defenders;

public class Character
{
	public string name;

	public string shipClass;

	public Color color;

	public string abilityType;

	public ushort[] relics = new ushort[15];

	public List<float> ability = new List<float>(4);

	public uint numberOfKills = 0u;

	public ushort level;

	public int experience;

	public int nextLevel;

	public ushort relic
	{
		get
		{
			if (relics.Length > 0)
			{
				int num = Game1.currentLevel;
				if (num > relics.Length)
				{
					num = relics.Length;
				}
				if (num < 0)
				{
					num = 0;
				}
				return relics[num];
			}
			return 0;
		}
		set
		{
			relics[Game1.currentLevel] = value;
		}
	}

	public void Reset()
	{
		ushort[] array = new ushort[15];
		relics = array;
		ability = new List<float>(4) { 0f, 0f, 0f, 0f };
		numberOfKills = 0u;
		level = 0;
		nextLevel = 750;
		experience = 0;
	}

	public ushort nRelics()
	{
		ushort num = 0;
		for (int i = 0; i < 15; i++)
		{
			num += relics[i];
		}
		return num;
	}

	public Character()
	{
	}

	public Character(string name, string shipClass, Color color, string abilityType, ushort[] relics, List<float> abilities, uint numberOfKills, ushort level, int experience, int nextLevel)
	{
		this.name = name;
		this.shipClass = shipClass;
		this.color = color;
		this.abilityType = abilityType;
		for (int i = 0; i < 15; i++)
		{
			this.relics[i] = relics[i];
		}
		ability = abilities;
		if (abilities.Count > 0)
		{
			for (int i = 0; i < 4; i++)
			{
				ability.Add(abilities[i]);
			}
		}
		else
		{
			for (int i = 0; i < 4; i++)
			{
				ability.Add(0f);
			}
		}
		this.numberOfKills = numberOfKills;
		this.level = level;
		this.experience = experience;
		this.nextLevel = nextLevel;
	}

	public Character Cloning()
	{
		return new Character(name, shipClass, color, abilityType, relics, ability, numberOfKills, level, experience, nextLevel);
	}
}
