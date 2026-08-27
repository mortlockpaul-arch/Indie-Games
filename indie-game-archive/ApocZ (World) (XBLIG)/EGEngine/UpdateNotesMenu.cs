using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class UpdateNotesMenu(GameMenus id) : Menu(id)
{
	private Color shadow = Color.Black;

	private Color diffuse = Color.Black;

	private int lineIndexStart;

	private List<string> updatenotes = new List<string>();

	private float BusyIconAngle;

	private float menuPlayerYaw;

	private Rectangle tmpRec = default(Rectangle);

	private Rectangle titleRec = default(Rectangle);

	private Rectangle titleSelectedRec = default(Rectangle);

	public override void LoadContent()
	{
		base.LoadContent();
		updatenotes.Add("*Update 1.2 notes: Following items fixed");
		updatenotes.Add("  1-Tents not saving during host migrate and at other times");
		updatenotes.Add("    A Maximum of 8 tents, 192 items stored in the tents will");
		updatenotes.Add("    be enforced in next update ( average of 24 items per tent ).");
		updatenotes.Add("  2-Player Status & inventory deleting or resetting.");
		updatenotes.Add("  3-Vehicles floating and/or passing through terrain due to player desync.");
		updatenotes.Add("  4-Vehicles stopping when player(s) join the server.");
		updatenotes.Add("  5-Vehicles randomly getting stuck in collision on trees and not being able to drive.");
		updatenotes.Add("  6-Player invisibility related to vehicles and death.");
		updatenotes.Add("  7-Host not creating the world new when in a 'sync-to-server'");
		updatenotes.Add("    and chosen by Xbox Live to migrate the host process too");
		updatenotes.Add("  8-Host lagging out on updating players across the session worked on.");
		updatenotes.Add("    Refactored code to ensure some states of players are being updated across the session.");
		updatenotes.Add("  9-Duplication exploit by exiting to dashboard at certain times.");
		updatenotes.Add(" 10-Bullet collision with terrain and objects infront of survivors.");
		updatenotes.Add(" 11-Clan diamond tags not tracking survivors.");
		updatenotes.Add(" 12-Hardware encrypted save data.");
		updatenotes.Add("");
		updatenotes.Add("*Update 1.2 adds the following");
		updatenotes.Add("  1-Block invites to clan from specific players in a server.");
		updatenotes.Add("  2-Up to 4 survivors per vehicle.");
		updatenotes.Add("  3-Lengthened daytime.");
		updatenotes.Add("");
		updatenotes.Add("");
		updatenotes.Add("*Update 1.1 notes: Following items fixed");
		updatenotes.Add(" 1-Tents not saving during host migrate and at other times.");
		updatenotes.Add(" 2-Taking bullet damage in Vehicles when weapons being shot from random places on the map.");
		updatenotes.Add(" 3-Vehicles tires/light/fuel not syncing across session or being modified by joining player.");
		updatenotes.Add(" 4-Vehicles spawning with no tires.");
		updatenotes.Add(" 5-Inventory magazines bullets being depleted when reloading weapons that are not empty.");
		updatenotes.Add(" 6-Weapons and Magazines not saving existing bullet counts when dropped in the world.");
		updatenotes.Add(" 7-Session join timeout causing to be kicked from server when trying to join.");
		updatenotes.Add(" 8-'Quality' value being bogus, replaced with actual 'Ping' value and updated with out");
		updatenotes.Add("   effecting displayed value.");
		updatenotes.Add(" 9-Join server moved from 'Menu' button to 'A' button in Xbox Live menu.");
		updatenotes.Add("10-Error tracking code to allow player to send message on state of crash.");
		updatenotes.Add("11-Data encryption on Player status, when proven to be solid will be used to encrypt all");
		updatenotes.Add("   saved data (tents, character gear, inventory...)");
		updatenotes.Add("12-Code 4 when purchase from trial mode and selecting 'Invite Friend'.");
		updatenotes.Add("13-Added Clan invite and accept to 'Player' menu.");
	}

	public override void Update(float eTime)
	{
		UpdateTransition(eTime);
		if (base.IsActive)
		{
			if (LevelBaseMenu.InputUpdate.menuInput == MenuInput.MenuUp || LevelBaseMenu.InputUpdate.menuInputContinuos == MenuInput.MenuUp)
			{
				lineIndexStart = ((lineIndexStart > 0) ? (lineIndexStart - 1) : 0);
			}
			else if (LevelBaseMenu.InputUpdate.menuInput == MenuInput.MenuDown || LevelBaseMenu.InputUpdate.menuInputContinuos == MenuInput.MenuDown)
			{
				lineIndexStart = ((lineIndexStart < updatenotes.Count - 8) ? (lineIndexStart + 1) : (updatenotes.Count - 8));
			}
			else if (LevelBaseMenu.InputUpdate.menuInput == MenuInput.MenuBack)
			{
				EndGameEngine.menuMgr.MakeActive(GameMenus.MainMenu);
			}
		}
		base.Update(eTime);
	}

	public override void Draw()
	{
		Rectangle a = default(Rectangle);
		Viewport viewport = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport;
		_ = LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value];
		Menu.spriteBatch.Begin();
		Vector2 zero = Vector2.Zero;
		zero.X = viewport.TitleSafeArea.Left;
		zero.Y = viewport.TitleSafeArea.Top + 32;
		for (int i = lineIndexStart; i < updatenotes.Count; i++)
		{
			if (updatenotes[i].Contains("*Update"))
			{
				Menu.spriteBatch.DrawString(Menu.defaultFont, updatenotes[i], zero, Color.DarkRed, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
				zero.Y += 36f;
			}
			else
			{
				Menu.spriteBatch.DrawString(Menu.defaultFont, updatenotes[i], zero, Color.LightGray, 0f, Vector2.Zero, 0.9f, SpriteEffects.None, 0);
				zero.Y += 30f;
			}
			if (zero.Y > (float)(viewport.TitleSafeArea.Bottom - 80))
			{
				break;
			}
		}
		DrawButtonControl(EndGameEngine.GraphicMgr.GraphicsDevice.Viewport, drawSelect: false, drawBack: true, drawReady: false);
		a.X = viewport.TitleSafeArea.Left + 200;
		a.Y = viewport.TitleSafeArea.Bottom - 48;
		a.Width = 32;
		a.Height = 32;
		Menu.spriteBatch.Draw(Menu.dpUp, a, buttonColor);
		a.X += 42;
		Menu.spriteBatch.Draw(Menu.dpDown, a, buttonColor);
		zero.X = a.X + 42;
		zero.Y = a.Y;
		Menu.spriteBatch.DrawString(Menu.defaultFont, "Scroll", zero, buttonColor, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
		Menu.spriteBatch.End();
	}

	public override void DrawBackground()
	{
	}

	public override void MakeActive(MenuMgr e)
	{
		base.MakeActive(e);
		lineIndexStart = 0;
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
