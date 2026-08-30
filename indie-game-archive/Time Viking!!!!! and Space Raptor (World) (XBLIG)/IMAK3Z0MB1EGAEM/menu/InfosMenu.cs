using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Viking_x86;

namespace IMAK3Z0MB1EGAEM.menu;

public class InfosMenu : MenuLevel
{
	private string[] desc;

	public InfosMenu()
	{
		title = "HIGH SC0RES!1";
		item = new string[1] { "" };
		desc = new string[12]
		{
			">BE DUDE WITH LIZARD", ">I TRAVEL TO THE BIG CITY TO MAKE MONEY", ">LOTS OF OBSTACLES, BUT I USE MY HAED", ">(LEFT AND RIGHT THUMBSTICKS TO USE MY HEAD)", ">MEET LOTS OF CRAZY CHARACTERS ON RIDE TO THE TOP", ">CLIMB HIGHER, I'M ON TOP OF THE WORLD", ">BIG PAYOUT IS IN SIGHT, GOTTA KEEP TO IT", ">MOON EXPLODES.", ">I AM SHOWERED IN SWEET, SWEET MOON GOLD", ">SPACE RAPTOR CAN'T EAT GOLD",
			">SHOULD OF BROUGHT SNACKS", ">HUNGRYRAPTOR.JPG"
		};
	}

	public override void Draw(Vector2 orig)
	{
		SpriteTools.End();
		SpriteTools.BeginAlphaPoint();
		SpriteTools.sprite.Draw(Game1.vgame.grassTex, new Vector2(80f, 80f), new Rectangle(576, 272, 75, 75), Color.White, 0f, default(Vector2), 4f, SpriteEffects.None, 1f);
		for (int i = 0; i < desc.Length; i++)
		{
			Text.DrawString(desc[i], new Vector2(200f + ((i < 8) ? 200f : 0f), 88f + (float)i * 40f), 4f, new Color(1f, 1f, 1f, 1f), Text.Justify.Left);
		}
	}

	public override void Accept()
	{
		Menu.infos = -1;
		Menu.grace = 3;
		base.Accept();
	}

	public override void Cancel()
	{
		Menu.infos = -1;
		Menu.grace = 3;
		base.Cancel();
	}
}
