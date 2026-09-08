using System.IO;
using Loot.Awardments;
using Loot.Dungeon;
using Loot.Items.Junks;
using Loot.Screens;

namespace Loot.Encounters;

public class EncounterIdol : Encounter
{
	public EncounterIdol(Location loc)
		: base(loc)
	{
	}

	public EncounterIdol(BinaryReader reader)
		: base(reader)
	{
	}

	public override void OnClick()
	{
		base.OnClick();
		Dialog.Display(null, "You discover an ancient stone idol sitting upon a throne of skulls.\n\nHis glinting red eyes are made with bright rubies, and a blue sapphire glows upon his brow.", new DialogOption("Those gems would look better in my pocket.", loot), new DialogOption("I should honor this ancient god.", pray), new DialogOption("I have no use for gods or gems."));
	}

	private void loot()
	{
		if (base.goodOutcome)
		{
			Dialog.Display(null, "You greedily pry the glittering jewels from the old idol, waiting for a crash of thunder or a booming voice.\n\nBut, all you hear is you own gleeful cackle as you stuff the gems into your pocket.", new DialogOption("Muahaha..."));
			DM.Player.AddItemOrDrop(new Ruby());
			DM.Player.AddItemOrDrop(new Ruby());
			DM.Player.AddItemOrDrop(new Sapphire());
		}
		else
		{
			Dialog.Display(null, "You snatch the first ruby from the idol's eye.  It is a fine gem, indeed!\n\nBut as you reach out for the next ruby, the idol's stone hand grabs you and pulls you close.  With a rumbling voice from a distant age, he utters the following terrifying words:", new DialogOption("...", eyeForAnEye));
		}
	}

	private void eyeForAnEye()
	{
		DM.Player.Base.DEX -= 3;
		if (DM.Player.Base.DEX < 0)
		{
			DM.Player.Base.DEX = 0;
		}
		DM.Player.CalcStats();
		DM.Player.HP -= (int)((double)DM.Player.MaxHP * 0.25);
		if (DM.Player.HP < 1)
		{
			DM.Player.HP = 1;
		}
		DM.Player.AddItemOrDrop(new Ruby());
		Dialog.Display(null, "\"An eye for an eye, " + DM.Player.ClassName + ".  As you have mine, I too shall have yours!\"\n\nAnd, with that, the idol pulls you even closer and snatches out your left eye!\n\nYou tumble backwards in horror, clutching your bleeding eye socket! (-3 DEX)");
	}

	private void pray()
	{
		Dialog.Display(null, "You kneel down before the ancient idol and pray.\n\nThe red eyes shine brightly and the idol speaks: \"Long have I waited for sacrifice, " + DM.Player.ClassName + ".  Do you come to offer your blood?\"", new DialogOption("Yes!  I am your slave!", blood), new DialogOption("I think not!", noBlood));
	}

	private void blood()
	{
		Dialog.Display(null, "\"Then let me drink of you!\"\n\nAs you place your hands upon the idol, blood drains from your fingertips and crawls up the sides of the idol into its open stone mouth...\n\n\"Enough!\" The idol booms, before drinking your last drop.  \"You have served me well.\"", new DialogOption("I only live to serve...", blood2));
	}

	private void blood2()
	{
		DM.Player.HP = 1;
		Dialog.Display(null, "\"You shall have a reward.\"  The idol murmurs, eyes aglow.  \"You may now be weak, but you will soon be strong!\"\n\nThe blue sapphire upon his brow glows and a flood of knowledge washes over you in an instant! (+1 level)", new DialogOption("Farewell, great one!", blood3));
		Profile.Awardments.Unlock(Awardment.OldGods);
	}

	private void blood3()
	{
		DM.Player.AddXP(DM.Player.NextXP - DM.Player.PrevXP);
		DM.Player.HP = 1;
	}

	public void noBlood()
	{
		DM.Player.HP = 1;
		Dialog.Display(null, "The idol's red eyes flare in anger.\n\n\"Your hollow worship in hope of gain shall end in pain... Yes, much pain!\"\n\nYou collapse to the ground in screaming agony until all is blackness...", new DialogOption("I won't try that again!"));
	}
}
