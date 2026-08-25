using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Media;

namespace JamSouls;

public class JamSoulGame : Game
{
	public static GraphicsDeviceManager graphics;

	private ScreenManager screenManager;

	public static AudioManager audioManager;

	public JamSoulGame()
	{
		base.Content.RootDirectory = "Content";
		graphics = new GraphicsDeviceManager(this);
		graphics.PreferredBackBufferWidth = 1280;
		graphics.PreferredBackBufferHeight = 720;
		screenManager = new ScreenManager(this);
		base.Components.Add(screenManager);
		TextManager.LoadLanguage(TextManager.Languages.ENGLISH, base.Content.RootDirectory + "\\Text\\TextBase.txt");
		MediaPlayer.IsRepeating = true;
		for (int i = 0; i < Gamer.SignedInGamers.Count; i++)
		{
			GameContext.Pinfo[i].CharacterIdx = -1;
			GameContext.Pinfo[i].Controller = PlayerController.NONE;
			GameContext.Pinfo[i].SbireDef = PlayerConfig.SBIRE_DEF.NONE;
		}
		base.IsFixedTimeStep = true;
		base.TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 8);
		base.Components.Add(new GamerServicesComponent(this));
		SaveHandler.InitSaveHandler();
		GameContext.Pinfo[0].CharacterIdx = 7;
		GameContext.Pinfo[0].Controller = PlayerController.PLAYER;
		GameContext.Pinfo[0].pIndex = PlayerIndex.One;
		GameContext.Pinfo[0].SbireDef = PlayerConfig.SBIRE_DEF.NONE;
		GameContext.Pinfo[0].SlotIdx = 0;
		GameContext.Pinfo[0].Name = "Player0";
		GameContext.Pinfo[1].CharacterIdx = 8;
		GameContext.Pinfo[1].Controller = PlayerController.PLAYER_BOT;
		GameContext.Pinfo[1].pIndex = PlayerIndex.Two;
		GameContext.Pinfo[1].SbireDef = PlayerConfig.SBIRE_DEF.NONE;
		GameContext.Pinfo[1].SlotIdx = 1;
		GameContext.Pinfo[1].Name = "Player1";
		GameContext.Pinfo[2].CharacterIdx = 1;
		GameContext.Pinfo[2].Controller = PlayerController.PLAYER_BOT;
		GameContext.Pinfo[2].pIndex = PlayerIndex.Three;
		GameContext.Pinfo[2].SbireDef = PlayerConfig.SBIRE_DEF.NONE;
		GameContext.Pinfo[2].SlotIdx = 2;
		GameContext.Pinfo[2].Name = "Player2";
		GameContext.Pinfo[3].CharacterIdx = 9;
		GameContext.Pinfo[3].Controller = PlayerController.PLAYER_BOT;
		GameContext.Pinfo[3].pIndex = PlayerIndex.Four;
		GameContext.Pinfo[3].SbireDef = PlayerConfig.SBIRE_DEF.NONE;
		GameContext.Pinfo[3].SlotIdx = 3;
		GameContext.Pinfo[3].Name = "Player3";
		screenManager.AddScreen(new BumperScreen(), null);
		MediaPlayer.Volume = 0.5f;
	}

	protected override void Draw(GameTime gameTime)
	{
		base.Draw(gameTime);
	}
}
