using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;

namespace Loot.Items;

public abstract class Potion : Item
{
	private string name;

	private Sprite sprite;

	public override string Name => (!Identified) ? "Potion (?)" : name;

	protected abstract int RealValue { get; }

	public override int Value => (!Identified) ? 100 : RealValue;

	public override Sprite Sprite
	{
		get
		{
			if (sprite != null)
			{
				return sprite;
			}
			sprite = DM.PotionMaster.GetSprite(GetType());
			return sprite;
		}
	}

	public override bool Identified => DM.PotionMaster.Identified(GetType());

	public Potion(string name)
	{
		this.name = name;
	}

	public Potion(BinaryReader reader, string name)
		: base(reader)
	{
		this.name = name;
	}

	public override void Identify()
	{
		DM.PotionMaster.Identify(GetType());
	}

	public override void onActivate(int id)
	{
		Identify();
		PlaySound.Potion();
		DM.Player.RemoveItem(id);
	}
}
