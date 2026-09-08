using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Elements.Cameras;
using Quasar.Global;
using Quasar.Render.Passes;

namespace Quasar.Render.Processes;

public class CubeMapRenderProcess : RenderProcess
{
	private CubeMapRenderPass[] facePasses;

	private CubeMapCamera[] cameras;

	private RenderTargetCube cubeMap;

	private Vector3 renderOrigin;

	public RenderTargetCube CubeMap => cubeMap;

	public Vector3 RenderOrigin
	{
		get
		{
			return renderOrigin;
		}
		set
		{
			renderOrigin = value;
		}
	}

	public static RenderTargetCube CreateRenderTarget(int size)
	{
		RenderTargetCube renderTargetCube = new RenderTargetCube(Engine.Device, size, mipMap: false, SurfaceFormat.Color, DepthFormat.Depth24, 1, RenderTargetUsage.DiscardContents);
		RenderTargetBinding[] deviceRenderTargets = Engine.Instance.GetDeviceRenderTargets();
		Engine.Instance.SetDeviceRenderTarget(renderTargetCube, CubeMapFace.NegativeX);
		Engine.Instance.SetDeviceRenderTarget(renderTargetCube, CubeMapFace.NegativeY);
		Engine.Instance.SetDeviceRenderTarget(renderTargetCube, CubeMapFace.NegativeZ);
		Engine.Instance.SetDeviceRenderTarget(renderTargetCube, CubeMapFace.PositiveX);
		Engine.Instance.SetDeviceRenderTarget(renderTargetCube, CubeMapFace.PositiveY);
		Engine.Instance.SetDeviceRenderTarget(renderTargetCube, CubeMapFace.PositiveZ);
		Engine.Instance.SetDeviceRenderTargets(deviceRenderTargets);
		return renderTargetCube;
	}

	public CubeMapRenderProcess(int cubeSize)
		: base((RenderPass)null)
	{
		cubeMap = CreateRenderTarget(cubeSize);
		initPasses();
	}

	private void initPasses()
	{
		facePasses = new CubeMapRenderPass[6];
		cameras = new CubeMapCamera[6];
		for (int i = 0; i < 6; i++)
		{
			facePasses[i] = new CubeMapRenderPass(cubeMap, (CubeMapFace)i);
			facePasses[i].MustClearDepth = true;
			if (i < 5)
			{
				addIntermediatePass(facePasses[i]);
			}
			else
			{
				finalRenderPass = facePasses[i];
			}
			cameras[i] = new CubeMapCamera((CubeMapFace)i);
		}
	}

	public override void addSource(SceneRenderData scene)
	{
		for (int i = 0; i < 6; i++)
		{
			facePasses[i].addSource(new SceneRenderData(scene.Scene, cameras[i], scene.Sorter));
		}
	}

	public override void Render()
	{
		for (int i = 0; i < 6; i++)
		{
			cameras[i].Transform.Translation = renderOrigin;
			cameras[i].Update();
		}
		base.Render();
	}

	public override void Dispose()
	{
		if (cubeMap != null)
		{
			cubeMap.Dispose();
			cubeMap = null;
		}
		facePasses = null;
		cameras = null;
		base.Dispose();
	}
}
