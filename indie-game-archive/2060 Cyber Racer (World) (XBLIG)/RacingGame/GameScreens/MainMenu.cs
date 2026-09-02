using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RacingGame.GameLogic;
using RacingGame.Graphics;
using RacingGame.Sounds;

namespace RacingGame.GameScreens;

internal class MainMenu : IGameScreen
{
	private const int NumberOfButtons = 5;

	private const int ActiveButtonWidth = 132;

	private const int InactiveButtonWidth = 108;

	private const int DistanceBetweenButtons = 14;

	private const float TimeOutMenu = 10000f;

	private static readonly Rectangle[] ButtonRects;

	private static readonly Rectangle[] TextRects;

	private int selectedButton;

	private float[] currentButtonSizes = new float[5] { 1f, 0f, 0f, 0f, 0f };

	private bool ignoreMouse = true;

	private float idleTime;

	private float pressedLeftMs;

	private float pressedRightMs;

	private int SelectedButton
	{
		get
		{
			return selectedButton;
		}
		set
		{
			selectedButton = value;
			idleTime = 0f;
		}
	}

	internal static Rectangle InterpolateRect(Rectangle rect1, Rectangle rect2, float interpolation)
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		return new Rectangle((int)Math.Round((float)rect1.X * interpolation + (float)rect2.X * (1f - interpolation)), (int)Math.Round((float)rect1.Y * interpolation + (float)rect2.Y * (1f - interpolation)), (int)Math.Round((float)rect1.Width * interpolation + (float)rect2.Width * (1f - interpolation)), (int)Math.Round((float)rect1.Height * interpolation + (float)rect2.Height * (1f - interpolation)));
	}

	public bool Render()
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.UI.PostScreenMenuShader.Start();
		BaseGame.UI.RenderMenuBackground();
		BaseGame.UI.RenderBlackBar(280, 192);
		int num = -1;
		if (Input.HasMouseMoved || Input.MouseLeftButtonJustPressed)
		{
			ignoreMouse = false;
		}
		Rectangle rect = BaseGame.CalcRectangleCenteredWithGivenHeight(0, 0, 132, ButtonRects[0]);
		Rectangle rect2 = BaseGame.CalcRectangleCenteredWithGivenHeight(0, 0, 108, ButtonRects[0]);
		int num2 = rect.Width + 4 * rect2.Width + 4 * BaseGame.XToRes(14);
		int num3 = BaseGame.XToRes(512) - num2 / 2;
		int num4 = BaseGame.YToRes(316);
		Rectangle rect3 = default(Rectangle);
		Rectangle rect4 = default(Rectangle);
		for (int i = 0; i < 5; i++)
		{
			bool flag = i == SelectedButton;
			currentButtonSizes[i] += (float)(flag ? 1 : (-1)) * BaseGame.MoveFactorPerSecond * 2f;
			if (currentButtonSizes[i] < 0f)
			{
				currentButtonSizes[i] = 0f;
			}
			if (currentButtonSizes[i] > 1f)
			{
				currentButtonSizes[i] = 1f;
			}
			Rectangle val = InterpolateRect(rect, rect2, currentButtonSizes[i]);
			((Rectangle)(ref rect3))._002Ector(num3, num4 - (val.Height - rect2.Height) / 2, val.Width, val.Height);
			BaseGame.UI.Buttons.RenderOnScreen(rect3, ButtonRects[i], (Color)(flag ? Color.White : new Color((byte)192, (byte)192, (byte)192, (byte)192)));
			if (flag)
			{
				BaseGame.UI.Buttons.RenderOnScreen(rect3, UIRenderer.MenuButtonSelectionGfxRect);
			}
			((Rectangle)(ref rect4))._002Ector(num3, ((Rectangle)(ref rect3)).Bottom + BaseGame.YToRes(5), rect3.Width, rect3.Height * TextRects[0].Height / ButtonRects[0].Height);
			if (flag)
			{
				BaseGame.UI.Buttons.RenderOnScreen(rect4, TextRects[i], (Color)(flag ? Color.White : new Color((byte)192, (byte)192, (byte)192, (byte)192)));
			}
			if (Input.MouseInBox(rect3))
			{
				num = i;
			}
			num3 += val.Width + BaseGame.XToRes(14);
		}
		if (!ignoreMouse && num >= 0)
		{
			SelectedButton = num;
		}
		if (Input.KeyboardLeftPressed || Input.GamePadLeftPressed)
		{
			pressedLeftMs += BaseGame.ElapsedTimeThisFrameInMilliseconds;
		}
		else
		{
			pressedLeftMs = 0f;
		}
		if (Input.KeyboardRightPressed || Input.GamePadRightPressed)
		{
			pressedRightMs += BaseGame.ElapsedTimeThisFrameInMilliseconds;
		}
		else
		{
			pressedRightMs = 0f;
		}
		if (Input.GamePadLeftJustPressed || Input.KeyboardLeftJustPressed || (pressedLeftMs > 250f && (Input.KeyboardLeftPressed || Input.GamePadLeftPressed)))
		{
			pressedLeftMs -= 250f;
			Sound.Play(Sound.Sounds.Highlight);
			SelectedButton = (SelectedButton + 5 - 1) % 5;
			ignoreMouse = true;
		}
		else if (Input.GamePadRightJustPressed || Input.KeyboardRightJustPressed || (pressedRightMs > 250f && (Input.KeyboardRightPressed || Input.GamePadRightPressed)))
		{
			pressedRightMs -= 250f;
			Sound.Play(Sound.Sounds.Highlight);
			SelectedButton = (SelectedButton + 1) % 5;
			ignoreMouse = true;
		}
		if ((num >= 0 && Input.MouseLeftButtonJustPressed) || Input.GamePadAJustPressed || Input.KeyboardSpaceJustPressed)
		{
			idleTime = 0f;
			switch (SelectedButton)
			{
			case 0:
				RacingGameManager.AddGameScreen(new CarSelection());
				break;
			case 1:
				RacingGameManager.AddGameScreen(new Highscores());
				break;
			case 2:
				RacingGameManager.AddGameScreen(new Options());
				break;
			case 3:
				RacingGameManager.AddGameScreen(new Help());
				break;
			case 4:
				return true;
			}
		}
		idleTime += BaseGame.ElapsedTimeThisFrameInMilliseconds;
		if (Input.KeyboardEscapeJustPressed || Input.GamePadBackJustPressed || idleTime > 10000f)
		{
			idleTime = 0f;
			RacingGameManager.AddGameScreen(new SplashScreen());
		}
		return false;
	}

	static MainMenu()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		ButtonRects = (Rectangle[])(object)new Rectangle[5]
		{
			UIRenderer.MenuButtonPlayGfxRect,
			UIRenderer.MenuButtonHighscoresGfxRect,
			UIRenderer.MenuButtonOptionsGfxRect,
			UIRenderer.MenuButtonHelpGfxRect,
			UIRenderer.MenuButtonQuitGfxRect
		};
		TextRects = (Rectangle[])(object)new Rectangle[5]
		{
			UIRenderer.MenuTextPlayGfxRect,
			UIRenderer.MenuTextHighscoresGfxRect,
			UIRenderer.MenuTextOptionsGfxRect,
			UIRenderer.MenuTextHelpGfxRect,
			UIRenderer.MenuTextQuitGfxRect
		};
	}
}
