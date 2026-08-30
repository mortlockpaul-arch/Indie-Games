namespace IMAK3Z0MB1EGAEM.menu;

public class QuitYouSure : MenuLevel
{
	public QuitYouSure()
	{
		title = "quit: y0u sure!??";
		item = new string[2] { "i shall quit!1", "i shall not quit!!1" };
	}

	public override void Accept()
	{
		switch (sel)
		{
		case 0:
			Menu.needsQuit = true;
			break;
		case 1:
			Menu.quitYouSure = -1;
			Menu.grace = 3;
			break;
		}
		base.Accept();
	}

	public override void Cancel()
	{
		Menu.quitYouSure = -1;
		Menu.grace = 3;
		base.Cancel();
	}
}
