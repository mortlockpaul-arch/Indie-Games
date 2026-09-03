using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class StretchTex
{
	private Texture2D tex;

	private int x1;

	private int x2;

	private int y1;

	private int y2;

	public void Initialize(int _x1, int _x2, int _y1, int _y2, string _texName)
	{
		x1 = _x1;
		x2 = _x2;
		y1 = _y1;
		y2 = _y2;
		tex = BaseGame.Get().content.Load<Texture2D>(_texName);
	}

	public void Draw(Vector2 start, Vector2 pos1, Vector2 pos2, Vector2 end, Color tint)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Draw(start, pos1, pos2, end, tint, 1f);
	}

	public void Draw(Vector2 start, Vector2 pos1, Vector2 pos2, Vector2 end, Color tint, float layer)
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		int num = (int)start.X;
		int num2 = (int)(start.X + pos1.X);
		int num3 = (int)(start.X + pos2.X);
		int num4 = (int)end.X;
		int num5 = (int)start.Y;
		int num6 = (int)(start.Y + pos1.Y);
		int num7 = (int)(start.Y + pos2.Y);
		int num8 = (int)end.Y;
		BaseGame.Get().spriteBatch.Draw(tex, new Rectangle(num, num5, num2 - num, num6 - num5), (Rectangle?)new Rectangle(0, 0, x1, y1), tint, 0f, Vector2.Zero, (SpriteEffects)0, layer);
		BaseGame.Get().spriteBatch.Draw(tex, new Rectangle(num2, num5, num3 - num2, num6 - num5), (Rectangle?)new Rectangle(x1, 0, x2 - x1, y1), tint, 0f, Vector2.Zero, (SpriteEffects)0, layer);
		BaseGame.Get().spriteBatch.Draw(tex, new Rectangle(num3, num5, num4 - num3, num6 - num5), (Rectangle?)new Rectangle(x2, 0, tex.Width - x2, y1), tint, 0f, Vector2.Zero, (SpriteEffects)0, layer);
		BaseGame.Get().spriteBatch.Draw(tex, new Rectangle(num, num6, num2 - num, num7 - num6), (Rectangle?)new Rectangle(0, y1, x1, y2 - y1), tint, 0f, Vector2.Zero, (SpriteEffects)0, layer);
		BaseGame.Get().spriteBatch.Draw(tex, new Rectangle(num2, num6, num3 - num2, num7 - num6), (Rectangle?)new Rectangle(x1, y1, x2 - x1, y2 - y1), tint, 0f, Vector2.Zero, (SpriteEffects)0, layer);
		BaseGame.Get().spriteBatch.Draw(tex, new Rectangle(num3, num6, num4 - num3, num7 - num6), (Rectangle?)new Rectangle(x2, y1, tex.Width - x2, y2 - y1), tint, 0f, Vector2.Zero, (SpriteEffects)0, layer);
		BaseGame.Get().spriteBatch.Draw(tex, new Rectangle(num, num7, num2 - num, num8 - num7), (Rectangle?)new Rectangle(0, y2, x1, tex.Height - y2), tint, 0f, Vector2.Zero, (SpriteEffects)0, layer);
		BaseGame.Get().spriteBatch.Draw(tex, new Rectangle(num2, num7, num3 - num2, num8 - num7), (Rectangle?)new Rectangle(x1, y2, x2 - x1, tex.Height - y2), tint, 0f, Vector2.Zero, (SpriteEffects)0, layer);
		BaseGame.Get().spriteBatch.Draw(tex, new Rectangle(num3, num7, num4 - num3, num8 - num7), (Rectangle?)new Rectangle(x2, y2, tex.Width - x2, tex.Height - y2), tint, 0f, Vector2.Zero, (SpriteEffects)0, layer);
	}

	public void Draw(Vector2 start, Vector2 end, Color tint)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Draw(start, end, tint, 1f);
	}

	public void Draw(Vector2 start, Vector2 end, Color tint, float layer)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		Draw(start, new Vector2((float)x1, (float)y1), new Vector2(end.X - (float)(tex.Width - x2) - start.X, end.Y - (float)(tex.Height - y2) - start.Y), end, tint, layer);
	}
}
