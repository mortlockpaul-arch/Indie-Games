using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;

namespace Quasar.Render.Passes;

public class AntialiasRenderPass : RenderPass2D
{
	public AntialiasRenderPass()
		: this(Engine.BackBufferSize, 2)
	{
	}

	public new static RenderTarget2D CreateRenderTarget()
	{
		return CreateRenderTarget(Engine.BackBufferSize, 2);
	}

	public static RenderTarget2D CreateRenderTarget(Vector2 size, int sampleCount)
	{
		RenderTarget2D renderTarget2D = new RenderTarget2D(Engine.Device, (int)size.X, (int)size.Y, mipMap: false, SurfaceFormat.Color, DepthFormat.Depth24, sampleCount, RenderTargetUsage.DiscardContents);
		RenderTargetBinding[] deviceRenderTargets = Engine.Instance.GetDeviceRenderTargets();
		Engine.Instance.SetDeviceRenderTarget(renderTarget2D);
		Engine.Instance.SetDeviceRenderTargets(deviceRenderTargets);
		return renderTarget2D;
	}

	public AntialiasRenderPass(Vector2 size, int multisampleCount)
		: base(createRenderTarget: false)
	{
		SetRenderTarget(CreateRenderTarget(size, multisampleCount), setOwner: true);
	}
}
