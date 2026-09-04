using System;
using Microsoft.Xna.Framework;

namespace SpaceBlast.Weapons;

internal class Blaster : Weapon
{
	private const float constBulletSpeed = 650f;

	private const float constBulletLifetime = 10f;

	private float m_ShotDelay = 0.25f;

	public Blaster(WeaponSystem system, Player player)
		: base(system, player)
	{
		m_GunCount = 1;
		BlasterRound.SetupStatics();
	}

	public override void FireWeapon()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		if (!(TimeManager.TotalSeconds < m_EarliestNextShot) && m_Ammo > 0)
		{
			Vector3 velocity = m_Ship.Velocity;
			Vector3 val = ((Matrix)(ref m_Ship.RotationMatrix)).Forward;
			((Vector3)(ref val)).Normalize();
			val *= 650f;
			Vector3 val2 = velocity + val;
			double num = TimeManager.TotalSeconds + 10.0;
			m_Ship.GetWeaponBayPositions(out var frontBay, out var _, out var leftBay, out var rightBay);
			if (m_GunCount == 1 || m_GunCount == 3)
			{
				m_WeaponSystem.ActiveRounds.Add(new BlasterRound(frontBay, val2, num));
				m_Ammo--;
			}
			if ((m_GunCount == 2 || m_GunCount == 3) && m_Ammo > 0)
			{
				m_WeaponSystem.ActiveRounds.Add(new BlasterRound(leftBay, val2, num));
				m_WeaponSystem.ActiveRounds.Add(new BlasterRound(rightBay, val2, num));
				m_Ammo -= 2;
			}
			m_EarliestNextShot = (float)TimeManager.TotalSeconds + m_ShotDelay;
			MainGame.AudioMan.Play(Sound.Laser2, m_Ship.Position);
			if (MainGame.NetMan.IsNetworkGame)
			{
				Vector3 endpos = m_Ship.Position + GameConstants.constBulletLifetime * val2 * 60f;
				MainGame.NetMan.SendWeaponFiredPacket(m_Player.PlayerID, num, WeaponType.Blaster, m_GunCount, endpos);
			}
		}
	}

	public override void FireRemoteWeapon(int guncount, ref Vector3 endPos, double endtime, object extradata)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		m_Ship.GetWeaponBayPositions(out var frontBay, out var rearBay, out var leftBay, out var rightBay);
		Vector3 vec = endPos - m_Ship.Position;
		float angle = Utils.AngleFromVector(ref vec);
		m_Ship.GetWeaponBayPositions(endPos, angle, out var _, out var _, out var _, out var _);
		double num = endtime - TimeManager.TotalSeconds;
		Vector3 velocity = vec / (float)(num * 60.0);
		if (guncount == 1 || guncount == 3)
		{
			m_WeaponSystem.ActiveRounds.Add(new BlasterRound(frontBay, velocity, endtime));
		}
		if (guncount == 2 || guncount == 3)
		{
			m_WeaponSystem.ActiveRounds.Add(new BlasterRound(leftBay, velocity, endtime));
			m_WeaponSystem.ActiveRounds.Add(new BlasterRound(rightBay, velocity, endtime));
		}
		if (guncount == -1)
		{
			m_WeaponSystem.ActiveRounds.Add(new BlasterRound(rearBay, velocity, endtime));
		}
		MainGame.AudioMan.Play(Sound.Laser2, m_Ship.Position);
	}

	public override void FireRearWeapon()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		if (IsRearWeaponFitted && !(TimeManager.TotalSeconds < m_EarliestNextShot) && m_Ammo > 0)
		{
			Vector3 velocity = m_Ship.Velocity;
			Vector3 val = ((Matrix)(ref m_Ship.RotationMatrix)).Backward;
			((Vector3)(ref val)).Normalize();
			val *= 650f;
			Vector3 val2 = velocity + val;
			double num = TimeManager.TotalSeconds + 10.0;
			m_Ship.GetWeaponBayPositions(out var _, out var rearBay, out var _, out var _);
			m_WeaponSystem.ActiveRounds.Add(new BlasterRound(rearBay, val2, num));
			m_Ammo--;
			m_EarliestNextShot = (float)TimeManager.TotalSeconds + m_ShotDelay;
			MainGame.AudioMan.Play(Sound.Laser2, m_Ship.Position);
			if (MainGame.NetMan.IsNetworkGame)
			{
				Vector3 endpos = m_Ship.Position + GameConstants.constBulletLifetime * val2 * 60f;
				MainGame.NetMan.SendWeaponFiredPacket(m_Player.PlayerID, num, WeaponType.Blaster, -1, endpos);
			}
		}
	}

	public override void ApplyAmmoPack()
	{
		m_Ammo += 100;
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
		m_Ammo = 100;
	}
}
