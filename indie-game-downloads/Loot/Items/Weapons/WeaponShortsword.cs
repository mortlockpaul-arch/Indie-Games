using System.IO;
using Loot.Dungeon;

namespace Loot.Items.Weapons;

public class WeaponShortsword : Weapon
{
	private const string weaponName = "Shortsword";

	protected override WeaponSprite WeaponSprite => Loot.Items.WeaponSprite.Shortsword;

	public WeaponShortsword()
		: base("Shortsword")
	{
		setDamage();
	}

	public WeaponShortsword(EquipmentTier tier)
		: base(tier, "Shortsword")
	{
		setDamage();
	}

	public WeaponShortsword(BinaryReader reader)
		: base(reader, "Shortsword")
	{
	}

	private void setDamage()
	{
		Stats.DMG = new DMGRange((int)Tier * 3 + 5, (int)Tier * 3 + 10);
	}
}
