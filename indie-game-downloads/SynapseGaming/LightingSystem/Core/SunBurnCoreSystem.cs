using System;
using System.Diagnostics;
using System.Resources;
using _0003;
using _0018;
using _8;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using SynapseGaming.LightingSystem.Effects;
using p;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Provides and manages engine specific resources
/// such as lighting textures, effects, and helper models.
/// At least one instance must be created before interacting
/// with SunBurn.
/// </summary>
public class SunBurnCoreSystem
{
	private class DA_0018
	{
		private IServiceProvider _3A_0018;

		private ResourceContentManager _3AL;

		public ResourceContentManager EmbeddedResourceManager
		{
			get
			{
				if (_3AL == null)
				{
					_3AL = new ResourceContentManager(_3A_0018, ResourceManager);
				}
				return _3AL;
			}
		}

		public DA_0018(IServiceProvider serviceprovider)
		{
			_3A_0018 = serviceprovider;
		}

		public void Unload()
		{
			if (_3AL != null)
			{
				_3AL.Dispose();
				_3AL = null;
			}
		}
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static int _3A_0018 = 10000;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static SunBurnCoreSystem _3AL;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Texture2D _3A_0019;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Texture2D _3A3;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool _3A6 = true;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private TextureCube _3AD;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private TextureCube _3A_0017;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Texture2D _3A_0003;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private SpriteFont _3Al;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private SpriteBatch _3At;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private GraphicsDeviceSupport _3AF;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private IServiceProvider _3Ac;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private IGraphicsDeviceService _3Ag;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private DA_0018 _3AI;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private ResourceManager _3A8;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private _0003.l _3AZ;

	/// <summary>
	/// Provides global access to the game's SunBurnCoreSystem. An instance of this class
	/// must be created before calling the property.
	/// </summary>
	[DebuggerHidden]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static SunBurnCoreSystem Instance
	{
		[DebuggerHidden]
		get
		{
			if (_3AL == null)
			{
				throw new ArgumentException("SunBurnCoreSystem unavailable, please create an instance of the manager before using this object.");
			}
			global::_0018._0018._0019();
			return _3AL;
		}
	}

	/// <summary>
	/// Returns the edition of the loaded SunBurn assembly.
	/// </summary>
	[DebuggerHidden]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static string Edition
	{
		[DebuggerHidden]
		get
		{
			return "Indie";
		}
	}

	/// <summary>
	/// Returns the public key token of the loaded SunBurn assembly.
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[DebuggerHidden]
	public static string PublicKeyToken
	{
		[DebuggerHidden]
		get
		{
			return "eb76e51de43fcd70";
		}
	}

	/// <summary>
	/// Returns the version of the loaded SunBurn assembly.
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[DebuggerHidden]
	public static string Version => "2.0.18.7";

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[DebuggerHidden]
	private static System.Resources.ResourceManager ResourceManager
	{
		[DebuggerHidden]
		get
		{
			return _8._6.ResourceManager;
		}
	}

	[DebuggerHidden]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	internal static int MaxLightsPerGroup
	{
		get
		{
			return _3A_0018;
		}
		set
		{
			_3A_0018 = num;
		}
	}

	/// <summary>
	/// Determines if SunBurn should throw an exception when the frame buffers exceed the
	/// viewport size. This helps detect performance issues due to mismatched buffer sizes.
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[DebuggerHidden]
	public bool DetectOverSizedFrameBuffers
	{
		[DebuggerHidden]
		get
		{
			return _3A6;
		}
		[DebuggerHidden]
		set
		{
			_3A6 = value;
		}
	}

	/// <summary>
	/// Provides a default resource manager for use without access to a scene interface.
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[DebuggerHidden]
	public ResourceManager DefaultResourceManager => _3A8;

	/// <summary>
	/// Provides access to the game's XNA services.
	/// </summary>
	[DebuggerHidden]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public IServiceProvider Services => _3Ac;

	/// <summary>
	/// The current GraphicsDeviceManager used by the game.
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[DebuggerHidden]
	public IGraphicsDeviceService GraphicsDeviceManager => _3Ag;

	/// <summary>
	/// Creates a new SunBurnCoreSystem instance.
	/// </summary>
	/// <param name="service"></param>
	/// <param name="managerwithactivationfile">Content manager that contains the SunBurn activation file.</param>
	[DebuggerHidden]
	public SunBurnCoreSystem(IServiceProvider service, ContentManager managerwithactivationfile)
	{
		_0018._0019.ActivationPath = managerwithactivationfile.RootDirectory;
		_3AL = this;
		_3Ac = service;
		_3Ag = service.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;
		_3AI = new DA_0018(service);
		_3A8 = new ResourceManager(null);
		_3AZ = new _0003.l();
		_3AZ.LT();
		global::_0018._0018._0019();
	}

	internal void LH(FrameBuffers P_0)
	{
		if (_3A6)
		{
			GraphicsDevice graphicsDevice = _3Ag.GraphicsDevice;
			Viewport viewport = graphicsDevice.Viewport;
			if (viewport.Width < P_0.Width || viewport.Height < P_0.Height)
			{
				throw new Exception("Supplied frame buffers are too large for final target viewport, this will cause performance issues. Supply properly sized buffers or disable the SunBurnCoreSystem's DetectOverSizedFrameBuffers property to ignore.");
			}
		}
	}

	/// <summary>
	/// Manually loads a SunBurn plugin class and ties the plugin
	/// into SunBurn's scene interface for initialization and unload
	/// scheme for resource cleanup.
	/// </summary>
	/// <typeparam name="T">Plugin class that implements the IPlugin interface.</typeparam>
	public void ManuallyLoadPlugin<T>() where T : IPlugin
	{
		_3AZ.Ly<T>(false);
	}

	public void InitializeSceneInterfaceWithLoadedPlugins(IManagerServiceProvider sceneinterface, bool includeautoloaded)
	{
		_3AZ.L_0015(sceneinterface, includeautoloaded);
	}

	/// <summary>
	/// Gets the system's prefered render target usage for the current platform.
	/// </summary>
	/// <returns></returns>
	public RenderTargetUsage GetBestRenderTargetUsage()
	{
		return RenderTargetUsage.PlatformContents;
	}

	internal EffectData LM(string P_0)
	{
		return _3AI.EmbeddedResourceManager.Load<EffectData>(P_0);
	}

	internal Model LP(string P_0)
	{
		return _3AI.EmbeddedResourceManager.Load<Model>(P_0);
	}

	internal Texture2D Lw(string P_0)
	{
		return _3AI.EmbeddedResourceManager.Load<Texture2D>(P_0);
	}

	internal Texture2D Le()
	{
		if (_3A3 == null)
		{
			_ = _3Ag.GraphicsDevice;
			_3A3 = Lw("SplashScreen");
		}
		return _3A3;
	}

	internal TextureCube LO()
	{
		if (_3AD == null)
		{
			GraphicsDevice graphicsDevice = _3Ag.GraphicsDevice;
			_3AD = new TextureCube(graphicsDevice, 1, mipMap: false, SurfaceFormat.Color);
			Color[] array = new Color[1];
			for (int i = 0; i < 6; i++)
			{
				switch (i)
				{
				case 0:
				{
					ref Color reference6 = ref array[0];
					reference6 = new Color(255, 0, 0, 255);
					break;
				}
				case 1:
				{
					ref Color reference5 = ref array[0];
					reference5 = new Color(255, 0, 0, 0);
					break;
				}
				case 2:
				{
					ref Color reference4 = ref array[0];
					reference4 = new Color(0, 255, 0, 255);
					break;
				}
				case 3:
				{
					ref Color reference3 = ref array[0];
					reference3 = new Color(0, 255, 0, 0);
					break;
				}
				case 4:
				{
					ref Color reference2 = ref array[0];
					reference2 = new Color(0, 0, 255, 255);
					break;
				}
				case 5:
				{
					ref Color reference = ref array[0];
					reference = new Color(0, 0, 255, 0);
					break;
				}
				}
				_3AD.SetData((CubeMapFace)i, array);
			}
		}
		return _3AD;
	}

	internal TextureCube LN()
	{
		if (_3A_0017 == null)
		{
			GraphicsDevice graphicsDevice = _3Ag.GraphicsDevice;
			_3A_0017 = new TextureCube(graphicsDevice, 256, mipMap: false, SurfaceFormat.Color);
			Color[] array = new Color[_3A_0017.Size * _3A_0017.Size];
			int size = _3A_0017.Size;
			float num = 1f / (float)(_3A_0017.Size - 1);
			Vector3[] array2 = new Vector3[6]
			{
				new Vector3(1f, 0f, 0f),
				new Vector3(-1f, 0f, 0f),
				new Vector3(0f, 1f, 0f),
				new Vector3(0f, -1f, 0f),
				new Vector3(0f, 0f, 1f),
				new Vector3(0f, 0f, -1f)
			};
			Vector3[] array3 = new Vector3[6]
			{
				new Vector3(0f, 0f, -1f),
				new Vector3(0f, 0f, 1f),
				new Vector3(1f, 0f, 0f),
				new Vector3(1f, 0f, 0f),
				new Vector3(1f, 0f, 0f),
				new Vector3(-1f, 0f, 0f)
			};
			Vector3[] array4 = new Vector3[6]
			{
				new Vector3(0f, -1f, 0f),
				new Vector3(0f, -1f, 0f),
				new Vector3(0f, 0f, 1f),
				new Vector3(0f, 0f, -1f),
				new Vector3(0f, -1f, 0f),
				new Vector3(0f, -1f, 0f)
			};
			for (int i = 0; i < 6; i++)
			{
				for (int j = 0; j < size; j++)
				{
					for (int k = 0; k < size; k++)
					{
						Vector3 vector = array2[i] + ((float)k * num * 2f - 1f) * array3[i] + ((float)j * num * 2f - 1f) * array4[i];
						vector.Normalize();
						float num2;
						float num3;
						float num4;
						switch (i)
						{
						case 0:
							num2 = 0f - vector.X;
							num3 = vector.Z;
							num4 = vector.Y;
							break;
						case 1:
							num2 = vector.X;
							num3 = 0f - vector.Z;
							num4 = vector.Y;
							break;
						case 2:
							num2 = 0f - vector.Y;
							num3 = vector.X;
							num4 = vector.Z;
							break;
						case 3:
							num2 = vector.Y;
							num3 = 0f - vector.X;
							num4 = vector.Z;
							break;
						case 4:
							num2 = vector.Z;
							num3 = vector.X;
							num4 = 0f - vector.Y;
							break;
						default:
							num2 = vector.Z;
							num3 = vector.X;
							num4 = vector.Y;
							break;
						}
						vector.X = MathHelper.Clamp(num3 / num2 * 0.5f + 0.5f, 0f, 1f);
						vector.Y = MathHelper.Clamp(num4 / num2 * 0.5f + 0.5f, 0f, 1f);
						ref Color reference = ref array[j * size + k];
						reference = new Color((byte)(vector.X * 255f + 0.5f), (byte)(vector.Y * 255f + 0.5f), (byte)(vector.Z * 255f + 0.5f), 0);
					}
				}
				_3A_0017.SetData((CubeMapFace)i, array);
			}
		}
		return _3A_0017;
	}

	internal Texture2D Lm()
	{
		if (_3A_0003 == null)
		{
			GraphicsDevice graphicsDevice = _3Ag.GraphicsDevice;
			_3A_0003 = new Texture2D(graphicsDevice, 16, 16, mipMap: false, SurfaceFormat.HalfSingle);
			HalfSingle[] data = new HalfSingle[_3A_0003.Width * _3A_0003.Height];
			_3A_0003.SetData(data);
		}
		return _3A_0003;
	}

	internal Texture2D LE()
	{
		if (_3A_0019 == null)
		{
			GraphicsDevice graphicsDevice = _3Ag.GraphicsDevice;
			_3A_0019 = new Texture2D(graphicsDevice, 64, 256, mipMap: true, SurfaceFormat.Color);
			int num = _3A_0019.Width;
			int num2 = _3A_0019.Height;
			for (int i = 0; i < _3A_0019.LevelCount; i++)
			{
				Color[] array = new Color[num * num2];
				float num3 = 1f / (float)(num - 1);
				float num4 = 1f / (float)(num2 - 1);
				int num5 = 0;
				for (int j = 0; j < num2; j++)
				{
					for (int k = 0; k < num; k++)
					{
						float num6 = (float)k * num3;
						float num7 = (float)j * num4;
						float num8 = num6 * num6;
						float num9 = num7 * num7;
						float num10 = (float)Math.PI * 4f * num8 * num9 * num9;
						if (num10 == 0f)
						{
							num10 = 1E-07f;
						}
						num10 = 1f / num10;
						float num11 = num8 * num9;
						if (num11 == 0f)
						{
							num11 = 1E-07f;
						}
						num11 = (num9 - 1f) / num11;
						byte b = (byte)(MathHelper.Clamp(num10 * (float)Math.Exp(num11), 0f, 1f) * 255f);
						ref Color reference = ref array[num5++];
						reference = new Color(b, b, b);
					}
				}
				_3A_0019.SetData(i, null, array, 0, array.Length);
				num = Math.Max(num / 2, 1);
				num2 = Math.Max(num2 / 2, 1);
			}
		}
		return _3A_0019;
	}

	internal SpriteFont LS()
	{
		if (_3Al != null)
		{
			return _3Al;
		}
		_3Al = _3AI.EmbeddedResourceManager.Load<SpriteFont>("ConsoleFont");
		_3Al.DefaultCharacter = '_';
		return _3Al;
	}

	internal SpriteBatch LQ()
	{
		if (_3At != null)
		{
			return _3At;
		}
		GraphicsDevice graphicsDevice = _3Ag.GraphicsDevice;
		_3At = new SpriteBatch(graphicsDevice);
		return _3At;
	}

	/// <summary>
	/// Returns information on the currently configured and supported graphic device features.
	/// </summary>
	/// <returns></returns>
	public GraphicsDeviceSupport GetGraphicsDeviceSupport()
	{
		if (_3AF == null)
		{
			GraphicsDevice graphicsDevice = _3Ag.GraphicsDevice;
			_3AF = new GraphicsDeviceSupport(graphicsDevice);
		}
		return _3AF;
	}

	/// <summary>
	/// Unloads all lighting system and device specific data.  Must be called
	/// when the device is reset (during Game.UnloadGraphicsContent()).
	/// </summary>
	public void Unload()
	{
		_3AI.Unload();
		_3A8.Unload();
		_3AZ.X();
		_3Al = null;
		_3AF = null;
		p._0018._6_0006(ref _3At);
		p._0018._6_0006(ref _3A_0019);
		p._0018._6_0006(ref _3A3);
		p._0018._6_0006(ref _3AD);
		p._0018._6_0006(ref _3A_0017);
		p._0018._6_0006(ref _3A_0003);
	}
}
