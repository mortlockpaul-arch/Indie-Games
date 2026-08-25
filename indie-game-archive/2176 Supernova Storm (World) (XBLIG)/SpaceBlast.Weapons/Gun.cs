using System;
using Microsoft.Xna.Framework;

namespace SpaceBlast.Weapons;

internal class Gun : Weapon
{
	private const float constBulletSpeed = 400f;

	private const float constBulletLifetime = 10f;

	private float m_ShotDelay = 0.25f;

	public Gun(WeaponSystem system, Player player)
		: base(system, player)
	{
		m_GunCount = 1;
		GunRound.SetupStatics();
	}

	public override void FireWeapon()
	{
		if (!(TimeManager.TotalSeconds < m_EarliestNextShot) && m_Ammo > 0)
		{
			Vector3 velocity = m_Ship.Velocity;
			Vector3 forward = m_Ship.RotationMatrix.Forward;
			forward.Normalize();
			forward *= 400f;
			Vector3 vector = velocity + forward;
			double num = TimeManager.TotalSeconds + 10.0;
			m_Ship.GetWeaponBayPositions(out var frontBay, out var _, out var leftBay, out var rightBay);
			if (m_GunCount == 1 || m_GunCount == 3)
			{
				m_WeaponSystem.ActiveRounds.Add(new GunRound(frontBay, vector, num));
				m_Ammo--;
			}
			if ((m_GunCount == 2 || m_GunCount == 3) && m_Ammo > 0)
			{
				m_WeaponSystem.ActiveRounds.Add(new GunRound(leftBay, vector, num));
				m_WeaponSystem.ActiveRounds.Add(new GunRound(rightBay, vector, num));
				m_Ammo -= 2;
			}
			m_EarliestNextShot = (float)TimeManager.TotalSeconds + m_ShotDelay;
			MainGame.AudioMan.Play(Sound.GunFire, m_Ship.Position);
			if (MainGame.NetMan.IsNetworkGame)
			{
				Vector3 endpos = m_Ship.Position + GameConstants.constBulletLifetime * vector * 60f;
				MainGame.NetMan.SendWeaponFiredPacket(m_Player.PlayerID, num, WeaponType.Gun, m_GunCount, endpos);
			}
		}
	}

	public override void FireRemoteWeapon(int guncount, ref Vector3 endPos, double endtime, object extradata)
	{
		m_Ship.GetWeaponBayPositions(out var frontBay, out var rearBay, out var leftBay, out var rightBay);
		Vector3 vec = endPos - m_Ship.Position;
		float angle = Utils.AngleFromVector(ref vec);
		m_Ship.GetWeaponBayPositions(endPos, angle, out var _, out var _, out var _, out var _);
		double num = endtime - TimeManager.TotalSeconds;
		Vector3 velocity = vec / (float)(num * 60.0);
		if (guncount == 1 || guncount == 3)
		{
			m_WeaponSystem.ActiveRounds.Add(new GunRound(frontBay, velocity, endtime));
		}
		if (guncount == 2 || guncount == 3)
		{
			m_WeaponSystem.ActiveRounds.Add(new GunRound(leftBay, velocity, endtime));
			m_WeaponSystem.ActiveRounds.Add(new GunRound(rightBay, velocity, endtime));
		}
		if (guncount == -1)
		{
			m_WeaponSystem.ActiveRounds.Add(new GunRound(rearBay, velocity, endtime));
		}
		MainGame.AudioMan.Play(Sound.GunFire, m_Ship.Position);
	}

	public override void FireRearWeapon()
	{
		if (IsRearWeaponFitted && !(TimeManager.TotalSeconds < m_EarliestNextShot) && m_Ammo > 0)
		{
			Vector3 velocity = m_Ship.Velocity;
			Vector3 backward = m_Ship.RotationMatrix.Backward;
			backward.Normalize();
			backward *= 400f;
			Vector3 vector = velocity + backward;
			double num = TimeManager.TotalSeconds + 10.0;
			m_Ship.GetWeaponBayPositions(out var _, out var rearBay, out var _, out var _);
			m_WeaponSystem.ActiveRounds.Add(new GunRound(rearBay, vector, num));
			m_Ammo--;
			m_EarliestNextShot = (float)TimeManager.TotalSeconds + m_ShotDelay;
			MainGame.AudioMan.Play(Sound.GunFire, m_Ship.Position);
			if (MainGame.NetMan.IsNetworkGame)
			{
				Vector3 endpos = m_Ship.Position + GameConstants.constBulletLifetime * vector * 60f;
				MainGame.NetMan.SendWeaponFiredPacket(m_Player.PlayerID, num, WeaponType.Gun, -1, endpos);
			}
		}
	}

	public override void ApplyAmmoPack()
	{
		m_Ammo += 200;
	}

	public override void ApplyWeaponUpgrade()
	{
		m_GunCount = Math.Min(m_GunCount + 1, 3);
	}

	public override void IncreaseFireRate()
	{
		m_ShotDelay = MathHelper.Max(m_ShotDelay * 0.8f, 0.1f);
	}

	public override void FitWeaponToShip()
	{
		m_IsWeaponFitted = true;
		m_Ammo = 200;
	}
}
