using System.IO;
using Eyehook.Framework;
using Loot.Awardments;
using Loot.Dungeon;
using Loot.Screens;
using Microsoft.Xna.Framework;

namespace Loot.Widgets;

public class WidgetTombstone : WidgetAction
{
	private bool looted;

	private Tombstone tombstone;

	private static Color textColor = new Color(255, 255, 255);

	private static Color shadowColor = new Color(0, 0, 0);

	protected override Sprite Sprite => WidgetSprite.Tombstone;

	public WidgetTombstone(BinaryReader reader)
		: base(reader)
	{
	}

	public WidgetTombstone(Location loc, Tombstone tombstone)
		: base(loc)
	{
		this.tombstone = tombstone;
	}

	public override void OnMove()
	{
		if (!looted)
		{
			base.OnMove();
		}
	}

	public override void OnClick()
	{
		if (!looted)
		{
			looted = true;
			base.OnClick();
			PlaySound.Encounter();
			if (tombstone.Item == null)
			{
				MC.ScreenManager.addScreen(new RippleScreen(noItem));
			}
			else
			{
				MC.ScreenManager.addScreen(new RippleScreen(hasItem));
			}
		}
	}

	private void noItem()
	{
		Dialog.Display("A Forgotten Grave", "You discover the grave of " + tombstone.Name + " and take a few moments to honor your fallen comrade.\n\nHopefully, you won't meet the same fate!", new DialogOption("Sniff... Sniff...", unlockAwardment));
	}

	private void hasItem()
	{
		DM.Player.AddItemOrDrop(tombstone.Item);
		Dialog.Display("A Forgotten Grave", "You discover the grave of " + tombstone.Name + " and take a few moments to honor your fallen comrade.\n\nAs you step away, you notice his " + tombstone.Item.Name + " lying nearby.  It will serve you better than him, now.", new DialogOption("Your death will be avenged!", unlockAwardment));
	}

	private void unlockAwardment()
	{
		if (tombstone.Name != null && tombstone.Name.Equals(DM.Player.Name))
		{
			Profile.Awardments.Unlock(Awardment.Spooky);
		}
	}

	public override void Read(BinaryReader reader)
	{
		base.Read(reader);
		looted = reader.ReadBoolean();
		tombstone = new Tombstone(reader);
	}

	public override void Write(BinaryWriter writer)
	{
		base.Write(writer);
		writer.Write(looted);
		tombstone.Write(writer);
	}
}
