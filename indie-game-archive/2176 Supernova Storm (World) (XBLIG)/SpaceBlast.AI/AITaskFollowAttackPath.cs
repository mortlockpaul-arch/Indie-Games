using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SpaceBlast.PathFinding;

namespace SpaceBlast.AI;

internal class AITaskFollowAttackPath : AITaskPathFollower
{
	private Player m_Enemy;

	public AITaskFollowAttackPath(AIBrain brain, PlannedPath path, Player enemy)
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
		if (m_Brain.TimeSlice == 3 && m_Brain.AttackOrRetreat(m_Enemy) != EAttackOrRetreat.Attack)
		{
			return true;
		}
		if (m_Brain.TimeSlice == 5)
		{
			Ship theShip = m_Brain.Player.TheShip;
			Vector3 start = theShip.Position;
			Vector3 end = m_Enemy.TheShip.Position;
			Line line = new Line(ref start, ref end, (int)theShip.Diameter);
			if (!MainGame.LevelData.StaticWorldObjects.CollisionTest(line))
			{
				newTask = new AITaskDirectAttack(m_Brain, m_Enemy);
				return true;
			}
		}
		if (m_Brain.TimeSlice == 7)
		{
			PowerUp powerUp = ChoosePowerup();
			if (powerUp != null)
			{
				newTask = new AITaskPlanPowerupPickup(m_Brain, powerUp, abortForCombat: false);
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
		MainGame.LevelData.PowerUps.FindAllPowerupsInRange(ref position, 20000f, out powerups);
		_ = (theShip.Strength + theShip.Shields) / 2f;
		(m_Enemy.TheShip.Position - theShip.Position).Length();
		PowerUp result = null;
		int num = 0;
		float num2 = float.MaxValue;
		foreach (PowerUp item in powerups)
		{
			int num3 = -1;
			switch (item.Type)
			{
			case PowerUpType.FrontAmmo:
				if (m_Brain.Player.TheShip.Weapons.ActiveFrontWeapon.Ammo < 50)
				{
					num3 = 30;
				}
				break;
			case PowerUpType.MegaDamage:
				num3 = 100;
				break;
			case PowerUpType.Cloak:
				num3 = 80;
				break;
			case PowerUpType.Invincible:
				num3 = 90;
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
