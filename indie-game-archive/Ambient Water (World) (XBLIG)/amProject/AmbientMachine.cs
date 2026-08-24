using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace amProject;

public class AmbientMachine : Game
{
	private enum AmbientState
	{
		beachState,
		rainState,
		streamState,
		docksState,
		waterfallState,
		underwaterState
	}

	private enum GameState
	{
		gameState,
		titleState,
		exitState,
		upsellState,
		nopowerState
	}

	private enum HUDState
	{
		controlsUp,
		controlsDown
	}

	private enum StartState
	{
		fadeIn,
		fadeOut
	}

	private GraphicsDeviceManager graphics;

	private SpriteBatch spriteBatch;

	private SpriteFont font;

	private string outputString;

	private Vector2 outputVector;

	private Texture2D titleTexture;

	private Texture2D controlsTexture;

	private Texture2D controlsTrialTexture;

	private Texture2D startTexture;

	private Texture2D exitTexture;

	private Texture2D exitTrialTexture;

	private Texture2D upsellTexture;

	private Texture2D nopowerTexture;

	private Texture2D buttonTexture0;

	private Texture2D buttonTexture1;

	private Texture2D buttonTexture2;

	private Texture2D buttonTexture3;

	private Texture2D buttonTexture4;

	private Texture2D buttonTexture5;

	private Texture2D highTexture;

	private Texture2D highTexture0;

	private Texture2D highTexture1;

	private Texture2D highTexture2;

	private Texture2D highTexture3;

	private Texture2D highTexture4;

	private Texture2D highTexture5;

	private float highlight;

	private Color AlphaColor;

	private Rectangle viewRect;

	private bool bShowControls;

	private AmbientState stateAmbient;

	private AmbientState statePrevious;

	private SoundEffectInstance sfxInstance;

	private SoundEffect sfxStream;

	private SoundEffect sfxDocks;

	private SoundEffect sfxBeach;

	private SoundEffect sfxWaterfall;

	private SoundEffect sfxRain;

	private SoundEffect sfxUnderwater;

	private SoundEffect soundControls;

	private SoundEffect soundChange;

	private SoundEffect soundEnter;

	private Texture2D ambientTexture;

	private Texture2D textureStream;

	private Texture2D textureDocks;

	private Texture2D textureBeach;

	private Texture2D textureWaterfall;

	private Texture2D textureRain;

	private Texture2D textureUnderwater;

	private bool bFirstTimeThroughGameState;

	private GameState state;

	private HUDState stateHUD;

	private StartState startState;

	private Color ColorStart;

	private float startAlpha;

	private GamePadState gamepadCurrent;

	private GamePadState gamepadPrevious;

	private PlayerIndex controllingPlayer;

	private SignedInGamer gamer;

	private bool bHasTriedToBuyFromNoPowerAlready;

	private bool isTryingToBuyOnWayOut;

	public AmbientMachine()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		statePrevious = AmbientState.waterfallState;
		bFirstTimeThroughGameState = true;
		state = GameState.titleState;
		((Game)this)._002Ector();
		graphics = new GraphicsDeviceManager((Game)(object)this);
		graphics.PreferredBackBufferWidth = 1280;
		graphics.PreferredBackBufferHeight = 720;
		((Game)this).Content.RootDirectory = "Content";
	}

	protected override void Initialize()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)new GamerServicesComponent((Game)(object)this));
		AlphaColor = new Color(1f, 1f, 1f, 1f);
		((Game)this).Initialize();
	}

	protected override void LoadContent()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		spriteBatch = new SpriteBatch(((Game)this).GraphicsDevice);
		font = ((Game)this).Content.Load<SpriteFont>("Arial");
		Viewport viewport = graphics.GraphicsDevice.Viewport;
		int width = ((Viewport)(ref viewport)).Width;
		Viewport viewport2 = graphics.GraphicsDevice.Viewport;
		viewRect = new Rectangle(0, 0, width, ((Viewport)(ref viewport2)).Height);
		sfxStream = ((Game)this).Content.Load<SoundEffect>("loopStream");
		sfxDocks = ((Game)this).Content.Load<SoundEffect>("loopDocks");
		sfxBeach = ((Game)this).Content.Load<SoundEffect>("loopBeach");
		sfxWaterfall = ((Game)this).Content.Load<SoundEffect>("loopWaterfall");
		sfxRain = ((Game)this).Content.Load<SoundEffect>("loopRain");
		sfxUnderwater = ((Game)this).Content.Load<SoundEffect>("loopUnderwater");
		soundChange = ((Game)this).Content.Load<SoundEffect>("soundChange");
		soundEnter = ((Game)this).Content.Load<SoundEffect>("soundEnter");
		soundControls = ((Game)this).Content.Load<SoundEffect>("soundControls");
		textureDocks = ((Game)this).Content.Load<Texture2D>("screenDocks");
		textureStream = ((Game)this).Content.Load<Texture2D>("screenStream");
		textureBeach = ((Game)this).Content.Load<Texture2D>("screenBeach");
		textureWaterfall = ((Game)this).Content.Load<Texture2D>("screenWaterfall");
		textureRain = ((Game)this).Content.Load<Texture2D>("screenRain");
		textureUnderwater = ((Game)this).Content.Load<Texture2D>("screenUnderwater");
		titleTexture = ((Game)this).Content.Load<Texture2D>("screenTitle");
		controlsTexture = ((Game)this).Content.Load<Texture2D>("screenControls");
		controlsTrialTexture = ((Game)this).Content.Load<Texture2D>("screenControlsTrial");
		startTexture = ((Game)this).Content.Load<Texture2D>("screenPressStart");
		exitTexture = ((Game)this).Content.Load<Texture2D>("screenExit");
		exitTrialTexture = ((Game)this).Content.Load<Texture2D>("screenExitTrial");
		upsellTexture = ((Game)this).Content.Load<Texture2D>("screenUpsell");
		nopowerTexture = ((Game)this).Content.Load<Texture2D>("screenNoPurchasePower");
		buttonTexture0 = ((Game)this).Content.Load<Texture2D>("screenButton0");
		buttonTexture1 = ((Game)this).Content.Load<Texture2D>("screenButton1");
		buttonTexture2 = ((Game)this).Content.Load<Texture2D>("screenButton2");
		buttonTexture3 = ((Game)this).Content.Load<Texture2D>("screenButton3");
		buttonTexture4 = ((Game)this).Content.Load<Texture2D>("screenButton4");
		buttonTexture5 = ((Game)this).Content.Load<Texture2D>("screenButton5");
		highTexture0 = ((Game)this).Content.Load<Texture2D>("screenHighlight0");
		highTexture1 = ((Game)this).Content.Load<Texture2D>("screenHighlight1");
		highTexture2 = ((Game)this).Content.Load<Texture2D>("screenHighlight2");
		highTexture3 = ((Game)this).Content.Load<Texture2D>("screenHighlight3");
		highTexture4 = ((Game)this).Content.Load<Texture2D>("screenHighlight4");
		highTexture5 = ((Game)this).Content.Load<Texture2D>("screenHighlight5");
		highTexture = highTexture0;
		playAmbient();
		soundEnter.Play();
	}

	protected override void UnloadContent()
	{
	}

	protected override void Update(GameTime gameTime)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		gamepadCurrent = GamePad.GetState(controllingPlayer);
		switch (state)
		{
		case GameState.titleState:
			updateTitle();
			break;
		case GameState.gameState:
			updateGame();
			break;
		case GameState.nopowerState:
			updateNoPower();
			break;
		case GameState.upsellState:
			updateUpsell();
			break;
		case GameState.exitState:
			updateExit();
			break;
		}
		checkIfControllerConnected();
		gamepadPrevious = gamepadCurrent;
		((Game)this).Update(gameTime);
	}

	private void updateGame()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Invalid comparison between Unknown and I4
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Invalid comparison between Unknown and I4
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Invalid comparison between Unknown and I4
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Invalid comparison between Unknown and I4
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Invalid comparison between Unknown and I4
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Invalid comparison between Unknown and I4
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Invalid comparison between Unknown and I4
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		if (!Guide.IsVisible)
		{
			GamePadButtons buttons = ((GamePadState)(ref gamepadCurrent)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).Back == 1)
			{
				GamePadButtons buttons2 = ((GamePadState)(ref gamepadPrevious)).Buttons;
				if ((int)((GamePadButtons)(ref buttons2)).Back == 0)
				{
					state = GameState.exitState;
				}
			}
		}
		if (!bFirstTimeThroughGameState && !Guide.IsVisible)
		{
			GamePadButtons buttons3 = ((GamePadState)(ref gamepadCurrent)).Buttons;
			if ((int)((GamePadButtons)(ref buttons3)).Start == 1)
			{
				GamePadButtons buttons4 = ((GamePadState)(ref gamepadPrevious)).Buttons;
				if ((int)((GamePadButtons)(ref buttons4)).Start == 0)
				{
					toggleControls();
				}
			}
		}
		if (Guide.IsTrialMode && !Guide.IsVisible && stateAmbient >= AmbientState.streamState)
		{
			GamePadButtons buttons5 = ((GamePadState)(ref gamepadCurrent)).Buttons;
			if ((int)((GamePadButtons)(ref buttons5)).A == 1)
			{
				GamePadButtons buttons6 = ((GamePadState)(ref gamepadPrevious)).Buttons;
				if ((int)((GamePadButtons)(ref buttons6)).A == 0)
				{
					state = GameState.upsellState;
					return;
				}
			}
		}
		if (!Guide.IsVisible)
		{
			GamePadButtons buttons7 = ((GamePadState)(ref gamepadCurrent)).Buttons;
			if ((int)((GamePadButtons)(ref buttons7)).X == 1)
			{
				GamePadButtons buttons8 = ((GamePadState)(ref gamepadPrevious)).Buttons;
				if ((int)((GamePadButtons)(ref buttons8)).X == 0)
				{
					buyGame();
				}
			}
		}
		if (!Guide.IsVisible)
		{
			GamePadButtons buttons9 = ((GamePadState)(ref gamepadCurrent)).Buttons;
			if ((int)((GamePadButtons)(ref buttons9)).A == 1)
			{
				GamePadButtons buttons10 = ((GamePadState)(ref gamepadPrevious)).Buttons;
				if ((int)((GamePadButtons)(ref buttons10)).A == 0)
				{
					playAmbient();
					soundEnter.Play();
				}
			}
		}
		if (!Guide.IsVisible)
		{
			GamePadThumbSticks thumbSticks = ((GamePadState)(ref gamepadCurrent)).ThumbSticks;
			if (((GamePadThumbSticks)(ref thumbSticks)).Left.X > 0.8f)
			{
				GamePadThumbSticks thumbSticks2 = ((GamePadState)(ref gamepadPrevious)).ThumbSticks;
				if (((GamePadThumbSticks)(ref thumbSticks2)).Left.X < 0.8f)
				{
					soundChange.Play();
					stateAmbient++;
				}
			}
		}
		if (!Guide.IsVisible)
		{
			GamePadThumbSticks thumbSticks3 = ((GamePadState)(ref gamepadCurrent)).ThumbSticks;
			if (((GamePadThumbSticks)(ref thumbSticks3)).Left.X < -0.8f)
			{
				GamePadThumbSticks thumbSticks4 = ((GamePadState)(ref gamepadPrevious)).ThumbSticks;
				if (((GamePadThumbSticks)(ref thumbSticks4)).Left.X > -0.8f)
				{
					soundChange.Play();
					stateAmbient--;
				}
			}
		}
		if (!Guide.IsVisible)
		{
			GamePadDPad dPad = ((GamePadState)(ref gamepadCurrent)).DPad;
			if ((int)((GamePadDPad)(ref dPad)).Right == 1)
			{
				GamePadDPad dPad2 = ((GamePadState)(ref gamepadPrevious)).DPad;
				if ((int)((GamePadDPad)(ref dPad2)).Right == 0)
				{
					soundChange.Play();
					stateAmbient++;
				}
			}
		}
		if (!Guide.IsVisible)
		{
			GamePadDPad dPad3 = ((GamePadState)(ref gamepadCurrent)).DPad;
			if ((int)((GamePadDPad)(ref dPad3)).Left == 1)
			{
				GamePadDPad dPad4 = ((GamePadState)(ref gamepadPrevious)).DPad;
				if ((int)((GamePadDPad)(ref dPad4)).Left == 0)
				{
					soundChange.Play();
					stateAmbient--;
				}
			}
		}
		normalizeAmbientState();
		if (highlight > 0f)
		{
			highlight -= 0.05f;
		}
		if (highlight < 0f)
		{
			highlight = 0f;
		}
		AlphaColor = new Color(1f, 1f, 1f, highlight);
		bFirstTimeThroughGameState = false;
	}

	private void toggleControls()
	{
		soundControls.Play();
		switch (bShowControls)
		{
		case true:
			bShowControls = false;
			break;
		case false:
			bShowControls = true;
			break;
		}
	}

	public void playAmbient()
	{
		highlight = 1f;
		if (statePrevious != stateAmbient)
		{
			if (sfxInstance != null)
			{
				sfxInstance.Dispose();
			}
			switch (stateAmbient)
			{
			case AmbientState.docksState:
				sfxInstance = sfxDocks.CreateInstance();
				ambientTexture = textureDocks;
				highTexture = highTexture3;
				break;
			case AmbientState.rainState:
				sfxInstance = sfxRain.CreateInstance();
				ambientTexture = textureRain;
				highTexture = highTexture1;
				break;
			case AmbientState.beachState:
				sfxInstance = sfxBeach.CreateInstance();
				ambientTexture = textureBeach;
				highTexture = highTexture0;
				break;
			case AmbientState.streamState:
				sfxInstance = sfxStream.CreateInstance();
				ambientTexture = textureStream;
				highTexture = highTexture2;
				break;
			case AmbientState.waterfallState:
				sfxInstance = sfxWaterfall.CreateInstance();
				ambientTexture = textureWaterfall;
				highTexture = highTexture4;
				break;
			case AmbientState.underwaterState:
				sfxInstance = sfxUnderwater.CreateInstance();
				ambientTexture = textureUnderwater;
				highTexture = highTexture5;
				break;
			}
			sfxInstance.IsLooped = true;
			sfxInstance.Play();
		}
		statePrevious = stateAmbient;
	}

	private void normalizeAmbientState()
	{
		int num = 6;
		if (stateAmbient < AmbientState.beachState)
		{
			stateAmbient = AmbientState.beachState;
		}
		if ((int)stateAmbient > num - 1)
		{
			stateAmbient = (AmbientState)(num - 1);
		}
	}

	private void updateTitle()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Invalid comparison between Unknown and I4
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Invalid comparison between Unknown and I4
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Invalid comparison between Unknown and I4
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		PlayerIndex val = (PlayerIndex)0;
		while ((int)val <= 3)
		{
			GamePadState val2 = GamePad.GetState(val);
			GamePadButtons buttons = ((GamePadState)(ref val2)).Buttons;
			_ = ((GamePadButtons)(ref buttons)).Back;
			_ = 1;
			GamePadState val3 = GamePad.GetState(val);
			GamePadButtons buttons2 = ((GamePadState)(ref val3)).Buttons;
			if ((int)((GamePadButtons)(ref buttons2)).Start != 1)
			{
				GamePadState val4 = GamePad.GetState(val);
				GamePadButtons buttons3 = ((GamePadState)(ref val4)).Buttons;
				if ((int)((GamePadButtons)(ref buttons3)).A != 1)
				{
					val = (PlayerIndex)(val + 1);
					continue;
				}
			}
			controllingPlayer = val;
			gamer = Gamer.SignedInGamers[controllingPlayer];
			if (gamer == null && !Guide.IsVisible)
			{
				_ = Guide.IsTrialMode;
			}
			state = GameState.gameState;
			bShowControls = true;
			playAmbient();
			soundEnter.Play();
			break;
		}
	}

	private void updateUpsell()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Invalid comparison between Unknown and I4
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Invalid comparison between Unknown and I4
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		GamePadButtons buttons = ((GamePadState)(ref gamepadCurrent)).Buttons;
		if ((int)((GamePadButtons)(ref buttons)).A == 1)
		{
			GamePadButtons buttons2 = ((GamePadState)(ref gamepadPrevious)).Buttons;
			if ((int)((GamePadButtons)(ref buttons2)).A == 0)
			{
				buyGame();
			}
		}
		GamePadButtons buttons3 = ((GamePadState)(ref gamepadCurrent)).Buttons;
		if ((int)((GamePadButtons)(ref buttons3)).B == 1)
		{
			GamePadButtons buttons4 = ((GamePadState)(ref gamepadPrevious)).Buttons;
			if ((int)((GamePadButtons)(ref buttons4)).B == 0)
			{
				buyGame();
			}
		}
		GamePadState val = GamePad.GetState(controllingPlayer);
		GamePadButtons buttons5 = ((GamePadState)(ref val)).Buttons;
		if ((int)((GamePadButtons)(ref buttons5)).X == 1 || !Guide.IsTrialMode)
		{
			stateHUD = HUDState.controlsUp;
			state = GameState.gameState;
		}
	}

	private void updateNoPower()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Invalid comparison between Unknown and I4
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Invalid comparison between Unknown and I4
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		if (!Guide.IsTrialMode)
		{
			state = GameState.gameState;
		}
		if (CanPlayerBuyGame(controllingPlayer) && !Guide.IsVisible && !bHasTriedToBuyFromNoPowerAlready)
		{
			buyGame();
			bHasTriedToBuyFromNoPowerAlready = true;
		}
		if (CanPlayerBuyGame(controllingPlayer) && !Guide.IsVisible && bHasTriedToBuyFromNoPowerAlready)
		{
			state = GameState.gameState;
		}
		if (!Guide.IsVisible)
		{
			GamePadButtons buttons = ((GamePadState)(ref gamepadCurrent)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).A == 1)
			{
				GamePadButtons buttons2 = ((GamePadState)(ref gamepadPrevious)).Buttons;
				if ((int)((GamePadButtons)(ref buttons2)).A == 0)
				{
					Guide.ShowSignIn(1, true);
				}
			}
		}
		GamePadButtons buttons3 = ((GamePadState)(ref gamepadCurrent)).Buttons;
		if ((int)((GamePadButtons)(ref buttons3)).B == 1)
		{
			GamePadButtons buttons4 = ((GamePadState)(ref gamepadPrevious)).Buttons;
			if ((int)((GamePadButtons)(ref buttons4)).B == 0)
			{
				state = GameState.gameState;
			}
		}
	}

	private void checkIfControllerConnected()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		GamePadState val = GamePad.GetState(controllingPlayer);
		if (!((GamePadState)(ref val)).IsConnected)
		{
			stateAmbient = AmbientState.beachState;
			playAmbient();
			state = GameState.titleState;
		}
	}

	public static bool CanPlayerBuyGame(PlayerIndex player)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		SignedInGamer val = Gamer.SignedInGamers[player];
		if (val == null)
		{
			return false;
		}
		if (!val.IsSignedInToLive)
		{
			return false;
		}
		return val.Privileges.AllowPurchaseContent;
	}

	private void buyGame()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (!Guide.IsTrialMode)
		{
			state = GameState.gameState;
			stateHUD = HUDState.controlsUp;
		}
		if (CanPlayerBuyGame(controllingPlayer) && !Guide.IsVisible)
		{
			Guide.ShowMarketplace(controllingPlayer);
		}
		else if (!CanPlayerBuyGame(controllingPlayer))
		{
			bHasTriedToBuyFromNoPowerAlready = false;
			state = GameState.nopowerState;
		}
	}

	private void updateExit()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Invalid comparison between Unknown and I4
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Invalid comparison between Unknown and I4
		GamePadButtons buttons = ((GamePadState)(ref gamepadCurrent)).Buttons;
		if ((int)((GamePadButtons)(ref buttons)).A == 1)
		{
			GamePadButtons buttons2 = ((GamePadState)(ref gamepadPrevious)).Buttons;
			if ((int)((GamePadButtons)(ref buttons2)).A == 0)
			{
				((Game)this).Exit();
			}
		}
		GamePadState val = GamePad.GetState(controllingPlayer);
		GamePadButtons buttons3 = ((GamePadState)(ref val)).Buttons;
		if ((int)((GamePadButtons)(ref buttons3)).B == 1)
		{
			state = GameState.gameState;
		}
		if (Guide.IsTrialMode)
		{
			GamePadState val2 = GamePad.GetState(controllingPlayer);
			GamePadButtons buttons4 = ((GamePadState)(ref val2)).Buttons;
			if ((int)((GamePadButtons)(ref buttons4)).X == 1)
			{
				state = GameState.gameState;
				buyGame();
			}
		}
	}

	protected override void Draw(GameTime gameTime)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		((Game)this).GraphicsDevice.Clear(Color.CornflowerBlue);
		spriteBatch.Begin();
		switch (state)
		{
		case GameState.titleState:
			drawTitle();
			break;
		case GameState.gameState:
			drawGame();
			break;
		case GameState.exitState:
			drawExit();
			break;
		case GameState.upsellState:
			drawUpsell();
			break;
		case GameState.nopowerState:
			spriteBatch.Draw(ambientTexture, viewRect, Color.White);
			spriteBatch.Draw(nopowerTexture, viewRect, Color.White);
			break;
		}
		outputString = state.ToString();
		outputVector = new Vector2(100f, 100f);
		spriteBatch.End();
		((Game)this).Draw(gameTime);
	}

	private void drawUpsell()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		spriteBatch.Draw(ambientTexture, viewRect, Color.White);
		spriteBatch.Draw(upsellTexture, viewRect, Color.White);
	}

	private void drawExit()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		spriteBatch.Draw(ambientTexture, viewRect, Color.White);
		spriteBatch.Draw(controlsTexture, viewRect, Color.White);
		if (Guide.IsTrialMode)
		{
			spriteBatch.Draw(exitTrialTexture, viewRect, Color.White);
		}
		else if (!Guide.IsTrialMode)
		{
			spriteBatch.Draw(exitTexture, viewRect, Color.White);
		}
	}

	private void drawTitle()
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		if (startState == StartState.fadeIn)
		{
			startAlpha += 0.02f;
			if (startAlpha > 2f)
			{
				startAlpha = 1f;
				startState = StartState.fadeOut;
			}
		}
		if (startState == StartState.fadeOut)
		{
			startAlpha -= 0.1f;
			if (startAlpha < 0f)
			{
				startAlpha = 0f;
				startState = StartState.fadeIn;
			}
		}
		spriteBatch.Draw(ambientTexture, viewRect, Color.White);
		spriteBatch.Draw(titleTexture, viewRect, Color.White);
		ColorStart = new Color(1f, 1f, 1f, startAlpha);
		spriteBatch.Draw(startTexture, viewRect, ColorStart);
	}

	private void drawGame()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (ambientTexture == null)
		{
			return;
		}
		spriteBatch.Draw(ambientTexture, viewRect, Color.White);
		if (bShowControls)
		{
			if (Guide.IsTrialMode)
			{
				spriteBatch.Draw(controlsTrialTexture, viewRect, Color.White);
			}
			if (!Guide.IsTrialMode)
			{
				spriteBatch.Draw(controlsTexture, viewRect, Color.White);
			}
			spriteBatch.Draw(highTexture, viewRect, AlphaColor);
		}
		if (bShowControls)
		{
			switch (stateAmbient)
			{
			case AmbientState.beachState:
				spriteBatch.Draw(buttonTexture0, viewRect, Color.White);
				break;
			case AmbientState.rainState:
				spriteBatch.Draw(buttonTexture1, viewRect, Color.White);
				break;
			case AmbientState.streamState:
				spriteBatch.Draw(buttonTexture2, viewRect, Color.White);
				break;
			case AmbientState.docksState:
				spriteBatch.Draw(buttonTexture3, viewRect, Color.White);
				break;
			case AmbientState.waterfallState:
				spriteBatch.Draw(buttonTexture4, viewRect, Color.White);
				break;
			case AmbientState.underwaterState:
				spriteBatch.Draw(buttonTexture5, viewRect, Color.White);
				break;
			}
		}
	}
}
