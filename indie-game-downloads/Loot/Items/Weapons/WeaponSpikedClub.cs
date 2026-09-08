using System.IO;
using Loot.Dungeon;

namespace Loot.Items.Weapons;

public class WeaponSpikedClub : Weapon
{
	private const string weaponName = "Spiked Club";

	protected override WeaponSprite WeaponSprite => Loot.Items.WeaponSprite.SpikedClub;

	public WeaponSpikedClub()
		: base("Spiked Club")
	{
		setDamage();
	}

	public WeaponSpikedClub(EquipmentTier tier)
		: base(tier, "Spiked Club")
	{
		setDamage();
	}

	public WeaponSpikedClub(BinaryReader reader)
		: base(reader, "Spiked Club")
	{
	}

	private void setDamage()
	{
		Stats.DMG = new DMGRange((int)Tier * 2 + 3, (int)Tier * 2 + 3);
	}
}
