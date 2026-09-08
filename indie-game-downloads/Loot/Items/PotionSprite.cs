using Eyehook.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Items;

public static class PotionSprite
{
	private const int miscCount = 6;

	public static Sprite Health;

	public static Sprite Oil;

	public static Sprite[] Misc = new Sprite[6];

	public static void LoadContent(ContentManager content)
	{
		Health = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Potion\\Health"));
		Oil = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Potion\\Oil"));
		Misc[0] = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Potion\\Blue"));
		Misc[1] = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Potion\\Brown"));
		Misc[2] = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Potion\\Cyan"));
		Misc[3] = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Potion\\Gold"));
		Misc[4] = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Potion\\Green"));
		Misc[5] = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Potion\\Purple"));
	}
}
