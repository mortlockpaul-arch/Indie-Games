using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.Items;
using Loot.Items.Armors;
using Loot.Items.Weapons;
using Loot.Screens;
using Loot.Skills;

namespace Loot.PC;

public class Gambler : Player
{
	public override PlayerClassType ClassType => PlayerClassType.Gambler;

	public override Sprite Portrait => Picture.Gambler;

	public override string ClassName => "Gambler";

	public override PlayerSprite Sprite => PlayerSprite.Gambler;

	public override Skill PrimarySkill => SkillSet.Poison;

	public Gambler(BinaryReader reader)
		: base(reader)
	{
	}

	public Gambler()
	{
		Base.DMG = new DMGRange(3, 3);
		Base.DEF = 3;
		Base.DEX = 5;
		Base.LCK = 0;
		SkillSet.Poison.Level = 2;
		Equipment[0] = new ArmorCloak(EquipmentTier.Basic);
		Equipment[1] = new WeaponDagger(EquipmentTier.Basic);
		CalcStats();
	}

	public override void Intro()
	{
		Dialog.Display("Gambler", "I can't believe that farmer chucked me down a well.\n\nI mean, really!  You'd think he'd be happy to have me courting all three of his daughters...\n\nI'm a bit of a catch, after all!", new DialogOption("Adventure Awaits!"));
	}
}
