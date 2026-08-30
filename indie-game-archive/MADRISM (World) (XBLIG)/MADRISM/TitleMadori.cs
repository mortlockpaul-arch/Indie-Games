using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TechArts;

namespace MADRISM
{
	internal class TitleMadori : TaskObj
	{
		private Texture2D img;

		private Vector2 pos;

		private Vector2 dpos;

		private Color col;

		private float ang;

		private float dang;

		public TitleMadori(Texture2D t)
		{
			Random rnd = GameEngine.core.rnd;
			img = t;
			pos = new Vector2(rnd.Next(1024), rnd.Next(640));
			dpos = new Vector2((float)rnd.Next(128) / 80f + 0.2f, (float)rnd.Next(128) / 80f + 0.2f);
			if (rnd.Next(100) < 50)
			{
				dpos.X *= -1f;
			}
			if (rnd.Next(100) < 50)
			{
				dpos.Y *= -1f;
			}
			col = new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
			ang = rnd.Next(180) - 90;
			dang = (float)(rnd.Next(200) - 100) / 100f * 0.25f;
		}

		public override IEnumerator<int> Update()
		{
			for (int i = 0; i < 64; i++)
			{
				col.A++;
				yield return 0;
			}
			for (int j = 0; j < 300; j++)
			{
				yield return 0;
			}
			for (int k = 0; k < 64; k++)
			{
				col.A--;
				yield return 0;
			}
		}

		public override void PostUpdate()
		{
			pos += dpos;
			ang += dang;
			if (!GlobalState.inState)
			{
				manager.Remove(this);
			}
		}

		private float radian(float n)
		{
			return n * 3.141596f / 180f;
		}

		public override void Draw()
		{
			if (GlobalState.inState)
			{
				GameEngine.core.spriteBatch.Draw(img, pos, null, col, radian(ang), new Vector2(img.Width / 2, img.Height / 2), 1f, SpriteEffects.None, 1f);
			}
		}
	}
}
