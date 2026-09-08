using Eyehook.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Items;

public static class AmuletSprite
{
	public static Sprite Rune;

	public static Sprite Leather;

	public static Sprite Wooden;

	public static Sprite Iron;

	public static Sprite Silver;

	public static Sprite Lapis;

	public static Sprite Gold;

	public static Sprite Bone;

	public static void LoadContent(ContentManager content)
	{
		Rune = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Amulet\\Rune"));
		Leather = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Amulet\\Leather"));
		Wooden = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Amulet\\Wooden"));
		Iron = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Amulet\\Iron"));
		Silver = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Amulet\\Silver"));
		Lapis = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Amulet\\Lapis"));
		Gold = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Amulet\\Gold"));
		Bone = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Amulet\\Bone"));
	}
}
