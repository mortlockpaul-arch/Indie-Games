using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SpaceBlast.Weapons;

namespace SpaceBlast.AI;

internal class AITaskDirectAttack : AITask
{
	private Player m_Enemy;

	public AITaskDirectAttack(AIBrain brain, Player enemy)
		: base(brain)
	{
		m_Enemy = enemy;
	}

	public override bool UpdateTask(out AITask newTask, bool terminate)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		newTask = null;
		if (terminate)
		{
			return true;
		}
		m_Brain.Player.AttemptFireFrontWeapons();
		if (m_Brain.TimeSlice == 1 && m_Brain.Player.TheShip.Weapons.ActiveSpecialWeaponType != SpecialWeaponType.None && !m_Brain.Player.IsPowerCut)
		{
			Vector3 val = m_Enemy.TheShip.Position - m_Brain.Player.TheShip.Position;
			if (((Vector3)(ref val)).Length() < 20000f)
			{
				m_Brain.Player.TheShip.Weapons.FireSpecialWeapon();
			}
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
		if (m_Brain.TimeSlice == 5 && m_Brain.AttackOrRetreat(m_Enemy) != EAttackOrRetreat.Attack)
		{
			return true;
		}
		if (m_Brain.TimeSlice == 7)
		{
			Ship theShip = m_Brain.Player.TheShip;
			Vector3 start = theShip.Position;
			Vector3 end = m_Enemy.TheShip.Position;
			Line line = new Line(ref start, ref end, (int)theShip.Diameter);
			if (MainGame.LevelData.StaticWorldObjects.CollisionTest(line))
			{
				return true;
			}
		}
		if (m_Brain.TimeSlice == 9)
		{
			Ship theShip2 = m_Brain.Player.TheShip;
			Vector3 position = theShip2.Position;
			Vector3 position2 = m_Enemy.TheShip.Position;
			Vector3 val2 = position2 - position;
			Vector3 val3 = position2 + m_Enemy.TheShip.Velocity * (((Vector3)(ref val2)).Length() / 200f) * m_Brain.Personality.Accuracy;
			Vector3 vec = val3 - position;
			float targetRotation = Utils.AngleFromVector(ref vec) - (float)Math.PI / 2f;
			theShip2.TargetRotation = targetRotation;
			float num = ((Vector3)(ref vec)).Length();
			theShip2.TargetSpeed = MathHelper.Clamp((num - 10000f) / 40f, 0f, theShip2.MaxSpeed);
		}
		return false;
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
