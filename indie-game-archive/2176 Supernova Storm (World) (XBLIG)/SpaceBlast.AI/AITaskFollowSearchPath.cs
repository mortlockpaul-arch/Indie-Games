using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SpaceBlast.PathFinding;

namespace SpaceBlast.AI;

internal class AITaskFollowSearchPath : AITaskPathFollower
{
	public AITaskFollowSearchPath(AIBrain brain, PlannedPath plannedPath)
		: base(brain, plannedPath)
	{
	}

	public override bool UpdateTask(out AITask newTask, bool terminate)
	{
		newTask = null;
		if (terminate)
		{
			return true;
		}
		if (m_Brain.TimeSlice == 1)
		{
			Player enemy = null;
			float num = MainGame.Players.FindNearestVisibleEnemy(m_Brain.Player, out enemy);
			if (num < (float)m_Brain.Personality.VisualRange)
			{
				newTask = new AITaskPlanCombat(m_Brain, enemy);
				return true;
			}
		}
		if (m_Brain.TimeSlice == 3)
		{
			PowerUp powerUp = ChoosePowerup();
			if (powerUp != null)
			{
				newTask = new AITaskPlanPowerupPickup(m_Brain, powerUp, abortForCombat: true);
				return true;
			}
		}
		return FollowPath();
	}

	private PowerUp ChoosePowerup()
	{
		List<PowerUp> powerups = new List<PowerUp>();
		Ship theShip = m_Brain.Player.TheShip;
		Vector2 position = new Vector2(theShip.Position.X, theShip.Position.Y);
		MainGame.LevelData.PowerUps.FindAllPowerupsInRange(ref position, m_Brain.Personality.VisualRange, out powerups);
		_ = (theShip.Strength + theShip.Shields) / 2f;
		PowerUp result = null;
		int num = -1;
		float num2 = float.MaxValue;
		foreach (PowerUp item in powerups)
		{
			int num3 = 0;
			switch (item.Type)
			{
			case PowerUpType.Acceleration:
			case PowerUpType.TopSpeed:
				num3 = 25;
				break;
			case PowerUpType.Repair:
				num3 = (int)MathHelper.Clamp(80f - theShip.Strength, 0f, 100f);
				break;
			case PowerUpType.ShieldBoost:
				num3 = (int)MathHelper.Clamp(80f - theShip.Shields, 0f, 100f);
				break;
			case PowerUpType.ShieldRegenRate:
				num3 = (int)(MathHelper.Clamp(80f - theShip.Shields, 0f, 100f) / 2f);
				break;
			case PowerUpType.FrontAmmo:
				num3 = (int)MathHelper.Clamp(200 - m_Brain.Player.TheShip.Weapons.ActiveFrontWeapon.Ammo, 0f, 100f);
				break;
			case PowerUpType.FrontGun:
				num3 = 33;
				break;
			case PowerUpType.RearGun:
				if (!m_Brain.Player.TheShip.Weapons.ActiveFrontWeapon.IsRearWeaponFitted)
				{
					num3 = 32;
				}
				break;
			case PowerUpType.FrontBlaster:
				num3 = 34;
				break;
			case PowerUpType.FrontVBlaster:
				num3 = 35;
				break;
			case PowerUpType.IncreaseFireRate:
				num3 = 40;
				break;
			case PowerUpType.MegaDamage:
				num3 = 90;
				break;
			case PowerUpType.Cloak:
				num3 = 95;
				break;
			case PowerUpType.Invincible:
				num3 = 100;
				break;
			case PowerUpType.EMP:
				num3 = 85;
				break;
			case PowerUpType.Starburst:
				num3 = 87;
				break;
			case PowerUpType.Shockwave:
				num3 = 89;
				break;
			}
			if (num3 > num)
			{
				result = item;
				num = num3;
				num2 = (theShip.Position - item.Position).Length();
			}
			else if ((float)num3 == num2)
			{
				float num4 = (theShip.Position - item.Position).Length();
				if (num4 < num2)
				{
					result = item;
					num = num3;
					num2 = num4;
				}
			}
		}
		return result;
	}
}
