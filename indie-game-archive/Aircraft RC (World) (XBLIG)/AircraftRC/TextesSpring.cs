using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AircraftRC;

public class TextesSpring
{
	public SpriteBatch spriteBatch;

	private SpriteFont police;

	private Texture2D crasht;

	private Texture2D gameover;

	private Texture2D bouttonS;

	private Texture2D bouttonAC;

	private Texture2D loading;

	private Texture2D pressA;

	private Texture2D pressB;

	private Texture2D restart;

	private Texture2D Help;

	private Texture2D notavailable;

	private Texture2D notcroix;

	private Texture2D Conection;

	private Texture2D ConectionB;

	private Texture2D ConectionM;

	private Texture2D TrainR;

	private Texture2D TrainS;

	private Texture2D Jauge;

	private Texture2D Buy;

	private Texture2D Time;

	private Texture2D Best;

	private Texture2D ringstart;

	private Texture2D ringendtime;

	private Texture2D ringok;

	private Texture2D Fuel;

	private Texture2D Timecrash;

	private Texture2D JaugefuelV;

	private Texture2D JaugefuelR;

	private Texture2D VoulezQuit;

	private Texture2D QuitGM;

	private Texture2D Exit;

	private Texture2D AStart;

	private ManetteConfig inputStateConfig;

	public TextesSpring(CustomPhysicsGame game)
	{
		inputStateConfig = new ManetteConfig(game);
	}

	public void Initialize()
	{
	}

	public void LoadContent(CustomPhysicsGame game)
	{
		spriteBatch = new SpriteBatch(game.GraphicsDevice);
		loading = game.LoadLocalizedAsset<Texture2D>("Textures/loading");
		notcroix = game.Content.Load<Texture2D>("Textures/no in demo");
		notavailable = game.LoadLocalizedAsset<Texture2D>("Textures/not availa");
		bouttonS = game.LoadLocalizedAsset<Texture2D>("Textures/bouttonS");
		bouttonAC = game.Content.Load<Texture2D>("Textures/bouttonAC");
		crasht = game.Content.Load<Texture2D>("Textures/crash");
		gameover = game.Content.Load<Texture2D>("Textures/game over");
		police = game.Content.Load<SpriteFont>("Font/DataFont");
		pressA = game.LoadLocalizedAsset<Texture2D>("Textures/pressA");
		pressB = game.LoadLocalizedAsset<Texture2D>("Textures/pressB");
		restart = game.LoadLocalizedAsset<Texture2D>("Textures/restart");
		Help = game.LoadLocalizedAsset<Texture2D>("Textures/help");
		Conection = game.LoadLocalizedAsset<Texture2D>("Textures/conection");
		ConectionB = game.Content.Load<Texture2D>("Textures/connecBon");
		ConectionM = game.Content.Load<Texture2D>("Textures/connecMal");
		TrainR = game.Content.Load<Texture2D>("Textures/trainR");
		TrainS = game.Content.Load<Texture2D>("Textures/trainS");
		Jauge = game.Content.Load<Texture2D>("Textures/jauge");
		Time = game.LoadLocalizedAsset<Texture2D>("Textures/time");
		Best = game.LoadLocalizedAsset<Texture2D>("Textures/best");
		Buy = game.LoadLocalizedAsset<Texture2D>("Textures/buy");
		ringstart = game.LoadLocalizedAsset<Texture2D>("Textures/entrering");
		ringendtime = game.LoadLocalizedAsset<Texture2D>("Textures/endring");
		ringok = game.LoadLocalizedAsset<Texture2D>("Textures/10ring");
		Fuel = game.LoadLocalizedAsset<Texture2D>("Textures/Fuel");
		Timecrash = game.LoadLocalizedAsset<Texture2D>("Textures/time-crash");
		JaugefuelV = game.Content.Load<Texture2D>("Textures/JaugefuelV");
		JaugefuelR = game.Content.Load<Texture2D>("Textures/JaugefuelR");
		Exit = game.LoadLocalizedAsset<Texture2D>("Textures/exit");
		AStart = game.LoadLocalizedAsset<Texture2D>("Textures/press a start");
		VoulezQuit = game.LoadLocalizedAsset<Texture2D>("Textures/quitter");
		QuitGM = game.LoadLocalizedAsset<Texture2D>("Textures/GMQuit");
	}

	public void Update(CustomPhysicsGame game, GameTime gameTime)
	{
		inputStateConfig.Update(game);
	}

	public void Draw(CustomPhysicsGame game, GameTime gameTime)
	{
		if (game.gameState == CustomPhysicsGame.GameState.Partie)
		{
			if (game.gamemode != CustomPhysicsGame.GameMode.M0 && game.SortirGM)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(QuitGM, new Vector2(240f, 170f), Color.White);
				spriteBatch.End();
			}
			if (game.gamemode == CustomPhysicsGame.GameMode.M1 && game.hideHUD == CustomPhysicsGame.HideHUD.Hcommande)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(Exit, new Vector2(135f, 625f), Color.White);
				spriteBatch.End();
			}
			if (game.gamemode == CustomPhysicsGame.GameMode.M1 && game.hideHUD == CustomPhysicsGame.HideHUD.tout)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(Exit, new Vector2(256f, 520f), Color.White);
				spriteBatch.End();
			}
			if (game.gamemode == CustomPhysicsGame.GameMode.M1)
			{
				if (game.activeSR)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(ringstart, new Vector2(560f, 95f), Color.White);
					spriteBatch.End();
				}
				if (game.jeux.finAficheR)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(ringok, new Vector2(560f, 95f), Color.White);
					spriteBatch.End();
					if (game.jeux.A < 10)
					{
						string text = $"0{game.jeux.A}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text, new Vector2(818f, 400f), Color.Red);
						spriteBatch.End();
					}
					else
					{
						string text2 = $"{game.jeux.A}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text2, new Vector2(818f, 400f), Color.Red);
						spriteBatch.End();
					}
					if (game.jeux.timecounterM < 10)
					{
						string text3 = $"0{game.jeux.timecounterM}:";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text3, new Vector2(805f, 454f), Color.Red);
						spriteBatch.End();
					}
					else
					{
						string text4 = $"{game.jeux.timecounterM}:";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text4, new Vector2(805f, 454f), Color.Red);
						spriteBatch.End();
					}
					if (game.jeux.timecounterS < 10)
					{
						string text5 = $"0{game.jeux.timecounterS}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text5, new Vector2(865f, 454f), Color.Red);
						spriteBatch.End();
					}
					else
					{
						string text6 = $"{game.jeux.timecounterS}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text6, new Vector2(865f, 454f), Color.Red);
						spriteBatch.End();
					}
				}
				if (game.jeux.finAficheT)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(ringendtime, new Vector2(560f, 95f), Color.White);
					spriteBatch.End();
					if (game.jeux.A < 10)
					{
						string text7 = $"0{game.jeux.A}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text7, new Vector2(818f, 395f), Color.Red);
						spriteBatch.End();
					}
					else
					{
						string text8 = $"{game.jeux.A}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text8, new Vector2(818f, 395f), Color.Red);
						spriteBatch.End();
					}
					if (game.jeux.timecounterM < 10)
					{
						string text9 = $"0{game.jeux.timecounterM}:";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text9, new Vector2(805f, 451f), Color.Red);
						spriteBatch.End();
					}
					else
					{
						string text10 = $"{game.jeux.timecounterM}:";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text10, new Vector2(805f, 451f), Color.Red);
						spriteBatch.End();
					}
					if (game.jeux.timecounterS < 10)
					{
						string text11 = $"0{game.jeux.timecounterS}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text11, new Vector2(865f, 451f), Color.Red);
						spriteBatch.End();
					}
					else
					{
						string text12 = $"{game.jeux.timecounterS}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text12, new Vector2(865f, 451f), Color.Red);
						spriteBatch.End();
					}
				}
				if (!game.activeSR && !game.jeux.finAficheR && !game.jeux.finAficheT)
				{
					if (game.string1 == "0" || game.string1 == "2")
					{
						spriteBatch.Begin();
						spriteBatch.Draw(Time, new Vector2(880f, 90f), Color.White);
						spriteBatch.End();
					}
					else
					{
						spriteBatch.Begin();
						spriteBatch.Draw(Time, new Vector2(905f, 90f), Color.White);
						spriteBatch.End();
					}
					if (game.string1 == "0" || game.string1 == "2")
					{
						spriteBatch.Begin();
						spriteBatch.Draw(Best, new Vector2(995f, 510f), Color.White);
						spriteBatch.End();
					}
					else
					{
						spriteBatch.Begin();
						spriteBatch.Draw(Best, new Vector2(1020f, 505f), Color.White);
						spriteBatch.End();
					}
					if (!game.jeux.couleurG)
					{
						if (game.jeux.A < 10)
						{
							string text13 = $"0{game.jeux.A}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text13, new Vector2(1050f, 80f), Color.Red);
							spriteBatch.End();
						}
						else
						{
							string text14 = $"{game.jeux.A}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text14, new Vector2(1050f, 80f), Color.Red);
							spriteBatch.End();
						}
					}
					if (game.jeux.couleurG)
					{
						if (game.jeux.A < 10)
						{
							string text15 = $"0{game.jeux.A}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text15, new Vector2(1050f, 80f), Color.GreenYellow);
							spriteBatch.End();
						}
						else
						{
							string text16 = $"{game.jeux.A}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text16, new Vector2(1050f, 80f), Color.GreenYellow);
							spriteBatch.End();
						}
					}
					if (game.jeux.ReA < 10)
					{
						string text17 = $"0{game.jeux.ReA}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text17, new Vector2(1060f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					else
					{
						string text18 = $"{game.jeux.ReA}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text18, new Vector2(1060f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					if (!game.jeux.couleurT)
					{
						if (game.jeux.timecounterM < 10)
						{
							string text19 = $"0{game.jeux.timecounterM}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text19, new Vector2(1040f, 110f), Color.Red);
							spriteBatch.End();
						}
						else
						{
							string text20 = $"{game.jeux.timecounterM}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text20, new Vector2(1040f, 110f), Color.Red);
							spriteBatch.End();
						}
						if (game.jeux.timecounterS < 10)
						{
							string text21 = $"0{game.jeux.timecounterS}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text21, new Vector2(1100f, 110f), Color.Red);
							spriteBatch.End();
						}
						else
						{
							string text22 = $"{game.jeux.timecounterS}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text22, new Vector2(1100f, 110f), Color.Red);
							spriteBatch.End();
						}
					}
					if (game.jeux.couleurT)
					{
						if (game.jeux.timecounterM < 10)
						{
							string text23 = $"0{game.jeux.timecounterM}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text23, new Vector2(1040f, 110f), Color.GreenYellow);
							spriteBatch.End();
						}
						else
						{
							string text24 = $"{game.jeux.timecounterM}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text24, new Vector2(1040f, 110f), Color.GreenYellow);
							spriteBatch.End();
						}
						if (game.jeux.timecounterS < 10)
						{
							string text25 = $"0{game.jeux.timecounterS}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text25, new Vector2(1100f, 110f), Color.GreenYellow);
							spriteBatch.End();
						}
						else
						{
							string text26 = $"{game.jeux.timecounterS}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text26, new Vector2(1100f, 110f), Color.GreenYellow);
							spriteBatch.End();
						}
					}
					if (game.jeux.timecounter1M < 10)
					{
						string text27 = $"0{game.jeux.timecounter1M}:";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text27, new Vector2(1030f, 560f), Color.GreenYellow);
						spriteBatch.End();
					}
					else
					{
						string text28 = $"{game.jeux.timecounter1M}:";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text28, new Vector2(1030f, 560f), Color.GreenYellow);
						spriteBatch.End();
					}
					if (game.jeux.timecounter1S < 10)
					{
						string text29 = $"0{game.jeux.timecounter1S}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text29, new Vector2(1090f, 560f), Color.GreenYellow);
						spriteBatch.End();
					}
					else
					{
						string text30 = $"{game.jeux.timecounter1S}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text30, new Vector2(1090f, 560f), Color.GreenYellow);
						spriteBatch.End();
					}
				}
			}
		}
		if (game.menu.avionChoix == MenuModel.AvionChoix.A1 && Guide.IsTrialMode && game.gameState == CustomPhysicsGame.GameState.Loading && game.camera.position == new Vector3(0f, 15f, 10f))
		{
			if (game.string1 == "0")
			{
				spriteBatch.Begin();
				spriteBatch.Draw(loading, new Vector2(350f, 560f), Color.White);
				spriteBatch.End();
			}
			else
			{
				spriteBatch.Begin();
				spriteBatch.Draw(loading, new Vector2(490f, 560f), Color.White);
				spriteBatch.End();
			}
		}
		if (!Guide.IsTrialMode && game.gameState == CustomPhysicsGame.GameState.Loading && game.camera.position == new Vector3(0f, 15f, 10f))
		{
			if (game.string1 == "0")
			{
				spriteBatch.Begin();
				spriteBatch.Draw(loading, new Vector2(350f, 560f), Color.White);
				spriteBatch.End();
			}
			else
			{
				spriteBatch.Begin();
				spriteBatch.Draw(loading, new Vector2(490f, 560f), Color.White);
				spriteBatch.End();
			}
		}
		if (game.gameState == CustomPhysicsGame.GameState.Menu && game.camera.position == new Vector3(0f, 15f, 10f))
		{
			if (game.menu.avionChoix == MenuModel.AvionChoix.A2 && Guide.IsTrialMode)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(notcroix, new Vector2(520f, 252f), Color.White);
				spriteBatch.End();
				if (game.input.CurrentGamePadStates[game.pla].Buttons.A == ButtonState.Pressed)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(notavailable, new Vector2(400f, 554f), Color.White);
					spriteBatch.End();
				}
			}
			if (game.menu.avionChoix == MenuModel.AvionChoix.A3 && Guide.IsTrialMode)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(notcroix, new Vector2(520f, 252f), Color.White);
				spriteBatch.End();
				if (game.input.CurrentGamePadStates[game.pla].Buttons.A == ButtonState.Pressed)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(notavailable, new Vector2(400f, 554f), Color.White);
					spriteBatch.End();
				}
			}
			if (game.menu.avionChoix == MenuModel.AvionChoix.A4 && Guide.IsTrialMode)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(notcroix, new Vector2(520f, 252f), Color.White);
				spriteBatch.End();
				if (game.input.CurrentGamePadStates[game.pla].Buttons.A == ButtonState.Pressed)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(notavailable, new Vector2(400f, 554f), Color.White);
					spriteBatch.End();
				}
			}
			if (game.menu.avionChoix == MenuModel.AvionChoix.A5 && Guide.IsTrialMode)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(notcroix, new Vector2(520f, 252f), Color.White);
				spriteBatch.End();
				if (game.input.CurrentGamePadStates[game.pla].Buttons.A == ButtonState.Pressed)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(notavailable, new Vector2(400f, 554f), Color.White);
					spriteBatch.End();
				}
			}
			if (game.menu.avionChoix == MenuModel.AvionChoix.A6 && Guide.IsTrialMode)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(notcroix, new Vector2(520f, 252f), Color.White);
				spriteBatch.End();
				if (game.input.CurrentGamePadStates[game.pla].Buttons.A == ButtonState.Pressed)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(notavailable, new Vector2(400f, 554f), Color.White);
					spriteBatch.End();
				}
			}
		}
		if (game.gameState == CustomPhysicsGame.GameState.Partie && game.menu.avionChoix == MenuModel.AvionChoix.A1)
		{
			if (game.gamemode == CustomPhysicsGame.GameMode.M0)
			{
				if (game.hideHUD == CustomPhysicsGame.HideHUD.tout || game.hideHUD == CustomPhysicsGame.HideHUD.Hcommande)
				{
					if (game.string1 == "0" || game.string1 == "2")
					{
						spriteBatch.Begin();
						spriteBatch.Draw(Best, new Vector2(990f, 515f), Color.White);
						spriteBatch.End();
					}
					else
					{
						spriteBatch.Begin();
						spriteBatch.Draw(Best, new Vector2(1010f, 515f), Color.White);
						spriteBatch.End();
					}
					if (game.string1 == "0")
					{
						spriteBatch.Begin();
						spriteBatch.Draw(Timecrash, new Vector2(730f, 73f), Color.White);
						spriteBatch.End();
					}
					if (game.string1 == "1")
					{
						spriteBatch.Begin();
						spriteBatch.Draw(Timecrash, new Vector2(760f, 73f), Color.White);
						spriteBatch.End();
					}
					if (game.string1 == "2")
					{
						spriteBatch.Begin();
						spriteBatch.Draw(Timecrash, new Vector2(675f, 74f), Color.White);
						spriteBatch.End();
					}
					if (game.conteurSpad.couleurT)
					{
						if (game.conteurSpad.timecounterHA < 10)
						{
							string text31 = $"0{game.conteurSpad.timecounterHA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text31, new Vector2(1000f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
						else
						{
							string text32 = $"{game.conteurSpad.timecounterHA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text32, new Vector2(1000f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
						if (game.conteurSpad.timecounterMA < 10)
						{
							string text33 = $"0{game.conteurSpad.timecounterMA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text33, new Vector2(1055f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
						else
						{
							string text34 = $"{game.conteurSpad.timecounterMA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text34, new Vector2(1055f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
						if (game.conteurSpad.timecounterSA < 10)
						{
							string text35 = $"0{game.conteurSpad.timecounterSA}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text35, new Vector2(1110f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
						else
						{
							string text36 = $"{game.conteurSpad.timecounterSA}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text36, new Vector2(1110f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
					}
					if (!game.conteurSpad.couleurT)
					{
						if (game.conteurSpad.timecounterHA < 10)
						{
							string text37 = $"0{game.conteurSpad.timecounterHA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text37, new Vector2(1000f, 63f), Color.Red);
							spriteBatch.End();
						}
						else
						{
							string text38 = $"{game.conteurSpad.timecounterHA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text38, new Vector2(1000f, 63f), Color.Red);
							spriteBatch.End();
						}
						if (game.conteurSpad.timecounterMA < 10)
						{
							string text39 = $"0{game.conteurSpad.timecounterMA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text39, new Vector2(1055f, 63f), Color.Red);
							spriteBatch.End();
						}
						else
						{
							string text40 = $"{game.conteurSpad.timecounterMA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text40, new Vector2(1055f, 63f), Color.Red);
							spriteBatch.End();
						}
						if (game.conteurSpad.timecounterSA < 10)
						{
							string text41 = $"0{game.conteurSpad.timecounterSA}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text41, new Vector2(1110f, 63f), Color.Red);
							spriteBatch.End();
						}
						else
						{
							string text42 = $"{game.conteurSpad.timecounterSA}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text42, new Vector2(1110f, 63f), Color.Red);
							spriteBatch.End();
						}
					}
					if (game.conteurSpad.timecounterHA1 < 10)
					{
						string text43 = $"0{game.conteurSpad.timecounterHA1}:";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text43, new Vector2(1000f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					else
					{
						string text44 = $"{game.conteurSpad.timecounterHA1}:";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text44, new Vector2(1000f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					if (game.conteurSpad.timecounterMA1 < 10)
					{
						string text45 = $"0{game.conteurSpad.timecounterMA1}:";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text45, new Vector2(1055f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					else
					{
						string text46 = $"{game.conteurSpad.timecounterMA1}:";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text46, new Vector2(1055f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					if (game.conteurSpad.timecounterSA1 < 10)
					{
						string text47 = $"0{game.conteurSpad.timecounterSA1}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text47, new Vector2(1110f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					else
					{
						string text48 = $"{game.conteurSpad.timecounterSA1}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text48, new Vector2(1110f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					if (game.conteurSpad.couleurT)
					{
						if (game.avion1.compteCrash < 10)
						{
							string text49 = $"0{game.avion1.compteCrash}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text49, new Vector2(992f, 92f), Color.GreenYellow);
							spriteBatch.End();
						}
						else
						{
							string text50 = $"{game.avion1.compteCrash}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text50, new Vector2(992f, 92f), Color.GreenYellow);
							spriteBatch.End();
						}
					}
					if (!game.conteurSpad.couleurT)
					{
						if (game.avion1.compteCrash < 10)
						{
							string text51 = $"0{game.avion1.compteCrash}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text51, new Vector2(992f, 92f), Color.Red);
							spriteBatch.End();
						}
						else
						{
							string text52 = $"{game.avion1.compteCrash}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text52, new Vector2(992f, 92f), Color.Red);
							spriteBatch.End();
						}
					}
					if (game.conteurSpad.totalCrash < 10)
					{
						string text53 = $"0{game.conteurSpad.totalCrash}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text53, new Vector2(992f, 560f), Color.GreenYellow);
						spriteBatch.End();
					}
					else
					{
						string text54 = $"{game.conteurSpad.totalCrash}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text54, new Vector2(992f, 560f), Color.GreenYellow);
						spriteBatch.End();
					}
				}
				if ((game.hideHUD == CustomPhysicsGame.HideHUD.tout && game.hideHUD != CustomPhysicsGame.HideHUD.Hfeul) || game.hideHUD == CustomPhysicsGame.HideHUD.compter)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(Fuel, new Vector2(254f, 500f), Color.White);
					spriteBatch.End();
					if (game.avion1.JaugeFuel >= 280f)
					{
						spriteBatch.Begin();
						spriteBatch.Draw(JaugefuelV, new Vector2(game.avion1.JaugeFuel, 536f), Color.White);
						spriteBatch.End();
					}
					if (game.avion1.JaugeFuel <= 280f)
					{
						spriteBatch.Begin();
						spriteBatch.Draw(JaugefuelR, new Vector2(game.avion1.JaugeFuel, 536f), Color.White);
						spriteBatch.End();
					}
				}
				if (game.hideHUD != CustomPhysicsGame.HideHUD.tout && game.hideHUD != CustomPhysicsGame.HideHUD.Hfeul)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(Fuel, new Vector2(254f, 600f), Color.White);
					spriteBatch.End();
					if (game.avion1.JaugeFuel >= 280f)
					{
						spriteBatch.Begin();
						spriteBatch.Draw(JaugefuelV, new Vector2(game.avion1.JaugeFuel, 636f), Color.White);
						spriteBatch.End();
					}
					if (game.avion1.JaugeFuel <= 280f)
					{
						spriteBatch.Begin();
						spriteBatch.Draw(JaugefuelR, new Vector2(game.avion1.JaugeFuel, 636f), Color.White);
						spriteBatch.End();
					}
				}
			}
			if (game.hide == CustomPhysicsGame.Hide.vu)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(restart, new Vector2(128f, 72f), Color.White);
				spriteBatch.End();
			}
			else if (game.hideHUD != CustomPhysicsGame.HideHUD.tout && game.hideHUD != CustomPhysicsGame.HideHUD.compter)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(Help, new Vector2(135f, 575f), Color.White);
				spriteBatch.End();
			}
			else
			{
				spriteBatch.Begin();
				spriteBatch.Draw(Help, new Vector2(135f, 430f), Color.White);
				spriteBatch.End();
			}
			if (game.avion1.Avioncasse)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(crasht, new Vector2(450f, 620f), Color.White);
				spriteBatch.End();
			}
			if (game.avion1.tempcrash >= 15f)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(gameover, new Vector2(810f, 620f), Color.White);
				spriteBatch.End();
			}
			if (game.avion1.temploin >= 20f && game.avion1.Avionloin && game.avion1.AvionloinA)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(gameover, new Vector2(820f, 620f), Color.White);
				spriteBatch.End();
			}
			if (game.hideHUD == CustomPhysicsGame.HideHUD.tout || game.hideHUD == CustomPhysicsGame.HideHUD.compter)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(bouttonS, new Vector2(128f, 465f), Color.White);
				spriteBatch.Draw(bouttonAC, new Vector2(161f, game.avion1.MDboutton), Color.White);
				spriteBatch.End();
				spriteBatch.Begin();
				spriteBatch.Draw(Jauge, new Vector2(game.avion1.JaugeBoutton, 465f), Color.White);
				spriteBatch.End();
				spriteBatch.Begin();
				spriteBatch.Draw(ConectionM, new Vector2(128f, 465f), Color.White);
				spriteBatch.End();
				if (!game.avion1.AvionloinA)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(ConectionB, new Vector2(128f, 465f), Color.White);
					spriteBatch.End();
				}
				spriteBatch.Begin();
				spriteBatch.Draw(TrainS, new Vector2(128f, 465f), Color.White);
				spriteBatch.End();
			}
			if (game.avion1.AvionloinA && !game.avion1.Avioncasse)
			{
				if (game.string1 == "0")
				{
					spriteBatch.Begin();
					spriteBatch.Draw(Conection, new Vector2(430f, 560f), Color.White);
					spriteBatch.End();
				}
				else
				{
					spriteBatch.Begin();
					spriteBatch.Draw(Conection, new Vector2(430f, 620f), Color.White);
					spriteBatch.End();
				}
			}
		}
		if (game.gameState == CustomPhysicsGame.GameState.Partie && game.menu.avionChoix == MenuModel.AvionChoix.A2)
		{
			if (game.gamemode == CustomPhysicsGame.GameMode.M0)
			{
				if (game.hideHUD == CustomPhysicsGame.HideHUD.tout || game.hideHUD == CustomPhysicsGame.HideHUD.Hcommande)
				{
					if (game.string1 == "0" || game.string1 == "2")
					{
						spriteBatch.Begin();
						spriteBatch.Draw(Best, new Vector2(990f, 515f), Color.White);
						spriteBatch.End();
					}
					else
					{
						spriteBatch.Begin();
						spriteBatch.Draw(Best, new Vector2(1010f, 515f), Color.White);
						spriteBatch.End();
					}
					if (game.string1 == "0")
					{
						spriteBatch.Begin();
						spriteBatch.Draw(Timecrash, new Vector2(730f, 73f), Color.White);
						spriteBatch.End();
					}
					if (game.string1 == "1")
					{
						spriteBatch.Begin();
						spriteBatch.Draw(Timecrash, new Vector2(760f, 73f), Color.White);
						spriteBatch.End();
					}
					if (game.string1 == "2")
					{
						spriteBatch.Begin();
						spriteBatch.Draw(Timecrash, new Vector2(675f, 74f), Color.White);
						spriteBatch.End();
					}
					if (game.conteurCanadair.couleurT)
					{
						if (game.conteurCanadair.timecounterHA < 10)
						{
							string text55 = $"0{game.conteurCanadair.timecounterHA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text55, new Vector2(1000f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
						else
						{
							string text56 = $"{game.conteurCanadair.timecounterHA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text56, new Vector2(1000f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
						if (game.conteurCanadair.timecounterMA < 10)
						{
							string text57 = $"0{game.conteurCanadair.timecounterMA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text57, new Vector2(1055f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
						else
						{
							string text58 = $"{game.conteurCanadair.timecounterMA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text58, new Vector2(1055f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
						if (game.conteurCanadair.timecounterSA < 10)
						{
							string text59 = $"0{game.conteurCanadair.timecounterSA}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text59, new Vector2(1110f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
						else
						{
							string text60 = $"{game.conteurCanadair.timecounterSA}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text60, new Vector2(1110f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
					}
					if (!game.conteurCanadair.couleurT)
					{
						if (game.conteurCanadair.timecounterHA < 10)
						{
							string text61 = $"0{game.conteurCanadair.timecounterHA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text61, new Vector2(1000f, 63f), Color.Red);
							spriteBatch.End();
						}
						else
						{
							string text62 = $"{game.conteurCanadair.timecounterHA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text62, new Vector2(1000f, 63f), Color.Red);
							spriteBatch.End();
						}
						if (game.conteurCanadair.timecounterMA < 10)
						{
							string text63 = $"0{game.conteurCanadair.timecounterMA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text63, new Vector2(1055f, 63f), Color.Red);
							spriteBatch.End();
						}
						else
						{
							string text64 = $"{game.conteurCanadair.timecounterMA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text64, new Vector2(1055f, 63f), Color.Red);
							spriteBatch.End();
						}
						if (game.conteurCanadair.timecounterSA < 10)
						{
							string text65 = $"0{game.conteurCanadair.timecounterSA}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text65, new Vector2(1110f, 63f), Color.Red);
							spriteBatch.End();
						}
						else
						{
							string text66 = $"{game.conteurCanadair.timecounterSA}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text66, new Vector2(1110f, 63f), Color.Red);
							spriteBatch.End();
						}
					}
					if (game.conteurCanadair.timecounterHA1 < 10)
					{
						string text67 = $"0{game.conteurCanadair.timecounterHA1}:";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text67, new Vector2(1000f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					else
					{
						string text68 = $"{game.conteurCanadair.timecounterHA1}:";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text68, new Vector2(1000f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					if (game.conteurCanadair.timecounterMA1 < 10)
					{
						string text69 = $"0{game.conteurCanadair.timecounterMA1}:";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text69, new Vector2(1055f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					else
					{
						string text70 = $"{game.conteurCanadair.timecounterMA1}:";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text70, new Vector2(1055f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					if (game.conteurCanadair.timecounterSA1 < 10)
					{
						string text71 = $"0{game.conteurCanadair.timecounterSA1}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text71, new Vector2(1110f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					else
					{
						string text72 = $"{game.conteurCanadair.timecounterSA1}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text72, new Vector2(1110f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					if (game.conteurCanadair.couleurT)
					{
						if (game.avion4.compteCrash < 10)
						{
							string text73 = $"0{game.avion4.compteCrash}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text73, new Vector2(992f, 92f), Color.GreenYellow);
							spriteBatch.End();
						}
						else
						{
							string text74 = $"{game.avion4.compteCrash}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text74, new Vector2(992f, 92f), Color.GreenYellow);
							spriteBatch.End();
						}
					}
					if (!game.conteurCanadair.couleurT)
					{
						if (game.avion4.compteCrash < 10)
						{
							string text75 = $"0{game.avion4.compteCrash}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text75, new Vector2(992f, 92f), Color.Red);
							spriteBatch.End();
						}
						else
						{
							string text76 = $"{game.avion4.compteCrash}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text76, new Vector2(992f, 92f), Color.Red);
							spriteBatch.End();
						}
					}
					if (game.conteurCanadair.totalCrash < 10)
					{
						string text77 = $"0{game.conteurCanadair.totalCrash}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text77, new Vector2(992f, 560f), Color.GreenYellow);
						spriteBatch.End();
					}
					else
					{
						string text78 = $"{game.conteurCanadair.totalCrash}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text78, new Vector2(992f, 560f), Color.GreenYellow);
						spriteBatch.End();
					}
				}
				if ((game.hideHUD == CustomPhysicsGame.HideHUD.tout && game.hideHUD != CustomPhysicsGame.HideHUD.Hfeul) || game.hideHUD == CustomPhysicsGame.HideHUD.compter)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(Fuel, new Vector2(254f, 500f), Color.White);
					spriteBatch.End();
					if (game.avion4.JaugeFuel >= 280f)
					{
						spriteBatch.Begin();
						spriteBatch.Draw(JaugefuelV, new Vector2(game.avion4.JaugeFuel, 536f), Color.White);
						spriteBatch.End();
					}
					if (game.avion4.JaugeFuel <= 280f)
					{
						spriteBatch.Begin();
						spriteBatch.Draw(JaugefuelR, new Vector2(game.avion4.JaugeFuel, 536f), Color.White);
						spriteBatch.End();
					}
				}
				if (game.hideHUD != CustomPhysicsGame.HideHUD.tout && game.hideHUD != CustomPhysicsGame.HideHUD.Hfeul)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(Fuel, new Vector2(254f, 600f), Color.White);
					spriteBatch.End();
					if (game.avion4.JaugeFuel >= 280f)
					{
						spriteBatch.Begin();
						spriteBatch.Draw(JaugefuelV, new Vector2(game.avion4.JaugeFuel, 636f), Color.White);
						spriteBatch.End();
					}
					if (game.avion4.JaugeFuel <= 280f)
					{
						spriteBatch.Begin();
						spriteBatch.Draw(JaugefuelR, new Vector2(game.avion4.JaugeFuel, 636f), Color.White);
						spriteBatch.End();
					}
				}
			}
			if (game.hide == CustomPhysicsGame.Hide.vu)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(restart, new Vector2(128f, 72f), Color.White);
				spriteBatch.End();
			}
			else if (game.hideHUD != CustomPhysicsGame.HideHUD.tout && game.hideHUD != CustomPhysicsGame.HideHUD.compter)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(Help, new Vector2(135f, 575f), Color.White);
				spriteBatch.End();
			}
			else
			{
				spriteBatch.Begin();
				spriteBatch.Draw(Help, new Vector2(135f, 430f), Color.White);
				spriteBatch.End();
			}
			if (game.avion4.Avioncasse)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(crasht, new Vector2(450f, 620f), Color.White);
				spriteBatch.End();
			}
			if (game.avion4.tempcrash >= 15f)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(gameover, new Vector2(810f, 620f), Color.White);
				spriteBatch.End();
			}
			if (game.avion4.temploin >= 20f && game.avion4.Avionloin && game.avion4.AvionloinA)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(gameover, new Vector2(820f, 620f), Color.White);
				spriteBatch.End();
			}
			if (game.hideHUD == CustomPhysicsGame.HideHUD.tout || game.hideHUD == CustomPhysicsGame.HideHUD.compter)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(bouttonS, new Vector2(128f, 465f), Color.White);
				spriteBatch.Draw(bouttonAC, new Vector2(161f, game.avion4.MDboutton), Color.White);
				spriteBatch.End();
				spriteBatch.Begin();
				spriteBatch.Draw(Jauge, new Vector2(game.avion4.JaugeBoutton, 465f), Color.White);
				spriteBatch.End();
				spriteBatch.Begin();
				spriteBatch.Draw(ConectionM, new Vector2(128f, 465f), Color.White);
				spriteBatch.End();
				if (game.avion4.Ytrain >= -1.07f)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(TrainR, new Vector2(128f, 465f), Color.White);
					spriteBatch.End();
				}
				if (game.avion4.Ytrain <= -1.09f)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(TrainS, new Vector2(128f, 465f), Color.White);
					spriteBatch.End();
				}
				if (!game.avion4.AvionloinA)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(ConectionB, new Vector2(128f, 465f), Color.White);
					spriteBatch.End();
				}
			}
			if (game.avion4.AvionloinA && !game.avion4.Avioncasse)
			{
				if (game.string1 == "0")
				{
					spriteBatch.Begin();
					spriteBatch.Draw(Conection, new Vector2(430f, 560f), Color.White);
					spriteBatch.End();
				}
				else
				{
					spriteBatch.Begin();
					spriteBatch.Draw(Conection, new Vector2(430f, 620f), Color.White);
					spriteBatch.End();
				}
			}
		}
		if (game.gameState == CustomPhysicsGame.GameState.Partie && game.menu.avionChoix == MenuModel.AvionChoix.A3)
		{
			if (game.gamemode == CustomPhysicsGame.GameMode.M0)
			{
				if (game.hideHUD == CustomPhysicsGame.HideHUD.tout || game.hideHUD == CustomPhysicsGame.HideHUD.Hcommande)
				{
					if (game.string1 == "0" || game.string1 == "2")
					{
						spriteBatch.Begin();
						spriteBatch.Draw(Best, new Vector2(990f, 515f), Color.White);
						spriteBatch.End();
					}
					else
					{
						spriteBatch.Begin();
						spriteBatch.Draw(Best, new Vector2(1010f, 515f), Color.White);
						spriteBatch.End();
					}
					if (game.string1 == "0")
					{
						spriteBatch.Begin();
						spriteBatch.Draw(Timecrash, new Vector2(730f, 73f), Color.White);
						spriteBatch.End();
					}
					if (game.string1 == "1")
					{
						spriteBatch.Begin();
						spriteBatch.Draw(Timecrash, new Vector2(760f, 73f), Color.White);
						spriteBatch.End();
					}
					if (game.string1 == "2")
					{
						spriteBatch.Begin();
						spriteBatch.Draw(Timecrash, new Vector2(675f, 74f), Color.White);
						spriteBatch.End();
					}
					if (game.conteurCorsair.couleurT)
					{
						if (game.conteurCorsair.timecounterHA < 10)
						{
							string text79 = $"0{game.conteurCorsair.timecounterHA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text79, new Vector2(1000f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
						else
						{
							string text80 = $"{game.conteurCorsair.timecounterHA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text80, new Vector2(1000f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
						if (game.conteurCorsair.timecounterMA < 10)
						{
							string text81 = $"0{game.conteurCorsair.timecounterMA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text81, new Vector2(1055f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
						else
						{
							string text82 = $"{game.conteurCorsair.timecounterMA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text82, new Vector2(1055f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
						if (game.conteurCorsair.timecounterSA < 10)
						{
							string text83 = $"0{game.conteurCorsair.timecounterSA}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text83, new Vector2(1110f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
						else
						{
							string text84 = $"{game.conteurCorsair.timecounterSA}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text84, new Vector2(1110f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
					}
					if (!game.conteurCorsair.couleurT)
					{
						if (game.conteurCorsair.timecounterHA < 10)
						{
							string text85 = $"0{game.conteurCorsair.timecounterHA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text85, new Vector2(1000f, 63f), Color.Red);
							spriteBatch.End();
						}
						else
						{
							string text86 = $"{game.conteurCorsair.timecounterHA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text86, new Vector2(1000f, 63f), Color.Red);
							spriteBatch.End();
						}
						if (game.conteurCorsair.timecounterMA < 10)
						{
							string text87 = $"0{game.conteurCorsair.timecounterMA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text87, new Vector2(1055f, 63f), Color.Red);
							spriteBatch.End();
						}
						else
						{
							string text88 = $"{game.conteurCorsair.timecounterMA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text88, new Vector2(1055f, 63f), Color.Red);
							spriteBatch.End();
						}
						if (game.conteurCorsair.timecounterSA < 10)
						{
							string text89 = $"0{game.conteurCorsair.timecounterSA}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text89, new Vector2(1110f, 63f), Color.Red);
							spriteBatch.End();
						}
						else
						{
							string text90 = $"{game.conteurCorsair.timecounterSA}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text90, new Vector2(1110f, 63f), Color.Red);
							spriteBatch.End();
						}
					}
					if (game.conteurCorsair.timecounterHA1 < 10)
					{
						string text91 = $"0{game.conteurCorsair.timecounterHA1}:";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text91, new Vector2(1000f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					else
					{
						string text92 = $"{game.conteurCorsair.timecounterHA1}:";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text92, new Vector2(1000f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					if (game.conteurCorsair.timecounterMA1 < 10)
					{
						string text93 = $"0{game.conteurCorsair.timecounterMA1}:";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text93, new Vector2(1055f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					else
					{
						string text94 = $"{game.conteurCorsair.timecounterMA1}:";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text94, new Vector2(1055f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					if (game.conteurCorsair.timecounterSA1 < 10)
					{
						string text95 = $"0{game.conteurCorsair.timecounterSA1}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text95, new Vector2(1110f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					else
					{
						string text96 = $"{game.conteurCorsair.timecounterSA1}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text96, new Vector2(1110f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					if (game.conteurCorsair.couleurT)
					{
						if (game.avion2.compteCrash < 10)
						{
							string text97 = $"0{game.avion2.compteCrash}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text97, new Vector2(992f, 92f), Color.GreenYellow);
							spriteBatch.End();
						}
						else
						{
							string text98 = $"{game.avion2.compteCrash}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text98, new Vector2(992f, 92f), Color.GreenYellow);
							spriteBatch.End();
						}
					}
					if (!game.conteurCorsair.couleurT)
					{
						if (game.avion2.compteCrash < 10)
						{
							string text99 = $"0{game.avion2.compteCrash}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text99, new Vector2(992f, 92f), Color.Red);
							spriteBatch.End();
						}
						else
						{
							string text100 = $"{game.avion2.compteCrash}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text100, new Vector2(992f, 92f), Color.Red);
							spriteBatch.End();
						}
					}
					if (game.conteurCorsair.totalCrash < 10)
					{
						string text101 = $"0{game.conteurCorsair.totalCrash}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text101, new Vector2(992f, 560f), Color.GreenYellow);
						spriteBatch.End();
					}
					else
					{
						string text102 = $"{game.conteurCorsair.totalCrash}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text102, new Vector2(992f, 560f), Color.GreenYellow);
						spriteBatch.End();
					}
				}
				if ((game.hideHUD == CustomPhysicsGame.HideHUD.tout && game.hideHUD != CustomPhysicsGame.HideHUD.Hfeul) || game.hideHUD == CustomPhysicsGame.HideHUD.compter)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(Fuel, new Vector2(254f, 500f), Color.White);
					spriteBatch.End();
					if (game.avion2.JaugeFuel >= 280f)
					{
						spriteBatch.Begin();
						spriteBatch.Draw(JaugefuelV, new Vector2(game.avion2.JaugeFuel, 536f), Color.White);
						spriteBatch.End();
					}
					if (game.avion2.JaugeFuel <= 280f)
					{
						spriteBatch.Begin();
						spriteBatch.Draw(JaugefuelR, new Vector2(game.avion2.JaugeFuel, 536f), Color.White);
						spriteBatch.End();
					}
				}
				if (game.hideHUD != CustomPhysicsGame.HideHUD.tout && game.hideHUD != CustomPhysicsGame.HideHUD.Hfeul)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(Fuel, new Vector2(254f, 600f), Color.White);
					spriteBatch.End();
					if (game.avion2.JaugeFuel >= 280f)
					{
						spriteBatch.Begin();
						spriteBatch.Draw(JaugefuelV, new Vector2(game.avion2.JaugeFuel, 636f), Color.White);
						spriteBatch.End();
					}
					if (game.avion2.JaugeFuel <= 280f)
					{
						spriteBatch.Begin();
						spriteBatch.Draw(JaugefuelR, new Vector2(game.avion2.JaugeFuel, 636f), Color.White);
						spriteBatch.End();
					}
				}
			}
			if (game.hide == CustomPhysicsGame.Hide.vu)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(restart, new Vector2(128f, 72f), Color.White);
				spriteBatch.End();
			}
			else if (game.hideHUD != CustomPhysicsGame.HideHUD.tout && game.hideHUD != CustomPhysicsGame.HideHUD.compter)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(Help, new Vector2(135f, 575f), Color.White);
				spriteBatch.End();
			}
			else
			{
				spriteBatch.Begin();
				spriteBatch.Draw(Help, new Vector2(135f, 430f), Color.White);
				spriteBatch.End();
			}
			if (game.avion2.Avioncasse)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(crasht, new Vector2(450f, 620f), Color.White);
				spriteBatch.End();
			}
			if (game.avion2.tempcrash >= 15f)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(gameover, new Vector2(810f, 620f), Color.White);
				spriteBatch.End();
			}
			if (game.avion2.temploin >= 20f && game.avion2.Avionloin && game.avion2.AvionloinA)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(gameover, new Vector2(820f, 620f), Color.White);
				spriteBatch.End();
			}
			if (game.hideHUD == CustomPhysicsGame.HideHUD.tout || game.hideHUD == CustomPhysicsGame.HideHUD.compter)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(bouttonS, new Vector2(128f, 465f), Color.White);
				spriteBatch.Draw(bouttonAC, new Vector2(161f, game.avion2.MDboutton), Color.White);
				spriteBatch.End();
				spriteBatch.Begin();
				spriteBatch.Draw(Jauge, new Vector2(game.avion2.JaugeBoutton, 465f), Color.White);
				spriteBatch.End();
				spriteBatch.Begin();
				spriteBatch.Draw(ConectionM, new Vector2(128f, 465f), Color.White);
				spriteBatch.End();
				if (game.avion2.Ytrain >= -1.07f)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(TrainR, new Vector2(128f, 465f), Color.White);
					spriteBatch.End();
				}
				if (game.avion2.Ytrain <= -1.09f)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(TrainS, new Vector2(128f, 465f), Color.White);
					spriteBatch.End();
				}
				if (!game.avion2.AvionloinA)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(ConectionB, new Vector2(128f, 465f), Color.White);
					spriteBatch.End();
				}
			}
			if (game.avion2.AvionloinA && !game.avion2.Avioncasse)
			{
				if (game.string1 == "0")
				{
					spriteBatch.Begin();
					spriteBatch.Draw(Conection, new Vector2(430f, 560f), Color.White);
					spriteBatch.End();
				}
				else
				{
					spriteBatch.Begin();
					spriteBatch.Draw(Conection, new Vector2(430f, 620f), Color.White);
					spriteBatch.End();
				}
			}
		}
		if (game.gameState == CustomPhysicsGame.GameState.Partie && game.menu.avionChoix == MenuModel.AvionChoix.A4)
		{
			if (game.gamemode == CustomPhysicsGame.GameMode.M0)
			{
				if (game.hideHUD == CustomPhysicsGame.HideHUD.tout || game.hideHUD == CustomPhysicsGame.HideHUD.Hcommande)
				{
					if (game.string1 == "0" || game.string1 == "2")
					{
						spriteBatch.Begin();
						spriteBatch.Draw(Best, new Vector2(990f, 515f), Color.White);
						spriteBatch.End();
					}
					else
					{
						spriteBatch.Begin();
						spriteBatch.Draw(Best, new Vector2(1010f, 515f), Color.White);
						spriteBatch.End();
					}
					if (game.string1 == "0")
					{
						spriteBatch.Begin();
						spriteBatch.Draw(Timecrash, new Vector2(730f, 73f), Color.White);
						spriteBatch.End();
					}
					if (game.string1 == "1")
					{
						spriteBatch.Begin();
						spriteBatch.Draw(Timecrash, new Vector2(760f, 73f), Color.White);
						spriteBatch.End();
					}
					if (game.string1 == "2")
					{
						spriteBatch.Begin();
						spriteBatch.Draw(Timecrash, new Vector2(675f, 74f), Color.White);
						spriteBatch.End();
					}
					if (game.conteurac130.couleurT)
					{
						if (game.conteurac130.timecounterHA < 10)
						{
							string text103 = $"0{game.conteurac130.timecounterHA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text103, new Vector2(1000f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
						else
						{
							string text104 = $"{game.conteurac130.timecounterHA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text104, new Vector2(1000f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
						if (game.conteurac130.timecounterMA < 10)
						{
							string text105 = $"0{game.conteurac130.timecounterMA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text105, new Vector2(1055f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
						else
						{
							string text106 = $"{game.conteurac130.timecounterMA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text106, new Vector2(1055f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
						if (game.conteurac130.timecounterSA < 10)
						{
							string text107 = $"0{game.conteurac130.timecounterSA}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text107, new Vector2(1110f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
						else
						{
							string text108 = $"{game.conteurac130.timecounterSA}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text108, new Vector2(1110f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
					}
					if (!game.conteurac130.couleurT)
					{
						if (game.conteurac130.timecounterHA < 10)
						{
							string text109 = $"0{game.conteurac130.timecounterHA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text109, new Vector2(1000f, 63f), Color.Red);
							spriteBatch.End();
						}
						else
						{
							string text110 = $"{game.conteurac130.timecounterHA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text110, new Vector2(1000f, 63f), Color.Red);
							spriteBatch.End();
						}
						if (game.conteurac130.timecounterMA < 10)
						{
							string text111 = $"0{game.conteurac130.timecounterMA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text111, new Vector2(1055f, 63f), Color.Red);
							spriteBatch.End();
						}
						else
						{
							string text112 = $"{game.conteurac130.timecounterMA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text112, new Vector2(1055f, 63f), Color.Red);
							spriteBatch.End();
						}
						if (game.conteurac130.timecounterSA < 10)
						{
							string text113 = $"0{game.conteurac130.timecounterSA}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text113, new Vector2(1110f, 63f), Color.Red);
							spriteBatch.End();
						}
						else
						{
							string text114 = $"{game.conteurac130.timecounterSA}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text114, new Vector2(1110f, 63f), Color.Red);
							spriteBatch.End();
						}
					}
					if (game.conteurac130.timecounterHA1 < 10)
					{
						string text115 = $"0{game.conteurac130.timecounterHA1}:";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text115, new Vector2(1000f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					else
					{
						string text116 = $"{game.conteurac130.timecounterHA1}:";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text116, new Vector2(1000f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					if (game.conteurac130.timecounterMA1 < 10)
					{
						string text117 = $"0{game.conteurac130.timecounterMA1}:";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text117, new Vector2(1055f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					else
					{
						string text118 = $"{game.conteurac130.timecounterMA1}:";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text118, new Vector2(1055f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					if (game.conteurac130.timecounterSA1 < 10)
					{
						string text119 = $"0{game.conteurac130.timecounterSA1}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text119, new Vector2(1110f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					else
					{
						string text120 = $"{game.conteurac130.timecounterSA1}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text120, new Vector2(1110f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					if (game.conteurac130.couleurT)
					{
						if (game.avion5.compteCrash < 10)
						{
							string text121 = $"0{game.avion5.compteCrash}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text121, new Vector2(992f, 92f), Color.GreenYellow);
							spriteBatch.End();
						}
						else
						{
							string text122 = $"{game.avion5.compteCrash}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text122, new Vector2(992f, 92f), Color.GreenYellow);
							spriteBatch.End();
						}
					}
					if (!game.conteurac130.couleurT)
					{
						if (game.avion5.compteCrash < 10)
						{
							string text123 = $"0{game.avion5.compteCrash}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text123, new Vector2(992f, 92f), Color.Red);
							spriteBatch.End();
						}
						else
						{
							string text124 = $"{game.avion5.compteCrash}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text124, new Vector2(992f, 92f), Color.Red);
							spriteBatch.End();
						}
					}
					if (game.conteurac130.totalCrash < 10)
					{
						string text125 = $"0{game.conteurac130.totalCrash}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text125, new Vector2(992f, 560f), Color.GreenYellow);
						spriteBatch.End();
					}
					else
					{
						string text126 = $"{game.conteurac130.totalCrash}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text126, new Vector2(992f, 560f), Color.GreenYellow);
						spriteBatch.End();
					}
				}
				if ((game.hideHUD == CustomPhysicsGame.HideHUD.tout && game.hideHUD != CustomPhysicsGame.HideHUD.Hfeul) || game.hideHUD == CustomPhysicsGame.HideHUD.compter)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(Fuel, new Vector2(254f, 500f), Color.White);
					spriteBatch.End();
					if (game.avion5.JaugeFuel >= 280f)
					{
						spriteBatch.Begin();
						spriteBatch.Draw(JaugefuelV, new Vector2(game.avion5.JaugeFuel, 536f), Color.White);
						spriteBatch.End();
					}
					if (game.avion5.JaugeFuel <= 280f)
					{
						spriteBatch.Begin();
						spriteBatch.Draw(JaugefuelR, new Vector2(game.avion5.JaugeFuel, 536f), Color.White);
						spriteBatch.End();
					}
				}
				if (game.hideHUD != CustomPhysicsGame.HideHUD.tout && game.hideHUD != CustomPhysicsGame.HideHUD.Hfeul)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(Fuel, new Vector2(254f, 600f), Color.White);
					spriteBatch.End();
					if (game.avion5.JaugeFuel >= 280f)
					{
						spriteBatch.Begin();
						spriteBatch.Draw(JaugefuelV, new Vector2(game.avion5.JaugeFuel, 636f), Color.White);
						spriteBatch.End();
					}
					if (game.avion5.JaugeFuel <= 280f)
					{
						spriteBatch.Begin();
						spriteBatch.Draw(JaugefuelR, new Vector2(game.avion5.JaugeFuel, 636f), Color.White);
						spriteBatch.End();
					}
				}
			}
			if (game.hide == CustomPhysicsGame.Hide.vu)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(restart, new Vector2(128f, 72f), Color.White);
				spriteBatch.End();
			}
			else if (game.hideHUD != CustomPhysicsGame.HideHUD.tout && game.hideHUD != CustomPhysicsGame.HideHUD.compter)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(Help, new Vector2(135f, 575f), Color.White);
				spriteBatch.End();
			}
			else
			{
				spriteBatch.Begin();
				spriteBatch.Draw(Help, new Vector2(135f, 430f), Color.White);
				spriteBatch.End();
			}
			if (game.avion5.Avioncasse)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(crasht, new Vector2(450f, 620f), Color.White);
				spriteBatch.End();
			}
			if (game.avion5.tempcrash >= 15f)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(gameover, new Vector2(810f, 620f), Color.White);
				spriteBatch.End();
			}
			if (game.avion5.temploin >= 20f && game.avion5.Avionloin && game.avion5.AvionloinA)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(gameover, new Vector2(820f, 620f), Color.White);
				spriteBatch.End();
			}
			if (game.hideHUD == CustomPhysicsGame.HideHUD.tout || game.hideHUD == CustomPhysicsGame.HideHUD.compter)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(bouttonS, new Vector2(128f, 465f), Color.White);
				spriteBatch.Draw(bouttonAC, new Vector2(161f, game.avion5.MDboutton), Color.White);
				spriteBatch.End();
				spriteBatch.Begin();
				spriteBatch.Draw(Jauge, new Vector2(game.avion5.JaugeBoutton, 465f), Color.White);
				spriteBatch.End();
				spriteBatch.Begin();
				spriteBatch.Draw(ConectionM, new Vector2(128f, 465f), Color.White);
				spriteBatch.End();
				if (!game.avion5.AvionloinA)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(ConectionB, new Vector2(128f, 465f), Color.White);
					spriteBatch.End();
				}
				if (game.avion5.Ytrain >= -1.07f)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(TrainR, new Vector2(128f, 465f), Color.White);
					spriteBatch.End();
				}
				if (game.avion5.Ytrain <= -1.09f)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(TrainS, new Vector2(128f, 465f), Color.White);
					spriteBatch.End();
				}
			}
			if (game.avion5.AvionloinA && !game.avion5.Avioncasse)
			{
				if (game.string1 == "0")
				{
					spriteBatch.Begin();
					spriteBatch.Draw(Conection, new Vector2(430f, 560f), Color.White);
					spriteBatch.End();
				}
				else
				{
					spriteBatch.Begin();
					spriteBatch.Draw(Conection, new Vector2(430f, 620f), Color.White);
					spriteBatch.End();
				}
			}
		}
		if (game.gameState == CustomPhysicsGame.GameState.Partie && game.menu.avionChoix == MenuModel.AvionChoix.A5)
		{
			if (game.gamemode == CustomPhysicsGame.GameMode.M0)
			{
				if (game.hideHUD == CustomPhysicsGame.HideHUD.tout || game.hideHUD == CustomPhysicsGame.HideHUD.Hcommande)
				{
					if (game.string1 == "0" || game.string1 == "2")
					{
						spriteBatch.Begin();
						spriteBatch.Draw(Best, new Vector2(990f, 515f), Color.White);
						spriteBatch.End();
					}
					else
					{
						spriteBatch.Begin();
						spriteBatch.Draw(Best, new Vector2(1010f, 515f), Color.White);
						spriteBatch.End();
					}
					if (game.string1 == "0")
					{
						spriteBatch.Begin();
						spriteBatch.Draw(Timecrash, new Vector2(730f, 73f), Color.White);
						spriteBatch.End();
					}
					if (game.string1 == "1")
					{
						spriteBatch.Begin();
						spriteBatch.Draw(Timecrash, new Vector2(760f, 73f), Color.White);
						spriteBatch.End();
					}
					if (game.string1 == "2")
					{
						spriteBatch.Begin();
						spriteBatch.Draw(Timecrash, new Vector2(675f, 74f), Color.White);
						spriteBatch.End();
					}
					if (game.conteurDH112.couleurT)
					{
						if (game.conteurDH112.timecounterHA < 10)
						{
							string text127 = $"0{game.conteurDH112.timecounterHA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text127, new Vector2(1000f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
						else
						{
							string text128 = $"{game.conteurDH112.timecounterHA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text128, new Vector2(1000f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
						if (game.conteurDH112.timecounterMA < 10)
						{
							string text129 = $"0{game.conteurDH112.timecounterMA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text129, new Vector2(1055f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
						else
						{
							string text130 = $"{game.conteurDH112.timecounterMA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text130, new Vector2(1055f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
						if (game.conteurDH112.timecounterSA < 10)
						{
							string text131 = $"0{game.conteurDH112.timecounterSA}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text131, new Vector2(1110f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
						else
						{
							string text132 = $"{game.conteurDH112.timecounterSA}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text132, new Vector2(1110f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
					}
					if (!game.conteurDH112.couleurT)
					{
						if (game.conteurDH112.timecounterHA < 10)
						{
							string text133 = $"0{game.conteurDH112.timecounterHA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text133, new Vector2(1000f, 63f), Color.Red);
							spriteBatch.End();
						}
						else
						{
							string text134 = $"{game.conteurDH112.timecounterHA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text134, new Vector2(1000f, 63f), Color.Red);
							spriteBatch.End();
						}
						if (game.conteurDH112.timecounterMA < 10)
						{
							string text135 = $"0{game.conteurDH112.timecounterMA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text135, new Vector2(1055f, 63f), Color.Red);
							spriteBatch.End();
						}
						else
						{
							string text136 = $"{game.conteurDH112.timecounterMA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text136, new Vector2(1055f, 63f), Color.Red);
							spriteBatch.End();
						}
						if (game.conteurDH112.timecounterSA < 10)
						{
							string text137 = $"0{game.conteurDH112.timecounterSA}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text137, new Vector2(1110f, 63f), Color.Red);
							spriteBatch.End();
						}
						else
						{
							string text138 = $"{game.conteurDH112.timecounterSA}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text138, new Vector2(1110f, 63f), Color.Red);
							spriteBatch.End();
						}
					}
					if (game.conteurDH112.timecounterHA1 < 10)
					{
						string text139 = $"0{game.conteurDH112.timecounterHA1}:";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text139, new Vector2(1000f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					else
					{
						string text140 = $"{game.conteurDH112.timecounterHA1}:";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text140, new Vector2(1000f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					if (game.conteurDH112.timecounterMA1 < 10)
					{
						string text141 = $"0{game.conteurDH112.timecounterMA1}:";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text141, new Vector2(1055f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					else
					{
						string text142 = $"{game.conteurDH112.timecounterMA1}:";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text142, new Vector2(1055f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					if (game.conteurDH112.timecounterSA1 < 10)
					{
						string text143 = $"0{game.conteurDH112.timecounterSA1}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text143, new Vector2(1110f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					else
					{
						string text144 = $"{game.conteurDH112.timecounterSA1}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text144, new Vector2(1110f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					if (game.conteurDH112.couleurT)
					{
						if (game.avion3.compteCrash < 10)
						{
							string text145 = $"0{game.avion3.compteCrash}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text145, new Vector2(992f, 92f), Color.GreenYellow);
							spriteBatch.End();
						}
						else
						{
							string text146 = $"{game.avion3.compteCrash}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text146, new Vector2(992f, 92f), Color.GreenYellow);
							spriteBatch.End();
						}
					}
					if (!game.conteurDH112.couleurT)
					{
						if (game.avion3.compteCrash < 10)
						{
							string text147 = $"0{game.avion3.compteCrash}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text147, new Vector2(992f, 92f), Color.Red);
							spriteBatch.End();
						}
						else
						{
							string text148 = $"{game.avion3.compteCrash}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text148, new Vector2(992f, 92f), Color.Red);
							spriteBatch.End();
						}
					}
					if (game.conteurDH112.totalCrash < 10)
					{
						string text149 = $"0{game.conteurDH112.totalCrash}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text149, new Vector2(992f, 560f), Color.GreenYellow);
						spriteBatch.End();
					}
					else
					{
						string text150 = $"{game.conteurDH112.totalCrash}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text150, new Vector2(992f, 560f), Color.GreenYellow);
						spriteBatch.End();
					}
				}
				if ((game.hideHUD == CustomPhysicsGame.HideHUD.tout && game.hideHUD != CustomPhysicsGame.HideHUD.Hfeul) || game.hideHUD == CustomPhysicsGame.HideHUD.compter)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(Fuel, new Vector2(254f, 500f), Color.White);
					spriteBatch.End();
					if (game.avion3.JaugeFuel >= 280f)
					{
						spriteBatch.Begin();
						spriteBatch.Draw(JaugefuelV, new Vector2(game.avion3.JaugeFuel, 536f), Color.White);
						spriteBatch.End();
					}
					if (game.avion3.JaugeFuel <= 280f)
					{
						spriteBatch.Begin();
						spriteBatch.Draw(JaugefuelR, new Vector2(game.avion3.JaugeFuel, 536f), Color.White);
						spriteBatch.End();
					}
				}
				if (game.hideHUD != CustomPhysicsGame.HideHUD.tout && game.hideHUD != CustomPhysicsGame.HideHUD.Hfeul)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(Fuel, new Vector2(254f, 600f), Color.White);
					spriteBatch.End();
					if (game.avion3.JaugeFuel >= 280f)
					{
						spriteBatch.Begin();
						spriteBatch.Draw(JaugefuelV, new Vector2(game.avion3.JaugeFuel, 636f), Color.White);
						spriteBatch.End();
					}
					if (game.avion3.JaugeFuel <= 280f)
					{
						spriteBatch.Begin();
						spriteBatch.Draw(JaugefuelR, new Vector2(game.avion3.JaugeFuel, 636f), Color.White);
						spriteBatch.End();
					}
				}
			}
			if (game.hide == CustomPhysicsGame.Hide.vu)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(restart, new Vector2(128f, 72f), Color.White);
				spriteBatch.End();
			}
			else if (game.hideHUD != CustomPhysicsGame.HideHUD.tout && game.hideHUD != CustomPhysicsGame.HideHUD.compter)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(Help, new Vector2(135f, 575f), Color.White);
				spriteBatch.End();
			}
			else
			{
				spriteBatch.Begin();
				spriteBatch.Draw(Help, new Vector2(135f, 430f), Color.White);
				spriteBatch.End();
			}
			if (game.avion3.Avioncasse)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(crasht, new Vector2(450f, 620f), Color.White);
				spriteBatch.End();
			}
			if (game.avion3.tempcrash >= 15f)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(gameover, new Vector2(810f, 620f), Color.White);
				spriteBatch.End();
			}
			if (game.avion3.temploin >= 20f && game.avion3.Avionloin && game.avion3.AvionloinA)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(gameover, new Vector2(820f, 620f), Color.White);
				spriteBatch.End();
			}
			if (game.hideHUD == CustomPhysicsGame.HideHUD.tout || game.hideHUD == CustomPhysicsGame.HideHUD.compter)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(bouttonS, new Vector2(128f, 465f), Color.White);
				spriteBatch.Draw(bouttonAC, new Vector2(161f, game.avion3.MDboutton), Color.White);
				spriteBatch.End();
				spriteBatch.Begin();
				spriteBatch.Draw(Jauge, new Vector2(game.avion3.JaugeBoutton, 465f), Color.White);
				spriteBatch.End();
				spriteBatch.Begin();
				spriteBatch.Draw(ConectionM, new Vector2(128f, 465f), Color.White);
				spriteBatch.End();
				if (!game.avion3.AvionloinA)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(ConectionB, new Vector2(128f, 465f), Color.White);
					spriteBatch.End();
				}
				if (game.avion3.Ytrain >= -0.52f)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(TrainR, new Vector2(128f, 465f), Color.White);
					spriteBatch.End();
				}
				if (game.avion3.Ytrain <= -0.53f)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(TrainS, new Vector2(128f, 465f), Color.White);
					spriteBatch.End();
				}
			}
			if (game.avion3.AvionloinA && !game.avion3.Avioncasse)
			{
				if (game.string1 == "0")
				{
					spriteBatch.Begin();
					spriteBatch.Draw(Conection, new Vector2(430f, 560f), Color.White);
					spriteBatch.End();
				}
				else
				{
					spriteBatch.Begin();
					spriteBatch.Draw(Conection, new Vector2(430f, 620f), Color.White);
					spriteBatch.End();
				}
			}
		}
		if (game.gameState == CustomPhysicsGame.GameState.Partie && game.menu.avionChoix == MenuModel.AvionChoix.A6)
		{
			if (game.gamemode == CustomPhysicsGame.GameMode.M0)
			{
				if (game.hideHUD == CustomPhysicsGame.HideHUD.tout || game.hideHUD == CustomPhysicsGame.HideHUD.Hcommande)
				{
					if (game.string1 == "0" || game.string1 == "2")
					{
						spriteBatch.Begin();
						spriteBatch.Draw(Best, new Vector2(990f, 515f), Color.White);
						spriteBatch.End();
					}
					else
					{
						spriteBatch.Begin();
						spriteBatch.Draw(Best, new Vector2(1010f, 515f), Color.White);
						spriteBatch.End();
					}
					if (game.string1 == "0")
					{
						spriteBatch.Begin();
						spriteBatch.Draw(Timecrash, new Vector2(730f, 73f), Color.White);
						spriteBatch.End();
					}
					if (game.string1 == "1")
					{
						spriteBatch.Begin();
						spriteBatch.Draw(Timecrash, new Vector2(760f, 73f), Color.White);
						spriteBatch.End();
					}
					if (game.string1 == "2")
					{
						spriteBatch.Begin();
						spriteBatch.Draw(Timecrash, new Vector2(675f, 74f), Color.White);
						spriteBatch.End();
					}
					if (game.conteurF22.couleurT)
					{
						if (game.conteurF22.timecounterHA < 10)
						{
							string text151 = $"0{game.conteurF22.timecounterHA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text151, new Vector2(1000f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
						else
						{
							string text152 = $"{game.conteurF22.timecounterHA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text152, new Vector2(1000f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
						if (game.conteurF22.timecounterMA < 10)
						{
							string text153 = $"0{game.conteurF22.timecounterMA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text153, new Vector2(1055f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
						else
						{
							string text154 = $"{game.conteurF22.timecounterMA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text154, new Vector2(1055f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
						if (game.conteurF22.timecounterSA < 10)
						{
							string text155 = $"0{game.conteurF22.timecounterSA}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text155, new Vector2(1110f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
						else
						{
							string text156 = $"{game.conteurF22.timecounterSA}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text156, new Vector2(1110f, 63f), Color.GreenYellow);
							spriteBatch.End();
						}
					}
					if (!game.conteurF22.couleurT)
					{
						if (game.conteurF22.timecounterHA < 10)
						{
							string text157 = $"0{game.conteurF22.timecounterHA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text157, new Vector2(1000f, 63f), Color.Red);
							spriteBatch.End();
						}
						else
						{
							string text158 = $"{game.conteurF22.timecounterHA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text158, new Vector2(1000f, 63f), Color.Red);
							spriteBatch.End();
						}
						if (game.conteurF22.timecounterMA < 10)
						{
							string text159 = $"0{game.conteurF22.timecounterMA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text159, new Vector2(1055f, 63f), Color.Red);
							spriteBatch.End();
						}
						else
						{
							string text160 = $"{game.conteurF22.timecounterMA}:";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text160, new Vector2(1055f, 63f), Color.Red);
							spriteBatch.End();
						}
						if (game.conteurF22.timecounterSA < 10)
						{
							string text161 = $"0{game.conteurF22.timecounterSA}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text161, new Vector2(1110f, 63f), Color.Red);
							spriteBatch.End();
						}
						else
						{
							string text162 = $"{game.conteurF22.timecounterSA}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text162, new Vector2(1110f, 63f), Color.Red);
							spriteBatch.End();
						}
					}
					if (game.conteurF22.timecounterHA1 < 10)
					{
						string text163 = $"0{game.conteurF22.timecounterHA1}:";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text163, new Vector2(1000f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					else
					{
						string text164 = $"{game.conteurF22.timecounterHA1}:";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text164, new Vector2(1000f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					if (game.conteurF22.timecounterMA1 < 10)
					{
						string text165 = $"0{game.conteurF22.timecounterMA1}:";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text165, new Vector2(1055f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					else
					{
						string text166 = $"{game.conteurF22.timecounterMA1}:";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text166, new Vector2(1055f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					if (game.conteurF22.timecounterSA1 < 10)
					{
						string text167 = $"0{game.conteurF22.timecounterSA1}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text167, new Vector2(1110f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					else
					{
						string text168 = $"{game.conteurF22.timecounterSA1}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text168, new Vector2(1110f, 530f), Color.GreenYellow);
						spriteBatch.End();
					}
					if (game.conteurF22.couleurT)
					{
						if (game.avion6.compteCrash < 10)
						{
							string text169 = $"0{game.avion6.compteCrash}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text169, new Vector2(992f, 92f), Color.GreenYellow);
							spriteBatch.End();
						}
						else
						{
							string text170 = $"{game.avion6.compteCrash}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text170, new Vector2(992f, 92f), Color.GreenYellow);
							spriteBatch.End();
						}
					}
					if (!game.conteurF22.couleurT)
					{
						if (game.avion6.compteCrash < 10)
						{
							string text171 = $"0{game.avion6.compteCrash}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text171, new Vector2(992f, 92f), Color.Red);
							spriteBatch.End();
						}
						else
						{
							string text172 = $"{game.avion6.compteCrash}";
							spriteBatch.Begin();
							spriteBatch.DrawString(police, text172, new Vector2(992f, 92f), Color.Red);
							spriteBatch.End();
						}
					}
					if (game.conteurF22.totalCrash < 10)
					{
						string text173 = $"0{game.conteurF22.totalCrash}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text173, new Vector2(992f, 560f), Color.GreenYellow);
						spriteBatch.End();
					}
					else
					{
						string text174 = $"{game.conteurF22.totalCrash}";
						spriteBatch.Begin();
						spriteBatch.DrawString(police, text174, new Vector2(992f, 560f), Color.GreenYellow);
						spriteBatch.End();
					}
				}
				if ((game.hideHUD == CustomPhysicsGame.HideHUD.tout && game.hideHUD != CustomPhysicsGame.HideHUD.Hfeul) || game.hideHUD == CustomPhysicsGame.HideHUD.compter)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(Fuel, new Vector2(254f, 500f), Color.White);
					spriteBatch.End();
					if (game.avion6.JaugeFuel >= 280f)
					{
						spriteBatch.Begin();
						spriteBatch.Draw(JaugefuelV, new Vector2(game.avion6.JaugeFuel, 536f), Color.White);
						spriteBatch.End();
					}
					if (game.avion6.JaugeFuel <= 280f)
					{
						spriteBatch.Begin();
						spriteBatch.Draw(JaugefuelR, new Vector2(game.avion6.JaugeFuel, 536f), Color.White);
						spriteBatch.End();
					}
				}
				if (game.hideHUD != CustomPhysicsGame.HideHUD.tout && game.hideHUD != CustomPhysicsGame.HideHUD.Hfeul)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(Fuel, new Vector2(254f, 600f), Color.White);
					spriteBatch.End();
					if (game.avion6.JaugeFuel >= 280f)
					{
						spriteBatch.Begin();
						spriteBatch.Draw(JaugefuelV, new Vector2(game.avion6.JaugeFuel, 636f), Color.White);
						spriteBatch.End();
					}
					if (game.avion6.JaugeFuel <= 280f)
					{
						spriteBatch.Begin();
						spriteBatch.Draw(JaugefuelR, new Vector2(game.avion6.JaugeFuel, 636f), Color.White);
						spriteBatch.End();
					}
				}
			}
			if (game.hide == CustomPhysicsGame.Hide.vu)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(restart, new Vector2(128f, 72f), Color.White);
				spriteBatch.End();
			}
			else if (game.hideHUD != CustomPhysicsGame.HideHUD.tout && game.hideHUD != CustomPhysicsGame.HideHUD.compter)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(Help, new Vector2(135f, 575f), Color.White);
				spriteBatch.End();
			}
			else
			{
				spriteBatch.Begin();
				spriteBatch.Draw(Help, new Vector2(135f, 430f), Color.White);
				spriteBatch.End();
			}
			if (game.avion6.Avioncasse)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(crasht, new Vector2(450f, 620f), Color.White);
				spriteBatch.End();
			}
			if (game.avion6.tempcrash >= 15f)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(gameover, new Vector2(810f, 620f), Color.White);
				spriteBatch.End();
			}
			if (game.avion6.temploin >= 20f && game.avion6.Avionloin && game.avion6.AvionloinA)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(gameover, new Vector2(820f, 620f), Color.White);
				spriteBatch.End();
			}
			if (game.hideHUD == CustomPhysicsGame.HideHUD.tout || game.hideHUD == CustomPhysicsGame.HideHUD.compter)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(bouttonS, new Vector2(128f, 465f), Color.White);
				spriteBatch.Draw(bouttonAC, new Vector2(161f, game.avion6.MDboutton), Color.White);
				spriteBatch.End();
				spriteBatch.Begin();
				spriteBatch.Draw(Jauge, new Vector2(game.avion6.JaugeBoutton, 465f), Color.White);
				spriteBatch.End();
				spriteBatch.Begin();
				spriteBatch.Draw(ConectionM, new Vector2(128f, 465f), Color.White);
				spriteBatch.End();
				if (game.avion6.Ytrain >= -0.52f)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(TrainR, new Vector2(128f, 465f), Color.White);
					spriteBatch.End();
				}
				if (game.avion6.Ytrain <= -0.53f)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(TrainS, new Vector2(128f, 465f), Color.White);
					spriteBatch.End();
				}
				if (!game.avion6.AvionloinA)
				{
					spriteBatch.Begin();
					spriteBatch.Draw(ConectionB, new Vector2(128f, 465f), Color.White);
					spriteBatch.End();
				}
			}
			if (game.avion6.AvionloinA && !game.avion6.Avioncasse)
			{
				if (game.string1 == "0")
				{
					spriteBatch.Begin();
					spriteBatch.Draw(Conection, new Vector2(430f, 560f), Color.White);
					spriteBatch.End();
				}
				else
				{
					spriteBatch.Begin();
					spriteBatch.Draw(Conection, new Vector2(430f, 620f), Color.White);
					spriteBatch.End();
				}
			}
		}
		if (game.gameState == CustomPhysicsGame.GameState.pressA)
		{
			spriteBatch.Begin();
			spriteBatch.Draw(AStart, new Vector2(170f, 80f), Color.White);
			spriteBatch.End();
		}
		if (game.gameState == CustomPhysicsGame.GameState.Debut && game.scoreP == CustomPhysicsGame.ScoreP.vu)
		{
			if (!game.menu.Sortir)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(pressA, new Vector2(128f, 72f), Color.White);
				spriteBatch.End();
			}
			if (game.menu.Sortir)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(VoulezQuit, new Vector2(170f, 72f), Color.White);
				spriteBatch.End();
			}
			if (Guide.IsTrialMode)
			{
				if (game.string1 == "0" || game.string1 == "2")
				{
					spriteBatch.Begin();
					spriteBatch.Draw(Buy, new Vector2(970f, 80f), Color.White);
					spriteBatch.End();
				}
				else
				{
					spriteBatch.Begin();
					spriteBatch.Draw(Buy, new Vector2(1020f, 80f), Color.White);
					spriteBatch.End();
				}
			}
		}
		if (game.gameState != CustomPhysicsGame.GameState.Menu || game.scoreP != CustomPhysicsGame.ScoreP.vu)
		{
			return;
		}
		spriteBatch.Begin();
		spriteBatch.Draw(pressB, new Vector2(128f, 72f), Color.White);
		spriteBatch.End();
		if (Guide.IsTrialMode)
		{
			if (game.string1 == "0" || game.string1 == "2")
			{
				spriteBatch.Begin();
				spriteBatch.Draw(Buy, new Vector2(970f, 80f), Color.White);
				spriteBatch.End();
			}
			else
			{
				spriteBatch.Begin();
				spriteBatch.Draw(Buy, new Vector2(1020f, 80f), Color.White);
				spriteBatch.End();
			}
		}
	}
}
