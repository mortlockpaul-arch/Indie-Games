using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TechArts;

namespace MADRISM
{
	internal class ScoreDispFont : TaskObj
	{
		private Texture2D font;

		private float alpha;

		private float scale;

		private float accG;

		private float angle;

		private float dx;

		private float dang;

		private float sscl;

		private Vector2 pos;

		internal ScoreDispFont(Texture2D t, Vector2 p, float mx, float s)
		{
			font = t;
			scale = 1f;
			pos = p;
			alpha = 1f;
			angle = 0f;
			accG = -24f - ((float)GameEngine.core.rnd.NextDouble() - 0.5f) * 8f;
			dx = mx;
			dang = ((float)GameEngine.core.rnd.NextDouble() - 0.5f) * ((float)Math.PI / 180f);
			sscl = s;
		}

		public override IEnumerator<int> Update()
		{
			for (int i = 0; i < 120; i++)
			{
				yield return 0;
			}
			manager.Remove(this);
		}

		public override void PostUpdate()
		{
			pos.X += dx;
			pos.Y += accG;
			accG += 1.3f;
			scale += 0.075f;
			alpha -= 1f / 120f;
			if (alpha < 0f)
			{
				alpha = 0f;
			}
			angle += dang;
		}

		public override void Draw()
		{
			GameEngine.core.DrawSprite(font, pos, new Color(1f, 1f, 1f, alpha), angle, scale * sscl, 1f);
		}
	}
}
