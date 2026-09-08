using System.IO;
using Loot.Dungeon;
using Loot.Effects;
using Loot.Items;
using Loot.Items.Junks;
using Loot.Screens;
using Loot.Widgets;

namespace Loot.Encounters;

public class EncounterCollapsingFloor : Encounter
{
	private Item item;

	public EncounterCollapsingFloor(Location loc)
		: base(loc)
	{
	}

	public EncounterCollapsingFloor(BinaryReader reader)
		: base(reader)
	{
	}

	public override void OnClick()
	{
		base.OnClick();
		switch (DM.Random.Next(3))
		{
		case 0:
			item = new Ruby();
			break;
		case 1:
			item = new Sapphire();
			break;
		case 2:
			item = new Emerald();
			break;
		}
		Dialog.Display(null, "Something on the floor catches your eye, and you bend down to examine it more closely.\n\nWell, well!  It looks like a " + item.Name + ".  It should be easy enough to remove.", new DialogOption("Dig it out.", dig), new DialogOption("Leave it."));
	}

	private void dig()
	{
		if (base.goodOutcome || base.depth >= 50)
		{
			getItem();
		}
		else
		{
			fall();
		}
	}

	private void getItem()
	{
		Dialog.Display(null, "You scrape around its edges, and after a bit more effort than you expected, you pull the " + item.Name + " from the floor.\n\nWith a grin, you stuff it into your pocket and go along your merry way.", new DialogOption("Cha-ching!"));
		DM.Player.AddItemOrDrop(item);
	}

	private void fall()
	{
		DM.Map.SetWidget(new WidgetHole(Location));
		Dialog.Display(null, "Wow, that thing is really stuck.\n\nIn frustration, you kick it with all your might.\n\nThe earth beneath you trembles and then suddenly gives way, sending you tumbling into the abyss...", new DialogOption("Aaaaaaah...", GoDown));
	}

	private void GoDown()
	{
		SaveScreen.GoToDepth(DM.Player.Depth + 1, () => DM.Map.FindNearest(DM.Player.Location, DM.Map.IsOpenFloor), delegate
		{
			DM.AddEffect(new FXDustCloud(DM.Player.Location));
		});
	}
}
