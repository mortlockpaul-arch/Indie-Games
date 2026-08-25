using System;
using Microsoft.Xna.Framework;

namespace SpaceBlast.Weapons;

internal class VBlaster : Weapon
{
	private const float constBulletSpeed = 600f;

	private const float constBulletLifetime = 10f;

	private float m_ShotDelay = 0.25f;

	public VBlaster(WeaponSystem system, Player player)
		: base(system, player)
	{
		m_GunCount = 2;
		VBlasterRound.SetupStatics();
	}

	public override void FireWeapon()
	{
		if (!(TimeManager.TotalSeconds < m_EarliestNextShot) && m_Ammo > 0)
		{
			Vector3 velocity = m_Ship.Velocity;
			Vector3 forward = m_Ship.RotationMatrix.Forward;
			forward.Normalize();
			Vector3 vector = forward;
			vector *= 600f;
			Vector3 vector2 = velocity + vector;
			double num = TimeManager.TotalSeconds + 10.0;
			m_Ship.GetWeaponBayPositions(out var frontBay, out var _, out var leftBay, out var rightBay);
			if (m_GunCount == 3 || m_GunCount == 5)
			{
				m_WeaponSystem.ActiveRounds.Add(new VBlasterRound(frontBay, vector2, num));
				m_Ammo--;
			}
			Vector3 vec = vector2;
			Vector3 vec2 = vector2;
			if (m_Ammo >= 2)
			{
				Utils.AdjustVector(ref vec, 0.05f);
				Utils.AdjustVector(ref vec2, -0.05f);
				m_WeaponSystem.ActiveRounds.Add(new VBlasterRound(leftBay, vec, num));
				m_WeaponSystem.ActiveRounds.Add(new VBlasterRound(rightBay, vec2, num));
				m_Ammo -= 2;
			}
			Vector3 vec3 = vector2;
			Vector3 vec4 = vector2;
			if ((m_GunCount == 4 || m_GunCount == 5) && m_Ammo >= 2)
			{
				Utils.AdjustVector(ref vec3, 0.15f);
				Utils.AdjustVector(ref vec4, -0.15f);
				m_WeaponSystem.ActiveRounds.Add(new VBlasterRound(leftBay, vec3, num));
				m_WeaponSystem.ActiveRounds.Add(new VBlasterRound(rightBay, vec4, num));
				m_Ammo -= 2;
			}
			m_EarliestNextShot = (float)TimeManager.TotalSeconds + m_ShotDelay;
			MainGame.AudioMan.Play(Sound.Laser1, m_Ship.Position);
			if (MainGame.NetMan.IsNetworkGame)
			{
				Vector3 centre_endpos = m_Ship.Position + vector2 * GameConstants.constBulletLifetime * 60f;
				Vector3 innerleft_endpos = m_Ship.Position + vec * GameConstants.constBulletLifetime * 60f;
				Vector3 innerright_endpos = m_Ship.Position + vec2 * GameConstants.constBulletLifetime * 60f;
				Vector3 outerleft_endpos = m_Ship.Position + vec3 * GameConstants.constBulletLifetime * 60f;
				Vector3 outerright_endpos = m_Ship.Position + vec4 * GameConstants.constBulletLifetime * 60f;
				MainGame.NetMan.SendVBlasterFiredPacket(m_Player.PlayerID, num, m_GunCount, ref centre_endpos, ref innerleft_endpos, ref innerright_endpos, ref outerleft_endpos, ref outerright_endpos);
			}
		}
	}

	public void FireRemoteWeapon(int guncount, double endtime, ref Vector3 centre_endpos, ref Vector3 innerleft_endpos, ref Vector3 innerright_endpos, ref Vector3 outerleft_endpos, ref Vector3 outerright_endpos)
	{
		m_Ship.GetWeaponBayPositions(out var frontBay, out var _, out var leftBay, out var rightBay);
		double num = endtime - TimeManager.TotalSeconds;
		Vector3 velocity;
		if (guncount == 3 || guncount == 5)
		{
			velocity = (centre_endpos - frontBay) / (float)(num * 60.0);
			m_WeaponSystem.ActiveRounds.Add(new VBlasterRound(frontBay, velocity, endtime));
		}
		velocity = (innerleft_endpos - leftBay) / (float)(num * 60.0);
		m_WeaponSystem.ActiveRounds.Add(new VBlasterRound(leftBay, velocity, endtime));
		velocity = (innerright_endpos - rightBay) / (float)(num * 60.0);
		m_WeaponSystem.ActiveRounds.Add(new VBlasterRound(rightBay, velocity, endtime));
		if (guncount == 4 || guncount == 5)
		{
			velocity = (outerleft_endpos - leftBay) / (float)(num * 60.0);
			m_WeaponSystem.ActiveRounds.Add(new VBlasterRound(leftBay, velocity, endtime));
			velocity = (outerright_endpos - rightBay) / (float)(num * 60.0);
			m_WeaponSystem.ActiveRounds.Add(new VBlasterRound(rightBay, velocity, endtime));
		}
		MainGame.AudioMan.Play(Sound.Laser1, m_Ship.Position);
	}

	public override void FireRemoteWeapon(int guncount, ref Vector3 endPos, double endtime, object extradata)
	{
	}

	public override void FireRearWeapon()
	{
		if (IsRearWeaponFitted && !(TimeManager.TotalSeconds < m_EarliestNextShot) && m_Ammo > 0)
		{
			Vector3 velocity = m_Ship.Velocity;
			Vector3 backward = m_Ship.RotationMatrix.Backward;
			backward.Normalize();
			backward *= 600f;
			Vector3 vector = velocity + backward;
			double num = TimeManager.TotalSeconds + 10.0;
			m_Ship.GetWeaponBayPositions(out var _, out var rearBay, out var _, out var _);
			m_WeaponSystem.ActiveRounds.Add(new VBlasterRound(rearBay, vector, num));
			m_Ammo--;
			m_EarliestNextShot = (float)TimeManager.TotalSeconds + m_ShotDelay;
			MainGame.AudioMan.Play(Sound.Laser1, m_Ship.Position);
			if (MainGame.NetMan.IsNetworkGame)
			{
				Vector3 endpos = m_Ship.Position + GameConstants.constBulletLifetime * vector * 60f;
				MainGame.NetMan.SendWeaponFiredPacket(m_Player.PlayerID, num, WeaponType.VBlaster, -1, endpos);
			}
		}
	}

	public override void ApplyAmmoPack()
	{
		m_Ammo += 200;
	}

	public override void ApplyWeaponUpgrade()
	{
		m_GunCount = Math.Min(m_GunCount + 1, 5);
	}

	public override void IncreaseFireRate()
	{
		m_ShotDelay = MathHelper.Max(m_ShotDelay * 0.8f, 0.1f);
	}

	public override void FitWeaponToShip()
	{
		m_IsWeaponFitted = true;
		m_Ammo = 600;
	}
}
