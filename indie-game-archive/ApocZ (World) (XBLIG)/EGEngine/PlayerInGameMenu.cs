using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class PlayerInGameMenu : InGameMenuE
{
	private enum InGameMenuTypes
	{
		Main,
		Controller,
		NumOf
	}

	private MenuEntry ResumeMenu = new MenuEntry();

	private MenuEntry ControllerMenu = new MenuEntry();

	private MenuEntry AudioVideoMenu = new MenuEntry();

	private MenuEntry ExitMatchMenu = new MenuEntry();

	private ControllerMenu controllerMenu;

	private AudioVideoMenu audiovideoMenu;

	private GraphicsDevice gfxDevice;

	public PlayerBase SetPlayerRef
	{
		get
		{
			return playerRef;
		}
		set
		{
			playerRef = value;
		}
	}

	public PlayerInGameMenu(GameMenus id)
		: base(id)
	{
	}

	public override void LoadContent()
	{
		base.LoadContent();
		base.State = MenuState.Hidden;
		_ = EndGameEngine.DefualtViewport.TitleSafeArea;
		controllerMenu = new ControllerMenu(GameMenus.FPSControllerMenu);
		controllerMenu.LoadContent();
		audiovideoMenu = new AudioVideoMenu(GameMenus.AudioVideoMenu);
		audiovideoMenu.LoadContent();
		SetupResumeMenu();
	}

	public override void Update(float eTime)
	{
		if (playerRef == null)
		{
			return;
		}
		if (controllerMenu.State == MenuState.Active)
		{
			controllerMenu.Update(eTime);
			audiovideoMenu.Update(eTime);
			return;
		}
		for (int i = 0; i < menuEntryList.Count; i++)
		{
			menuEntryList[i].Update(eTime, (int)transitionAlpha);
		}
		HandleInput();
		if (CurrentInput == MenuInput.MenuBack)
		{
			ResumeFunc(null, null);
		}
	}

	public override void Draw()
	{
		if (playerRef == null)
		{
			return;
		}
		gfxDevice = EndGameEngine.GraphicMgr.GraphicsDevice;
		Viewport viewport = gfxDevice.Viewport;
		gfxDevice.Viewport = playerRef.vpViewPort;
		Menu.spriteBatch.Begin();
		gfxDevice.BlendState = BlendState.AlphaBlend;
		Menu.spriteBatch.Draw(a: new Rectangle(0, 0, playerRef.vpViewPort.Width, playerRef.vpViewPort.Height), t: LevelBaseMenu.texBlack, c: new Color(0, 0, 0, 180));
		Menu.spriteBatch.End();
		if (controllerMenu.State == MenuState.Active)
		{
			controllerMenu.Draw();
		}
		else if (audiovideoMenu.State == MenuState.Active)
		{
			audiovideoMenu.Draw();
		}
		else
		{
			for (int i = 0; i < menuEntryList.Count; i++)
			{
				menuEntryList[i].Draw();
			}
			DrawButtonControl(playerRef.vpViewPort, drawSelect: true, drawBack: true, drawReady: false);
		}
		gfxDevice.Viewport = viewport;
	}

	public override void DrawBackground()
	{
	}

	public override void MakeActive(MenuMgr e)
	{
		base.MakeActive(e);
		SetPlayerRef = null;
		controllerMenu.State = MenuState.Hidden;
		audiovideoMenu.State = MenuState.Hidden;
	}

	private void SetupResumeMenu()
	{
		Vector2 zero = Vector2.Zero;
		zero.X = (float)Menu.titleSafeArea.X + (float)Menu.titleSafeArea.Width * 0.15f;
		zero.Y = (float)Menu.titleSafeArea.Y + (float)Menu.titleSafeArea.Height * 0.2f;
		menuEntryList.Add(ResumeMenu.Set(MenuEntryType.Text, (MenuEntryAttribute)5, "Resume", zero, null, Color.DarkGray, "menus\\button01", "menus\\button02", "menus\\button03", ResumeFunc, EndGameEngine.GameAssetMgr));
		ResumeMenu.isSelected = true;
		zero.Y += ResumeMenu.textHeight;
		menuEntryList.Add(ControllerMenu.Set(MenuEntryType.Text, (MenuEntryAttribute)5, "Controller", zero, null, Color.DarkGray, "menus\\button01", "menus\\button02", "menus\\button03", ControllerFunc, EndGameEngine.GameAssetMgr));
		ControllerMenu.isSelected = false;
		zero.Y += ControllerMenu.textHeight;
		menuEntryList.Add(AudioVideoMenu.Set(MenuEntryType.Text, (MenuEntryAttribute)5, "Audio/Video", zero, null, Color.DarkGray, "menus\\button01", "menus\\button02", "menus\\button03", AudioVideoFunc, EndGameEngine.GameAssetMgr));
		AudioVideoMenu.isSelected = false;
		zero.Y += AudioVideoMenu.textHeight;
		menuEntryList.Add(ExitMatchMenu.Set(MenuEntryType.Text, (MenuEntryAttribute)5, "Exit Match", zero, null, Color.DarkGray, "menus\\button01", "menus\\button02", "menus\\button03", ExitMatchMenuFunc, EndGameEngine.GameAssetMgr));
		ExitMatchMenu.isSelected = false;
		zero.Y += ExitMatchMenu.textHeight;
	}

	private void ResumeFunc(object sender, MenuEntry e)
	{
		if (playerRef != null)
		{
			playerRef.MenuState = PlayerMenuState.InGame;
		}
		base.State = MenuState.Hidden;
		controllerMenu.State = MenuState.Hidden;
	}

	private void ControllerFunc(object sender, MenuEntry e)
	{
		controllerMenu.MakeActive(EndGameEngine.menuMgr);
	}

	private void AudioVideoFunc(object sender, MenuEntry e)
	{
		audiovideoMenu.MakeActive(EndGameEngine.menuMgr);
	}

	private void ExitMatchMenuFunc(object sender, MenuEntry e)
	{
		base.State = MenuState.Hidden;
		controllerMenu.State = MenuState.Hidden;
		TryExitMenuDelegate(e);
	}
}
