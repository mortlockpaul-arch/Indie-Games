using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SpaceBlast.PathFinding;
using SpaceBlast.Weapons;

namespace SpaceBlast.AI;

internal class AITaskFollowRetreatPath : AITaskPathFollower
{
	private Player m_Enemy;

	public AITaskFollowRetreatPath(AIBrain brain, PlannedPath path, Player enemy)
		: base(brain, path)
	{
		m_Enemy = enemy;
	}

	public override bool UpdateTask(out AITask newTask, bool terminate)
	{
		newTask = null;
		if (terminate)
		{
			return true;
		}
		m_Brain.Player.AttemptFireRearWeapon();
		if (m_Brain.TimeSlice == 1 && m_Brain.Player.TheShip.Weapons.ActiveSpecialWeaponType != SpecialWeaponType.None && !m_Brain.Player.IsPowerCut && (m_Enemy.TheShip.Position - m_Brain.Player.TheShip.Position).Length() < 30000f)
		{
			m_Brain.Player.TheShip.Weapons.FireSpecialWeapon();
		}
		if (m_Brain.TimeSlice == 3)
		{
			PowerUp powerUp = ChoosePowerup();
			if (powerUp != null)
			{
				newTask = new AITaskPlanPowerupPickup(m_Brain, powerUp, abortForCombat: false);
				return true;
			}
		}
		if (m_Brain.TimeSlice == 5 && m_Brain.AttackOrRetreat(m_Enemy) != EAttackOrRetreat.Retreat)
		{
			return true;
		}
		return FollowPath();
	}

	private PowerUp ChoosePowerup()
	{
		List<PowerUp> powerups = new List<PowerUp>();
		Ship theShip = m_Brain.Player.TheShip;
		Vector2 position = new Vector2(theShip.Position.X, theShip.Position.Y);
		MainGame.LevelData.PowerUps.FindAllPowerupsInRange(ref position, 20000f, out powerups);
		_ = (theShip.Strength + theShip.Shields) / 2f;
		float num = (m_Enemy.TheShip.Position - theShip.Position).Length();
		PowerUp result = null;
		int num2 = 0;
		float num3 = float.MaxValue;
		foreach (PowerUp item in powerups)
		{
			int num4 = -1;
			switch (item.Type)
			{
			case PowerUpType.Repair:
				num4 = (int)MathHelper.Clamp(70f - theShip.Strength, 0f, 100f);
				break;
			case PowerUpType.ShieldBoost:
				num4 = (int)MathHelper.Clamp(40f - theShip.Shields, 0f, 100f);
				break;
			case PowerUpType.RearGun:
				if (num > 40000f && !m_Brain.Player.TheShip.Weapons.ActiveFrontWeapon.IsRearWeaponFitted)
				{
					num4 = 25;
				}
				break;
			case PowerUpType.MegaDamage:
				num4 = 80;
				break;
			case PowerUpType.Cloak:
				num4 = 90;
				break;
			case PowerUpType.Invincible:
				num4 = 100;
				break;
			case PowerUpType.EMP:
				num4 = 85;
				break;
			case PowerUpType.Starburst:
				num4 = 87;
				break;
			case PowerUpType.Shockwave:
				num4 = 89;
				break;
			}
			if (num4 > num2)
			{
				result = item;
				num2 = num4;
				num3 = (theShip.Position - item.Position).Length();
			}
			else if ((float)num4 == num3)
			{
				float num5 = (theShip.Position - item.Position).Length();
				if (num5 < num3)
				{
					result = item;
					num2 = num4;
					num3 = num5;
				}
			}
		}
		return result;
	}
}
