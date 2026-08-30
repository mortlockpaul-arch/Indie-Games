using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using TechArts;

namespace MADRISM
{
	internal class TitleState : TaskObj
	{
		private Texture2D logo;

		private Texture2D copyright;

		private Texture2D pusha;

		private Vector2 logopos;

		private Vector2 copyrightpos;

		private Vector2 pushapos;

		private Texture2D madori00;

		private Texture2D madori01;

		private SoundEffect startsound;

		private Song bgm_op;

		private Color col;

		private bool bFadeout;

		private bool bOK;

		public TitleState()
		{
			logo = GameEngine.core.Content.Load<Texture2D>("Sprite/Title/Logo");
			logopos = new Vector2(398f, 154f);
			copyright = GameEngine.core.Content.Load<Texture2D>("Sprite/Title/Copyright");
			copyrightpos = new Vector2(446f, 606f);
			pusha = GameEngine.core.Content.Load<Texture2D>("Sprite/Title/PushAButton");
			pushapos = new Vector2(576f, 390f);
			madori00 = GameEngine.core.Content.Load<Texture2D>("Sprite/Title/madori00");
			madori01 = GameEngine.core.Content.Load<Texture2D>("Sprite/Title/madori01");
			bgm_op = GameEngine.core.Content.Load<Song>("Sound/op");
			startsound = GameEngine.core.Content.Load<SoundEffect>("SE/ok");
			bOK = false;
		}

		public override IEnumerator<int> Update()
		{
			col = new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
			GameEngine.core.fader.Brightness = 1f;
			bFadeout = false;
			GlobalState.toAttract = true;
			GameEngine.core.particles.Reset();
			for (int i = 0; i < 30; i++)
			{
				yield return 0;
			}
			MediaPlayer.IsRepeating = false;
			MediaPlayer.Play(bgm_op);
			GameEngine.core.fader.WithBGM = false;
			for (int j = 0; j < 60; j++)
			{
				GameEngine.core.fader.Brightness -= 1f / 15f;
				yield return 0;
			}
			GameEngine.core.fader.Brightness = 0f;
			for (int k = 0; k < 60; k++)
			{
				yield return 0;
			}
			for (int l = 0; l < 127; l++)
			{
				if (col.A < 254)
				{
					col.A += 2;
				}
				yield return 0;
			}
			for (int m = 0; m < 1800; m++)
			{
				yield return 0;
			}
			if (!bFadeout)
			{
				bFadeout = true;
				GlobalState.toAttract = true;
			}
			while (true)
			{
				yield return 0;
			}
		}

		public override void PostUpdate()
		{
			if (!GameEngine.core.IsPressed_A_Ctr())
			{
				bOK = true;
			}
			if (bFadeout)
			{
				GameEngine.core.fader.WithBGM = true;
				GameEngine.core.fader.Brightness += 1f / 60f;
				if (GameEngine.core.fader.Brightness >= 1f)
				{
					GlobalState.inState = false;
					manager.Remove(this);
					return;
				}
			}
			else if (bOK && GameEngine.core.IsPressed_A_Ctr())
			{
				startsound.Play();
				GlobalState.toAttract = false;
				bFadeout = true;
			}
			if (GameEngine.core.vcount % 60 == 0)
			{
				if (GameEngine.core.rnd.Next(100) < 50)
				{
					manager.Entry(new TitleMadori(madori00));
				}
				else
				{
					manager.Entry(new TitleMadori(madori01));
				}
			}
		}

		public override void Draw()
		{
			if (GameEngine.core.vcount % 60 < 30)
			{
				GameEngine.core.spriteBatch.Draw(pusha, pushapos, Color.White);
			}
			GameEngine.core.spriteBatch.Draw(logo, logopos, col);
			GameEngine.core.spriteBatch.Draw(copyright, copyrightpos, Color.White);
		}
	}
}
