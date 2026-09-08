using System.IO;
using Loot.Dungeon;

namespace Loot.Items.Weapons;

public class WeaponClaymore : Weapon
{
	private const string weaponName = "Claymore";

	protected override WeaponSprite WeaponSprite => Loot.Items.WeaponSprite.Claymore;

	public WeaponClaymore()
		: base("Claymore")
	{
		setDamage();
	}

	public WeaponClaymore(EquipmentTier tier)
		: base(tier, "Claymore")
	{
		setDamage();
	}

	public WeaponClaymore(BinaryReader reader)
		: base(reader, "Claymore")
	{
	}

	private void setDamage()
	{
		Stats.DMG = new DMGRange((int)Tier * 5 + 15, (int)Tier * 5 + 20);
	}
}
