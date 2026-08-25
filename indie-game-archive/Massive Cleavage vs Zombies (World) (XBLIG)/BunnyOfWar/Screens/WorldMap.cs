using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;

namespace BunnyOfWar.Screens;

public class WorldMap
{
	private static bool isShowingPolishedMap = true;

	private List<Texture2D> levelImages = new List<Texture2D>();

	private Texture2D background;

	private Texture2D backgroundTrial;

	private Texture2D cursor;

	private string[,] map = new string[5, 9]
	{
		{ "Quit", "", "PvP", "", "", "", "", "", "" },
		{ "1", "2", "3", "4", "5", "6", "7", "8", "9" },
		{ "", "", "", "", "", "", "", "", "10" },
		{ "", "", "", "", "", "", "", "", "11" },
		{ "", "19", "18", "17", "16", "15", "14", "13", "12" }
	};

	public int x;

	public int y = 1;

	private PacketWriter packetWriter = new PacketWriter();

	private PacketReader packetReader = new PacketReader();

	private static NetworkSession networkSession => Networking.networkSession;

	public WorldMap()
	{
		Load(RandomStaticGlobals.Content);
	}

	public void Draw()
	{
		if (GraphicsManager.messages != null && GraphicsManager.messages.Count > 0)
		{
			return;
		}
		if (isShowingPolishedMap)
		{
			Texture2D texture2D = ((RandomStaticGlobals.currentlySelectedLevel > 19) ? GraphicsManager.LoadTexture("screens/worldmap/" + (RandomStaticGlobals.currentlySelectedLevel - 19), cacheResult: true) : GraphicsManager.LoadTexture("screens/worldmap/" + RandomStaticGlobals.currentlySelectedLevel, cacheResult: true));
			if (texture2D == null)
			{
				texture2D = GraphicsManager.imgBlack;
			}
			if (!RandomStaticGlobals.IsTrial())
			{
				GraphicsManager.spriteBatch.Draw(background, new Rectangle(0, 0, 1920, 1080), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
			}
			else
			{
				GraphicsManager.spriteBatch.Draw(backgroundTrial, new Rectangle(0, 0, 1920, 1080), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
			}
			GraphicsManager.spriteBatch.Draw(texture2D, new Rectangle(366, 188, 1200, 675), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, Definitions.LayerDepthForGround);
			if (!RandomStaticGlobals.IsTrial())
			{
				GraphicsManager.DrawString(620, 855, "Level " + RandomStaticGlobals.currentlySelectedLevel + " / 38", Color.WhiteSmoke, GraphicsManager.fontMedium);
			}
			else
			{
				GraphicsManager.DrawString(700, 855, "Level " + RandomStaticGlobals.currentlySelectedLevel + " / 38", Color.WhiteSmoke, GraphicsManager.fontMedium);
			}
		}
		else
		{
			GraphicsManager.Draw(background, new Rectangle(0, 0, 1920, 1080), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
			int num = 100;
			int num2 = 100;
			if (y > 0)
			{
				num2 = y * 200 + 100;
			}
			if (x > 0)
			{
				num = x * 200 + 100;
			}
			GraphicsManager.Draw(cursor, new Vector2(num, num2), null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 1f);
		}
	}

	public void ProcessInput()
	{
		if (RandomStaticGlobals.currentlySelectedLevel < 0)
		{
			RandomStaticGlobals.currentlySelectedLevel = 1;
		}
		if (RandomStaticGlobals.currentlySelectedLevel >= LevelManager.mapForBabewatch.Length)
		{
			RandomStaticGlobals.currentlySelectedLevel = LevelManager.mapForBabewatch.Length - 1;
		}
		List<FighterObject> humanPlayers = FighterManager.getHumanPlayers(onlyLiving: false, canBeDying: true);
		if (humanPlayers.Count == 0)
		{
			ScreenManager.ShowMainMenu();
		}
		for (int i = 0; i < humanPlayers.Count; i++)
		{
			if (!FighterManager.humanPlayers[i].PROPERTIES.PlayerIndexControllerNumber.HasValue || !FighterManager.humanPlayers[i].PROPERTIES.isLocal)
			{
				continue;
			}
			InputFromAnywhere playerInput = InputManager.GetPlayerInput(FighterManager.humanPlayers[i].PROPERTIES.PlayerIndexControllerNumber.Value, ref FighterManager.humanPlayers[i].PROPERTIES.previousGamePadState, ref InputManager.previousKeyboardStateMenu);
			if (GraphicsManager.messages != null && GraphicsManager.messages.Count > 0)
			{
				if (playerInput.B_pressed)
				{
					GraphicsManager.messages.Clear();
				}
				break;
			}
			if (FighterManager.humanPlayers[i].PROPERTIES.previousGamePadState.HasValue)
			{
				FigureOutInput(playerInput, FighterManager.humanPlayers[i].PROPERTIES.PlayerIndexControllerNumber.Value);
			}
		}
	}

	private void FigureOutPhoneInput(int x, int y)
	{
	}

	private void FigureOutInput(InputFromAnywhere anywhereInput, PlayerIndex? pi)
	{
		if (!pi.HasValue || Guide.IsVisible || !isShowingPolishedMap)
		{
			return;
		}
		if (anywhereInput.LEFT_pressed)
		{
			int num = RandomStaticGlobals.currentlySelectedLevel - 1;
			if (isThisMoveAllowed(RandomStaticGlobals.currentlySelectedLevel.ToString(), num.ToString()))
			{
				RandomStaticGlobals.currentlySelectedLevel--;
				UpdateNetworkGamers();
			}
		}
		if (anywhereInput.RIGHT_pressed)
		{
			int num2 = RandomStaticGlobals.currentlySelectedLevel + 1;
			if (isThisMoveAllowed(RandomStaticGlobals.currentlySelectedLevel.ToString(), num2.ToString()))
			{
				RandomStaticGlobals.currentlySelectedLevel++;
				UpdateNetworkGamers();
			}
			else
			{
				RandomStaticGlobals.BuyMe(pi.Value);
			}
		}
		if (RandomStaticGlobals.currentlySelectedLevel <= 0)
		{
			RandomStaticGlobals.currentlySelectedLevel = 1;
		}
		if (anywhereInput.X_pressed)
		{
			RandomStaticGlobals.BuyMe(pi.Value);
		}
		if (anywhereInput.SELECT_pressed)
		{
			ScreenManager.ShowMainMenu();
		}
		if (anywhereInput.B_pressed)
		{
			LevelManager.isCurrentLevelActuallyALevel = false;
			LevelManager.LoadLevel("MoreFrom.lvl", isPvP: false);
		}
		if (anywhereInput.A_pressed || anywhereInput.START_pressed)
		{
			SoundManager.PlayMenuClick();
			SendPackets(NetworkGameplayManager.PacketType.SelectedALevel, RandomStaticGlobals.currentlySelectedLevel, null);
			LevelManager.currentLevel = RandomStaticGlobals.currentlySelectedLevel;
			LevelManager.isCurrentLevelActuallyALevel = true;
			if (!RandomStaticGlobals.IsTrial())
			{
				LevelManager.LoadLevel(LevelManager.mapForBabewatch[RandomStaticGlobals.currentlySelectedLevel], isPvP: false);
			}
			else
			{
				LevelManager.LoadLevel(LevelManager.mapForBabewatchTRIAL[RandomStaticGlobals.currentlySelectedLevel], isPvP: false);
			}
		}
		if (anywhereInput.SELECT_pressed)
		{
			ScreenManager.ShowMainMenu();
		}
		if (anywhereInput.Y_pressed)
		{
			ScreenManager.ShowCredits();
		}
	}

	private bool isThisMoveAllowed(string currentSpot, string futureSpot)
	{
		if (RandomStaticGlobals.IsTrial())
		{
			switch (futureSpot)
			{
			case "0":
			case "1":
			case "2":
			case "3":
			case "4":
			case "5":
			case "6":
				if (RandomStaticGlobals.GameProgress.ContainsKey(currentSpot))
				{
					return true;
				}
				if (RandomStaticGlobals.GameProgress.ContainsKey(futureSpot))
				{
					return true;
				}
				return false;
			case "-1":
				return false;
			default:
				return false;
			}
		}
		if (futureSpot == "0")
		{
			return false;
		}
		if (int.Parse(futureSpot) >= LevelManager.mapForBabewatch.Length)
		{
			return false;
		}
		try
		{
			if (RandomStaticGlobals.currentlySelectedLevel > int.Parse(futureSpot))
			{
				return true;
			}
			if (int.Parse(currentSpot) > int.Parse(futureSpot))
			{
				return true;
			}
		}
		catch (Exception)
		{
		}
		if (RandomStaticGlobals.GameProgress.ContainsKey(currentSpot))
		{
			return true;
		}
		if (RandomStaticGlobals.GameProgress.ContainsKey(futureSpot))
		{
			return true;
		}
		switch (futureSpot)
		{
		case "Quit":
		case "Store":
		case "Home":
		case "x":
		case "1":
		case "PvP":
			return true;
		default:
			return false;
		}
	}

	private void moveUp()
	{
		string currentSpot = map[y, x];
		y--;
		if (y < 0 || map[y, x] == "")
		{
			y++;
			return;
		}
		string futureSpot = map[y, x];
		if (!isThisMoveAllowed(currentSpot, futureSpot))
		{
			y++;
		}
		else
		{
			UpdateNetworkGamers();
		}
	}

	private void moveDown()
	{
		string currentSpot = map[y, x];
		y++;
		if (y > 10 || map[y, x] == "")
		{
			y--;
			return;
		}
		string futureSpot = map[y, x];
		if (!isThisMoveAllowed(currentSpot, futureSpot))
		{
			y--;
		}
		else
		{
			UpdateNetworkGamers();
		}
	}

	private void moveRight()
	{
		string currentSpot = map[y, x];
		x++;
		if (x > 20 || map[y, x] == "")
		{
			x--;
			return;
		}
		string futureSpot = map[y, x];
		if (!isThisMoveAllowed(currentSpot, futureSpot))
		{
			x--;
		}
		else
		{
			UpdateNetworkGamers();
		}
	}

	private void moveLeft()
	{
		string currentSpot = map[y, x];
		x--;
		if (x < 0 || map[y, x] == "")
		{
			x++;
			return;
		}
		string futureSpot = map[y, x];
		if (!isThisMoveAllowed(currentSpot, futureSpot))
		{
			x++;
		}
		else
		{
			UpdateNetworkGamers();
		}
	}

	private void selectedSomething()
	{
		if (map[y, x] == "")
		{
			return;
		}
		try
		{
			int levelNumber = int.Parse(map[y, x]);
			SendPackets(NetworkGameplayManager.PacketType.SelectedALevel, RandomStaticGlobals.currentlySelectedLevel, null);
			SoundManager.PlayMenuClick();
			LevelManager.LoadLevel(levelNumber);
		}
		catch (Exception)
		{
		}
	}

	public void Load(ContentManager Content)
	{
		if (RandomStaticGlobals.IsTrial())
		{
			backgroundTrial = GraphicsManager.LoadTexture("screens/WorldMapTRIAL");
		}
		background = GraphicsManager.LoadTexture("screens/WorldMap");
		cursor = GraphicsManager.LoadTexture("screens/cursor");
		Clear();
	}

	public void Clear()
	{
		if (RandomStaticGlobals.IsTrial())
		{
			if (RandomStaticGlobals.currentlySelectedLevel <= 0)
			{
				RandomStaticGlobals.currentlySelectedLevel = 1;
			}
			if (RandomStaticGlobals.currentlySelectedLevel > 6)
			{
				RandomStaticGlobals.currentlySelectedLevel = 6;
			}
		}
		if (RandomStaticGlobals.currentlySelectedLevel != -1)
		{
			return;
		}
		RandomStaticGlobals.currentlySelectedLevel = 1;
		foreach (string key in RandomStaticGlobals.GameProgress.Keys)
		{
			try
			{
				int num = int.Parse(RandomStaticGlobals.GameProgress[key].ToString());
				if (num > RandomStaticGlobals.currentlySelectedLevel)
				{
					RandomStaticGlobals.currentlySelectedLevel = num;
				}
			}
			catch (Exception)
			{
			}
		}
		if (isThisMoveAllowed(RandomStaticGlobals.currentlySelectedLevel.ToString(), (RandomStaticGlobals.currentlySelectedLevel + 1).ToString()))
		{
			RandomStaticGlobals.currentlySelectedLevel++;
		}
	}

	private void UpdateNetworkGamers()
	{
		SendPackets(NetworkGameplayManager.PacketType.WorldMapPosition, RandomStaticGlobals.currentlySelectedLevel, 0);
	}

	public void SendPackets(NetworkGameplayManager.PacketType pt, int? a, int? b)
	{
		if (Networking.NullCheckSucceed())
		{
			packetWriter.Write((byte)pt);
			if (a.HasValue)
			{
				packetWriter.Write(a.Value);
			}
			if (b.HasValue)
			{
				packetWriter.Write(b.Value);
			}
			networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.ReliableInOrder);
		}
	}
}
