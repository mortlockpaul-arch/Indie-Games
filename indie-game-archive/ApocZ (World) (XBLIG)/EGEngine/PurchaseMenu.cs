using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EGEngine;

public class PurchaseMenu : Menu
{
	public static bool ExitGameOnBack = false;

	public new static Texture2D backgraoundTexture = null;

	private static Texture2D WorldMap;

	private static Texture2D[] ScreenShots = new Texture2D[4];

	private static float x = 0f;

	public PurchaseMenu(GameMenus id)
		: base(id)
	{
	}

	public override void LoadContent()
	{
		base.LoadContent();
		SetupMenuEntries();
		WorldMap = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\purchase\\WorldMap");
		ScreenShots[0] = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\purchase\\ss00");
		ScreenShots[1] = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\purchase\\ss01");
		ScreenShots[2] = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\purchase\\ss02");
		ScreenShots[3] = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\purchase\\ss03");
	}

	public override void Update(float eTime)
	{
		UpdateTransition(eTime);
		bgTextureColor.R = (byte)(255f * transitionDelta);
		bgTextureColor.G = (byte)(255f * transitionDelta);
		bgTextureColor.B = (byte)(255f * transitionDelta);
		bgTextureColor.A = (byte)(255f * transitionDelta);
		Menu.PlayMusic(BackgroundMusic.Menu);
	}

	public override void Draw()
	{
		if (LevelBaseMenu.LoadState != LevelLoadState.Loaded)
		{
			return;
		}
		Vector2 zero = Vector2.Zero;
		Rectangle a = default(Rectangle);
		EndGameEngine.GraphicMgr.GraphicsDevice.Viewport = EndGameEngine.DefualtViewport;
		bgTextureColor.R = (byte)(60f * transitionDelta);
		bgTextureColor.G = (byte)(60f * transitionDelta);
		bgTextureColor.B = (byte)(60f * transitionDelta);
		bgTextureColor.A = (byte)(60f * transitionDelta);
		Menu.spriteBatch.Begin();
		int height = (int)((float)EndGameEngine.DefualtViewport.TitleSafeArea.Width / (float)Menu.titleTexture.Width * (float)Menu.titleTexture.Height);
		Menu.spriteBatch.Draw(d: new Rectangle(EndGameEngine.DefualtViewport.TitleSafeArea.X, EndGameEngine.DefualtViewport.TitleSafeArea.Y, EndGameEngine.DefualtViewport.TitleSafeArea.Width, height), s: new Rectangle(4, 4, Menu.titleTexture.Width - 8, Menu.titleTexture.Height - 8), t: Menu.titleTexture, c: bgTextureColor);
		if (x % 1f < 0.9f)
		{
			x += 0.05f;
		}
		else
		{
			x += 0.001f;
		}
		if (x >= 5f)
		{
			x = 0f;
		}
		int num = (int)x;
		int num2 = (int)x + 1;
		num = ((num != 5) ? num : 0);
		num2 = ((num2 != 5) ? num2 : 0);
		float num3 = x - (float)num;
		bgTextureColor.R = (byte)(255f * num3 * transitionDelta);
		bgTextureColor.G = bgTextureColor.R;
		bgTextureColor.B = bgTextureColor.R;
		bgTextureColor.A = bgTextureColor.R;
		if (num2 < 4)
		{
			a.Width = 512;
			a.Height = 272;
			a.X = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.Width / 2 - a.Width / 2;
			a.Y = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.Height / 2 - a.Height / 2;
			Menu.spriteBatch.Draw(ScreenShots[num2], a, bgTextureColor);
		}
		else if (num2 < 5)
		{
			a.Width = 360;
			a.Height = 360;
			a.X = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.Width / 2 - a.Width / 2;
			a.Y = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.Height / 2 - a.Height / 2;
			Menu.spriteBatch.Draw(WorldMap, a, bgTextureColor);
		}
		bgTextureColor.R = (byte)(255f * (1f - num3) * transitionDelta);
		bgTextureColor.G = bgTextureColor.R;
		bgTextureColor.B = bgTextureColor.R;
		bgTextureColor.A = bgTextureColor.R;
		if (num < 4)
		{
			a.Width = 512;
			a.Height = 272;
			a.X = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.Width / 2 - a.Width / 2;
			a.Y = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.Height / 2 - a.Height / 2;
			Menu.spriteBatch.Draw(ScreenShots[num], a, bgTextureColor);
		}
		else if (num < 5)
		{
			a.Width = 360;
			a.Height = 360;
			a.X = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.Width / 2 - a.Width / 2;
			a.Y = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.Height / 2 - a.Height / 2;
			Menu.spriteBatch.Draw(WorldMap, a, bgTextureColor);
		}
		bgTextureColor.R = (byte)(255f * transitionDelta);
		bgTextureColor.G = (byte)(255f * transitionDelta);
		bgTextureColor.B = (byte)(255f * transitionDelta);
		bgTextureColor.A = (byte)(255f * transitionDelta);
		a.X = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.X + 32;
		a.Y = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Bottom - 106;
		a.Width = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Width / 20;
		a.Height = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Width / 20;
		int num4 = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Width / 20;
		for (int i = 1; i < 20; i++)
		{
			if (!EquipmentCls.EquipmentItemDesc[i].Contains("Empty"))
			{
				Menu.spriteBatch.Draw(InventoryCls.EquipmentTexture[i], a, bgTextureColor);
				a.X += num4;
			}
		}
		a.X = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.X + 64;
		a.Y = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Y + 8;
		for (int j = 1; j < 9; j++)
		{
			if (!ConsumableCls.ConsumableItemsDesc[j].Contains("Empty"))
			{
				Menu.spriteBatch.Draw(InventoryCls.ConsumableTexture[j], a, bgTextureColor);
				a.X += num4;
			}
		}
		int num5 = 0;
		a.X = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Right - 260;
		a.Y = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Y + 8;
		a.Width = 200;
		for (int k = 1; k < 44; k++)
		{
			if (InventoryCls.WeaponTexture[k] != null)
			{
				Menu.spriteBatch.Draw(InventoryCls.WeaponTexture[k], a, bgTextureColor);
				num5++;
				if (num5 >= 6)
				{
					a.X = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Right - 124;
					a.Width = 64;
					a.Y += 38;
				}
				else
				{
					a.Y += 52;
				}
			}
		}
		float g = 0.6f;
		zero.X = EndGameEngine.DefualtViewport.TitleSafeArea.Left;
		zero.Y = EndGameEngine.DefualtViewport.TitleSafeArea.Top + 145;
		Menu.spriteBatch.DrawString(Menu.defaultFont, "2000 Zombies", zero, Color.Gray, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
		zero.Y += 36f;
		Menu.spriteBatch.DrawString(Menu.defaultFont, "Over 700 3D Objects In World", zero, Color.Gray, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
		zero.Y += 36f;
		Menu.spriteBatch.DrawString(Menu.defaultFont, "3 Towns", zero, Color.Gray, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
		zero.Y += 36f;
		Menu.spriteBatch.DrawString(Menu.defaultFont, "7 Villages", zero, Color.Gray, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
		zero.Y += 36f;
		Menu.spriteBatch.DrawString(Menu.defaultFont, "2 Military Compounds", zero, Color.Gray, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
		zero.Y += 36f;
		Menu.spriteBatch.DrawString(Menu.defaultFont, "5 Vehicles", zero, Color.Gray, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
		zero.Y += 36f;
		Menu.spriteBatch.DrawString(Menu.defaultFont, "Over 100 Item And Weapon Spawns", zero, Color.Gray, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
		zero.Y += 36f;
		Menu.spriteBatch.DrawString(Menu.defaultFont, "16 Player Online", zero, Color.Gray, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
		zero.X = EndGameEngine.DefualtViewport.TitleSafeArea.Left + 96;
		zero.Y = EndGameEngine.DefualtViewport.TitleSafeArea.Bottom - 34;
		a.X = (int)zero.X;
		a.Y = (int)zero.Y;
		a.Width = 32;
		a.Height = 32;
		zero.X += 38f;
		zero.Y -= 2f;
		Menu.spriteBatch.Draw(Menu.aButton, a, buttonColor);
		Menu.spriteBatch.DrawString(Menu.defaultFont, "Unlock Full Game", zero, buttonColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
		zero.X += 300f;
		a.X += 300;
		Menu.spriteBatch.Draw(Menu.bButton, a, buttonColor);
		if (ExitGameOnBack)
		{
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Exit Game", zero, buttonColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
		}
		else
		{
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Back", zero, buttonColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
		}
		if (!Guide.IsTrialMode && base.State == MenuState.Active)
		{
			Manager.MakeActive(GameMenus.MainMenu);
		}
		Menu.spriteBatch.End();
		HandleInput();
	}

	public override void DrawBackground()
	{
	}

	public override void MakeActive(MenuMgr e)
	{
		base.MakeActive(e);
		base.BackMenuDelegate += ExitPurchaseFunc;
	}

	private void SetupMenuEntries()
	{
		Vector2 zero = Vector2.Zero;
		zero.X = (float)Menu.titleSafeArea.X + (float)Menu.titleSafeArea.Width * 0.2f;
		zero.Y = (float)Menu.titleSafeArea.Y + (float)Menu.titleSafeArea.Height * 0.25f;
		zero.X = 560f;
		zero.Y = 480f;
		MenuEntry menuEntry = new MenuEntry();
		new MenuEntry();
		menuEntryList.Add(menuEntry.Set(MenuEntryType.Text, (MenuEntryAttribute)5, "Purchase", zero, null, Color.DarkGray, "menus\\button01", "menus\\button02", "menus\\button03", PurchaseGameFunc, EndGameEngine.ContentMgr));
		menuEntry.isSelected = true;
		zero.Y += menuEntry.textHeight;
		zero.X += 30f;
	}

	private void PurchaseGameFunc(object sender, MenuEntry e)
	{
		SignedInGamer signedInGamer = Gamer.SignedInGamers[EndGameEngine.controllingPlayer.Value];
		if (signedInGamer == null || !signedInGamer.IsSignedInToLive)
		{
			if (!Guide.IsVisible)
			{
				Guide.ShowSignIn(1, onlineOnly: true);
			}
		}
		else if (signedInGamer != null)
		{
			if (!signedInGamer.Privileges.AllowPurchaseContent || signedInGamer.Privileges.AllowUserCreatedContent == GamerPrivilegeSetting.Blocked)
			{
				LevelBaseMenu.InputUpdate.menuInput = MenuInput.None;
				LevelBaseMenu.InputUpdate.menuInputContinuos = MenuInput.None;
				LevelBaseMenu.InputUpdate.menuInputRightStick = MenuInput.None;
				GamePad.GetState(EndGameEngine.controllingPlayer.Value);
				ErrorMessage.AddMessage("The current gamer profile does not have suitable privileges to perform this operation. You may require a LIVE Gold account, or need to change your parental control settings.", backToMain: true);
			}
			else if (!Guide.IsVisible)
			{
				Guide.ShowMarketplace(EndGameEngine.controllingPlayer.Value);
			}
		}
	}

	private void ExitPurchaseFunc(object sender, MenuEntry e)
	{
		if (ExitGameOnBack)
		{
			LevelBaseMenu.ThreadMenuRunning = false;
			LevelBaseMenu.UpdateThreadRunning = false;
			EndGameEngine.EGEGame.Exit();
		}
		else
		{
			Manager.MakeActive(GameMenus.MainMenu);
		}
	}
}
