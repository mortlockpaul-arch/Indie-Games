using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;

namespace Quasar.Render;

public class RenderPass2D : RenderPass
{
	private RenderTarget2D renderTarget;

	private bool targetOwner;

	public RenderTarget2D RenderTarget => renderTarget;

	public bool TargetOwner
	{
		get
		{
			return targetOwner;
		}
		set
		{
			targetOwner = value;
		}
	}

	public void SetRenderTarget(RenderTarget2D target, bool setOwner)
	{
		if (target == renderTarget)
		{
			targetOwner = setOwner;
			return;
		}
		ClearRenderTarget();
		renderTarget = target;
		targetOwner = setOwner;
	}

	private void ClearRenderTarget()
	{
		if (!targetOwner)
		{
			renderTarget = null;
			return;
		}
		if (renderTarget != null)
		{
			renderTarget.Dispose();
		}
		renderTarget = null;
	}

	public static RenderTarget2D CreateRenderTarget()
	{
		return CreateRenderTarget(Engine.BackBufferSize);
	}

	public static RenderTarget2D CreateRenderTarget(Vector2 size)
	{
		RenderTarget2D renderTarget2D = new RenderTarget2D(Engine.Device, (int)size.X, (int)size.Y, mipMap: false, SurfaceFormat.Color, DepthFormat.Depth24, 1, RenderTargetUsage.DiscardContents);
		RenderTargetBinding[] deviceRenderTargets = Engine.Instance.GetDeviceRenderTargets();
		Engine.Instance.SetDeviceRenderTarget(renderTarget2D);
		Engine.Instance.SetDeviceRenderTargets(deviceRenderTargets);
		return renderTarget2D;
	}

	public static RenderTarget2D CreatePreserveContentsRenderTarget(Color backgroundColor)
	{
		return CreatePreserveContentsRenderTarget(backgroundColor, Engine.BackBufferSize);
	}

	public static RenderTarget2D CreatePreserveContentsRenderTarget(Color backgroundColor, Vector2 size)
	{
		RenderTarget2D renderTarget2D = new RenderTarget2D(Engine.Device, (int)size.X, (int)size.Y, mipMap: false, SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);
		RenderTargetBinding[] deviceRenderTargets = Engine.Instance.GetDeviceRenderTargets();
		Engine.Instance.SetDeviceRenderTarget(renderTarget2D);
		Engine.Device.Clear(ClearOptions.Target, backgroundColor, 0f, 0);
		Engine.Instance.SetDeviceRenderTargets(deviceRenderTargets);
		return renderTarget2D;
	}

	public static RenderTarget2D CreateShadowRenderTarget()
	{
		return CreateShadowRenderTarget(Engine.BackBufferSize);
	}

	public static RenderTarget2D CreateShadowRenderTarget(Vector2 size)
	{
		RenderTarget2D renderTarget2D = new RenderTarget2D(Engine.Device, (int)size.X, (int)size.Y, mipMap: false, SurfaceFormat.Single, DepthFormat.Depth24, 1, RenderTargetUsage.DiscardContents);
		RenderTargetBinding[] deviceRenderTargets = Engine.Instance.GetDeviceRenderTargets();
		Engine.Instance.SetDeviceRenderTarget(renderTarget2D);
		Engine.Instance.SetDeviceRenderTargets(deviceRenderTargets);
		return renderTarget2D;
	}

	public RenderPass2D(bool createRenderTarget)
	{
		if (createRenderTarget)
		{
			SetRenderTarget(CreateRenderTarget(), setOwner: true);
		}
	}

	public RenderPass2D(Vector2 renderTargetSize)
	{
		SetRenderTarget(CreateRenderTarget(renderTargetSize), setOwner: true);
	}

	public RenderPass2D()
		: this(createRenderTarget: true)
	{
	}

	protected override void SetRenderTarget()
	{
		Engine.Instance.SetDeviceRenderTarget(RenderTarget);
	}

	public override void Dispose()
	{
		if (targetOwner && renderTarget != null)
		{
			renderTarget.Dispose();
			renderTarget = null;
		}
		base.Dispose();
	}
}
