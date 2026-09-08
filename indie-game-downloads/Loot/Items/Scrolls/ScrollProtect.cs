using System;
using System.IO;
using Loot.Dungeon;
using Loot.Screens;
using Loot.Statuses;

namespace Loot.Items.Scrolls;

public class ScrollProtect : Scroll
{
	public ScrollProtect()
		: base("Scroll of Protection")
	{
	}

	public ScrollProtect(BinaryReader reader)
		: base(reader, "Scroll of Protection")
	{
	}

	public override void onActivate(int id)
	{
		base.onActivate(id);
		Message.Display("You feel protected!");
		DM.Player.Status.Add(new StatusProtect(TimeSpan.FromSeconds(40.0)));
	}
}
