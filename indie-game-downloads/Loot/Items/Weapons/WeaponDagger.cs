using System.IO;
using Loot.Dungeon;

namespace Loot.Items.Weapons;

public class WeaponDagger : Weapon
{
	private const string weaponName = "Dagger";

	protected override WeaponSprite WeaponSprite => Loot.Items.WeaponSprite.Dagger;

	public WeaponDagger()
		: base("Dagger")
	{
		setDamage();
	}

	public WeaponDagger(EquipmentTier tier)
		: base(tier, "Dagger")
	{
		setDamage();
	}

	public WeaponDagger(BinaryReader reader)
		: base(reader, "Dagger")
	{
	}

	private void setDamage()
	{
		Stats.DMG = new DMGRange((int)Tier * 2 + 1, (int)Tier * 2 + 5);
	}
}
