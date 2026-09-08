using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.Items;
using Loot.Items.Armors;
using Loot.Items.Weapons;
using Loot.Screens;
using Loot.Skills;

namespace Loot.PC;

public class Tinkerer : Player
{
	public override PlayerClassType ClassType => PlayerClassType.Tinkerer;

	public override Sprite Portrait => Picture.Tinkerer;

	public override string ClassName => "Tinkerer";

	public override PlayerSprite Sprite => PlayerSprite.Tinkerer;

	public override Skill PrimarySkill => SkillSet.Orb;

	public Tinkerer(BinaryReader reader)
		: base(reader)
	{
	}

	public Tinkerer()
	{
		Base.DMG = new DMGRange(3, 3);
		Base.DEF = 3;
		Base.DEX = 5;
		Base.LCK = 0;
		SkillSet.Orb.Level = 2;
		Equipment[0] = new ArmorClothes(EquipmentTier.Basic);
		Equipment[1] = new WeaponClub(EquipmentTier.Basic);
		CalcStats();
	}

	public override void Intro()
	{
		Dialog.Display("Tinkerer", "Goodness!\n\nIt looks like Mother has drugged me and buried me alive again...  Oh, how she loves to play tricks on me!  Tee hee!\n\nBut, oh!  What a perfect place to test my latest invention...", new DialogOption("Adventure Awaits!"));
	}
}
