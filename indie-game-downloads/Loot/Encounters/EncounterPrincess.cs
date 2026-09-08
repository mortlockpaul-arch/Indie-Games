using System.IO;
using Loot.Dungeon;
using Loot.Effects;
using Loot.NPCs;
using Loot.Screens;

namespace Loot.Encounters;

public class EncounterPrincess : Encounter
{
	public EncounterPrincess(Location loc)
		: base(loc)
	{
	}

	public EncounterPrincess(BinaryReader reader)
		: base(reader)
	{
	}

	public override void OnClick()
	{
		base.OnClick();
		Dialog.Display(null, "In the distance, you hear a gentle plea.\n\n\"Oh, who will help me?  Woe is me.  Woe is me!\"", new DialogOption("I should investigate.", investigate), new DialogOption("Not my problem!", end));
	}

	private void investigate()
	{
		Dialog.Display(null, "As you draw closer, you discover a large gilded cage containing a princess in a puffy pink gown.\n\n\"Oh, help me.  Help me, please!\"  She cries as you come closer.", new DialogOption("\"I'll save you!\"", saveyou), new DialogOption("\"What's in it for me?\"", whatsinit));
	}

	private void saveyou()
	{
		Dialog.Display(null, "\"My hero!\"  she cries.  \"How long have I waited for one such as thee?\"\n\nShe runs to the golden bars and puckers her lips.  \"Come!  Give us a kiss!\"", new DialogOption("Ooooh yeah.", oohyeah), new DialogOption("Um, no thanks.", nothanks));
	}

	private void whatsinit()
	{
		Dialog.Display(null, "\"Nothing, if that's going to be your attitude!\"  She says icily.\n\nShe then turns her back to you and stomps her foot angrily.\n\n\"Go away!  I'm going to wait for a REAL hero!\"", new DialogOption("\"Fine with me.  Bye!\"", end), new DialogOption("\"C'mon baby!  I was just kidding.\"", kidding));
	}

	private void kidding()
	{
		Dialog.Display(null, "\"Oh! Tee hee!\"  She giggles.  \"You are so clever!  I thought you were a naughty, naughty man, but you're not!\"\n\nShe runs to the golden bars and puckers her lips.  \"Come!  Give us a kiss!\"", new DialogOption("Ooooh yeah.", oohyeah), new DialogOption("Um, no thanks.", nothanks));
	}

	private void oohyeah()
	{
		DM.Player.Gold -= (1 + DM.Player.Depth / 5) * 25;
		if (DM.Player.Gold < 0)
		{
			DM.Player.Gold = 0;
		}
		Dialog.Display(null, "Your lips lock, and you think for a moment that you can hear the angels singing.  But no, it's just the ringing in your ears from being struck from behind.\n\nAs the world fades to blackness, you see the princess skipping away, hand in hand with an orc and sack of your gold.", new DialogOption("I got mugged by a princess?  How embarassing.", end));
	}

	private void nothanks()
	{
		Dialog.Display(null, "The princess steps back.  \"Well, I never!\"\n\n\"Bruno!\"  She cries.  \"You'll have to kill this one.\"  And then, turning to you, she says with a wink, \"Sorry, love.\"", new DialogOption("Bruno?", bruno));
	}

	private void bruno()
	{
		Location location = DM.Map.FindNearest(DM.Player.Location, DM.Map.IsOpenFloor);
		if (!(location == Location.Zero))
		{
			NPC nPC = new Orc(DM.Player.Depth, location);
			nPC.Elite();
			DM.AddNPC(nPC);
			DM.AddEffect(new FXPoof(nPC));
		}
	}

	private void end()
	{
	}
}
