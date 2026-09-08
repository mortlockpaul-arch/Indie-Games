using Eyehook.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Dungeon;

public static class Picture
{
	public static Sprite Berserker;

	public static Sprite Gambler;

	public static Sprite Shaman;

	public static Sprite Tinkerer;

	public static Sprite Goblin;

	public static Sprite Peasant;

	public static void LoadContent(ContentManager content)
	{
		Berserker = new StillSprite(content.Load<Texture2D>("Sprites\\Portraits\\Berserker"));
		Gambler = new StillSprite(content.Load<Texture2D>("Sprites\\Portraits\\Gambler"));
		Shaman = new StillSprite(content.Load<Texture2D>("Sprites\\Portraits\\Shaman"));
		Tinkerer = new StillSprite(content.Load<Texture2D>("Sprites\\Portraits\\Tinkerer"));
		Goblin = new StillSprite(content.Load<Texture2D>("Sprites\\Portraits\\Goblin"));
		Peasant = new StillSprite(content.Load<Texture2D>("Sprites\\Portraits\\Peasant"));
	}
}
