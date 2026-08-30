using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TechArts;

namespace MADRISM
{
	internal class GameOverLogoProc : TaskObj
	{
		private Texture2D logo;

		private Texture2D text;

		private float alpha;

		public GameOverLogoProc(Texture2D tex, Texture2D tex2)
		{
			logo = tex;
			text = tex2;
			alpha = 0f;
		}

		public override IEnumerator<int> Update()
		{
			while (true)
			{
				yield return 0;
			}
		}

		public override void PostUpdate()
		{
			if (!GlobalState.inState)
			{
				manager.Remove(this);
			}
		}

		public override void Draw2()
		{
			Vector2 pos = new Vector2(640f, 360f);
			GameEngine.core.DrawSprite(logo, pos, Color.White, 0f, 1f, 1f);
			if (GameEngine.core.vcount % 60 < 30)
			{
				pos = new Vector2(576f, 390f);
				GameEngine.core.spriteBatch.Draw(text, pos, Color.Black);
			}
		}
	}
}
