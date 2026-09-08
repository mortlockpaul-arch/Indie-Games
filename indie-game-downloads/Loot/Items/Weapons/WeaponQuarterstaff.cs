using System.IO;
using Loot.Dungeon;

namespace Loot.Items.Weapons;

public class WeaponQuarterstaff : Weapon
{
	private const string weaponName = "Quarterstaff";

	protected override WeaponSprite WeaponSprite => Loot.Items.WeaponSprite.Quarterstaff;

	public WeaponQuarterstaff()
		: base("Quarterstaff")
	{
		setDamage();
	}

	public WeaponQuarterstaff(EquipmentTier tier)
		: base(tier, "Quarterstaff")
	{
		setDamage();
	}

	public WeaponQuarterstaff(BinaryReader reader)
		: base(reader, "Quarterstaff")
	{
	}

	private void setDamage()
	{
		Stats.DMG = new DMGRange((int)Tier * 2 + 5, (int)Tier * 2 + 5);
	}
}
