using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.Items;
using Loot.Items.Armors;
using Loot.Items.Weapons;
using Loot.Screens;
using Loot.Skills;

namespace Loot.PC;

public class Shaman : Player
{
	public override PlayerClassType ClassType => PlayerClassType.Shaman;

	public override Sprite Portrait => Picture.Shaman;

	public override string ClassName => "Shaman";

	public override PlayerSprite Sprite => PlayerSprite.Shaman;

	public override Skill PrimarySkill => SkillSet.Freeze;

	public Shaman(BinaryReader reader)
		: base(reader)
	{
	}

	public Shaman()
	{
		Base.DMG = new DMGRange(3, 3);
		Base.DEF = 3;
		Base.DEX = 5;
		Base.LCK = 0;
		SkillSet.Freeze.Level = 2;
		Equipment[0] = new ArmorRags(EquipmentTier.Basic);
		Equipment[1] = new WeaponClub(EquipmentTier.Basic);
		CalcStats();
	}

	public override void Intro()
	{
		Dialog.Display("Shaman", "The earth suffers, but I will bring her peace.\n\nI know what lurks beneath, and I have no fear.  My magic is great, and my will is strong.\n\nTremble, foul creatures, for I come for you!", new DialogOption("Adventure Awaits!"));
	}
}
