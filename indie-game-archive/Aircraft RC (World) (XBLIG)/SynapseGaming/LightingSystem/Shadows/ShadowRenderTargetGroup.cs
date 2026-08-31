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
	private RenderTarget2D HCB;

	private Viewport HC_0002;

	private List<ShadowGroup> HC_0012 = new List<ShadowGroup>(16);

	private Viewport HCH;

	private RenderTargetBinding[] HC7;

	/// <summary>
	/// The current RenderTarget used by this object.
	/// </summary>
	public RenderTarget2D RenderTarget => HCB;

	/// <summary>
	/// Viewport that encapsulates the entire render target.
	/// </summary>
	public Viewport Viewport => HC_0002;

	/// <summary>
	/// List of shadow groups managed by this object.
	/// </summary>
	public List<ShadowGroup> ShadowGroups => HC_0012;

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
			foreach (ShadowGroup item in HC_0012)
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
		return HCB != null;
	}

	/// <summary>
	/// Builds the render target group information based on the
	/// provided render target and depth buffer.
	/// </summary>
	/// <param name="shadowmaprendertarget"></param>
	public void Build(RenderTarget2D shadowmaprendertarget)
	{
		HCB = shadowmaprendertarget;
		if (shadowmaprendertarget != null)
		{
			HC_0002.X = 0;
			HC_0002.Y = 0;
			HC_0002.Width = shadowmaprendertarget.Width;
			HC_0002.Height = shadowmaprendertarget.Height;
			HC_0002.MinDepth = 0f;
			HC_0002.MaxDepth = 1f;
		}
		else
		{
			HC_0002 = default(Viewport);
		}
	}

	/// <summary>
	/// Releases resources allocated by this object.
	/// </summary>
	public void Dispose()
	{
		HCB = null;
		HC_0012.Clear();
		HC7 = null;
	}

	/// <summary>
	/// Sets up the render target group for generating the shadow maps.
	/// </summary>
	public override void Begin()
	{
		base.Begin();
		if (HCB == null)
		{
			throw new Exception("Render target is null. This group dosn't contain shadows, begin cannot be called.");
		}
		if (HCB == null)
		{
			throw new Exception("Unsupported render target type. Must be RenderTarget2D.");
		}
		GraphicsDevice graphicsDevice = HCB.GraphicsDevice;
		HCH = graphicsDevice.Viewport;
		HC7 = graphicsDevice.GetRenderTargets();
		graphicsDevice.SetRenderTarget(HCB);
		graphicsDevice.Viewport = HC_0002;
		graphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil, Color.White, 1f, 0);
	}

	/// <summary>
	/// Finalizes rendering.
	/// </summary>
	public override void End()
	{
		base.End();
		if (HCB != null)
		{
			GraphicsDevice graphicsDevice = HCB.GraphicsDevice;
			graphicsDevice.SetRenderTargets(HC7);
			graphicsDevice.Viewport = HCH;
		}
	}
}
