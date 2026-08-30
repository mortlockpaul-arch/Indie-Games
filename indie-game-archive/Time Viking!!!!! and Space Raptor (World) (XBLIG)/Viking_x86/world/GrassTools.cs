using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Viking_x86.world;

internal class GrassTools
{
	public Vector3[] cloud;

	public GrassTools()
	{
		cloud = new Vector3[32];
		for (int i = 0; i < cloud.Length; i++)
		{
			ref Vector3 reference = ref cloud[i];
			reference = Rand.GetRandomVec3(-1000f, 500f, 400f, 700f, 0.25f, 1f);
		}
	}

	public void DrawBack(float alpha)
	{
		float y = 800f;
		for (int i = 0; i < 2; i++)
		{
			SpriteTools.sprite.Draw(Game1.vgame.grassTex, VScroll.GetScreenLoc(new Vector2((float)i * 800f, y), 0.6f), new Rectangle(0, 72, 1024, 150), new Color(0.5f * alpha, 0.5f * alpha, 0.5f * alpha, 1f), VScroll.angle, new Vector2(512f, 100f), 1.2f * VScroll.zoom * 0.714f, SpriteEffects.FlipHorizontally, 1f);
		}
		for (int j = 0; j < 2; j++)
		{
			SpriteTools.sprite.Draw(Game1.vgame.grassTex, VScroll.GetScreenLoc(new Vector2((float)j * 800f, y), 0.8f), new Rectangle(0, 72, 1024, 150), new Color(alpha, alpha, alpha, 1f), VScroll.angle, new Vector2(512f, 100f), 1.2f * VScroll.zoom * 0.714f, SpriteEffects.None, 1f);
		}
		for (int k = 0; k < cloud.Length; k++)
		{
			SpriteTools.sprite.Draw(Game1.vgame.spritesTex, VScroll.GetScreenLoc(new Vector2(cloud[k].X + Game1.vgame.world.risingTrackBase.X, cloud[k].Y + Game1.vgame.world.risingTrackBase.Y), cloud[k].Z), new Rectangle(128, 64, 128, 128), new Color(alpha, alpha, alpha, 0.2f), VScroll.angle, new Vector2(64f, 64f), new Vector2(3f, 0.5f) * VScroll.zoom, SpriteEffects.None, 1f);
		}
	}

	internal void DrawGrass(float alpha)
	{
		float y = 800f;
		SpriteTools.sprite.Draw(Game1.nullTex, VScroll.GetScreenLoc(new Vector2(VScroll.scroll.X, y), 1f), new Rectangle(0, 0, 1, 1), new Color(0f, 0f, 0f, 1f), VScroll.angle, new Vector2(0.5f, 0f), new Vector2(1400f, 500f), SpriteEffects.None, 1f);
		for (int i = 0; i < 2; i++)
		{
			SpriteTools.sprite.Draw(Game1.vgame.grassTex, VScroll.GetScreenLoc(new Vector2((float)i * 850f, y), 1f), new Rectangle(0, 0, 1024, 64), new Color(alpha, alpha, alpha, 1f), VScroll.angle, new Vector2(512f, 40f), 1.2f * VScroll.zoom * 0.714f, SpriteEffects.None, 1f);
		}
		DrawTree(new Vector2(-30f, y), 1f, 1.3f, 0f, alpha);
		DrawTree(new Vector2(-100f, y), 1f, 1.8f, -0.1f, alpha);
		DrawTree(new Vector2(Game1.vgame.world.towerX / 2f, y), 1f, 1.8f, 0f, alpha);
		DrawTree(new Vector2(Game1.vgame.world.towerX, y), 1f, 1.2f, -0.16f, alpha);
		DrawTree(new Vector2(Game1.vgame.world.towerX + 320f, y), 1f, 1.4f, 0.16f, alpha);
	}

	private void DrawTree(Vector2 loc, float scale, float depth, float angle, float alpha)
	{
		SpriteTools.sprite.Draw(Game1.vgame.grassTex, VScroll.GetScreenLoc(loc, depth), new Rectangle(0, 256, 448, 448), new Color(alpha, alpha, alpha, 1f), angle + VScroll.angle, new Vector2(224f, 448f), scale * VScroll.zoom * depth, SpriteEffects.None, 1f);
	}
}
