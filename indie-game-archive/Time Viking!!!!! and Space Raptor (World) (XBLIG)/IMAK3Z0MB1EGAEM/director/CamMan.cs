using IMAK3Z0MB1EGAEM.character;
using IMAK3Z0MB1EGAEM.map;
using Microsoft.Xna.Framework;

namespace IMAK3Z0MB1EGAEM.director;

public class CamMan
{
	public static Vector2 endlessTransStart;

	public static Vector2 endlessTransEnd;

	public static float endlessTransFrame;

	public static void Update()
	{
		Vector2 vector = default(Vector2);
		Vector2 vector2 = default(Vector2);
		bool flag = true;
		float num = 200f;
		bool flag2 = GameState.state == GameState.State.EndlessZombiesPlaying;
		if (flag2 && endlessTransFrame > 0f)
		{
			endlessTransFrame -= FMan.frameTime * 0.45f;
		}
		Hero[] hero = CharMan.hero;
		foreach (Hero hero2 in hero)
		{
			if (hero2.exists)
			{
				if (flag)
				{
					vector = (vector2 = hero2.loc);
					flag = false;
				}
				if (hero2.loc.X < vector.X)
				{
					vector.X = hero2.loc.X;
				}
				if (hero2.loc.Y < vector.Y)
				{
					vector.Y = hero2.loc.Y;
				}
				if (hero2.loc.X > vector2.X)
				{
					vector2.X = hero2.loc.X;
				}
				if (hero2.loc.Y > vector2.Y)
				{
					vector2.Y = hero2.loc.Y;
				}
			}
		}
		vector -= new Vector2(num, num);
		vector2 += new Vector2(num, num);
		Vector2 vector3 = (vector + vector2) / 2f;
		if (flag2)
		{
			ZombieGame.UpdateEndlessScroll(vector3);
		}
		ScrollMan.scroll += (vector3 - ScrollMan.scroll) * FMan.frameTime;
		float num2 = 1f;
		num2 = 0.8f;
		Vector2 vector4 = vector2 - vector;
		Vector2 vector5 = new Vector2(ScrollMan.screenSize.X / vector4.X, ScrollMan.screenSize.Y / vector4.Y);
		if (vector5.X < 1f || vector5.Y < 1f)
		{
			num2 = ((!(vector5.X < vector5.Y)) ? vector5.Y : vector5.X);
		}
		ScrollMan.zoom += (num2 - ScrollMan.zoom) * FMan.frameTime * 3f;
		if (ScrollMan.zoom < 1f / MapMan.mapScale)
		{
			ScrollMan.zoom = 1f / MapMan.mapScale;
		}
		Vector2 loc = default(Vector2);
		Vector2 mapSize = MapMan.mapSize;
		if (!flag2)
		{
			vector = ScrollMan.GetScreenLoc(loc, 1f);
			vector2 = ScrollMan.GetScreenLoc(mapSize, 1f);
			if (vector.X > 0f)
			{
				ScrollMan.scroll.X += vector.X / ScrollMan.zoom;
			}
			if (vector.Y > 0f)
			{
				ScrollMan.scroll.Y += vector.Y / ScrollMan.zoom;
			}
			if (vector2.X < ScrollMan.screenSize.X)
			{
				ScrollMan.scroll.X -= (ScrollMan.screenSize.X - vector2.X) / ScrollMan.zoom;
			}
			if (vector2.Y < ScrollMan.screenSize.Y)
			{
				ScrollMan.scroll.Y -= (ScrollMan.screenSize.Y - vector2.Y) / ScrollMan.zoom;
			}
		}
	}
}
