using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;

namespace Quasar.Shaders.Fixed;

public class ParticleMultiplyShader : BasicShader
{
	private BlendState multiplyState;

	public ParticleMultiplyShader()
	{
		multiplyState = new BlendState();
		multiplyState.AlphaDestinationBlend = Blend.InverseSourceAlpha;
		multiplyState.AlphaSourceBlend = Blend.Zero;
	}

	public override void BeginRender(Transform motion, Material material)
	{
		base.BeginRender(motion, material);
		if (effect != null)
		{
			effect.VertexColorEnabled = true;
			if (material.GetIntParameter(0) == 1)
			{
				effect.World = Matrix.Identity;
			}
		}
	}

	public override void ApplyPass(int pass)
	{
		effect.CurrentTechnique.Passes[pass].Apply();
		Engine.Device.DepthStencilState = DepthStencilState.DepthRead;
		Engine.Device.BlendState = multiplyState;
	}
}
