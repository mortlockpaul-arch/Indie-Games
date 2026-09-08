using System.Collections.Generic;
using _0003;
using Microsoft.Xna.Framework.Graphics;
using p;

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
	private int _3A_0018;

	private int _3AL;

	private bool _3A_0019;

	private DetailPreference _3A3;

	private DetailPreference _3A6;

	private IGraphicsDeviceService _3AD;

	private _0003.D _3A_0017;

	private SurfaceFormat[] _3A_0003 = new SurfaceFormat[6];

	private Dictionary<int, RenderTarget2D> _3Al = new Dictionary<int, RenderTarget2D>(8);

	private FullFrameQuad _3At;

	private Dictionary<string, CustomFrameBufferCollection> _3AF = new Dictionary<string, CustomFrameBufferCollection>(8);

	/// <summary>
	/// Current width of the frame buffers.
	/// </summary>
	public int Width => _3A_0018;

	/// <summary>
	/// Current height of the frame buffers.
	/// </summary>
	public int Height => _3AL;

	/// <summary>
	/// Increases visual quality at the cost of performance.
	/// Generally used in visualizations, most games do not need this option.
	/// </summary>
	public DetailPreference PrecisionMode => _3A3;

	/// <summary>
	/// Increases lighting quality at the cost of performance.
	/// Adds additional lighting range when using HDR.
	/// </summary>
	public DetailPreference LightingRange => _3A6;

	/// <summary>
	/// Provides a full frame renderable quad sized specifically for the contained buffers.
	/// </summary>
	public FullFrameQuad FullFrameQuad
	{
		get
		{
			if (_3At == null)
			{
				_3At = new FullFrameQuad(_3AD.GraphicsDevice, _3A_0018, _3AL);
			}
			return _3At;
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
		_3AD = SunBurnCoreSystem.Instance.GraphicsDeviceManager;
		_3A_0019 = true;
		_3A_0018 = customwidth;
		_3AL = customheight;
		_3A3 = precisionmode;
		_3A6 = lightingrange;
		_0018();
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
		_3AD = SunBurnCoreSystem.Instance.GraphicsDeviceManager;
		_3A_0019 = false;
		_3A3 = precisionmode;
		_3A6 = lightingrange;
		_0018();
	}

	private void _0018()
	{
		if (_3A3 == DetailPreference.High)
		{
			_3A_0003[0] = SurfaceFormat.Vector2;
			_3A_0003[1] = SurfaceFormat.HalfVector4;
		}
		else
		{
			_3A_0003[0] = SurfaceFormat.HalfVector2;
			_3A_0003[1] = SurfaceFormat.Color;
		}
		if (_3A6 == DetailPreference.High)
		{
			_3A_0003[2] = SurfaceFormat.HdrBlendable;
			_3A_0003[3] = SurfaceFormat.HdrBlendable;
			_3A_0003[4] = SurfaceFormat.HdrBlendable;
			_3A_0003[5] = SurfaceFormat.HdrBlendable;
		}
		else
		{
			_3A_0003[2] = SurfaceFormat.Rgba1010102;
			_3A_0003[3] = SurfaceFormat.Rgba1010102;
			_3A_0003[4] = SurfaceFormat.Color;
			_3A_0003[5] = SurfaceFormat.Color;
		}
		_3A_0017 = new _0003.D();
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
		if (_3Al.TryGetValue((int)buffertype, out var value))
		{
			return value;
		}
		if (!createmissing)
		{
			return null;
		}
		GraphicsDevice graphicsDevice = _3AD.GraphicsDevice;
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
		value = new RenderTarget2D(graphicsDevice, _3A_0018, _3AL, mipMap, _3A_0003[(int)buffertype], preferredDepthFormat, preferredMultiSampleCount, RenderTargetUsage.PlatformContents);
		_3Al.Add((int)buffertype, value);
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
		if (_3AF.TryGetValue(name, out var value))
		{
			return value;
		}
		if (!createmissing)
		{
			return null;
		}
		value = new CustomFrameBufferCollection();
		_3AF.Add(name, value);
		return value;
	}

	/// <summary>
	/// Sets up the object prior to rendering.
	/// </summary>
	/// <param name="scenestate"></param>
	public void BeginFrameRendering(ISceneState scenestate)
	{
		if (_3A_0017.Changed)
		{
			Unload();
		}
		if (!_3A_0019)
		{
			PresentationParameters presentationParameters = _3AD.GraphicsDevice.PresentationParameters;
			if (_3A_0018 != presentationParameters.BackBufferWidth || _3AL != presentationParameters.BackBufferHeight)
			{
				_3A_0018 = presentationParameters.BackBufferWidth;
				_3AL = presentationParameters.BackBufferHeight;
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
		foreach (KeyValuePair<int, RenderTarget2D> item in _3Al)
		{
			item.Value.Dispose();
		}
		_3Al.Clear();
		foreach (KeyValuePair<string, CustomFrameBufferCollection> item2 in _3AF)
		{
			foreach (RenderTarget2D item3 in item2.Value)
			{
				item3.Dispose();
			}
		}
		_3AF.Clear();
		p._0018._6_0006(ref _3At);
	}
}
