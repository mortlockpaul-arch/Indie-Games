using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;
using Quasar.Render;

namespace Quasar.Shaders.Fixed;

public class ParticleAlphaShader : Shader
{
	protected BasicEffect effect;

	public ParticleAlphaShader()
	{
		effect = new BasicEffect(Engine.Device);
	}

	public override bool HasTechnique(string technique)
	{
		return false;
	}

	public override void BeginRender(Transform motion, Material material)
	{
		if (effect == null)
		{
			return;
		}
		fillVariables(motion, material);
		if (material.Texture is Texture2D texture2D)
		{
			if (!GameMath.IsPowerOfTwo(texture2D.Width) || !GameMath.IsPowerOfTwo(texture2D.Height))
			{
				Engine.Device.SamplerStates[0] = SamplerState.LinearClamp;
			}
			else
			{
				Engine.Device.SamplerStates[0] = SamplerState.LinearWrap;
			}
		}
	}

	public override void BeginRender(Transform motion, Material material, string technique)
	{
		if (effect == null)
		{
			return;
		}
		fillVariables(motion, material);
		if (material.Texture is Texture2D texture2D)
		{
			if (!GameMath.IsPowerOfTwo(texture2D.Width) || !GameMath.IsPowerOfTwo(texture2D.Height))
			{
				Engine.Device.SamplerStates[0] = SamplerState.LinearClamp;
			}
			else
			{
				Engine.Device.SamplerStates[0] = SamplerState.LinearWrap;
			}
		}
	}

	public override void EndRender()
	{
	}

	private void fillVariables(Transform motion, Material material)
	{
		effect.DiffuseColor = material.Diffuse;
		effect.EmissiveColor = Vector3.One;
		effect.FogEnabled = false;
		effect.PreferPerPixelLighting = false;
		effect.LightingEnabled = false;
		effect.Projection = SceneRenderData.CurrentRenderData.Camera.Projection;
		effect.Texture = material.Texture as Texture2D;
		effect.TextureEnabled = material.Textures.Count > 0;
		effect.View = SceneRenderData.CurrentRenderData.Camera.View;
		effect.VertexColorEnabled = true;
		if (material.GetIntParameter(0) == 1)
		{
			effect.World = Matrix.Identity;
		}
		else
		{
			effect.World = motion.WorldMatrix;
		}
	}

	public override void ApplyPass(int pass)
	{
		effect.CurrentTechnique.Passes[pass].Apply();
		Engine.Device.DepthStencilState = DepthStencilState.DepthRead;
		Engine.Device.BlendState = BlendState.NonPremultiplied;
	}

	public override int PassNumber(Material material)
	{
		return effect.CurrentTechnique.Passes.Count;
	}
}
