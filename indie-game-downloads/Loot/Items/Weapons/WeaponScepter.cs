using System.IO;
using Loot.Dungeon;

namespace Loot.Items.Weapons;

public class WeaponScepter : Weapon
{
	private const string weaponName = "Scepter";

	protected override WeaponSprite WeaponSprite => Loot.Items.WeaponSprite.Scepter;

	public WeaponScepter()
		: base("Scepter")
	{
		setDamage();
	}

	public WeaponScepter(EquipmentTier tier)
		: base(tier, "Scepter")
	{
		setDamage();
	}

	public WeaponScepter(BinaryReader reader)
		: base(reader, "Scepter")
	{
	}

	private void setDamage()
	{
		Stats.DMG = new DMGRange((int)Tier * 3 + 15, (int)Tier * 3 + 15);
	}
}
