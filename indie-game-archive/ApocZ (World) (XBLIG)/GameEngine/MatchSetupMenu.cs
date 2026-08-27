using EGEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace GameEngine;

public class MatchSetupMenu(GameMenus id) : Menu(id)
{
	public static float UPDATE_FIND_FREQUENCEY = 20f;

	private MenuEntry CreateMatch = new MenuEntry();

	private MenuEntry LoadoutMenu = new MenuEntry();

	private MenuEntry MyLdrBrdMenu = new MenuEntry();

	private MenuEntry ControllerSetMenu = new MenuEntry();

	private MenuEntry JoinMatch = new MenuEntry();

	private MatchSessionList SessionList = new MatchSessionList();

	private float UpdateSessionListTimer;

	private static Vector2 drawMsgPos = Vector2.Zero;

	private static Vector2 drawMsgPosOffset = new Vector2(1f, 1f);

	public override void LoadContent()
	{
		base.LoadContent();
		SetupMenuEntries();
		SessionList.Reset();
		SessionList.JoinSessionDelegate += LoadJoinMatch;
	}

	public override void Update(float eTime)
	{
		UpdateTransition(eTime);
		int num = ((menuListCountOverride > 0) ? menuListCountOverride : menuEntryList.Count);
		for (int i = 0; i < num; i++)
		{
			menuEntryList[i].Update(eTime, transitionDelta);
		}
		if (base.State == MenuState.Active)
		{
			UpdateSessionListTimer += eTime;
			_ = LevelBaseMenu.InputUpdate.menuInput;
			_ = 14;
		}
		if (!SessionList.Update(eTime))
		{
			HandleInput();
		}
		Menu.PlayMusic(BackgroundMusic.Menu);
	}

	public override void Draw()
	{
		Menu.spriteBatch.Begin();
		Menu.spriteBatch.Draw(Menu.texGradientVertical, Menu.menuGradientRec, bgTextureColor);
		DrawButtonControl(EndGameEngine.GraphicMgr.GraphicsDevice.Viewport, drawSelect: true, drawBack: true, drawReady: false);
		Menu.spriteBatch.End();
		SessionList.diffus = CreateMatch.diffuse;
		SessionList.shadow = CreateMatch.shadow;
		SessionList.texClr = bgTextureColor;
		SessionList.Draw();
		int num = ((menuListCountOverride > 0) ? menuListCountOverride : menuEntryList.Count);
		for (int i = 0; i < num; i++)
		{
			menuEntryList[i].Draw(!SessionList.hasFocus);
		}
	}

	public override void MakeActive(MenuMgr e)
	{
		base.MakeActive(e);
		SessionList.hasFocus = false;
		SessionList.Reset();
		LevelBaseMenu.FPSCameraActive = false;
		if (IsBackDelegateNull())
		{
			base.BackMenuDelegate += ExitMatchSetupMenuFunc;
		}
		UpdateSessionListTimer = UPDATE_FIND_FREQUENCEY;
		EndGameEngine.UpdatePresence(GamerPresenceMode.Multiplayer);
	}

	private void SetupMenuEntries()
	{
		Vector2 zero = Vector2.Zero;
		zero.X = (float)Menu.titleSafeArea.X + (float)Menu.titleSafeArea.Width * 0.12f;
		zero.Y = (float)Menu.titleSafeArea.Y + (float)Menu.titleSafeArea.Height * 0.2f;
		menuEntryList.Add(CreateMatch.Set(MenuEntryType.Text, (MenuEntryAttribute)5, "Create Match", zero, null, Color.DarkGray, "menus\\button01", "menus\\button02", "menus\\button03", LoadCreateMatchFunc, EndGameEngine.GameAssetMgr));
		CreateMatch.isSelected = true;
		zero.Y += CreateMatch.textHeight;
		menuEntryList.Add(LoadoutMenu.Set(MenuEntryType.Text, (MenuEntryAttribute)5, "Loadout", zero, null, Color.DarkGray, "menus\\button01", "menus\\button02", "menus\\button03", LoadoutFunc, EndGameEngine.GameAssetMgr));
		LoadoutMenu.isSelected = false;
		zero.Y += LoadoutMenu.textHeight;
		menuEntryList.Add(MyLdrBrdMenu.Set(MenuEntryType.Text, (MenuEntryAttribute)5, "EOD Leaders", zero, null, Color.DarkGray, "menus\\button01", "menus\\button02", "menus\\button03", MyLeaderboardFunc, EndGameEngine.GameAssetMgr));
		MyLdrBrdMenu.isSelected = false;
		zero.Y += MyLdrBrdMenu.textHeight;
		menuEntryList.Add(ControllerSetMenu.Set(MenuEntryType.Text, (MenuEntryAttribute)5, "Controller", zero, null, Color.DarkGray, "menus\\button01", "menus\\button02", "menus\\button03", LoadControllerFunc, EndGameEngine.GameAssetMgr));
		ControllerSetMenu.isSelected = false;
		zero.Y += ControllerSetMenu.textHeight;
	}

	private void LoadCreateMatchFunc(object sender, MenuEntry e)
	{
	}

	private void LoadoutFunc(object sender, MenuEntry e)
	{
		if (Menu.BackDelegateEntry == e)
		{
			EndGameEngine.menuMgr.MakeActive(GameMenus.MatchSetupMenu);
			return;
		}
		EndGameEngine.menuMgr.SetBackMenuFunction(GameMenus.MatchLoadoutMenu, LoadoutFunc);
		EndGameEngine.menuMgr.MakeActive(GameMenus.MatchLoadoutMenu);
	}

	private void MyLeaderboardFunc(object sender, MenuEntry e)
	{
		if (Menu.BackDelegateEntry == e)
		{
			EndGameEngine.menuMgr.MakeActive(GameMenus.MatchSetupMenu);
			return;
		}
		EndGameEngine.menuMgr.SetBackMenuFunction(GameMenus.EODLeaderboard, MyLeaderboardFunc);
		EndGameEngine.menuMgr.MakeActive(GameMenus.EODLeaderboard);
	}

	private void LoadControllerFunc(object sender, MenuEntry e)
	{
		if (Menu.BackDelegateEntry == e)
		{
			EndGameEngine.menuMgr.MakeActive(GameMenus.MatchSetupMenu);
			return;
		}
		EndGameEngine.menuMgr.SetBackMenuFunction(GameMenus.FPSControllerMenu, LoadControllerFunc);
		EndGameEngine.menuMgr.MakeActive(GameMenus.FPSControllerMenu);
	}

	private void LoadJoinMatch(object sender, JoinSessionArgs e)
	{
	}

	private void ExitMatchSetupMenuFunc(object sender, MenuEntry e)
	{
		EndGameEngine.menuMgr.MakeActive(GameMenus.MatchTypeMenu);
	}
}
