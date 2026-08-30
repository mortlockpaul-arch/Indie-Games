using Microsoft.Xna.Framework;

namespace IMAK3Z0MB1EGAEM.menu;

public class HighScoreMenu : MenuLevel
{
	public HighScoreMenu()
	{
		title = "HIGH SC0RES!1";
		item = new string[1] { "" };
	}

	public override void Accept()
	{
		if (sel == 0)
		{
			Menu.scoreMode = -1;
			Menu.grace = 3;
		}
		base.Accept();
	}

	public override void Draw(Vector2 orig)
	{
		HighScores.DrawScores(new Vector2(490f, 210f));
		base.Draw(orig);
	}

	public override void Cancel()
	{
		Menu.scoreMode = -1;
		Menu.grace = 3;
		base.Cancel();
	}
}
