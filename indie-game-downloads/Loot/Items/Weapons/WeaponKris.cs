using System.IO;
using Loot.Dungeon;

namespace Loot.Items.Weapons;

public class WeaponKris : Weapon
{
	private const string weaponName = "Kris";

	protected override WeaponSprite WeaponSprite => Loot.Items.WeaponSprite.Kris;

	public WeaponKris()
		: base("Kris")
	{
		setDamage();
	}

	public WeaponKris(EquipmentTier tier)
		: base(tier, "Kris")
	{
		setDamage();
	}

	public WeaponKris(BinaryReader reader)
		: base(reader, "Kris")
	{
	}

	private void setDamage()
	{
		Stats.DMG = new DMGRange((int)Tier * 2 + 2, (int)Tier * 2 + 6);
	}
}
