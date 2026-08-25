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
		newTask = null;
		if (terminate)
		{
			return true;
		}
		m_Brain.Player.AttemptFireFrontWeapons();
		if (m_Brain.TimeSlice == 1 && m_Brain.Player.TheShip.Weapons.ActiveSpecialWeaponType != SpecialWeaponType.None && !m_Brain.Player.IsPowerCut && (m_Enemy.TheShip.Position - m_Brain.Player.TheShip.Position).Length() < 20000f)
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
			Vector3 vector = position2 - position;
			Vector3 vector2 = position2 + m_Enemy.TheShip.Velocity * (vector.Length() / 200f) * m_Brain.Personality.Accuracy;
			Vector3 vec = vector2 - position;
			float targetRotation = Utils.AngleFromVector(ref vec) - (float)Math.PI / 2f;
			theShip2.TargetRotation = targetRotation;
			float num = vec.Length();
			theShip2.TargetSpeed = MathHelper.Clamp((num - 10000f) / 40f, 0f, theShip2.MaxSpeed);
		}
		return false;
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
