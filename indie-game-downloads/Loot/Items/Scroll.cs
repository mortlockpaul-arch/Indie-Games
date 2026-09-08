using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;

namespace Loot.Items;

public class Scroll : Item
{
	private string name;

	private Sprite sprite;

	public override string Name => (!Identified) ? "Scroll (?)" : name;

	public override int Value => 100;

	public override Sprite Sprite
	{
		get
		{
			if (sprite != null)
			{
				return sprite;
			}
			sprite = DM.ScrollMaster.GetSprite(GetType());
			return sprite;
		}
	}

	public override bool Identified => DM.ScrollMaster.Identified(GetType());

	public Scroll(string name)
	{
		this.name = name;
	}

	public Scroll(BinaryReader reader, string name)
		: base(reader)
	{
		this.name = name;
	}

	public override void Identify()
	{
		DM.ScrollMaster.Identify(GetType());
	}

	public override void onActivate(int id)
	{
		Identify();
		DM.Player.RemoveItem(id);
	}
}
