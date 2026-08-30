using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TabeBallMk4;

public class Menus : GameComponent
{
	private int menuDelayCounter = 30;

	private int menuDelay = 30;

	private int pointerPointAt = 0;

	private int pointerLag = 0;

	private int setPointerLag = 10;

	private int pointerPosX;

	private int pointerPosY;

	private bool showPointer = false;

	private TabeBallTable table;

	private Texture2D BlankMenu;

	private Texture2D PressStart;

	private Texture2D MainMenu;

	private Texture2D GameSetup;

	private Texture2D PointerT;

	private Texture2D[] GS_PlayerSelectT = (Texture2D[])(object)new Texture2D[8];

	private Texture2D[] GS_TimeLimitSlider = (Texture2D[])(object)new Texture2D[6];

	private Texture2D[] GS_StickStyle = (Texture2D[])(object)new Texture2D[4];

	private int GS_TimeLimitSliderIndex = 0;

	private Texture2D[] FinalScores = (Texture2D[])(object)new Texture2D[3];

	private SpriteFont Font1;

	private SpriteBatch menuSpriteBatch;

	private GamePadState menuGP;

	public Menus(Game game)
		: base(game)
	{
		table = new TabeBallTable(game);
	}

	public bool LoadGameContent()
	{
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Expected O, but got Unknown
		BlankMenu = ((GameComponent)this).Game.Content.Load<Texture2D>("Textures\\BlankMenu");
		PressStart = ((GameComponent)this).Game.Content.Load<Texture2D>("Textures\\PressStart");
		MainMenu = ((GameComponent)this).Game.Content.Load<Texture2D>("Textures\\MainMenu");
		GameSetup = ((GameComponent)this).Game.Content.Load<Texture2D>("Textures\\GameSetup");
		PointerT = ((GameComponent)this).Game.Content.Load<Texture2D>("Textures\\PointerMK1");
		GS_PlayerSelectT[0] = ((GameComponent)this).Game.Content.Load<Texture2D>("Textures\\GS_LeftArrow");
		GS_PlayerSelectT[1] = ((GameComponent)this).Game.Content.Load<Texture2D>("Textures\\GS_Player1");
		GS_PlayerSelectT[2] = ((GameComponent)this).Game.Content.Load<Texture2D>("Textures\\GS_Player2");
		GS_PlayerSelectT[3] = ((GameComponent)this).Game.Content.Load<Texture2D>("Textures\\GS_Player3");
		GS_PlayerSelectT[4] = ((GameComponent)this).Game.Content.Load<Texture2D>("Textures\\GS_Player4");
		GS_PlayerSelectT[5] = ((GameComponent)this).Game.Content.Load<Texture2D>("Textures\\GS_AIEasy");
		GS_PlayerSelectT[6] = ((GameComponent)this).Game.Content.Load<Texture2D>("Textures\\GS_AINormal");
		GS_PlayerSelectT[7] = ((GameComponent)this).Game.Content.Load<Texture2D>("Textures\\GS_AIHard");
		GS_TimeLimitSlider[0] = ((GameComponent)this).Game.Content.Load<Texture2D>("Textures\\TL_2min");
		GS_TimeLimitSlider[1] = ((GameComponent)this).Game.Content.Load<Texture2D>("Textures\\TL_5min");
		GS_TimeLimitSlider[2] = ((GameComponent)this).Game.Content.Load<Texture2D>("Textures\\TL_10min");
		GS_TimeLimitSlider[3] = ((GameComponent)this).Game.Content.Load<Texture2D>("Textures\\TL_20min");
		GS_TimeLimitSlider[4] = ((GameComponent)this).Game.Content.Load<Texture2D>("Textures\\TL_45min");
		GS_TimeLimitSlider[5] = ((GameComponent)this).Game.Content.Load<Texture2D>("Textures\\TL_90min");
		GS_StickStyle[0] = ((GameComponent)this).Game.Content.Load<Texture2D>("Textures\\CS_Both");
		GS_StickStyle[1] = ((GameComponent)this).Game.Content.Load<Texture2D>("Textures\\CS_LeftOnly");
		GS_StickStyle[2] = ((GameComponent)this).Game.Content.Load<Texture2D>("Textures\\CS_RightOnly");
		GS_StickStyle[3] = ((GameComponent)this).Game.Content.Load<Texture2D>("Textures\\CS_BothCPU");
		FinalScores[0] = ((GameComponent)this).Game.Content.Load<Texture2D>("Textures\\FS_RedTeamWin");
		FinalScores[1] = ((GameComponent)this).Game.Content.Load<Texture2D>("Textures\\FS_BlueTeamWin");
		FinalScores[2] = ((GameComponent)this).Game.Content.Load<Texture2D>("Textures\\FS_Draw");
		menuSpriteBatch = new SpriteBatch(((GameComponent)this).Game.GraphicsDevice);
		table.LoadModels();
		return true;
	}

	public override void Initialize()
	{
		((GameComponent)this).Initialize();
	}

	public override void Update(GameTime gameTime)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Invalid comparison between Unknown and I4
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Invalid comparison between Unknown and I4
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Invalid comparison between Unknown and I4
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Invalid comparison between Unknown and I4
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Invalid comparison between Unknown and I4
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Invalid comparison between Unknown and I4
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Invalid comparison between Unknown and I4
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_053a: Invalid comparison between Unknown and I4
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Invalid comparison between Unknown and I4
		//IL_0542: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_054b: Unknown result type (might be due to invalid IL or missing references)
		//IL_17ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_1804: Unknown result type (might be due to invalid IL or missing references)
		//IL_1808: Unknown result type (might be due to invalid IL or missing references)
		//IL_180e: Invalid comparison between Unknown and I4
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05da: Unknown result type (might be due to invalid IL or missing references)
		//IL_05de: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e4: Invalid comparison between Unknown and I4
		//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0695: Unknown result type (might be due to invalid IL or missing references)
		//IL_069a: Unknown result type (might be due to invalid IL or missing references)
		//IL_069e: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a4: Invalid comparison between Unknown and I4
		//IL_07f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0800: Unknown result type (might be due to invalid IL or missing references)
		//IL_0806: Invalid comparison between Unknown and I4
		//IL_06ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0deb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0df0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0df4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dfa: Invalid comparison between Unknown and I4
		//IL_080e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0813: Unknown result type (might be due to invalid IL or missing references)
		//IL_0817: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fcd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fdc: Invalid comparison between Unknown and I4
		//IL_0e02: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e07: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0738: Unknown result type (might be due to invalid IL or missing references)
		//IL_073d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0741: Unknown result type (might be due to invalid IL or missing references)
		//IL_0747: Invalid comparison between Unknown and I4
		//IL_15c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_15c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_15ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_15d0: Invalid comparison between Unknown and I4
		//IL_0fe4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fe9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fed: Unknown result type (might be due to invalid IL or missing references)
		//IL_096f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0974: Unknown result type (might be due to invalid IL or missing references)
		//IL_0978: Unknown result type (might be due to invalid IL or missing references)
		//IL_097e: Invalid comparison between Unknown and I4
		//IL_074f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0754: Unknown result type (might be due to invalid IL or missing references)
		//IL_0758: Unknown result type (might be due to invalid IL or missing references)
		//IL_179d: Unknown result type (might be due to invalid IL or missing references)
		//IL_17a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_17a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_17ac: Invalid comparison between Unknown and I4
		//IL_15d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_15dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_15e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ece: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ed3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ed7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0edd: Invalid comparison between Unknown and I4
		//IL_0986: Unknown result type (might be due to invalid IL or missing references)
		//IL_098b: Unknown result type (might be due to invalid IL or missing references)
		//IL_098f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0875: Unknown result type (might be due to invalid IL or missing references)
		//IL_087a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1145: Unknown result type (might be due to invalid IL or missing references)
		//IL_114a: Unknown result type (might be due to invalid IL or missing references)
		//IL_114e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1154: Invalid comparison between Unknown and I4
		//IL_0ee5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eee: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_16a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_16a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_16ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_16b3: Invalid comparison between Unknown and I4
		//IL_115c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1161: Unknown result type (might be due to invalid IL or missing references)
		//IL_1165: Unknown result type (might be due to invalid IL or missing references)
		//IL_104b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1050: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_16bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_16c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_16c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_108c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1091: Unknown result type (might be due to invalid IL or missing references)
		//IL_10cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a04: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a09: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a84: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a89: Unknown result type (might be due to invalid IL or missing references)
		//IL_11da: Unknown result type (might be due to invalid IL or missing references)
		//IL_11df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b49: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ace: Unknown result type (might be due to invalid IL or missing references)
		//IL_125a: Unknown result type (might be due to invalid IL or missing references)
		//IL_125f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c56: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b91: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b96: Unknown result type (might be due to invalid IL or missing references)
		//IL_131f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1324: Unknown result type (might be due to invalid IL or missing references)
		//IL_129f: Unknown result type (might be due to invalid IL or missing references)
		//IL_12a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bdb: Unknown result type (might be due to invalid IL or missing references)
		//IL_142c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1431: Unknown result type (might be due to invalid IL or missing references)
		//IL_1367: Unknown result type (might be due to invalid IL or missing references)
		//IL_136c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ceb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1474: Unknown result type (might be due to invalid IL or missing references)
		//IL_1479: Unknown result type (might be due to invalid IL or missing references)
		//IL_13ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_13b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d30: Unknown result type (might be due to invalid IL or missing references)
		//IL_14bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_14c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1501: Unknown result type (might be due to invalid IL or missing references)
		//IL_1506: Unknown result type (might be due to invalid IL or missing references)
		GamePadButtons buttons;
		if (table.gameMode == 0)
		{
			bool flag = false;
			GamePadState state = GamePad.GetState((PlayerIndex)0);
			GamePadState state2 = GamePad.GetState((PlayerIndex)1);
			GamePadState state3 = GamePad.GetState((PlayerIndex)2);
			GamePadState state4 = GamePad.GetState((PlayerIndex)3);
			buttons = ((GamePadState)(ref state)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).Start == 1)
			{
				table.menuController = 1;
				table.redController = 1;
				flag = true;
			}
			buttons = ((GamePadState)(ref state2)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).Start == 1)
			{
				table.menuController = 2;
				table.redController = 2;
				flag = true;
			}
			buttons = ((GamePadState)(ref state3)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).Start == 1)
			{
				table.menuController = 3;
				table.redController = 3;
				flag = true;
			}
			buttons = ((GamePadState)(ref state4)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).Start == 1)
			{
				table.menuController = 4;
				table.redController = 4;
				flag = true;
			}
			if (flag)
			{
				table.kickHard.Play();
				table.gameMode = 1;
				menuDelayCounter = menuDelay;
				table.blueController = 6;
			}
			return;
		}
		if (table.menuController == 1)
		{
			menuGP = GamePad.GetState((PlayerIndex)0);
		}
		if (table.menuController == 2)
		{
			menuGP = GamePad.GetState((PlayerIndex)1);
		}
		if (table.menuController == 3)
		{
			menuGP = GamePad.GetState((PlayerIndex)2);
		}
		if (table.menuController == 4)
		{
			menuGP = GamePad.GetState((PlayerIndex)3);
		}
		GamePadDPad dPad;
		GamePadThumbSticks thumbSticks;
		if (table.gameMode == 1)
		{
			if (menuDelayCounter <= 0)
			{
				if (pointerLag > 0)
				{
					pointerLag--;
					goto IL_03a3;
				}
				dPad = ((GamePadState)(ref menuGP)).DPad;
				if ((int)((GamePadDPad)(ref dPad)).Down != 1)
				{
					thumbSticks = ((GamePadState)(ref menuGP)).ThumbSticks;
					if (!((double)((GamePadThumbSticks)(ref thumbSticks)).Left.Y < -0.5))
					{
						goto IL_02fd;
					}
				}
				pointerLag = setPointerLag;
				pointerPointAt++;
				if (pointerPointAt > 3)
				{
					pointerPointAt = 3;
					table.wallBounce.Play();
				}
				else
				{
					table.kickSoft.Play();
				}
				goto IL_02fd;
			}
			menuDelayCounter--;
		}
		goto IL_0476;
		IL_02fd:
		dPad = ((GamePadState)(ref menuGP)).DPad;
		if ((int)((GamePadDPad)(ref dPad)).Up != 1)
		{
			thumbSticks = ((GamePadState)(ref menuGP)).ThumbSticks;
			if (!((double)((GamePadThumbSticks)(ref thumbSticks)).Left.Y > 0.5))
			{
				goto IL_03a3;
			}
		}
		pointerLag = setPointerLag;
		pointerPointAt--;
		if (pointerPointAt < 0)
		{
			pointerPointAt = 0;
			table.wallBounce.Play();
		}
		else
		{
			table.kickSoft.Play();
		}
		goto IL_03a3;
		IL_03a3:
		buttons = ((GamePadState)(ref menuGP)).Buttons;
		if ((int)((GamePadButtons)(ref buttons)).B == 1)
		{
			table.wallBounce.Play();
			menuDelayCounter = menuDelay;
			table.gameMode = 0;
		}
		buttons = ((GamePadState)(ref menuGP)).Buttons;
		if ((int)((GamePadButtons)(ref buttons)).A == 1)
		{
			if (pointerPointAt == 0)
			{
				table.kickHard.Play();
				menuDelayCounter = menuDelay;
				table.gameMode = 2;
				pointerPointAt = 0;
			}
			if (pointerPointAt == 3)
			{
				((GameComponent)this).Game.Exit();
			}
		}
		goto IL_0476;
		IL_17f9:
		buttons = ((GamePadState)(ref menuGP)).Buttons;
		if ((int)((GamePadButtons)(ref buttons)).B == 1)
		{
			table.wallBounce.Play();
			menuDelayCounter = menuDelay;
			table.gameMode = 1;
			pointerPointAt = 0;
		}
		goto IL_184d;
		IL_0969:
		dPad = ((GamePadState)(ref menuGP)).DPad;
		if ((int)((GamePadDPad)(ref dPad)).Left != 1)
		{
			thumbSticks = ((GamePadState)(ref menuGP)).ThumbSticks;
			if (!((double)((GamePadThumbSticks)(ref thumbSticks)).Left.X < -0.5))
			{
				goto IL_17f9;
			}
		}
		if (table.redController == 1)
		{
			pointerLag = setPointerLag;
			table.wallBounce.Play();
		}
		GamePadState state5;
		if (table.redController == 2)
		{
			state5 = GamePad.GetState((PlayerIndex)0);
			if (((GamePadState)(ref state5)).IsConnected)
			{
				table.redController = 1;
				pointerLag = setPointerLag;
				table.kickSoft.Play();
			}
			else
			{
				pointerLag = setPointerLag;
				table.wallBounce.Play();
			}
		}
		if (table.redController == 3)
		{
			state5 = GamePad.GetState((PlayerIndex)1);
			if (((GamePadState)(ref state5)).IsConnected)
			{
				table.redController = 2;
				pointerLag = setPointerLag;
				table.kickSoft.Play();
			}
			else
			{
				state5 = GamePad.GetState((PlayerIndex)0);
				if (((GamePadState)(ref state5)).IsConnected)
				{
					table.redController = 1;
					pointerLag = setPointerLag;
					table.kickSoft.Play();
				}
				else
				{
					pointerLag = setPointerLag;
					table.wallBounce.Play();
				}
			}
		}
		if (table.redController == 4)
		{
			state5 = GamePad.GetState((PlayerIndex)2);
			if (((GamePadState)(ref state5)).IsConnected)
			{
				table.redController = 3;
				pointerLag = setPointerLag;
				table.kickSoft.Play();
			}
			else
			{
				state5 = GamePad.GetState((PlayerIndex)1);
				if (((GamePadState)(ref state5)).IsConnected)
				{
					table.redController = 2;
					pointerLag = setPointerLag;
					table.kickSoft.Play();
				}
				else
				{
					state5 = GamePad.GetState((PlayerIndex)0);
					if (((GamePadState)(ref state5)).IsConnected)
					{
						table.redController = 1;
						pointerLag = setPointerLag;
						table.kickSoft.Play();
					}
					else
					{
						pointerLag = setPointerLag;
						table.wallBounce.Play();
					}
				}
			}
		}
		if (table.redController == 5)
		{
			state5 = GamePad.GetState((PlayerIndex)3);
			if (((GamePadState)(ref state5)).IsConnected)
			{
				table.redController = 4;
				pointerLag = setPointerLag;
				table.kickSoft.Play();
			}
			else
			{
				state5 = GamePad.GetState((PlayerIndex)2);
				if (((GamePadState)(ref state5)).IsConnected)
				{
					table.redController = 3;
					pointerLag = setPointerLag;
					table.kickSoft.Play();
				}
				else
				{
					state5 = GamePad.GetState((PlayerIndex)1);
					if (((GamePadState)(ref state5)).IsConnected)
					{
						table.redController = 2;
						pointerLag = setPointerLag;
						table.kickSoft.Play();
					}
					else
					{
						state5 = GamePad.GetState((PlayerIndex)0);
						if (((GamePadState)(ref state5)).IsConnected)
						{
							table.redController = 1;
							pointerLag = setPointerLag;
							table.kickSoft.Play();
						}
					}
				}
			}
		}
		if (table.redController == 6 || table.redController == 7)
		{
			pointerLag = setPointerLag;
			table.redController--;
			table.kickSoft.Play();
		}
		goto IL_17f9;
		IL_0ec8:
		dPad = ((GamePadState)(ref menuGP)).DPad;
		if ((int)((GamePadDPad)(ref dPad)).Left != 1)
		{
			thumbSticks = ((GamePadState)(ref menuGP)).ThumbSticks;
			if (!((double)((GamePadThumbSticks)(ref thumbSticks)).Left.X < -0.5))
			{
				goto IL_17f9;
			}
		}
		pointerLag = setPointerLag;
		if (table.redController > 4)
		{
			table.wallBounce.Play();
		}
		else
		{
			table.redStickStyle--;
			if (table.redStickStyle < 0)
			{
				table.redStickStyle = 0;
				table.wallBounce.Play();
			}
			else
			{
				table.kickSoft.Play();
			}
		}
		goto IL_17f9;
		IL_184d:
		if (table.gameMode == 3)
		{
		}
		if (table.gameMode == 4)
		{
		}
		if (table.gameMode == 6)
		{
			menuDelayCounter--;
			if (menuDelayCounter == 0)
			{
				table.gameMode = 7;
				menuDelayCounter = menuDelay;
			}
			else
			{
				((GameComponent)table).Update(gameTime);
			}
		}
		if (table.gameMode == 7)
		{
			((GameComponent)table).Update(gameTime);
		}
		if (table.gameMode == 9)
		{
			((GameComponent)table).Update(gameTime);
		}
		((GameComponent)this).Update(gameTime);
		return;
		IL_0732:
		dPad = ((GamePadState)(ref menuGP)).DPad;
		if ((int)((GamePadDPad)(ref dPad)).Left != 1)
		{
			thumbSticks = ((GamePadState)(ref menuGP)).ThumbSticks;
			if (!((double)((GamePadThumbSticks)(ref thumbSticks)).Left.X < -0.5))
			{
				goto IL_17f9;
			}
		}
		pointerLag = setPointerLag;
		GS_TimeLimitSliderIndex--;
		if (GS_TimeLimitSliderIndex < 0)
		{
			GS_TimeLimitSliderIndex = 0;
			table.wallBounce.Play();
		}
		else
		{
			table.kickSoft.Play();
		}
		goto IL_17f9;
		IL_169e:
		dPad = ((GamePadState)(ref menuGP)).DPad;
		if ((int)((GamePadDPad)(ref dPad)).Left != 1)
		{
			thumbSticks = ((GamePadState)(ref menuGP)).ThumbSticks;
			if (!((double)((GamePadThumbSticks)(ref thumbSticks)).Left.X < -0.5))
			{
				goto IL_17f9;
			}
		}
		pointerLag = setPointerLag;
		if (table.blueController > 4)
		{
			table.wallBounce.Play();
		}
		else
		{
			table.blueStickStyle--;
			if (table.blueStickStyle < 0)
			{
				table.blueStickStyle = 0;
				table.wallBounce.Play();
			}
			else
			{
				table.kickSoft.Play();
			}
		}
		goto IL_17f9;
		IL_113f:
		dPad = ((GamePadState)(ref menuGP)).DPad;
		if ((int)((GamePadDPad)(ref dPad)).Left != 1)
		{
			thumbSticks = ((GamePadState)(ref menuGP)).ThumbSticks;
			if (!((double)((GamePadThumbSticks)(ref thumbSticks)).Left.X < -0.5))
			{
				goto IL_17f9;
			}
		}
		if (table.blueController == 1)
		{
			pointerLag = setPointerLag;
			table.wallBounce.Play();
		}
		if (table.blueController == 2)
		{
			state5 = GamePad.GetState((PlayerIndex)0);
			if (((GamePadState)(ref state5)).IsConnected)
			{
				table.blueController = 1;
				pointerLag = setPointerLag;
				table.kickSoft.Play();
			}
			else
			{
				pointerLag = setPointerLag;
				table.wallBounce.Play();
			}
		}
		if (table.blueController == 3)
		{
			state5 = GamePad.GetState((PlayerIndex)1);
			if (((GamePadState)(ref state5)).IsConnected)
			{
				table.blueController = 2;
				pointerLag = setPointerLag;
				table.kickSoft.Play();
			}
			else
			{
				state5 = GamePad.GetState((PlayerIndex)0);
				if (((GamePadState)(ref state5)).IsConnected)
				{
					table.blueController = 1;
					pointerLag = setPointerLag;
					table.kickSoft.Play();
				}
				else
				{
					pointerLag = setPointerLag;
					table.wallBounce.Play();
				}
			}
		}
		if (table.blueController == 4)
		{
			state5 = GamePad.GetState((PlayerIndex)2);
			if (((GamePadState)(ref state5)).IsConnected)
			{
				table.blueController = 3;
				pointerLag = setPointerLag;
				table.kickSoft.Play();
			}
			else
			{
				state5 = GamePad.GetState((PlayerIndex)1);
				if (((GamePadState)(ref state5)).IsConnected)
				{
					table.blueController = 2;
					pointerLag = setPointerLag;
					table.kickSoft.Play();
				}
				else
				{
					state5 = GamePad.GetState((PlayerIndex)0);
					if (((GamePadState)(ref state5)).IsConnected)
					{
						table.blueController = 1;
						pointerLag = setPointerLag;
						table.kickSoft.Play();
					}
					else
					{
						pointerLag = setPointerLag;
						table.wallBounce.Play();
					}
				}
			}
		}
		if (table.blueController == 5)
		{
			state5 = GamePad.GetState((PlayerIndex)3);
			if (((GamePadState)(ref state5)).IsConnected)
			{
				table.blueController = 4;
				pointerLag = setPointerLag;
				table.kickSoft.Play();
			}
			else
			{
				state5 = GamePad.GetState((PlayerIndex)2);
				if (((GamePadState)(ref state5)).IsConnected)
				{
					table.blueController = 3;
					pointerLag = setPointerLag;
					table.kickSoft.Play();
				}
				else
				{
					state5 = GamePad.GetState((PlayerIndex)1);
					if (((GamePadState)(ref state5)).IsConnected)
					{
						table.blueController = 2;
						pointerLag = setPointerLag;
						table.kickSoft.Play();
					}
					else
					{
						state5 = GamePad.GetState((PlayerIndex)0);
						if (((GamePadState)(ref state5)).IsConnected)
						{
							table.blueController = 1;
							pointerLag = setPointerLag;
							table.kickSoft.Play();
						}
					}
				}
			}
		}
		if (table.blueController == 6 || table.blueController == 7)
		{
			pointerLag = setPointerLag;
			table.blueController--;
			table.kickSoft.Play();
		}
		goto IL_17f9;
		IL_0476:
		if (table.gameMode == 2)
		{
			if (table.redController > 4)
			{
				table.redStickStyle = 0;
			}
			if (table.blueController > 4)
			{
				table.blueStickStyle = 0;
			}
			if (menuDelayCounter <= 0)
			{
				if (pointerLag > 0)
				{
					pointerLag--;
				}
				else
				{
					dPad = ((GamePadState)(ref menuGP)).DPad;
					if ((int)((GamePadDPad)(ref dPad)).Down != 1)
					{
						thumbSticks = ((GamePadState)(ref menuGP)).ThumbSticks;
						if (!((double)((GamePadThumbSticks)(ref thumbSticks)).Left.Y < -0.5))
						{
							dPad = ((GamePadState)(ref menuGP)).DPad;
							if ((int)((GamePadDPad)(ref dPad)).Up != 1)
							{
								thumbSticks = ((GamePadState)(ref menuGP)).ThumbSticks;
								if (!((double)((GamePadThumbSticks)(ref thumbSticks)).Left.Y > 0.5))
								{
									if (pointerPointAt == 0)
									{
										dPad = ((GamePadState)(ref menuGP)).DPad;
										if ((int)((GamePadDPad)(ref dPad)).Right != 1)
										{
											thumbSticks = ((GamePadState)(ref menuGP)).ThumbSticks;
											if (!((double)((GamePadThumbSticks)(ref thumbSticks)).Left.X > 0.5))
											{
												goto IL_0732;
											}
										}
										pointerLag = setPointerLag;
										GS_TimeLimitSliderIndex++;
										if (GS_TimeLimitSliderIndex > 5)
										{
											GS_TimeLimitSliderIndex = 5;
											table.wallBounce.Play();
										}
										else
										{
											table.kickSoft.Play();
										}
										goto IL_0732;
									}
									if (pointerPointAt == 1)
									{
										dPad = ((GamePadState)(ref menuGP)).DPad;
										if ((int)((GamePadDPad)(ref dPad)).Right != 1)
										{
											thumbSticks = ((GamePadState)(ref menuGP)).ThumbSticks;
											if (!((double)((GamePadThumbSticks)(ref thumbSticks)).Left.X > 0.5))
											{
												goto IL_0969;
											}
										}
										pointerLag = setPointerLag;
										table.redController++;
										if (table.redController == 2)
										{
											state5 = GamePad.GetState((PlayerIndex)1);
											if (!((GamePadState)(ref state5)).IsConnected)
											{
												table.redController++;
											}
										}
										if (table.redController == 3)
										{
											state5 = GamePad.GetState((PlayerIndex)2);
											if (!((GamePadState)(ref state5)).IsConnected)
											{
												table.redController++;
											}
										}
										if (table.redController == 4)
										{
											state5 = GamePad.GetState((PlayerIndex)3);
											if (!((GamePadState)(ref state5)).IsConnected)
											{
												table.redController++;
											}
										}
										if (table.redController > 7)
										{
											table.redController = 7;
											table.wallBounce.Play();
										}
										else
										{
											table.kickSoft.Play();
										}
										goto IL_0969;
									}
									if (pointerPointAt == 2)
									{
										dPad = ((GamePadState)(ref menuGP)).DPad;
										if ((int)((GamePadDPad)(ref dPad)).Right != 1)
										{
											thumbSticks = ((GamePadState)(ref menuGP)).ThumbSticks;
											if (!((double)((GamePadThumbSticks)(ref thumbSticks)).Left.X > 0.5))
											{
												goto IL_0ec8;
											}
										}
										pointerLag = setPointerLag;
										if (table.redController > 4)
										{
											table.wallBounce.Play();
										}
										else
										{
											table.redStickStyle++;
											if (table.redStickStyle > 2)
											{
												table.redStickStyle = 2;
												table.wallBounce.Play();
											}
											else
											{
												table.kickSoft.Play();
											}
										}
										goto IL_0ec8;
									}
									if (pointerPointAt == 3)
									{
										dPad = ((GamePadState)(ref menuGP)).DPad;
										if ((int)((GamePadDPad)(ref dPad)).Right != 1)
										{
											thumbSticks = ((GamePadState)(ref menuGP)).ThumbSticks;
											if (!((double)((GamePadThumbSticks)(ref thumbSticks)).Left.X > 0.5))
											{
												goto IL_113f;
											}
										}
										pointerLag = setPointerLag;
										table.blueController++;
										if (table.blueController == 2)
										{
											state5 = GamePad.GetState((PlayerIndex)1);
											if (!((GamePadState)(ref state5)).IsConnected)
											{
												table.blueController++;
											}
										}
										if (table.blueController == 3)
										{
											state5 = GamePad.GetState((PlayerIndex)2);
											if (!((GamePadState)(ref state5)).IsConnected)
											{
												table.blueController++;
											}
										}
										if (table.blueController == 4)
										{
											state5 = GamePad.GetState((PlayerIndex)3);
											if (!((GamePadState)(ref state5)).IsConnected)
											{
												table.blueController++;
											}
										}
										if (table.blueController > 7)
										{
											table.blueController = 7;
											table.wallBounce.Play();
										}
										else
										{
											table.kickSoft.Play();
										}
										goto IL_113f;
									}
									if (pointerPointAt == 4)
									{
										dPad = ((GamePadState)(ref menuGP)).DPad;
										if ((int)((GamePadDPad)(ref dPad)).Right != 1)
										{
											thumbSticks = ((GamePadState)(ref menuGP)).ThumbSticks;
											if (!((double)((GamePadThumbSticks)(ref thumbSticks)).Left.X > 0.5))
											{
												goto IL_169e;
											}
										}
										pointerLag = setPointerLag;
										if (table.blueController > 4)
										{
											table.wallBounce.Play();
										}
										else
										{
											table.blueStickStyle++;
											if (table.blueStickStyle > 2)
											{
												table.blueStickStyle = 2;
												table.wallBounce.Play();
											}
											else
											{
												table.kickSoft.Play();
											}
										}
										goto IL_169e;
									}
									if (pointerPointAt == 5)
									{
										buttons = ((GamePadState)(ref menuGP)).Buttons;
										if ((int)((GamePadButtons)(ref buttons)).A == 1)
										{
											pointerPointAt = 0;
											table.timeLimitIndex = GS_TimeLimitSliderIndex;
											menuDelayCounter = menuDelay * 2;
											table.resetTable();
											table.gameMode = 6;
										}
									}
									goto IL_17f9;
								}
							}
							pointerLag = setPointerLag;
							pointerPointAt--;
							if (pointerPointAt < 0)
							{
								pointerPointAt = 0;
								table.wallBounce.Play();
							}
							else
							{
								table.kickSoft.Play();
							}
							goto IL_17f9;
						}
					}
					pointerLag = setPointerLag;
					pointerPointAt++;
					if (pointerPointAt > 5)
					{
						pointerPointAt = 5;
						table.wallBounce.Play();
					}
					else
					{
						table.kickSoft.Play();
					}
				}
				goto IL_17f9;
			}
			menuDelayCounter--;
		}
		goto IL_184d;
	}

	public void Draw()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0963: Unknown result type (might be due to invalid IL or missing references)
		//IL_0965: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0515: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0610: Unknown result type (might be due to invalid IL or missing references)
		//IL_0615: Unknown result type (might be due to invalid IL or missing references)
		//IL_0598: Unknown result type (might be due to invalid IL or missing references)
		//IL_059d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0685: Unknown result type (might be due to invalid IL or missing references)
		//IL_068a: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0704: Unknown result type (might be due to invalid IL or missing references)
		//IL_0709: Unknown result type (might be due to invalid IL or missing references)
		//IL_0790: Unknown result type (might be due to invalid IL or missing references)
		//IL_0795: Unknown result type (might be due to invalid IL or missing references)
		//IL_0715: Unknown result type (might be due to invalid IL or missing references)
		//IL_071a: Unknown result type (might be due to invalid IL or missing references)
		//IL_088b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0890: Unknown result type (might be due to invalid IL or missing references)
		//IL_0813: Unknown result type (might be due to invalid IL or missing references)
		//IL_0818: Unknown result type (might be due to invalid IL or missing references)
		Rectangle titleSafeArea = GetTitleSafeArea(1f);
		Rectangle val = default(Rectangle);
		((Rectangle)(ref val))._002Ector((titleSafeArea.Width - titleSafeArea.Height) / 2, titleSafeArea.Y, titleSafeArea.Height, titleSafeArea.Height);
		int num = (int)((double)titleSafeArea.Height * 0.0825);
		int num2 = (int)((double)titleSafeArea.Height * 0.05575158786167961);
		int num3 = (int)((double)num2 * 1.4050632911392404);
		GamePadState state;
		if (table.gameMode == 0)
		{
			menuSpriteBatch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)1);
			menuSpriteBatch.Draw(PressStart, val, Color.White);
			menuSpriteBatch.End();
		}
		else if (table.gameMode == 1)
		{
			if (menuDelayCounter > 0)
			{
				menuSpriteBatch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)1);
				menuSpriteBatch.Draw(BlankMenu, val, Color.White);
				menuSpriteBatch.End();
			}
			else
			{
				pointerPosY = (int)((double)titleSafeArea.Height * 0.4234297812279464 + (double)(num * pointerPointAt)) - num / 2;
				pointerPosX = (int)((double)titleSafeArea.Height * 0.58);
				menuSpriteBatch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)1);
				menuSpriteBatch.Draw(MainMenu, val, Color.White);
				menuSpriteBatch.Draw(PointerT, new Rectangle(pointerPosX, pointerPosY, num2, num3), Color.WhiteSmoke);
				menuSpriteBatch.End();
			}
		}
		else if (table.gameMode == 2)
		{
			int[] array = new int[6]
			{
				(int)((double)titleSafeArea.Height * 0.3),
				(int)((double)titleSafeArea.Height * 0.42),
				(int)((double)titleSafeArea.Height * 0.49),
				(int)((double)titleSafeArea.Height * 0.603),
				(int)((double)titleSafeArea.Height * 0.673),
				(int)((double)titleSafeArea.Height * 0.785)
			};
			int num4 = (num4 = (int)((double)titleSafeArea.Height * 0.24));
			if (menuDelayCounter <= 0)
			{
				menuSpriteBatch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)1);
				menuSpriteBatch.Draw(GameSetup, val, Color.White);
				menuSpriteBatch.Draw(PointerT, new Rectangle(num4, array[pointerPointAt], num2, num3), Color.WhiteSmoke);
				menuSpriteBatch.Draw(GS_TimeLimitSlider[GS_TimeLimitSliderIndex], new Rectangle((int)((double)titleSafeArea.Height * 0.8), (int)((double)titleSafeArea.Height * 0.31), (int)((double)titleSafeArea.Height * 0.3), (int)((double)titleSafeArea.Height * 0.07)), Color.White);
				menuSpriteBatch.Draw(GS_PlayerSelectT[table.redController], new Rectangle((int)((double)titleSafeArea.Height * 0.8), (int)((double)titleSafeArea.Height * 0.43), (int)((double)titleSafeArea.Height * 0.3), (int)((double)titleSafeArea.Height * 0.07)), Color.White);
				if (table.redController == 2)
				{
					state = GamePad.GetState((PlayerIndex)0);
					if (((GamePadState)(ref state)).IsConnected)
					{
						goto IL_04ba;
					}
				}
				if (table.redController != 3)
				{
					goto IL_0469;
				}
				state = GamePad.GetState((PlayerIndex)0);
				if (!((GamePadState)(ref state)).IsConnected)
				{
					state = GamePad.GetState((PlayerIndex)1);
					if (!((GamePadState)(ref state)).IsConnected)
					{
						goto IL_0469;
					}
				}
				goto IL_04ba;
			}
			menuSpriteBatch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)1);
			menuSpriteBatch.Draw(BlankMenu, val, Color.White);
			menuSpriteBatch.End();
		}
		else
		{
			table.DrawTable();
		}
		goto IL_08ba;
		IL_07a1:
		if (table.blueController > 4)
		{
			menuSpriteBatch.Draw(GS_StickStyle[3], new Rectangle((int)((double)titleSafeArea.Height * 0.8), (int)((double)titleSafeArea.Height * 0.68), (int)((double)titleSafeArea.Height * 0.3), (int)((double)titleSafeArea.Height * 0.07)), Color.White);
		}
		else
		{
			menuSpriteBatch.Draw(GS_StickStyle[table.blueStickStyle], new Rectangle((int)((double)titleSafeArea.Height * 0.8), (int)((double)titleSafeArea.Height * 0.68), (int)((double)titleSafeArea.Height * 0.3), (int)((double)titleSafeArea.Height * 0.07)), Color.White);
		}
		menuSpriteBatch.End();
		goto IL_08ba;
		IL_06e4:
		if (table.blueController == 4)
		{
			state = GamePad.GetState((PlayerIndex)0);
			if (!((GamePadState)(ref state)).IsConnected)
			{
				state = GamePad.GetState((PlayerIndex)1);
				if (!((GamePadState)(ref state)).IsConnected)
				{
					state = GamePad.GetState((PlayerIndex)2);
					if (!((GamePadState)(ref state)).IsConnected)
					{
						goto IL_07a1;
					}
				}
			}
			goto IL_0735;
		}
		goto IL_07a1;
		IL_0526:
		if (table.redController > 4)
		{
			menuSpriteBatch.Draw(GS_StickStyle[3], new Rectangle((int)((double)titleSafeArea.Height * 0.8), (int)((double)titleSafeArea.Height * 0.5), (int)((double)titleSafeArea.Height * 0.3), (int)((double)titleSafeArea.Height * 0.07)), Color.White);
		}
		else
		{
			menuSpriteBatch.Draw(GS_StickStyle[table.redStickStyle], new Rectangle((int)((double)titleSafeArea.Height * 0.8), (int)((double)titleSafeArea.Height * 0.5), (int)((double)titleSafeArea.Height * 0.3), (int)((double)titleSafeArea.Height * 0.07)), Color.White);
		}
		menuSpriteBatch.Draw(GS_PlayerSelectT[table.blueController], new Rectangle((int)((double)titleSafeArea.Height * 0.8), (int)((double)titleSafeArea.Height * 0.61), (int)((double)titleSafeArea.Height * 0.3), (int)((double)titleSafeArea.Height * 0.07)), Color.White);
		if (table.blueController == 2)
		{
			state = GamePad.GetState((PlayerIndex)0);
			if (((GamePadState)(ref state)).IsConnected)
			{
				goto IL_0735;
			}
		}
		if (table.blueController != 3)
		{
			goto IL_06e4;
		}
		state = GamePad.GetState((PlayerIndex)0);
		if (!((GamePadState)(ref state)).IsConnected)
		{
			state = GamePad.GetState((PlayerIndex)1);
			if (!((GamePadState)(ref state)).IsConnected)
			{
				goto IL_06e4;
			}
		}
		goto IL_0735;
		IL_04ba:
		menuSpriteBatch.Draw(GS_PlayerSelectT[0], new Rectangle((int)((double)titleSafeArea.Height * 0.8), (int)((double)titleSafeArea.Height * 0.43), (int)((double)titleSafeArea.Height * 0.3), (int)((double)titleSafeArea.Height * 0.07)), Color.White);
		goto IL_0526;
		IL_0735:
		menuSpriteBatch.Draw(GS_PlayerSelectT[0], new Rectangle((int)((double)titleSafeArea.Height * 0.8), (int)((double)titleSafeArea.Height * 0.61), (int)((double)titleSafeArea.Height * 0.3), (int)((double)titleSafeArea.Height * 0.07)), Color.White);
		goto IL_07a1;
		IL_08ba:
		if (table.gameMode == 9)
		{
			int num5 = titleSafeArea.Width / 2;
			int num6 = (int)((double)titleSafeArea.Width / 2.0 * 0.6250944822373394);
			Rectangle val2 = default(Rectangle);
			((Rectangle)(ref val2))._002Ector((titleSafeArea.Width - num5) / 2, (titleSafeArea.Height - num6) / 3, num5, num6);
			menuSpriteBatch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)1);
			if (table.redScore > table.blueScore)
			{
				menuSpriteBatch.Draw(FinalScores[0], val2, Color.WhiteSmoke);
			}
			else if (table.redScore < table.blueScore)
			{
				menuSpriteBatch.Draw(FinalScores[1], val2, Color.WhiteSmoke);
			}
			else
			{
				menuSpriteBatch.Draw(FinalScores[2], val2, Color.WhiteSmoke);
			}
			menuSpriteBatch.End();
		}
		return;
		IL_0469:
		if (table.redController == 4)
		{
			state = GamePad.GetState((PlayerIndex)0);
			if (!((GamePadState)(ref state)).IsConnected)
			{
				state = GamePad.GetState((PlayerIndex)1);
				if (!((GamePadState)(ref state)).IsConnected)
				{
					state = GamePad.GetState((PlayerIndex)2);
					if (!((GamePadState)(ref state)).IsConnected)
					{
						goto IL_0526;
					}
				}
			}
			goto IL_04ba;
		}
		goto IL_0526;
	}

	protected Rectangle GetTitleSafeArea(float percent)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		Viewport viewport = ((GameComponent)this).Game.GraphicsDevice.Viewport;
		int x = ((Viewport)(ref viewport)).X;
		viewport = ((GameComponent)this).Game.GraphicsDevice.Viewport;
		int y = ((Viewport)(ref viewport)).Y;
		viewport = ((GameComponent)this).Game.GraphicsDevice.Viewport;
		int width = ((Viewport)(ref viewport)).Width;
		viewport = ((GameComponent)this).Game.GraphicsDevice.Viewport;
		Rectangle result = default(Rectangle);
		((Rectangle)(ref result))._002Ector(x, y, width, ((Viewport)(ref viewport)).Height);
		float num = (1f - percent) / 2f;
		result.X = (int)(num * (float)result.Width);
		result.Y = (int)(num * (float)result.Height);
		result.Width = (int)(percent * (float)result.Width);
		result.Height = (int)(percent * (float)result.Height);
		return result;
	}
}
