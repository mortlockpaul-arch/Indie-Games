using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RacingGame.GameLogic;
using RacingGame.Graphics;
using RacingGame.Properties;
using RacingGame.Sounds;

namespace RacingGame.GameScreens;

internal class Options : IGameScreen
{
	private readonly Rectangle Line4ArrowGfxRect;

	private readonly Rectangle Line5ArrowGfxRect;

	private readonly Rectangle Line6ArrowGfxRect;

	private readonly Rectangle Resolution640x480GfxRect;

	private readonly Rectangle Resolution800x600GfxRect;

	private readonly Rectangle Resolution1024x768GfxRect;

	private readonly Rectangle Resolution1280x1024GfxRect;

	private readonly Rectangle ResolutionAutoGfxRect;

	private readonly Rectangle FullscreenGfxRect;

	private readonly Rectangle PostScreenEffectsGfxRect;

	private readonly Rectangle ShadowsGfxRect;

	private readonly Rectangle HighDetailGfxRect;

	private readonly Rectangle SoundGfxRect;

	private readonly Rectangle MusicGfxRect;

	private readonly Rectangle SensitivityGfxRect;

	private string currentPlayerName;

	private int currentOptionsNumber;

	private int currentResolution;

	private bool fullscreen;

	private bool usePostScreenShaders;

	private bool useShadowMapping;

	private bool useHighDetail;

	private float currentMusicVolume;

	private float currentSoundVolume;

	private float currentSensitivity;

	public Options()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		Line4ArrowGfxRect = new Rectangle(154, 284, 62, 39);
		Line5ArrowGfxRect = new Rectangle(160, 354, 62, 39);
		Line6ArrowGfxRect = new Rectangle(72, 437, 62, 39);
		Resolution640x480GfxRect = new Rectangle(339, 112, 98, 32);
		Resolution800x600GfxRect = new Rectangle(454, 112, 98, 32);
		Resolution1024x768GfxRect = new Rectangle(575, 112, 108, 32);
		Resolution1280x1024GfxRect = new Rectangle(704, 112, 116, 32);
		ResolutionAutoGfxRect = new Rectangle(838, 112, 69, 32);
		FullscreenGfxRect = new Rectangle(339, 182, 105, 36);
		PostScreenEffectsGfxRect = new Rectangle(339, 226, 206, 36);
		ShadowsGfxRect = new Rectangle(616, 226, 90, 36);
		HighDetailGfxRect = new Rectangle(784, 226, 120, 36);
		SoundGfxRect = new Rectangle(384, 281, 448, 39);
		MusicGfxRect = new Rectangle(384, 354, 448, 39);
		SensitivityGfxRect = new Rectangle(384, 428, 448, 39);
		currentPlayerName = GameSettings.Default.PlayerName;
		currentResolution = 4;
		fullscreen = true;
		usePostScreenShaders = true;
		useShadowMapping = true;
		useHighDetail = true;
		currentMusicVolume = 1f;
		currentSoundVolume = 1f;
		currentSensitivity = 1f;
		base._002Ector();
		if (BaseGame.Width == 640 && BaseGame.Height == 480)
		{
			currentResolution = 0;
		}
		if (BaseGame.Width == 800 && BaseGame.Height == 600)
		{
			currentResolution = 1;
		}
		if (BaseGame.Width == 1024 && BaseGame.Height == 768)
		{
			currentResolution = 2;
		}
		if (BaseGame.Width == 1280 && BaseGame.Height == 1024)
		{
			currentResolution = 3;
		}
		fullscreen = BaseGame.Fullscreen;
		usePostScreenShaders = BaseGame.UsePostScreenShaders;
		useShadowMapping = BaseGame.AllowShadowMapping;
		useHighDetail = BaseGame.HighDetail;
		currentMusicVolume = GameSettings.Default.MusicVolume;
		currentSoundVolume = GameSettings.Default.SoundVolume;
		currentSensitivity = GameSettings.Default.ControllerSensitivity;
	}

	public bool Render()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0519: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.UI.PostScreenMenuShader.Start();
		BaseGame.UI.RenderMenuBackground();
		int num = 10;
		int num2 = 18;
		if (Environment.OSVersion.Platform != PlatformID.Win32NT)
		{
			num += 36;
			num2 += 26;
		}
		BaseGame.UI.Headers.RenderOnScreenRelative1600(num, num2, UIRenderer.HeaderOptionsGfxRect);
		BaseGame.UI.OptionsScreen.RenderOnScreenRelative4To3(0, 125, BaseGame.UI.OptionsScreen.GfxRectangle);
		int x = BaseGame.XToRes(352);
		int y = BaseGame.YToRes768(170);
		TextureFont.WriteText(x, y, currentPlayerName);
		Rectangle rect = BaseGame.CalcRectangleKeep4To3(SoundGfxRect);
		rect.Y += BaseGame.YToRes768(125);
		if (Input.MouseInBox(rect) && Input.MouseLeftButtonJustPressed)
		{
			currentSoundVolume = (float)(Input.MousePos.X - rect.X) / (float)rect.Width;
			Sound.Play(Sound.Sounds.Highlight);
		}
		if (currentOptionsNumber == 0)
		{
			if (Input.GamePadLeftJustPressed || Input.KeyboardLeftJustPressed)
			{
				currentSoundVolume -= 0.1f;
				Sound.Play(Sound.Sounds.Highlight);
			}
			if (Input.GamePadRightJustPressed || Input.KeyboardRightJustPressed)
			{
				currentSoundVolume += 0.1f;
				Sound.Play(Sound.Sounds.Highlight);
			}
			if (currentSoundVolume < 0f)
			{
				currentSoundVolume = 0f;
			}
			if (currentSoundVolume > 1f)
			{
				currentSoundVolume = 1f;
			}
		}
		Rectangle selectionRadioButtonGfxRect = UIRenderer.SelectionRadioButtonGfxRect;
		BaseGame.UI.Buttons.RenderOnScreen(new Rectangle(rect.X + (int)((float)rect.Width * currentSoundVolume) - BaseGame.XToRes(selectionRadioButtonGfxRect.Width) / 2, rect.Y, BaseGame.XToRes(selectionRadioButtonGfxRect.Width), BaseGame.YToRes768(selectionRadioButtonGfxRect.Height)), selectionRadioButtonGfxRect);
		Rectangle rect2 = BaseGame.CalcRectangleKeep4To3(MusicGfxRect);
		rect2.Y += BaseGame.YToRes768(125);
		if (Input.MouseInBox(rect2) && Input.MouseLeftButtonJustPressed)
		{
			currentMusicVolume = (float)(Input.MousePos.X - rect2.X) / (float)rect2.Width;
			Sound.Play(Sound.Sounds.Highlight);
		}
		if (currentOptionsNumber == 1)
		{
			if (Input.GamePadLeftJustPressed || Input.KeyboardLeftJustPressed)
			{
				currentMusicVolume -= 0.1f;
				Sound.Play(Sound.Sounds.Highlight);
			}
			if (Input.GamePadRightJustPressed || Input.KeyboardRightJustPressed)
			{
				currentMusicVolume += 0.1f;
				Sound.Play(Sound.Sounds.Highlight);
			}
			if (currentMusicVolume < 0f)
			{
				currentMusicVolume = 0f;
			}
			if (currentMusicVolume > 1f)
			{
				currentMusicVolume = 1f;
			}
		}
		BaseGame.UI.Buttons.RenderOnScreen(new Rectangle(rect2.X + (int)((float)rect2.Width * currentMusicVolume) - BaseGame.XToRes(selectionRadioButtonGfxRect.Width) / 2, rect2.Y, BaseGame.XToRes(selectionRadioButtonGfxRect.Width), BaseGame.YToRes768(selectionRadioButtonGfxRect.Height)), selectionRadioButtonGfxRect);
		Sound.SetVolumes(currentSoundVolume, currentMusicVolume);
		Rectangle rect3 = BaseGame.CalcRectangleKeep4To3(SensitivityGfxRect);
		rect3.Y += BaseGame.YToRes768(125);
		if (Input.MouseInBox(rect3) && Input.MouseLeftButtonJustPressed)
		{
			currentSensitivity = (float)(Input.MousePos.X - rect3.X) / (float)rect3.Width;
			Sound.Play(Sound.Sounds.Highlight);
		}
		if (currentOptionsNumber == 2)
		{
			if (Input.GamePadLeftJustPressed || Input.KeyboardLeftJustPressed)
			{
				currentSensitivity -= 0.1f;
				Sound.Play(Sound.Sounds.Highlight);
			}
			if (Input.GamePadRightJustPressed || Input.KeyboardRightJustPressed)
			{
				currentSensitivity += 0.1f;
				Sound.Play(Sound.Sounds.Highlight);
			}
			if (currentSensitivity < 0f)
			{
				currentSensitivity = 0f;
			}
			if (currentSensitivity > 1f)
			{
				currentSensitivity = 1f;
			}
		}
		BaseGame.UI.Buttons.RenderOnScreen(new Rectangle(rect3.X + (int)((float)rect3.Width * currentSensitivity) - BaseGame.XToRes(selectionRadioButtonGfxRect.Width) / 2, rect3.Y, BaseGame.XToRes(selectionRadioButtonGfxRect.Width), BaseGame.YToRes768(selectionRadioButtonGfxRect.Height)), selectionRadioButtonGfxRect);
		Rectangle[] array = (Rectangle[])(object)new Rectangle[3] { Line4ArrowGfxRect, Line5ArrowGfxRect, Line6ArrowGfxRect };
		for (int i = 0; i < array.Length; i++)
		{
			Rectangle rect4 = BaseGame.CalcRectangleKeep4To3(array[i]);
			rect4.Y += BaseGame.YToRes768(125);
			rect4.X -= BaseGame.XToRes(8 + (int)Math.Round(8.0 * Math.Sin(BaseGame.TotalTime / 0.21212f)));
			if (currentOptionsNumber == i)
			{
				BaseGame.UI.Buttons.RenderOnScreen(rect4, UIRenderer.SelectionArrowGfxRect, Color.White);
			}
		}
		if (Input.GamePadUpJustPressed || Input.KeyboardUpJustPressed)
		{
			Sound.Play(Sound.Sounds.Highlight);
			currentOptionsNumber = (array.Length + currentOptionsNumber - 1) % array.Length;
		}
		else if (Input.GamePadDownJustPressed || Input.KeyboardDownJustPressed)
		{
			Sound.Play(Sound.Sounds.Highlight);
			currentOptionsNumber = (currentOptionsNumber + 1) % array.Length;
		}
		BaseGame.UI.RenderBottomButtons(onlyBack: true);
		if (Input.KeyboardEscapeJustPressed || Input.GamePadBJustPressed || Input.GamePadBackJustPressed || BaseGame.UI.backButtonPressed)
		{
			GameSettings.Default.PlayerName = currentPlayerName;
			switch (currentResolution)
			{
			case 0:
				GameSettings.Default.ResolutionWidth = 640;
				GameSettings.Default.ResolutionHeight = 480;
				break;
			case 1:
				GameSettings.Default.ResolutionWidth = 800;
				GameSettings.Default.ResolutionHeight = 600;
				break;
			case 2:
				GameSettings.Default.ResolutionWidth = 1024;
				GameSettings.Default.ResolutionHeight = 768;
				break;
			case 3:
				GameSettings.Default.ResolutionWidth = 1280;
				GameSettings.Default.ResolutionHeight = 1024;
				break;
			case 4:
				GameSettings.Default.ResolutionWidth = 0;
				GameSettings.Default.ResolutionHeight = 0;
				break;
			}
			GameSettings.Default.Fullscreen = fullscreen;
			GameSettings.Default.PostScreenEffects = usePostScreenShaders;
			GameSettings.Default.ShadowMapping = useShadowMapping;
			GameSettings.Default.HighDetail = useHighDetail;
			GameSettings.Default.MusicVolume = currentMusicVolume;
			GameSettings.Default.SoundVolume = currentSoundVolume;
			GameSettings.Default.ControllerSensitivity = currentSensitivity;
			GameSettings.Save();
			BaseGame.CheckOptionsAndPSVersion();
			return true;
		}
		return false;
	}
}
