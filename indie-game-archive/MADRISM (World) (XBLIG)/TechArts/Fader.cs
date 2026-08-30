using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace TechArts
{
	public class Fader
	{
		private Texture2D mask;

		private Rectangle rect;

		private float alpha;

		private float mastervol;

		private bool bWithBGM;

		public bool WithBGM
		{
			get
			{
				return bWithBGM;
			}
			set
			{
				bWithBGM = value;
				volUpdate();
			}
		}

		public float Brightness
		{
			get
			{
				return alpha;
			}
			set
			{
				alpha = value;
				if (alpha > 1f)
				{
					alpha = 1f;
				}
				if (alpha < 0f)
				{
					alpha = 0f;
				}
				volUpdate();
			}
		}

		private void volUpdate()
		{
			float num = mastervol;
			if (bWithBGM)
			{
				num *= 1f - Brightness;
			}
			MediaPlayer.Volume = num;
		}

		public Fader(Texture2D masktex)
		{
			mastervol = MediaPlayer.Volume;
			bWithBGM = false;
			mask = masktex;
			rect = new Rectangle(0, 0, 1280, 720);
			alpha = 0.5f;
		}

		public void Draw()
		{
			if (alpha > 0f)
			{
				GameEngine.core.spriteBatch.Draw(mask, rect, new Color(1f, 1f, 1f, alpha));
			}
		}
	}
}
