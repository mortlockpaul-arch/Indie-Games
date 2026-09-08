using System.IO;
using Loot.Dungeon;
using Loot.Screens;

namespace Loot.Items.Scrolls;

public class ScrollMap : Scroll
{
	public ScrollMap()
		: base("Magic Map")
	{
	}

	public ScrollMap(BinaryReader reader)
		: base(reader, "Magic Map")
	{
	}

	public override void onActivate(int id)
	{
		base.onActivate(id);
		Message.Display("You become aware of your surroundings.");
		for (int i = 1; i < DM.Map.Rows - 1; i++)
		{
			for (int j = 1; j < DM.Map.Cols - 1; j++)
			{
				Location location = new Location(i, j);
				if (DM.CanMove(DM.Player, location))
				{
					DM.Discover(location);
				}
			}
		}
	}
}
