using System;
using RacingGame.GameLogic;
using RacingGame.Graphics;

namespace RacingGame.GameScreens;

internal class Help : IGameScreen
{
	public bool Render()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.UI.PostScreenMenuShader.Start();
		BaseGame.UI.RenderMenuBackground();
		int num = 10;
		int num2 = 18;
		if (Environment.OSVersion.Platform != PlatformID.Win32NT)
		{
			num += 36;
			num2 += 26;
		}
		BaseGame.UI.Headers.RenderOnScreenRelative1600(num, num2, UIRenderer.HeaderHelpGfxRect);
		if (Environment.OSVersion.Platform != PlatformID.Win32NT)
		{
			BaseGame.UI.HelpScreen.RenderOnScreen(BaseGame.CalcRectangleKeep4To3(25, 130, BaseGame.UI.HelpScreen.GfxRectangle.Width - 50, BaseGame.UI.HelpScreen.GfxRectangle.Height - 12), BaseGame.UI.HelpScreen.GfxRectangle);
		}
		else
		{
			BaseGame.UI.HelpScreen.RenderOnScreenRelative4To3(0, 125, BaseGame.UI.HelpScreen.GfxRectangle);
		}
		BaseGame.UI.RenderBottomButtons(onlyBack: true);
		if (Input.KeyboardEscapeJustPressed || Input.GamePadBJustPressed || Input.GamePadBackJustPressed || Input.MouseLeftButtonJustPressed)
		{
			return true;
		}
		return false;
	}
}
