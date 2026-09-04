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
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		List<PowerUp> powerups = new List<PowerUp>();
		Ship theShip = m_Brain.Player.TheShip;
		Vector2 position = new Vector2(theShip.Position.X, theShip.Position.Y);
		MainGame.LevelData.PowerUps.FindAllPowerupsInRange(ref position, 20000f, out powerups);
		_ = (theShip.Strength + theShip.Shields) / 2f;
		Vector3 val = m_Enemy.TheShip.Position - theShip.Position;
		((Vector3)(ref val)).Length();
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
				Vector3 val2 = theShip.Position - item.Position;
				num2 = ((Vector3)(ref val2)).Length();
			}
			else if ((float)num3 == num2)
			{
				Vector3 val3 = theShip.Position - item.Position;
				float num4 = ((Vector3)(ref val3)).Length();
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
