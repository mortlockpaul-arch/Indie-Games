using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.Screens;

namespace Loot.Widgets;

public class WidgetShop : WidgetAction
{
	private Shop shop;

	protected override Sprite Sprite => WidgetSprite.Shop;

	public WidgetShop(Location loc, Shop shop)
		: base(loc)
	{
		this.shop = shop;
	}

	public WidgetShop(BinaryReader reader)
		: base(reader)
	{
	}

	public override void OnClick()
	{
		base.OnClick();
		ShopScreen.Display(shop);
	}

	public override void Read(BinaryReader reader)
	{
		base.Read(reader);
		shop = new Shop(reader);
	}

	public override void Write(BinaryWriter writer)
	{
		base.Write(writer);
		shop.Write(writer);
	}
}
