using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class OtherGamesMenu(GameMenus id) : Menu(id)
{
	private const int NumGames = 6;

	private const int DisplayPositions = 10;

	private string[] GameDirectoryNames = new string[6] { "AliensVsRomans", "NW2030", "EODIvM", "EODSurvivor", "TheKeep", "KGB" };

	private string[] GameNames = new string[6] { "Aliens Vs Romans", "Nuclear Wasteland", "End of Days Infected vs Mercs", "End of Days Survivor", "The Keep: Zombie Horde", "KGB Episode One" };

	private Texture2D[][] GamesTextures = new Texture2D[6][];

	public static float BusyIconAngle = 0f;

	public static Texture2D BusyIcon;

	private Texture2D SKTitle;

	private Texture2D TitleButtons;

	private Texture2D ScreenShotButtons;

	private bool[] GameLoaded = new bool[6];

	private MyContentManager menuContent;

	private Color shadow = Color.Black;

	private Color diffuse = Color.Black;

	private int yRoll;

	private int titleMargin = 10;

	private int titleRecWidth;

	private int titleRecHeight;

	private int selectedIndex;

	private int screenshotIndex;

	private bool drawGameScreenShots;

	private static bool AssetsLoaded = false;

	private Rectangle tmpRec = default(Rectangle);

	private Rectangle titleRec = default(Rectangle);

	private Rectangle titleSelectedRec = default(Rectangle);

	private static int gameIndex = 0;

	private static int assestIndex = 0;

	public override void LoadContent()
	{
		base.LoadContent();
		BusyIcon = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\engine\\busyicon");
		defaultBackground = EndGameEngine.ContentMgr.Load<Texture2D>("OtherGames\\backgrounddrop");
		SKTitle = EndGameEngine.ContentMgr.Load<Texture2D>("OtherGames\\sickkreations");
		TitleButtons = EndGameEngine.ContentMgr.Load<Texture2D>("OtherGames\\titlebuttons");
		ScreenShotButtons = EndGameEngine.ContentMgr.Load<Texture2D>("OtherGames\\screenshotbuttons");
		for (int i = 0; i < 6; i++)
		{
			GamesTextures[i] = new Texture2D[5];
			GameLoaded[i] = false;
		}
		SetupOtherGamesMenu();
	}

	public override void Update(float eTime)
	{
		base.Update(eTime);
		_ = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value];
		if (base.State != MenuState.Active || !AssetsLoaded)
		{
			return;
		}
		if (!drawGameScreenShots)
		{
			int num = 32;
			if (LevelBaseMenu.InputUpdate.menuInputContinuos == MenuInput.MenuRight)
			{
				yRoll--;
			}
			else if (LevelBaseMenu.InputUpdate.menuInputContinuos == MenuInput.MenuLeft)
			{
				yRoll++;
			}
			if (yRoll > 0)
			{
				yRoll += num;
				if (yRoll > titleRecWidth)
				{
					yRoll = 0;
					int num2 = selectedIndex - 1;
					num2 = ((num2 < 0) ? 5 : num2);
					selectedIndex = num2;
					Menu.PlayQuickSelect();
				}
			}
			else if (yRoll < 0)
			{
				yRoll -= num;
				if (yRoll < -titleRecWidth)
				{
					yRoll = 0;
					int num3 = selectedIndex + 1;
					num3 = ((num3 < 6) ? num3 : 0);
					selectedIndex = num3;
					Menu.PlayQuickSelect();
				}
			}
			if (LevelBaseMenu.InputUpdate.menuInput == MenuInput.MenuSelect)
			{
				Menu.PlaySelect();
				drawGameScreenShots = true;
				screenshotIndex = 0;
			}
			if (LevelBaseMenu.InputUpdate.menuInput == MenuInput.MenuBack)
			{
				Manager.MakeActive(GameMenus.MainMenu);
				Thread thread = new Thread(UnLoadGameTexturesThread);
				thread.Start();
				Thread.Sleep(5);
			}
		}
		else
		{
			if (LevelBaseMenu.InputUpdate.menuInput == MenuInput.MenuSelect)
			{
				Menu.PlayQuickSelect();
				int num4 = screenshotIndex + 1;
				num4 = ((num4 <= 3) ? num4 : 0);
				screenshotIndex = num4;
			}
			if (LevelBaseMenu.InputUpdate.menuInput == MenuInput.MenuRight)
			{
				Menu.PlayQuickSelect();
				int num5 = screenshotIndex + 1;
				num5 = ((num5 <= 3) ? num5 : 0);
				screenshotIndex = num5;
			}
			else if (LevelBaseMenu.InputUpdate.menuInput == MenuInput.MenuLeft)
			{
				Menu.PlayQuickSelect();
				int num6 = screenshotIndex - 1;
				num6 = ((num6 < 0) ? 3 : num6);
				screenshotIndex = num6;
			}
			if (LevelBaseMenu.InputUpdate.menuInput == MenuInput.MenuBack)
			{
				Menu.PlayQuickSelect();
				drawGameScreenShots = false;
			}
		}
	}

	public override void Draw()
	{
		if (gameIndex < 6)
		{
			if (assestIndex == 0)
			{
				GamesTextures[gameIndex][0] = menuContent.Load<Texture2D>("OtherGames\\" + GameDirectoryNames[gameIndex] + "\\title");
			}
			else if (assestIndex == 1)
			{
				GamesTextures[gameIndex][1] = menuContent.Load<Texture2D>("OtherGames\\" + GameDirectoryNames[gameIndex] + "\\ss0");
			}
			else if (assestIndex == 2)
			{
				GamesTextures[gameIndex][2] = menuContent.Load<Texture2D>("OtherGames\\" + GameDirectoryNames[gameIndex] + "\\ss1");
			}
			else if (assestIndex == 3)
			{
				GamesTextures[gameIndex][3] = menuContent.Load<Texture2D>("OtherGames\\" + GameDirectoryNames[gameIndex] + "\\ss2");
			}
			else if (assestIndex == 4)
			{
				GamesTextures[gameIndex][4] = menuContent.Load<Texture2D>("OtherGames\\" + GameDirectoryNames[gameIndex] + "\\ss3");
			}
			assestIndex++;
			if (assestIndex > 4)
			{
				GameLoaded[gameIndex] = true;
				assestIndex = 0;
				gameIndex++;
			}
		}
		else
		{
			AssetsLoaded = true;
		}
		Viewport viewport = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport;
		Color white = Color.White;
		white.R = transitionAlpha;
		white.G = transitionAlpha;
		white.B = transitionAlpha;
		white.A = transitionAlpha;
		Menu.spriteBatch.Begin();
		Menu.spriteBatch.Draw(defaultBackground, viewport.Bounds, white);
		int num = 32;
		titleRec.Width = titleRecWidth;
		titleRec.Height = titleRecHeight;
		titleRec.X = viewport.TitleSafeArea.X - (titleRec.Width + titleMargin) * 2 + yRoll;
		titleRec.Y = viewport.TitleSafeArea.Y + (viewport.TitleSafeArea.Height - titleRecHeight) / 2 + num;
		titleSelectedRec.Width = titleRecWidth + 30;
		titleSelectedRec.Height = (int)((float)titleSelectedRec.Width * 1.6111f);
		titleSelectedRec.Y = viewport.TitleSafeArea.Y + (viewport.TitleSafeArea.Height - titleSelectedRec.Height) / 2 + num;
		int num2 = selectedIndex - 2;
		if (num2 < 0)
		{
			num2 = 6 - Math.Abs(num2);
		}
		if (!drawGameScreenShots)
		{
			for (int i = 0; i < 10; i++)
			{
				if (GameLoaded[num2])
				{
					if (num2 == selectedIndex)
					{
						if (yRoll == 0)
						{
							titleSelectedRec.X = titleRec.X;
							Menu.spriteBatch.Draw(GamesTextures[num2][0], titleSelectedRec, white);
						}
						else
						{
							Menu.spriteBatch.Draw(GamesTextures[num2][0], titleRec, white);
						}
						titleRec.X += 30;
					}
					else
					{
						Menu.spriteBatch.Draw(GamesTextures[num2][0], titleRec, white);
					}
				}
				titleRec.X += titleRec.Width + titleMargin;
				num2++;
				if (num2 >= 6)
				{
					num2 = 0;
				}
			}
			tmpRec.Width = TitleButtons.Width;
			tmpRec.Height = TitleButtons.Height;
			tmpRec.X = viewport.TitleSafeArea.Left;
			tmpRec.Y = viewport.TitleSafeArea.Bottom - tmpRec.Height;
			Menu.spriteBatch.Draw(TitleButtons, tmpRec, white);
		}
		else if (GamesTextures[selectedIndex][screenshotIndex + 1] != null)
		{
			tmpRec = viewport.TitleSafeArea;
			tmpRec.Width = (int)((float)tmpRec.Width * 0.75f);
			tmpRec.Height = (int)((float)tmpRec.Height * 0.75f);
			tmpRec.X += (viewport.TitleSafeArea.Width - tmpRec.Width) / 2;
			tmpRec.Y += (viewport.TitleSafeArea.Height - tmpRec.Height) / 2;
			Menu.spriteBatch.Draw(GamesTextures[selectedIndex][screenshotIndex + 1], tmpRec, white);
			tmpRec.Width = ScreenShotButtons.Width;
			tmpRec.Height = ScreenShotButtons.Height;
			tmpRec.X = viewport.TitleSafeArea.Left;
			tmpRec.Y = viewport.TitleSafeArea.Bottom - tmpRec.Height;
			Menu.spriteBatch.Draw(ScreenShotButtons, tmpRec, white);
		}
		if (!AssetsLoaded)
		{
			Vector2 e = new Vector2(50f, 50f);
			Rectangle empty = Rectangle.Empty;
			empty.X = viewport.TitleSafeArea.Center.X;
			empty.Y = viewport.TitleSafeArea.Center.Y;
			empty.Width = 60;
			empty.Height = 60;
			Rectangle empty2 = Rectangle.Empty;
			empty2.Width = 100;
			empty2.Height = 100;
			BusyIconAngle += 0.2f;
			Menu.spriteBatch.Draw(BusyIcon, empty, empty2, Color.White, BusyIconAngle, e, SpriteEffects.None, 0);
		}
		tmpRec.Width = SKTitle.Width;
		tmpRec.Height = SKTitle.Height;
		tmpRec.X = viewport.TitleSafeArea.Right - tmpRec.Width;
		tmpRec.Y = viewport.TitleSafeArea.Top;
		Menu.spriteBatch.Draw(SKTitle, tmpRec, white);
		Vector2 zero = Vector2.Zero;
		zero.X = viewport.TitleSafeArea.X;
		zero.Y = viewport.TitleSafeArea.Y;
		Menu.spriteBatch.DrawString(Menu.systemFont, GameNames[selectedIndex], zero, white);
		Menu.spriteBatch.End();
	}

	private void DrawOtherGamesMenu()
	{
	}

	public override void DrawBackground()
	{
	}

	public override void MakeActive(MenuMgr e)
	{
		base.MakeActive(e);
		yRoll = 0;
		drawGameScreenShots = false;
		selectedIndex = 0;
		screenshotIndex = 0;
		titleRecWidth = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Width / 5 - titleMargin;
		titleRecHeight = (int)((float)titleRecWidth * 1.6111f);
		AssetsLoaded = false;
		for (int i = 0; i < 6; i++)
		{
			GameLoaded[i] = false;
		}
		menuContent = new MyContentManager(EndGameEngine.ContentMgr.ServiceProvider);
		menuContent.RootDirectory = "EngineContent";
		LoadGameTexturesThread();
	}

	private void LoadGameTexturesThread()
	{
		gameIndex = 0;
		assestIndex = 0;
	}

	private void UnLoadGameTexturesThread()
	{
		while (base.State != MenuState.Hidden)
		{
		}
		Thread.Sleep(10);
		menuContent.Unload();
	}

	private void SetupOtherGamesMenu()
	{
	}

	private void AimnAssistFunc(object sender, MenuEntry e)
	{
		LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].AimAssist = !LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].AimAssist;
	}

	private void InvertYFunc(object sender, MenuEntry e)
	{
		LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].InvertY = ((LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].InvertY > 0f) ? (-1f) : 1f);
	}

	private void SensitivityFunc(object sender, MenuEntry e)
	{
	}
}
