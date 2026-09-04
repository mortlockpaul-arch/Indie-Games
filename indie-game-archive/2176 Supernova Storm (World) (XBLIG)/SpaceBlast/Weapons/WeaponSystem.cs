using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SpaceBlast.Weapons;

internal class WeaponSystem
{
	private List<WeaponRound> m_ActiveRounds = new List<WeaponRound>();

	private Dictionary<WeaponType, Weapon> m_FrontWeapons = new Dictionary<WeaponType, Weapon>();

	private WeaponType m_ActiveWeaponType;

	private Weapon m_ActiveWeapon;

	private Player m_Player;

	private SpecialWeapon m_SpecialWeapon;

	private SpecialWeaponType m_SpecialWeaponType = SpecialWeaponType.None;

	public Weapon ActiveFrontWeapon => m_ActiveWeapon;

	public SpecialWeaponType ActiveSpecialWeaponType => m_SpecialWeaponType;

	public List<WeaponRound> ActiveRounds => m_ActiveRounds;

	public WeaponSystem(Player player)
	{
		m_Player = player;
		Reset();
	}

	public void Reset()
	{
		m_FrontWeapons.Clear();
		m_FrontWeapons.Add(WeaponType.Gun, new Gun(this, m_Player));
		m_FrontWeapons.Add(WeaponType.Blaster, new Blaster(this, m_Player));
		m_FrontWeapons.Add(WeaponType.VBlaster, new VBlaster(this, m_Player));
		m_ActiveWeaponType = WeaponType.Gun;
		m_ActiveWeapon = m_FrontWeapons[WeaponType.Gun];
		m_ActiveWeapon.FitWeaponToShip();
		m_SpecialWeapon = null;
		m_SpecialWeaponType = SpecialWeaponType.None;
		m_ActiveRounds = new List<WeaponRound>();
	}

	public void FireFrontWeapon()
	{
		m_ActiveWeapon.FireWeapon();
	}

	public void FireRearWeapon()
	{
		m_ActiveWeapon.FireRearWeapon();
	}

	public void FireSpecialWeapon()
	{
		if (m_SpecialWeapon != null)
		{
			m_SpecialWeapon.FireWeapon();
			m_SpecialWeapon = null;
			m_SpecialWeaponType = SpecialWeaponType.None;
		}
	}

	public void CycleMainWeaponLeft()
	{
		int activeWeaponType = (int)m_ActiveWeaponType;
		for (int num = activeWeaponType - 1; num >= 0; num--)
		{
			if (m_FrontWeapons[(WeaponType)num].IsWeaponFitted)
			{
				m_ActiveWeaponType = (WeaponType)num;
				m_ActiveWeapon = m_FrontWeapons[(WeaponType)num];
				return;
			}
		}
		for (int num2 = 2; num2 > activeWeaponType; num2--)
		{
			if (m_FrontWeapons[(WeaponType)num2].IsWeaponFitted)
			{
				m_ActiveWeaponType = (WeaponType)num2;
				m_ActiveWeapon = m_FrontWeapons[(WeaponType)num2];
				break;
			}
		}
	}

	public void CycleMainWeaponRight()
	{
		int activeWeaponType = (int)m_ActiveWeaponType;
		for (int i = activeWeaponType + 1; i <= 2; i++)
		{
			if (m_FrontWeapons[(WeaponType)i].IsWeaponFitted)
			{
				m_ActiveWeaponType = (WeaponType)i;
				m_ActiveWeapon = m_FrontWeapons[(WeaponType)i];
				return;
			}
		}
		for (int j = 0; j < activeWeaponType; j++)
		{
			if (m_FrontWeapons[(WeaponType)j].IsWeaponFitted)
			{
				m_ActiveWeaponType = (WeaponType)j;
				m_ActiveWeapon = m_FrontWeapons[(WeaponType)j];
				break;
			}
		}
	}

	public void ApplyFrontWeaponUpgrade()
	{
		m_ActiveWeapon.ApplyWeaponUpgrade();
	}

	public void IncreaseFrontFireRate()
	{
		m_ActiveWeapon.IncreaseFireRate();
	}

	public void ApplyAmmoPack()
	{
		m_ActiveWeapon.ApplyAmmoPack();
	}

	public void WeaponPickedUp(WeaponType type)
	{
		if (m_FrontWeapons[type].IsWeaponFitted)
		{
			m_FrontWeapons[type].ApplyWeaponUpgrade();
		}
		else
		{
			m_FrontWeapons[type].FitWeaponToShip();
		}
		m_ActiveWeapon = m_FrontWeapons[type];
		m_ActiveWeaponType = type;
	}

	public void SpecialWeaponPickedUp(SpecialWeaponType type)
	{
		switch (type)
		{
		case SpecialWeaponType.Starburst:
			m_SpecialWeapon = new StarBurstSpecialWeapon(this, m_Player);
			m_SpecialWeaponType = SpecialWeaponType.Starburst;
			break;
		case SpecialWeaponType.ShockWave:
			m_SpecialWeapon = new StarBurstSpecialWeapon(this, m_Player);
			m_SpecialWeaponType = SpecialWeaponType.Starburst;
			break;
		case SpecialWeaponType.EMP:
			m_SpecialWeapon = new EMPSpecialWeapon(this, m_Player);
			m_SpecialWeaponType = SpecialWeaponType.EMP;
			break;
		}
	}

	public void RearWeaponPickedUp()
	{
		m_ActiveWeapon.IsRearWeaponFitted = true;
	}

	public void Update()
	{
		for (int num = m_ActiveRounds.Count - 1; num >= 0; num--)
		{
			if (!m_ActiveRounds[num].Update())
			{
				m_ActiveRounds.RemoveAt(num);
			}
		}
	}

	public void Draw()
	{
		foreach (WeaponRound activeRound in m_ActiveRounds)
		{
			activeRound.Draw();
		}
	}

	public Weapon GetWeapon(WeaponType type)
	{
		return m_FrontWeapons[type];
	}

	public void FireRemoteWeapon(WeaponType type, int guncount, ref Vector3 endPos, double expires, object extradata)
	{
		m_FrontWeapons[type].FireRemoteWeapon(guncount, ref endPos, expires, extradata);
	}

	public void FireRemoteWeapon(SpecialWeaponType type, ref Vector3 startPos, double expires)
	{
		SpecialWeapon specialWeapon = null;
		switch (type)
		{
		case SpecialWeaponType.EMP:
			specialWeapon = new EMPSpecialWeapon(this, m_Player);
			break;
		case SpecialWeaponType.Starburst:
			specialWeapon = new StarBurstSpecialWeapon(this, m_Player);
			break;
		}
		specialWeapon.FireRemoteWeapon(ref startPos, expires);
	}

	public bool IsWeaponFitted(WeaponType type)
	{
		return m_FrontWeapons[type].IsWeaponFitted;
	}

	public bool IsWeaponFittedEx(WeaponType type, out int guncount, out int ammo, out bool currentweapon)
	{
		Weapon weapon = m_FrontWeapons[type];
		if (weapon.IsWeaponFitted)
		{
			ammo = weapon.Ammo;
			guncount = weapon.GunCount;
			currentweapon = ((weapon == m_ActiveWeapon) ? true : false);
			return true;
		}
		ammo = 0;
		guncount = 0;
		currentweapon = false;
		return false;
	}
}
