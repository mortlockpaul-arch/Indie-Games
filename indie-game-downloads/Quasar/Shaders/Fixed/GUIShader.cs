using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;
using Quasar.Render;

namespace Quasar.Shaders.Fixed;

public class GUIShader : Shader
{
	private BasicEffect effect;

	public GUIShader()
	{
		effect = new BasicEffect(Engine.Device);
	}

	private void fillVariables(Transform motion, Material material)
	{
		effect.Alpha = material.Alpha;
		effect.DiffuseColor = material.Ambient + material.Diffuse;
		effect.EmissiveColor = Vector3.Zero;
		effect.FogEnabled = false;
		effect.PreferPerPixelLighting = false;
		effect.LightingEnabled = false;
		effect.Projection = SceneRenderData.CurrentRenderData.Camera.Projection;
		effect.Texture = material.Texture as Texture2D;
		effect.TextureEnabled = material.Textures.Count > 0;
		effect.View = SceneRenderData.CurrentRenderData.Camera.View;
		effect.World = motion.WorldMatrix;
		effect.Alpha = 1f;
		effect.DiffuseColor = Vector3.One;
		effect.Texture = null;
		effect.TextureEnabled = false;
		effect.Alpha = material.Alpha;
		effect.DiffuseColor = material.Ambient + material.Diffuse;
		Texture2D texture2D = material.Texture as Texture2D;
		effect.Texture = texture2D;
		effect.TextureEnabled = texture2D != null;
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

	public override void ApplyPass(int pass)
	{
		effect.CurrentTechnique.Passes[pass].Apply();
		Engine.Device.DepthStencilState = DepthStencilState.None;
		Engine.Device.BlendState = BlendState.NonPremultiplied;
	}

	public override int PassNumber(Material material)
	{
		return effect.CurrentTechnique.Passes.Count;
	}
}
