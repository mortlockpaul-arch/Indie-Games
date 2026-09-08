using System.IO;
using Eyehook.Framework;

namespace Loot.Items.Junks;

public class Ruby : Junk
{
	public override string Name => "Ruby";

	public override int Value => 750;

	public override Sprite Sprite => JunkSprite.Ruby;

	public Ruby()
	{
	}

	public Ruby(BinaryReader reader)
		: base(reader)
	{
	}
}
