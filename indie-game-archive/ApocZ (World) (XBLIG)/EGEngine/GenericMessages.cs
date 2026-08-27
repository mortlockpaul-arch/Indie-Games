using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class GenericMessages
{
	private static string message;

	private static float messageLife;

	private static float textScale;

	private static Vector2 posOffset = Vector2.Zero;

	private static Vector2 msgPos = Vector2.Zero;

	private static Vector2 tmpTextPos = Vector2.Zero;

	private static Color tmpColor = Color.Black;

	private static Color tmpShadow = Color.Black;

	public static bool IsActive()
	{
		return messageLife > 0f;
	}

	public static void Clear()
	{
		messageLife = 0f;
	}

	public static void Add(string msg, int t)
	{
		message = msg;
		messageLife = t;
		textScale = 1.25f;
		posOffset.X = Menu.defaultFont.MeasureString(message).X * 0.5f * (0f - textScale);
		posOffset.Y = 24f;
	}

	public static void Update()
	{
		messageLife -= 0.03f;
	}

	public static void DrawPost(int qIndex, PlayerBase playerRef)
	{
		Viewport viewport = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport;
		if (messageLife > 0f)
		{
			float num = messageLife;
			num = ((num < 0f) ? 0f : num);
			num = ((num > 1f) ? 1f : num);
			byte b = (tmpShadow.B = 0);
			byte r = (tmpShadow.G = b);
			tmpShadow.R = r;
			tmpShadow.A = (byte)(num * 255f);
			byte b4 = (tmpColor.B = (byte)(num * 211f));
			byte r2 = (tmpColor.G = b4);
			tmpColor.R = r2;
			tmpColor.A = (byte)(num * 255f);
			tmpTextPos.X = viewport.TitleSafeArea.Center.X;
			tmpTextPos.Y = viewport.TitleSafeArea.Center.Y;
			tmpTextPos += posOffset;
			Menu.spriteBatch.Begin();
			Menu.spriteBatch.DrawString(Menu.defaultFont, message, tmpTextPos, tmpShadow, 0f, new Vector2(-2f, -2f), textScale, SpriteEffects.None, 0);
			Menu.spriteBatch.DrawString(Menu.defaultFont, message, tmpTextPos, tmpColor, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0);
			Menu.spriteBatch.End();
		}
	}
}
