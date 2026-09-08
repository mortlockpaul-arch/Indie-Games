using System.IO;
using Eyehook.Framework;

namespace Loot.Items.Junks;

public class Emerald : Junk
{
	public override string Name => "Emerald";

	public override int Value => 250;

	public override Sprite Sprite => JunkSprite.Emerald;

	public Emerald()
	{
	}

	public Emerald(BinaryReader reader)
		: base(reader)
	{
	}
}
