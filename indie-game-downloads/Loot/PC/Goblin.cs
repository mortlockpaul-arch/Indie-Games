using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.Items;
using Loot.Items.Armors;
using Loot.Items.Weapons;
using Loot.Screens;
using Loot.Skills;

namespace Loot.PC;

public class Goblin : Player
{
	public override PlayerClassType ClassType => PlayerClassType.Goblin;

	public override Sprite Portrait => Picture.Goblin;

	public override string ClassName => "Goblin";

	public override PlayerSprite Sprite => PlayerSprite.Goblin;

	public override Skill PrimarySkill => SkillSet.Regen;

	public Goblin(BinaryReader reader)
		: base(reader)
	{
	}

	public Goblin()
	{
		Base.DMG = new DMGRange(3, 3);
		Base.DEF = 3;
		Base.DEX = 5;
		Base.LCK = 0;
		SkillSet.Regen.Level = 2;
		Equipment[0] = new ArmorRags(EquipmentTier.Basic);
		Equipment[1] = new WeaponClub(EquipmentTier.Basic);
		CalcStats();
	}

	public override void Intro()
	{
		Dialog.Display("Goblin", "Ha!  Gak-Gak think I not good enough for his goobly-woobly daughter.\n\nPfft.  I make big gold and open big shop and charge Gak-Gak big prices!\n\nHeh heh heh.", new DialogOption("Adventure Awaits!"));
	}
}
