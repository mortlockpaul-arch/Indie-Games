using System.IO;
using Loot.Dungeon;

namespace Loot.Items.Weapons;

public class WeaponLongsword : Weapon
{
	private const string weaponName = "Longsword";

	protected override WeaponSprite WeaponSprite => Loot.Items.WeaponSprite.Longsword;

	public WeaponLongsword()
		: base("Longsword")
	{
		setDamage();
	}

	public WeaponLongsword(EquipmentTier tier)
		: base(tier, "Longsword")
	{
		setDamage();
	}

	public WeaponLongsword(BinaryReader reader)
		: base(reader, "Longsword")
	{
	}

	private void setDamage()
	{
		Stats.DMG = new DMGRange((int)Tier * 4 + 10, (int)Tier * 4 + 15);
	}
}
