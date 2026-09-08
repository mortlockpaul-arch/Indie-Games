using Eyehook.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Items;

public static class ItemSprite
{
	public static Sprite Gold;

	public static Sprite GP;

	public static Sprite CharmResurrect;

	public static void LoadContent(ContentManager content)
	{
		Gold = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Gold"));
		GP = new StillSprite(content.Load<Texture2D>("Sprites\\UI\\GP"));
		CharmResurrect = new StillSprite(content.Load<Texture2D>("Sprites\\Items\\Misc\\CharmResurrect"));
	}
}
