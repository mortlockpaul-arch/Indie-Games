using System;
using System.Diagnostics;
using System.Resources;
using _0003;
using B;
using F;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using SynapseGaming.LightingSystem.Effects;
using Z;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Provides and manages engine specific resources
/// such as lighting textures, effects, and helper models.
/// At least one instance must be created before interacting
/// with SunBurn.
/// </summary>
public class SunBurnCoreSystem
{
	private class _0001CB
	{
		private IServiceProvider HCB;

		private ResourceContentManager HC_0002;

		public ResourceContentManager EmbeddedResourceManager
		{
			get
			{
				if (HC_0002 == null)
				{
					HC_0002 = new ResourceContentManager(HCB, ResourceManager);
				}
				return HC_0002;
			}
		}

		public _0001CB(IServiceProvider serviceprovider)
		{
			HCB = serviceprovider;
		}

		public void Unload()
		{
			if (HC_0002 != null)
			{
				HC_0002.Dispose();
				HC_0002 = null;
			}
		}
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static int HCB = 10000;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static SunBurnCoreSystem HC_0002;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Texture2D HC_0012;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Texture2D HCH;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private bool HC7 = true;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private TextureCube HC_0001;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private TextureCube HCw;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Texture2D HCZ;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private SpriteFont HC_000F;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private SpriteBatch HCy;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private GraphicsDeviceSupport HC6;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private IServiceProvider HCD;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private IGraphicsDeviceService HC_0011;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private _0001CB HCK;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private ResourceManager HC_0003;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private Z._000F HCk;

	/// <summary>
	/// Provides global access to the game's SunBurnCoreSystem. An instance of this class
	/// must be created before calling the property.
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[DebuggerHidden]
	public static SunBurnCoreSystem Instance
	{
		[DebuggerHidden]
		get
		{
			if (HC_0002 == null)
			{
				throw new ArgumentException("SunBurnCoreSystem unavailable, please create an instance of the manager before using this object.");
			}
			global::B.B._0012();
			return HC_0002;
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
	[DebuggerHidden]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
			return _0003._7.ResourceManager;
		}
	}

	[DebuggerHidden]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	internal static int MaxLightsPerGroup
	{
		get
		{
			return HCB;
		}
		set
		{
			HCB = hCB;
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
			return HC7;
		}
		[DebuggerHidden]
		set
		{
			HC7 = value;
		}
	}

	/// <summary>
	/// Provides a default resource manager for use without access to a scene interface.
	/// </summary>
	[DebuggerHidden]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public ResourceManager DefaultResourceManager => HC_0003;

	/// <summary>
	/// Provides access to the game's XNA services.
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[DebuggerHidden]
	public IServiceProvider Services => HCD;

	/// <summary>
	/// The current GraphicsDeviceManager used by the game.
	/// </summary>
	[DebuggerHidden]
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public IGraphicsDeviceService GraphicsDeviceManager => HC_0011;

	/// <summary>
	/// Creates a new SunBurnCoreSystem instance.
	/// </summary>
	/// <param name="service"></param>
	/// <param name="managerwithactivationfile">Content manager that contains the SunBurn activation file.</param>
	[DebuggerHidden]
	public SunBurnCoreSystem(IServiceProvider service, ContentManager managerwithactivationfile)
	{
		B._0012.ActivationPath = managerwithactivationfile.RootDirectory;
		HC_0002 = this;
		HCD = service;
		HC_0011 = service.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;
		HCK = new _0001CB(service);
		HC_0003 = new ResourceManager(null);
		HCk = new Z._000F();
		HCk._0002z();
		global::B.B._0012();
	}

	internal void _0002_0019(FrameBuffers P_0)
	{
		if (HC7)
		{
			GraphicsDevice graphicsDevice = HC_0011.GraphicsDevice;
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
		HCk._0002A<T>(false);
	}

	internal void _0002p(IManagerServiceProvider P_0, bool P_1)
	{
		HCk._0002c(P_0, P_1);
	}

	/// <summary>
	/// Gets the system's prefered render target usage for the current platform.
	/// </summary>
	/// <returns></returns>
	public RenderTargetUsage GetBestRenderTargetUsage()
	{
		return RenderTargetUsage.PlatformContents;
	}

	internal EffectData _00021(string P_0)
	{
		return HCK.EmbeddedResourceManager.Load<EffectData>(P_0);
	}

	internal Model _0002b(string P_0)
	{
		return HCK.EmbeddedResourceManager.Load<Model>(P_0);
	}

	internal Texture2D _0002l(string P_0)
	{
		return HCK.EmbeddedResourceManager.Load<Texture2D>(P_0);
	}

	internal Texture2D _0002g()
	{
		if (HCH == null)
		{
			_ = HC_0011.GraphicsDevice;
			HCH = _0002l("SplashScreen");
		}
		return HCH;
	}

	internal TextureCube _0002W()
	{
		if (HC_0001 == null)
		{
			GraphicsDevice graphicsDevice = HC_0011.GraphicsDevice;
			HC_0001 = new TextureCube(graphicsDevice, 1, mipMap: false, SurfaceFormat.Color);
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
				HC_0001.SetData((CubeMapFace)i, array);
			}
		}
		return HC_0001;
	}

	internal TextureCube _0002_0018()
	{
		if (HCw == null)
		{
			GraphicsDevice graphicsDevice = HC_0011.GraphicsDevice;
			HCw = new TextureCube(graphicsDevice, 256, mipMap: false, SurfaceFormat.Color);
			Color[] array = new Color[HCw.Size * HCw.Size];
			int size = HCw.Size;
			float num = 1f / (float)(HCw.Size - 1);
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
				HCw.SetData((CubeMapFace)i, array);
			}
		}
		return HCw;
	}

	internal Texture2D _0002_000E()
	{
		if (HCZ == null)
		{
			GraphicsDevice graphicsDevice = HC_0011.GraphicsDevice;
			HCZ = new Texture2D(graphicsDevice, 16, 16, mipMap: false, SurfaceFormat.HalfSingle);
			HalfSingle[] data = new HalfSingle[HCZ.Width * HCZ.Height];
			HCZ.SetData(data);
		}
		return HCZ;
	}

	internal Texture2D _0002d()
	{
		if (HC_0012 == null)
		{
			GraphicsDevice graphicsDevice = HC_0011.GraphicsDevice;
			HC_0012 = new Texture2D(graphicsDevice, 64, 256, mipMap: true, SurfaceFormat.Color);
			int num = HC_0012.Width;
			int num2 = HC_0012.Height;
			for (int i = 0; i < HC_0012.LevelCount; i++)
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
				HC_0012.SetData(i, null, array, 0, array.Length);
				num = Math.Max(num / 2, 1);
				num2 = Math.Max(num2 / 2, 1);
			}
		}
		return HC_0012;
	}

	internal SpriteFont _0002n()
	{
		if (HC_000F != null)
		{
			return HC_000F;
		}
		HC_000F = HCK.EmbeddedResourceManager.Load<SpriteFont>("ConsoleFont");
		HC_000F.DefaultCharacter = '_';
		return HC_000F;
	}

	internal SpriteBatch _00025()
	{
		if (HCy != null)
		{
			return HCy;
		}
		GraphicsDevice graphicsDevice = HC_0011.GraphicsDevice;
		HCy = new SpriteBatch(graphicsDevice);
		return HCy;
	}

	/// <summary>
	/// Returns information on the currently configured and supported graphic device features.
	/// </summary>
	/// <returns></returns>
	public GraphicsDeviceSupport GetGraphicsDeviceSupport()
	{
		if (HC6 == null)
		{
			GraphicsDevice graphicsDevice = HC_0011.GraphicsDevice;
			HC6 = new GraphicsDeviceSupport(graphicsDevice);
		}
		return HC6;
	}

	/// <summary>
	/// Unloads all lighting system and device specific data.  Must be called
	/// when the device is reset (during Game.UnloadGraphicsContent()).
	/// </summary>
	public void Unload()
	{
		HCK.Unload();
		HC_0003.Unload();
		HCk.u();
		HC_000F = null;
		HC6 = null;
		F.B._7_0004(ref HCy);
		F.B._7_0004(ref HC_0012);
		F.B._7_0004(ref HCH);
		F.B._7_0004(ref HC_0001);
		F.B._7_0004(ref HCw);
		F.B._7_0004(ref HCZ);
	}
}
