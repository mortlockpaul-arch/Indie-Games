using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.Items;
using Loot.Items.Armors;
using Loot.Items.Weapons;
using Loot.Screens;
using Loot.Skills;

namespace Loot.PC;

public class Peasant : Player
{
	public override PlayerClassType ClassType => PlayerClassType.Peasant;

	public override Sprite Portrait => Picture.Peasant;

	public override string ClassName => "Peasant";

	public override PlayerSprite Sprite => PlayerSprite.Peasant;

	public override Skill PrimarySkill => null;

	public Peasant(BinaryReader reader)
		: base(reader)
	{
	}

	public Peasant()
	{
		Base.DMG = new DMGRange(3, 3);
		Base.DEF = 3;
		Base.DEX = 5;
		Base.LCK = 0;
		Equipment[0] = new ArmorRags(EquipmentTier.Basic);
		Equipment[1] = new WeaponCrook(EquipmentTier.Basic);
		CalcStats();
	}

	public override void Intro()
	{
		Dialog.Display("Peasant", "Now, where's me lovely?  It truly ain't like her to go runnin' off like this.\n\nPerhaps she's hidin' down this here hole?  Hmph.\n\nIf she weren't so blessed sweet and wooly, I'd be right miffed.", new DialogOption("Adventure Awaits!"));
	}
}
