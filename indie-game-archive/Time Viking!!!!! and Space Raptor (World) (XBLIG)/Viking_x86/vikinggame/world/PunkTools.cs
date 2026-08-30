using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Viking_x86.director;

namespace Viking_x86.vikinggame.world;

public class PunkTools
{
	public void Draw()
	{
		SpriteTools.sprite.Draw(Game1.nullTex, new Rectangle(0, 0, (int)VScroll.screenSize.X, (int)VScroll.screenSize.Y), Color.Black);
		Rectangle value = new Rectangle
		{
			X = TimeMgr.CurTMgr().beat % 3 * 224,
			Y = TimeMgr.CurTMgr().beat % 12 / 3 * 160,
			Width = 224,
			Height = 160
		};
		for (float num = (float)TimeMgr.CurTMgr().pulse * 2f; num > 1f; num--)
		{
		}
		SpriteTools.sprite.Draw(Game1.vgame.punkTex[(!((float)TimeMgr.CurTMgr().pulse > 0.1f)) ? 1u : 0u], VScroll.screenSize / 2f, value, new Color(0.65f, 0.65f, 0.65f, 1f), VScroll.angle, new Vector2(112f, 80f), (4f + (float)TimeMgr.CurTMgr().pulse * 0.35f) * 1.5f, SpriteEffects.None, 1f);
	}
}
