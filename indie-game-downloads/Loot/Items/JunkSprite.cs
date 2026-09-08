using Eyehook.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Items;

public static class JunkSprite
{
	public static Sprite Emerald;

	public static Sprite Ruby;

	public static Sprite Sapphire;

	public static void LoadContent(ContentManager content)
	{
		Emerald = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Gems\\Emerald"));
		Ruby = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Gems\\Ruby"));
		Sapphire = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Gems\\Sapphire"));
	}
}
