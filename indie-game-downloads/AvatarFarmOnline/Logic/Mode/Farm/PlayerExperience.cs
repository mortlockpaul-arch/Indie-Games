using System;
using System.Xml.Linq;
using Quasar.Global;

namespace AvatarFarmOnline.Logic.Mode.Farm;

internal class PlayerExperience
{
	private int xp;

	private int level = 1;

	public int Xp => xp;

	public int XpToNextLevel
	{
		get
		{
			if (level < 100)
			{
				return AvatarFarmOnline.Logic.GameGlobals.PlayerLevelUpXp(level);
			}
			return 0;
		}
	}

	public int Level => level;

	public event Action<AvatarFarmOnline.Logic.Mode.Farm.PlayerExperience> OnXpChange;

	public event Action<AvatarFarmOnline.Logic.Mode.Farm.PlayerExperience> OnLevelUp;

	public void Init()
	{
		xp = 0;
		level = 1;
	}

	public bool EarnXp(int xp)
	{
		this.xp += xp;
		bool result = false;
		if (level < 100)
		{
			while (this.xp >= XpToNextLevel)
			{
				LevelUp();
				result = true;
				if (level >= 100)
				{
					break;
				}
			}
		}
		if (OnXpChange != null)
		{
			OnXpChange(this);
		}
		return result;
	}

	public void LevelUp()
	{
		level++;
		if (OnLevelUp != null)
		{
			OnLevelUp(this);
		}
	}

	public void ToXml(XElement xe)
	{
		xe.SetIntAttribute("xp", xp);
		xe.SetIntAttribute("level", level);
	}

	public void ParseXml(XElement xe)
	{
		xp = xe.ParseIntAttribute("xp");
		level = xe.ParseIntAttribute("level", 1);
	}
}
