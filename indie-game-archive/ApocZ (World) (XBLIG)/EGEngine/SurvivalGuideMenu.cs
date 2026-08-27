using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EGEngine;

public class SurvivalGuideMenu(GameMenus id) : Menu(id)
{
	private const int NumGames = 6;

	private const int DisplayPositions = 10;

	public static Texture2D BusyIcon;

	private Texture2D SKTitle;

	private Texture2D TitleButtons;

	private Texture2D ScreenShotButtons;

	private int currentPage;

	private int NumPages = 4;

	private Texture2D MenuTexture;

	private Texture2D JeepIcon;

	private Texture2D ZombieIcon;

	private Texture2D WallIcon;

	public static Texture2D ControllerTex;

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
		MenuTexture = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\SurvivalMenu");
		JeepIcon = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\JeepGuideIcon");
		ZombieIcon = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\ZombieIcon");
		WallIcon = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\MilFacility");
		ControllerTex = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\ControllerTex");
		SetupOtherGamesMenu();
	}

	public override void Update(float eTime)
	{
		if (base.IsActive)
		{
			if (InputBase.playerGamePad[(int)EndGameEngine.controllingPlayer.Value].currentGamePadState.IsButtonDown(Buttons.LeftShoulder) && InputBase.playerGamePad[(int)EndGameEngine.controllingPlayer.Value].lastGamePadState.IsButtonUp(Buttons.LeftShoulder))
			{
				if (currentPage > 0)
				{
					currentPage--;
					Menu.PlayQuickSelect();
				}
			}
			else if (InputBase.playerGamePad[(int)EndGameEngine.controllingPlayer.Value].currentGamePadState.IsButtonDown(Buttons.RightShoulder) && InputBase.playerGamePad[(int)EndGameEngine.controllingPlayer.Value].lastGamePadState.IsButtonUp(Buttons.RightShoulder) && currentPage < NumPages - 1)
			{
				currentPage++;
				Menu.PlayQuickSelect();
			}
		}
		base.Update(eTime);
	}

	public override void Draw()
	{
		Rectangle rec = default(Rectangle);
		Viewport viewport = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport;
		_ = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value];
		Menu.spriteBatch.Begin();
		bgTextureColor.R = (byte)(200f * transitionDelta);
		bgTextureColor.G = (byte)(200f * transitionDelta);
		bgTextureColor.B = (byte)(200f * transitionDelta);
		bgTextureColor.A = (byte)(200f * transitionDelta);
		Menu.spriteBatch.Draw(MenuTexture, EndGameEngine.DefualtViewport.TitleSafeArea, bgTextureColor);
		rec.X = viewport.TitleSafeArea.X + 24;
		rec.Y = viewport.TitleSafeArea.Y + 12;
		rec.Width = 64;
		rec.Height = 40;
		if (currentPage > 0)
		{
			Menu.DrawButton(rec, Buttons.LeftShoulder, Color.White);
		}
		rec.X = viewport.TitleSafeArea.Right - 88;
		if (currentPage < NumPages - 1)
		{
			Menu.DrawButton(rec, Buttons.RightShoulder, Color.White);
		}
		DrawButtonControl(EndGameEngine.GraphicMgr.GraphicsDevice.Viewport, drawSelect: false, drawBack: true, drawReady: false);
		Vector2 zero = Vector2.Zero;
		zero.X = (float)viewport.TitleSafeArea.Center.X - Menu.defaultFont.MeasureString("Survival Guide").X * 0.5f * 1.25f;
		zero.Y = viewport.TitleSafeArea.Y + 16;
		Menu.spriteBatch.DrawString(Menu.defaultFont, "Survival Guide", zero, Color.Black, 0f, Vector2.Zero, 1.25f, SpriteEffects.None, 0);
		if (currentPage == 0)
		{
			DrawPageOne();
		}
		else if (currentPage == 1)
		{
			DrawPageTwo();
		}
		else if (currentPage == 2)
		{
			DrawPageThree();
		}
		else if (currentPage == 3)
		{
			DrawPageFour();
		}
		Menu.spriteBatch.End();
	}

	private void DrawPageOne()
	{
		Rectangle a = new Rectangle
		{
			Width = 78,
			Height = 78
		};
		Color black = Color.Black;
		black.R = 32;
		black.G = 32;
		black.B = 32;
		Viewport viewport = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport;
		Vector2 zero = Vector2.Zero;
		int num = viewport.TitleSafeArea.Y + 64;
		int num2 = 70;
		int num3 = 24;
		string b = "This is your health, if your blood falls below 30 it will effect run endurance.";
		string b2 = "If your blood falls to zero you will die.";
		zero.X = viewport.TitleSafeArea.X + 32;
		zero.Y = num;
		Menu.spriteBatch.DrawString(Menu.defaultFont, "BLOOD", zero, Color.DarkRed);
		zero.X = viewport.TitleSafeArea.X + 128;
		zero.Y += 4f;
		Menu.spriteBatch.DrawString(Menu.defaultFont, b, zero, black, 0f, Vector2.Zero, 0.9f, SpriteEffects.None, 0);
		zero.Y += num3;
		Menu.spriteBatch.DrawString(Menu.defaultFont, b2, zero, black, 0f, Vector2.Zero, 0.9f, SpriteEffects.None, 0);
		string b3 = "Hydration, if your water falls below 30 it will effect your run endurance.";
		string b4 = "If your water falls to zero it will effect your blood.";
		zero.X = viewport.TitleSafeArea.X + 32;
		zero.Y = num + num2;
		Menu.spriteBatch.DrawString(Menu.defaultFont, "Water", zero, Color.DarkBlue);
		zero.X = viewport.TitleSafeArea.X + 128;
		zero.Y += 4f;
		Menu.spriteBatch.DrawString(Menu.defaultFont, b3, zero, black, 0f, Vector2.Zero, 0.9f, SpriteEffects.None, 0);
		zero.Y += num3;
		Menu.spriteBatch.DrawString(Menu.defaultFont, b4, zero, black, 0f, Vector2.Zero, 0.9f, SpriteEffects.None, 0);
		string b5 = "Food, if your food falls to zero it will effect your blood.";
		string b6 = "Eating food regenerates your blood.";
		zero.X = viewport.TitleSafeArea.X + 32;
		zero.Y = num + num2 * 2;
		Menu.spriteBatch.DrawString(Menu.defaultFont, "Food", zero, Color.DarkGreen);
		zero.X = viewport.TitleSafeArea.X + 128;
		zero.Y += 4f;
		Menu.spriteBatch.DrawString(Menu.defaultFont, b5, zero, black, 0f, Vector2.Zero, 0.9f, SpriteEffects.None, 0);
		zero.Y += num3;
		Menu.spriteBatch.DrawString(Menu.defaultFont, b6, zero, black, 0f, Vector2.Zero, 0.9f, SpriteEffects.None, 0);
		string b7 = "Bandages can be used to stop bleeding.";
		a.X = viewport.TitleSafeArea.X + 32;
		a.Y = num + num2 * 3;
		Menu.spriteBatch.Draw(InventoryCls.ConsumableTexture[7], a, Color.White);
		zero.X = viewport.TitleSafeArea.X + 128;
		zero.Y = num + num2 * 3 + 12;
		Menu.spriteBatch.DrawString(Menu.defaultFont, b7, zero, black, 0f, Vector2.Zero, 0.9f, SpriteEffects.None, 0);
		string b8 = "Pain pills increase endurance when your blood or water is below 30.";
		a.X = viewport.TitleSafeArea.X + 32;
		a.Y = num + num2 * 4;
		Menu.spriteBatch.Draw(InventoryCls.ConsumableTexture[8], a, Color.White);
		zero.X = viewport.TitleSafeArea.X + 128;
		zero.Y = num + num2 * 4 + 12;
		Menu.spriteBatch.DrawString(Menu.defaultFont, b8, zero, black, 0f, Vector2.Zero, 0.9f, SpriteEffects.None, 0);
		string b9 = "Food and water can be found around buildings in towns and villages.";
		a.X = viewport.TitleSafeArea.X + 28;
		a.Y = num + num2 * 5;
		a.Width = 72;
		a.Height = 72;
		Menu.spriteBatch.Draw(InventoryCls.ConsumableTexture[2], a, Color.White);
		a.X = viewport.TitleSafeArea.X + 62;
		a.Y = num + num2 * 5 + 28;
		a.Width = 72;
		a.Height = 72;
		Menu.spriteBatch.Draw(InventoryCls.ConsumableTexture[4], a, Color.White);
		zero.X = viewport.TitleSafeArea.X + 128;
		zero.Y = num + num2 * 5 + 24;
		Menu.spriteBatch.DrawString(Menu.defaultFont, b9, zero, black, 0f, Vector2.Zero, 0.9f, SpriteEffects.None, 0);
	}

	private void DrawPageTwo()
	{
		Rectangle a = new Rectangle
		{
			Width = 78,
			Height = 78
		};
		Color black = Color.Black;
		black.R = 32;
		black.G = 32;
		black.B = 32;
		Viewport viewport = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport;
		Vector2 zero = Vector2.Zero;
		int num = viewport.TitleSafeArea.Y + 64;
		int num2 = 68;
		int num3 = 22;
		string b = "Vehicles can take damage and be repaired.";
		a.X = viewport.TitleSafeArea.X + 24;
		a.Y = num - 16;
		a.Width = 108;
		a.Height = 108;
		Menu.spriteBatch.Draw(JeepIcon, a, Color.White);
		zero.X = viewport.TitleSafeArea.X + 132;
		zero.Y = num + 8;
		Menu.spriteBatch.DrawString(Menu.defaultFont, b, zero, black, 0f, Vector2.Zero, 0.9f, SpriteEffects.None, 0);
		string b2 = "Vehicles consume petroleum and need to be refueled.";
		a.X = viewport.TitleSafeArea.X + 32;
		a.Y = num + num2;
		a.Width = 72;
		a.Height = 72;
		Menu.spriteBatch.Draw(InventoryCls.EquipmentTexture[3], a, Color.White);
		zero.X = viewport.TitleSafeArea.X + 128;
		zero.Y = num + num2 + 18;
		Menu.spriteBatch.DrawString(Menu.defaultFont, b2, zero, black, 0f, Vector2.Zero, 0.9f, SpriteEffects.None, 0);
		string b3 = "Vehicle tires can be repaired.";
		a.X = viewport.TitleSafeArea.X + 32;
		a.Y = num + num2 * 2;
		a.Width = 72;
		a.Height = 72;
		Menu.spriteBatch.Draw(InventoryCls.EquipmentTexture[10], a, Color.White);
		zero.X = viewport.TitleSafeArea.X + 128;
		zero.Y = num + num2 * 2 + 18;
		Menu.spriteBatch.DrawString(Menu.defaultFont, b3, zero, black, 0f, Vector2.Zero, 0.9f, SpriteEffects.None, 0);
		string b4 = "Tool box is needed to repair vehicles.";
		a.X = viewport.TitleSafeArea.X + 32;
		a.Y = num + num2 * 3;
		a.Width = 72;
		a.Height = 72;
		Menu.spriteBatch.Draw(InventoryCls.EquipmentTexture[8], a, Color.White);
		zero.X = viewport.TitleSafeArea.X + 128;
		zero.Y = num + num2 * 3 + 18;
		Menu.spriteBatch.DrawString(Menu.defaultFont, b4, zero, black, 0f, Vector2.Zero, 0.9f, SpriteEffects.None, 0);
		string b5 = "Backpacks can increase inventory capacity. There are 3 backpack sizes.";
		a.X = viewport.TitleSafeArea.X + 32;
		a.Y = num + num2 * 4;
		a.Width = 72;
		a.Height = 72;
		Menu.spriteBatch.Draw(InventoryCls.EquipmentTexture[5], a, Color.White);
		zero.X = viewport.TitleSafeArea.X + 128;
		zero.Y = num + num2 * 4 + 12;
		Menu.spriteBatch.DrawString(Menu.defaultFont, b5, zero, black, 0f, Vector2.Zero, 0.9f, SpriteEffects.None, 0);
		string b6 = "Tents can be used to save items. When you create or join a server your tents";
		string b7 = "will spawn in with you, when you leave a session your tents despawn with you.";
		string b8 = "If another player finds your tent and picks it up it will be deleted from your";
		string b9 = "inventory and the items it contained will be droppped in the world.";
		a.X = viewport.TitleSafeArea.X + 24;
		a.Y = num + num2 * 5;
		a.Width = 72;
		a.Height = 72;
		Menu.spriteBatch.Draw(InventoryCls.EquipmentTexture[11], a, Color.White);
		a.X = viewport.TitleSafeArea.X + 38;
		a.Y = num + num2 * 5 + 50;
		a.Width = 72;
		a.Height = 72;
		Menu.spriteBatch.Draw(InventoryCls.EquipmentTexture[1], a, Color.White);
		zero.X = viewport.TitleSafeArea.X + 128;
		zero.Y = num + num2 * 5 + 12;
		Menu.spriteBatch.DrawString(Menu.defaultFont, b6, zero, black, 0f, Vector2.Zero, 0.88f, SpriteEffects.None, 0);
		zero.Y += num3;
		Menu.spriteBatch.DrawString(Menu.defaultFont, b7, zero, black, 0f, Vector2.Zero, 0.88f, SpriteEffects.None, 0);
		zero.Y += num3;
		Menu.spriteBatch.DrawString(Menu.defaultFont, b8, zero, black, 0f, Vector2.Zero, 0.88f, SpriteEffects.None, 0);
		zero.Y += num3;
		Menu.spriteBatch.DrawString(Menu.defaultFont, b9, zero, black, 0f, Vector2.Zero, 0.88f, SpriteEffects.None, 0);
	}

	private void DrawPageThree()
	{
		Rectangle a = new Rectangle
		{
			Width = 78,
			Height = 78
		};
		Color black = Color.Black;
		black.R = 32;
		black.G = 32;
		black.B = 32;
		Viewport viewport = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport;
		Vector2 zero = Vector2.Zero;
		int num = viewport.TitleSafeArea.Y + 100;
		int num2 = 80;
		int num3 = 24;
		string b = "Walkers move slow but inflict severe damage and can cause bleeding.";
		string b2 = "Avoid entering walker infested areas without a weapon. If you have to get";
		string b3 = "close to walkers be sure to have bandages, food and water to regain health.";
		a.X = viewport.TitleSafeArea.X + 40;
		a.Y = num - 40;
		a.Width = 64;
		a.Height = 128;
		Menu.spriteBatch.Draw(ZombieIcon, a, Color.White);
		zero.X = viewport.TitleSafeArea.X + 132;
		zero.Y = num - 32;
		Menu.spriteBatch.DrawString(Menu.defaultFont, b, zero, black, 0f, Vector2.Zero, 0.9f, SpriteEffects.None, 0);
		zero.Y += num3;
		Menu.spriteBatch.DrawString(Menu.defaultFont, b2, zero, black, 0f, Vector2.Zero, 0.9f, SpriteEffects.None, 0);
		zero.Y += num3;
		Menu.spriteBatch.DrawString(Menu.defaultFont, b3, zero, black, 0f, Vector2.Zero, 0.9f, SpriteEffects.None, 0);
		string b4 = "Weapons spawn in specific places.";
		a.X = viewport.TitleSafeArea.X + 32;
		a.Y = num + num2 + 12;
		a.Width = 256;
		a.Height = 84;
		Menu.spriteBatch.Draw(InventoryCls.WeaponTexture[3], a, Color.White);
		zero.X = viewport.TitleSafeArea.X + 196;
		zero.Y = num + num2 + 60;
		Menu.spriteBatch.DrawString(Menu.defaultFont, b4, zero, black, 0f, Vector2.Zero, 0.9f, SpriteEffects.None, 0);
		string b5 = "Each weapon uses a specific type of ammunition.";
		a.X = viewport.TitleSafeArea.X + 20;
		a.Y = num + num2 * 3 - 40;
		a.Width = 78;
		a.Height = 78;
		Menu.spriteBatch.Draw(InventoryCls.EquipmentTexture[15], a, Color.White);
		a.X = viewport.TitleSafeArea.X + 54;
		a.Y = num + num2 * 3 - 8;
		a.Width = 78;
		a.Height = 78;
		Menu.spriteBatch.Draw(InventoryCls.EquipmentTexture[13], a, Color.White);
		zero.X = viewport.TitleSafeArea.X + 128;
		zero.Y = num + num2 * 3 - 10;
		Menu.spriteBatch.DrawString(Menu.defaultFont, b5, zero, black, 0f, Vector2.Zero, 0.9f, SpriteEffects.None, 0);
		string b6 = "Weapons and Ammunition always spawn at military facilities.";
		a.X = viewport.TitleSafeArea.X + 32;
		a.Y = num + num2 * 4 - 12;
		a.Width = 78;
		a.Height = 86;
		Menu.spriteBatch.Draw(WallIcon, a, Color.White);
		zero.X = viewport.TitleSafeArea.X + 128;
		zero.Y = num + num2 * 4 + 8;
		Menu.spriteBatch.DrawString(Menu.defaultFont, b6, zero, black, 0f, Vector2.Zero, 0.9f, SpriteEffects.None, 0);
	}

	private void DrawPageFour()
	{
		Rectangle rectangle = default(Rectangle);
		rectangle = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea;
		rectangle.Y += 32;
		rectangle.Height = 512;
		Menu.spriteBatch.Draw(ControllerTex, rectangle, Color.White);
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
		currentPage = 0;
		Menu.PlaySelect();
	}

	private void SetupOtherGamesMenu()
	{
		MenuEntry menuEntry = new MenuEntry();
		Vector2 zero = Vector2.Zero;
		menuEntryList.Add(menuEntry.Set("", MenuTextJustify.Left, zero, DummyFunc, EndGameEngine.GameAssetMgr));
		menuEntry.isSelected = true;
		zero.Y += menuEntry.textHeight;
	}

	private void DummyFunc(object sender, MenuEntry e)
	{
	}
}
