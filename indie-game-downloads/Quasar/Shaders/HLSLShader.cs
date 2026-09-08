using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;
using Quasar.Render;
using Quasar.Textures;

namespace Quasar.Shaders;

public class HLSLShader : Shader
{
	private enum ShaderHandles
	{
		WorldViewProjection,
		World,
		View,
		Projection,
		ViewInverse,
		WorldInverseTranspose,
		Diffuse,
		GlobalDiffuse,
		AlphaTest,
		Ambient,
		GlobalAmbient,
		Specular,
		GlobalSpecular,
		ElapsedTime,
		Time,
		ErrorTexture,
		ViewProjection,
		PixelUV,
		FrameNumber,
		FogStart,
		FogRange,
		FogColor,
		Count
	}

	private enum ShaderVariableHandles
	{
		LightPosition,
		LightAmbient,
		LightDiffuse,
		LightSpecular,
		LightAttenuation,
		Texture,
		FloatParameter,
		Float4Parameter,
		MatrixParameter,
		Float4ArrayParameter,
		IntParameter,
		MatrixArrayParameter,
		GlobalTexture,
		GlobalFloatParameter,
		GlobalFloat4Parameter,
		GlobalMatrixParameter,
		GlobalFloat4ArrayParameter,
		GlobalIntParameter,
		GlobalMatrixArrayParameter,
		Count
	}

	private class ShaderParameter
	{
		public ShaderHandles HandleType;

		public EffectParameter Parameter;

		public ShaderParameter(EffectParameter parameter, ShaderHandles handleType)
		{
			HandleType = handleType;
			Parameter = parameter;
		}
	}

	private class VariableShaderParameter
	{
		public ShaderVariableHandles HandleType;

		public EffectParameter Parameter;

		public int Index;

		public VariableShaderParameter(EffectParameter parameter, ShaderVariableHandles handleType, int index)
		{
			HandleType = handleType;
			Parameter = parameter;
			Index = index;
		}
	}

	private List<ShaderParameter> handles;

	private List<VariableShaderParameter> variableHandles;

	private Effect effect;

	private EffectTechnique defaultTechnique;

	private bool defaultTechniqueHasRestorePass;

	private Matrix tempMatrix;

	private static readonly Dictionary<string, ShaderHandles> standardSemantics;

	private static readonly Dictionary<string, ShaderVariableHandles> variableSemantics;

	public HLSLShader(string filename)
	{
		effect = Engine.ContentManager.Load<Effect>(filename);
		EffectTechnique technique = effect.Techniques[0];
		defaultTechniqueHasRestorePass = HasRestorePass(technique);
		defaultTechnique = technique;
		loadHandles();
	}

	public override bool HasTechnique(string technique)
	{
		return effect.Techniques[technique] != null;
	}

	private bool checkSemantic(EffectParameter ep)
	{
		string semantic = ep.Semantic;
		if (standardSemantics.TryGetValue(semantic, out var value))
		{
			handles.Add(new ShaderParameter(ep, value));
			return true;
		}
		int num = semantic.Length;
		while (num > 0 && char.IsDigit(semantic[num - 1]))
		{
			num--;
		}
		if (num < semantic.Length)
		{
			string key = semantic.Substring(0, num);
			int index = GameMath.ParseInt(semantic.Substring(num), 0);
			if (variableSemantics.TryGetValue(key, out var value2))
			{
				variableHandles.Add(new VariableShaderParameter(ep, value2, index));
				return true;
			}
		}
		return false;
	}

	private void loadHandles()
	{
		handles = new List<ShaderParameter>(8);
		variableHandles = new List<VariableShaderParameter>(2);
		foreach (EffectParameter parameter in effect.Parameters)
		{
			checkSemantic(parameter);
		}
	}

	private void fillVariables(Transform motion, Material material)
	{
		int count = handles.Count;
		for (int i = 0; i < count; i++)
		{
			ShaderParameter shaderParameter = handles[i];
			switch (shaderParameter.HandleType)
			{
			case ShaderHandles.WorldViewProjection:
				SceneRenderData.CurrentRenderData.Camera.GetWVP(motion, out tempMatrix);
				shaderParameter.Parameter.SetValue(tempMatrix);
				break;
			case ShaderHandles.World:
				shaderParameter.Parameter.SetValue(motion.WorldMatrix);
				break;
			case ShaderHandles.View:
				shaderParameter.Parameter.SetValue(SceneRenderData.CurrentRenderData.Camera.View);
				break;
			case ShaderHandles.Projection:
				shaderParameter.Parameter.SetValue(SceneRenderData.CurrentRenderData.Camera.Projection);
				break;
			case ShaderHandles.ViewProjection:
				shaderParameter.Parameter.SetValue(SceneRenderData.CurrentRenderData.Camera.ViewProjection);
				break;
			case ShaderHandles.ViewInverse:
				shaderParameter.Parameter.SetValue(SceneRenderData.CurrentRenderData.Camera.Transform.WorldMatrix);
				break;
			case ShaderHandles.WorldInverseTranspose:
				shaderParameter.Parameter.SetValueTranspose(Matrix.Invert(motion.WorldMatrix));
				break;
			case ShaderHandles.PixelUV:
				shaderParameter.Parameter.SetValue(Engine.Instance.PixelSize);
				break;
			case ShaderHandles.AlphaTest:
				shaderParameter.Parameter.SetValue(material.AlphaTest);
				break;
			case ShaderHandles.Diffuse:
				shaderParameter.Parameter.SetValue(material.DiffuseWithAlpha);
				break;
			case ShaderHandles.Ambient:
				shaderParameter.Parameter.SetValue(material.Ambient);
				break;
			case ShaderHandles.Specular:
				shaderParameter.Parameter.SetValue(material.SpecularWithShininess);
				break;
			case ShaderHandles.GlobalDiffuse:
				shaderParameter.Parameter.SetValue(SceneRenderData.CurrentRenderData.Scene.GlobalMaterial.DiffuseWithAlpha);
				break;
			case ShaderHandles.GlobalAmbient:
				shaderParameter.Parameter.SetValue(SceneRenderData.CurrentRenderData.Scene.GlobalMaterial.Ambient);
				break;
			case ShaderHandles.GlobalSpecular:
				shaderParameter.Parameter.SetValue(SceneRenderData.CurrentRenderData.Scene.GlobalMaterial.SpecularWithShininess);
				break;
			case ShaderHandles.ElapsedTime:
				shaderParameter.Parameter.SetValue(Timer.DefaultTimer.LastInterval);
				break;
			case ShaderHandles.Time:
				shaderParameter.Parameter.SetValue(Timer.DefaultTimer.TotalTimeSeconds);
				break;
			case ShaderHandles.FrameNumber:
				shaderParameter.Parameter.SetValue(Engine.Instance.ElapsedFrames);
				break;
			case ShaderHandles.FogStart:
				shaderParameter.Parameter.SetValue(SceneRenderData.CurrentRenderData.Scene.FogStart);
				break;
			case ShaderHandles.FogRange:
				shaderParameter.Parameter.SetValue(SceneRenderData.CurrentRenderData.Scene.FogRange);
				break;
			case ShaderHandles.FogColor:
				shaderParameter.Parameter.SetValue(SceneRenderData.CurrentRenderData.Scene.FogColor);
				break;
			case ShaderHandles.ErrorTexture:
				shaderParameter.Parameter.SetValue(TextureManager.Textures.ErrorItem);
				break;
			}
		}
		count = variableHandles.Count;
		for (int j = 0; j < count; j++)
		{
			VariableShaderParameter variableShaderParameter = variableHandles[j];
			switch (variableShaderParameter.HandleType)
			{
			case ShaderVariableHandles.FloatParameter:
				material.SetFloatShaderParameter(variableShaderParameter.Parameter, variableShaderParameter.Index);
				break;
			case ShaderVariableHandles.IntParameter:
				material.SetIntShaderParameter(variableShaderParameter.Parameter, variableShaderParameter.Index);
				break;
			case ShaderVariableHandles.Float4Parameter:
				material.SetVector4ShaderParameter(variableShaderParameter.Parameter, variableShaderParameter.Index);
				break;
			case ShaderVariableHandles.Float4ArrayParameter:
				material.SetVector4ArrayShaderParameter(variableShaderParameter.Parameter, variableShaderParameter.Index);
				break;
			case ShaderVariableHandles.MatrixParameter:
				material.SetMatrixShaderParameter(variableShaderParameter.Parameter, variableShaderParameter.Index);
				break;
			case ShaderVariableHandles.MatrixArrayParameter:
				material.SetMatrixArrayShaderParameter(variableShaderParameter.Parameter, variableShaderParameter.Index);
				break;
			case ShaderVariableHandles.Texture:
				if (material.Textures.Count > variableShaderParameter.Index)
				{
					variableShaderParameter.Parameter.SetValue(material.Textures[variableShaderParameter.Index]);
				}
				else
				{
					variableShaderParameter.Parameter.SetValue(TextureManager.Textures.ErrorItem);
				}
				break;
			case ShaderVariableHandles.GlobalFloatParameter:
				SceneRenderData.CurrentRenderData.Scene.GlobalMaterial.SetFloatShaderParameter(variableShaderParameter.Parameter, variableShaderParameter.Index);
				break;
			case ShaderVariableHandles.GlobalIntParameter:
				SceneRenderData.CurrentRenderData.Scene.GlobalMaterial.SetIntShaderParameter(variableShaderParameter.Parameter, variableShaderParameter.Index);
				break;
			case ShaderVariableHandles.GlobalFloat4Parameter:
				SceneRenderData.CurrentRenderData.Scene.GlobalMaterial.SetVector4ShaderParameter(variableShaderParameter.Parameter, variableShaderParameter.Index);
				break;
			case ShaderVariableHandles.GlobalFloat4ArrayParameter:
				SceneRenderData.CurrentRenderData.Scene.GlobalMaterial.SetVector4ArrayShaderParameter(variableShaderParameter.Parameter, variableShaderParameter.Index);
				break;
			case ShaderVariableHandles.GlobalMatrixParameter:
				SceneRenderData.CurrentRenderData.Scene.GlobalMaterial.SetMatrixShaderParameter(variableShaderParameter.Parameter, variableShaderParameter.Index);
				break;
			case ShaderVariableHandles.GlobalMatrixArrayParameter:
				SceneRenderData.CurrentRenderData.Scene.GlobalMaterial.SetMatrixArrayShaderParameter(variableShaderParameter.Parameter, variableShaderParameter.Index);
				break;
			case ShaderVariableHandles.GlobalTexture:
				if (SceneRenderData.CurrentRenderData.Scene.GlobalMaterial.Textures.Count > variableShaderParameter.Index)
				{
					variableShaderParameter.Parameter.SetValue(SceneRenderData.CurrentRenderData.Scene.GlobalMaterial.Textures[variableShaderParameter.Index]);
				}
				else
				{
					variableShaderParameter.Parameter.SetValue(TextureManager.Textures.ErrorItem);
				}
				break;
			case ShaderVariableHandles.LightPosition:
				if (SceneRenderData.CurrentRenderData.CurrentLights.Count > variableShaderParameter.Index)
				{
					variableShaderParameter.Parameter.SetValue(new Vector4(SceneRenderData.CurrentRenderData.CurrentLights[variableShaderParameter.Index].Transform.WorldTranslation, 1f));
				}
				else
				{
					variableShaderParameter.Parameter.SetValue(new Vector4(0f, 0f, 0f, 1f));
				}
				break;
			case ShaderVariableHandles.LightDiffuse:
				if (SceneRenderData.CurrentRenderData.CurrentLights.Count > variableShaderParameter.Index)
				{
					variableShaderParameter.Parameter.SetValue(SceneRenderData.CurrentRenderData.CurrentLights[variableShaderParameter.Index].Diffuse);
				}
				else
				{
					variableShaderParameter.Parameter.SetValue(Vector3.Zero);
				}
				break;
			case ShaderVariableHandles.LightAmbient:
				if (SceneRenderData.CurrentRenderData.CurrentLights.Count > variableShaderParameter.Index)
				{
					variableShaderParameter.Parameter.SetValue(SceneRenderData.CurrentRenderData.CurrentLights[variableShaderParameter.Index].Ambient);
				}
				else
				{
					variableShaderParameter.Parameter.SetValue(Vector3.Zero);
				}
				break;
			case ShaderVariableHandles.LightSpecular:
				if (SceneRenderData.CurrentRenderData.CurrentLights.Count > variableShaderParameter.Index)
				{
					variableShaderParameter.Parameter.SetValue(SceneRenderData.CurrentRenderData.CurrentLights[variableShaderParameter.Index].Specular);
				}
				else
				{
					variableShaderParameter.Parameter.SetValue(Vector3.Zero);
				}
				break;
			case ShaderVariableHandles.LightAttenuation:
				if (SceneRenderData.CurrentRenderData.CurrentLights.Count > variableShaderParameter.Index)
				{
					variableShaderParameter.Parameter.SetValue(SceneRenderData.CurrentRenderData.CurrentLights[variableShaderParameter.Index].Attenuation);
				}
				else
				{
					variableShaderParameter.Parameter.SetValue(Vector3.UnitX);
				}
				break;
			}
		}
	}

	public override int PassNumber(Material material)
	{
		if (effect == null)
		{
			return 0;
		}
		EffectTechnique effectTechnique = null;
		if (material.Technique != null)
		{
			effectTechnique = effect.Techniques[material.Technique];
		}
		if (effectTechnique == null)
		{
			effectTechnique = defaultTechnique;
		}
		if (HasRestorePass(effectTechnique))
		{
			return effectTechnique.Passes.Count - 1;
		}
		return effectTechnique.Passes.Count;
	}

	public bool HasRestorePass(EffectTechnique technique)
	{
		if (technique == defaultTechnique)
		{
			return defaultTechniqueHasRestorePass;
		}
		if (technique == null)
		{
			return false;
		}
		EffectPassCollection passes = technique.Passes;
		if (passes.Count == 1)
		{
			return false;
		}
		if (passes[passes.Count - 1].Name == "Restore")
		{
			return true;
		}
		return false;
	}

	public override void ApplyPass(int pass)
	{
		if (effect != null)
		{
			effect.CurrentTechnique.Passes[pass].Apply();
		}
	}

	public override void BeginRender(Transform motion, Material material)
	{
		if (effect == null)
		{
			return;
		}
		if (material.Technique != null)
		{
			EffectTechnique effectTechnique = effect.Techniques[material.Technique];
			if (effectTechnique != null)
			{
				effect.CurrentTechnique = effectTechnique;
			}
		}
		fillVariables(motion, material);
	}

	public override void BeginRender(Transform motion, Material material, string technique)
	{
		if (effect == null)
		{
			return;
		}
		if (technique != null)
		{
			EffectTechnique effectTechnique = effect.Techniques[technique];
			if (effectTechnique != null)
			{
				effect.CurrentTechnique = effectTechnique;
			}
		}
		fillVariables(motion, material);
	}

	public override void EndRender()
	{
		if (effect != null && HasRestorePass(effect.CurrentTechnique))
		{
			EffectPass effectPass = effect.CurrentTechnique.Passes[effect.CurrentTechnique.Passes.Count - 1];
			effectPass.Apply();
		}
	}

	public override void Dispose()
	{
		effect = null;
		handles = null;
		variableHandles = null;
		base.Dispose();
	}

	static HLSLShader()
	{
		standardSemantics = new Dictionary<string, ShaderHandles>(StringComparer.InvariantCultureIgnoreCase);
		variableSemantics = new Dictionary<string, ShaderVariableHandles>(StringComparer.InvariantCultureIgnoreCase);
		for (int i = 0; i < 22; i++)
		{
			standardSemantics.Add(((ShaderHandles)i).ToString(), (ShaderHandles)i);
		}
		for (int j = 0; j < 19; j++)
		{
			variableSemantics.Add(((ShaderVariableHandles)j).ToString(), (ShaderVariableHandles)j);
		}
	}
}
