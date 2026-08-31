using System.Collections.Generic;
using F;
using Microsoft.Xna.Framework.Graphics;
using Z;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Provides automatic creation, storage, and management of shared
/// buffers (render targets) used during rendering.
///
/// These buffers include g-buffers, lighting-buffers, and post
/// processing buffers.
/// </summary>
public class FrameBuffers : IUnloadable
{
	private int HCB;

	private int HC_0002;

	private bool HC_0012;

	private DetailPreference HCH;

	private DetailPreference HC7;

	private IGraphicsDeviceService HC_0001;

	private Z._0001 HCw;

	private SurfaceFormat[] HCZ = new SurfaceFormat[6];

	private Dictionary<int, RenderTarget2D> HC_000F = new Dictionary<int, RenderTarget2D>(8);

	private FullFrameQuad HCy;

	private Dictionary<string, CustomFrameBufferCollection> HC6 = new Dictionary<string, CustomFrameBufferCollection>(8);

	/// <summary>
	/// Current width of the frame buffers.
	/// </summary>
	public int Width => HCB;

	/// <summary>
	/// Current height of the frame buffers.
	/// </summary>
	public int Height => HC_0002;

	/// <summary>
	/// Increases visual quality at the cost of performance.
	/// Generally used in visualizations, most games do not need this option.
	/// </summary>
	public DetailPreference PrecisionMode => HCH;

	/// <summary>
	/// Increases lighting quality at the cost of performance.
	/// Adds additional lighting range when using HDR.
	/// </summary>
	public DetailPreference LightingRange => HC7;

	/// <summary>
	/// Provides a full frame renderable quad sized specifically for the contained buffers.
	/// </summary>
	public FullFrameQuad FullFrameQuad
	{
		get
		{
			if (HCy == null)
			{
				HCy = new FullFrameQuad(HC_0001.GraphicsDevice, HCB, HC_0002);
			}
			return HCy;
		}
	}

	/// <summary>
	/// Creates a new FrameBuffers instance.
	/// </summary>
	/// <param name="customwidth">Custom buffer width.</param>
	/// <param name="customheight">Custom buffer height.</param>
	/// <param name="precisionmode">Increases visual quality at the cost of performance.
	/// Generally used in visualizations, most games do not need this option.</param>
	/// <param name="lightingrange">Increases lighting quality at the cost of performance.
	/// Adds additional lighting range when using HDR.</param>
	public FrameBuffers(int customwidth, int customheight, DetailPreference precisionmode, DetailPreference lightingrange)
	{
		HC_0001 = SunBurnCoreSystem.Instance.GraphicsDeviceManager;
		HC_0012 = true;
		HCB = customwidth;
		HC_0002 = customheight;
		HCH = precisionmode;
		HC7 = lightingrange;
		B();
	}

	/// <summary>
	/// Creates a new FrameBuffers instance.
	/// </summary>
	/// <param name="precisionmode">Increases visual quality at the cost of performance.
	/// Generally used in visualizations, most games do not need this option.</param>
	/// <param name="lightingrange">Increases lighting quality at the cost of performance.
	/// Adds additional lighting range when using HDR.</param>
	public FrameBuffers(DetailPreference precisionmode, DetailPreference lightingrange)
	{
		HC_0001 = SunBurnCoreSystem.Instance.GraphicsDeviceManager;
		HC_0012 = false;
		HCH = precisionmode;
		HC7 = lightingrange;
		B();
	}

	private void B()
	{
		if (HCH == DetailPreference.High)
		{
			HCZ[0] = SurfaceFormat.Vector2;
			HCZ[1] = SurfaceFormat.HalfVector4;
		}
		else
		{
			HCZ[0] = SurfaceFormat.HalfVector2;
			HCZ[1] = SurfaceFormat.Color;
		}
		if (HC7 == DetailPreference.High)
		{
			HCZ[2] = SurfaceFormat.HdrBlendable;
			HCZ[3] = SurfaceFormat.HdrBlendable;
			HCZ[4] = SurfaceFormat.HdrBlendable;
			HCZ[5] = SurfaceFormat.HdrBlendable;
		}
		else
		{
			HCZ[2] = SurfaceFormat.Rgba1010102;
			HCZ[3] = SurfaceFormat.Rgba1010102;
			HCZ[4] = SurfaceFormat.Color;
			HCZ[5] = SurfaceFormat.Color;
		}
		HCw = new Z._0001();
	}

	/// <summary>
	/// Gets one of the common frame buffers (only valid between
	/// calls to BeginFrameRendering and EndFrameRendering).
	/// </summary>
	/// <param name="buffertype"></param>
	/// <param name="createmissing">Determines if the buffer should be created
	/// when it does not exist, otherwise null is returned.</param>
	/// <returns></returns>
	public RenderTarget2D GetBuffer(FrameBufferType buffertype, bool createmissing)
	{
		if (HC_000F.TryGetValue((int)buffertype, out var value))
		{
			return value;
		}
		if (!createmissing)
		{
			return null;
		}
		GraphicsDevice graphicsDevice = HC_0001.GraphicsDevice;
		DepthFormat preferredDepthFormat = DepthFormat.None;
		if (buffertype == FrameBufferType.DeferredDepthAndSpecularPower || buffertype == FrameBufferType.DeferredLightingDiffuse)
		{
			preferredDepthFormat = DepthFormat.Depth24Stencil8;
		}
		bool mipMap = false;
		int preferredMultiSampleCount = 0;
		if (buffertype == FrameBufferType.PostProcessing1 || buffertype == FrameBufferType.PostProcessing2)
		{
			preferredDepthFormat = DepthFormat.Depth24Stencil8;
			preferredMultiSampleCount = ((graphicsDevice.PresentationParameters.MultiSampleCount > 0) ? 2 : 0);
		}
		value = new RenderTarget2D(graphicsDevice, HCB, HC_0002, mipMap, HCZ[(int)buffertype], preferredDepthFormat, preferredMultiSampleCount, RenderTargetUsage.PlatformContents);
		HC_000F.Add((int)buffertype, value);
		return value;
	}

	/// <summary>
	/// Gets a collection of implementation specific buffers defined and used by the caller.
	/// </summary>
	/// <param name="name">Unique name of the collection to find.</param>
	/// <param name="createmissing">Determines if the collection should be created if it does not exist.</param>
	/// <returns></returns>
	public CustomFrameBufferCollection GetCustomFrameBufferCollection(string name, bool createmissing)
	{
		if (HC6.TryGetValue(name, out var value))
		{
			return value;
		}
		if (!createmissing)
		{
			return null;
		}
		value = new CustomFrameBufferCollection();
		HC6.Add(name, value);
		return value;
	}

	/// <summary>
	/// Sets up the object prior to rendering.
	/// </summary>
	/// <param name="scenestate"></param>
	public void BeginFrameRendering(ISceneState scenestate)
	{
		if (HCw.Changed)
		{
			Unload();
		}
		if (!HC_0012)
		{
			PresentationParameters presentationParameters = HC_0001.GraphicsDevice.PresentationParameters;
			if (HCB != presentationParameters.BackBufferWidth || HC_0002 != presentationParameters.BackBufferHeight)
			{
				HCB = presentationParameters.BackBufferWidth;
				HC_0002 = presentationParameters.BackBufferHeight;
				Unload();
			}
		}
	}

	/// <summary>
	/// Finalizes rendering.
	/// </summary>
	public void EndFrameRendering()
	{
	}

	/// <summary>
	/// Disposes any graphics resource used internally by this object, and removes
	/// scene resources managed by this object. Commonly used during Game.UnloadContent.
	/// </summary>
	public void Unload()
	{
		foreach (KeyValuePair<int, RenderTarget2D> item in HC_000F)
		{
			item.Value.Dispose();
		}
		HC_000F.Clear();
		foreach (KeyValuePair<string, CustomFrameBufferCollection> item2 in HC6)
		{
			foreach (RenderTarget2D item3 in item2.Value)
			{
				item3.Dispose();
			}
		}
		HC6.Clear();
		F.B._7_0004(ref HCy);
	}
}
