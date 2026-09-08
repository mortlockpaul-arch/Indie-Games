using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.Effects;
using Microsoft.Xna.Framework;

namespace Loot.Items;

public class Gold : Item
{
	private const string name = "Gold";

	private int value;

	private Color goldTextColor = new Color(255, 255, 51);

	private Color goldTextShadowColor = new Color(204, 153, 0);

	public override string Name => "Gold";

	public override Sprite Sprite => ItemSprite.Gold;

	public override int Value => value;

	public Gold()
	{
		value = DM.Random.Next(DM.Player.Depth + 5) + 1;
	}

	public Gold(int value)
	{
		this.value = value;
	}

	public Gold(BinaryReader reader)
		: base(reader)
	{
	}

	public override void onActivate(int id)
	{
	}

	public override bool onStep()
	{
		DM.Player.Gold += Value;
		PlaySound.Coin();
		DM.AddEffect(FXText.GetFX(DM.Player.Location, '+', Value, " GP", goldTextColor, goldTextShadowColor));
		return true;
	}

	public override void Read(BinaryReader reader)
	{
		value = reader.ReadInt32();
	}

	public override void Write(BinaryWriter writer)
	{
		writer.Write(value);
	}
}
