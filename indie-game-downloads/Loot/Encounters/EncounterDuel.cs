using System.IO;
using Loot.Dungeon;
using Loot.Effects;
using Loot.NPCs;
using Loot.Screens;

namespace Loot.Encounters;

public class EncounterDuel : Encounter
{
	public EncounterDuel(Location loc)
		: base(loc)
	{
	}

	public EncounterDuel(BinaryReader reader)
		: base(reader)
	{
	}

	public override void OnClick()
	{
		base.OnClick();
		Dialog.Display(null, "You see a rabble of tough-looking creatures shouting \"Fight!  Fight!  Fight!\"", new DialogOption("This looks interesting...", join), new DialogOption("Eek!  They look scary!  Run away!", end));
	}

	private void join()
	{
		Dialog.Display(null, "You shoulder your way into the crowd and see a scrappy goblin dodging and weaving around a punch-drunk orc.\n\nThe goblin leaps into the air and lands a ferocioius blow right between the orc's eyes, sending him staggering to the ground.", new DialogOption("Hurray!", hurray));
	}

	private void hurray()
	{
		Dialog.Display(null, "The goblin cracks his knuckles and lifts a sack of coins from the dazed orc, jingling it in front of his battered face.\n\n\"Anybody else want to try?\"", new DialogOption("Ooh!  Me!  Me!  Me!", mememe), new DialogOption("Not me!  Walk away.", end));
	}

	private void mememe()
	{
		Dialog.Display(null, "You push your way forward, and the goblin eyes you with a grin.\n\n\"All right then, " + DM.Player.ClassName + ".\"  He says with a sneer and tosses you the bag of coins.", new DialogOption("Bring it!", bringit));
	}

	private void bringit()
	{
		DM.Player.Gold += (1 + DM.Player.Depth / 5) * 25;
		Location location = DM.Map.FindNearest(DM.Player.Location, DM.Map.IsOpenFloor);
		if (!(location == Location.Zero))
		{
			NPC nPC = new Goblin(DM.Player.Depth, location);
			nPC.Boss();
			DM.AddNPC(nPC);
			DM.AddEffect(new FXPoof(nPC));
		}
	}

	private void end()
	{
	}
}
