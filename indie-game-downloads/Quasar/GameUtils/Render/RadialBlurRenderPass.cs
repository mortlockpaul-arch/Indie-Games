using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;
using Quasar.Render;
using Quasar.Scenes;
using Quasar.Shaders;

namespace Quasar.GameUtils.Render;

public class RadialBlurRenderPass : RenderPass2D
{
	private PostprocessScene ppScene;

	private Vector2 center = new Vector2(0.5f);

	public Vector2 CenterPoint
	{
		get
		{
			return ppScene.PostProcess.Material.GetVector4Parameter(0).ToVector2();
		}
		set
		{
			ppScene.PostProcess.Material.SetVector4Parameter(0, new Vector4(value, 0f, 0f));
		}
	}

	public float BlurWidth
	{
		get
		{
			return ppScene.PostProcess.Material.GetFloatParameter(0);
		}
		set
		{
			ppScene.PostProcess.Material.SetFloatParameter(0, value);
		}
	}

	public float Desaturate
	{
		get
		{
			return ppScene.PostProcess.Material.GetFloatParameter(1);
		}
		set
		{
			ppScene.PostProcess.Material.SetFloatParameter(1, value);
		}
	}

	public float MinSaturation
	{
		get
		{
			return ppScene.PostProcess.Material.GetFloatParameter(2);
		}
		set
		{
			ppScene.PostProcess.Material.SetFloatParameter(2, value);
		}
	}

	public float OriginalPixelWeight
	{
		get
		{
			return ppScene.PostProcess.Material.GetFloatParameter(3);
		}
		set
		{
			ppScene.PostProcess.Material.SetFloatParameter(3, value);
		}
	}

	public RadialBlurRenderPass(Texture texture)
		: this(texture, createRenderTarget: true)
	{
	}

	public RadialBlurRenderPass(Texture texture, bool createRenderTarget)
		: base(createRenderTarget)
	{
		initTargets();
		SetSource(texture);
	}

	public void SetSource(Texture texture)
	{
		ppScene.PostProcess.Material.Texture = texture;
	}

	private void initTargets()
	{
		ppScene = new PostprocessScene(null, ShaderManager.Shaders["RadialBlur"]);
		ppScene.PostProcess.Material.AddFloatParameter(0.02f);
		ppScene.PostProcess.Material.AddFloatParameter(25f);
		ppScene.PostProcess.Material.AddFloatParameter(0.25f);
		ppScene.PostProcess.Material.AddFloatParameter(4f);
		ppScene.PostProcess.Material.AddVector4Parameter(new Vector4(center, 0f, 0f));
		addSource(ppScene);
		MustClearColor = false;
		MustClearDepth = false;
	}

	public override void Render()
	{
		base.Render();
	}
}
