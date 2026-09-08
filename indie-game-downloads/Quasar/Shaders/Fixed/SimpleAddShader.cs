using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;

namespace Quasar.Shaders.Fixed;

public class SimpleAddShader : SimpleShader
{
	public override void ApplyPass(int pass)
	{
		effect.CurrentTechnique.Passes[pass].Apply();
		Engine.Device.DepthStencilState = DepthStencilState.Default;
		Engine.Device.BlendState = BlendState.Additive;
	}
}
