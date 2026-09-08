using System;
using System.IO;
using Loot.Dungeon;
using Loot.Screens;
using Loot.Widgets;

namespace Loot.Items.Scrolls;

public class ScrollTeleport : Scroll
{
	private bool goShop;

	private bool goExitUp;

	private bool goExitDown;

	private int targetDepth = 1;

	public ScrollTeleport()
		: base("Teleport Scroll")
	{
	}

	public ScrollTeleport(BinaryReader reader)
		: base(reader, "Teleport Scroll")
	{
	}

	public override void onActivate(int id)
	{
		base.onActivate(id);
		if (DM.Player.Depth == 1)
		{
			Dialog.Display("Teleport", null, new DialogOption("Nearest Shop", teleportShop), new DialogOption("One Level Down", teleportDown));
		}
		else if (DM.Player.Depth >= 50)
		{
			Dialog.Display("Teleport", null, new DialogOption("Nearest Shop", teleportShop), new DialogOption("One Level Up", teleportUp));
		}
		else
		{
			Dialog.Display("Teleport", null, new DialogOption("Nearest Shop", teleportShop), new DialogOption("One Level Up", teleportUp), new DialogOption("One Level Down", teleportDown));
		}
	}

	private void teleportShop()
	{
		if (DM.Player.Depth <= 3)
		{
			targetDepth = 3;
		}
		else if (DM.Player.Depth >= 48)
		{
			targetDepth = 48;
		}
		else
		{
			switch (DM.Player.Depth % 3)
			{
			case 0:
				targetDepth = DM.Player.Depth;
				break;
			case 1:
				targetDepth = DM.Player.Depth - 1;
				break;
			case 2:
				targetDepth = DM.Player.Depth + 1;
				break;
			}
		}
		goShop = true;
		teleport();
	}

	private void teleportUp()
	{
		if (DM.Player.Depth == 1)
		{
			Message.Display("You can't do that...");
			return;
		}
		goExitDown = true;
		targetDepth = DM.Player.Depth - 1;
		teleport();
	}

	private void teleportDown()
	{
		if (DM.Player.Depth + 1 >= 51)
		{
			Message.Display("You can't do that...");
			return;
		}
		goExitUp = true;
		targetDepth = DM.Player.Depth + 1;
		teleport();
	}

	private void teleport()
	{
		SaveScreen.GoToDepth(targetDepth, teleportLoc);
	}

	private Location teleportLoc()
	{
		if (goShop)
		{
			return shopLoc();
		}
		if (goExitUp)
		{
			return DM.Map.ExitUp;
		}
		if (goExitDown)
		{
			return DM.Map.ExitDown;
		}
		throw new Exception("Unknown teleport destination...");
	}

	private Location shopLoc()
	{
		for (int i = 0; i < DM.Map.Rows; i++)
		{
			for (int j = 0; j < DM.Map.Cols; j++)
			{
				Location location = new Location(i, j);
				Widget widget = DM.Map.GetWidget(location);
				if (widget != null && widget is WidgetShop)
				{
					return location;
				}
			}
		}
		return DM.Map.FindRandom(DM.Map.IsOpenFloor);
	}
}
