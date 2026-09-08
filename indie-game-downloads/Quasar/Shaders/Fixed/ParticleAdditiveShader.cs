using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;

namespace Quasar.Shaders.Fixed;

public class ParticleAdditiveShader : ParticleAlphaShader
{
	public override void ApplyPass(int pass)
	{
		effect.CurrentTechnique.Passes[pass].Apply();
		Engine.Device.DepthStencilState = DepthStencilState.DepthRead;
		Engine.Device.BlendState = BlendState.Additive;
	}
}
