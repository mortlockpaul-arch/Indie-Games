using System;
using System.Collections.Generic;
using System.Linq;
using Maximinus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Billard3;

public class GameMenus : Menus.ManagerV2
{
	public enum ScreenID
	{
		Main,
		Lobby,
		Message,
		YourTurn,
		GameOver,
		PromptAddCpu,
		PromptCpuLevel,
		CheatPrompt,
		CheatPromptSP,
		CheatPromptMP,
		HowToPlay,
		Rules,
		ReallyQuit,
		FirstPrompt,
		Pause,
		Credits1,
		Credits2,
		Purchase,
		EndTrial,
		PopupControls
	}

	private enum ActionMain
	{
		SinglePlayer,
		MultiPlayer,
		Rules,
		HowToPlay,
		About,
		Quit
	}

	private enum ActionPause
	{
		Resume,
		Controls,
		MainMenu
	}

	private enum OtherGame
	{
		GlobeClicker,
		MissileEscape,
		AvatarSlamDunk
	}

	public class Textures
	{
		public static Texture2D checkMark;

		public static Texture2D howToPlay;

		public static Texture2D MenuBG;

		public static Texture2D MenuSP;

		public static Texture2D MenuMP;

		public static Texture2D MenuRules;

		public static Texture2D MenuHTP;

		public static Texture2D MenuQuit;

		public static Texture2D MenuStart;

		public static Texture2D MenuAbout;

		public static Texture2D MaximinusSmall;

		public static Texture2D GameGC;

		public static Texture2D GameME;

		public static Texture2D GameASD;

		public static Texture2D GameName;

		public static Texture2D black;

		public static Texture2D buttY;

		public static Texture2D buttB;

		public static Texture2D BoxArt;

		public static Texture2D MenuRulesFunky;

		public static Video[] videos = new Video[3];

		public static void LoadContent(ContentManager Content)
		{
			string texSizeName = MaximinusGame.TexSizeName;
			checkMark = Content.Load<Texture2D>("tex/checkmark" + texSizeName);
			howToPlay = Content.Load<Texture2D>("tex/howToPlay" + texSizeName);
			MenuBG = Content.Load<Texture2D>("tex/menuBG");
			MenuSP = Content.Load<Texture2D>("tex/menuSP" + texSizeName);
			MenuMP = Content.Load<Texture2D>("tex/menuMP" + texSizeName);
			MenuHTP = Content.Load<Texture2D>("tex/menuHTP" + texSizeName);
			MenuRules = Content.Load<Texture2D>("tex/rules" + texSizeName);
			MenuQuit = Content.Load<Texture2D>("tex/quit" + texSizeName);
			MenuStart = Content.Load<Texture2D>("tex/start" + texSizeName);
			MenuAbout = Content.Load<Texture2D>("tex/menuAbout" + texSizeName);
			MaximinusSmall = Content.Load<Texture2D>("tex/maximinusSmall" + texSizeName);
			GameGC = Content.Load<Texture2D>("tex/GameGC" + texSizeName);
			GameME = Content.Load<Texture2D>("tex/GameME" + texSizeName);
			GameASD = Content.Load<Texture2D>("tex/GameASD" + texSizeName);
			GameName = Content.Load<Texture2D>("tex/gamename" + ((MaximinusGame.Id == MaximinusGame.ID.Billard9Ball) ? "-9ball" : "") + ((MaximinusGame.Id == MaximinusGame.ID.FunkyPool) ? "Funky" : "") + texSizeName);
			buttY = Content.Load<Texture2D>("tex/buttY" + texSizeName);
			buttB = Content.Load<Texture2D>("tex/buttB" + texSizeName);
			BoxArt = Content.Load<Texture2D>("tex/BoxArt" + ((MaximinusGame.Id == MaximinusGame.ID.FunkyPool) ? "Funky" : "") + texSizeName);
			black = new Texture2D(Statics.draw2D.Device, 1, 1);
			black.SetData(new Color[1] { Color.Black });
			videos[0] = Content.Load<Video>("videos/videoGC");
			videos[1] = Content.Load<Video>("videos/videoME");
			videos[2] = Content.Load<Video>("videos/videoASD");
			if (MaximinusGame.Id == MaximinusGame.ID.FunkyPool)
			{
				MenuRulesFunky = Content.Load<Texture2D>("tex/menuRulesFunky" + texSizeName);
			}
		}
	}

	private const bool defaultDeco = false;

	private const double infoMaxTime = 3.0;

	private const double quickinfoMaxTime = 3.0;

	public static readonly Color ColorOutline = Color.White;

	private static readonly Color ColorTextNotSelected = new Color(Vector3.One * 0.4f);

	public static readonly Color ColorText = new Color(Vector3.One * 1f);

	public static readonly Color ColorOverlay = Utils.ColorWithAlpha(Color.White, 0.85f);

	public Menus.ScreenInfo PauseMenu;

	public bool Paused;

	public PlayerIndex PauseController = (PlayerIndex)(-2);

	private GameTime gameTime;

	private Menus.MenuScreen promptAddCpu;

	private Menus.MenuScreen screenCred2;

	private Menus.MenuScreen purchase;

	private Menus.ScreenInfo endTrial;

	private double endTrialStartTime;

	private RoundedRectangle videoRect;

	private VideoPlayer vPlayer = new VideoPlayer();

	private bool musicBeforeScreenCredits;

	public Menus.ScreenInfo popupControls;

	private List<PlayerIndex> promptAddCpuTeamA;

	private List<PlayerIndex> promptAddCpuTeamB;

	private bool promptAddCpuResult;

	private GameModeRules.Type promptCputLevelGameType;

	private Bot.Level promptCpuLevelResult;

	private bool reallyQuit;

	private OtherGame gameChosen;

	private Texture2D vTexture => vPlayer.GetTexture();

	public bool AllowCameraMove => screenCred2.state != Menus.Screen.State.Active;

	public bool PromptPurchase => purchase.state != Menus.Screen.State.Hidden;

	public bool PromptEndTrial => endTrial.state != Menus.Screen.State.Hidden;

	public bool DisableGameplayInput => FindScreenInfo(3).HasToBeDrawn;

	private void PlayVideo()
	{
		vPlayer.Volume = 0.57f;
		vPlayer.Play(Textures.videos[(int)gameChosen]);
	}

	public override void AddScreen(Menus.Screen s)
	{
		s.Activated += Activate;
		s.Cancelled += Back;
		s.TransitionTimeSeconds = 0.4f;
		base.AddScreen(s);
	}

	private void AddScreen(Menus.MenuScreen s)
	{
		s.ChangedValue += ChangedValue;
		AddScreen((Menus.Screen)s);
	}

	public override void Update(GameTime gameTime)
	{
		this.gameTime = gameTime;
		if (Paused && GameState.Current == GameState.Type.GAME_OVER)
		{
			Statics.callbacks.PauseOFF();
		}
		if (GameState.Current != GameState.Type.MENUS)
		{
			if (base.AnyActiveScreen)
			{
				base.ActiveScreen.Disable();
			}
		}
		else
		{
			List<Menus.ScreenInfo> list = new List<Menus.ScreenInfo>();
			if (AnyActiveScreenNotMenu(list))
			{
				foreach (Menus.ScreenInfo item in list)
				{
					item.DisableIfNecessary();
				}
			}
		}
		if (endTrial.state == Menus.Screen.State.Active)
		{
			if (!Trial.IsTrial)
			{
				ChangeFocus(0);
			}
			else if (Statics.callbacks.GameIsActive && gameTime.TotalGameTime.TotalSeconds > endTrialStartTime + 30.0)
			{
				Statics.callbacks.Exit();
			}
		}
		if (Trial.IsTrial && !PromptEndTrial && gameTime.TotalGameTime.TotalMinutes > 4.0)
		{
			if (GameState.Current == GameState.Type.MENUS)
			{
				if (vPlayer.State == MediaState.Playing)
				{
					vPlayer.Stop();
				}
				ChangeFocus(18);
			}
			else
			{
				Statics.callbacks.Menus(gameTime, 18);
			}
		}
		base.Update(gameTime);
	}

	public void AddDebug(List<string> ls)
	{
	}

	public override void render(GameTime gameTime)
	{
		base.render(gameTime);
		if (screenCred2.state == Menus.Screen.State.TransitionOn && vPlayer.State != MediaState.Playing)
		{
			PlayVideo();
		}
		if (screenCred2.state == Menus.Screen.State.Active && vTexture != null)
		{
			draw2D.SpriteBatch.Draw(vTexture, videoRect.Rect, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.1f);
			videoRect.Draw(draw2D.SpriteBatch);
		}
		if (purchase.state != Menus.Screen.State.Hidden)
		{
			if (!Trial.IsTrial)
			{
				purchase.DisableIfNecessary();
			}
			DrawDeco(purchase.Overlay, ColorOutline, purchase.TransitionPosition, 1f);
		}
	}

	private void AddCredits(Menus.MenuScreen cred, string s)
	{
		Menus.MenuEntryValue<string> menuEntryValue = new Menus.MenuEntryValue<string>(cred.entries.Count, s, draw2D.Font, 0);
		menuEntryValue.Selectable = false;
		menuEntryValue.OverrideStringColorBack(ColorText);
		cred.AddEntryValue(menuEntryValue);
	}

	public override void HandleInput(Utils.Input.ActionMenu action)
	{
		ScreenID activeScreenId = (ScreenID)base.ActiveScreenId;
		if (((activeScreenId != ScreenID.Credits1 && activeScreenId != ScreenID.HowToPlay && activeScreenId != ScreenID.Rules) || action == Utils.Input.ActionMenu.MENU_ACTIVATE || action == Utils.Input.ActionMenu.MENU_BACK) && action != Utils.Input.ActionMenu.MENU_BUTTON_X && activeScreenId != ScreenID.EndTrial && ((action != Utils.Input.ActionMenu.MENU_LEFT && action != Utils.Input.ActionMenu.MENU_RIGHT) || (activeScreenId != ScreenID.Credits1 && activeScreenId != ScreenID.EndTrial && activeScreenId != ScreenID.FirstPrompt && activeScreenId != ScreenID.GameOver && activeScreenId != ScreenID.HowToPlay && activeScreenId != ScreenID.Main && activeScreenId != ScreenID.Message && activeScreenId != ScreenID.Pause && activeScreenId != ScreenID.Purchase && activeScreenId != ScreenID.YourTurn)) && (action != Utils.Input.ActionMenu.MENU_BACK || activeScreenId != ScreenID.Main))
		{
			Audio.PlaySFX(Audio.SFXID.Menu);
			base.HandleInput(action);
		}
	}

	protected override void ChangeFocus(int id)
	{
		if (id == 12 && Trial.IsTrial)
		{
			id = 18;
		}
		if (id == 18)
		{
			if (Statics.input.PadIndexFound)
			{
				Menus.MenuEntryValue<TextureWithName> menuEntryValue = new Menus.MenuEntryValue<TextureWithName>(0, new TextureWithName(Textures.buttY, "   PURCHASE   ", TextureWithName.RelativePos.NameRightOfTexture), draw2D.Font, 0);
				menuEntryValue.OverrideStringColorBack(Color.Gold);
				Menus.MenuEntryValue<TextureWithName> menuEntryValue2 = new Menus.MenuEntryValue<TextureWithName>(0, new TextureWithName(Textures.buttB, "   QUIT       ", TextureWithName.RelativePos.NameRightOfTexture), draw2D.Font, 0);
				menuEntryValue2.OverrideStringColorBack(ColorText);
				endTrial.AddEntryValue(new Menus.MenuEntryMultipleChoice<List<Menus.MenuEntryValue<TextureWithName>>>(0, "", draw2D.Font, 0, Trial.UserCanPurchase(Statics.input.PlayerIndex) ? new List<Menus.MenuEntryValue<TextureWithName>> { menuEntryValue, menuEntryValue2 } : new List<Menus.MenuEntryValue<TextureWithName>> { menuEntryValue2 }, -2));
			}
			endTrialStartTime = gameTime.TotalGameTime.TotalSeconds;
		}
		base.ChangeFocus(id);
		if (id == 0 && Trial.IsTrialAndCanPurchase(Statics.input.PlayerIndex))
		{
			purchase.Enable();
		}
		if (id == 16)
		{
			musicBeforeScreenCredits = Audio.SongStatus;
			Audio.SongStatus = false;
		}
	}

	public GameMenus(ContentManager Content)
		: base(Style.Texture, Statics.draw2D, (int)(Statics.draw2D.ScreenSize.X / 120f), (int)(Statics.draw2D.ScreenSize.X / 40f), ColorOutline)
	{
		Textures.LoadContent(Content);
		styleTextureBG = Textures.MenuBG;
		OverlayColor = ColorOverlay;
		Menus.Screen.colorStringSelected = ColorText;
		Menus.Screen.colorStringNotSelected = ColorTextNotSelected;
		PauseMenu = new Menus.ScreenInfo(14, "", new Vector2(0.25f, 0.5f), 1f);
		PauseMenu.AddNonSelectableEntry("PAUSE", overrideSelectionTransition: true);
		for (int i = 0; i < 5; i++)
		{
			PauseMenu.AddSeparator();
		}
		PauseMenu.AddEntry("RESUME", 0);
		PauseMenu.AddEntry("MAIN MENU", 2);
		PauseMenu.Activated += Activate;
		PauseMenu.DefaultSelection = PauseMenu.entries.Count - 2;
		AddScreenInfo(PauseMenu);
		Menus.MenuScreen menuScreen = new Menus.MenuScreen(15, "", Statics.draw2D, Menus.AlignX.Center, useDefaultDeco: false);
		menuScreen.AddNonSelectableEntry("CREDITS", overrideSelectionTransition: true);
		Texture2D texture2D = new Texture2D(draw2D.Device, (int)((float)draw2D.ScreenSizePoint.X * 0.5f), 1);
		Color[] array = new Color[texture2D.Width];
		for (int j = 0; j < texture2D.Width; j++)
		{
			ref Color reference = ref array[j];
			reference = Color.Transparent;
		}
		texture2D.SetData(array);
		menuScreen.AddEntryValue(new Menus.MenuEntryValue<Texture2D>(1, texture2D, draw2D.Font, 0));
		Menus.MenuEntryValue<TextureWithName> menuEntryValue = new Menus.MenuEntryValue<TextureWithName>(menuScreen.entries.Count, new TextureWithName(Textures.MaximinusSmall, "DESIGN & EXECUTION  BY", TextureWithName.RelativePos.NameLeftOfTexture), draw2D.Font, 0);
		menuEntryValue.Selectable = false;
		menuEntryValue.OverrideStringColorBack(ColorText);
		menuScreen.AddEntryValue(menuEntryValue);
		AddCredits(menuScreen, "A ONE-MAN TEAM FROM GRENOBLE, FRANCE");
		AddCredits(menuScreen, "WEB     : WWW.MAXIMINUS.FR" + Utils.newLine + "EMAIL : GAMES@MAXIMINUS.FR");
		menuScreen.AddSeparator(1);
		AddCredits(menuScreen, " MUSIC : RIVER MEDITATION by" + Utils.newLine + "JASON SHAW @ AUDIONAUTIX.COM");
		AddScreen(menuScreen);
		TextureWithName.RelativePos relativePos = TextureWithName.RelativePos.NameBelowTexture;
		screenCred2 = new Menus.MenuScreen(16, "", Statics.draw2D, Menus.AlignX.Center, useDefaultDeco: false);
		AddScreen(screenCred2);
		screenCred2.AddNonSelectableEntry("OTHER GAMES BY MAXIMINUS", overrideSelectionTransition: true);
		Menus.MenuEntryValue<TextureWithName> menuEntryValue2 = new Menus.MenuEntryValue<TextureWithName>(0, new TextureWithName(Textures.GameGC, "GLOBE CLICKER", relativePos), draw2D.Font, 0, 5f / 6f, 1f);
		menuEntryValue2.OverrideStringColorFront(ColorText);
		menuEntryValue2.OverrideStringColorBack(ColorTextNotSelected);
		Menus.MenuEntryValue<TextureWithName> menuEntryValue3 = new Menus.MenuEntryValue<TextureWithName>(1, new TextureWithName(Textures.GameME, "MISSILE ESCAPE", relativePos), draw2D.Font, 1, 5f / 6f, 1f);
		menuEntryValue3.OverrideStringColorFront(ColorText);
		menuEntryValue3.OverrideStringColorBack(ColorTextNotSelected);
		Menus.MenuEntryValue<TextureWithName> menuEntryValue4 = new Menus.MenuEntryValue<TextureWithName>(2, new TextureWithName(Textures.GameASD, "AVATAR SLAM DUNK", relativePos), draw2D.Font, 2, 5f / 6f, 1f);
		menuEntryValue4.OverrideStringColorFront(ColorText);
		menuEntryValue4.OverrideStringColorBack(ColorTextNotSelected);
		gameChosen = OtherGame.GlobeClicker;
		Menus.MenuEntryMultipleChoice<List<Menus.MenuEntryValue<TextureWithName>>> e = new Menus.MenuEntryMultipleChoice<List<Menus.MenuEntryValue<TextureWithName>>>(menuScreen.entries.Count, "", draw2D.Font, 0, new List<Menus.MenuEntryValue<TextureWithName>> { menuEntryValue2, menuEntryValue3, menuEntryValue4 }, (int)gameChosen);
		screenCred2.AddEntryValue(e);
		screenCred2.DefaultSelection = screenCred2.entries.Count - 1;
		screenCred2.ChangedValue += ChangedValue;
		Menus.MenuEntryValue<string> menuEntryValue5 = new Menus.MenuEntryValue<string>(0, "", draw2D.Font, 0);
		menuEntryValue5.OverrideStringColorFront(ColorText);
		menuEntryValue5.OverrideStringColorBack(ColorTextNotSelected);
		menuEntryValue5.Selectable = false;
		screenCred2.AddSeparator(3);
		screenCred2.AddEntryValue(menuEntryValue5);
		UpdateGameChosen();
		int num = (int)(draw2D.ScreenSize.X * 0.4f);
		Rectangle rect = new Rectangle((int)(draw2D.ScreenSize.X * 0.5f), (int)(draw2D.ScreenSize.Y * 0.52f), num, num * 9 / 16);
		videoRect = new RoundedRectangle(rect);
		videoRect.TexWidth = draw2D.ScreenSizePoint.X / 80;
		videoRect.Color = ColorOutline;
		Menus.MenuScreen menuScreen2 = new Menus.MenuScreen(13, "", Statics.draw2D, Menus.AlignX.Center, useDefaultDeco: false, -1, draw2D.ScreenSizePoint.X / 80);
		menuScreen2.RatioXOverride = 0.25f;
		menuScreen2.AddEntryValue(new Menus.MenuEntryValue<Texture2D>(0, Textures.GameName, draw2D.Font, 0));
		menuScreen2.AddSeparator(2);
		menuScreen2.AddEntryValue(new Menus.MenuEntryValue<TextureWithName>(0, new TextureWithName(Textures.MenuStart, "    PRESS START        ", TextureWithName.RelativePos.NameLeftOfTexture), Statics.draw2D.Font, 0));
		menuScreen2.AddSeparator(2);
		AddScreen(menuScreen2);
		AddScreenInfo(new Menus.ScreenInfo(2, "", Menus.AlignX.Center, useDefaultDeco: false, 3.0, killAfterTimer: true, draw2D.ScreenSizePoint.X / 40, 1f));
		screensInfo[screensInfo.Count - 1].RatioXOverride = 0.5f;
		screensInfo[screensInfo.Count - 1].RatioYOverride = 0.8f;
		AddScreenInfo(3, Vector2.One * 0.5f, 3.0, killAfterTimer: true, 1f);
		AddScreenInfo(4, Vector2.One * 0.5f, 3.0, killAfterTimer: false, 1f);
		Menus.MenuScreen menuScreen3 = new Menus.MenuScreen(0, "", Statics.draw2D, Menus.AlignX.Left, useDefaultDeco: false);
		menuScreen3.AddEntryValue(new Menus.MenuEntryValue<TextureWithName>(menuScreen3.entries.Count, new TextureWithName(Textures.MenuSP, "SINGLE PLAYER", TextureWithName.RelativePos.NameRightOfTexture), draw2D.Font, 0));
		menuScreen3.AddEntryValue(new Menus.MenuEntryValue<TextureWithName>(menuScreen3.entries.Count, new TextureWithName(Textures.MenuMP, "MULTI PLAYER", TextureWithName.RelativePos.NameRightOfTexture), draw2D.Font, 1));
		menuScreen3.AddEntryValue(new Menus.MenuEntryValue<TextureWithName>(menuScreen3.entries.Count, new TextureWithName(Textures.MenuHTP, "HOW TO PLAY", TextureWithName.RelativePos.NameRightOfTexture), draw2D.Font, 3));
		if (MaximinusGame.Id == MaximinusGame.ID.Billard9Ball || MaximinusGame.Id == MaximinusGame.ID.FunkyPool)
		{
			menuScreen3.AddEntryValue(new Menus.MenuEntryValue<TextureWithName>(menuScreen3.entries.Count, new TextureWithName(Textures.MenuRules, "      RULES      ", TextureWithName.RelativePos.NameRightOfTexture), draw2D.Font, 2));
		}
		menuScreen3.AddEntryValue(new Menus.MenuEntryValue<TextureWithName>(menuScreen3.entries.Count, new TextureWithName(Textures.MenuAbout, "      CREDITS      ", TextureWithName.RelativePos.NameRightOfTexture), draw2D.Font, 4));
		menuScreen3.AddEntryValue(new Menus.MenuEntryValue<TextureWithName>(menuScreen3.entries.Count, new TextureWithName(Textures.MenuQuit, "          QUIT          ", TextureWithName.RelativePos.NameRightOfTexture), draw2D.Font, 5));
		AddScreen(menuScreen3);
		endTrial = new Menus.ScreenInfo(18, "", Vector2.One * 0.5f, 1f);
		endTrial.AddNonSelectableEntry("END OF TRIAL", overrideSelectionTransition: true);
		Menus.MenuEntryValue<TextureWithName> menuEntryValue6 = new Menus.MenuEntryValue<TextureWithName>(0, new TextureWithName(Textures.BoxArt, "PURCHASE FULL GAME" + Utils.newLine + "TO CONTINUE PLAYING" + Utils.newLine + Utils.newLine + "ONLY 80 MS POINTS / 1 $", TextureWithName.RelativePos.NameRightOfTexture), draw2D.Font, 0);
		menuEntryValue6.OverrideStringColorBack(ColorText);
		endTrial.AddEntryValue(menuEntryValue6);
		endTrial.DefaultSelection = -2;
		AddScreen(endTrial);
		purchase = new Menus.MenuScreen(17, "", draw2D, Menus.AlignX.Right, useDefaultDeco: false, -2, draw2D.ScreenSizePoint.X / 200);
		purchase.RatioXOverride = 0.8f;
		purchase.RatioYOverride = 0.8f;
		Menus.MenuEntryValue<TextureWithName> menuEntryValue7 = new Menus.MenuEntryValue<TextureWithName>(0, new TextureWithName(Textures.buttY, "   UNLOCK FULL GAME    ", TextureWithName.RelativePos.NameRightOfTexture), draw2D.Font, 0);
		menuEntryValue7.OverrideStringColorBack(ColorText);
		purchase.AddEntryValue(menuEntryValue7);
		purchase.DefaultSelection = -2;
		AddScreen(purchase);
		Menus.MenuScreen menuScreen4 = new Menus.MenuScreen(12, "", Statics.draw2D, Menus.AlignX.Left, useDefaultDeco: false);
		Menus.MenuEntryValue<string> menuEntryValue8 = new Menus.MenuEntryValue<string>(0, "REALLY QUIT ?", Statics.draw2D.Font, 0);
		menuEntryValue8.Selectable = false;
		menuEntryValue8.SelectionTransitionOverride(1f);
		menuScreen4.AddEntryValue(menuEntryValue8);
		for (int k = 0; k < 5; k++)
		{
			menuScreen4.AddSeparator();
		}
		menuScreen4.AddBooleanEntry("", 0, defaultValue: true, Menus.BoolEntryType.YesNo);
		menuScreen4.DefaultSelection = menuScreen4.entries.Count - 1;
		reallyQuit = true;
		AddScreen(menuScreen4);
		Menus.MenuScreen menuScreen5 = new Menus.MenuScreen(11, "", Statics.draw2D, Menus.AlignX.Center, useDefaultDeco: false);
		menuScreen5.AddNonSelectableEntry((MaximinusGame.Id == MaximinusGame.ID.FunkyPool) ? "27 BALL FUNKY POOL : RULES" : "9 BALL POOL : RULES", overrideSelectionTransition: true);
		menuScreen5.AddNonSelectableEntry("                                                                                                                                    ", overrideSelectionTransition: true);
		if (MaximinusGame.Id == MaximinusGame.ID.FunkyPool)
		{
			menuScreen5.AddNonSelectableEntry("SOLIDS                                            STRIPS", overrideSelectionTransition: true);
			menuScreen5.AddEntryValue(new Menus.MenuEntryValue<Texture2D>(0, Textures.MenuRulesFunky, draw2D.Font, 0));
			menuScreen5.entries[menuScreen5.entries.Count - 1].Selectable = false;
			menuScreen5.AddNonSelectableEntry(Utils.newLine + "STEP  1 - CHOOSE YOUR SIDE BY POCKETING YOUR FIRST BALL" + Utils.newLine + Utils.newLine + "STEP 2 - POCKET ALL YOUR BALLS : STRIPS OR SOLIDS" + Utils.newLine + Utils.newLine + "STEP 3 - ONCE YOU HAVE POCKETED ALL YOUR BALLS," + Utils.newLine + "                   POCKET THE BLACK BALL TO WIN THE GAME" + Utils.newLine, overrideSelectionTransition: true);
		}
		else
		{
			menuScreen5.AddNonSelectableEntry(Utils.newLine + "The object of the game is to pocket the 9 ball." + Utils.newLine + Utils.newLine + "On each shot the first ball the white ball contacts" + Utils.newLine + "must be the lowest-numbered ball on the table," + Utils.newLine + "but the balls need not be pocketed in order." + Utils.newLine + Utils.newLine + "If a player fouls by not contacting the lowest" + Utils.newLine + "numbered ball or pocketing the white ball," + Utils.newLine + "his opponents turn starts with ball in hand." + Utils.newLine + Utils.newLine + "If a player pockets any ball on a legal shot," + Utils.newLine + "he remains at the table for another shot." + Utils.newLine + Utils.newLine, overrideSelectionTransition: true);
		}
		AddScreen(menuScreen5);
		Menus.MenuScreen menuScreen6 = new Menus.MenuScreen(10, "", Statics.draw2D, Menus.AlignX.Center, useDefaultDeco: false);
		menuScreen6.AddNonSelectableEntry("      HOW TO PLAY", overrideSelectionTransition: true);
		menuScreen6.AddEntryValue(new Menus.MenuEntryValue<Texture2D>(0, Textures.howToPlay, Statics.draw2D.Font, 0));
		AddScreen(menuScreen6);
		popupControls = new Menus.ScreenInfo(19, "", Menus.AlignX.Center, useDefaultDeco: false, 5.0, killAfterTimer: true, -1, 1f);
		popupControls.RatioYOverride = 0.6f;
		popupControls.AddNonSelectableEntry("      HOW TO PLAY", overrideSelectionTransition: true);
		popupControls.AddEntryValue(new Menus.MenuEntryValue<Texture2D>(0, Textures.howToPlay, Statics.draw2D.Font, 0));
		AddScreenInfo(popupControls);
		Menus.MenuScreen menuScreen7 = new Menus.MenuScreen(6, "", Statics.draw2D, Menus.AlignX.Center, useDefaultDeco: false);
		Menus.MenuEntryValue<string> menuEntryValue9 = new Menus.MenuEntryValue<string>(0, "CPU OPPONENT LEVEL ?", Statics.draw2D.Font, 0);
		menuEntryValue9.Selectable = false;
		menuEntryValue9.SelectionTransitionOverride(1f);
		menuScreen7.AddEntryValue(menuEntryValue9);
		for (int l = 0; l < 5; l++)
		{
			menuScreen7.AddSeparator();
		}
		menuScreen7.AddMultipleChoiceEntry("", 0, new List<Menus.MenuEntryValue<string>>
		{
			new Menus.MenuEntryValue<string>(0, "EASY", Statics.draw2D.Font, 0),
			new Menus.MenuEntryValue<string>(1, "MEDIUM", Statics.draw2D.Font, 1),
			new Menus.MenuEntryValue<string>(2, "HARD", Statics.draw2D.Font, 0)
		}, 0);
		menuScreen7.DefaultSelection = menuScreen7.entries.Count - 1;
		AddScreen(menuScreen7);
		promptAddCpu = new Menus.MenuScreen(5, "", Statics.draw2D, Menus.AlignX.Center, useDefaultDeco: false);
		Menus.MenuEntryValue<string> menuEntryValue10 = new Menus.MenuEntryValue<string>(0, "   ADD CPU TEAMMATE   ", Statics.draw2D.Font, 0);
		menuEntryValue10.Selectable = false;
		menuEntryValue10.SelectionTransitionOverride(1f);
		promptAddCpu.AddEntryValue(menuEntryValue10);
		promptAddCpu.AddEntryValue(menuEntryValue10);
		for (int m = 0; m < 5; m++)
		{
			promptAddCpu.AddSeparator();
		}
		promptAddCpu.AddBooleanEntry("", 0, defaultValue: false, Menus.BoolEntryType.YesNo);
		promptAddCpu.DefaultSelection = promptAddCpu.entries.Count - 1;
		AddScreen(promptAddCpu);
	}

	private void UpdateGameChosen()
	{
		string text = "";
		int num = (int)((float)draw2D.ScreenSizePoint.X * 0.85f);
		int num2 = (int)((float)draw2D.ScreenSizePoint.Y * 0.66f);
		switch (gameChosen)
		{
		case OtherGame.GlobeClicker:
			text = "Test your geographical skills with Globe Clicker," + Utils.newLine + "a full 3D experience of Planet Earth! Rotate and" + Utils.newLine + "zoom to point at the given location. Points are" + Utils.newLine + "awarded for accuracy and speed. Try the challenge" + Utils.newLine + "mode to be the first in the scoreboards, or play" + Utils.newLine + "only with a restricted list : World Capitals," + Utils.newLine + "USA, Europe, World Heritage Sites, ...";
			break;
		case OtherGame.MissileEscape:
			text = "Missile Escape is simple : go flying, evade" + Utils.newLine + "many missiles and unlock new fighters" + Utils.newLine + "along the way ! " + Utils.newLine + "Warning : Fighter pilot spirit required." + Utils.newLine + Utils.newLine + Utils.newLine;
			break;
		case OtherGame.AvatarSlamDunk:
			text = "Slam Dunk fun for your avatars!" + Utils.newLine + "Featuring slam dunks and free" + Utils.newLine + "throw contests with local multiplayer" + Utils.newLine + "support" + Utils.newLine + Utils.newLine + Utils.newLine;
			break;
		}
		num -= (int)draw2D.Font.MeasureString(text).X;
		num2 -= (int)draw2D.Font.MeasureString(text).Y;
		Texture2D texture2D = new Texture2D(draw2D.Device, num, num2);
		Color[] array = new Color[num * num2];
		for (int i = 0; i < num * num2; i++)
		{
			ref Color reference = ref array[i];
			reference = Color.Transparent;
		}
		texture2D.SetData(array);
		Menus.MenuEntryValue<TextureWithName> menuEntryValue = new Menus.MenuEntryValue<TextureWithName>(0, new TextureWithName(texture2D, text, TextureWithName.RelativePos.NameLeftOfTexture), draw2D.Font, 0);
		menuEntryValue.OverrideStringColorBack(ColorText);
		menuEntryValue.Selectable = false;
		screenCred2.entries[screenCred2.entries.Count - 1] = menuEntryValue;
		screenCred2.UpdatePositions();
		if (screenCred2.state == Menus.Screen.State.Active)
		{
			PlayVideo();
		}
	}

	public void GameOver(GameTime gameTime, int winningTeamIndex)
	{
		string text = "       ";
		List<string> list = new List<string>();
		List<Color> list2 = new List<Color>();
		list.Add("GAME OVER");
		list2.Add(ColorText);
		string text2 = "";
		List<PlayerIndex> players = GameModeRules.AllTeams[winningTeamIndex].players;
		text2 = Utils.newLine + text + "WINNER" + ((players.Count > 1) ? "S" : "") + " : ";
		for (int i = 0; i < players.Count; i++)
		{
			text2 += GameModeRules.Team.NameOf(players[i]);
			if (i == 0 && players.Count > 1)
			{
				text2 += " & ";
			}
		}
		text2 += text;
		list.Add(text2);
		list2.Add(new Color(GameModeRules.Team.Colors[winningTeamIndex].ToVector3() * 1.52f));
		EnableScreenInfo(gameTime, 4, list, list2);
	}

	public void Message(string str)
	{
		List<string> list = str.Split(Utils.newLineChar).ToList();
		for (int i = 0; i < list.Count; i++)
		{
			list[i] = "       " + list[i] + "       ";
		}
		EnableScreenInfo(gameTime, 2, list, new List<Color> { Color.Red });
	}

	public void YourTurn(string message)
	{
		PlayerIndex currentPlayer = GameModeRules.CurrentPlayer;
		int num = (int)(currentPlayer + 1);
		string text = ((currentPlayer == GameModeRules.IndexCPU) ? "CPU TURN" : ("  PLAYER " + num.ToString("0" + Utils.newLine + "YOUR TURN")));
		if (message != "")
		{
			text = message + Utils.newLine + text;
		}
		EnableScreenInfo(gameTime, 3, text.Split(Utils.newLineChar).ToList());
	}

	public void PromptAddCpu(GameTime gameTime, List<PlayerIndex> teamA, List<PlayerIndex> teamB)
	{
		promptAddCpuTeamA = teamA;
		promptAddCpuTeamB = teamB;
		Menus.MenuEntry menuEntry = new Menus.MenuEntryValue<string>(0, "FOR PLAYER " + ((int)(((teamA.Count == 1) ? teamA[0] : teamB[0]) + 1)).ToString("0") + " ?", draw2D.Font, 0);
		menuEntry.Selectable = false;
		menuEntry.SelectionTransitionOverride(1f);
		promptAddCpu.ChangeEntry(1, menuEntry);
		GameState.Change(GameState.Type.MENUS, gameTime);
		wantedScreenId = 5;
		Enable();
	}

	public void PromptCpuLevel(GameTime gameTime, GameModeRules.Type gameType)
	{
		promptCputLevelGameType = gameType;
		GameState.Change(GameState.Type.MENUS, gameTime);
		wantedScreenId = 6;
		Enable();
	}

	private void Activate(object sender, Utils.EventArgsInteger e)
	{
		if (Paused)
		{
			Statics.callbacks.PauseOFF();
			switch ((ActionPause)e.value)
			{
			case ActionPause.MainMenu:
				Statics.callbacks.MainMenu(gameTime);
				break;
			case ActionPause.Controls:
				break;
			}
			return;
		}
		switch ((ScreenID)base.ActiveScreen.Id)
		{
		case ScreenID.Main:
			purchase.DisableIfNecessary();
			switch ((ActionMain)e.value)
			{
			case ActionMain.SinglePlayer:
				GameState.Change(GameState.Type.CHEAT_PROMPT, gameTime);
				break;
			case ActionMain.MultiPlayer:
				GameState.Change(GameState.Type.LOBBY, gameTime);
				break;
			case ActionMain.Rules:
				ChangeFocus(11);
				break;
			case ActionMain.HowToPlay:
				ChangeFocus(10);
				break;
			case ActionMain.About:
				ChangeFocus(15);
				break;
			case ActionMain.Quit:
				ChangeFocus(12);
				break;
			}
			break;
		case ScreenID.ReallyQuit:
			if (reallyQuit)
			{
				Statics.callbacks.Exit();
			}
			else
			{
				Back(this, new EventArgs());
			}
			break;
		case ScreenID.HowToPlay:
		case ScreenID.Rules:
		case ScreenID.Credits1:
		case ScreenID.Credits2:
			Back(this, new EventArgs());
			break;
		case ScreenID.PromptCpuLevel:
			Bot.SetLevel(promptCpuLevelResult);
			GameModeRules.InitializeFinal(gameTime, promptCputLevelGameType);
			break;
		case ScreenID.PromptAddCpu:
			if (promptAddCpuResult)
			{
				if (promptAddCpuTeamA.Count == 1)
				{
					promptAddCpuTeamA.Add(GameModeRules.IndexCPU);
				}
				else
				{
					promptAddCpuTeamB.Add(GameModeRules.IndexCPU);
				}
			}
			GameModeRules.InitializeMultiPlayer(gameTime, promptAddCpuTeamA, promptAddCpuTeamB, promptForAddCpu: false);
			break;
		}
	}

	private void ChangedValue(object Sender, Utils.EventArgs2Integers e)
	{
		switch ((ScreenID)base.ActiveScreen.Id)
		{
		case ScreenID.PromptCpuLevel:
			promptCpuLevelResult = (Bot.Level)e.value2;
			break;
		case ScreenID.PromptAddCpu:
			promptAddCpuResult = Utils.IntToBool(e.value2);
			break;
		case ScreenID.ReallyQuit:
			reallyQuit = Utils.IntToBool(e.value2);
			break;
		case ScreenID.Credits2:
			gameChosen = (OtherGame)e.value2;
			UpdateGameChosen();
			break;
		}
	}

	private void Back(object sender, EventArgs e)
	{
		switch ((ScreenID)base.ActiveScreen.Id)
		{
		case ScreenID.Lobby:
		case ScreenID.GameOver:
		case ScreenID.HowToPlay:
		case ScreenID.Rules:
		case ScreenID.ReallyQuit:
			ChangeFocus(0);
			break;
		case ScreenID.PromptCpuLevel:
			if (promptCputLevelGameType == GameModeRules.Type.SinglePlayer)
			{
				ChangeFocus(0);
			}
			else if (promptCputLevelGameType == GameModeRules.Type.MultiPlayer)
			{
				GameState.Change(GameState.Type.LOBBY, gameTime);
			}
			break;
		case ScreenID.PromptAddCpu:
			GameState.Change(GameState.Type.LOBBY, gameTime);
			break;
		case ScreenID.Credits1:
			ChangeFocus(16);
			break;
		case ScreenID.Credits2:
			vPlayer.Stop();
			Audio.SongStatus = musicBeforeScreenCredits;
			ChangeFocus(0);
			break;
		case ScreenID.Main:
		case ScreenID.Message:
		case ScreenID.YourTurn:
		case ScreenID.CheatPrompt:
		case ScreenID.CheatPromptSP:
		case ScreenID.CheatPromptMP:
		case ScreenID.FirstPrompt:
		case ScreenID.Pause:
			break;
		}
	}
}
