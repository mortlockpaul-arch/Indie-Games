using System.IO;
using Loot.Dungeon;

namespace Loot.Items.Weapons;

public class WeaponClub : Weapon
{
	private const string weaponName = "Club";

	protected override WeaponSprite WeaponSprite => Loot.Items.WeaponSprite.Club;

	public WeaponClub()
		: base("Club")
	{
		setDamage();
	}

	public WeaponClub(EquipmentTier tier)
		: base(tier, "Club")
	{
		setDamage();
	}

	public WeaponClub(BinaryReader reader)
		: base(reader, "Club")
	{
	}

	private void setDamage()
	{
		Stats.DMG = new DMGRange((int)Tier * 2 + 1, (int)Tier * 2 + 1);
	}
}
