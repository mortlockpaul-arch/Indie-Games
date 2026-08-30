using System.Collections.Generic;
using System.IO;
using System.Threading;
using IMAK3Z0MB1EGAEM.audio;
using IMAK3Z0MB1EGAEM.director;
using IMAK3Z0MB1EGAEM.hud;
using IMAK3Z0MB1EGAEM.menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SheetEdit.TextureSheet;
using Viking_x86.character;
using Viking_x86.director;
using Viking_x86.particles;
using Viking_x86.vikinggame;
using Viking_x86.vikinggame.hud;
using Viking_x86.world;

namespace Viking_x86;

public class VikingGame
{
	public const int MONSTER_IDX = 2;

	public static Dictionary<string, XTexture> textures;

	public float initFrame;

	public bool paused;

	public CharacterMgr charMgr;

	public World world;

	public ParticleMgr pMgr;

	public Texture2D spritesTex;

	public Viking_x86.vikinggame.SpawnMgr spawnMgr;

	public Texture2D grassTex;

	public Texture2D blueTex;

	public Texture2D cityTex;

	public Texture2D grayTex;

	public Texture2D heartTex;

	public Texture2D urbanTex;

	public Texture2D[] punkTex;

	public Texture2D atmosTex;

	public Texture2D moonTex;

	public VikingHUD hud;

	public static int[] mainPlayerIdx = new int[2] { -1, -1 };

	public VikingDirector vikingDirector;

	private ContentManager Content;

	private bool loadComplete;

	private float loadFrame;

	public VikingGame(ContentManager Content)
	{
		this.Content = Content;
	}

	public void Update(GameTime gameTime)
	{
		if (!loadComplete)
		{
			loadFrame += Game1.frameTime;
			Game1.loader.Update();
			return;
		}
		if (initFrame > 0f)
		{
			initFrame -= Game1.frameTime;
		}
		switch (GameState.state)
		{
		case GameState.State.VikingMenu:
			Menu.Update();
			if (!HUD.playersInited)
			{
				HUD.InitPlayers();
				HUD.playersInited = true;
			}
			break;
		case GameState.State.VikingPlaying:
		{
			if (paused)
			{
				HUD.pauseMenu.Update(HUD.pauseOwner);
				break;
			}
			vikingDirector.Update();
			charMgr.Update();
			world.Update();
			pMgr.Update();
			spawnMgr.Update();
			hud.Update();
			GamePad.GetState(PlayerIndex.One);
			Vector2 vector = default(Vector2);
			float num = 0f;
			for (int i = 0; i < 2; i++)
			{
				if (charMgr.character[i].exists && charMgr.character[i].nameIn <= 0)
				{
					vector += charMgr.character[i].loc + new Vector2(0f, -100f);
					num++;
				}
			}
			vector /= num;
			if (num > 0f)
			{
				float num2 = 1.82f;
				if (charMgr.moon.active && charMgr.moon.GetDif() >= 500f)
				{
					num2 = 1.43f;
					float num3 = 1.82f - VScroll.zoom;
					vector.Y -= 200f * num3;
				}
				if (VScroll.zoom < num2)
				{
					VScroll.zoom += Game1.frameTime * 0.05f;
					if (VScroll.zoom > num2)
					{
						VScroll.zoom = num2;
					}
				}
				else if (VScroll.zoom > num2)
				{
					VScroll.zoom -= Game1.frameTime * 0.05f;
					if (VScroll.zoom < num2)
					{
						VScroll.zoom = num2;
					}
				}
				if (VScroll.scroll.X < world.towerX - 50f)
				{
					VScroll.angle = (world.towerX - 50f - VScroll.scroll.X) / -4000f;
					vector.Y += (world.towerX - 50f - VScroll.scroll.X) * 0.12f;
				}
				else
				{
					VScroll.angle += (world.goalRotation - VScroll.angle) * Game1.frameTime * 0.1f;
				}
				VScroll.scroll += (vector - VScroll.scroll) * Game1.frameTime * 10f;
				if (VScroll.scroll.X < 100f)
				{
					VScroll.scroll.X = 100f;
				}
				VikingQuake.Update();
			}
			if (VScroll.zoom < 0.1f)
			{
				VScroll.zoom = 0.1f;
			}
			VScroll.Bake();
			if (!HUD.playersInited)
			{
				HUD.InitPlayers();
				HUD.playersInited = true;
			}
			if (!charMgr.character[0].exists && !charMgr.character[1].exists)
			{
				GameState.state = GameState.State.VikingMenu;
				Menu.Reset();
				Music.Stop();
			}
			break;
		}
		}
	}

	public void Draw(GraphicsDevice dev)
	{
		dev.Clear(Color.Black);
		if (!loadComplete)
		{
			SpriteTools.BeginAdditive();
			Menu.DrawVikingMenu((int)(loadFrame * 0.5f));
			SpriteTools.End();
			Game1.loader.Draw(100f);
			return;
		}
		switch (GameState.state)
		{
		case GameState.State.VikingMenu:
			SpriteTools.BeginAdditive();
			Menu.Draw();
			SpriteTools.End();
			break;
		case GameState.State.VikingPlaying:
			SpriteTools.BeginAlpha();
			pMgr.GetCount();
			world.DrawBack();
			charMgr.Draw();
			pMgr.Draw(alpha: false);
			SpriteTools.End();
			SpriteTools.BeginAdditive();
			pMgr.Draw(alpha: true);
			SpriteTools.End();
			SpriteTools.BeginAlpha();
			world.Draw();
			SpriteTools.End();
			SpriteTools.BeginAdditive();
			hud.Draw();
			HUD.Draw();
			SpriteTools.End();
			if (initFrame > 0f)
			{
				SpriteTools.BeginAlpha();
				SpriteTools.sprite.Draw(Game1.nullTex, new Rectangle(0, 0, (int)VScroll.screenSize.X, (int)VScroll.screenSize.Y), new Color(0f, 0f, 0f, initFrame));
				SpriteTools.End();
			}
			break;
		}
	}

	internal void Init()
	{
		loadComplete = false;
		loadFrame = 0f;
		Thread thread = new Thread(ThreadedLoad);
		thread.Start();
	}

	public void ThreadedLoad()
	{
		initFrame = 1f;
		textures = new Dictionary<string, XTexture>();
		DirectoryInfo directoryInfo = new DirectoryInfo("gfx");
		FileInfo[] files = directoryInfo.GetFiles("*.zsx");
		FileInfo[] array = files;
		foreach (FileInfo fileInfo in array)
		{
			string text = fileInfo.Name.Substring(0, fileInfo.Name.Length - 4);
			textures.Add(text, new XTexture(Content, "gfx/" + text));
		}
		CharDefMgr.Initialize();
		vikingDirector = new VikingDirector();
		charMgr = new CharacterMgr();
		world = new World();
		pMgr = new ParticleMgr();
		spritesTex = Content.Load<Texture2D>("gfx/sprites");
		grassTex = Content.Load<Texture2D>("gfx/grass");
		blueTex = Content.Load<Texture2D>("gfx/blue");
		cityTex = Content.Load<Texture2D>("gfx/city");
		grayTex = Content.Load<Texture2D>("gfx/gray");
		heartTex = Content.Load<Texture2D>("gfx/hearts");
		urbanTex = Content.Load<Texture2D>("gfx/urban");
		atmosTex = Content.Load<Texture2D>("gfx/atmos");
		moonTex = Content.Load<Texture2D>("gfx/moon");
		punkTex = new Texture2D[2]
		{
			Content.Load<Texture2D>("gfx/punk"),
			Content.Load<Texture2D>("gfx/punk2")
		};
		hud = new VikingHUD();
		spawnMgr = new Viking_x86.vikinggame.SpawnMgr();
		Menu.Reset();
		Music.Init(Content);
		loadComplete = true;
	}

	internal void Play()
	{
		pMgr.Reset();
		charMgr.Reset();
		world.Reset();
		paused = false;
		VScroll.zoom = 1.5f;
		VScroll.scroll = new Vector2(100f, 580f);
		vikingDirector.Init();
		TimeMgr.time = 1;
		TimeMgr.CurTMgr().playMode = BaseTimeMgr.PlayMode.Stopped;
		Music.Stop();
		TimeMgr.CurTMgr().playNum = 0;
		GameState.state = GameState.State.VikingPlaying;
	}
}
