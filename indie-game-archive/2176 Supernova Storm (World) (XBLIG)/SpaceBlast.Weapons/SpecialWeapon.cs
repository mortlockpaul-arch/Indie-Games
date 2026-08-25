using Microsoft.Xna.Framework;

namespace SpaceBlast.Weapons;

internal abstract class SpecialWeapon
{
	protected Player m_Player;

	protected Ship m_Ship;

	protected WeaponSystem m_WeaponSystem;

	public SpecialWeapon(WeaponSystem weaponSystem, Player player)
	{
		m_WeaponSystem = weaponSystem;
		m_Player = player;
		m_Ship = player.TheShip;
	}

	public abstract void FireWeapon();

	public abstract void FireRemoteWeapon(ref Vector3 startPos, double expires);
}
