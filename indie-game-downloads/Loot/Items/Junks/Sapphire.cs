using System.IO;
using Eyehook.Framework;

namespace Loot.Items.Junks;

public class Sapphire : Junk
{
	public override string Name => "Sapphire";

	public override int Value => 500;

	public override Sprite Sprite => JunkSprite.Sapphire;

	public Sapphire()
	{
	}

	public Sapphire(BinaryReader reader)
		: base(reader)
	{
	}
}
