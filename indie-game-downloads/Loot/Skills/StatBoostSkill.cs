using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;

namespace Loot.Skills;

public class StatBoostSkill : Skill
{
	public override string Name => "Stat Boost";

	public override Sprite Sprite => SkillSprite.StatBoost;

	public StatBoostSkill()
	{
	}

	public StatBoostSkill(BinaryReader reader)
		: base(reader)
	{
	}

	public void BoostStats()
	{
		if (Level != 0)
		{
			if (Level == 1)
			{
				boost(1);
			}
			else if (Level <= 3)
			{
				boost(2);
			}
			else if (Level <= 5)
			{
				boost(3);
			}
			else if (Level <= 7)
			{
				boost(4);
			}
			else if (Level <= 9)
			{
				boost(5);
			}
			else if (Level == 10)
			{
				boost(10);
			}
		}
	}

	private void boost(int amt)
	{
		for (int i = 0; i < 4; i++)
		{
			DM.Player.Base.Modify(i, amt);
		}
		DM.Player.CalcStats();
	}
}
