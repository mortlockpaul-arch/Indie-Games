using System.Threading;
using IMAK3Z0MB1EGAEM.audio;
using IMAK3Z0MB1EGAEM.character;
using IMAK3Z0MB1EGAEM.director;
using IMAK3Z0MB1EGAEM.hud;
using IMAK3Z0MB1EGAEM.map;
using IMAK3Z0MB1EGAEM.menu;
using IMAK3Z0MB1EGAEM.particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Viking_x86;
using Viking_x86.director;
using Viking_x86.zomb1es.endless;

namespace IMAK3Z0MB1EGAEM;

public class ZombieGame
{
	public static Texture2D spritesTex;

	public static Texture2D grassTex;

	public static Texture2D skaTex;

	public static Texture2D spaceTex;

	public static Texture2D concreteTex;

	public static Texture2D gridTex;

	public static Texture2D fireTex;

	public static Texture2D nekoTex;

	public static Texture2D psychoNekoTex;

	private static EndlessUpdate endlessUpdate;

	public static int mainPlayerIndex = -1;

	private ContentManager Content;

	private bool loadcomplete;

	public ZombieGame(ContentManager Content)
	{
		this.Content = Content;
	}

	public static EndlessNode GetEndlessRoom(int x, int y)
	{
		return endlessUpdate.GetRoom(x, y);
	}

	public static int GetEndlessRound()
	{
		return endlessUpdate.round;
	}

	public void Update(GameTime gameTime)
	{
		_ = Menu.needsQuit;
		if (!loadcomplete)
		{
			Game1.loader.Update();
			return;
		}
		FMan.frameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
		if (mainPlayerIndex < 0)
		{
			for (int i = 0; i < 4; i++)
			{
				if (Menu.playerState[i] != Menu.PlayerState.Out)
				{
					mainPlayerIndex = i;
				}
			}
		}
		switch (GameState.state)
		{
		case GameState.State.ZombiesMenu:
		case GameState.State.EndlessZombiesMenu:
			Menu.Update();
			break;
		case GameState.State.ZombiesPlaying:
		case GameState.State.EndlessZombiesPlaying:
		{
			bool flag = GameState.state == GameState.State.EndlessZombiesPlaying;
			if (flag)
			{
				MapMan.map.Update(gameTime);
			}
			if (TimeMgr.ZombieTMgr().playMode == BaseTimeMgr.PlayMode.Paused)
			{
				HUD.pauseMenu.Update(HUD.pauseOwner);
				break;
			}
			CharMan.Update();
			ParticleMan.Update();
			CamMan.Update();
			if (flag)
			{
				endlessUpdate.Update();
				Music.Update(2);
			}
			else
			{
				Music.Update(0);
				TimeMgr.ZombieTMgr().Update();
				MapMan.Update();
			}
			bool flag2 = true;
			for (int j = 0; j < CharMan.hero.Length; j++)
			{
				if (CharMan.hero[j].exists)
				{
					flag2 = false;
				}
			}
			if (flag2)
			{
				if (flag)
				{
					GameState.state = GameState.State.EndlessZombiesMenu;
				}
				else
				{
					GameState.state = GameState.State.ZombiesMenu;
				}
				for (int k = 0; k < 4; k++)
				{
					Menu.playerState[k] = Menu.PlayerState.Out;
				}
				Menu.timeGo = 0f;
				Music.Stop();
				Menu.scoreMode = 0;
			}
			break;
		}
		}
		if (!HUD.playersInited && Gamer.SignedInGamers.Count > 0)
		{
			HUD.InitPlayers();
			HUD.playersInited = true;
		}
	}

	public void Draw(GameTime gameTime, GraphicsDevice dev)
	{
		dev.Clear(Color.Black);
		if (!loadcomplete)
		{
			Game1.loader.Draw();
			return;
		}
		switch (GameState.state)
		{
		case GameState.State.ZombiesMenu:
		case GameState.State.EndlessZombiesMenu:
			SpriteTools.BeginAdditive();
			Menu.Draw();
			SpriteTools.End();
			break;
		case GameState.State.ZombiesPlaying:
		case GameState.State.EndlessZombiesPlaying:
		{
			bool flag = GameState.state == GameState.State.EndlessZombiesPlaying;
			if (flag)
			{
				MapMan.DrawClassicMapFloor();
			}
			else
			{
				MapMan.DrawMap();
			}
			SpriteTools.BeginAlpha();
			CharMan.Draw();
			ParticleMan.Draw(alpha: false);
			DrawDebug();
			SpriteTools.End();
			SpriteTools.BeginAdditive();
			if (CharMan.areAlphas)
			{
				CharMan.DrawAlphas();
			}
			ParticleMan.Draw(alpha: true);
			for (int i = 0; i < 4; i++)
			{
				if (!CharMan.hero[i].exists || !(CharMan.hero[i].spawnFrame > 0f))
				{
					continue;
				}
				for (int j = 0; j < 2; j++)
				{
					float num;
					for (num = CharMan.hero[i].spawnFrame + (float)j * 0.2f; num > 0.4f; num -= 0.4f)
					{
					}
					CharMan.DrawUnderglow(CharMan.hero[i].loc, i, num);
				}
			}
			if (flag)
			{
				SpriteTools.End();
				MapMan.DrawClassicMapOverlay();
				SpriteTools.BeginAdditive();
			}
			else
			{
				MapMan.DrawOverMap();
			}
			HUD.Draw();
			SpriteTools.End();
			break;
		}
		case GameState.State.VikingMenu:
		case GameState.State.VikingPlaying:
			break;
		}
	}

	private void DrawDebug()
	{
	}

	internal void Init()
	{
		Init(endless: false);
	}

	internal void Init(bool endless)
	{
		loadcomplete = false;
		if (endless)
		{
			Thread thread = new Thread(ThreadedLoadEndless);
			thread.Start();
		}
		else
		{
			Thread thread2 = new Thread(ThreadedLoadNormal);
			thread2.Start();
		}
	}

	public void ThreadedLoadNormal()
	{
		MapMan.Init();
		ScrollMan.screenSize = new Vector2(1280f, 720f);
		SharedLoad();
		loadcomplete = true;
	}

	public void ThreadedLoadEndless()
	{
		MapMan.Init();
		ScrollMan.screenSize = new Vector2(1280f, 720f);
		endlessUpdate = new EndlessUpdate();
		MapMan.ReadMap(Content);
		MapMan.mapSize = new Vector2(3686.4001f, 2764.8f);
		SharedLoad();
		loadcomplete = true;
	}

	private void SharedLoad()
	{
		ParticleMan.Init();
		spritesTex = Content.Load<Texture2D>("gfx/zombie/sprites");
		grassTex = Content.Load<Texture2D>("gfx/zombie/grass");
		skaTex = Content.Load<Texture2D>("gfx/zombie/ska");
		spaceTex = Content.Load<Texture2D>("gfx/zombie/space");
		concreteTex = Content.Load<Texture2D>("gfx/zombie/concrete");
		gridTex = Content.Load<Texture2D>("gfx/zombie/grid");
		fireTex = Content.Load<Texture2D>("gfx/zombie/fire");
		nekoTex = Content.Load<Texture2D>("gfx/zombie/neko");
		psychoNekoTex = Content.Load<Texture2D>("gfx/zombie/psychoneko");
		Menu.Reset();
	}

	internal void Play(bool endless)
	{
		for (int i = 0; i < CharMan.monster.Length; i++)
		{
			CharMan.monster[i].exists = false;
		}
		for (int j = 0; j < ParticleMan.particle.Length; j++)
		{
			ParticleMan.particle[j].exists = false;
		}
		for (int k = 0; k < 4; k++)
		{
			CharMan.hero[k].exists = false;
			if (Menu.playerState[k] == Menu.PlayerState.Ready)
			{
				CharMan.hero[k].Init(MapMan.mapSize / 2f + Rand.GetRandomVec2(-200f, 200f, -200f, 200f));
			}
		}
		if (endless)
		{
			GameState.state = GameState.State.EndlessZombiesPlaying;
			endlessUpdate.Init();
			Music.song = 2;
			Music.Start();
		}
		else
		{
			TimeMgr.ZombieTMgr().phase = 0;
			TimeMgr.time = 0;
			TimeMgr.ZombieTMgr().playNum = 0;
			GameState.state = GameState.State.ZombiesPlaying;
			Music.song = 0;
			Music.Start();
		}
	}

	internal void Play()
	{
		Play(endless: false);
	}

	internal static void UpdateEndlessScroll(Vector2 scrollGoal)
	{
		endlessUpdate.UpdateScroll(scrollGoal);
	}

	internal static int GetEndlessRoomX()
	{
		return endlessUpdate.x_room;
	}

	internal static int GetEndlessRoomY()
	{
		return endlessUpdate.y_room;
	}

	internal static Vector2 GetEndlessRoomTL()
	{
		return new Vector2((float)endlessUpdate.x_room * 64f * 16f * 1.2f, (float)endlessUpdate.y_room * 64f * 12f * 1.2f) + new Vector2(64f, 64f);
	}

	internal static Vector2 GetEndlessRoomBR()
	{
		return new Vector2((float)(endlessUpdate.x_room + 1) * 64f * 16f * 1.2f, (float)(endlessUpdate.y_room + 1) * 64f * 12f * 1.2f) + new Vector2(64f, 64f);
	}
}
