using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TechArts
{
	internal class Particle
	{
		public static Texture2D pt;

		internal float life;

		internal float dlife;

		internal float scl;

		internal Vector2 pos;

		internal Vector2 dpos;

		internal Vector2 org;

		internal Particle(Vector2 p)
		{
			pos = p;
			dpos = new Vector2((float)GameEngine.core.rnd.Next(2400) / 100f - 12f, (float)GameEngine.core.rnd.Next(2400) / 100f - 12f);
			dpos *= 3f;
			life = 1f;
			dlife = 0.01f + (float)GameEngine.core.rnd.Next(100) / 500f;
			org = new Vector2(0f, 0f);
			scl = 0.5f;
		}
	}
}
