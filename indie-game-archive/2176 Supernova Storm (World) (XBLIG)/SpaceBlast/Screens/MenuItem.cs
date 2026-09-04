using System.Collections.Generic;

namespace SpaceBlast.Screens;

internal class MenuItem
{
	public string MenuText;

	public int ItemID;

	public List<MenuItem> Values;

	public MenuItem(string text, int id)
	{
		MenuText = text;
		ItemID = id;
		Values = null;
	}
}
