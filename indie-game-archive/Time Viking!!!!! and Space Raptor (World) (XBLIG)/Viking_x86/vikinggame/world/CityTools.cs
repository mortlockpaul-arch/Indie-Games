using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Viking_x86.director;

namespace Viking_x86.vikinggame.world;

public class CityTools
{
	public void DrawBack()
	{
		Vector2 vector = Game1.vgame.world.GetBase();
		vector.Y += 700f;
		float num = 800f;
		WorldTools worldTools = Game1.vgame.world.worldTools;
		Vector3 vector2 = new Vector3(vector.X, vector.Y, 0f);
		SpriteTools.sprite.Draw(Game1.vgame.cityTex, VScroll.GetScreenLoc(vector, 0.25f), new Rectangle(0, 0, 512, 810), Color.Black, VScroll.angle, new Vector2(256f, 810f), VScroll.zoom * 1.2f * 1.5f, SpriteEffects.FlipHorizontally, 1f);
		worldTools.DrawLight(new Color(1f, 1f, 1f, 0.27f), vector2 + new Vector3(100f, 0f, 0.4f), -1.47f, 1.3f);
		worldTools.DrawLight(new Color(1f, 1f, 1f, 0.29f), vector2 + new Vector3(-100f, 0f, 0.45f), -1.6700001f, 1.3f);
		SpriteTools.sprite.Draw(Game1.vgame.cityTex, VScroll.GetScreenLoc(vector, 0.3f), new Rectangle(0, 0, 512, 810), Color.White, VScroll.angle, new Vector2(256f, 810f), VScroll.zoom, SpriteEffects.None, 1f);
		SpriteTools.sprite.Draw(Game1.vgame.cityTex, VScroll.GetScreenLoc(vector + new Vector2(0f - num, 0f), 0.35f), new Rectangle(512, 0, 192, 640), Color.White, VScroll.angle, new Vector2(96f, 640f), VScroll.zoom, SpriteEffects.None, 1f);
		SpriteTools.sprite.Draw(Game1.vgame.cityTex, VScroll.GetScreenLoc(vector + new Vector2(num, 0f), 0.35f), new Rectangle(704, 0, 192, 640), Color.White, VScroll.angle, new Vector2(96f, 640f), VScroll.zoom, SpriteEffects.None, 1f);
		SpriteTools.sprite.Draw(Game1.vgame.cityTex, VScroll.GetScreenLoc(vector, 0.35f), new Rectangle(992, 0, 32, 768), Color.White, VScroll.angle, new Vector2(16f, 750f), VScroll.zoom, SpriteEffects.None, 1f);
		SpriteTools.sprite.Draw(Game1.vgame.cityTex, VScroll.GetScreenLoc(vector + new Vector2(-550f, 0f), 0.4f), new Rectangle(992, 0, 32, 768), Color.White, VScroll.angle, new Vector2(16f, 750f), VScroll.zoom * 1.09f, SpriteEffects.None, 1f);
		SpriteTools.sprite.Draw(Game1.vgame.cityTex, VScroll.GetScreenLoc(vector + new Vector2(550f, 0f), 0.45f), new Rectangle(992, 0, 32, 768), Color.White, VScroll.angle, new Vector2(16f, 750f), VScroll.zoom * 1.11f, SpriteEffects.None, 1f);
		SpriteTools.End();
		SpriteTools.BeginAdditive();
		for (int i = 0; i < 10; i++)
		{
			worldTools.DrawBlip(1f, 0.2f, 0.2f, vector2 + new Vector3(0f, (float)i * -232f, 0.35f));
		}
		for (int j = 0; j < 10; j++)
		{
			worldTools.DrawBlip(1f, 0.2f, 0.2f, vector2 + new Vector3(-550f, (float)j * -220f, 0.4f));
		}
		for (int k = 0; k < 10; k++)
		{
			worldTools.DrawBlip(1f, 0.2f, 0.2f, vector2 + new Vector3(550f, (float)k * -200f, 0.45f));
		}
		worldTools.DrawLight(new Color(1f, 1f, 1f, 0.5f), vector2 + new Vector3(-600f, 0f, 0.4f), -1.1700001f, 1f);
		worldTools.DrawLight(new Color(1f, 1f, 1f, 0.5f), vector2 + new Vector3(600f, 0f, 0.45f), -1.97f, 1f);
		worldTools.DrawLight(new Color(1f, 1f, 1f, 0.5f), vector2 + new Vector3(-700f, 0f, 0.5f), -1.47f, 1f);
		worldTools.DrawLight(new Color(1f, 1f, 1f, 0.5f), vector2 + new Vector3(700f, 0f, 0.55f), -1.6700001f, 1f);
		if (TimeMgr.CurTMgr().trackLeft < 2.0)
		{
			SpriteTools.sprite.Draw(Game1.nullTex, new Rectangle(0, 0, (int)VScroll.screenSize.X, (int)VScroll.screenSize.Y), new Color(1f, 1f, 1f, (float)(1.0 - TimeMgr.CurTMgr().trackLeft / 2.0)));
		}
		SpriteTools.End();
		SpriteTools.BeginAlpha();
	}

	public void DrawFore()
	{
		Vector2 vector = Game1.vgame.world.GetBase();
		Vector3 vector2 = new Vector3(vector.X, vector.Y + 100f, 0f);
		WorldTools worldTools = Game1.vgame.world.worldTools;
		worldTools.DrawLight(new Color(1f, 1f, 1f, 0.15f), vector2 + new Vector3(-300f, 11f, 1.25f), -1.47f, 1f);
		worldTools.DrawLight(new Color(1f, 1f, 1f, 0.15f), vector2 + new Vector3(300f, 11f, 1.25f), -1.6700001f, 1f);
		SpriteTools.sprite.Draw(Game1.nullTex, VScroll.GetScreenLoc(new Vector2(VScroll.scroll.X, vector.Y + 10f), 1f), new Rectangle(0, 0, 1, 1), new Color(0f, 0f, 0f, 1f), VScroll.angle, new Vector2(0.5f, 0f), new Vector2(1400f, 500f), SpriteEffects.None, 1f);
		SpriteTools.sprite.Draw(Game1.vgame.cityTex, VScroll.GetScreenLoc(new Vector2(vector.X, vector.Y), 1f), new Rectangle(0, 992, 1024, 32), Color.White, VScroll.angle, new Vector2(512f, 12f), new Vector2(2f, 1f), SpriteEffects.None, 1f);
	}
}
