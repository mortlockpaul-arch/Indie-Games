using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;

namespace TechArts
{
	public class GameEngine : Game
	{
		public const int SCREEN_WIDTH = 1280;

		public const int SCREEN_HEIGHT = 720;

		private const int TRIALTIME = 18000;

		public static GameEngine core;

		public GraphicsDeviceManager graphics;

		public SpriteBatch spriteBatch;

		protected TaskManager tasks;

		protected int updatetime;

		public int vcount;

		public SpriteFont font;

		public Random rnd;

		public DebugConsole console0;

		public DebugConsole console1;

		public ParticleManager particles;

		public Fader fader;

		private Queue<byte> PadQueue;

		private byte PadState;

		private bool bRecord;

		private bool bReplay;

		private bool bBGMStopped;

		private PlayerIndex playctrIndex;

		private Texture2D pixel;

		private bool bGuideVisible;

		private bool bOffered;

		public Rectangle SafeArea
		{
			get
			{
				return base.GraphicsDevice.Viewport.TitleSafeArea;
			}
		}

		public void BeginRecord()
		{
			bReplay = false;
			PadQueue.Clear();
			rnd = new Random(1973);
			bRecord = true;
		}

		public void EndRecord()
		{
			bRecord = false;
		}

		public void BeginReplay(string filename)
		{
			bRecord = false;
			PadQueue.Clear();
			string path = Path.Combine(StorageContainer.TitleLocation, "Content/" + filename);
			FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read);
			while (true)
			{
				int num = fileStream.ReadByte();
				if (num < 0)
				{
					break;
				}
				PadQueue.Enqueue((byte)num);
			}
			fileStream.Close();
			rnd = new Random(1973);
			bReplay = true;
		}

		public void EndReplay()
		{
			PadQueue.Clear();
			bReplay = false;
		}

		public bool InReplay()
		{
			return bReplay;
		}

		public GameEngine()
		{
			core = this;
			PadQueue = new Queue<byte>();
			base.Components.Add(new GamerServicesComponent(this));
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = 1280;
			graphics.PreferredBackBufferHeight = 720;
			base.Content.RootDirectory = "Content";
			tasks = new TaskManager();
			updatetime = 0;
			vcount = 0;
			bRecord = (bReplay = false);
			rnd = new Random(1973);
			bGuideVisible = false;
			playctrIndex = PlayerIndex.One;
			bOffered = false;
		}

		protected override void Initialize()
		{
			base.Initialize();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(base.GraphicsDevice);
			font = base.Content.Load<SpriteFont>("Font/Hud");
			console0 = new DebugConsole(spriteBatch, font, new Vector2(4f, 0f), 30, Color.Black, 10);
			console1 = new DebugConsole(spriteBatch, font, new Vector2(4f, 512f), 20, Color.Black, 10);
			particles = new ParticleManager(base.Content.Load<Texture2D>("Sprite/Game/font_maru"));
			pixel = base.Content.Load<Texture2D>("Sprite/Game/pixel");
			fader = new Fader(base.Content.Load<Texture2D>("Sprite/Game/White"));
		}

		protected override void UnloadContent()
		{
		}

		protected void PadKeyScan()
		{
			if (bReplay)
			{
				if (PadQueue.Count > 0)
				{
					PadState = PadQueue.Dequeue();
				}
				else
				{
					PadState = 0;
				}
				return;
			}
			PadState = 0;
			GamePadState state = GamePad.GetState(playctrIndex);
			if (state.IsButtonDown(Buttons.DPadLeft))
			{
				PadState |= 1;
			}
			if (state.IsButtonDown(Buttons.LeftThumbstickLeft))
			{
				PadState |= 1;
			}
			if (state.IsButtonDown(Buttons.DPadRight))
			{
				PadState |= 2;
			}
			if (state.IsButtonDown(Buttons.LeftThumbstickRight))
			{
				PadState |= 2;
			}
			if (state.IsButtonDown(Buttons.DPadUp))
			{
				PadState |= 4;
			}
			if (state.IsButtonDown(Buttons.LeftThumbstickUp))
			{
				PadState |= 4;
			}
			if (state.IsButtonDown(Buttons.DPadDown))
			{
				PadState |= 8;
			}
			if (state.IsButtonDown(Buttons.LeftThumbstickDown))
			{
				PadState |= 8;
			}
			if (state.IsButtonDown(Buttons.LeftShoulder))
			{
				PadState |= 16;
			}
			if (state.IsButtonDown(Buttons.LeftTrigger))
			{
				PadState |= 16;
			}
			if (state.IsButtonDown(Buttons.RightShoulder))
			{
				PadState |= 32;
			}
			if (state.IsButtonDown(Buttons.RightTrigger))
			{
				PadState |= 32;
			}
			if (state.IsButtonDown(Buttons.A))
			{
				PadState |= 64;
			}
			if (state.IsButtonDown(Buttons.Start))
			{
				PadState |= 128;
			}
		}

		public bool IsPressed_A_Ctr()
		{
			for (PlayerIndex playerIndex = PlayerIndex.One; playerIndex <= PlayerIndex.Four; playerIndex++)
			{
				if (GamePad.GetState(playerIndex).Buttons.A == ButtonState.Pressed)
				{
					playctrIndex = playerIndex;
					return true;
				}
			}
			return false;
		}

		public bool IsPressed_Left()
		{
			return (PadState & 1) != 0;
		}

		public bool IsPressed_Right()
		{
			return (PadState & 2) != 0;
		}

		public bool IsPressed_Up()
		{
			return (PadState & 4) != 0;
		}

		public bool IsPressed_Down()
		{
			return (PadState & 8) != 0;
		}

		public bool IsPressed_RotL()
		{
			return (PadState & 0x10) != 0;
		}

		public bool IsPressed_RotR()
		{
			return (PadState & 0x20) != 0;
		}

		public bool IsPressed_A()
		{
			return (PadState & 0x40) != 0;
		}

		public bool IsPressed_START()
		{
			return (PadState & 0x80) != 0;
		}

		public void FillRect(Rectangle r, Color c)
		{
			spriteBatch.Draw(pixel, r, c);
		}

		public void DrawString(string s, Vector2 p, Color c)
		{
			spriteBatch.DrawString(font, s, p, c);
		}

		public void DrawSprite(Texture2D img, Vector2 pos, Color col, float ang, float scl, float depth)
		{
			spriteBatch.Draw(img, pos, null, col, ang, new Vector2(img.Width / 2, img.Height / 2), scl, SpriteEffects.None, depth);
		}

		protected override void Update(GameTime gameTime)
		{
			if (!Guide.IsVisible)
			{
				if (Guide.IsTrialMode)
				{
					if (bOffered)
					{
						Exit();
					}
					else if (vcount >= 18000)
					{
						Guide.ShowMarketplace(PlayerIndex.One);
						bOffered = true;
					}
				}
				PadKeyScan();
				int num = gameTime.ElapsedGameTime.Milliseconds;
				if (bGuideVisible)
				{
					num = 16;
					bGuideVisible = false;
					if (!bBGMStopped)
					{
						MediaPlayer.Resume();
					}
				}
				else if (num >= 128)
				{
					num = 128;
				}
				updatetime += num;
				while (updatetime >= 16)
				{
					tasks.Update();
					particles.Update();
					updatetime -= 16;
					vcount++;
				}
			}
			else if (!bGuideVisible)
			{
				bGuideVisible = true;
				bBGMStopped = MediaPlayer.State == MediaState.Stopped;
				if (!bBGMStopped)
				{
					MediaPlayer.Pause();
				}
			}
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			base.GraphicsDevice.Clear(Color.White);
			spriteBatch.Begin();
			tasks.Draw();
			particles.Draw();
			fader.Draw();
			console0.Draw();
			console1.Draw();
			spriteBatch.End();
			base.Draw(gameTime);
		}
	}
}
