using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;
using Quasar.Render;

namespace Quasar.Shaders.Fixed;

public class VertexColorShader : Shader
{
	private BasicEffect effect;

	public VertexColorShader()
	{
		effect = new BasicEffect(Engine.Device);
	}

	private void fillVariables(Transform motion, Material material)
	{
		effect.Alpha = material.Alpha;
		effect.DiffuseColor = material.Diffuse;
		effect.EmissiveColor = Vector3.Zero;
		effect.FogEnabled = false;
		effect.PreferPerPixelLighting = false;
		effect.LightingEnabled = false;
		effect.Projection = SceneRenderData.CurrentRenderData.Camera.Projection;
		effect.TextureEnabled = false;
		effect.View = SceneRenderData.CurrentRenderData.Camera.View;
		effect.World = motion.WorldMatrix;
		effect.VertexColorEnabled = true;
	}

	public override bool HasTechnique(string technique)
	{
		return false;
	}

	public override void BeginRender(Transform motion, Material material)
	{
		if (effect != null)
		{
			fillVariables(motion, material);
		}
	}

	public override void BeginRender(Transform motion, Material material, string technique)
	{
		if (effect != null)
		{
			fillVariables(motion, material);
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
