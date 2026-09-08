using System.IO;
using Loot.Dungeon;
using Loot.Effects;
using Loot.Items;
using Loot.NPCs;
using Loot.Screens;

namespace Loot.Encounters;

public class EncounterSpiderQueen : Encounter
{
	public EncounterSpiderQueen(Location loc)
		: base(loc)
	{
	}

	public EncounterSpiderQueen(BinaryReader reader)
		: base(reader)
	{
	}

	public override void OnClick()
	{
		base.OnClick();
		Dialog.Display(null, "A giant cobweb stretches out before you, and, in its center, a tiny winged creature struggles impossibly against its sticky bonds.", new DialogOption("Poor thing.  I should help it.", help), new DialogOption("It's the circle of life.  Leave it be."));
	}

	private void help()
	{
		if (base.goodOutcome)
		{
			fairy();
		}
		else
		{
			spiderQueen();
		}
	}

	private void fairy()
	{
		Dialog.Display(null, "You move closer and discover that the struggling creature is a tiny fairy.  It's eyes turn toward you desperately.\n\n\"Quick!  Quick!\" It cries.", new DialogOption("Free the fairy.", freeIt), new DialogOption("Fairies are nothing but trouble!  Leave it.", leaveIt));
	}

	private void freeIt()
	{
		Dialog.Display(null, "You gently pull the fairy away from the web, hoping its wings don't tear off.\n\nAfter a few yelps from the fairy, you finally release it, wings and all.", new DialogOption("There you go!", freeIt2));
	}

	private void freeIt2()
	{
		Weapon weapon = DM.Player.Weapon;
		if (weapon == null)
		{
			Dialog.Display(null, "\"Thanks!\" Squeaks the fairy as it flutters off into the darkness.");
			return;
		}
		Dialog.Display(null, "\"Phew!\" Squeaks the fairy.  \"I thought I was a goner!\"\n\nWith a flutter, the fairy flies to your weapon and gives it a kiss. (+2 Weapon DMG)\n\nAnd, with no further ado, the fairy flits off into the darkness.", new DialogOption("Farewell!"));
		DMGRange dMG = weapon.Stats.DMG;
		weapon.Stats.DMG = new DMGRange(dMG.Min + 2, dMG.Max + 2);
		DM.Player.CalcStats();
	}

	private void leaveIt()
	{
		Dialog.Display(null, "You give the fairy a helpless shrug.\n\n\"Nooo...\" Cries the fairy, but you know better.\n\nAs you turn away, you see a large spider begin to pick its way toward its new prey...", new DialogOption("Farwell, little fairy."));
	}

	private void spiderQueen()
	{
		Dialog.Display(null, "As you approach the web you feel a hundred eyes upon you.\n\nA tingle runs down your spine and you swiftly turn around to see a Spider Queen dangling from a silken thread behind you!", new DialogOption("Fight for you life!", fight));
	}

	private void fight()
	{
		Spider spider = new Spider(DM.Player.Depth + 3, DM.Map.FindNearest(DM.Player.Location, DM.Map.IsOpenFloor));
		spider.Elite();
		DM.AddNPC(spider);
		DM.AddEffect(new FXPoof(spider));
		for (int i = 0; i < 3; i++)
		{
			Spider spider2 = new Spider(DM.Player.Depth, DM.Map.FindNearest(DM.Player.Location, DM.Map.IsOpenFloor));
			DM.AddNPC(spider2);
			DM.AddEffect(new FXPoof(spider2));
		}
	}
}
