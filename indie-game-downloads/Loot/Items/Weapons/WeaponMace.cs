using System.IO;
using Loot.Dungeon;

namespace Loot.Items.Weapons;

public class WeaponMace : Weapon
{
	private const string weaponName = "Mace";

	protected override WeaponSprite WeaponSprite => Loot.Items.WeaponSprite.Mace;

	public WeaponMace()
		: base("Mace")
	{
		setDamage();
	}

	public WeaponMace(EquipmentTier tier)
		: base(tier, "Mace")
	{
		setDamage();
	}

	public WeaponMace(BinaryReader reader)
		: base(reader, "Mace")
	{
	}

	private void setDamage()
	{
		Stats.DMG = new DMGRange((int)Tier * 3 + 10, (int)Tier * 3 + 10);
	}
}
