using System.IO;
using Loot.Dungeon;

namespace Loot.Items.Weapons;

public class WeaponDirk : Weapon
{
	private const string weaponName = "Dirk";

	protected override WeaponSprite WeaponSprite => Loot.Items.WeaponSprite.Dirk;

	public WeaponDirk()
		: base("Dirk")
	{
		setDamage();
	}

	public WeaponDirk(EquipmentTier tier)
		: base(tier, "Dirk")
	{
		setDamage();
	}

	public WeaponDirk(BinaryReader reader)
		: base(reader, "Dirk")
	{
	}

	private void setDamage()
	{
		Stats.DMG = new DMGRange((int)Tier * 3 + 5, (int)Tier * 3 + 15);
	}
}
