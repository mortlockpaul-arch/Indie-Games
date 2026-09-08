using System.IO;
using Loot.Dungeon;

namespace Loot.Items.Weapons;

public class WeaponPike : Weapon
{
	private const string weaponName = "Pike";

	protected override WeaponSprite WeaponSprite => Loot.Items.WeaponSprite.Pike;

	public WeaponPike()
		: base("Pike")
	{
		setDamage();
	}

	public WeaponPike(EquipmentTier tier)
		: base(tier, "Pike")
	{
		setDamage();
	}

	public WeaponPike(BinaryReader reader)
		: base(reader, "Pike")
	{
	}

	private void setDamage()
	{
		Stats.DMG = new DMGRange((int)Tier * 2, (int)Tier * 2 + 15);
	}
}
