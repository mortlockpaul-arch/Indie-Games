using System.IO;
using Loot.Dungeon;

namespace Loot.Items.Weapons;

public class WeaponGreatAxe : Weapon
{
	private const string weaponName = "Great Axe";

	protected override WeaponSprite WeaponSprite => Loot.Items.WeaponSprite.GreatAxe;

	public WeaponGreatAxe()
		: base("Great Axe")
	{
		setDamage();
	}

	public WeaponGreatAxe(EquipmentTier tier)
		: base(tier, "Great Axe")
	{
		setDamage();
	}

	public WeaponGreatAxe(BinaryReader reader)
		: base(reader, "Great Axe")
	{
	}

	private void setDamage()
	{
		Stats.DMG = new DMGRange((int)Tier * 5 + 10, (int)Tier * 5 + 15);
	}
}
