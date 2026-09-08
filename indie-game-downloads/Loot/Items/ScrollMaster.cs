using System.IO;
using Eyehook.Framework;
using Loot.Items.Scrolls;

namespace Loot.Items;

public class ScrollMaster : GenericMaster
{
	protected override Sprite[] MiscSprites => ScrollSprite.Misc;

	public ScrollMaster(BinaryReader reader)
		: base(reader)
	{
	}

	public ScrollMaster()
	{
		Identify(typeof(ScrollIdentify));
	}
}
