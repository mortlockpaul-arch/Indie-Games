using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Xclna.Xna.Animation;

namespace OluXNA;

internal class BaseGame
{
	public enum Beats
	{
		Whole = 16,
		Half = 8,
		Quarter = 4,
		Eighth = 2,
		Sixteenth = 1
	}

	public enum TriBeats
	{
		Whole = 12,
		Quarter = 4,
		Eighth = 2,
		Sixteenth = 1
	}

	public static int WIDTH;

	public static int HEIGHT;

	public static string fontName;

	public static string bigFontName;

	public static int GAP;

	public static int S_WIDTH;

	public static int S_HEIGHT;

	public static int C_WIDTH;

	public static int FOG_START;

	public static int FOG_END;

	public static int CHANNEL_NUM;

	public static float BEAT;

	public static float frameRat;

	public static float gravFactor;

	public bool MEGA_ON;

	public static float FREEZE_TIME;

	public float freezeLeft;

	public bool FREEZE_ON;

	public GameTime emptytime;

	public static bool demo;

	public static bool quickload;

	public static bool credits;

	public static bool release;

	public static bool PROFILE;

	public static bool bloom_on;

	public static string contentRoot;

	public Thread loadThread;

	public bool contentLoad;

	public bool continueWithoutSaving;

	public object modelLock = new object();

	public float textFlash;

	public float flashInc;

	private Game coreGame;

	private float _curTime;

	private float _freezeTime;

	private int _curBeat;

	public int maxBeat;

	private int _freezeBeat;

	public bool playBGMusic;

	public int zoneToLoad = -1;

	public bool selector;

	public int numTargeted;

	public bool paused;

	public bool invert;

	public bool rumble = true;

	public bool HSSaved;

	public bool PlayerSaved;

	public bool HSLoad;

	public bool PlayerLoad;

	private float hitCountdown;

	private float maxCountdown = 3f;

	public FillMode fillMode;

	public List<TargetEffect> targetFX;

	private GraphicsDeviceManager _graphics;

	private ContentManager _content;

	private ModelManager _models;

	private SpriteBatch _spriteBatch;

	private VertexDeclaration _vertDec;

	public VertexDeclaration VPNTDec;

	private Matrix _view;

	private Matrix _world;

	private Matrix _projection;

	private Effect _fogEffect;

	public static Vector4 T_MIX;

	public static Vector4 T_ADD;

	public static Vector4 T_TEX;

	public static Vector4 T_MUL;

	private bool _specifyAlpha;

	private Effect _alphaEffect;

	private Effect _blurEffect;

	private Effect _combineEffect;

	private Effect _flattenEffect;

	public ParticleSystem ps;

	public SkyParticleSystem skyPS;

	public float skyCooldown;

	public float skyMaxCooldown;

	public Vector3 skyFlow;

	public bool skyFlowToggle = true;

	private RenderTarget2D _worldTarget;

	private RenderTarget2D _glowTarget;

	private RenderTarget2D _glowTarget2;

	private RenderTarget2D _glowTarget3;

	private BasicEffect _flatEffect;

	private VertexBuffer fullScreenBuff;

	private VertexDeclaration fullScreenDec;

	private bool _debug;

	private Level _level;

	private Vector3 _cursorDir;

	private Vector3 _cursorUp;

	private Vector3 _cameraLoc;

	private Vector3 _cameraDir;

	private Vector3 _cameraUp;

	private Vector3 _cameraTarget;

	private Vector3 _playerPos;

	private Vector3 _playerDir;

	private Vector3 _playerUp;

	private List<Vector2> _window;

	private List<TargetEffect> _fx;

	private int _score;

	public int[] powerScore;

	public int[] powerAmounts;

	private int[] _maxPower;

	private List<Enemy> _enems;

	private Player _player;

	private List<FallingObject> _fallFX;

	private string _textFlow;

	private ScoreGroupCol _scoreFlow;

	private List<VertexPositionColor[]> fog;

	private double _totalTime;

	public int actualEnem;

	public bool movingToNextZone;

	public int zoneEndTime = 32;

	public int elaspedEndTime;

	public float[] channels = new float[CHANNEL_NUM];

	private TargetEffectCol selFX;

	private TargetEffectCol maxFX;

	private SoundBank sB;

	private AudioEngine engine;

	private WaveBank wavebank;

	private SoundBank sBOpen;

	private AudioEngine engineOpen;

	private WaveBank wavebankOpen;

	private List<Cue> activeCues;

	private List<Cue> bgCues;

	private GraphicsStack _matStack;

	private GraphicsStack _flatStack;

	private bool _firstPass;

	private bool _freezeFirstPass;

	private float _paneDepth;

	private bool _firstBeat = true;

	private bool _freezeFirstBeat;

	private bool _zoneBeat = true;

	private HUD _hud;

	private Texture2D _backTex;

	private float[] horBlurSampleWeights;

	private float[] verBlurSampleWeights;

	private Vector2[] horBlurSampleOffsets;

	private Vector2[] verBlurSampleOffsets;

	private InputState _input;

	public PythDigitCollection pythdigit01;

	public Fish01Collection fish01;

	public SerpentTailCollection sTail01;

	public TextDisplayCol tdColl;

	public TextDisplay2Col tdColl2;

	private bool _easyMode;

	public Menu levelMenu;

	public Menu optionMenu;

	public bool levelLoaded;

	public Matrix targetTransform;

	public Matrix worldViewProjTransform;

	public TitleSaveData hiScores;

	public UserSaveData curUserData;

	public StorageDevice storageDevice;

	public StorageDevice globStorageDevice;

	public StorageContainer storageContainer;

	public StorageContainer globStorageContainer;

	private Random _r;

	private static BaseGame obj;

	private DepthStencilBuffer bitTarget;

	private bool FXLoaded;

	public static int CAN_TARGET => (3 * FOG_END + FOG_START) / 4;

	public float flashMod => 1f - textFlash * textFlash;

	public Game CoreGame => coreGame;

	public float curTime
	{
		get
		{
			if (FREEZE_ON)
			{
				return _freezeTime;
			}
			return _curTime;
		}
		set
		{
			if (FREEZE_ON)
			{
				_freezeTime = value;
			}
			else
			{
				_curTime = value;
			}
		}
	}

	public int curBeat
	{
		get
		{
			if (FREEZE_ON)
			{
				return _freezeBeat;
			}
			return _curBeat;
		}
		set
		{
			if (FREEZE_ON)
			{
				_freezeBeat = value;
			}
			else
			{
				_curBeat = value;
			}
		}
	}

	public float HitProgress => hitCountdown / maxCountdown;

	public GraphicsDeviceManager graphics => _graphics;

	public ContentManager content => _content;

	public ModelManager models => _models;

	public SpriteBatch spriteBatch => _spriteBatch;

	public VertexDeclaration VertDec => _vertDec;

	public Matrix viewMatrix
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _view;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_view = value;
		}
	}

	public Matrix world
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _world;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_world = value;
		}
	}

	public Matrix projectionMatrix
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _projection;
		}
	}

	public Effect fogEffect => _fogEffect;

	public bool SpecifyAlpha
	{
		get
		{
			return _specifyAlpha;
		}
		set
		{
			_specifyAlpha = value;
		}
	}

	public Effect combineEffect => _combineEffect;

	public RenderTarget2D worldTarget => _worldTarget;

	public RenderTarget2D glowTarget => _glowTarget;

	public RenderTarget2D glowTarget2 => _glowTarget2;

	public RenderTarget2D glowTarget3 => _glowTarget3;

	public BasicEffect flatEffect => _flatEffect;

	public Viewport viewport
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return _graphics.GraphicsDevice.Viewport;
		}
	}

	public bool debug
	{
		get
		{
			return _debug;
		}
		set
		{
			_debug = value;
		}
	}

	public Level level => _level;

	public Vector3 cursorPos
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			Viewport val = _graphics.GraphicsDevice.Viewport;
			Vector3 val2 = ((Viewport)(ref val)).Project(playerPos + Vector3.Transform(50f * cursorDir, MapObjectToSystem(Vector3.Zero, playerDir, playerUp)), _projection, _view, Matrix.Identity);
			return new Vector3(val2.X, val2.Y, 0f);
		}
	}

	public Vector3 cursorDir
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _cursorDir;
		}
	}

	public Vector3 cursorUp
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _cursorUp;
		}
	}

	public Vector3 cameraLoc
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _cameraLoc;
		}
	}

	public Vector3 cameraDir
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _cameraDir;
		}
	}

	public Vector3 cameraUp
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _cameraUp;
		}
	}

	public Vector3 cameraTarget
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _cameraTarget;
		}
	}

	public Vector3 playerPos
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _playerPos;
		}
	}

	public Vector3 playerDir
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _playerDir;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_playerDir = value;
		}
	}

	public Vector3 playerUp
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _playerUp;
		}
	}

	public List<Vector2> window
	{
		get
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			_window[0] = new Vector2(cursorPos.X - (float)C_WIDTH, cursorPos.Y - (float)C_WIDTH);
			_window[1] = new Vector2(cursorPos.X + (float)C_WIDTH, cursorPos.Y + (float)C_WIDTH);
			return _window;
		}
	}

	public List<TargetEffect> fx => _fx;

	public int score
	{
		get
		{
			return _score;
		}
		set
		{
			_score = value;
		}
	}

	public int[] maxPower => _maxPower;

	public List<Enemy> enems => _enems;

	public Player player => _player;

	public List<FallingObject> fallFX => _fallFX;

	public string textFlow => _textFlow;

	public ScoreGroupCol scoreFlow => _scoreFlow;

	public EnemyQueue eQueue => _level.ActiveZone.eq;

	public double totalTime => _totalTime;

	public float weaponMode
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			if ((int)fillMode != 2)
			{
				return 1f - channels[30];
			}
			return channels[30];
		}
	}

	public GraphicsStack matStack => _matStack;

	public GraphicsStack flatStack => _flatStack;

	public bool firstPass
	{
		get
		{
			if (FREEZE_ON)
			{
				return _freezeFirstPass;
			}
			return _firstPass;
		}
		set
		{
			if (FREEZE_ON)
			{
				_freezeFirstPass = value;
			}
			else
			{
				_firstPass = value;
			}
		}
	}

	public float paneDepth => _paneDepth;

	public bool firstBeat
	{
		get
		{
			if (FREEZE_ON)
			{
				return _freezeFirstBeat;
			}
			return _firstBeat;
		}
		set
		{
			if (FREEZE_ON)
			{
				_freezeFirstBeat = value;
			}
			else
			{
				_firstBeat = value;
			}
		}
	}

	public HUD hud => _hud;

	public Texture2D backTex => _backTex;

	public InputState input => _input;

	public bool EasyMode
	{
		get
		{
			return _easyMode;
		}
		set
		{
			_easyMode = value;
		}
	}

	public Random r => _r;

	public Vector3 ActualCameraPos()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		return _playerPos + Vector3.Transform(_cameraLoc + _cameraTarget, MapObjectToSystem2(Vector3.Zero, _cameraDir, _cameraUp) * MapObjectToSystem(Vector3.Zero, _playerDir, _playerUp));
	}

	public Vector3 ActualCameraPosNoPlayerPos()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Transform(_cameraLoc + _cameraTarget, MapObjectToSystem2(Vector3.Zero, _cameraDir, _cameraUp) * MapObjectToSystem(Vector3.Zero, _playerDir, _playerUp));
	}

	public void LineUpCamera()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ActualCameraPos();
		_world = Matrix.Identity;
		matStack.Clear();
		_view = Matrix.CreateLookAt(val, _playerPos + Vector3.Transform(_cameraTarget, MapObjectToSystem2(Vector3.Zero, _cameraDir, _cameraUp) * MapObjectToSystem(Vector3.Zero, _playerDir, _playerUp)), Vector3.Transform(_cameraUp, MapObjectToSystem(Vector3.Zero, _playerDir, _playerUp)));
		_fogEffect.Parameters["xView"].SetValue(_view);
		_fogEffect.Parameters["EyePosition"].SetValue(new Vector4(val, 1f));
		matStack.PushMatrix();
		matStack.ApplyMatrix(_world);
	}

	public Matrix LineUpCameraNoPlayerPos()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ActualCameraPosNoPlayerPos();
		_world = Matrix.Identity;
		return Matrix.CreateLookAt(val, Vector3.Transform(_cameraTarget, MapObjectToSystem2(Vector3.Zero, _cameraDir, _cameraUp) * MapObjectToSystem(Vector3.Zero, _playerDir, _playerUp)), Vector3.Transform(_cameraUp, MapObjectToSystem(Vector3.Zero, _playerDir, _playerUp)));
	}

	public static BaseGame Get()
	{
		if (obj == null)
		{
			obj = new BaseGame();
		}
		return obj;
	}

	public static void Trash()
	{
		obj = null;
	}

	private BaseGame()
	{
	}

	public void MakeObj_Major(Game _game)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected O, but got Unknown
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Expected O, but got Unknown
		coreGame = _game;
		if (quickload)
		{
			_easyMode = true;
		}
		if (_graphics == null)
		{
			_graphics = new GraphicsDeviceManager(_game);
			_graphics.PreparingDeviceSettings += graphics_PreparingDeviceSettings;
		}
		if (engineOpen == null)
		{
			engineOpen = new AudioEngine("Content\\Opening.xgs");
			wavebankOpen = new WaveBank(engineOpen, "Content\\Open Wave Bank.xwb");
			sBOpen = new SoundBank(engineOpen, "Content\\Open Sound Bank.xsb");
		}
		if (engine == null)
		{
			engine = new AudioEngine("Content\\Olu.xgs");
			wavebank = new WaveBank(engine, "Content\\Wave Bank.xwb");
			sB = new SoundBank(engine, "Content\\Sound Bank.xsb");
		}
		_content = new ContentManager((IServiceProvider)_game.Services);
		_input = new InputState();
		fillMode = (FillMode)3;
	}

	private void BuildFullscreenBuffer()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Expected O, but got Unknown
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Expected O, but got Unknown
		List<VertexPositionTexture> list = new List<VertexPositionTexture>();
		list.Add(new VertexPositionTexture(new Vector3(-1f, -1f, 0f), new Vector2(0f, 1f)));
		list.Add(new VertexPositionTexture(new Vector3(-1f, 1f, 0f), new Vector2(0f, 0f)));
		list.Add(new VertexPositionTexture(new Vector3(1f, 1f, 0f), new Vector2(1f, 0f)));
		list.Add(new VertexPositionTexture(new Vector3(1f, -1f, 0f), new Vector2(1f, 1f)));
		fullScreenBuff = new VertexBuffer(_graphics.GraphicsDevice, list.Count * VertexPositionTexture.SizeInBytes, (BufferUsage)8);
		fullScreenBuff.SetData<VertexPositionTexture>(list.ToArray());
		fullScreenDec = new VertexDeclaration(_graphics.GraphicsDevice, VertexPositionTexture.VertexElements);
	}

	public void MakeObj_Minor()
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		Thread.CurrentThread.SetProcessorAffinity(new int[1] { 4 });
		FXLoaded = false;
		_models = new ModelManager(_content);
		ps = new ParticleSystem();
		_r = new Random();
		_player = new Player();
		emptytime = new GameTime(new TimeSpan(0, 0, 0, 0, 0), new TimeSpan(0, 0, 0, 0, 0), new TimeSpan(0, 0, 0, 0, 0), new TimeSpan(0, 0, 0, 0, 0));
		invert = true;
		ReloadLevelObj();
		_hud = new HUD();
		SignedInGamer.SignedIn += SignedInGamer_SignedIn;
		SignedInGamer.SignedOut += SignedInGamer_SignedOut;
		loadThread = null;
	}

	public void BeginLoadHS()
	{
		HSLoad = false;
		if (Get().continueWithoutSaving)
		{
			HSLoad = true;
		}
		else if (globStorageDevice == null || !globStorageDevice.IsConnected)
		{
			RetryLoadHS("Please reattach high score storage device");
		}
		else
		{
			LoadHighScores();
		}
	}

	public void RetryLoadHS(string strError)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		HSLoad = false;
		foreach (GameComponent item in (Collection<IGameComponent>)(object)CoreGame.Components)
		{
			GameComponent val = item;
			if (!(val is GamerServicesComponent))
			{
				val.Enabled = false;
			}
		}
		((Collection<IGameComponent>)(object)CoreGame.Components).Add((IGameComponent)(object)new VerifySaveLocationComponent(CoreGame, strError, IOModes.LoadHS));
	}

	public void LoadHighScores()
	{
		if (globStorageContainer != null)
		{
			globStorageContainer.Dispose();
		}
		FileStream fileStream;
		try
		{
			globStorageContainer = globStorageDevice.OpenContainer("Olu");
			fileStream = File.Open(Path.Combine(globStorageContainer.Path, "title.sav"), FileMode.OpenOrCreate, FileAccess.Read);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(TitleSaveData));
			if (fileStream.Length > 0)
			{
				hiScores = (TitleSaveData)xmlSerializer.Deserialize(fileStream);
			}
			if (fileStream.Length <= 0 || hiScores.topNames == null)
			{
				hiScores = new TitleSaveData(1);
				for (int i = 0; i < 4; i++)
				{
					for (int j = 0; j < 10; j++)
					{
						hiScores.topNames[i][j] = "Ferret";
						hiScores.topScores[i][j] = (10 - j) * ((i == 3) ? 5000 : 10000);
					}
				}
			}
			if (hiScores.topNames.Length == 3)
			{
				string[][] array = new string[4][];
				for (int k = 0; k < 4; k++)
				{
					array[k] = new string[10];
				}
				for (int l = 0; l < 3; l++)
				{
					for (int m = 0; m < 10; m++)
					{
						array[l][m] = hiScores.topNames[l][m];
					}
				}
				hiScores.topNames = array;
			}
			if (hiScores.topScores.Length == 3)
			{
				int[][] array2 = new int[4][];
				for (int n = 0; n < 4; n++)
				{
					array2[n] = new int[10];
				}
				for (int num = 0; num < 3; num++)
				{
					for (int num2 = 0; num2 < 10; num2++)
					{
						array2[num][num2] = hiScores.topScores[num][num2];
					}
				}
				hiScores.topScores = array2;
			}
			fileStream.Close();
			globStorageContainer.Dispose();
			HSLoad = true;
		}
		catch (Exception)
		{
			if (globStorageDevice == null || !globStorageDevice.IsConnected)
			{
				RetryLoadHS("Please reattach high score storage device");
			}
			else
			{
				hiScores = new TitleSaveData(1);
				for (int num3 = 0; num3 < 4; num3++)
				{
					for (int num4 = 0; num4 < 10; num4++)
					{
						hiScores.topNames[num3][num4] = "Ferret";
						hiScores.topScores[num3][num4] = (10 - num4) * ((num3 == 3) ? 5000 : 10000);
					}
				}
				HSLoad = true;
			}
			if (globStorageContainer != null)
			{
				globStorageContainer.Dispose();
			}
		}
		fileStream = null;
		globStorageContainer = null;
	}

	public void BeginSaveHS()
	{
		HSSaved = false;
		if (Get().continueWithoutSaving)
		{
			HSSaved = true;
		}
		else if (globStorageDevice == null || !globStorageDevice.IsConnected)
		{
			RetrySaveHS("Please reattach high score storage device");
		}
		else
		{
			SaveHighScores();
		}
	}

	public void RetrySaveHS(string strError)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		HSSaved = false;
		foreach (GameComponent item in (Collection<IGameComponent>)(object)CoreGame.Components)
		{
			GameComponent val = item;
			if (!(val is GamerServicesComponent))
			{
				val.Enabled = false;
			}
		}
		((Collection<IGameComponent>)(object)CoreGame.Components).Add((IGameComponent)(object)new VerifySaveLocationComponent(CoreGame, strError, IOModes.SaveHS));
	}

	public void SaveHighScores()
	{
		if (globStorageContainer != null)
		{
			globStorageContainer.Dispose();
		}
		FileStream fileStream;
		try
		{
			globStorageContainer = globStorageDevice.OpenContainer("Olu");
			fileStream = File.Open(Path.Combine(globStorageContainer.Path, "title.sav"), FileMode.Create);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(TitleSaveData));
			xmlSerializer.Serialize(fileStream, hiScores);
			fileStream.Close();
			globStorageContainer.Dispose();
			HSSaved = true;
		}
		catch (Exception)
		{
			RetrySaveHS("Please reattach high score storage device");
			if (globStorageContainer != null)
			{
				globStorageContainer.Dispose();
			}
		}
		fileStream = null;
		globStorageContainer = null;
	}

	public void BeginLoadPlayer()
	{
		PlayerLoad = false;
		if (Get().continueWithoutSaving)
		{
			PlayerLoad = true;
		}
		else if (storageDevice == null || !storageDevice.IsConnected)
		{
			RetryLoadPlayer("Please reattach player storage device");
		}
		else
		{
			LoadPlayerData();
		}
	}

	public void RetryLoadPlayer(string strError)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		PlayerLoad = false;
		foreach (GameComponent item in (Collection<IGameComponent>)(object)CoreGame.Components)
		{
			GameComponent val = item;
			if (!(val is GamerServicesComponent))
			{
				val.Enabled = false;
			}
		}
		((Collection<IGameComponent>)(object)CoreGame.Components).Add((IGameComponent)(object)new VerifySaveLocationComponent(CoreGame, strError, IOModes.LoadPlayer));
	}

	public void LoadPlayerData()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (storageContainer != null)
		{
			storageContainer.Dispose();
		}
		FileStream fileStream;
		try
		{
			storageContainer = storageDevice.OpenContainer("Olu-" + ((Gamer)Gamer.SignedInGamers[input.ActivePlayerIndex]).Gamertag);
			fileStream = File.Open(Path.Combine(storageContainer.Path, "player.sav"), FileMode.OpenOrCreate);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserSaveData));
			if (fileStream.Length > 0)
			{
				curUserData = (UserSaveData)xmlSerializer.Deserialize(fileStream);
			}
			else
			{
				curUserData = default(UserSaveData);
				curUserData.rumble = true;
			}
			invert = curUserData.invert;
			rumble = curUserData.rumble;
			PlayerLoad = true;
			fileStream.Close();
			storageContainer.Dispose();
		}
		catch (Exception)
		{
			if (storageDevice == null || !storageDevice.IsConnected)
			{
				RetryLoadPlayer("Please reattach player storage device");
			}
			else
			{
				curUserData = default(UserSaveData);
				curUserData.rumble = true;
				invert = curUserData.invert;
				rumble = curUserData.rumble;
				PlayerLoad = true;
			}
			if (storageContainer != null)
			{
				storageContainer.Dispose();
			}
		}
		fileStream = null;
		storageContainer = null;
		TrialModeSettings(Guide.IsTrialMode);
	}

	public void BeginSavePlayer()
	{
		PlayerSaved = false;
		if (Get().continueWithoutSaving)
		{
			PlayerSaved = true;
		}
		else if (storageDevice == null || !storageDevice.IsConnected)
		{
			RetrySavePlayer("Please reattach player storage device");
		}
		else
		{
			SavePlayerData();
		}
	}

	public void RetrySavePlayer(string strError)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		PlayerSaved = false;
		foreach (GameComponent item in (Collection<IGameComponent>)(object)CoreGame.Components)
		{
			GameComponent val = item;
			if (!(val is GamerServicesComponent))
			{
				val.Enabled = false;
			}
		}
		((Collection<IGameComponent>)(object)CoreGame.Components).Add((IGameComponent)(object)new VerifySaveLocationComponent(CoreGame, strError, IOModes.SavePlayer));
	}

	public void SavePlayerData()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (storageContainer != null)
		{
			storageContainer.Dispose();
		}
		FileStream fileStream;
		try
		{
			storageContainer = storageDevice.OpenContainer("Olu-" + ((Gamer)Gamer.SignedInGamers[input.ActivePlayerIndex]).Gamertag);
			fileStream = File.Open(Path.Combine(storageContainer.Path, "player.sav"), FileMode.Create);
			curUserData.invert = invert;
			curUserData.rumble = rumble;
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserSaveData));
			xmlSerializer.Serialize(fileStream, curUserData);
			PlayerSaved = true;
			fileStream.Close();
			storageContainer.Dispose();
			if (storageContainer != null)
			{
				storageContainer.Dispose();
			}
		}
		catch (Exception)
		{
			RetrySavePlayer("Please reattach player storage device");
		}
		fileStream = null;
		storageContainer = null;
	}

	public int EndStageSavePartOne(int stageNumber)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		int num = -1;
		if (stageNumber > curUserData.levelsCleared)
		{
			curUserData.levelsCleared = stageNumber;
		}
		if (!Get().EasyMode && !Get().continueWithoutSaving)
		{
			string text = ((Gamer)Gamer.SignedInGamers[Get().input.ActivePlayerIndex]).Gamertag;
			int num2 = score;
			stageNumber--;
			if (stageNumber >= 0)
			{
				for (int i = 0; i < 10; i++)
				{
					if (hiScores.topScores[stageNumber][i] < num2)
					{
						if (num < 0)
						{
							num = i;
						}
						int num3 = hiScores.topScores[stageNumber][i];
						string text2 = hiScores.topNames[stageNumber][i];
						hiScores.topScores[stageNumber][i] = num2;
						hiScores.topNames[stageNumber][i] = text;
						num2 = num3;
						text = text2;
					}
				}
			}
		}
		return num;
	}

	public void EndStageSavePartTwo()
	{
		if (!Get().EasyMode)
		{
			BeginSaveHS();
		}
	}

	public void EndStageSavePartThree()
	{
		BeginSavePlayer();
	}

	private void SignedInGamer_SignedOut(object sender, SignedOutEventArgs e)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		if (continueWithoutSaving)
		{
			return;
		}
		_ = Get().input.playerIndex;
		if (e.Gamer.PlayerIndex != Get().input.playerIndex)
		{
			return;
		}
		foreach (GameComponent item in (Collection<IGameComponent>)(object)CoreGame.Components)
		{
			GameComponent val = item;
			if (val is SignBackInComponent)
			{
				return;
			}
		}
		foreach (GameComponent item2 in (Collection<IGameComponent>)(object)CoreGame.Components)
		{
			GameComponent val2 = item2;
			if (!(val2 is GamerServicesComponent))
			{
				val2.Enabled = false;
			}
		}
		((Collection<IGameComponent>)(object)CoreGame.Components).Add((IGameComponent)(object)new SignBackInComponent(CoreGame, ((Gamer)e.Gamer).Gamertag));
	}

	public void SignOut()
	{
		if (continueWithoutSaving)
		{
			return;
		}
		for (int num = ((Collection<IGameComponent>)(object)CoreGame.Components).Count - 1; num >= 0; num--)
		{
			if (((Collection<IGameComponent>)(object)CoreGame.Components)[num] is BaseComponent || ((Collection<IGameComponent>)(object)CoreGame.Components)[num] is MainMenuComponent || ((Collection<IGameComponent>)(object)CoreGame.Components)[num] is PauseComponent || ((Collection<IGameComponent>)(object)coreGame.Components)[num] is SignBackInComponent)
			{
				((Collection<IGameComponent>)(object)CoreGame.Components).RemoveAt(num);
			}
		}
		curUserData = default(UserSaveData);
		curUserData.rumble = true;
		((Collection<IGameComponent>)(object)CoreGame.Components).Add((IGameComponent)(object)new MainMenuComponent(coreGame));
	}

	private void SignedInGamer_SignedIn(object sender, SignedInEventArgs e)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Expected O, but got Unknown
		bool flag = false;
		if (continueWithoutSaving)
		{
			return;
		}
		_ = Get().input.playerIndex;
		if (e.Gamer.PlayerIndex != Get().input.playerIndex)
		{
			return;
		}
		foreach (GameComponent item in (Collection<IGameComponent>)(object)CoreGame.Components)
		{
			GameComponent val = item;
			if (val is VerifySaveLocationComponent)
			{
				flag = true;
				val.Enabled = true;
			}
		}
		if (!flag)
		{
			foreach (GameComponent item2 in (Collection<IGameComponent>)(object)CoreGame.Components)
			{
				GameComponent val2 = item2;
				if (!(val2 is GamerServicesComponent))
				{
					val2.Enabled = true;
				}
			}
		}
		for (int num = ((Collection<IGameComponent>)(object)CoreGame.Components).Count - 1; num >= 0; num--)
		{
			if (((Collection<IGameComponent>)(object)CoreGame.Components)[num] is SignBackInComponent)
			{
				if (SignBackInComponent.origGamerTag == ((Gamer)e.Gamer).Gamertag)
				{
					((Collection<IGameComponent>)(object)CoreGame.Components).RemoveAt(num);
				}
				else
				{
					SignOut();
				}
			}
		}
	}

	private void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Invalid comparison between Unknown and I4
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		DisplayModeCollection supportedDisplayModes = GraphicsAdapter.DefaultAdapter.SupportedDisplayModes;
		foreach (DisplayMode item in (DisplayModeCollection)(ref supportedDisplayModes))
		{
			DisplayMode current = item;
			if (((DisplayMode)(ref current)).Width == WIDTH && ((DisplayMode)(ref current)).Height == HEIGHT && (int)((DisplayMode)(ref current)).Format == 2)
			{
				e.GraphicsDeviceInformation.PresentationParameters.BackBufferFormat = ((DisplayMode)(ref current)).Format;
				e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight = ((DisplayMode)(ref current)).Height;
				e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth = ((DisplayMode)(ref current)).Width;
				e.GraphicsDeviceInformation.PresentationParameters.IsFullScreen = true;
				e.GraphicsDeviceInformation.PresentationParameters.FullScreenRefreshRateInHz = ((DisplayMode)(ref current)).RefreshRate;
			}
		}
	}

	public void PrepareGraphicsObj_Major()
	{
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Expected O, but got Unknown
		_graphics.GraphicsDevice.RenderState.AlphaBlendEnable = true;
		_graphics.GraphicsDevice.RenderState.SourceBlend = (Blend)5;
		_graphics.GraphicsDevice.RenderState.AlphaSourceBlend = (Blend)5;
		_graphics.GraphicsDevice.RenderState.DestinationBlend = (Blend)6;
		_graphics.GraphicsDevice.RenderState.AlphaDestinationBlend = (Blend)6;
		_graphics.GraphicsDevice.RenderState.AlphaTestEnable = true;
		_graphics.GraphicsDevice.PresentationParameters.BackBufferFormat = (SurfaceFormat)1;
		_graphics.PreferredDepthStencilFormat = (DepthFormat)48;
		GraphicsSettings();
		_graphics.ApplyChanges();
		_spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);
		SignedInGamer.SignedOut += SignedInGamer_SignedOut;
	}

	public void PrepareGraphicsObj_Finish()
	{
		Thread.CurrentThread.SetProcessorAffinity(new int[1] { 4 });
		PrepareGraphicsObj_Menu();
		PrepareGraphicsObj_Minor();
		loadThread = null;
		contentLoad = true;
	}

	public void PrepareGraphicsObj_Menu()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Expected O, but got Unknown
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Expected O, but got Unknown
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Expected O, but got Unknown
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Expected O, but got Unknown
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Expected O, but got Unknown
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Expected O, but got Unknown
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Expected O, but got Unknown
		BuildFullscreenBuffer();
		float num = MathHelper.ToRadians(70f);
		Viewport val = _graphics.GraphicsDevice.Viewport;
		float num2 = ((Viewport)(ref val)).Width;
		Viewport val2 = _graphics.GraphicsDevice.Viewport;
		_projection = Matrix.CreatePerspectiveFieldOfView(num, num2 / (float)((Viewport)(ref val2)).Height, 1f, 400000f);
		_paneDepth = (float)((double)((float)S_HEIGHT / 2f) / Math.Tan(Math.PI / 4.0));
		_fogEffect = GetFogEffect();
		_alphaEffect = _content.Load<Effect>("Content/AlphaEffect");
		_blurEffect = _content.Load<Effect>("Content/BlurEffect");
		_combineEffect = _content.Load<Effect>("Content/Combine");
		_combineEffect.Parameters["BloomIntensity"].SetValue(5.5f);
		_combineEffect.Parameters["BaseIntensity"].SetValue(1);
		_combineEffect.Parameters["BloomSaturation"].SetValue(2);
		_combineEffect.Parameters["BaseSaturation"].SetValue(1);
		_combineEffect.Parameters["HalfTexelSize"].SetValue(new Vector2(1f / (2f * (float)WIDTH), 1f / (2f * (float)HEIGHT)));
		_flattenEffect = _content.Load<Effect>("Content/Flatten");
		PresentationParameters presentationParameters = _graphics.GraphicsDevice.PresentationParameters;
		presentationParameters.EnableAutoDepthStencil = false;
		if (_graphics.GraphicsDevice.GraphicsDeviceCapabilities.MaxSimultaneousRenderTargets < 2)
		{
			bloom_on = false;
		}
		int backBufferWidth = presentationParameters.BackBufferWidth;
		int backBufferHeight = presentationParameters.BackBufferHeight;
		SurfaceFormat backBufferFormat = presentationParameters.BackBufferFormat;
		_worldTarget = new RenderTarget2D(_graphics.GraphicsDevice, backBufferWidth, backBufferHeight, 1, backBufferFormat);
		if (bloom_on)
		{
			_glowTarget = new RenderTarget2D(_graphics.GraphicsDevice, backBufferWidth, backBufferHeight, 1, backBufferFormat);
			backBufferWidth /= 2;
			backBufferHeight /= 2;
			_glowTarget3 = new RenderTarget2D(_graphics.GraphicsDevice, backBufferWidth, backBufferHeight, 1, backBufferFormat);
			_glowTarget2 = new RenderTarget2D(_graphics.GraphicsDevice, backBufferWidth, backBufferHeight, 1, backBufferFormat);
			_fogEffect.Parameters["BloomDisable"].SetValue(false);
		}
		else
		{
			_fogEffect.Parameters["BloomDisable"].SetValue(true);
		}
		_flatEffect = new BasicEffect(_graphics.GraphicsDevice, (EffectPool)null);
		_flatEffect.VertexColorEnabled = true;
		_flatEffect.View = Matrix.Identity;
		_flatEffect.Projection = Matrix.CreateOrthographicOffCenter(0f, (float)WIDTH, (float)HEIGHT, 0f, 0f, 1f);
		_matStack = new GraphicsStack(_fogEffect);
		_matStack.PushMatrix();
		_matStack.ApplyMatrix(Matrix.Identity);
		CalculateBlur(1f / (float)WIDTH, 0f, out horBlurSampleWeights, out horBlurSampleOffsets);
		CalculateBlur(0f, 1f / (float)HEIGHT, out verBlurSampleWeights, out verBlurSampleOffsets);
		SetBlurEffectParameters(ref horBlurSampleWeights, ref horBlurSampleOffsets, ref verBlurSampleWeights, ref verBlurSampleOffsets);
		_flatStack = new GraphicsStack((Effect)(object)_flatEffect);
		_vertDec = new VertexDeclaration(_graphics.GraphicsDevice, VertexPositionColor.VertexElements);
		VPNTDec = new VertexDeclaration(_graphics.GraphicsDevice, VertexPositionNormalTexture.VertexElements);
		hud.LoadGraphics();
		_backTex = content.Load<Texture2D>("Content/backTex");
		bitTarget = _graphics.GraphicsDevice.DepthStencilBuffer;
	}

	public void PrepareGraphicsObj_Minor()
	{
		if (!FXLoaded)
		{
			player.mGrid = models.GetModel("Content/Player/ChangeGrid");
			SetAllEPCs(player.mGrid.epc, "xEnableLighting", false);
			TargetEffect.CreateFX();
			TargetEffectBase.CreateShockFX();
			Bird01.LoadModel();
			Gift.LoadModel();
			Hypatia.LoadModel();
			Olu.LoadOluModel();
			SharkTail.LoadModel();
			EnemyCube.GenerateFX();
			Fish01.LoadModel();
			Digit.LoadModel();
			Serpent.LoadModel();
			SerpentTail.LoadModel();
		}
		if (!FXLoaded)
		{
			TextDisplay.LoadGraphics();
			tdColl = new TextDisplayCol();
			tdColl2 = new TextDisplay2Col();
			Sled01.LoadModel();
			pythdigit01 = new PythDigitCollection();
			fish01 = new Fish01Collection();
			sTail01 = new SerpentTailCollection();
			Fish02.LoadModel();
			Chand.LoadModel();
			Whale01.LoadModel();
			Tower.LoadModel();
			BulletB.LoadModel();
			Surfer.LoadModel();
			Shark.LoadModel();
			Pine.LoadModel();
			BulletGlow.LoadModel();
			BulletC.LoadModel();
			Pythagoras.LoadModel();
			PythDigit.LoadModel();
			Euler.LoadModel();
			Note.LoadModel();
		}
		FXLoaded = true;
	}

	public void ReloadLevelObj()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		skyCooldown = (skyMaxCooldown = 1f);
		skyFlow = new Vector3(0f, 0f, -100f);
		skyFlowToggle = true;
		_totalTime = 0.0;
		_fx = new List<TargetEffect>();
		if (_enems != null && _enems.Count > 0)
		{
			for (int num = _enems.Count - 1; num >= 0; num--)
			{
				_enems[num].leave();
			}
		}
		_enems = new List<Enemy>();
		targetFX = new List<TargetEffect>();
		_fallFX = new List<FallingObject>();
		_textFlow = "";
		_scoreFlow = new ScoreGroupCol();
		fog = new List<VertexPositionColor[]>();
		activeCues = new List<Cue>();
		bgCues = new List<Cue>();
		curTime = BEAT;
		curBeat = 0;
		firstBeat = true;
		firstPass = true;
		_zoneBeat = true;
		playBGMusic = true;
		_cameraLoc = new Vector3(0f, 0f, -5f);
		_cameraDir = new Vector3(0f, 0f, 1f);
		_cameraUp = new Vector3(0f, 1f, 0f);
		_cameraTarget = new Vector3(0f, 2f, 0.5f);
		_cursorDir = new Vector3(0f, 0f, 1f);
		_cursorUp = new Vector3(0f, 1f, 0f);
		_playerPos = new Vector3(0f, 0f, 0f);
		_playerDir = new Vector3(0f, 0f, 1f);
		_playerUp = new Vector3(0f, 1f, 0f);
		_window = new List<Vector2>();
		_window.Add(Vector2.Zero);
		_window.Add(Vector2.Zero);
		actualEnem = 0;
		paused = false;
		movingToNextZone = false;
		elaspedEndTime = 0;
		powerAmounts = new int[2];
		powerScore = new int[2];
		_maxPower = new int[2];
		score = 0;
		powerScore[0] = 0;
		powerScore[1] = 0;
		_maxPower[0] = 5000;
		_maxPower[1] = 5000;
		powerAmounts[0] = 0;
		powerAmounts[1] = 0;
		maxFX = new TargetEffectCol();
		channels[8] = 1f;
	}

	public void ReloadLevelGraphics(string levelName)
	{
		ReloadLevelGraphics(levelName, 0);
	}

	public void ReloadLevelGraphics(string levelName, int zoneStart)
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		levelLoaded = false;
		Thread.CurrentThread.SetProcessorAffinity(new int[1] { 4 });
		LevelLoader.LoadLevel(levelName, out _level);
		player.mPlayer = models.GetModel(contentRoot + level.playerModelPath, copyData: true, copyEPC: false);
		SetAllEPCs(player.mPlayer.epc, "xEnableLighting", true);
		SetAllEPCs(player.mPlayer.epc, "DirLight0Direction", (object)new Vector3(-0.5f, -0.5f, -0.5f));
		SetAllEPCs(player.mPlayer.epc, "TexMode", T_MUL);
		player.pdColl = new PlaneDetachColl(ref player.mPlayer);
		hitCountdown = 0f;
		player.playerBones = new Dictionary<ModelBone, int>();
		player.playerAnim = new ModelOluAnimator(coreGame, player.mPlayer, fogEffect);
		player.idle = new AnimationController(coreGame, player.playerAnim.Animations["idle"]);
		player.spin = new AnimationController(coreGame, player.playerAnim.Animations["spin"]);
		RunController(player.playerAnim, player.spin);
		for (int i = 0; i < ((ReadOnlyCollection<ModelBone>)(object)player.mPlayer.model.Bones).Count; i++)
		{
			if (!player.playerBones.ContainsKey(((ReadOnlyCollection<ModelBone>)(object)player.mPlayer.model.Bones)[i]))
			{
				player.playerBones.Add(((ReadOnlyCollection<ModelBone>)(object)player.mPlayer.model.Bones)[i], i);
				if (i > 2)
				{
					player.playerAnim.bonePoses[i].enabled = false;
				}
			}
		}
		player.playerAnim.bonePoses[player.playerBones[player.mPlayer.model.Bones["Armature__1"]]].enabled = true;
		player.level = 1;
		((GameComponent)player.playerAnim).Update(emptytime);
		((GameComponent)player.idle).Update(emptytime);
		((GameComponent)player.spin).Update(emptytime);
		if (zoneStart > 0)
		{
			zoneToLoad = zoneStart;
		}
		ps.LoadGraphics();
		if (tdColl != null)
		{
			tdColl.tDisplay.Clear();
		}
		if (tdColl2 != null)
		{
			tdColl2.tDisplay.Clear();
		}
		skyPS = new SkyParticleSystem();
		skyFlow = new Vector3(0f, 0f, -100f);
		skyPS.LoadGraphics();
		for (int num = 3; num >= 0; num--)
		{
			skyPS.AddPlaneParticles(new Vector3(-100f, -100f, 0f) - 3f * skyFlow, new Vector3(100f, 100f, 0f) - 3f * skyFlow, new Vector3(0f, 0f, -100f), 0.2f, 0f, 4f, 0f, new Vector4(1f, 1f, 1f, 1f), 50, 0.01f, 0.1f, num, skyFlow);
		}
		zoneEndTime = level.ActiveZone.zoneEndTime;
		hud.LoadLevelColor(level.flashColor + level.baseColor);
		level.LoadGraphics();
		levelLoaded = true;
	}

	public void GraphicsSettings()
	{
		_graphics.GraphicsDevice.RenderState.SeparateAlphaBlendEnabled = true;
		_graphics.GraphicsDevice.RenderState.AlphaFunction = (CompareFunction)7;
		_graphics.GraphicsDevice.RenderState.ReferenceAlpha = 32;
		_graphics.GraphicsDevice.RenderState.CullMode = (CullMode)1;
		_graphics.GraphicsDevice.RenderState.DepthBufferEnable = true;
	}

	public void Update(GameTime gametime)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Invalid comparison between Unknown and I4
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Invalid comparison between Unknown and I4
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0709: Unknown result type (might be due to invalid IL or missing references)
		//IL_070e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0713: Unknown result type (might be due to invalid IL or missing references)
		//IL_0727: Unknown result type (might be due to invalid IL or missing references)
		//IL_0754: Unknown result type (might be due to invalid IL or missing references)
		//IL_076b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0943: Unknown result type (might be due to invalid IL or missing references)
		//IL_0949: Unknown result type (might be due to invalid IL or missing references)
		//IL_094f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0954: Unknown result type (might be due to invalid IL or missing references)
		//IL_0959: Unknown result type (might be due to invalid IL or missing references)
		//IL_095f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0964: Unknown result type (might be due to invalid IL or missing references)
		//IL_0969: Unknown result type (might be due to invalid IL or missing references)
		//IL_096e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0973: Unknown result type (might be due to invalid IL or missing references)
		//IL_097a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0980: Unknown result type (might be due to invalid IL or missing references)
		//IL_0985: Unknown result type (might be due to invalid IL or missing references)
		//IL_098b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0990: Unknown result type (might be due to invalid IL or missing references)
		//IL_0995: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a17: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a22: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a27: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a46: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a51: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a56: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d78: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fdb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ff1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ff6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f72: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f88: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f8d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dca: Unknown result type (might be due to invalid IL or missing references)
		input.Update();
		if (!paused)
		{
			GamePadState curPad = input.curPad;
			if (!((GamePadState)(ref curPad)).IsConnected)
			{
				PauseGame(controllerDisconnect: true);
			}
		}
		if (!coreGame.IsActive)
		{
			bool flag = true;
			foreach (Enemy enem in enems)
			{
				if (enem is GameplayChange && (((GameplayChange)enem).command == "quit" || ((GameplayChange)enem).command == "credits"))
				{
					flag = false;
				}
			}
			if (flag)
			{
				PauseGame(controllerDisconnect: false);
			}
		}
		if (paused)
		{
			return;
		}
		int index = 0;
		if (hitCountdown > 0f)
		{
			hitCountdown -= (float)gametime.ElapsedGameTime.TotalSeconds;
			if (hitCountdown <= 0f)
			{
				hitCountdown = 0f;
			}
		}
		curTime -= (float)gametime.ElapsedGameTime.TotalSeconds;
		if (zoneToLoad >= 0)
		{
			curTime = BEAT;
			curBeat = 0;
			firstBeat = true;
			firstPass = true;
			_zoneBeat = true;
			movingToNextZone = false;
			elaspedEndTime = 0;
			playBGMusic = true;
			level.LoadZone(zoneToLoad);
			_playerPos = Vector3.Zero;
			_playerDir = new Vector3(0f, 0f, 1f);
			_playerUp = new Vector3(0f, 1f, 0f);
			zoneToLoad = -1;
		}
		if (curTime < 0.001f)
		{
			firstPass = true;
			firstBeat = true;
			curTime += BEAT;
			curBeat = (curBeat + 1) % maxBeat;
			if (curBeat == 0)
			{
				CleanupCues();
			}
			if (movingToNextZone && !FREEZE_ON)
			{
				elaspedEndTime++;
				if (elaspedEndTime >= level.ActiveZone.zoneEndTime)
				{
					curBeat = 0;
					_zoneBeat = true;
					movingToNextZone = false;
					elaspedEndTime = 0;
					playBGMusic = true;
					numTargeted = 0;
					targetFX.Clear();
					level.LoadZone(level.activeZone + 1);
					player.level++;
					UpdatePlayerMesh(player.level, show: true);
					_playerPos = Vector3.Zero;
					_playerDir = new Vector3(0f, 0f, 1f);
					_playerUp = new Vector3(0f, 1f, 0f);
				}
			}
		}
		else
		{
			firstBeat = false;
		}
		if (_zoneBeat)
		{
			firstBeat = true;
			_zoneBeat = false;
		}
		if (!FREEZE_ON)
		{
			UpdateTotalTime(gametime);
		}
		textFlash += flashInc * 4f * (float)gametime.ElapsedGameTime.TotalSeconds;
		if (textFlash >= 16f * BEAT)
		{
			flashInc = -1f;
		}
		else if (textFlash <= 0f)
		{
			flashInc = 1f;
		}
		if (input.KeyPressed((Keys)68))
		{
			debug = !debug;
		}
		if (input.RightTriggerDown() || input.LeftTriggerDown())
		{
			if (input.triggerOld)
			{
				ShootTargets();
			}
			if (input.leftHeld)
			{
				if ((int)fillMode != 2)
				{
					channels[30] = 1f - channels[30];
				}
				fillMode = (FillMode)2;
			}
			else
			{
				if ((int)fillMode != 3)
				{
					channels[30] = 1f - channels[30];
				}
				fillMode = (FillMode)3;
			}
			if (!selector)
			{
				player.PlayOnDown();
			}
			selector = true;
		}
		else if ((input.oldRight && input.RightTriggerRelease()) || (input.oldLeft && input.LeftTriggerRelease()))
		{
			if (selector)
			{
				player.PlayOnUp();
			}
			selector = false;
			ShootTargets();
		}
		if ((input.KeyPressed((Keys)65) || input.PadPressed((Buttons)256)) && !MEGA_ON && powerAmounts[0] > 0)
		{
			MEGA_ON = true;
			Get().PlayCue("MegaAct");
		}
		if ((input.KeyPressed((Keys)83) || input.PadPressed((Buttons)512)) && !FREEZE_ON && powerAmounts[1] > 0 && !movingToNextZone)
		{
			FREEZE_ON = true;
			freezeLeft = FREEZE_TIME;
			_freezeFirstPass = _firstPass;
			_freezeFirstBeat = _firstBeat;
			_freezeBeat = _curBeat;
			_freezeTime = _curTime;
			PauseMusic();
			Get().PlayCue("FreezeAct");
			TargetEffectTimestop targetEffectTimestop = new TargetEffectTimestop();
			targetEffectTimestop.activated = true;
			targetEffectTimestop.countDown = FREEZE_TIME;
			targetEffectTimestop.maxCountdown = FREEZE_TIME;
			Get().targetFX.Insert(0, targetEffectTimestop);
		}
		if ((input.KeyPressed((Keys)80) || input.PadPressed((Buttons)16)) && !paused)
		{
			PauseGame();
		}
		if (FREEZE_ON)
		{
			freezeLeft -= (float)gametime.ElapsedGameTime.TotalSeconds;
			if (freezeLeft <= 0f)
			{
				FREEZE_ON = false;
				powerAmounts[1]--;
				PlayMusic();
			}
		}
		player.Update(gametime);
		ps.Update(gametime);
		hud.Update(gametime);
		if (!FREEZE_ON)
		{
			if (skyFlowToggle)
			{
				skyPS.Update(gametime);
				skyCooldown -= (float)gametime.ElapsedGameTime.TotalSeconds;
				if (skyCooldown <= 0f)
				{
					skyPS.AddPlaneParticles(new Vector3(-100f, -100f, 0f) - 3f * skyFlow, new Vector3(100f, 100f, 0f) - 3f * skyFlow, new Vector3(0f, 0f, -100f), 0.2f, 0f, 4f, 0f, new Vector4(1f, 1f, 1f, 1f), 50, 0.01f, 0.1f, 0f, skyFlow);
					skyCooldown += skyMaxCooldown;
				}
			}
			level.Update(gametime);
			if (movingToNextZone)
			{
				level.ActiveZone.playerPath.GetMatrix(ref _playerPos, ref _playerDir, ref _playerUp);
			}
			for (int num = enems.Count - 1; num >= 0; num--)
			{
				if (num < enems.Count)
				{
					enems[num].act(gametime);
				}
			}
			for (int i = 0; i < CHANNEL_NUM; i++)
			{
				if (channels[i] > 0f)
				{
					channels[i] -= (float)gametime.ElapsedGameTime.TotalMilliseconds / 500f;
					if (channels[i] < 0f)
					{
						channels[i] = 0f;
					}
				}
			}
		}
		else
		{
			for (int num2 = enems.Count - 1; num2 >= 0; num2--)
			{
				enems[num2].act(emptytime);
			}
			for (int j = 28; j < 30; j++)
			{
				if (channels[j] > 0f)
				{
					channels[j] -= (float)gametime.ElapsedGameTime.TotalMilliseconds / 500f;
					if (channels[j] < 0f)
					{
						channels[j] = 0f;
					}
				}
			}
		}
		if (((selector && numTargeted < 8) || MEGA_ON) && OnBeat(Beats.Sixteenth))
		{
			targetTransform = Matrix.Invert(MapObjectToSystem(Vector3.Zero, playerDir, playerUp)) * Matrix.CreateTranslation(-playerPos);
			worldViewProjTransform = Matrix.Multiply(Matrix.Multiply(world, viewMatrix), projectionMatrix);
			if (!MEGA_ON)
			{
				for (int num3 = enems.Count - 1; num3 >= 0; num3--)
				{
					selFX = enems[num3].lockOn(8 - numTargeted);
					if (selFX.fx.Count <= 0)
					{
						continue;
					}
					if (maxFX.fx.Count != 0)
					{
						Vector3 val = Vector3.Subtract(selFX.fx[0].eTarget.absolutePos(), cameraLoc);
						float num4 = ((Vector3)(ref val)).LengthSquared();
						Vector3 val2 = Vector3.Subtract(maxFX.fx[0].eTarget.absolutePos(), cameraLoc);
						if (!(num4 < ((Vector3)(ref val2)).LengthSquared()))
						{
							selFX.fx[0].enem.ClearLock(selFX.fx[0].eTarget);
							goto IL_0ae7;
						}
					}
					if (maxFX.fx.Count > 0)
					{
						maxFX.fx[0].enem.ClearLock(maxFX.fx[0].eTarget);
					}
					maxFX = selFX;
					index = num3;
					goto IL_0ae7;
					IL_0ae7:
					if (enems[num3] is SerpentTail)
					{
						break;
					}
				}
			}
			else
			{
				int num5 = enems.Count - 1;
				while (num5 >= 0 && maxFX.fx.Count < 32)
				{
					selFX = enems[num5].lockOn(32 - maxFX.fx.Count);
					for (int k = 0; k < selFX.fx.Count; k++)
					{
						if (maxFX.fx.Count < 32)
						{
							maxFX.fx.Add(selFX.fx[k]);
						}
						else
						{
							selFX.fx[k].enem.ClearLock(selFX.fx[k].eTarget);
						}
					}
					if (maxFX.fx.Count == 32)
					{
						break;
					}
					num5--;
				}
			}
			numTargeted += maxFX.fx.Count;
			if (maxFX.fx.Count > 0)
			{
				maxFX.fx[0].lockNum = numTargeted;
				for (int l = 0; l < maxFX.fx.Count; l++)
				{
					if (numTargeted > 1 && targetFX.Count > 0 && !MEGA_ON)
					{
						targetFX[targetFX.Count - 1].next = maxFX.fx[l];
						maxFX.fx[l].prev = targetFX[targetFX.Count - 1];
					}
					targetFX.Add(maxFX.fx[l]);
				}
				AddText(enems[index].name());
				player.PlayOnLock((targetFX[targetFX.Count - 1].eTarget.fillMode == targetFX[targetFX.Count - 1].fillMode) ? (-20f) : 0f);
				if (targetFX[targetFX.Count - 1].eTarget.fillMode != targetFX[targetFX.Count - 1].fillMode)
				{
					channels[28] = ((channels[28] > 0.75f) ? Math.Min(channels[28] + 0.05f, 1f) : 0.6f);
					hud.scale = 1.8f;
				}
			}
			if (MEGA_ON)
			{
				ShootTargets();
				MEGA_ON = false;
				powerAmounts[0]--;
			}
		}
		for (int num6 = targetFX.Count - 1; num6 >= 0; num6--)
		{
			targetFX[num6].Update(gametime);
		}
		for (int m = 0; m < 2; m++)
		{
			if (powerScore[m] <= maxPower[m])
			{
				continue;
			}
			if (powerAmounts[m] < 3)
			{
				powerScore[m] -= maxPower[m];
				powerAmounts[m]++;
				Get().PlayCue((m == 0) ? "Mega" : "Freeze", 0f);
				TargetEffectGlow targetEffectGlow = new TargetEffectGlow();
				targetEffectGlow.activated = true;
				targetEffectGlow.countDown = 3f;
				targetEffectGlow.maxCountdown = 3f;
				if (m == 0)
				{
					targetEffectGlow.screenPos = new Vector2(hud.wirePos[powerAmounts[m] - 1].X, hud.wirePos[powerAmounts[m] - 1].Y);
					targetEffectGlow.baseColor = new Vector3(0.7f, 0.7f, 2f);
				}
				else
				{
					targetEffectGlow.screenPos = new Vector2(hud.solidPos[powerAmounts[m] - 1].X, hud.solidPos[powerAmounts[m] - 1].Y);
					targetEffectGlow.baseColor = new Vector3(2f, 0.8f, 0.6f);
				}
				Get().targetFX.Insert(0, targetEffectGlow);
			}
			else
			{
				powerScore[m] = maxPower[m];
			}
		}
		for (int num7 = fallFX.Count - 1; num7 >= 0; num7--)
		{
			fallFX[num7].Update(gametime);
		}
		SharkTail.pdColl.act(gametime);
		maxFX.fx.Clear();
	}

	public void UpdateTotalTime(GameTime gametime)
	{
		_totalTime += gametime.ElapsedGameTime.TotalSeconds;
	}

	public void PlayerHit()
	{
		if (hitCountdown < 0.01f)
		{
			UpdatePlayerMesh(player.level, show: false);
			player.level--;
			if (player.level <= 0)
			{
				GameplayChange gameplayChange = new GameplayChange("fade", 3f);
				gameplayChange.start();
				enems.Add(gameplayChange);
				gameplayChange = new GameplayChange("quit", 0f);
				EnemyQueuePart enemyQueuePart = new EnemyQueuePart(gameplayChange, new TimeCondition(2.799999952316284));
				enemyQueuePart.cond.Start();
				Get().eQueue.PushAtFront(enemyQueuePart);
			}
			TargetEffectDamage targetEffectDamage = new TargetEffectDamage();
			targetEffectDamage.activated = true;
			targetEffectDamage.countDown = 3f;
			targetEffectDamage.maxCountdown = 3f;
			Get().targetFX.Insert(0, targetEffectDamage);
			Get().PlayCue("Damage", 0f);
			hitCountdown = maxCountdown;
		}
	}

	private void ShootPlayerTile(string boneName)
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		ModelWrapper mPlayer = Get().player.mPlayer;
		int num = 0;
		int num2 = mPlayer.boneNames[boneName][1];
		for (int i = 0; i < mPlayer.indices[num].Length - 1; i += 3)
		{
			if (flag)
			{
				break;
			}
			int num3 = mPlayer.vertices[num][mPlayer.indices[num][i]].boneNum(0);
			if (num2 == num3)
			{
				Get().player.pdColl.AddPlane(ref player.mPlayer, num, i, Get().player.pn, fillMode);
			}
		}
	}

	private void UpdatePlayerMesh(int playerLevel, bool show)
	{
		if (player.mPlayer.boneNames.ContainsKey("Armature__" + playerLevel))
		{
			ShootPlayerTile("Armature__" + playerLevel);
			player.playerAnim.bonePoses[player.playerBones[player.mPlayer.model.Bones["Armature__" + playerLevel]]].enabled = show;
			((GameComponent)player.playerAnim).Update(emptytime);
		}
	}

	private void PauseGame()
	{
		PauseGame(controllerDisconnect: false);
	}

	private void PauseGame(bool controllerDisconnect)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		foreach (GameComponent item in (Collection<IGameComponent>)(object)CoreGame.Components)
		{
			GameComponent val = item;
			if (val is PauseComponent)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			((Collection<IGameComponent>)(object)coreGame.Components).Add((IGameComponent)(object)new PauseComponent(coreGame, input.curPad, input.leftHeld, input.rightHeld));
		}
		if (controllerDisconnect)
		{
			Guide.BeginShowMessageBox(input.ActivePlayerIndex, "Please reconnect the controller", "Please reconnect the controller", (IEnumerable<string>)new string[1] { "Ok" }, 0, (MessageBoxIcon)1, (AsyncCallback)ControllerReturn, (object)null);
		}
		PauseMusic();
		paused = true;
	}

	private void ControllerReturn(IAsyncResult result)
	{
	}

	public void PauseMusic()
	{
		foreach (Cue bgCue in bgCues)
		{
			if (bgCue.IsPlaying)
			{
				bgCue.Pause();
			}
		}
		foreach (Cue activeCue in activeCues)
		{
			if (activeCue.IsPlaying)
			{
				activeCue.Pause();
			}
		}
	}

	public void PlayMusic()
	{
		foreach (Cue bgCue in bgCues)
		{
			if (bgCue.IsPaused)
			{
				bgCue.Resume();
			}
		}
		foreach (Cue activeCue in activeCues)
		{
			if (activeCue.IsPaused)
			{
				activeCue.Resume();
			}
		}
	}

	public void Draw(GameTime gametime)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		_fogEffect.CurrentTechnique = _fogEffect.Techniques["Colored"];
		_fogEffect.Parameters["FakeFog"].SetValue(false);
		_fogEffect.Parameters["xDoubleSided"].SetValue(true);
		_fogEffect.Parameters["xView"].SetValue(_view);
		Get().fogEffect.Parameters["xVProj"].SetValue(Get().viewMatrix * Get().projectionMatrix);
		_fogEffect.Parameters["xWorld"].SetValue(_world);
		_graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		float num = (2f - channels[0]) * channels[0];
		_fogEffect.Parameters["xFogColor"].SetValue(new Vector4(Vector3.Lerp(level.baseColor, level.flashColor, num), num));
		graphics.GraphicsDevice.RenderState.StencilEnable = true;
		graphics.GraphicsDevice.RenderState.StencilFunction = (CompareFunction)8;
		graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		graphics.GraphicsDevice.RenderState.ReferenceStencil = 1;
		graphics.GraphicsDevice.Clear((ClearOptions)4, Color.Black, 0f, 0);
		graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		graphics.GraphicsDevice.RenderState.ReferenceStencil = 1;
		DrawBackground(gametime);
		GraphicsSettings();
		LineUpCamera();
		graphics.GraphicsDevice.RenderState.DepthBufferEnable = true;
		for (int num2 = fallFX.Count - 1; num2 >= 0; num2--)
		{
			fallFX[num2].draw();
		}
		for (int num3 = enems.Count - 1; num3 >= 0; num3--)
		{
			enems[num3].draw(gametime);
		}
		pythdigit01.draw(gametime);
		fish01.draw(gametime);
		sTail01.draw(gametime);
		Get().SwitchEffectTechnique("Colored");
		ps.Draw(gametime);
		if (skyFlowToggle)
		{
			skyPS.Draw(gametime);
		}
		SharkTail.pdColl.draw(gametime);
		level.Draw(gametime);
		Matrix val = LineUpCameraNoPlayerPos();
		Get().fogEffect.Parameters["xView"].SetValue(val);
		Get().fogEffect.Parameters["xVProj"].SetValue(val * _projection);
		Get().fogEffect.Parameters["EyePosition"].SetValue(new Vector4(ActualCameraPosNoPlayerPos(), 1f));
		player.DrawPlayer(gametime);
		Get().fogEffect.Parameters["EyePosition"].SetValue(new Vector4(ActualCameraPos(), 1f));
		Get().fogEffect.Parameters["xView"].SetValue(_view);
		Get().fogEffect.Parameters["xVProj"].SetValue(_view * _projection);
		_hud.Draw_FrontCube(gametime);
		_graphics.GraphicsDevice.SetRenderTarget(0, (RenderTarget2D)null);
		if (bloom_on)
		{
			DrawGlow();
		}
		else
		{
			Get().DrawFullscreenQuad(Get().worldTarget.GetTexture(), WIDTH, HEIGHT, null, Color.White);
		}
		_graphics.GraphicsDevice.VertexDeclaration = _vertDec;
		((Effect)_flatEffect).Begin();
		((Effect)_flatEffect).CurrentTechnique.Passes[0].Begin();
		_hud.Draw(gametime);
		((Effect)_flatEffect).CurrentTechnique.Passes[0].End();
		((Effect)_flatEffect).End();
	}

	public void DrawBackground(GameTime gametime)
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		_graphics.GraphicsDevice.SetRenderTarget(0, glowTarget2);
		graphics.GraphicsDevice.Vertices[0].SetSource(fullScreenBuff, 0, VertexPositionTexture.SizeInBytes);
		_graphics.GraphicsDevice.Textures[0] = (Texture)(object)_backTex;
		graphics.GraphicsDevice.VertexDeclaration = fullScreenDec;
		_combineEffect.Parameters["ColorTint"].SetValue(_fogEffect.Parameters["xFogColor"].GetValueVector4());
		_combineEffect.Parameters["xGlow"].SetValue(false);
		_combineEffect.CurrentTechnique = _combineEffect.Techniques["NoEdit"];
		_combineEffect.Begin();
		_graphics.GraphicsDevice.Clear((ClearOptions)3, new Color(new Vector4(level.baseColor, 1f)), 1f, 0);
		_combineEffect.CurrentTechnique.Passes[0].Begin();
		_graphics.GraphicsDevice.DrawPrimitives((PrimitiveType)6, 0, 2);
		_combineEffect.CurrentTechnique.Passes[0].End();
		_graphics.GraphicsDevice.SetRenderTarget(0, worldTarget);
		graphics.GraphicsDevice.Clear((ClearOptions)3, new Color(new Vector4(0f, 0f, 0f, 0f)), 1f, 0);
		_combineEffect.Parameters["ColorTint"].SetValue(new Vector4(1f, 1f, 1f, 1f));
		_graphics.GraphicsDevice.Textures[0] = (Texture)(object)glowTarget2.GetTexture();
		_combineEffect.CurrentTechnique.Passes[0].Begin();
		_graphics.GraphicsDevice.DrawPrimitives((PrimitiveType)6, 0, 2);
		_combineEffect.CurrentTechnique.Passes[0].End();
		_combineEffect.End();
		_combineEffect.CurrentTechnique = _combineEffect.Techniques["FTheColor"];
		DrawFullscreenQuad(glowTarget2.GetTexture(), WIDTH, HEIGHT, null, Color.White, clearScreen: false, 0, 0);
		_hud.DrawInBack(gametime);
	}

	public void DrawGlow()
	{
		_graphics.GraphicsDevice.SetRenderTarget(0, (RenderTarget2D)null);
		graphics.GraphicsDevice.Vertices[0].SetSource(fullScreenBuff, 0, VertexPositionTexture.SizeInBytes);
		graphics.GraphicsDevice.VertexDeclaration = fullScreenDec;
		graphics.GraphicsDevice.RenderState.ReferenceStencil = 0;
		graphics.GraphicsDevice.RenderState.StencilFunction = (CompareFunction)3;
		_combineEffect.CurrentTechnique = _combineEffect.Techniques["Bloom"];
		_combineEffect.Begin();
		DrawFullScreen(ref _glowTarget, ref _worldTarget, 0, clear: true, addDepth: true);
		_graphics.GraphicsDevice.RenderState.StencilEnable = false;
		DrawFullScreen(ref _glowTarget2, ref _glowTarget, 1, clear: true, addDepth: false);
		DrawFullScreen(ref _glowTarget3, ref _glowTarget2, 2, clear: true, addDepth: false);
		_graphics.GraphicsDevice.Textures[1] = (Texture)(object)worldTarget.GetTexture();
		_graphics.GraphicsDevice.SetRenderTarget(0, (RenderTarget2D)null);
		DrawFullScreen(ref _glowTarget3, 3, clear: true, addDepth: false);
		_combineEffect.End();
	}

	public void DrawFullScreen(ref RenderTarget2D dest, ref RenderTarget2D source, int passNum, bool clear, bool addDepth)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		_graphics.GraphicsDevice.SetRenderTarget(0, dest);
		if (clear)
		{
			_graphics.GraphicsDevice.Clear((ClearOptions)(1 | (addDepth ? 1 : 2)), new Color(new Vector4(0f, 0f, 0f, 0f)), 1f, 0);
		}
		_graphics.GraphicsDevice.Textures[0] = (Texture)(object)source.GetTexture();
		if (addDepth)
		{
			_graphics.GraphicsDevice.DepthStencilBuffer = bitTarget;
			_graphics.GraphicsDevice.RenderState.DepthBufferEnable = true;
		}
		_combineEffect.CurrentTechnique.Passes[passNum].Begin();
		graphics.GraphicsDevice.DrawPrimitives((PrimitiveType)6, 0, 2);
		_combineEffect.CurrentTechnique.Passes[passNum].End();
	}

	public void DrawFullScreen(ref RenderTarget2D source, int passNum, bool clear, bool addDepth)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		_graphics.GraphicsDevice.SetRenderTarget(0, (RenderTarget2D)null);
		if (clear)
		{
			_graphics.GraphicsDevice.Clear((ClearOptions)(1 | (addDepth ? 1 : 2)), new Color(new Vector4(0f, 0f, 0f, 0f)), 1f, 0);
		}
		_graphics.GraphicsDevice.Textures[0] = (Texture)(object)source.GetTexture();
		if (addDepth)
		{
			_graphics.GraphicsDevice.DepthStencilBuffer = bitTarget;
			_graphics.GraphicsDevice.RenderState.DepthBufferEnable = true;
		}
		_combineEffect.CurrentTechnique.Passes[passNum].Begin();
		graphics.GraphicsDevice.DrawPrimitives((PrimitiveType)6, 0, 2);
		_combineEffect.CurrentTechnique.Passes[passNum].End();
	}

	private void ShootTargets()
	{
		ScoreGroup scoreGroup = new ScoreGroup();
		if (numTargeted <= 0)
		{
			return;
		}
		scoreGroup.totalPoints = numTargeted;
		string s = numTargeted.ToString("FIRE_[{0}]");
		AddText(s);
		numTargeted = 0;
		for (int i = 0; i < targetFX.Count; i++)
		{
			if (!targetFX[i].activated)
			{
				targetFX[i].waitBeat = (Get().curBeat + maxBeat - 1) % maxBeat;
				targetFX[i].activated = true;
				targetFX[i].countDown = -0.01f;
				targetFX[i].wade = targetFX[i].enem.GetSoundCue(i);
				if (MEGA_ON)
				{
					targetFX[i].wade.cueName = "";
				}
				scoreGroup.scores.Add(targetFX[i].eTarget.score);
			}
		}
		_scoreFlow.Push(scoreGroup);
	}

	public void AddText(string s)
	{
		_textFlow += s;
		if (_textFlow.Length > 1000)
		{
			_textFlow = _textFlow.Substring(_textFlow.IndexOf('\n') + 1);
		}
		_textFlow = WrapStringNoLine(_textFlow, 0.1f * (float)WIDTH, 0.1f * HUD.textScale, Get().hud.HUDfont);
	}

	public void PlayCue(string CueName, float volume)
	{
		if (CueName != "")
		{
			Cue cue = sB.GetCue(CueName);
			cue.SetVariable("Volume", volume);
			cue.Play();
			activeCues.Add(cue);
		}
	}

	public void OpenPlay(string CueName)
	{
		Cue cue = sBOpen.GetCue(CueName);
		cue.Play();
	}

	public void PlayCue(string CueName)
	{
		PlayCue(CueName, 0f);
	}

	public void BGPlayCue(string CueName)
	{
		Cue cue = sB.GetCue(CueName);
		cue.Play();
		bgCues.Add(cue);
	}

	public void CleanupCues()
	{
		for (int num = activeCues.Count - 1; num >= 0; num--)
		{
			if (activeCues[num].IsStopped)
			{
				activeCues[num].Dispose();
				activeCues.RemoveAt(num);
			}
		}
		for (int num = bgCues.Count - 1; num >= 0; num--)
		{
			if (bgCues[num].IsStopped)
			{
				bgCues[num].Dispose();
				bgCues.RemoveAt(num);
			}
		}
	}

	public void StopAndClearBGCues()
	{
		for (int num = bgCues.Count - 1; num >= 0; num--)
		{
			if (bgCues[num].IsPlaying || bgCues[num].IsPreparing)
			{
				bgCues[num].Stop((AudioStopOptions)1);
			}
			bgCues[num].Dispose();
			bgCues.RemoveAt(num);
		}
	}

	public void StopAndClearAllCues()
	{
		for (int num = activeCues.Count - 1; num >= 0; num--)
		{
			if (activeCues[num].IsPlaying || activeCues[num].IsPreparing)
			{
				activeCues[num].Stop((AudioStopOptions)1);
			}
			activeCues[num].Dispose();
			activeCues.RemoveAt(num);
		}
		StopAndClearBGCues();
	}

	public bool OnBeat(Beats _beat)
	{
		if (firstBeat)
		{
			return curBeat % (int)_beat == 0;
		}
		return false;
	}

	public bool OnExactBeat(int i)
	{
		if (firstBeat)
		{
			return curBeat == i;
		}
		return false;
	}

	public void DrawModel(ref ModelWrapper toDraw)
	{
		DrawModel(ref toDraw, clearEpc: false, disableAnim: false);
	}

	public void DrawModel(ref ModelWrapper toDraw, bool clearEpc)
	{
		DrawModel(ref toDraw, clearEpc, disableAnim: false);
	}

	public void DrawModel(ref ModelWrapper toDraw, bool clearEpc, bool disableAnim)
	{
		DrawModel(ref toDraw, clearEpc, disableAnim, ref toDraw.indicesToDraw, null);
	}

	public void DrawModel(ref ModelWrapper toDraw, bool clearEpc, bool disableAnim, ref List<int>[] indicesToDraw)
	{
		DrawModel(ref toDraw, clearEpc, disableAnim, ref indicesToDraw, null);
	}

	public void DrawModel(ref ModelWrapper toDraw, bool clearEpc, bool disableAnim, ref List<int>[] indicesToDraw, VertexDeclaration vertDec)
	{
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)(object)toDraw.model.Meshes).Count; i++)
		{
			if (vertDec == null)
			{
				_graphics.GraphicsDevice.VertexDeclaration = ((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)toDraw.model.Meshes)[i].MeshParts)[0].VertexDeclaration;
			}
			else
			{
				_graphics.GraphicsDevice.VertexDeclaration = vertDec;
			}
			SetUpEffect(ref toDraw.epc[i], clearEpc);
			if (toDraw.palette != null && toDraw.palette[i] != null && !disableAnim)
			{
				_fogEffect.Parameters["xPose"].SetValue(Matrix.Identity);
				_fogEffect.Parameters["usePalette"].SetValue(true);
				_fogEffect.Parameters["MatrixPalette"].SetValue(toDraw.palette[i]);
			}
			else
			{
				_fogEffect.Parameters["xPose"].SetValue(toDraw.transforms[((ReadOnlyCollection<ModelMesh>)(object)toDraw.model.Meshes)[i].ParentBone.Index]);
				_fogEffect.Parameters["usePalette"].SetValue(false);
			}
			DrawMesh(((ReadOnlyCollection<ModelMesh>)(object)toDraw.model.Meshes)[i], ref indicesToDraw[i]);
		}
		_graphics.GraphicsDevice.VertexDeclaration = VertDec;
		_fogEffect.Parameters["usePalette"].SetValue(false);
	}

	public void DrawModelWithOtherBuffer(ref ModelWrapper toDraw, bool clearEpc, bool disableAnim, ref List<int>[] indicesToDraw, VertexDeclaration vertDec)
	{
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < toDraw.indexBuffer.Length; i++)
		{
			_graphics.GraphicsDevice.VertexDeclaration = toDraw.vertDec[i];
			SetUpEffect(ref toDraw.epc[i], clearEpc);
			if (toDraw.palette != null && toDraw.palette[i] != null && !disableAnim)
			{
				_fogEffect.Parameters["xPose"].SetValue(Matrix.Identity);
				_fogEffect.Parameters["usePalette"].SetValue(true);
				_fogEffect.Parameters["MatrixPalette"].SetValue(toDraw.palette[i]);
			}
			else
			{
				_fogEffect.Parameters["xPose"].SetValue(toDraw.transforms[toDraw.parentBones[i]]);
				_fogEffect.Parameters["usePalette"].SetValue(false);
			}
			_fogEffect.Parameters["xDoubleSided"].SetValue(true);
			DrawVerts(toDraw.vertBuffer[i], toDraw.indexBuffer[i], toDraw.vertDec[i], toDraw.vertCount[i]);
		}
		_graphics.GraphicsDevice.VertexDeclaration = VertDec;
		_fogEffect.Parameters["usePalette"].SetValue(false);
	}

	public void DrawModelEffectStarted(ref ModelWrapper toDraw)
	{
		DrawModelEffectStarted(ref toDraw, clearEpc: false);
	}

	public void DrawModelEffectStarted(ref ModelWrapper toDraw, bool clearEpc)
	{
		for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)(object)toDraw.model.Meshes).Count; i++)
		{
			DrawMeshEffectStarted(((ReadOnlyCollection<ModelMesh>)(object)toDraw.model.Meshes)[i], ref toDraw.indicesToDraw[i]);
		}
	}

	public void DrawMeshEffectStarted(ModelMesh m, ref List<int> iToDraw)
	{
		int count = ((ReadOnlyCollection<ModelMeshPart>)(object)m.MeshParts).Count;
		for (int i = 0; i < count; i++)
		{
			DrawMeshPart(((ReadOnlyCollection<ModelMeshPart>)(object)m.MeshParts)[i], m, ref iToDraw);
		}
	}

	public void DrawMeshPartEffectStarted(ModelMeshPart m, ModelMesh par, ref List<int> iToDraw)
	{
		if (m.NumVertices > 0)
		{
			GraphicsDevice graphicsDevice = m.VertexDeclaration.GraphicsDevice;
			graphicsDevice.Indices = par.IndexBuffer;
			graphicsDevice.Vertices[0].SetSource(par.VertexBuffer, m.StreamOffset, m.VertexStride);
			for (int i = 0; i < iToDraw.Count - 1; i += 2)
			{
				graphicsDevice.DrawIndexedPrimitives((PrimitiveType)4, m.BaseVertex, 0, m.NumVertices, iToDraw[i], (iToDraw[i + 1] - iToDraw[i] + 1) / 3);
			}
		}
	}

	public void DrawMesh(ModelMesh m, ref List<int> iToDraw)
	{
		int count = ((ReadOnlyCollection<ModelMeshPart>)(object)m.MeshParts).Count;
		for (int i = 0; i < count; i++)
		{
			ModelMeshPart val = ((ReadOnlyCollection<ModelMeshPart>)(object)m.MeshParts)[i];
			Effect effect = val.Effect;
			effect.Begin((SaveStateMode)0);
			try
			{
				int count2 = effect.CurrentTechnique.Passes.Count;
				for (int j = 0; j < count2; j++)
				{
					EffectPass val2 = effect.CurrentTechnique.Passes[j];
					val2.Begin();
					DrawMeshPart(val, m, ref iToDraw);
					val2.End();
				}
			}
			finally
			{
				effect.End();
			}
		}
	}

	public void DrawVerts(VertexBuffer vb, IndexBuffer ib, VertexDeclaration vd, int vertNum)
	{
		fogEffect.Begin((SaveStateMode)0);
		for (int i = 0; i < fogEffect.CurrentTechnique.Passes.Count; i++)
		{
			fogEffect.CurrentTechnique.Passes[i].Begin();
			DrawVertPart(vb, ib, vd, vertNum);
			fogEffect.CurrentTechnique.Passes[i].End();
		}
		fogEffect.End();
	}

	public void DrawVertPart(VertexBuffer vb, IndexBuffer ib, VertexDeclaration vd, int vertNum)
	{
		GraphicsDevice graphicsDevice = graphics.GraphicsDevice;
		graphicsDevice.VertexDeclaration = vd;
		graphicsDevice.Vertices[0].SetSource(vb, 0, vb.SizeInBytes / vertNum);
		graphicsDevice.DrawPrimitives((PrimitiveType)4, 0, vertNum);
	}

	public void DrawMeshPart(ModelMeshPart m, ModelMesh par, ref List<int> iToDraw)
	{
		if (m.NumVertices > 0)
		{
			GraphicsDevice graphicsDevice = m.VertexDeclaration.GraphicsDevice;
			graphicsDevice.Indices = par.IndexBuffer;
			graphicsDevice.VertexDeclaration = m.VertexDeclaration;
			graphicsDevice.Vertices[0].SetSource(par.VertexBuffer, m.StreamOffset, m.VertexStride);
			for (int i = 0; i < iToDraw.Count - 1; i += 2)
			{
				graphicsDevice.DrawIndexedPrimitives((PrimitiveType)4, m.BaseVertex, 0, m.NumVertices, iToDraw[i], (iToDraw[i + 1] - iToDraw[i] + 1) / 3);
			}
		}
	}

	public unsafe void DrawModel(ModelWrapper m, int meshNum, Color c)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		Matrix[] array = (Matrix[])(object)new Matrix[((ReadOnlyCollection<ModelBone>)(object)m.model.Bones).Count];
		m.model.CopyAbsoluteBoneTransformsTo(array);
		fogEffect.Parameters["xProjection"].SetValue(_fogEffect.Parameters["xProjection"].GetValueMatrix());
		fogEffect.Parameters["xView"].SetValue(_fogEffect.Parameters["xView"].GetValueMatrix());
		for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)(object)m.model.Meshes).Count; i++)
		{
			Enumerator enumerator = ((ReadOnlyCollection<ModelMesh>)(object)m.model.Meshes)[i].Effects.GetEnumerator();
			try
			{
				while (((Enumerator)(ref enumerator)).MoveNext())
				{
					Effect current = ((Enumerator)(ref enumerator)).Current;
					SetUpEffect(ref m.epc[i], clearEpc: false);
					_fogEffect.Parameters["xPose"].SetValue(m.transforms[((ReadOnlyCollection<ModelMesh>)(object)m.model.Meshes)[i].ParentBone.Index]);
					_fogEffect.Parameters["usePalette"].SetValue(false);
					if (i == meshNum)
					{
						current.Parameters["EmissiveColor"].SetValue(((Color)(ref c)).ToVector3());
					}
				}
			}
			finally
			{
				((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
			}
			((ReadOnlyCollection<ModelMesh>)(object)m.model.Meshes)[i].Draw((SaveStateMode)0);
		}
		_fogEffect.Parameters["xWorld"].SetValue(Matrix.Identity);
	}

	public void SetUpEffect(ref EffectParameterCollectionRedux epc, bool clearEpc)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Expected O, but got Unknown
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		_fogEffect.Parameters["DiffuseColor"].SetValue((Vector3)epc["DiffuseColor"]);
		if (!SpecifyAlpha)
		{
			_fogEffect.Parameters["Alpha"].SetValue((float)epc["Alpha"]);
		}
		_fogEffect.Parameters["xEnableLighting"].SetValue((bool)epc["xEnableLighting"]);
		_fogEffect.Parameters["DirLight0Direction"].SetValue((Vector3)epc["DirLight0Direction"]);
		_fogEffect.Parameters["EmissiveColor"].SetValue((Vector3)epc["EmissiveColor"]);
		_fogEffect.Parameters["SpecularColor"].SetValue((Vector3)epc["SpecularColor"]);
		_fogEffect.Parameters["SpecularPower"].SetValue((float)epc["SpecularPower"]);
		_fogEffect.Parameters["TextureEnabled"].SetValue((bool)epc["TextureEnabled"]);
		if ((bool)epc["TextureEnabled"])
		{
			_fogEffect.Parameters["BasicTexture"].SetValue((Texture)(Texture2D)epc["BasicTexture"]);
			_fogEffect.Parameters["TextureMix"].SetValue((Vector4)epc["TextureMix"]);
		}
		if (epc["xWorld"] != null)
		{
			_fogEffect.Parameters["xWorld"].SetValue((Matrix)epc["xWorld"]);
		}
		else
		{
			_fogEffect.Parameters["xWorld"].SetValue(_world);
		}
		if (epc["ShinePos"] != null)
		{
			_fogEffect.Parameters["ShinePos"].SetValue((Vector3)epc["ShinePos"]);
			_fogEffect.Parameters["ShineDist"].SetValue((float)epc["ShineDist"]);
		}
		if (epc["xGlow"] != null && (bool)epc["xGlow"])
		{
			_graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		}
		else if (epc["xGlow"] != null && !(bool)epc["xGlow"])
		{
			_graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		}
	}

	public void MoveToNextZone()
	{
		movingToNextZone = true;
		StopAndClearBGCues();
		if (level.ActiveZone.muteSound)
		{
			playBGMusic = false;
		}
		curBeat = maxBeat - 1;
		curTime = -0.01f;
		elaspedEndTime = -1;
	}

	public void MoveCursor(Vector3 dir)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		float num = 0.02f;
		if (!(dir != Vector3.Zero))
		{
			return;
		}
		Vector3 val = Vector3.Normalize(Vector3.Transform(cursorDir, Matrix.CreateFromAxisAngle(Vector3.Cross(dir, new Vector3(0f, 0f, -1f)), (float)Math.Acos(Vector3.Dot(dir, new Vector3(0f, 0f, -1f))) * num)));
		if (Math.Acos(Vector3.Dot(val, Vector3.Backward)) <= Math.PI * 7.0 / 16.0)
		{
			_cursorDir = val;
			Vector3 val2 = Vector3.Cross(_cursorDir, _cursorUp);
			val2.Y = 0f;
			val2 = Vector3.Normalize(val2);
			_cursorUp = Vector3.Normalize(Vector3.Cross(val2, _cursorDir));
			if (Math.Acos(Vector3.Dot(cursorDir, cameraDir)) > Math.PI / 16.0)
			{
				val = Vector3.Normalize(Vector3.Transform(cameraDir, Matrix.CreateFromAxisAngle(Vector3.Cross(dir, new Vector3(0f, 0f, -1f)), (float)Math.Acos(Vector3.Dot(dir, new Vector3(0f, 0f, -1f))) * 0.02f)));
				_cameraDir = val;
				val2 = Vector3.Cross(_cameraDir, _cameraUp);
				val2.Y = 0f;
				val2 = Vector3.Normalize(val2);
				_cameraUp = Vector3.Normalize(Vector3.Cross(val2, _cameraDir));
			}
		}
	}

	public void MovePlayerDir(Vector3 dir)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.Normalize(Vector3.Cross(dir, Vector3.Up));
		_playerUp = Vector3.Normalize(Vector3.Cross(val, dir));
		_playerDir = dir;
	}

	public void SwitchEffectTechnique(string _tech)
	{
		if (_fogEffect.CurrentTechnique.Name != _tech)
		{
			_fogEffect.CurrentTechnique = _fogEffect.Techniques[_tech];
		}
	}

	public unsafe void LinkEffect(Model model, GraphicsDevice graphicsDevice, Effect effect)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Enumerator enumerator = model.Meshes.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				ModelMesh current = ((Enumerator)(ref enumerator)).Current;
				Enumerator enumerator2 = current.MeshParts.GetEnumerator();
				try
				{
					while (((Enumerator)(ref enumerator2)).MoveNext())
					{
						ModelMeshPart current2 = ((Enumerator)(ref enumerator2)).Current;
						current2.Effect = effect;
					}
				}
				finally
				{
					((IDisposable)(*(Enumerator*)(&enumerator2))/*cast due to constrained. prefix*/).Dispose();
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	public static Matrix MapObjectToSystem(Vector3 pos, Vector3 p_dir, Vector3 p_up)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = p_dir;
		Vector3 val2 = p_up;
		Matrix identity = Matrix.Identity;
		val = Vector3.Normalize(val);
		val2 = Vector3.Normalize(val2);
		Vector3 val3 = Vector3.Cross(val, val2);
		val2 = Vector3.Cross(val3, val);
		identity.M11 = val3.X;
		identity.M12 = val3.Y;
		identity.M13 = val3.Z;
		identity.M21 = val2.X;
		identity.M22 = val2.Y;
		identity.M23 = val2.Z;
		identity.M31 = val.X;
		identity.M32 = val.Y;
		identity.M33 = val.Z;
		identity = Matrix.Transpose(identity);
		return identity * Matrix.CreateTranslation(pos);
	}

	public static Matrix MapObjectToSystem2(Vector3 pos, Vector3 p_dir, Vector3 p_up)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = p_dir;
		Vector3 val2 = p_up;
		Matrix identity = Matrix.Identity;
		val = Vector3.Normalize(val);
		val2 = Vector3.Normalize(val2);
		Vector3 val3 = Vector3.Cross(val, val2);
		val2 = Vector3.Cross(val3, val);
		identity.M11 = val3.X;
		identity.M12 = val2.X;
		identity.M13 = val.X;
		identity.M21 = val3.Y;
		identity.M22 = val2.Y;
		identity.M23 = val.Y;
		identity.M31 = val3.Z;
		identity.M32 = val2.Z;
		identity.M33 = val.Z;
		identity = Matrix.Transpose(identity);
		return identity * Matrix.CreateTranslation(pos);
	}

	public static Vector3 GetUpVector(Vector3 dir, Vector3 up, Vector3 olddir)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.Normalize(Vector3.Cross(up, olddir));
		return Vector3.Cross(Vector3.Normalize(dir), val);
	}

	public static Vector3 MapSystemToObject(Vector3 dir, Vector3 up, Vector3 toFind)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		Matrix identity = Matrix.Identity;
		dir = Vector3.Normalize(dir);
		up = Vector3.Normalize(up);
		Vector3 val = Vector3.Normalize(Vector3.Cross(up, dir));
		identity.M11 = val.X;
		identity.M12 = val.Y;
		identity.M13 = val.Z;
		identity.M21 = up.X;
		identity.M22 = up.Y;
		identity.M23 = up.Z;
		identity.M31 = dir.X;
		identity.M32 = dir.Y;
		identity.M33 = dir.Z;
		identity = Matrix.Transpose(identity);
		identity = Matrix.Invert(identity);
		return Vector3.Transform(toFind, identity);
	}

	public static Vector3 MapSystemToObject2(Vector3 dir, Vector3 up, Vector3 toFind)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		Matrix identity = Matrix.Identity;
		dir = Vector3.Normalize(dir);
		up = Vector3.Normalize(up);
		Vector3 val = Vector3.Normalize(Vector3.Cross(dir, up));
		identity.M11 = val.X;
		identity.M12 = val.Y;
		identity.M13 = val.Z;
		identity.M21 = up.X;
		identity.M22 = up.Y;
		identity.M23 = up.Z;
		identity.M31 = dir.X;
		identity.M32 = dir.Y;
		identity.M33 = dir.Z;
		identity = Matrix.Transpose(identity);
		identity = Matrix.Invert(identity);
		return Vector3.Transform(toFind, identity);
	}

	public static Effect GetFogEffect()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		Effect val = Get().content.Load<Effect>("Content\\Fog");
		Get()._fogEffect = val;
		val.Parameters["xWorld"].SetValue(Matrix.Identity);
		val.Parameters["xProjection"].SetValue(Get()._projection);
		val.Parameters["xEnableLighting"].SetValue(true);
		val.Parameters["xLightDirection"].SetValue(new Vector3(-0.5f, -0.5f, -1f));
		val.Parameters["xFogColor"].SetValue(new Vector4(0f, 0f, 0f, 0f));
		val.Parameters["xFogStart"].SetValue((float)FOG_START);
		val.Parameters["xFogEnd"].SetValue((float)FOG_END);
		val.Parameters["xFogEnable"].SetValue(true);
		val.Parameters["xAmbient"].SetValue(0.45f);
		val.Parameters["TextureMix"].SetValue(T_MIX);
		return val;
	}

	public void DrawFullscreenQuad(Texture2D texture, RenderTarget2D renderTarget, Effect effect)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		_graphics.GraphicsDevice.SetRenderTarget(0, renderTarget);
		DrawFullscreenQuad(texture, ((RenderTarget)renderTarget).Width, ((RenderTarget)renderTarget).Height, effect, Color.White);
		_graphics.GraphicsDevice.SetRenderTarget(0, (RenderTarget2D)null);
	}

	public void DrawFullscreenQuad(Texture2D texture, int width, int height, Effect effect, Color tint)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		DrawFullscreenQuad(texture, width, height, effect, tint, clearScreen: true, 0, (effect != null) ? (effect.CurrentTechnique.Passes.Count - 1) : 0);
	}

	public void DrawFullscreenQuad(Texture2D texture, int width, int height, Effect effect, Color tint, bool clearScreen, int startPass, int endPass)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		if (clearScreen)
		{
			_graphics.GraphicsDevice.Clear((ClearOptions)3, new Color(new Vector4(0f, 0f, 0f, 0f)), 1f, 0);
		}
		spriteBatch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)0);
		if (effect != null)
		{
			effect.Begin();
		}
		if (effect == null)
		{
			_spriteBatch.Draw(texture, new Rectangle(0, 0, width, height), (Rectangle?)null, tint, 0f, Vector2.Zero, (SpriteEffects)0, 1f);
		}
		else
		{
			for (int i = startPass; i <= endPass; i++)
			{
				effect.CurrentTechnique.Passes[i].Begin();
				_spriteBatch.Draw(texture, new Rectangle(0, 0, width, height), tint);
				effect.CurrentTechnique.Passes[i].End();
			}
		}
		if (effect != null)
		{
			effect.End();
		}
		_spriteBatch.End();
	}

	public void DrawFullscreenPingPong(RenderTarget2D sourceTarget, RenderTarget2D endTarget, Effect effect)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		DrawFullscreenPingPong(sourceTarget, endTarget, effect, Color.White, effect.CurrentTechnique.Passes.Count);
	}

	public void DrawFullscreenPingPong(RenderTarget2D sourceTarget, RenderTarget2D endTarget, Effect effect, Color tint, int passes)
	{
		for (int i = 0; i < passes; i++)
		{
			_graphics.GraphicsDevice.SetRenderTarget(0, (i % 2 == 0) ? endTarget : sourceTarget);
			_graphics.GraphicsDevice.Textures[0] = (Texture)(object)((i % 2 == 0) ? sourceTarget : endTarget).GetTexture();
			effect.CurrentTechnique.Passes[i].Begin();
			graphics.GraphicsDevice.DrawPrimitives((PrimitiveType)6, 0, 2);
			effect.CurrentTechnique.Passes[i].End();
		}
	}

	private void CalculateBlur(float dx, float dy, out float[] sampleWeights, out Vector2[] sampleOffsets)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		int count = _blurEffect.Parameters["SampleWeightsH"].Elements.Count;
		sampleWeights = new float[count];
		sampleOffsets = (Vector2[])(object)new Vector2[count];
		sampleWeights[0] = ComputeGaussian(0f);
		ref Vector2 reference = ref sampleOffsets[0];
		reference = new Vector2(0f);
		float num = sampleWeights[0];
		for (int i = 0; i < count / 2; i++)
		{
			float num2 = ComputeGaussian(2 * i + 1);
			float num3 = ComputeGaussian(2 * i + 2);
			sampleWeights[i * 2 + 1] = num2 + num3;
			sampleWeights[i * 2 + 2] = num2 + num3;
			num += (num2 + num3) * 2f;
			float num4 = (float)(i * 2 + 1) + num3 / (num2 + num3);
			Vector2 val = new Vector2(dx, dy) * num4;
			ref Vector2 reference2 = ref sampleOffsets[i * 2 + 1];
			reference2 = -val;
			sampleOffsets[i * 2 + 2] = val;
		}
		for (int j = 0; j < sampleWeights.Length; j++)
		{
			sampleWeights[j] /= num;
		}
	}

	private void SetBlurEffectParameters(ref float[] sampWeightsH, ref Vector2[] sampOffsetsH, ref float[] sampWeightsV, ref Vector2[] sampOffsetsV)
	{
		_combineEffect.Parameters["SampleWeightsH"].SetValue(sampWeightsH);
		_combineEffect.Parameters["SampleOffsetsH"].SetValue(sampOffsetsH);
		_combineEffect.Parameters["SampleWeightsV"].SetValue(sampWeightsV);
		_combineEffect.Parameters["SampleOffsetsV"].SetValue(sampOffsetsV);
	}

	private float ComputeGaussian(float n)
	{
		float num = 4f;
		return (float)(1.0 / Math.Sqrt(Math.PI * 2.0 * (double)num) * Math.Exp((0f - n * n) / (2f * num * num)));
	}

	public static void RunController(ModelOluAnimator animator, AnimationController controller)
	{
		foreach (BonePose bonePose in animator.BonePoses)
		{
			bonePose.CurrentController = controller;
			bonePose.CurrentBlendController = null;
		}
	}

	public static void RunController(ModelOluAnimator anim, AnimationController controller, AnimationController blendController, float blendFactor)
	{
		foreach (BonePose bonePose in anim.BonePoses)
		{
			bonePose.CurrentController = controller;
			bonePose.CurrentBlendController = blendController;
			bonePose.BlendFactor = blendFactor;
		}
	}

	public static Vector3 FaceUpward(PathList path)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = path.curDir();
		val.Y *= 0f;
		val.X *= 0f;
		if (val.Z == 0f)
		{
			val.Z = -1f;
		}
		return Vector3.Normalize(val);
	}

	public static Vector3 GetRandVect(Vector3 vel, float rand)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		Random random = Get().r;
		if (vel == Vector3.Zero)
		{
			return Vector3.Zero;
		}
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(0f, 0f, ((Vector3)(ref vel)).Length());
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector(1f, 0f, 0f);
		Vector3 val3 = Vector3.Cross(val2, vel);
		if (val3 == Vector3.Zero)
		{
			val3 = Vector3.Up;
		}
		val3 = Vector3.Normalize(val3);
		val2 = -Vector3.Normalize(Vector3.Cross(val3, vel));
		if (rand > 0f)
		{
			val = Vector3.Transform(val, Matrix.CreateRotationX(MathHelper.ToRadians(rand * (float)random.NextDouble())));
			val = Vector3.Transform(val, Matrix.CreateRotationZ(MathHelper.ToRadians(360f) * (float)random.NextDouble()));
		}
		return Vector3.Transform(val, MapObjectToSystem(Vector3.Zero, vel, val3));
	}

	public static Vector3 GetRandPos(Vector3 corner1, Vector3 corner2)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(corner2.X - corner1.X, 0f, corner2.Z - corner1.Z);
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector(0f, corner2.Y - corner1.Y, 0f);
		return corner1 + (float)Get().r.NextDouble() * val + (float)Get().r.NextDouble() * val2;
	}

	public static Vector3 GetRandPosSide(Vector3 corner1, Vector3 corner2)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(corner2.X - corner1.X, corner2.Y - corner1.Y, 0f);
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector(0f, 0f, corner2.Z - corner1.Z);
		return corner1 + (float)Get().r.NextDouble() * val + (float)Get().r.NextDouble() * val2;
	}

	public static Vector3 GetRandPosCube(Vector3 corner1, Vector3 corner2)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(corner2.X - corner1.X, 0f, 0f);
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector(0f, corner2.Y - corner1.Y, 0f);
		Vector3 val3 = default(Vector3);
		((Vector3)(ref val3))._002Ector(0f, 0f, corner2.Z - corner1.Z);
		return corner1 + (float)Get().r.NextDouble() * val + (float)Get().r.NextDouble() * val2 + (float)Get().r.NextDouble() * val3;
	}

	public static void SetAllEPCs(EffectParameterCollectionRedux[] epc, string attName, object value)
	{
		foreach (EffectParameterCollectionRedux effectParameterCollectionRedux in epc)
		{
			effectParameterCollectionRedux[attName] = value;
		}
	}

	public static int GetModelIndex(ModelWrapper mw, string modelName)
	{
		for (int i = 0; i < ((ReadOnlyCollection<ModelMesh>)(object)mw.model.Meshes).Count; i++)
		{
			if (((ReadOnlyCollection<ModelMesh>)(object)mw.model.Meshes)[i].ParentBone.Parent.Name == modelName)
			{
				return i;
			}
		}
		return -1;
	}

	public static Vector3 GetVertexPos(ref ModelWrapper model, int meshNum, int indexNum, ref Enemy enem)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return GetVertexPos(ref model, meshNum, indexNum, ref enem, Matrix.Identity);
	}

	public static Vector3 GetVertexPos(ref ModelWrapper model, int meshNum, int indexNum, ref Enemy enem, Matrix modMatrix)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		VertexNormalTex vertexNormalTex = model.vertices[meshNum][model.indices[meshNum][indexNum]];
		Matrix identity = Matrix.Identity;
		if (model.palette != null)
		{
			identity *= Matrix.CreateScale(0f);
			for (int i = 0; i < 4; i++)
			{
				identity += model.palette[meshNum][vertexNormalTex.boneNum(i)] * vertexNormalTex.weight(i);
			}
			identity.M44 = 1f;
		}
		else
		{
			identity = model.model.Root.Transform;
		}
		return Vector3.Transform(vertexNormalTex.position, identity * modMatrix * enem.Transformation());
	}

	public static Vector3 GetVertexNorm(ref ModelWrapper model, int meshNum, int indexNum, ref Enemy enem)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		VertexNormalTex vertexNormalTex = model.vertices[meshNum][model.indices[meshNum][indexNum]];
		Matrix val = Matrix.Identity;
		if (model.palette != null)
		{
			val *= Matrix.CreateScale(0f);
			for (int i = 0; i < 4; i++)
			{
				val += model.palette[meshNum][vertexNormalTex.boneNum(i)] * vertexNormalTex.weight(i);
			}
			val.M44 = 1f;
		}
		return Vector3.TransformNormal(vertexNormalTex.normal, val * enem.Transformation());
	}

	public static string WrapString(string text, float width, float scale, SpriteFont spFont)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		string text2 = "";
		string[] array = text.Split(' ');
		string text3 = array[0];
		for (int i = 1; i < array.Length; i++)
		{
			if (spFont.MeasureString(text3 + " " + array[i]).X * scale < width)
			{
				text3 = text3 + " " + array[i];
				continue;
			}
			text2 = text2 + text3 + "\n";
			text3 = array[i];
		}
		return text2 + text3;
	}

	public static string WrapStringNoLine(string text, float width, float scale, SpriteFont spFont)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		string text2 = text.Substring(text.LastIndexOf('\n') + 1);
		float num = spFont.MeasureString(text2).X * scale;
		if (num > width)
		{
			int num2 = (int)(width / num * (float)text2.Length) - 1;
			text = text.Insert(text.LastIndexOf('\n') + num2 + 1, "\n");
		}
		return text;
	}

	public void TrialModeSettings(bool isTrial)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		Color green = Color.Green;
		Color white = Color.White;
		Color normal = default(Color);
		((Color)(ref normal))._002Ector((byte)100, (byte)100, (byte)100);
		levelMenu = new Menu();
		levelMenu.Add("[- Tutorial -]", new Vector2((float)(WIDTH / 8), (float)(2 * HEIGHT / 8)), green, white, "Levels//LevelOne.xml");
		levelMenu.Add("[- Addicted -]", new Vector2((float)(WIDTH / 8), (float)(3 * HEIGHT / 8)), green, white, "Levels//LevelOne.xml 1");
		if (curUserData.levelsCleared > 0 && !isTrial)
		{
			levelMenu.Add("[- Breathless -]", new Vector2((float)(WIDTH / 8), (float)(4 * HEIGHT / 8)), green, white, "Levels//LevelTwo.xml");
		}
		else
		{
			levelMenu.AddDisabled("[- LOCKED -]", new Vector2((float)(WIDTH / 8), (float)(4 * HEIGHT / 8)), normal, white, "Levels//LevelTwo.xml");
		}
		if (curUserData.levelsCleared > 1 && !isTrial)
		{
			levelMenu.Add("[- Hall of Giants -]", new Vector2((float)(WIDTH / 8), (float)(5 * HEIGHT / 8)), green, white, "Levels//LevelThree.xml");
		}
		else
		{
			levelMenu.AddDisabled("[- LOCKED -]", new Vector2((float)(WIDTH / 8), (float)(5 * HEIGHT / 8)), normal, white, "Levels//LevelThree.xml");
		}
		if (curUserData.levelsCleared > 2 && !isTrial)
		{
			levelMenu.Add("[- Voodoo -]", new Vector2((float)(WIDTH / 8), (float)(6 * HEIGHT / 8)), green, white, "Levels//LevelFinal.xml");
		}
		else
		{
			levelMenu.AddDisabled("[- LOCKED -]", new Vector2((float)(WIDTH / 8), (float)(6 * HEIGHT / 8)), normal, white, "Levels//LevelFinal.xml");
		}
		levelMenu.Add("[- Exit -]", new Vector2((float)(WIDTH / 8), (float)(7 * HEIGHT / 8)), green, white, "exit");
		optionMenu = new Menu();
		optionMenu.Add("[- Invert Y Axis -]", new Vector2((float)(WIDTH / 8), (float)(3 * HEIGHT / 8)), green, white, "invert", OnOptionSwitchedHandler, "On", "Off");
		((MenuItemOption)optionMenu.items[0]).SetChoice(Get().invert ? "On" : "Off");
		optionMenu.Add("[- Rumble -]", new Vector2((float)(WIDTH / 8), (float)(4 * HEIGHT / 8)), green, white, "rumble", OnOptionSwitchedHandler, "On", "Off");
		((MenuItemOption)optionMenu.items[1]).SetChoice(Get().rumble ? "On" : "Off");
		optionMenu.Add("[- Exit -]", new Vector2((float)(WIDTH / 8), (float)(5 * HEIGHT / 8)), green, white, "exit");
	}

	public void CheckAndResetRumble()
	{
		_ = Get().channels;
		if (channels[28] > 0f)
		{
			channels[28] = 0f;
		}
		if (channels[29] > 0f)
		{
			channels[29] = 0f;
		}
	}

	public static void OnOptionSwitchedHandler(string command, string option)
	{
		switch (command)
		{
		case "invert":
			Get().invert = option == "On";
			break;
		case "rumble":
			Get().rumble = option == "On";
			break;
		}
		Get().PlayCue("clap_2");
	}

	static BaseGame()
	{
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		WIDTH = 1280;
		HEIGHT = 720;
		fontName = "Content/HUDfontLarge";
		bigFontName = "Content/BigHUDfont";
		GAP = 0;
		S_WIDTH = WIDTH - 2 * GAP;
		S_HEIGHT = HEIGHT - 2 * GAP;
		C_WIDTH = (int)(0.083f * (float)HEIGHT);
		FOG_START = 100;
		FOG_END = 150;
		CHANNEL_NUM = 32;
		BEAT = 0.09095f;
		frameRat = 0.017f;
		gravFactor = 98f;
		FREEZE_TIME = 5f;
		demo = false;
		quickload = false;
		credits = false;
		release = true;
		PROFILE = false;
		bloom_on = true;
		T_MIX = new Vector4(1f, 1f, 0f, 2f);
		T_ADD = new Vector4(1f, 1f, 0f, 1f);
		T_TEX = new Vector4(0f, 1f, 0f, 1f);
		T_MUL = new Vector4(0f, 0f, 1f, 1f);
	}
}
