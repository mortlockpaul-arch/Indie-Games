using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;

namespace EGEngine;

public class PlayersMenu(GameMenus id) : Menu(id)
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

	private Rectangle tRec = default(Rectangle);

	public override void LoadContent()
	{
		base.LoadContent();
		SetupOtherGamesMenu();
	}

	public override void Update(float eTime)
	{
		base.Update(eTime);
		UpdateTransition(eTime);
		PlayerBase playerBase = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value];
		if (base.State != MenuState.Active || EGENetWorkNext.networkSession == null)
		{
			return;
		}
		int count = EGENetWorkNext.networkSession.AllGamers.Count;
		if (LevelBaseMenu.InputUpdate.menuInput == MenuInput.MenuUp)
		{
			VerticalSelection = ((VerticalSelection > 0) ? (VerticalSelection - 1) : 0);
		}
		else if (LevelBaseMenu.InputUpdate.menuInput == MenuInput.MenuDown)
		{
			VerticalSelection = ((VerticalSelection < count - 1) ? (VerticalSelection + 1) : (count - 1));
		}
		else if (playerBase.currentGamePadState.IsButtonDown(Buttons.A) && playerBase.lastGamePadState.IsButtonUp(Buttons.A))
		{
			if (VerticalSelection > 0)
			{
				int index = VerticalSelection - 1;
				PlayerBase playerBase2 = EGENetWorkNext.NextNetPlayerReference(ref index);
				if (playerBase2 != null && playerBase2.NetGamerRef != null)
				{
					EGENetWorkNext.packetWriter.Write((byte)151);
					EGENetWorkNext.networkSession.LocalGamers[0].SendData(EGENetWorkNext.packetWriter, SendDataOptions.ReliableInOrder, playerBase2.NetGamerRef);
				}
			}
		}
		else if (playerBase.currentGamePadState.IsButtonDown(Buttons.X) && playerBase.lastGamePadState.IsButtonUp(Buttons.X))
		{
			if (VerticalSelection > 0)
			{
				int index2 = VerticalSelection - 1;
				PlayerBase playerBase3 = EGENetWorkNext.NextNetPlayerReference(ref index2);
				if (playerBase3 != null && playerBase3.NetGamerRef != null)
				{
					AIBase.Clans.DeleteFromClan(playerBase3.NetGamerRef);
					EGENetWorkNext.packetWriter.Write((byte)153);
					EGENetWorkNext.networkSession.LocalGamers[0].SendData(EGENetWorkNext.packetWriter, SendDataOptions.ReliableInOrder, playerBase3.NetGamerRef);
				}
			}
		}
		else if (playerBase.currentGamePadState.IsButtonDown(Buttons.Y) && playerBase.lastGamePadState.IsButtonUp(Buttons.Y) && VerticalSelection > 0)
		{
			int index3 = VerticalSelection - 1;
			PlayerBase playerBase4 = EGENetWorkNext.NextNetPlayerReference(ref index3);
			if (playerBase4 != null && playerBase4.NetGamerRef != null)
			{
				AIBase.Clans.ToggleBlockFromInvites(playerBase4.NetGamerRef);
			}
		}
	}

	public override void Draw()
	{
		PlayerBase playerBase = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value];
		Menu.spriteBatch.Begin();
		bgTextureColor.R = (byte)(200f * transitionDelta);
		bgTextureColor.G = (byte)(200f * transitionDelta);
		bgTextureColor.B = (byte)(200f * transitionDelta);
		bgTextureColor.A = (byte)(200f * transitionDelta);
		Menu.spriteBatch.Draw(LevelBaseMenu.texBlack, EndGameEngine.DefualtViewport.TitleSafeArea, bgTextureColor);
		bgTextureColor.R = (byte)(40f * transitionDelta);
		bgTextureColor.G = (byte)(40f * transitionDelta);
		bgTextureColor.B = (byte)(40f * transitionDelta);
		bgTextureColor.A = (byte)(40f * transitionDelta);
		int height = (int)((float)EndGameEngine.DefualtViewport.TitleSafeArea.Width / (float)Menu.titleTexture.Width * (float)Menu.titleTexture.Height);
		Rectangle rectangle = new Rectangle(EndGameEngine.DefualtViewport.TitleSafeArea.X, EndGameEngine.DefualtViewport.TitleSafeArea.Y, EndGameEngine.DefualtViewport.TitleSafeArea.Width, height);
		Menu.spriteBatch.Draw(Menu.titleTexture, rectangle, bgTextureColor);
		int i = 0;
		float g = 0.6f;
		Vector2 zero = Vector2.Zero;
		Vector2 zero2 = Vector2.Zero;
		Vector2 zero3 = Vector2.Zero;
		zero2.X = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Center.X - 310;
		zero3.X = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Center.X + 155;
		zero.X = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Center.X - 230;
		if (EGENetWorkNext.networkSession != null)
		{
			zero.Y = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Top + 32;
			for (; i <= 15; i++)
			{
				int num = i + 1;
				string text = ((num < 10) ? (" " + num) : num.ToString());
				Menu.spriteBatch.DrawString(Menu.defaultFont, text + ".", zero, Color.Gray, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
				zero.Y += 30f;
			}
			i = 1;
			g = 0.75f;
			zero.X = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Center.X - 200;
			zero.Y = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Top + 32;
			zero2.Y = zero.Y;
			zero3.Y = zero.Y;
			Menu.spriteBatch.DrawString(Menu.defaultFont, playerBase.gamerTag, zero, Color.Gray, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Day " + (playerBase.CurrentDay / 24 + 1), zero3, Color.LightGray, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
			if (playerBase.NetGamerRef.IsHost)
			{
				Menu.spriteBatch.DrawString(Menu.defaultFont, "HOST", zero2, Color.Red, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
			}
			zero.Y += 30f;
			PlayerBase playerBase2 = null;
			int index = 0;
			while ((playerBase2 = EGENetWorkNext.NextNetPlayerReference(ref index)) != null)
			{
				g = 0.75f;
				Color d = Color.Gray;
				if (VerticalSelection - 1 == index)
				{
					g = 0.85f;
					d = Color.LightGray;
				}
				zero2.Y = zero.Y;
				zero3.Y = zero.Y;
				Menu.spriteBatch.DrawString(Menu.defaultFont, playerBase2.gamerTag, zero, d, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
				if (AIBase.Clans.IsInClan(playerBase2.gamerTag))
				{
					tRec.X = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Center.X - 254;
					tRec.Y = (int)zero.Y;
					tRec.Width = 22;
					tRec.Height = 22;
					Menu.spriteBatch.Draw(AIBase.Clans.diamondIcon, tRec, Color.LightGray);
				}
				else if (AIBase.Clans.IsBlocked(playerBase2.gamerTag))
				{
					tRec.X = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Center.X - 254;
					tRec.Y = (int)zero.Y;
					tRec.Width = 22;
					tRec.Height = 22;
					Menu.spriteBatch.Draw(AIBase.Clans.clanBlockIcon, tRec, Color.LightGray);
				}
				g = 0.75f;
				Menu.spriteBatch.DrawString(Menu.defaultFont, "Day " + (playerBase2.CurrentDay / 24 + 1), zero3, Color.LightGray, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
				if (playerBase2.IsHost)
				{
					Menu.spriteBatch.DrawString(Menu.defaultFont, "HOST", zero2, Color.Red, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
				}
				zero.Y += 30f;
				i++;
				index++;
			}
			for (; i <= 15; i++)
			{
				Menu.spriteBatch.DrawString(Menu.defaultFont, "Empty___________", zero, Color.Gray, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
				zero.Y += 30f;
			}
		}
		else
		{
			i = 1;
			g = 0.75f;
			zero.X = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Center.X - 200;
			zero.Y = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.TitleSafeArea.Top + 64;
			zero2.Y = zero.Y;
			zero3.Y = zero.Y;
			Menu.spriteBatch.DrawString(Menu.defaultFont, playerBase.gamerTag, zero, Color.LightGray, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Day " + (playerBase.CurrentDay / 24 + 1), zero3, Color.LightGray, 0f, Vector2.Zero, g, SpriteEffects.None, 0);
		}
		int num2 = EndGameEngine.DefualtViewport.TitleSafeArea.X + 8;
		int num3 = EndGameEngine.DefualtViewport.TitleSafeArea.Bottom - 40;
		Vector2 zero4 = Vector2.Zero;
		if (VerticalSelection > 0)
		{
			bgTextureColor = Color.LightGray;
		}
		zero4.X = num2;
		zero4.Y = num3 - 46;
		Menu.spriteBatch.DrawString(Menu.defaultFont, "Clan", zero4, bgTextureColor, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0);
		rectangle.X = num2;
		rectangle.Y = num3;
		rectangle.Width = 36;
		rectangle.Height = 36;
		Menu.DrawButton(rectangle, Buttons.A, bgTextureColor);
		zero4.X = rectangle.X + 48;
		zero4.Y = rectangle.Y - 4;
		Menu.spriteBatch.DrawString(Menu.defaultFont, "Invite", zero4, bgTextureColor, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0);
		num2 = (rectangle.X = num2 + 160);
		rectangle.Y = num3;
		rectangle.Width = 36;
		rectangle.Height = 36;
		Menu.DrawButton(rectangle, Buttons.X, bgTextureColor);
		zero4.X = rectangle.X + 48;
		zero4.Y = rectangle.Y - 4;
		Menu.spriteBatch.DrawString(Menu.defaultFont, "Delete", zero4, bgTextureColor, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0);
		num2 = (rectangle.X = num2 + 160);
		rectangle.Y = num3;
		rectangle.Width = 36;
		rectangle.Height = 36;
		Menu.DrawButton(rectangle, Buttons.Y, bgTextureColor);
		zero4.X = rectangle.X + 48;
		zero4.Y = rectangle.Y - 4;
		Menu.spriteBatch.DrawString(Menu.defaultFont, "Block/UnBlock", zero4, bgTextureColor, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0);
		num2 += 160;
		rectangle.X = EndGameEngine.DefualtViewport.TitleSafeArea.Right - 140;
		rectangle.Y = num3;
		rectangle.Width = 36;
		rectangle.Height = 36;
		Menu.DrawButton(rectangle, Buttons.B, Color.LightGray);
		zero4.X = rectangle.X + 48;
		zero4.Y = rectangle.Y - 4;
		Menu.spriteBatch.DrawString(Menu.defaultFont, "Back", zero4, Color.LightGray, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0);
		Menu.spriteBatch.End();
		base.Draw();
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
