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
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		if (!(TimeManager.TotalSeconds < m_EarliestNextShot) && m_Ammo > 0)
		{
			Vector3 velocity = m_Ship.Velocity;
			Vector3 forward = ((Matrix)(ref m_Ship.RotationMatrix)).Forward;
			((Vector3)(ref forward)).Normalize();
			Vector3 val = forward;
			val *= 600f;
			Vector3 val2 = velocity + val;
			double num = TimeManager.TotalSeconds + 10.0;
			m_Ship.GetWeaponBayPositions(out var frontBay, out var _, out var leftBay, out var rightBay);
			if (m_GunCount == 3 || m_GunCount == 5)
			{
				m_WeaponSystem.ActiveRounds.Add(new VBlasterRound(frontBay, val2, num));
				m_Ammo--;
			}
			Vector3 vec = val2;
			Vector3 vec2 = val2;
			if (m_Ammo >= 2)
			{
				Utils.AdjustVector(ref vec, 0.05f);
				Utils.AdjustVector(ref vec2, -0.05f);
				m_WeaponSystem.ActiveRounds.Add(new VBlasterRound(leftBay, vec, num));
				m_WeaponSystem.ActiveRounds.Add(new VBlasterRound(rightBay, vec2, num));
				m_Ammo -= 2;
			}
			Vector3 vec3 = val2;
			Vector3 vec4 = val2;
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
				Vector3 centre_endpos = m_Ship.Position + val2 * GameConstants.constBulletLifetime * 60f;
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
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
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
			val *= 600f;
			Vector3 val2 = velocity + val;
			double num = TimeManager.TotalSeconds + 10.0;
			m_Ship.GetWeaponBayPositions(out var _, out var rearBay, out var _, out var _);
			m_WeaponSystem.ActiveRounds.Add(new VBlasterRound(rearBay, val2, num));
			m_Ammo--;
			m_EarliestNextShot = (float)TimeManager.TotalSeconds + m_ShotDelay;
			MainGame.AudioMan.Play(Sound.Laser1, m_Ship.Position);
			if (MainGame.NetMan.IsNetworkGame)
			{
				Vector3 endpos = m_Ship.Position + GameConstants.constBulletLifetime * val2 * 60f;
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
