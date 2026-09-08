using Eyehook.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Items;

public static class RingSprite
{
	public static Sprite Gold;

	public static Sprite Ruby;

	public static Sprite Emerald;

	public static Sprite Sapphire;

	public static Sprite Amethyst;

	public static Sprite Diamond;

	public static Sprite Signet;

	public static Sprite Royal;

	public static void LoadContent(ContentManager content)
	{
		Gold = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Ring\\Gold"));
		Ruby = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Ring\\Ruby"));
		Emerald = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Ring\\Emerald"));
		Sapphire = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Ring\\Sapphire"));
		Amethyst = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Ring\\Amethyst"));
		Diamond = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Ring\\Diamond"));
		Signet = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Ring\\Signet"));
		Royal = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Ring\\Royal"));
	}
}
