using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.Items;
using Loot.Items.Armors;
using Loot.Items.Weapons;
using Loot.Screens;
using Loot.Skills;

namespace Loot.PC;

public class Berserker : Player
{
	public override PlayerClassType ClassType => PlayerClassType.Berserker;

	public override Sprite Portrait => Picture.Berserker;

	public override string ClassName => "Berserker";

	public override PlayerSprite Sprite => PlayerSprite.Berserker;

	public override Skill PrimarySkill => SkillSet.Frenzy;

	public Berserker(BinaryReader reader)
		: base(reader)
	{
	}

	public Berserker()
	{
		Base.DMG = new DMGRange(3, 3);
		Base.DEF = 3;
		Base.DEX = 5;
		Base.LCK = 0;
		SkillSet.Frenzy.Level = 2;
		Equipment[0] = new ArmorHarness(EquipmentTier.Basic);
		Equipment[1] = new WeaponDagger(EquipmentTier.Basic);
		CalcStats();
	}

	public override void Intro()
	{
		Dialog.Display("Berserker", "Oh, me aching head!  I haven't had a hangover this bad since me sixth bachelor party (this bein' me seventh, of course)... Arr, good times.  Good times.\n\nNow, where might I be?  No matter.  Me trusty blade'll guide me back home...", new DialogOption("Adventure Awaits!"));
	}
}
