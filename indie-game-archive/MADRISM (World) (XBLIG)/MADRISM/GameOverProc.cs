using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using TechArts;

namespace MADRISM
{
	internal class GameOverProc : TaskObj
	{
		private Texture2D logo;

		private float alpha;

		private bool InGame()
		{
			return !GlobalState.inAttract;
		}

		public GameOverProc(Texture2D tex)
		{
			logo = tex;
			alpha = 0f;
			MediaPlayer.Stop();
		}

		public override IEnumerator<int> Update()
		{
			for (int i = 0; i < 4; i++)
			{
				yield return 0;
			}
			MediaPlayer.IsRepeating = false;
			MediaPlayer.Play(PlayState.core.bgm_gameover);
			if (InGame())
			{
				manager.Entry(new GameOverProc2());
				for (int j = 0; j < 180; j++)
				{
					yield return 0;
				}
				while (!GameEngine.core.IsPressed_A_Ctr())
				{
					yield return 0;
				}
				while (GameEngine.core.IsPressed_A_Ctr())
				{
					yield return 0;
				}
				GameEngine.core.fader.WithBGM = true;
				while (GameEngine.core.fader.Brightness < 1f)
				{
					GameEngine.core.fader.Brightness += 1f / 60f;
					yield return 0;
				}
				GlobalState.inDestroy = true;
				manager.Remove(this);
			}
			else
			{
				while (GlobalState.inState)
				{
					yield return 0;
				}
				manager.Remove(this);
			}
		}

		public override void PostUpdate()
		{
			alpha += (InGame() ? 0.005f : 0.01f);
			if (alpha > 1f)
			{
				alpha = 1f;
			}
		}

		public override void Draw()
		{
			Vector2 pos = new Vector2(640f, 360f);
			GameEngine.core.DrawSprite(logo, pos, new Color(1f, 1f, 1f, alpha), 0f, 1f, 1f);
		}
	}
}
