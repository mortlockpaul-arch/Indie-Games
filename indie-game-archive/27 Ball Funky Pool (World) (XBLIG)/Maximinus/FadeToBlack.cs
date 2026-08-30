using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public class FadeToBlack
{
	private static readonly Color color = Color.Black;

	public static void FadeRectangle(Drawing2D draw2D, Vector2 pos, float ratio, float depth)
	{
		draw2D.SpriteBatch.Draw(draw2D.BlankTex, new Rectangle(0, 0, (int)MathHelper.Lerp(0f, pos.X, ratio), draw2D.ScreenSizePoint.Y), null, color, 0f, Vector2.Zero, SpriteEffects.None, depth);
		draw2D.SpriteBatch.Draw(draw2D.BlankTex, new Rectangle((int)MathHelper.Lerp(draw2D.ScreenSizePoint.X, pos.X, ratio), 0, draw2D.ScreenSizePoint.X, draw2D.ScreenSizePoint.Y), null, color, 0f, Vector2.Zero, SpriteEffects.None, depth);
		draw2D.SpriteBatch.Draw(draw2D.BlankTex, new Rectangle(0, 0, draw2D.ScreenSizePoint.X, (int)MathHelper.Lerp(0f, pos.Y, ratio)), null, color, 0f, Vector2.Zero, SpriteEffects.None, depth);
		draw2D.SpriteBatch.Draw(draw2D.BlankTex, new Rectangle(0, (int)MathHelper.Lerp(draw2D.ScreenSizePoint.Y, pos.Y, ratio), draw2D.ScreenSizePoint.X, draw2D.ScreenSizePoint.Y), null, color, 0f, Vector2.Zero, SpriteEffects.None, depth);
	}
}
