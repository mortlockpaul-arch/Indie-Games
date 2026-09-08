using Eyehook.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Items;

public static class ScrollSprite
{
	private const int miscCount = 7;

	public static Sprite Identify;

	public static Sprite[] Misc = new Sprite[7];

	public static void LoadContent(ContentManager content)
	{
		Identify = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Scroll\\Identify"));
		Misc[0] = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Scroll\\Black"));
		Misc[1] = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Scroll\\Blue"));
		Misc[2] = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Scroll\\Brown"));
		Misc[3] = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Scroll\\Gold"));
		Misc[4] = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Scroll\\Green"));
		Misc[5] = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Scroll\\Purple"));
		Misc[6] = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Scroll\\Red"));
	}
}
