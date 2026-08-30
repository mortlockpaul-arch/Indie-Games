using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Viking_x86.director;

namespace Viking_x86.vikinggame.world;

public class OrbitTools
{
	private Vector2[] star;

	private Vector3[] cloud;

	public OrbitTools()
	{
		star = new Vector2[128];
		for (int i = 0; i < star.Length; i++)
		{
			ref Vector2 reference = ref star[i];
			reference = Rand.GetRandomVec2(0f, VScroll.screenSize.X, 0f, VScroll.screenSize.Y);
		}
		cloud = new Vector3[32];
		for (int j = 0; j < cloud.Length; j++)
		{
			ref Vector3 reference2 = ref cloud[j];
			reference2 = Rand.GetRandomVec3(-400f, 400f, -400f, 100f, 0.25f, 1f);
		}
	}

	public void Draw()
	{
		float num = 1f;
		if (TimeMgr.CurTMgr().trackLeft < 43.0)
		{
			num = (float)TimeMgr.CurTMgr().trackLeft / 43f;
		}
		SpriteTools.sprite.Draw(Game1.vgame.grayTex, VScroll.screenSize / 2f, new Rectangle(0, 0, 480, 480), new Color(0.1f * num, 0.1f * num, 0.1f * num, 1f), VScroll.angle, new Vector2(240f, 240f), 3f, SpriteEffects.None, 1f);
		for (int i = 0; i < star.Length; i++)
		{
			SpriteTools.sprite.Draw(Game1.vgame.spritesTex, star[i], new Rectangle(128, 64, 128, 128), new Color(1f, 1f, 1f, num), 0f, new Vector2(64f, 64f), (float)(i % 5) * 0.01f + 0.05f, SpriteEffects.None, 1f);
		}
		SpriteTools.sprite.Draw(Game1.vgame.atmosTex, VScroll.GetScreenLoc(Game1.vgame.world.risingTrackBase, 0.2f), new Rectangle(0, 0, 480, 480), new Color(1f, 1f, 1f, num), VScroll.angle, new Vector2(240f, 240f), new Vector2(3f, 3f), SpriteEffects.None, 1f);
		for (int j = 0; j < cloud.Length; j++)
		{
			SpriteTools.sprite.Draw(Game1.vgame.spritesTex, VScroll.GetScreenLoc(new Vector2(cloud[j].X + Game1.vgame.world.risingTrackBase.X, cloud[j].Y + Game1.vgame.world.risingTrackBase.Y), cloud[j].Z), new Rectangle(128, 64, 128, 128), new Color(1f, 1f, 1f, 0.2f * num), VScroll.angle, new Vector2(64f, 64f), new Vector2(3f, 0.5f) * VScroll.zoom, SpriteEffects.None, 1f);
		}
		if (TimeMgr.VikingTMgr().trackTime < 1.0)
		{
			SpriteTools.sprite.Draw(Game1.nullTex, new Rectangle(0, 0, (int)VScroll.screenSize.X, (int)VScroll.screenSize.Y), new Rectangle(0, 0, 1, 1), new Color(1f, 1f, 1f, 1f - (float)TimeMgr.VikingTMgr().trackTime));
		}
	}
}
