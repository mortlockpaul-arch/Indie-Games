using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;

namespace Quasar.Render.Passes;

public class CubeMapRenderPass : RenderPass
{
	private RenderTargetCube renderTarget;

	private CubeMapFace face;

	public RenderTargetCube RenderTarget
	{
		get
		{
			return renderTarget;
		}
		set
		{
			renderTarget = value;
		}
	}

	public CubeMapFace Face
	{
		get
		{
			return face;
		}
		set
		{
			face = value;
		}
	}

	public CubeMapRenderPass(RenderTargetCube cubemap, CubeMapFace face)
	{
		renderTarget = cubemap;
		this.face = face;
	}

	protected override void SetRenderTarget()
	{
		Engine.Instance.SetDeviceRenderTarget(renderTarget, face);
	}
}
