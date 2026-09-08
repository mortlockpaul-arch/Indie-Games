using System.IO;
using Eyehook.Framework;

namespace Loot.Items.Scrolls;

public class ScrollIdentify : Scroll
{
	public override Sprite Sprite => ScrollSprite.Identify;

	public override int Value => 50;

	public ScrollIdentify()
		: base("Identify Scroll")
	{
	}

	public ScrollIdentify(BinaryReader reader)
		: base(reader, "Identify Scroll")
	{
	}
}
