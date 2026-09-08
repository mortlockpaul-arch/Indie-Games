using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JamSouls;

public class Sprite
{
	public Rectangle rect;

	public int id;

	public string name;

	private Atlas atlas;

	public int Height => rect.Height;

	public int Width => rect.Width;

	public Sprite(int Id, string Name, Rectangle Rect, Atlas Atlas)
	{
		id = Id;
		name = Name;
		rect = Rect;
		atlas = Atlas;
	}

	public void Draw(Vector2 Position, Color color)
	{
		atlas.Draw(id, Position, color);
	}

	public void Draw(Vector2 Position, Color color, SpriteEffects effect, float zorder)
	{
		atlas.Draw(id, Position, effect, zorder, color);
	}

	public void Draw(Vector2 Position, Color color, SpriteEffects effect, float zorder, float rotation, Vector2 origin)
	{
		atlas.Draw(id, Position, effect, zorder, color, rotation, origin);
	}

	public void Draw(Vector2 Position, Color color, SpriteEffects effect, float zorder, float rotation, float scale)
	{
		atlas.Draw(id, Position, effect, zorder, color, rotation, scale);
	}
}
