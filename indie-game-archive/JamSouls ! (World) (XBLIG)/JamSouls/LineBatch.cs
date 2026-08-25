using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JamSouls;

public static class LineBatch
{
	private static Texture2D _empty_texture;

	private static bool _set_data = false;

	public static void Init(GraphicsDevice device)
	{
		_empty_texture = new Texture2D(device, 1, 1, mipMap: false, SurfaceFormat.Color);
	}

	public static void DrawLine(SpriteBatch batch, Color color, Vector2 point1, Vector2 point2)
	{
		DrawLine(batch, color, point1, point2, 1f);
	}

	public static void DrawLine(SpriteBatch batch, Color color, Vector2 point1, Vector2 point2, float Layer)
	{
		if (!_set_data)
		{
			_empty_texture.SetData(new Color[1] { Color.White });
			_set_data = true;
		}
		float rotation = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
		float x = (point2 - point1).Length();
		batch.Draw(_empty_texture, point1, null, color, rotation, Vector2.Zero, new Vector2(x, 1f), SpriteEffects.None, Layer);
	}
}
