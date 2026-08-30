using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public class Debug2D
{
	public class DebugRectangle
	{
		public static void Draw(Rectangle rec, Color color)
		{
			Point[] array = new Point[4];
			Utils.Rectangle_GetCorners(rec, array);
			MaximinusGame.Draw2D.SpriteBatch.Draw(MaximinusGame.Draw2D.BlankTex, new Rectangle(array[0].X, array[0].Y, rec.Width, 1), null, color, 0f, Vector2.Zero, SpriteEffects.None, 0f);
			MaximinusGame.Draw2D.SpriteBatch.Draw(MaximinusGame.Draw2D.BlankTex, new Rectangle(array[2].X, array[2].Y, rec.Width, 1), null, color, 0f, Vector2.Zero, SpriteEffects.None, 0f);
			MaximinusGame.Draw2D.SpriteBatch.Draw(MaximinusGame.Draw2D.BlankTex, new Rectangle(array[0].X, array[0].Y, 1, rec.Height), null, color, 0f, Vector2.Zero, SpriteEffects.None, 0f);
			MaximinusGame.Draw2D.SpriteBatch.Draw(MaximinusGame.Draw2D.BlankTex, new Rectangle(array[1].X, array[1].Y, 1, rec.Height + 1), null, color, 0f, Vector2.Zero, SpriteEffects.None, 0f);
		}
	}
}
