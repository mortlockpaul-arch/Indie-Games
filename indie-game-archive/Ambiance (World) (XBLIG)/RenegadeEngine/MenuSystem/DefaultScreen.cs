using Microsoft.Xna.Framework;

namespace RenegadeEngine.MenuSystem;

internal class DefaultScreen : MenuScreen
{
	public DefaultScreen()
	{
		Boundary.Width = Global.ScreenWidth;
		Boundary.Height = Global.ScreenHeight;
		buttons = new MenuButton[1];
		buttons[0] = new MenuButton("Exit", resizeToFont: true);
		buttons[0].Activated += On_Exit;
		highlightedButton = buttons[0];
		highlightedButton.HasFocus(hasFocus: true);
		OrganizeButtonsVertically(new Point(10, 100), buttons, 4);
	}

	private void On_Exit(object sender, PlayerIndexEventArgs e)
	{
		Dispose();
	}
}
