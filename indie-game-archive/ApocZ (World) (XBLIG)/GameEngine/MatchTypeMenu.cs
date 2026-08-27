using EGEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine;

public class MatchTypeMenu(GameMenus id) : Menu(id)
{
	private MenuEntry FreeForAll = new MenuEntry();

	private MenuEntry TeamDeathMatch = new MenuEntry();

	private MenuEntry HeadFestMatch = new MenuEntry();

	private static bool InvitesSubscribedTo;

	public override void LoadContent()
	{
		base.LoadContent();
	}

	public override void Update(float eTime)
	{
		if (base.State == MenuState.Active && LevelBaseMenu.InputUpdate.menuInput == MenuInput.MenuBack)
		{
			LevelBaseMenu.InputUpdate.menuInput = MenuInput.None;
			Manager.MakeActive(GameMenus.MainMenu);
		}
		base.Update(eTime);
		Menu.PlayMusic(BackgroundMusic.Menu);
	}

	public override void Draw()
	{
		Menu.spriteBatch.Begin();
		Color black = Color.Black;
		black.A = (byte)(255f * transitionDelta);
		Color black2 = Color.Black;
		black2.R = (byte)(220f * transitionDelta);
		black2.G = (byte)(220f * transitionDelta);
		black2.B = (byte)(220f * transitionDelta);
		black2.A = (byte)(220f * transitionDelta);
		Vector2 c = new Vector2(520f, EndGameEngine.DefualtViewport.TitleSafeArea.Top + 32);
		Menu.spriteBatch.DrawString(Menu.defaultFont, "Match Type", c, black, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
		c.X -= 2f;
		c.Y -= 2f;
		Menu.spriteBatch.DrawString(Menu.defaultFont, "Match Type", c, black2, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
		Menu.spriteBatch.Draw(Menu.texGradientVertical, Menu.menuGradientRec, bgTextureColor);
		DrawButtonControl(EndGameEngine.GraphicMgr.GraphicsDevice.Viewport, drawSelect: true, drawBack: true, drawReady: false);
		Menu.spriteBatch.Draw(LevelBaseMenu.texBrown, new Rectangle(640, 200, 420, 340), bgTextureColor);
		c.X = 660f;
		c.Y = 220f;
		string b = "        Description\n\n";
		Menu.spriteBatch.DrawString(Menu.defaultFont, b, c, black2, 0f, Vector2.Zero, 1.2f, SpriteEffects.None, 0);
		c.Y = 280f;
		if (SelectedEntry == 0)
		{
			b = "Free-For-All style match.\n\n";
			b += "Only head shots count.\n\n";
			b += "Heads are 2X size!\n\n";
			b += "Skills are preset for fast run,\n\n";
			b += "and quick reloading.\n\n";
			b += "Loadout as the character\n\n";
			b += "you choose.\n\n";
			Menu.spriteBatch.DrawString(Menu.defaultFont, b, c, black2, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
		}
		else if (SelectedEntry == 1)
		{
			b = "Skills you choose apply in match.\n\n";
			b += "Loadout as the character\n\n";
			b += "you choose.\n\n";
			Menu.spriteBatch.DrawString(Menu.defaultFont, b, c, black2, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
		}
		else if (SelectedEntry == 2)
		{
			b = "No Friendly fire.\n\n";
			b += "Skills you choose apply in match.\n\n";
			b += "Loadout character chosen by teams.\n\n";
			Menu.spriteBatch.DrawString(Menu.defaultFont, b, c, black2, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
		}
		Menu.spriteBatch.End();
		base.Draw();
	}

	public override void MakeActive(MenuMgr e)
	{
		base.MakeActive(e);
		LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].SavePlayerStatistics();
		LevelBaseMenu.ResetNetPlayers();
		EndGameEngine.UpdatePresence(GamerPresenceMode.AtMenu);
	}

	private void ExitMatchSetupMenuFunc(object sender, MenuEntry e)
	{
		Manager.MakeActive(GameMenus.MatchTypeMenu);
	}
}
