using Eyehook.Framework;
using Loot.Dungeon;
using Loot.Items;
using Loot.Skills;
using Microsoft.Xna.Framework;

namespace Loot.PC;

public class DemoPlayer : Player
{
	private PlayerClassType classType;

	private string className;

	private PlayerSprite sprite;

	private Sprite portrait;

	public bool IsIdle;

	private Skill primarySkill;

	public override PlayerClassType ClassType => classType;

	public override string ClassName => className;

	public override PlayerSprite Sprite => sprite;

	public override Sprite Portrait => portrait;

	public override Skill PrimarySkill => primarySkill;

	public override int HP
	{
		get
		{
			return base.HP;
		}
		set
		{
			base.HP = MaxHP;
		}
	}

	public DemoPlayer(Player p)
	{
		IsIdle = false;
		Base = p.Base;
		SkillSet = p.SkillSet;
		primarySkill = p.PrimarySkill;
		classType = p.ClassType;
		className = p.ClassName;
		sprite = p.Sprite;
		portrait = p.Portrait;
		for (int i = 0; i < 4; i++)
		{
			Equipment[i] = p.Equipment[i];
		}
		for (int j = 0; j < 32; j++)
		{
			Inventory[j] = p.Inventory[j];
		}
		CalcStats();
	}

	public void SetLevel(int l)
	{
		level = l;
	}

	public override void AddXP(int xp)
	{
	}

	public override bool AddItem(Item item)
	{
		if (item is Armor)
		{
			Equipment[0] = (Equipment)item;
		}
		if (item is Weapon)
		{
			Equipment[1] = (Equipment)item;
		}
		return true;
	}

	public override void UpdateAI(GameTime gameTime)
	{
		if (PrimarySkill is ActiveSkill activeSkill && (activeSkill.CanCast || activeSkill.CanChain))
		{
			if (PrimarySkill is OrbSkill)
			{
				if (DM.Player.Orb == null)
				{
					activeSkill.Cast();
					return;
				}
			}
			else
			{
				for (int i = 0; i < DM.NPCList.Count; i++)
				{
					if ((double)base.Location.Distance(DM.NPCList[i].Location) < 2.0)
					{
						activeSkill.Cast();
						return;
					}
				}
			}
		}
		float num = float.MaxValue;
		Location location = base.Location;
		for (int j = 0; j < DM.Map.Rows; j++)
		{
			for (int k = 0; k < DM.Map.Cols; k++)
			{
				Location location2 = new Location(j, k);
				if ((double)base.Location.Distance(location2) < (double)num && (DM.Map.IsEnemy(location2) || DM.Map.IsItem(location2) || DM.Map.IsDoor(location2) || (DM.Map.IsFloor(location2) && !DM.Map.IsDiscovered(location2))))
				{
					location = location2;
					num = base.Location.Distance(location);
				}
			}
		}
		Location location3 = base.Location;
		if ((double)num < 1.5)
		{
			location3 = location;
		}
		else
		{
			float num2 = float.MaxValue;
			for (int l = base.Location.Row - 1; l <= base.Location.Row + 1; l++)
			{
				for (int m = base.Location.Col - 1; m <= base.Location.Col + 1; m++)
				{
					Location location4 = new Location(l, m);
					if (DM.Map.IsValid(location4) && DM.CanMove(this, location4))
					{
						float num3 = location4.Distance(location);
						if ((double)num3 < (double)num2)
						{
							location3 = location4;
							num2 = num3;
						}
					}
				}
			}
		}
		if (base.Location == location3)
		{
			IsIdle = true;
		}
		DM.MovePlayer(location3);
	}
}
