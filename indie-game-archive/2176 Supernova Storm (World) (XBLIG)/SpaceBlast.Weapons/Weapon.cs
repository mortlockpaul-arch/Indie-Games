using Microsoft.Xna.Framework;

namespace SpaceBlast.Weapons;

internal abstract class Weapon
{
	protected Player m_Player;

	protected Ship m_Ship;

	protected int m_Ammo;

	protected int m_GunCount;

	protected bool m_IsWeaponFitted;

	public bool IsRearWeaponFitted;

	protected double m_EarliestNextShot;

	protected WeaponSystem m_WeaponSystem;

	public int Ammo => m_Ammo;

	public int GunCount => m_GunCount;

	public bool IsWeaponFitted => m_IsWeaponFitted;

	public double EarliestNextShot => m_EarliestNextShot;

	public Weapon(WeaponSystem weaponSystem, Player player)
	{
		m_WeaponSystem = weaponSystem;
		m_Player = player;
		m_Ship = player.TheShip;
	}

	public abstract void FireWeapon();

	public abstract void FireRearWeapon();

	public abstract void FireRemoteWeapon(int guncount, ref Vector3 endPos, double endtime, object extradata);

	public abstract void ApplyWeaponUpgrade();

	public abstract void IncreaseFireRate();

	public abstract void ApplyAmmoPack();

	public abstract void FitWeaponToShip();
}
