using System.IO;
using Loot.Dungeon;

namespace Loot.Items.Weapons;

public class WeaponJeweledClaymore : Weapon
{
	private const string weaponName = "Jeweled Claymore";

	protected override WeaponSprite WeaponSprite => Loot.Items.WeaponSprite.JeweledClaymore;

	public WeaponJeweledClaymore()
		: base("Jeweled Claymore")
	{
		setDamage();
	}

	public WeaponJeweledClaymore(EquipmentTier tier)
		: base(tier, "Jeweled Claymore")
	{
		setDamage();
	}

	public WeaponJeweledClaymore(BinaryReader reader)
		: base(reader, "Jeweled Claymore")
	{
	}

	private void setDamage()
	{
		Stats.DMG = new DMGRange((int)Tier * 5 + 20, (int)Tier * 5 + 25);
	}
}
