using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Elements;
using Quasar.Global;
using Quasar.Render;

namespace Quasar.Shaders.Fixed;

public class BasicShader : Shader
{
	protected BasicEffect effect;

	public BasicShader()
	{
		effect = new BasicEffect(Engine.Device);
	}

	private void setLightParams(DirectionalLight dLight, Light light)
	{
		dLight.Enabled = true;
		dLight.DiffuseColor = light.Diffuse;
		dLight.Direction = light.Transform.ZVector;
		dLight.SpecularColor = light.Specular;
	}

	private void fillVariables(Transform motion, Material material)
	{
		effect.Alpha = material.Alpha;
		effect.DiffuseColor = material.Diffuse;
		effect.AmbientLightColor = material.Ambient;
		effect.EmissiveColor = Vector3.Zero;
		effect.FogStart = SceneRenderData.CurrentRenderData.Scene.FogStart;
		effect.FogEnd = effect.FogStart + SceneRenderData.CurrentRenderData.Scene.FogRange;
		effect.FogColor = SceneRenderData.CurrentRenderData.Scene.FogColor;
		effect.FogEnabled = false;
		effect.PreferPerPixelLighting = false;
		effect.Projection = SceneRenderData.CurrentRenderData.Camera.Projection;
		effect.SpecularColor = material.Specular;
		effect.SpecularPower = material.Shininess;
		effect.Texture = material.Texture as Texture2D;
		effect.TextureEnabled = material.Textures.Count > 0;
		effect.View = SceneRenderData.CurrentRenderData.Camera.View;
		effect.World = motion.WorldMatrix;
		if (SceneRenderData.CurrentRenderData.Scene.DefaultLights.Count > 0)
		{
			effect.LightingEnabled = true;
			setLightParams(effect.DirectionalLight0, SceneRenderData.CurrentRenderData.Scene.DefaultLights[0]);
			if (SceneRenderData.CurrentRenderData.Scene.DefaultLights.Count > 1)
			{
				setLightParams(effect.DirectionalLight1, SceneRenderData.CurrentRenderData.Scene.DefaultLights[1]);
				if (SceneRenderData.CurrentRenderData.Scene.DefaultLights.Count > 2)
				{
					setLightParams(effect.DirectionalLight2, SceneRenderData.CurrentRenderData.Scene.DefaultLights[2]);
				}
				else
				{
					effect.DirectionalLight2.Enabled = false;
				}
			}
			else
			{
				effect.DirectionalLight1.Enabled = false;
			}
		}
		else
		{
			effect.DirectionalLight0.Enabled = false;
			effect.LightingEnabled = false;
		}
	}

	public override bool HasTechnique(string technique)
	{
		return false;
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

	public override void EndRender()
	{
	}

	public override void ApplyPass(int pass)
	{
		effect.CurrentTechnique.Passes[pass].Apply();
		Engine.Device.DepthStencilState = DepthStencilState.Default;
		Engine.Device.BlendState = BlendState.NonPremultiplied;
	}

	public override int PassNumber(Material material)
	{
		return effect.CurrentTechnique.Passes.Count;
	}
}
