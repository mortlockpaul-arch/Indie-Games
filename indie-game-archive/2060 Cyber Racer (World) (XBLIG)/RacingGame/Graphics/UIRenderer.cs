using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RacingGame.GameLogic;
using RacingGame.Helpers;
using RacingGame.Shaders;
using RacingGame.Tracks;

namespace RacingGame.Graphics;

public class UIRenderer : IDisposable
{
	public enum TrophyType
	{
		Gold,
		Silver,
		Bronze
	}

	public enum TimeFadeupMode
	{
		Plus,
		Minus,
		Normal
	}

	private class TimeFadeupText
	{
		public const float MaxShowTimeMs = 2250f;

		public string text;

		public Color color;

		public float showTimeMs;

		public TimeFadeupText(string setText, Color setColor)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			base._002Ector();
			text = setText;
			color = setColor;
			showTimeMs = 2250f;
		}
	}

	public static readonly Rectangle BackgroundGfxRect;

	public static readonly Rectangle RacingGameLogoGfxRect;

	public static readonly Rectangle BottomLeftBorderGfxRect;

	public static readonly Rectangle BottomRightBorderGfxRect;

	public static readonly Rectangle PressStartGfxRect;

	public static readonly Rectangle HeaderChooseCarGfxRect;

	public static readonly Rectangle HeaderOptionsGfxRect;

	public static readonly Rectangle HeaderSelectTrackGfxRect;

	public static readonly Rectangle HeaderHelpGfxRect;

	public static readonly Rectangle HeaderHighscoresGfxRect;

	public static readonly Rectangle BlackBarGfxRect;

	public static readonly Rectangle OptionsRadioButtonGfxRect;

	public static readonly Rectangle MenuButtonPlayGfxRect;

	public static readonly Rectangle MenuButtonHighscoresGfxRect;

	public static readonly Rectangle MenuButtonOptionsGfxRect;

	public static readonly Rectangle MenuButtonHelpGfxRect;

	public static readonly Rectangle MenuButtonQuitGfxRect;

	public static readonly Rectangle MenuButtonSelectionGfxRect;

	public static readonly Rectangle MenuTextPlayGfxRect;

	public static readonly Rectangle MenuTextHighscoresGfxRect;

	public static readonly Rectangle MenuTextOptionsGfxRect;

	public static readonly Rectangle MenuTextHelpGfxRect;

	public static readonly Rectangle MenuTextQuitGfxRect;

	public static readonly Rectangle BigArrowGfxRect;

	public static readonly Rectangle TrackButtonBeginnerGfxRect;

	public static readonly Rectangle TrackButtonAdvancedGfxRect;

	public static readonly Rectangle TrackButtonExpertGfxRect;

	public static readonly Rectangle TrackButtonSelectionGfxRect;

	public static readonly Rectangle TrackTextBeginnerGfxRect;

	public static readonly Rectangle TrackTextAdvancedGfxRect;

	public static readonly Rectangle TrackTextExpertGfxRect;

	public static readonly Rectangle BottomButtonSelectionGfxRect;

	public static readonly Rectangle BottomButtonAButtonGfxRect;

	public static readonly Rectangle BottomButtonBButtonGfxRect;

	public static readonly Rectangle BottomButtonBackButtonGfxRect;

	public static readonly Rectangle BottomButtonStartButtonGfxRect;

	public static readonly Rectangle SelectionArrowGfxRect;

	public static readonly Rectangle SelectionRadioButtonGfxRect;

	public static readonly Rectangle LapsGfxRect;

	public static readonly Rectangle TachoGfxRect;

	public static readonly Rectangle TachoArrowGfxRect;

	public static readonly Rectangle TachoMphGfxRect;

	public static readonly Rectangle TachoGearGfxRect;

	public static readonly Rectangle CurrentAndBestGfxRect;

	public static readonly Rectangle CurrentTimePosGfxRect;

	public static readonly Rectangle BestTimePosGfxRect;

	public static readonly Rectangle TrackNameGfxRect;

	public static readonly Rectangle Best5GfxRect;

	private Texture background;

	private Texture buttons;

	private Texture headers;

	private Texture helpScreen;

	private Texture optionsScreen;

	private Texture mouseCursor;

	private TextureFont font;

	private PostScreenMenu postScreenMenuShader;

	private PostScreenGlow postScreenGameShader;

	private PreScreenSkyCubeMapping skyCube;

	private LensFlare lensFlare;

	private Texture ingame;

	private Texture[] trophies;

	private List<TimeFadeupText> fadeupTexts;

	private Vector3 oldCarForward;

	private Vector3 oldCarUp;

	private float carMenuTime;

	private Vector3 carPos;

	private int randomCarNumber;

	private Color randomCarColor;

	public bool backButtonPressed;

	private bool showFps;

	public Texture Buttons => buttons;

	public Texture Headers => headers;

	public Texture HelpScreen => helpScreen;

	public Texture OptionsScreen => optionsScreen;

	public Texture Ingame => ingame;

	public PostScreenMenu PostScreenMenuShader => postScreenMenuShader;

	public PostScreenGlow PostScreenGlowShader => postScreenGameShader;

	public TextureCube SkyCubeMapTexture => skyCube.SkyCubeMapTexture;

	public Texture GetTrophyTexture(TrophyType trophyType)
	{
		return trophies[(int)trophyType];
	}

	public UIRenderer()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		trophies = new Texture[3];
		fadeupTexts = new List<TimeFadeupText>();
		oldCarForward = Vector3.Zero;
		oldCarUp = Vector3.Zero;
		carPos = RacingGameManager.Player.CarPosition;
		randomCarNumber = RandomHelper.GetRandomInt(3);
		randomCarColor = RandomHelper.RandomColor;
		base._002Ector();
		background = new Texture("background.png");
		buttons = new Texture("buttons.png");
		headers = new Texture("headers.png");
		helpScreen = new Texture("HelpScreenXbox360.png");
		optionsScreen = new Texture("OptionsScreenXbox360.png");
		mouseCursor = new Texture("MouseCursor.png");
		ingame = new Texture("Ingame.png");
		trophies[0] = new Texture("pokal1");
		trophies[1] = new Texture("pokal2");
		trophies[2] = new Texture("pokal3");
		font = new TextureFont();
		postScreenMenuShader = new PostScreenMenu();
		postScreenGameShader = new PostScreenGlow();
		skyCube = new PreScreenSkyCubeMapping();
		lensFlare = new LensFlare(LensFlare.DefaultSunPos);
		BaseGame.LightDirection = LensFlare.DefaultLightPos;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (background != null)
			{
				background.Dispose();
			}
			if (buttons != null)
			{
				buttons.Dispose();
			}
			if (headers != null)
			{
				headers.Dispose();
			}
			if (helpScreen != null)
			{
				helpScreen.Dispose();
			}
			if (optionsScreen != null)
			{
				optionsScreen.Dispose();
			}
			if (mouseCursor != null)
			{
				mouseCursor.Dispose();
			}
			if (font != null)
			{
				font.Dispose();
			}
			if (postScreenMenuShader != null)
			{
				postScreenMenuShader.Dispose();
			}
			if (postScreenGameShader != null)
			{
				postScreenGameShader.Dispose();
			}
			if (skyCube != null)
			{
				skyCube.Dispose();
			}
			if (lensFlare != null)
			{
				lensFlare.Dispose();
			}
			if (ingame != null)
			{
				ingame.Dispose();
			}
		}
	}

	public void AddTimeFadeupEffect(int timeMilliseconds, TimeFadeupMode mode)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		string text = timeMilliseconds / 1000 / 60 + ":" + (timeMilliseconds / 1000 % 60).ToString("00") + "." + (timeMilliseconds / 10 % 100).ToString("00");
		Color setColor = Color.White;
		switch (mode)
		{
		case TimeFadeupMode.Plus:
			text = "+ " + text;
			setColor = Color.Red;
			break;
		case TimeFadeupMode.Minus:
			text = "- " + text;
			setColor = Color.Green;
			break;
		}
		fadeupTexts.Add(new TimeFadeupText(text, setColor));
	}

	public void RenderTimeFadeupEffects()
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < fadeupTexts.Count; i++)
		{
			TimeFadeupText timeFadeupText = fadeupTexts[i];
			timeFadeupText.showTimeMs -= BaseGame.ElapsedTimeThisFrameInMilliseconds;
			if (timeFadeupText.showTimeMs < 0f)
			{
				fadeupTexts.Remove(timeFadeupText);
				i--;
				continue;
			}
			float newAlpha = 1f;
			if (timeFadeupText.showTimeMs < 1500f)
			{
				newAlpha = timeFadeupText.showTimeMs / 1500f;
			}
			float num = (2250f - timeFadeupText.showTimeMs) / 2250f;
			TextureFont.WriteTextCentered(BaseGame.Width / 2, BaseGame.Height / 3 - (int)(num * (float)BaseGame.Height / 3f), timeFadeupText.text, ColorHelper.ApplyAlphaToColor(timeFadeupText.color, newAlpha), 2.25f);
		}
	}

	public void RenderGameBackground()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (!Track.disableLensFlareInTunnel)
		{
			lensFlare.Render(Color.White);
		}
	}

	public void RenderMenuTrackBackground()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (!RacingGameManager.InCarSelectionScreen)
		{
			RacingGameManager.Landscape.Render();
			RacingGameManager.CarModel.RenderCar(randomCarNumber, randomCarColor, shadowCarMode: false, RacingGameManager.Player.CarRenderMatrix);
		}
	}

	public void UpdateCarInMenu()
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		carMenuTime += BaseGame.ElapsedTimeThisFrameInMilliseconds / 1000f;
		if (carMenuTime > RacingGameManager.Landscape.BestReplay.LapTime)
		{
			carMenuTime -= RacingGameManager.Landscape.BestReplay.LapTime;
		}
		Matrix carMatrixAtTime = RacingGameManager.Landscape.BestReplay.GetCarMatrixAtTime(carMenuTime);
		carPos = ((Matrix)(ref carMatrixAtTime)).Translation;
		RacingGameManager.Player.SetCarPosition(carPos, ((Matrix)(ref carMatrixAtTime)).Forward, ((Matrix)(ref carMatrixAtTime)).Up);
		Vector3 forward = ((Matrix)(ref carMatrixAtTime)).Forward;
		Vector3 up = ((Matrix)(ref carMatrixAtTime)).Up;
		if (oldCarForward == Vector3.Zero)
		{
			oldCarForward = forward;
		}
		if (oldCarUp == Vector3.Zero)
		{
			oldCarUp = up;
		}
		oldCarForward = oldCarForward * 0.95f + forward * 0.05f;
		oldCarUp = oldCarUp * 0.95f + up * 0.05f;
		RacingGameManager.Player.SetCameraPosition(carPos + oldCarForward * 13f - oldCarUp * 1.3f);
		RacingGameManager.Player.Update();
	}

	public void RenderMenuBackground()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.UI.UpdateCarInMenu();
		RenderGameBackground();
		RenderMenuTrackBackground();
		background.RenderOnScreen(BaseGame.ResolutionRect, BackgroundGfxRect, ColorHelper.ApplyAlphaToColor(Color.White, 0.85f));
		float bounceEffect = 1.005f + (float)Math.Sin(BaseGame.TotalTime / 0.46f) * 0.045f * (float)Math.Cos(BaseGame.TotalTime / 0.285f);
		background.RenderOnScreen(BaseGame.CalcRectangleWithBounce(362, 36, 601, 218, bounceEffect), RacingGameLogoGfxRect);
	}

	public void RenderBlackBar(int yPos, int height)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		buttons.RenderOnScreen(BaseGame.CalcRectangle(0, yPos, 1024, height), BlackBarGfxRect, ColorHelper.ApplyAlphaToColor(Color.White, 0.85f));
	}

	public bool RenderBottomButtons(bool onlyBack)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		Rectangle rect = BaseGame.CalcRectangleCenteredWithGivenHeight(0, 587, 48, BottomButtonBButtonGfxRect);
		rect.X = BaseGame.Width - rect.Width - BaseGame.XToRes(50);
		bool flag = Input.MouseInBox(rect);
		int num = BaseGame.XToRes(16);
		int num2 = BaseGame.YToRes(9);
		if (flag)
		{
			((Rectangle)(ref rect))._002Ector(rect.X - num / 2, rect.Y - num2 / 2, rect.Width + num, rect.Height + num2);
		}
		buttons.RenderOnScreen(rect, BottomButtonBButtonGfxRect);
		if (flag)
		{
			buttons.RenderOnScreen(rect, BottomButtonSelectionGfxRect);
		}
		backButtonPressed = flag && Input.MouseLeftButtonJustPressed;
		if (onlyBack)
		{
			return false;
		}
		Rectangle rect2 = BaseGame.CalcRectangleCenteredWithGivenHeight(0, 587, 48, BottomButtonAButtonGfxRect);
		rect2.X = BaseGame.Width - rect2.Width * 2 - BaseGame.XToRes(80);
		bool flag2 = Input.MouseInBox(rect2);
		if (flag2)
		{
			((Rectangle)(ref rect2))._002Ector(rect2.X - num / 2, rect2.Y - num2 / 2, rect2.Width + num, rect2.Height + num2);
		}
		buttons.RenderOnScreen(rect2, BottomButtonAButtonGfxRect);
		if (flag2)
		{
			buttons.RenderOnScreen(rect2, BottomButtonSelectionGfxRect);
			if (Input.MouseLeftButtonJustPressed)
			{
				return true;
			}
		}
		return false;
	}

	public void RenderGameUI(int currentGameTime, int bestLapTime, int lapNumber, float speed, int gear, float acceleration, string trackName, int[] top5LapTimes)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_0519: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0605: Unknown result type (might be due to invalid IL or missing references)
		//IL_0630: Unknown result type (might be due to invalid IL or missing references)
		//IL_0635: Unknown result type (might be due to invalid IL or missing references)
		//IL_063c: Unknown result type (might be due to invalid IL or missing references)
		//IL_064a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0661: Unknown result type (might be due to invalid IL or missing references)
		//IL_0671: Unknown result type (might be due to invalid IL or missing references)
		//IL_0691: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
		if (top5LapTimes == null)
		{
			throw new ArgumentNullException("top5LapTimes");
		}
		Color white = Color.White;
		if (RacingGameManager.Player.GameOver)
		{
			speed = 0f;
			gear = 1;
			acceleration = 0f;
		}
		Rectangle rect = BaseGame.CalcRectangle1600(60, 46, LapsGfxRect.Width, LapsGfxRect.Height);
		ingame.RenderOnScreen(rect, LapsGfxRect, white);
		Rectangle rect2 = BaseGame.CalcRectangle1600(60, 46, CurrentAndBestGfxRect.Width, CurrentAndBestGfxRect.Height);
		rect2.Y = BaseGame.Height - ((Rectangle)(ref rect2)).Bottom;
		ingame.RenderOnScreen(rect2, CurrentAndBestGfxRect, white);
		Rectangle rect3 = BaseGame.CalcRectangle1600(60, 46, TrackNameGfxRect.Width, TrackNameGfxRect.Height);
		rect3.X = BaseGame.Width - ((Rectangle)(ref rect3)).Right;
		ingame.RenderOnScreen(rect3, TrackNameGfxRect, white);
		Rectangle rect4 = BaseGame.CalcRectangle1600(60, 4, Best5GfxRect.Width, Best5GfxRect.Height);
		rect4.X = rect3.X;
		int y = rect4.Y;
		rect4.Y += ((Rectangle)(ref rect3)).Bottom;
		ingame.RenderOnScreen(rect4, Best5GfxRect, white);
		Rectangle rect5 = default(Rectangle);
		((Rectangle)(ref rect5))._002Ector(rect4.X, ((Rectangle)(ref rect4)).Bottom + y, rect4.Width, rect4.Height);
		ingame.RenderOnScreen(rect5, Best5GfxRect, white);
		Rectangle rect6 = default(Rectangle);
		((Rectangle)(ref rect6))._002Ector(rect4.X, ((Rectangle)(ref rect5)).Bottom + y, rect4.Width, rect4.Height);
		ingame.RenderOnScreen(rect6, Best5GfxRect, white);
		Rectangle rect7 = default(Rectangle);
		((Rectangle)(ref rect7))._002Ector(rect4.X, ((Rectangle)(ref rect6)).Bottom + y, rect4.Width, rect4.Height);
		ingame.RenderOnScreen(rect7, Best5GfxRect, white);
		Rectangle rect8 = default(Rectangle);
		((Rectangle)(ref rect8))._002Ector(rect4.X, ((Rectangle)(ref rect7)).Bottom + y, rect4.Width, rect4.Height);
		ingame.RenderOnScreen(rect8, Best5GfxRect, white);
		Rectangle rect9 = BaseGame.CalcRectangle1600(60, 46, TachoGfxRect.Width, TachoGfxRect.Height);
		rect9.X = BaseGame.Width - ((Rectangle)(ref rect9)).Right;
		rect9.Y = BaseGame.Height - ((Rectangle)(ref rect9)).Bottom;
		ingame.RenderOnScreen(rect9, TachoGfxRect, white);
		TextureFontBigNumbers.WriteNumber(rect.X + BaseGame.XToRes1600(15), rect.Y + BaseGame.YToRes1200(12), lapNumber);
		Color val = default(Color);
		((Color)(ref val))._002Ector(byte.MaxValue, (byte)185, (byte)80);
		int num = BaseGame.YToRes1200(74);
		TextureFont.WriteGameTime(rect2.X + BaseGame.XToRes1600(154), rect2.Y + num / 2 - TextureFont.Height / 2, currentGameTime, val);
		TextureFont.WriteGameTime(rect2.X + BaseGame.XToRes1600(154), rect2.Y + rect2.Height / 2 + num / 2 - TextureFont.Height / 2, bestLapTime, Color.White);
		TextureFont.WriteTextCentered(rect3.X + rect3.Width / 2, rect3.Y + num / 2, trackName);
		Color val2 = ((bestLapTime == top5LapTimes[0]) ? val : Color.White);
		TextureFont.WriteTextCentered(rect4.X + BaseGame.XToRes(32) / 2, rect4.Y + num / 2, "1.", val2, 1f);
		TextureFont.WriteGameTime(rect4.X + BaseGame.XToRes(50), rect4.Y + num / 2 - TextureFont.Height / 2, top5LapTimes[0], val2);
		val2 = ((bestLapTime == top5LapTimes[1]) ? val : Color.White);
		TextureFont.WriteTextCentered(rect5.X + BaseGame.XToRes(32) / 2, rect5.Y + num / 2, "2.", val2, 1f);
		TextureFont.WriteGameTime(rect5.X + BaseGame.XToRes(50), rect5.Y + num / 2 - TextureFont.Height / 2, top5LapTimes[1], val2);
		val2 = ((bestLapTime == top5LapTimes[2]) ? val : Color.White);
		TextureFont.WriteTextCentered(rect6.X + BaseGame.XToRes(32) / 2, rect6.Y + num / 2, "3.", val2, 1f);
		TextureFont.WriteGameTime(rect6.X + BaseGame.XToRes(50), rect6.Y + num / 2 - TextureFont.Height / 2, top5LapTimes[2], val2);
		val2 = ((bestLapTime == top5LapTimes[3]) ? val : Color.White);
		TextureFont.WriteTextCentered(rect7.X + BaseGame.XToRes(32) / 2, rect7.Y + num / 2, "4.", val2, 1f);
		TextureFont.WriteGameTime(rect7.X + BaseGame.XToRes(50), rect7.Y + num / 2 - TextureFont.Height / 2, top5LapTimes[3], val2);
		val2 = ((bestLapTime == top5LapTimes[4]) ? val : Color.White);
		TextureFont.WriteTextCentered(rect8.X + BaseGame.XToRes(32) / 2, rect8.Y + num / 2, "5.", val2, 1f);
		TextureFont.WriteGameTime(rect8.X + BaseGame.XToRes(50), rect8.Y + num / 2 - TextureFont.Height / 2, top5LapTimes[4], val2);
		Point val3 = default(Point);
		((Point)(ref val3))._002Ector(rect9.X + BaseGame.XToRes1600(194), rect9.Y + BaseGame.YToRes1200(194));
		if (acceleration < 0f)
		{
			acceleration = 0f;
		}
		if (acceleration > 1f)
		{
			acceleration = 1f;
		}
		float rotation = -2.33f + acceleration * 2.5f;
		int num2 = BaseGame.XToRes1600(TachoArrowGfxRect.Width);
		int num3 = BaseGame.YToRes1200(TachoArrowGfxRect.Height);
		Vector2 rotationPoint = default(Vector2);
		((Vector2)(ref rotationPoint))._002Ector((float)(TachoArrowGfxRect.Width / 2), (float)(TachoArrowGfxRect.Height - 13));
		ingame.RenderOnScreenWithRotation(new Rectangle(val3.X, val3.Y, num2, num3), TachoArrowGfxRect, rotation, rotationPoint);
		TextureFontBigNumbers.WriteNumber(rect9.X + BaseGame.XToRes1600(TachoMphGfxRect.X), rect9.Y + BaseGame.YToRes1200(TachoMphGfxRect.Y), TachoMphGfxRect.Height, (int)Math.Round(speed));
		TextureFontBigNumbers.WriteNumber(rect9.X + BaseGame.XToRes1600(TachoGearGfxRect.X), rect9.Y + BaseGame.YToRes1200(TachoGearGfxRect.Y), TachoGearGfxRect.Height, Math.Min(5, gear));
	}

	public static void Render(LineManager2D lineManager2D)
	{
		if (lineManager2D == null)
		{
			throw new ArgumentNullException("lineManager2D");
		}
		BaseGame.Device.RenderState.DepthBufferEnable = false;
		BaseGame.Device.VertexDeclaration = TangentVertex.VertexDeclaration;
		Texture.additiveSprite.End();
		Texture.alphaSprite.End();
		lineManager2D.Render();
	}

	public void RenderTextsAndMouseCursor()
	{
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		if (showFps)
		{
			TextureFont.WriteText(BaseGame.XToRes(200), BaseGame.YToRes(26), "Fps: " + BaseGame.Fps + " " + BaseGame.Width + "x" + BaseGame.Height);
		}
		RenderTimeFadeupEffects();
		font.WriteAll();
		if (Input.MouseDetected && RacingGameManager.ShowMouseCursor)
		{
			Texture.alphaSprite.Begin((SpriteBlendMode)2);
			Texture.additiveSprite.Begin((SpriteBlendMode)1);
			mouseCursor.RenderOnScreen(Input.MousePos);
			Texture.additiveSprite.End();
			Texture.alphaSprite.End();
		}
	}

	static UIRenderer()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		BackgroundGfxRect = new Rectangle(0, 0, 1024, 640);
		RacingGameLogoGfxRect = new Rectangle(0, 649, 1024, 374);
		BottomLeftBorderGfxRect = new Rectangle(2, 984, 38, 37);
		BottomRightBorderGfxRect = new Rectangle(42, 984, 38, 37);
		PressStartGfxRect = new Rectangle(2, 1, 631, 45);
		HeaderChooseCarGfxRect = new Rectangle(0, 212, 512, 100);
		HeaderOptionsGfxRect = new Rectangle(512, 212, 512, 100);
		HeaderSelectTrackGfxRect = new Rectangle(0, 312, 512, 100);
		HeaderHelpGfxRect = new Rectangle(512, 312, 512, 100);
		HeaderHighscoresGfxRect = new Rectangle(0, 412, 512, 100);
		BlackBarGfxRect = new Rectangle(99, 999, 1, 1);
		OptionsRadioButtonGfxRect = new Rectangle(128, 980, 25, 25);
		MenuButtonPlayGfxRect = new Rectangle(0, 0, 212, 212);
		MenuButtonHighscoresGfxRect = new Rectangle(212, 0, 212, 212);
		MenuButtonOptionsGfxRect = new Rectangle(424, 0, 212, 212);
		MenuButtonHelpGfxRect = new Rectangle(636, 0, 212, 212);
		MenuButtonQuitGfxRect = new Rectangle(212, 240, 212, 212);
		MenuButtonSelectionGfxRect = new Rectangle(636, 240, 212, 212);
		MenuTextPlayGfxRect = new Rectangle(0, 214, 212, 24);
		MenuTextHighscoresGfxRect = new Rectangle(212, 214, 212, 24);
		MenuTextOptionsGfxRect = new Rectangle(424, 214, 212, 24);
		MenuTextHelpGfxRect = new Rectangle(636, 214, 212, 24);
		MenuTextQuitGfxRect = new Rectangle(212, 454, 212, 24);
		BigArrowGfxRect = new Rectangle(867, 242, 127, 178);
		TrackButtonBeginnerGfxRect = new Rectangle(0, 480, 212, 352);
		TrackButtonAdvancedGfxRect = new Rectangle(212, 480, 212, 352);
		TrackButtonExpertGfxRect = new Rectangle(424, 480, 212, 352);
		TrackButtonSelectionGfxRect = new Rectangle(636, 480, 212, 352);
		TrackTextBeginnerGfxRect = new Rectangle(0, 834, 212, 24);
		TrackTextAdvancedGfxRect = new Rectangle(212, 834, 212, 24);
		TrackTextExpertGfxRect = new Rectangle(424, 834, 212, 24);
		BottomButtonSelectionGfxRect = new Rectangle(424, 240, 212, 92);
		BottomButtonAButtonGfxRect = new Rectangle(0, 872, 212, 92);
		BottomButtonBButtonGfxRect = new Rectangle(212, 872, 212, 92);
		BottomButtonBackButtonGfxRect = new Rectangle(424, 872, 212, 92);
		BottomButtonStartButtonGfxRect = new Rectangle(636, 872, 212, 92);
		SelectionArrowGfxRect = new Rectangle(874, 426, 53, 39);
		SelectionRadioButtonGfxRect = new Rectangle(935, 427, 39, 39);
		LapsGfxRect = new Rectangle(381, 132, 222, 160);
		TachoGfxRect = new Rectangle(0, 0, 343, 341);
		TachoArrowGfxRect = new Rectangle(347, 0, 28, 186);
		TachoMphGfxRect = new Rectangle(184, 256, 148, 72);
		TachoGearGfxRect = new Rectangle(286, 149, 52, 72);
		CurrentAndBestGfxRect = new Rectangle(381, 2, 342, 128);
		CurrentTimePosGfxRect = new Rectangle(540, 8, 170, 52);
		BestTimePosGfxRect = new Rectangle(540, 72, 170, 52);
		TrackNameGfxRect = new Rectangle(726, 2, 282, 62);
		Best5GfxRect = new Rectangle(726, 66, 282, 62);
	}
}
