using System.IO;
using Loot.Dungeon;

namespace Loot.Items.Weapons;

public class WeaponCrook : Weapon
{
	private const string weaponName = "Shepherd's Crook";

	protected override WeaponSprite WeaponSprite => Loot.Items.WeaponSprite.Crook;

	public WeaponCrook()
		: base("Shepherd's Crook")
	{
		setDamage();
	}

	public WeaponCrook(EquipmentTier tier)
		: base(tier, "Shepherd's Crook")
	{
		setDamage();
	}

	public WeaponCrook(BinaryReader reader)
		: base(reader, "Shepherd's Crook")
	{
	}

	private void setDamage()
	{
		Stats.DMG = new DMGRange((int)Tier * 2 + 2, (int)Tier * 2 + 2);
	}
}
