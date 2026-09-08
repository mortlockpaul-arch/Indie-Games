using System.IO;
using Loot.Dungeon;

namespace Loot.Items.Weapons;

public class WeaponWarHammer : Weapon
{
	private const string weaponName = "War Hammer";

	protected override WeaponSprite WeaponSprite => Loot.Items.WeaponSprite.WarHammer;

	public WeaponWarHammer()
		: base("War Hammer")
	{
		setDamage();
	}

	public WeaponWarHammer(EquipmentTier tier)
		: base(tier, "War Hammer")
	{
		setDamage();
	}

	public WeaponWarHammer(BinaryReader reader)
		: base(reader, "War Hammer")
	{
	}

	private void setDamage()
	{
		Stats.DMG = new DMGRange((int)Tier * 5 + 15, (int)Tier * 5 + 15);
	}
}
