using System.IO;
using Loot.Dungeon;

namespace Loot.Items.Weapons;

public class WeaponSai : Weapon
{
	private const string weaponName = "Sai";

	protected override WeaponSprite WeaponSprite => Loot.Items.WeaponSprite.Sai;

	public WeaponSai()
		: base("Sai")
	{
		setDamage();
	}

	public WeaponSai(EquipmentTier tier)
		: base(tier, "Sai")
	{
		setDamage();
	}

	public WeaponSai(BinaryReader reader)
		: base(reader, "Sai")
	{
	}

	private void setDamage()
	{
		Stats.DMG = new DMGRange((int)Tier * 4 + 10, (int)Tier * 4 + 20);
	}
}
