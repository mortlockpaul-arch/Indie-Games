using IMAK3Z0MB1EGAEM;
using Microsoft.Xna.Framework;

namespace Viking_x86.vikinggame.hud;

public class VikingHUD
{
	public string TheMoon = "the MOON";

	public string[] timeStr = new string[4] { "1152 AD", "VIKING TIME", "2131 AD", "FUTURE TIME" };

	public void Draw()
	{
		switch (Game1.vgame.vikingDirector.phase)
		{
		case 0:
		case 2:
			if (Game1.vgame.vikingDirector.timeStrFrame > 1f && Game1.vgame.vikingDirector.timeStrFrame < 4f)
			{
				int num = 0;
				if (Game1.vgame.vikingDirector.phase == 2)
				{
					num = 2;
				}
				float a = 4f - Game1.vgame.vikingDirector.timeStrFrame;
				Text.DrawString(timeStr[num], VScroll.screenSize / 2f + new Vector2(0f, 200f), 5f, new Color(1f, 1f, 1f, a), Text.Justify.Center);
				if (Game1.vgame.vikingDirector.timeStrFrame > 2f)
				{
					Text.DrawString(timeStr[num + 1], VScroll.screenSize / 2f + new Vector2(0f, 230f), 5f, new Color(1f, 1f, 1f, a), Text.Justify.Center);
				}
			}
			break;
		}
		if (Game1.vgame.charMgr.moon.active && Game1.vgame.charMgr.moon.GetDif() > 300f)
		{
			DrawMoonHealthBox();
		}
	}

	private void DrawMoonHealthBox()
	{
		int num = 400;
		int num2 = 15;
		float num3 = Game1.vgame.charMgr.moon.hp / 1500f;
		if (num3 < 0f)
		{
			num3 = 0f;
		}
		if (num3 > 1f)
		{
			num3 = 1f;
		}
		Color color = new Color(1f, 1f, 1f, 0.5f);
		SpriteTools.sprite.Draw(Game1.nullTex, new Rectangle((int)VScroll.screenSize.X / 2 - num / 2, (int)VScroll.screenSize.Y - 80, num, 1), color);
		SpriteTools.sprite.Draw(Game1.nullTex, new Rectangle((int)VScroll.screenSize.X / 2 - num / 2, (int)VScroll.screenSize.Y - 80 + num2 - 1, num, 1), color);
		SpriteTools.sprite.Draw(Game1.nullTex, new Rectangle((int)VScroll.screenSize.X / 2 - num / 2 - 1, (int)VScroll.screenSize.Y - 80, 1, num2), color);
		SpriteTools.sprite.Draw(Game1.nullTex, new Rectangle((int)VScroll.screenSize.X / 2 + num / 2, (int)VScroll.screenSize.Y - 80, 1, num2), color);
		int width = (int)((float)(num - 2) * num3);
		SpriteTools.sprite.Draw(Game1.nullTex, new Rectangle((int)VScroll.screenSize.X / 2 - num / 2 + 1, (int)VScroll.screenSize.Y - 80 + 2, width, num2 - 4), Color.Red);
		Text.DrawString(TheMoon, new Vector2(VScroll.screenSize.X / 2f, VScroll.screenSize.Y - 80f + (float)num2 / 2f), 1.5f, Color.White, Text.Justify.Center);
	}

	internal void Update()
	{
	}
}
