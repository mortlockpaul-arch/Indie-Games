using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;

namespace SynapseGaming.LightingSystem.Shadows;

/// <summary>
/// Class that manages shadow groups sharing the same render target.
/// </summary>
public class ShadowRenderTargetGroup : SafeSingletonBeginableObject, IDisposable
{
	private RenderTarget2D _3A_0018;

	private Viewport _3AL;

	private List<ShadowGroup> _3A_0019 = new List<ShadowGroup>(16);

	private Viewport _3A3;

	private RenderTargetBinding[] _3A6;

	/// <summary>
	/// The current RenderTarget used by this object.
	/// </summary>
	public RenderTarget2D RenderTarget => _3A_0018;

	/// <summary>
	/// Viewport that encapsulates the entire render target.
	/// </summary>
	public Viewport Viewport => _3AL;

	/// <summary>
	/// List of shadow groups managed by this object.
	/// </summary>
	public List<ShadowGroup> ShadowGroups => _3A_0019;

	/// <summary>
	/// Used to determine if the render target contents are valid or if the contents need
	/// to be re-rendered.
	///
	/// The default SunBurn shadow mapping implementation renders shadow map contents
	/// every frame, however custom implementations can provide static shadow maps.
	///
	/// Please note: if shadow maps are static and the contents are valid DO NOT call
	/// ShadowRenderTargetGroup Begin() and End().  On the Xbox this will invalidate the
	/// render target data.
	///
	/// However skipping calls to Begin and End require calling
	/// ShadowRenderTargetGroup.UpdateRenderTargetTexture() to ensure the shadow texture
	/// is up to date.
	///
	/// When using the built-in render managers this is all handled automatically.
	/// </summary>
	public bool ContentsAreValid
	{
		get
		{
			foreach (ShadowGroup item in _3A_0019)
			{
				if (!item.Shadow.ContentsAreValid)
				{
					return false;
				}
			}
			return true;
		}
	}

	/// <summary>
	/// Creates a new ShadowRenderTargetGroup instance.
	/// </summary>
	public ShadowRenderTargetGroup()
	{
	}

	/// <summary>
	/// Determines if the render target group uses shadows.
	/// </summary>
	/// <returns></returns>
	public bool HasShadows()
	{
		return _3A_0018 != null;
	}

	/// <summary>
	/// Builds the render target group information based on the
	/// provided render target and depth buffer.
	/// </summary>
	/// <param name="shadowmaprendertarget"></param>
	public void Build(RenderTarget2D shadowmaprendertarget)
	{
		_3A_0018 = shadowmaprendertarget;
		if (shadowmaprendertarget != null)
		{
			_3AL.X = 0;
			_3AL.Y = 0;
			_3AL.Width = shadowmaprendertarget.Width;
			_3AL.Height = shadowmaprendertarget.Height;
			_3AL.MinDepth = 0f;
			_3AL.MaxDepth = 1f;
		}
		else
		{
			_3AL = default(Viewport);
		}
	}

	/// <summary>
	/// Releases resources allocated by this object.
	/// </summary>
	public void Dispose()
	{
		_3A_0018 = null;
		_3A_0019.Clear();
		_3A6 = null;
	}

	/// <summary>
	/// Sets up the render target group for generating the shadow maps.
	/// </summary>
	public override void Begin()
	{
		base.Begin();
		if (_3A_0018 == null)
		{
			throw new Exception("Render target is null. This group dosn't contain shadows, begin cannot be called.");
		}
		if (_3A_0018 == null)
		{
			throw new Exception("Unsupported render target type. Must be RenderTarget2D.");
		}
		GraphicsDevice graphicsDevice = _3A_0018.GraphicsDevice;
		_3A3 = graphicsDevice.Viewport;
		_3A6 = graphicsDevice.GetRenderTargets();
		graphicsDevice.SetRenderTarget(_3A_0018);
		graphicsDevice.Viewport = _3AL;
		graphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil, Color.White, 1f, 0);
	}

	/// <summary>
	/// Finalizes rendering.
	/// </summary>
	public override void End()
	{
		base.End();
		if (_3A_0018 != null)
		{
			GraphicsDevice graphicsDevice = _3A_0018.GraphicsDevice;
			graphicsDevice.SetRenderTargets(_3A6);
			graphicsDevice.Viewport = _3A3;
		}
	}
}
