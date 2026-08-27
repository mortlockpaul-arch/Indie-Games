using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EGEngine;

public class SetCharacterMenu(GameMenus id) : Menu(id)
{
	private const int NumGames = 6;

	private const int DisplayPositions = 10;

	private Texture2D[][] GamesTextures = new Texture2D[6][];

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

	private int VerticalSelection;

	private Matrix tmpMatrix = Matrix.Identity;

	private float BusyIconAngle;

	private float menuPlayerYaw;

	private Rectangle tmpRec = default(Rectangle);

	private Rectangle titleRec = default(Rectangle);

	private Rectangle titleSelectedRec = default(Rectangle);

	public override void LoadContent()
	{
		base.LoadContent();
	}

	public override void Update(float eTime)
	{
		PlayerBase playerBase = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value];
		UpdateTransition(eTime);
		for (int i = 0; i < menuEntryList.Count; i++)
		{
			menuEntryList[i].Update(eTime, transitionDelta);
		}
		if (base.State == MenuState.Active)
		{
			float num = playerBase.ShirtIndex;
			float num2 = playerBase.PantstIndex;
			if (LevelBaseMenu.InputUpdate.menuInput == MenuInput.MenuBack)
			{
				Storage.SavePlayerInfo();
				Manager.MakeActive(GameMenus.MainMenu);
			}
			if (LevelBaseMenu.InputUpdate.menuInput == MenuInput.MenuUp)
			{
				if (VerticalSelection > 0)
				{
					Menu.PlaySelect();
				}
				int num3 = VerticalSelection - 1;
				num3 = ((num3 > 0) ? num3 : 0);
				VerticalSelection = num3;
			}
			else if (LevelBaseMenu.InputUpdate.menuInput == MenuInput.MenuDown)
			{
				if (VerticalSelection < 3)
				{
					Menu.PlaySelect();
				}
				int num4 = VerticalSelection + 1;
				num4 = ((num4 < 3) ? num4 : 2);
				VerticalSelection = num4;
			}
			if (LevelBaseMenu.InputUpdate.menuInput == MenuInput.MenuLeft)
			{
				Menu.PlayQuickSelect();
				if (VerticalSelection == 0)
				{
					if (playerBase.CharacterIndex == 1)
					{
						playerBase.SetCharacter(0, num, num2);
					}
					else
					{
						playerBase.SetCharacter(1, num, num2);
					}
				}
				else if (VerticalSelection == 1)
				{
					num = ((num > 0f) ? (num - 1f) : 5f);
				}
				else if (VerticalSelection == 2)
				{
					num2 = ((num2 > 0f) ? (num2 - 1f) : 5f);
				}
			}
			else if (LevelBaseMenu.InputUpdate.menuInput == MenuInput.MenuRight)
			{
				Menu.PlayQuickSelect();
				if (VerticalSelection == 0)
				{
					if (playerBase.CharacterIndex == 0)
					{
						playerBase.SetCharacter(1, num, num2);
					}
					else
					{
						playerBase.SetCharacter(0, num, num2);
					}
				}
				else if (VerticalSelection == 1)
				{
					num = ((num + 1f < 6f) ? (num + 1f) : 0f);
				}
				else if (VerticalSelection == 2)
				{
					num2 = ((num2 + 1f < 6f) ? (num2 + 1f) : 0f);
				}
			}
			float num5 = menuPlayerYaw;
			num5 = ((LevelBaseMenu.InputUpdate.menuInputRightStick == MenuInput.MenuLeft) ? (num5 - 0.02f) : num5);
			num5 = ((LevelBaseMenu.InputUpdate.menuInputRightStick == MenuInput.MenuRight) ? (num5 + 0.02f) : num5);
			menuPlayerYaw = ((num5 < -0.6f) ? (-0.6f) : ((num5 > 0.6f) ? 0.6f : num5));
			playerBase.ShirtIndex = num;
			playerBase.PantstIndex = num2;
		}
		tmpMatrix = Matrix.CreateRotationY(menuPlayerYaw);
		tmpMatrix *= PlayerBase.tmpPlayerScale;
		tmpMatrix.Translation = PlayerBase.CoOpOffset;
		tmpMatrix.Translation += new Vector3(0f, 15f, 0f);
		playerBase.Set3rdPersonHandPosition();
		playerBase.cPlayer.Update(EndGameEngine.currentEleapsedTime.ElapsedGameTime, ref tmpMatrix, 0, 0f);
	}

	public override void Draw()
	{
		float num = 1.5f;
		string text = "Set Character";
		EndGameEngine.GraphicMgr.GraphicsDevice.SetRenderTarget(LevelBaseMenu.DiffuseRenderTarget);
		EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = BlendState.Opaque;
		EndGameEngine.GraphicMgr.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
		EndGameEngine.GraphicMgr.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
		EndGameEngine.GraphicMgr.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);
		LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].DrawMenuPlayer(menuPlayerYaw);
		Vector2 zero = Vector2.Zero;
		zero.X = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Center.X;
		zero.Y = 0f;
		Menu.spriteBatch.Begin();
		bgTextureColor.R = (byte)(255f * transitionDelta);
		bgTextureColor.G = (byte)(255f * transitionDelta);
		bgTextureColor.B = (byte)(255f * transitionDelta);
		bgTextureColor.A = (byte)(255f * transitionDelta);
		zero.X -= Menu.defaultFont.MeasureString(text).X * 0.5f * num;
		Menu.spriteBatch.DrawString(Menu.defaultFont, text, zero, Color.LightGray, 0f, Vector2.Zero, num, SpriteEffects.None, 0);
		int num2 = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Height / 4;
		zero.X = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.X;
		zero.Y = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Y + num2;
		if (VerticalSelection == 0)
		{
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Character", zero, Color.LightGray, 0f, Vector2.Zero, 1.7f, SpriteEffects.None, 0);
			zero.Y += num2;
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Shirt", zero, Color.Gray, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
			zero.Y += num2;
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Pants", zero, Color.Gray, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
		}
		else if (VerticalSelection == 1)
		{
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Character", zero, Color.Gray, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
			zero.Y += num2;
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Shirt", zero, Color.LightGray, 0f, Vector2.Zero, 1.7f, SpriteEffects.None, 0);
			zero.Y += num2;
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Pants", zero, Color.Gray, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
		}
		else if (VerticalSelection == 2)
		{
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Character", zero, Color.Gray, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
			zero.Y += num2;
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Shirt", zero, Color.Gray, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
			zero.Y += num2;
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Pants", zero, Color.LightGray, 0f, Vector2.Zero, 1.7f, SpriteEffects.None, 0);
		}
		tmpRec.Width = 38;
		tmpRec.Height = 38;
		tmpRec.X = 16;
		tmpRec.Y = LevelBaseMenu.DiffuseRenderTarget.Height - 48;
		Menu.DrawButton(tmpRec, Buttons.B, Color.White);
		tmpRec.X = LevelBaseMenu.DiffuseRenderTarget.Width - 256;
		Menu.DrawButton(tmpRec, null, Color.White);
		zero.X = 64f;
		zero.Y = tmpRec.Y + 4;
		Menu.spriteBatch.DrawString(Menu.defaultFont, "Back", zero, Color.LightGray, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0);
		zero.X = LevelBaseMenu.DiffuseRenderTarget.Width - 204;
		Menu.spriteBatch.DrawString(Menu.defaultFont, "Navigate", zero, Color.LightGray, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0);
		Menu.spriteBatch.End();
		base.Draw();
		EndGameEngine.GraphicMgr.GraphicsDevice.SetRenderTarget(null);
		EndGameEngine.GraphicMgr.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);
		Menu.spriteBatch.Begin();
		Menu.spriteBatch.Draw(LevelBaseMenu.DiffuseRenderTarget, EndGameEngine.DefualtViewport.TitleSafeArea, LevelBaseMenu.DiffuseRenderTarget.Bounds, bgTextureColor);
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
		Storage.LoadPlayerInfo();
		EndGameEngine.UpdatePresence(GamerPresenceMode.CustomizingPlayer);
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
