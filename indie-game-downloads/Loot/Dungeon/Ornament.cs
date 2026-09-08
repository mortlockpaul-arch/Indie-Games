using Eyehook.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Dungeon;

public class Ornament
{
	private const int length = 4;

	public int Id;

	private Sprite sprite;

	private static Ornament[] gore;

	private static Texture2D texture;

	private Ornament(int id)
	{
		Id = id;
		sprite = new StillSprite(texture, new Rectangle(id * 64, 0, 64, 64), new Vector2(32f, 32f));
	}

	public static Ornament GetOrnament(int id)
	{
		return gore[id];
	}

	public static Ornament RandomGore()
	{
		return gore[DM.Random.Next(4)];
	}

	public static void LoadContent(ContentManager content)
	{
		texture = content.Load<Texture2D>("Sprites\\Tiles\\Ornaments");
		gore = new Ornament[4];
		for (int i = 0; i < 4; i++)
		{
			gore[i] = new Ornament(i);
		}
	}

	public void Draw(SpriteBatch spriteBatch, Vector2 pos, Color color, float scale)
	{
		sprite.Draw(spriteBatch, pos, color, scale);
	}
}
