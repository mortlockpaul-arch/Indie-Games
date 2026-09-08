using System.IO;
using Loot.Dungeon;

namespace Loot.Items.Weapons;

public class WeaponSpikedHammer : Weapon
{
	private const string weaponName = "Spiked Hammer";

	protected override WeaponSprite WeaponSprite => Loot.Items.WeaponSprite.SpikedHammer;

	public WeaponSpikedHammer()
		: base("Spiked Hammer")
	{
		setDamage();
	}

	public WeaponSpikedHammer(EquipmentTier tier)
		: base(tier, "Spiked Hammer")
	{
		setDamage();
	}

	public WeaponSpikedHammer(BinaryReader reader)
		: base(reader, "Spiked Hammer")
	{
	}

	private void setDamage()
	{
		Stats.DMG = new DMGRange((int)Tier * 5 + 20, (int)Tier * 5 + 20);
	}
}
