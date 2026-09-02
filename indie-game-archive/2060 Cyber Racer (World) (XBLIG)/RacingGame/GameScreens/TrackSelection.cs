using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RacingGame.GameLogic;
using RacingGame.Graphics;
using RacingGame.Sounds;

namespace RacingGame.GameScreens;

internal class TrackSelection : IGameScreen
{
	private const int NumberOfButtons = 3;

	private const int ActiveButtonWidth = 132;

	private const int InactiveButtonWidth = 108;

	private const int DistanceBetweenButtons = 32;

	private static readonly Rectangle[] ButtonRects;

	private static readonly Rectangle[] TextRects;

	private static int selectedButton;

	private float[] currentButtonSizes = new float[3] { 1f, 0f, 0f };

	private bool ignoreMouse = true;

	public static int SelectedTrackNumber => selectedButton;

	public static RacingGameManager.Level SelectedTrack => (RacingGameManager.Level)selectedButton;

	public bool Render()
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.UI.PostScreenMenuShader.Start();
		BaseGame.UI.RenderMenuBackground();
		BaseGame.UI.RenderBlackBar(220, 280);
		int num = 10;
		int num2 = 18;
		if (Environment.OSVersion.Platform != PlatformID.Win32NT)
		{
			num += 36;
			num2 += 26;
		}
		BaseGame.UI.Headers.RenderOnScreenRelative1600(num, num2, UIRenderer.HeaderSelectTrackGfxRect);
		int num3 = -1;
		if (Input.HasMouseMoved || Input.MouseLeftButtonJustPressed)
		{
			ignoreMouse = false;
		}
		Rectangle rect = BaseGame.CalcRectangleCenteredWithGivenHeight(0, 0, 132 * ButtonRects[0].Height / ButtonRects[0].Width, ButtonRects[0]);
		Rectangle rect2 = BaseGame.CalcRectangleCenteredWithGivenHeight(0, 0, 108 * ButtonRects[0].Height / ButtonRects[0].Width, ButtonRects[0]);
		int num4 = rect.Width + 2 * rect2.Width + 2 * BaseGame.XToRes(32);
		int num5 = BaseGame.XToRes(512) - num4 / 2;
		int num6 = BaseGame.YToRes(258);
		Rectangle rect3 = default(Rectangle);
		Rectangle rect4 = default(Rectangle);
		for (int i = 0; i < 3; i++)
		{
			bool flag = i == selectedButton;
			currentButtonSizes[i] += (float)(flag ? 1 : (-1)) * BaseGame.MoveFactorPerSecond * 2f;
			if (currentButtonSizes[i] < 0f)
			{
				currentButtonSizes[i] = 0f;
			}
			if (currentButtonSizes[i] > 1f)
			{
				currentButtonSizes[i] = 1f;
			}
			Rectangle val = MainMenu.InterpolateRect(rect, rect2, currentButtonSizes[i]);
			((Rectangle)(ref rect3))._002Ector(num5, num6 - (val.Height - rect2.Height) / 2, val.Width, val.Height);
			BaseGame.UI.Buttons.RenderOnScreen(rect3, ButtonRects[i], (Color)(flag ? Color.White : new Color((byte)192, (byte)192, (byte)192, (byte)192)));
			if (flag)
			{
				BaseGame.UI.Buttons.RenderOnScreen(rect3, UIRenderer.TrackButtonSelectionGfxRect);
			}
			((Rectangle)(ref rect4))._002Ector(num5, ((Rectangle)(ref rect3)).Bottom + BaseGame.YToRes(5), rect3.Width, rect3.Height * TextRects[0].Height / ButtonRects[0].Height);
			if (flag)
			{
				BaseGame.UI.Buttons.RenderOnScreen(rect4, TextRects[i], flag ? Color.White : Color.Gray);
			}
			if (Input.MouseInBox(rect3))
			{
				num3 = i;
			}
			num5 += val.Width + BaseGame.XToRes(32);
		}
		if (!ignoreMouse && num3 >= 0)
		{
			selectedButton = num3;
		}
		if (Input.GamePadLeftJustPressed || Input.KeyboardLeftJustPressed)
		{
			Sound.Play(Sound.Sounds.ButtonClick);
			selectedButton = (selectedButton + 3 - 1) % 3;
			ignoreMouse = true;
		}
		else if (Input.GamePadRightJustPressed || Input.KeyboardRightJustPressed)
		{
			Sound.Play(Sound.Sounds.ButtonClick);
			selectedButton = (selectedButton + 1) % 3;
			ignoreMouse = true;
		}
		bool flag2 = BaseGame.UI.RenderBottomButtons(onlyBack: false);
		if ((num3 >= 0 && Input.MouseLeftButtonJustPressed) || flag2 || Input.GamePadAJustPressed || Input.KeyboardSpaceJustPressed)
		{
			RacingGameManager.AddGameScreen(new GameScreen());
		}
		if (Input.KeyboardEscapeJustPressed || Input.GamePadBJustPressed || Input.GamePadBackJustPressed || BaseGame.UI.backButtonPressed)
		{
			return true;
		}
		return false;
	}

	static TrackSelection()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		ButtonRects = (Rectangle[])(object)new Rectangle[3]
		{
			UIRenderer.TrackButtonBeginnerGfxRect,
			UIRenderer.TrackButtonAdvancedGfxRect,
			UIRenderer.TrackButtonExpertGfxRect
		};
		TextRects = (Rectangle[])(object)new Rectangle[3]
		{
			UIRenderer.TrackTextBeginnerGfxRect,
			UIRenderer.TrackTextAdvancedGfxRect,
			UIRenderer.TrackTextExpertGfxRect
		};
		selectedButton = 1;
	}
}
