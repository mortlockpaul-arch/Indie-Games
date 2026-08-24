using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WorldAmbiance;

public class WorldAmbienceGame : Game
{
	private GraphicsDeviceManager graphics;

	private SpriteBatch spriteBatch;

	private Texture2D Background;

	private Texture2D Egypt;

	private Texture2D Hawaii;

	private Texture2D Ireland;

	private Texture2D Japan;

	private Texture2D Niagara;

	private Texture2D Paris;

	private Texture2D Phillippines;

	public Texture2D Menu;

	private Texture2D EgyptThumbnail;

	private Texture2D HawaiiThumbnail;

	private Texture2D IrelandThumbnail;

	private Texture2D JapanThumbnail;

	private Texture2D NiagaraThumbnail;

	private Texture2D ParisThumbnail;

	private Texture2D PhillippinesThumbnail;

	private Texture2D BuyMeScreen;

	private Texture2D StartScreen;

	private Texture2D black;

	private Texture2D white;

	private Texture2D downarrow;

	private Texture2D uparrow;

	private Texture2D ybutton;

	private Texture2D gray;

	private SpriteFont LargeFont;

	private SpriteFont SmallFont;

	private SpriteFont MenuFont;

	private SpriteFont SmallMenuFont;

	private SoundBank soundBank;

	private WaveBank waveBank;

	private AudioEngine engine;

	private Cue nowPlaying;

	private int SelectedWorld = 1;

	private int SelectedWorldChange;

	private float maxThumbstickTimer = 0.25f;

	private float leftThumbstickTimer;

	private bool goUp;

	private bool goDown;

	private bool drawTop1 = true;

	private bool drawTop2 = true;

	private bool drawInGameMenu;

	private bool drawStartMenu = true;

	private bool drawMainMenu;

	private bool ydown;

	private PlayerIndex _player;

	private bool drawBuyWarning;

	private bool drawPurchaseGuide;

	private bool adown;

	private bool bdown;

	private bool backDown;

	private bool drawBuyMe;

	private bool drawRed;

	private float FlashTimer;

	private int MainMenuSelected = 1;

	public WorldAmbienceGame()
	{
		graphics = new GraphicsDeviceManager(this);
		base.Content.RootDirectory = "Content";
		base.Components.Add(new GamerServicesComponent(this));
		graphics.PreferredBackBufferHeight = 720;
		graphics.PreferredBackBufferWidth = 1280;
		graphics.PreferMultiSampling = false;
	}

	protected override void Initialize()
	{
		engine = new AudioEngine("Content\\Audio\\WorldAmbianceSound.xgs");
		waveBank = new WaveBank(engine, "Content\\Audio\\Wave Bank.xwb");
		soundBank = new SoundBank(engine, "Content\\Audio\\Sound Bank.xsb");
		base.Initialize();
	}

	protected override void LoadContent()
	{
		spriteBatch = new SpriteBatch(base.GraphicsDevice);
		Egypt = base.Content.Load<Texture2D>("Pictures\\egypt");
		Phillippines = base.Content.Load<Texture2D>("Pictures\\phillippines");
		Hawaii = base.Content.Load<Texture2D>("Pictures\\hawaii");
		Ireland = base.Content.Load<Texture2D>("Pictures\\ireland");
		Japan = base.Content.Load<Texture2D>("Pictures\\japan");
		Niagara = base.Content.Load<Texture2D>("Pictures\\niagra falls");
		Paris = base.Content.Load<Texture2D>("Pictures\\paris");
		EgyptThumbnail = base.Content.Load<Texture2D>("Pictures\\egyptThumbnail");
		HawaiiThumbnail = base.Content.Load<Texture2D>("Pictures\\hawaiiThumbnail");
		IrelandThumbnail = base.Content.Load<Texture2D>("Pictures\\irelandThumbnail");
		JapanThumbnail = base.Content.Load<Texture2D>("Pictures\\japanThumbnail");
		NiagaraThumbnail = base.Content.Load<Texture2D>("Pictures\\niagra falls Thumbnail");
		ParisThumbnail = base.Content.Load<Texture2D>("Pictures\\parisThumbnail");
		PhillippinesThumbnail = base.Content.Load<Texture2D>("Pictures\\phillippinesThumbnail");
		StartScreen = base.Content.Load<Texture2D>("Menus\\startscreen");
		BuyMeScreen = base.Content.Load<Texture2D>("Menus\\buymescreen");
		LargeFont = base.Content.Load<SpriteFont>("LargeFont");
		SmallFont = base.Content.Load<SpriteFont>("SmallFont");
		MenuFont = base.Content.Load<SpriteFont>("MenuFont");
		SmallMenuFont = base.Content.Load<SpriteFont>("smallMenuFont");
		black = base.Content.Load<Texture2D>("Menus\\black");
		white = base.Content.Load<Texture2D>("Menus\\white");
		downarrow = base.Content.Load<Texture2D>("Menus\\downarrow");
		uparrow = base.Content.Load<Texture2D>("Menus\\uparrow");
		ybutton = base.Content.Load<Texture2D>("Menus\\YButton");
		gray = base.Content.Load<Texture2D>("Menus\\gray");
		Menu = base.Content.Load<Texture2D>("Menus\\Menuback");
		Background = Phillippines;
		nowPlaying = soundBank.GetCue("phillippines");
		nowPlaying.Play();
	}

	protected override void UnloadContent()
	{
	}

	private void leftThumbstickTimer_Elapsed()
	{
		if (drawInGameMenu || drawMainMenu)
		{
			SelectedWorld += SelectedWorldChange;
			if (SelectedWorld == 1)
			{
				drawTop1 = true;
			}
			else if (SelectedWorld == 2)
			{
				drawTop2 = true;
			}
			else if (SelectedWorld == 6)
			{
				drawTop1 = false;
			}
			else if (SelectedWorld == 7)
			{
				drawTop2 = false;
			}
			if (SelectedWorld < 1)
			{
				SelectedWorld = 1;
			}
			else if (SelectedWorld > 7)
			{
				SelectedWorld = 7;
			}
			else
			{
				soundBank.PlayCue("click");
			}
		}
	}

	protected override void Update(GameTime gameTime)
	{
		if (!nowPlaying.IsPlaying)
		{
			nowPlaying = soundBank.GetCue(nowPlaying.Name);
			nowPlaying.Play();
		}
		if (drawPurchaseGuide && !Guide.IsVisible)
		{
			drawPurchaseGuide = false;
			if (Gamer.SignedInGamers[_player] != null && Gamer.SignedInGamers[_player].Privileges.AllowPurchaseContent)
			{
				Guide.ShowMarketplace(_player);
				drawBuyWarning = false;
			}
			else
			{
				drawBuyWarning = true;
			}
		}
		else if (drawBuyWarning)
		{
			if (GamePad.GetState(_player).IsButtonDown(Buttons.A))
			{
				adown = true;
			}
			else if (GamePad.GetState(_player).IsButtonUp(Buttons.A) && adown)
			{
				adown = false;
				soundBank.PlayCue("click");
				ShowMarketPlace();
			}
			if (GamePad.GetState(_player).IsButtonDown(Buttons.B))
			{
				bdown = true;
			}
			else if (GamePad.GetState(_player).IsButtonUp(Buttons.B) && bdown)
			{
				bdown = false;
				soundBank.PlayCue("click");
				drawBuyWarning = false;
			}
		}
		else if (drawBuyMe)
		{
			if (!Guide.IsTrialMode)
			{
				drawBuyMe = false;
				drawStartMenu = true;
			}
			bool flag = false;
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
			{
				flag = true;
			}
			else if (GamePad.GetState(PlayerIndex.Two).Buttons.Back == ButtonState.Pressed)
			{
				flag = true;
			}
			else if (GamePad.GetState(PlayerIndex.Three).Buttons.Back == ButtonState.Pressed)
			{
				flag = true;
			}
			else if (GamePad.GetState(PlayerIndex.Four).Buttons.Back == ButtonState.Pressed)
			{
				flag = true;
			}
			if (flag && !backDown)
			{
				drawBuyMe = false;
				drawStartMenu = true;
				backDown = true;
			}
			else if (!flag && backDown)
			{
				backDown = false;
			}
			if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)
			{
				Exit();
			}
			else if (GamePad.GetState(PlayerIndex.Two).Buttons.Start == ButtonState.Pressed)
			{
				Exit();
			}
			else if (GamePad.GetState(PlayerIndex.Three).Buttons.Start == ButtonState.Pressed)
			{
				Exit();
			}
			else if (GamePad.GetState(PlayerIndex.Four).Buttons.Start == ButtonState.Pressed)
			{
				Exit();
			}
			if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed)
			{
				ShowMarketPlace(PlayerIndex.One);
				_player = PlayerIndex.One;
			}
			else if (GamePad.GetState(PlayerIndex.Two).Buttons.A == ButtonState.Pressed)
			{
				ShowMarketPlace(PlayerIndex.Two);
				_player = PlayerIndex.Two;
			}
			else if (GamePad.GetState(PlayerIndex.Three).Buttons.A == ButtonState.Pressed)
			{
				ShowMarketPlace(PlayerIndex.Three);
				_player = PlayerIndex.Three;
			}
			else if (GamePad.GetState(PlayerIndex.Four).Buttons.A == ButtonState.Pressed)
			{
				ShowMarketPlace(PlayerIndex.Four);
				_player = PlayerIndex.Four;
			}
		}
		else if (drawStartMenu)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)
			{
				_player = PlayerIndex.One;
				drawStartMenu = false;
				drawMainMenu = true;
				soundBank.PlayCue("click");
			}
			else if (GamePad.GetState(PlayerIndex.Two).Buttons.Start == ButtonState.Pressed)
			{
				_player = PlayerIndex.Two;
				drawStartMenu = false;
				drawMainMenu = true;
				soundBank.PlayCue("click");
			}
			else if (GamePad.GetState(PlayerIndex.Three).Buttons.Start == ButtonState.Pressed)
			{
				_player = PlayerIndex.Three;
				drawStartMenu = false;
				drawMainMenu = true;
				soundBank.PlayCue("click");
			}
			else if (GamePad.GetState(PlayerIndex.Four).Buttons.Start == ButtonState.Pressed)
			{
				_player = PlayerIndex.Four;
				drawStartMenu = false;
				drawMainMenu = true;
				soundBank.PlayCue("click");
			}
			bool flag2 = false;
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
			{
				flag2 = true;
			}
			else if (GamePad.GetState(PlayerIndex.Two).Buttons.Back == ButtonState.Pressed)
			{
				flag2 = true;
			}
			else if (GamePad.GetState(PlayerIndex.Three).Buttons.Back == ButtonState.Pressed)
			{
				flag2 = true;
			}
			else if (GamePad.GetState(PlayerIndex.Four).Buttons.Back == ButtonState.Pressed)
			{
				flag2 = true;
			}
			if (flag2 && !backDown)
			{
				if (Guide.IsTrialMode)
				{
					drawBuyMe = true;
					drawStartMenu = false;
					backDown = true;
				}
				else
				{
					Exit();
				}
			}
			else if (!flag2 && backDown)
			{
				backDown = false;
			}
		}
		else if (drawMainMenu)
		{
			FlashTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
			if ((double)FlashTimer > 0.3)
			{
				FlashTimer = 0f;
				flashTimer_Elapsed();
			}
			if ((double)GamePad.GetState(_player).ThumbSticks.Left.Y < -0.1)
			{
				if (MainMenuSelected != 2)
				{
					soundBank.PlayCue("click");
				}
				MainMenuSelected = 2;
			}
			else if ((double)GamePad.GetState(_player).ThumbSticks.Left.Y > 0.1)
			{
				if (MainMenuSelected != 1)
				{
					soundBank.PlayCue("click");
				}
				MainMenuSelected = 1;
			}
			if (GamePad.GetState(_player).IsButtonDown(Buttons.B) || GamePad.GetState(_player).IsButtonDown(Buttons.Back))
			{
				bdown = true;
				backDown = true;
			}
			else if (bdown)
			{
				bdown = false;
				drawMainMenu = false;
				drawStartMenu = true;
			}
			if (GamePad.GetState(_player).IsButtonDown(Buttons.A))
			{
				adown = true;
			}
			else if (adown)
			{
				adown = false;
				soundBank.PlayCue("click");
				if (MainMenuSelected == 1)
				{
					drawMainMenu = false;
					drawInGameMenu = true;
				}
				else if (Guide.IsTrialMode)
				{
					ShowMarketPlace();
				}
				else
				{
					drawMainMenu = false;
					drawStartMenu = true;
				}
			}
		}
		else if (drawInGameMenu)
		{
			if (GamePad.GetState(_player).IsButtonDown(Buttons.Start))
			{
				drawInGameMenu = false;
				drawMainMenu = true;
			}
			if (GamePad.GetState(_player).Buttons.Y == ButtonState.Pressed)
			{
				ydown = true;
			}
			else if (ydown)
			{
				ydown = false;
				drawInGameMenu = false;
				drawMainMenu = false;
				soundBank.PlayCue("click");
			}
			if (GamePad.GetState(_player).Buttons.A == ButtonState.Pressed)
			{
				adown = true;
			}
			else if (adown)
			{
				adown = false;
				if (SelectedWorld == 1)
				{
					if (Background != Phillippines)
					{
						soundBank.PlayCue("click");
						Background = Phillippines;
						nowPlaying.Stop(AudioStopOptions.Immediate);
						nowPlaying = soundBank.GetCue("phillippines");
						nowPlaying.Play();
					}
				}
				else if (SelectedWorld == 2)
				{
					if (Background != Ireland)
					{
						soundBank.PlayCue("click");
						Background = Ireland;
						nowPlaying.Stop(AudioStopOptions.Immediate);
						nowPlaying = soundBank.GetCue("ireland");
						nowPlaying.Play();
					}
				}
				else if (SelectedWorld == 3)
				{
					if (Guide.IsTrialMode)
					{
						ShowMarketPlace();
					}
					else if (Background != Paris)
					{
						soundBank.PlayCue("click");
						Background = Paris;
						nowPlaying.Stop(AudioStopOptions.Immediate);
						nowPlaying = soundBank.GetCue("paris");
						nowPlaying.Play();
					}
				}
				else if (SelectedWorld == 4)
				{
					if (Guide.IsTrialMode)
					{
						ShowMarketPlace();
					}
					else if (Background != Japan)
					{
						soundBank.PlayCue("click");
						Background = Japan;
						nowPlaying.Stop(AudioStopOptions.Immediate);
						nowPlaying = soundBank.GetCue("japan");
						nowPlaying.Play();
					}
				}
				else if (SelectedWorld == 5)
				{
					if (Guide.IsTrialMode)
					{
						ShowMarketPlace();
					}
					else if (Background != Niagara)
					{
						soundBank.PlayCue("click");
						Background = Niagara;
						nowPlaying.Stop(AudioStopOptions.Immediate);
						nowPlaying = soundBank.GetCue("niagaraFalls");
						nowPlaying.Play();
					}
				}
				else if (SelectedWorld == 6)
				{
					if (Guide.IsTrialMode)
					{
						ShowMarketPlace();
					}
					else if (Background != Hawaii)
					{
						soundBank.PlayCue("click");
						Background = Hawaii;
						nowPlaying.Stop(AudioStopOptions.Immediate);
						nowPlaying = soundBank.GetCue("hawaii");
						nowPlaying.Play();
					}
				}
				else if (SelectedWorld == 7)
				{
					if (Guide.IsTrialMode)
					{
						ShowMarketPlace();
					}
					else if (Background != Egypt)
					{
						soundBank.PlayCue("click");
						Background = Egypt;
						nowPlaying.Stop(AudioStopOptions.Immediate);
						nowPlaying = soundBank.GetCue("egypt");
						nowPlaying.Play();
					}
				}
			}
			if ((double)GamePad.GetState(_player).ThumbSticks.Left.Y > 0.1)
			{
				if (!goUp)
				{
					goUp = true;
					leftThumbstickTimer = 0f;
					SelectedWorldChange = -1;
					leftThumbstickTimer_Elapsed();
					maxThumbstickTimer = 0.5f;
				}
				if (goDown)
				{
					goDown = false;
				}
				if (goUp)
				{
					leftThumbstickTimer += GamePad.GetState(_player).ThumbSticks.Left.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;
					if (leftThumbstickTimer > maxThumbstickTimer)
					{
						leftThumbstickTimer = 0f;
						leftThumbstickTimer_Elapsed();
						maxThumbstickTimer = 0.35f;
					}
				}
			}
			else if ((double)GamePad.GetState(_player).ThumbSticks.Left.Y < -0.1)
			{
				if (!goDown)
				{
					goDown = true;
					leftThumbstickTimer = 0f;
					SelectedWorldChange = 1;
					leftThumbstickTimer_Elapsed();
					maxThumbstickTimer = 0.5f;
				}
				if (goUp)
				{
					goUp = false;
				}
				if (goDown)
				{
					leftThumbstickTimer += (0f - GamePad.GetState(_player).ThumbSticks.Left.Y) * (float)gameTime.ElapsedGameTime.TotalSeconds;
					if (leftThumbstickTimer > maxThumbstickTimer)
					{
						leftThumbstickTimer = 0f;
						leftThumbstickTimer_Elapsed();
						maxThumbstickTimer = 0.35f;
					}
				}
			}
			else
			{
				goUp = false;
				goDown = false;
			}
		}
		else if (!drawMainMenu && !drawStartMenu)
		{
			if (GamePad.GetState(_player).Buttons.Y == ButtonState.Pressed)
			{
				ydown = true;
			}
			else if (ydown)
			{
				ydown = false;
				drawInGameMenu = true;
				soundBank.PlayCue("click");
			}
			if (GamePad.GetState(_player).IsButtonDown(Buttons.Start))
			{
				drawMainMenu = true;
			}
		}
		base.Update(gameTime);
	}

	private void ShowMarketPlace()
	{
		if (!Guide.IsVisible)
		{
			if (Gamer.SignedInGamers[_player] != null && Gamer.SignedInGamers[_player].Privileges.AllowPurchaseContent)
			{
				Guide.ShowMarketplace(_player);
				return;
			}
			Guide.ShowSignIn(1, onlineOnly: true);
			drawPurchaseGuide = true;
		}
	}

	private void ShowMarketPlace(PlayerIndex currentPlayer)
	{
		if (Gamer.SignedInGamers[currentPlayer] != null && Gamer.SignedInGamers[currentPlayer].Privileges.AllowPurchaseContent)
		{
			Guide.ShowMarketplace(currentPlayer);
			return;
		}
		Guide.ShowSignIn(1, onlineOnly: true);
		drawPurchaseGuide = true;
	}

	private void flashTimer_Elapsed()
	{
		drawRed = !drawRed;
	}

	protected override void Draw(GameTime gameTime)
	{
		base.GraphicsDevice.Clear(Color.CornflowerBlue);
		int num = base.GraphicsDevice.Viewport.TitleSafeArea.X;
		int num2 = base.GraphicsDevice.Viewport.TitleSafeArea.Y + 27;
		int width = 100;
		int height = 75;
		int num3 = num + 110;
		float y = (float)(num2 + 37) - SmallFont.MeasureString("Egypt").Y / 2f;
		SpriteFont spriteFont = SmallFont;
		Color darkGray = Color.Gray;
		spriteBatch.Begin();
		spriteBatch.Draw(Background, new Rectangle(0, 0, 1280, 720), Color.White);
		if (drawBuyMe)
		{
			spriteBatch.Draw(BuyMeScreen, base.GraphicsDevice.Viewport.Bounds, Color.White);
		}
		if (drawStartMenu)
		{
			spriteBatch.Draw(StartScreen, base.GraphicsDevice.Viewport.Bounds, Color.White);
		}
		if (drawMainMenu)
		{
			Color color = Color.White;
			color = ((MainMenuSelected != 1 || !drawRed) ? Color.White : Color.Red);
			spriteBatch.Draw(black, new Rectangle(0, 0, 1280, 720), Color.White);
			spriteBatch.DrawString(MenuFont, "Travel the World", new Vector2((float)base.GraphicsDevice.Viewport.TitleSafeArea.Center.X - MenuFont.MeasureString("Travel the World").X / 2f, (float)base.GraphicsDevice.Viewport.TitleSafeArea.Center.Y - MenuFont.MeasureString("Travel the World").Y), color);
			color = ((MainMenuSelected != 2 || !drawRed) ? Color.White : Color.Red);
			string text = ((!Guide.IsTrialMode) ? "Quit Game" : "Purchase Game");
			spriteBatch.DrawString(MenuFont, text, new Vector2((float)base.GraphicsDevice.Viewport.TitleSafeArea.Center.X - MenuFont.MeasureString(text).X / 2f, base.GraphicsDevice.Viewport.TitleSafeArea.Center.Y + 10), color);
		}
		if (drawInGameMenu)
		{
			spriteBatch.Draw(black, new Rectangle(0, 0, 500, 720), Color.White);
			if (drawTop1)
			{
				if (SelectedWorld == 1)
				{
					spriteBatch.Draw(white, new Rectangle(num, num2, 180, 142), Color.White);
					num += 15;
					num2 += 15;
					width = 150;
					height = 112;
					num3 = num + 190;
					y = (float)(num2 + 56) - LargeFont.MeasureString("Egypt").Y / 2f;
					spriteFont = LargeFont;
					darkGray = Color.White;
				}
				spriteBatch.Draw(PhillippinesThumbnail, new Rectangle(num, num2, width, height), Color.White);
				if (SelectedWorld != 1)
				{
					spriteBatch.Draw(black, new Rectangle(num, num2, width, height), Color.White);
				}
				spriteBatch.DrawString(spriteFont, "Phillippines", new Vector2(num3, y), darkGray);
				num2 += 83;
				if (SelectedWorld == 1)
				{
					darkGray = Color.Gray;
					num2 += 62;
					num -= 15;
					width = 100;
					height = 75;
					num3 = num + 110;
					spriteFont = SmallFont;
				}
				y = (float)(num2 + 37) - SmallFont.MeasureString("Egypt").Y / 2f;
			}
			else
			{
				spriteBatch.Draw(uparrow, new Rectangle(num + 10, num2 - 27, 75, 10), Color.White);
				y = (float)(num2 + 37) - SmallFont.MeasureString("Egypt").Y / 2f;
			}
			if (drawTop2)
			{
				if (SelectedWorld == 2)
				{
					spriteBatch.Draw(white, new Rectangle(num, num2, 180, 142), Color.White);
					num += 15;
					num2 += 15;
					width = 150;
					height = 112;
					num3 = num + 190;
					y = (float)(num2 + 56) - LargeFont.MeasureString("Egypt").Y / 2f;
					spriteFont = LargeFont;
					darkGray = Color.White;
				}
				spriteBatch.Draw(IrelandThumbnail, new Rectangle(num, num2, width, height), Color.White);
				if (SelectedWorld != 2)
				{
					spriteBatch.Draw(black, new Rectangle(num, num2, width, height), Color.White);
				}
				spriteBatch.DrawString(spriteFont, "Ireland", new Vector2(num3, y), darkGray);
				num2 += 83;
				if (SelectedWorld == 2)
				{
					darkGray = Color.Gray;
					num2 += 62;
					num -= 15;
					width = 100;
					height = 75;
					num3 = num + 110;
					spriteFont = SmallFont;
				}
				y = (float)(num2 + 37) - SmallFont.MeasureString("Egypt").Y / 2f;
			}
			if (SelectedWorld == 3)
			{
				if (Guide.IsTrialMode)
				{
					darkGray = Color.DarkGray;
					spriteBatch.Draw(gray, new Rectangle(num, num2, 180, 142), Color.White);
				}
				else
				{
					darkGray = Color.White;
					spriteBatch.Draw(white, new Rectangle(num, num2, 180, 142), Color.White);
				}
				num += 15;
				num2 += 15;
				width = 150;
				height = 112;
				num3 = num + 190;
				y = (float)(num2 + 56) - LargeFont.MeasureString("Egypt").Y / 2f;
				spriteFont = LargeFont;
			}
			spriteBatch.Draw(ParisThumbnail, new Rectangle(num, num2, width, height), Color.White);
			if (SelectedWorld != 3)
			{
				spriteBatch.Draw(black, new Rectangle(num, num2, width, height), Color.White);
			}
			spriteBatch.DrawString(spriteFont, "Paris", new Vector2(num3, y), darkGray);
			num2 += 83;
			if (SelectedWorld == 3)
			{
				darkGray = Color.Gray;
				num2 += 62;
				num -= 15;
				width = 100;
				height = 75;
				num3 = num + 110;
				spriteFont = SmallFont;
			}
			y = (float)(num2 + 37) - SmallFont.MeasureString("Egypt").Y / 2f;
			if (SelectedWorld == 4)
			{
				if (Guide.IsTrialMode)
				{
					darkGray = Color.DarkGray;
					spriteBatch.Draw(gray, new Rectangle(num, num2, 180, 142), Color.White);
				}
				else
				{
					darkGray = Color.White;
					spriteBatch.Draw(white, new Rectangle(num, num2, 180, 142), Color.White);
				}
				num += 15;
				num2 += 15;
				width = 150;
				height = 112;
				num3 = num + 190;
				y = (float)(num2 + 56) - LargeFont.MeasureString("Egypt").Y / 2f;
				spriteFont = LargeFont;
			}
			spriteBatch.Draw(JapanThumbnail, new Rectangle(num, num2, width, height), Color.White);
			if (SelectedWorld != 4)
			{
				spriteBatch.Draw(black, new Rectangle(num, num2, width, height), Color.White);
			}
			spriteBatch.DrawString(spriteFont, "Japan", new Vector2(num3, y), darkGray);
			num2 += 83;
			if (SelectedWorld == 4)
			{
				darkGray = Color.Gray;
				num2 += 62;
				num -= 15;
				width = 100;
				height = 75;
				num3 = num + 110;
				spriteFont = SmallFont;
			}
			y = (float)(num2 + 37) - SmallFont.MeasureString("Egypt").Y / 2f;
			if (SelectedWorld == 5)
			{
				if (Guide.IsTrialMode)
				{
					darkGray = Color.DarkGray;
					spriteBatch.Draw(gray, new Rectangle(num, num2, 180, 142), Color.White);
				}
				else
				{
					darkGray = Color.White;
					spriteBatch.Draw(white, new Rectangle(num, num2, 180, 142), Color.White);
				}
				num += 15;
				num2 += 15;
				width = 150;
				height = 112;
				num3 = num + 190;
				y = (float)(num2 + 56) - LargeFont.MeasureString("Egypt").Y / 2f;
				spriteFont = LargeFont;
			}
			spriteBatch.Draw(NiagaraThumbnail, new Rectangle(num, num2, width, height), Color.White);
			if (SelectedWorld != 5)
			{
				spriteBatch.Draw(black, new Rectangle(num, num2, width, height), Color.White);
			}
			spriteBatch.DrawString(spriteFont, "Niagara Falls", new Vector2(num3, y), darkGray);
			num2 += 83;
			if (SelectedWorld == 5)
			{
				num2 += 62;
				num -= 15;
				width = 100;
				height = 75;
				num3 = num + 110;
				spriteFont = SmallFont;
				darkGray = Color.Gray;
			}
			y = (float)(num2 + 37) - SmallFont.MeasureString("Egypt").Y / 2f;
			if (!drawTop1)
			{
				if (SelectedWorld == 6)
				{
					if (Guide.IsTrialMode)
					{
						darkGray = Color.DarkGray;
						spriteBatch.Draw(gray, new Rectangle(num, num2, 180, 142), Color.White);
					}
					else
					{
						darkGray = Color.White;
						spriteBatch.Draw(white, new Rectangle(num, num2, 180, 142), Color.White);
					}
					num += 15;
					num2 += 15;
					width = 150;
					height = 112;
					num3 = num + 190;
					y = (float)(num2 + 56) - LargeFont.MeasureString("Egypt").Y / 2f;
					spriteFont = LargeFont;
				}
				spriteBatch.Draw(HawaiiThumbnail, new Rectangle(num, num2, width, height), Color.White);
				if (SelectedWorld != 6)
				{
					spriteBatch.Draw(black, new Rectangle(num, num2, width, height), Color.White);
				}
				spriteBatch.DrawString(spriteFont, "Hawaii", new Vector2(num3, y), darkGray);
				num2 += 83;
				if (SelectedWorld == 6)
				{
					darkGray = Color.Gray;
					num2 += 62;
					num -= 15;
					width = 100;
					height = 75;
					num3 = num + 110;
					spriteFont = SmallFont;
				}
				y = (float)(num2 + 37) - SmallFont.MeasureString("Egypt").Y / 2f;
			}
			if (!drawTop2)
			{
				if (SelectedWorld == 7)
				{
					if (Guide.IsTrialMode)
					{
						darkGray = Color.DarkGray;
						spriteBatch.Draw(gray, new Rectangle(num, num2, 180, 142), Color.White);
					}
					else
					{
						darkGray = Color.White;
						spriteBatch.Draw(white, new Rectangle(num, num2, 180, 142), Color.White);
					}
					num += 15;
					num2 += 15;
					width = 150;
					height = 112;
					num3 = num + 190;
					y = (float)(num2 + 56) - LargeFont.MeasureString("Egypt").Y / 2f;
					spriteFont = LargeFont;
				}
				spriteBatch.Draw(EgyptThumbnail, new Rectangle(num, num2, width, height), Color.White);
				if (SelectedWorld != 7)
				{
					spriteBatch.Draw(black, new Rectangle(num, num2, width, height), Color.White);
				}
				spriteBatch.DrawString(spriteFont, "Egypt", new Vector2(num3, y), darkGray);
			}
			else
			{
				spriteBatch.Draw(downarrow, new Rectangle(num + 10, num2 - 3, 75, 10), Color.White);
			}
			spriteBatch.Draw(ybutton, new Rectangle(base.GraphicsDevice.Viewport.TitleSafeArea.X, (int)((float)base.GraphicsDevice.Viewport.TitleSafeArea.Bottom - LargeFont.MeasureString("Hide Menu").Y), (int)LargeFont.MeasureString("Hide Menu").Y, (int)LargeFont.MeasureString("Hide Menu").Y), Color.White);
			spriteBatch.DrawString(LargeFont, "Hide Menu", new Vector2((float)base.GraphicsDevice.Viewport.TitleSafeArea.X + LargeFont.MeasureString("Hide Menu").Y + 5f, (float)base.GraphicsDevice.Viewport.TitleSafeArea.Bottom - LargeFont.MeasureString("Hide Menu").Y), Color.White);
		}
		spriteBatch.End();
		if (drawBuyWarning)
		{
			string description = ((!drawBuyMe && !drawMainMenu) ? "Purchase Ambient Travels to visit this destination\n\nTo be able to purchase the game you must be signed into an account that allows you to purchase content." : "To be able to purchase Ambient Travels you must be signed into an account that allows you to purchase content.");
			DialogScreen dialogScreen = new DialogScreen("Purchase Ambient travels", description, Menu, SmallMenuFont, MenuFont);
			dialogScreen.DescriptionColor = Color.Black;
			dialogScreen.TitleColor = Color.Black;
			dialogScreen.rightPadding = 0;
			dialogScreen.DescriptionPadding.X = 30f;
			dialogScreen.TitlePadding.X = 30f;
			dialogScreen.TitlePadding.Y -= 3f;
			dialogScreen.DescriptionPadding.Y += 5f;
			dialogScreen.OnDraw(base.GraphicsDevice, spriteBatch, gameTime);
		}
		base.Draw(gameTime);
	}
}
