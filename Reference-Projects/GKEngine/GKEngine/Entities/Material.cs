using System;
using System.Collections.Generic;
using GKEngine.Cameras;
using GKEngine.Scenes;
using GKEngine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GKEngine.Entities;

public class Material
{
	public enum State
	{
		None = -1,
		Solid,
		Alpha,
		AlphaNoDepthWrite,
		Add,
		Subtract,
		AlphaCulled,
		AlphaNoDepth
	}

	public const char DATA_DELIMETER = ':';

	public const char DATA_PARAM_DELIMETER = '=';

	public const char DATA_VALUE_DELIMETER = '|';

	public const string DATA_BONES_PARAM_NAME = "Bones";

	public const string DATA_TEXTURE_PARAM_TYPE = "Texture";

	public const string EFFECTS_PATH = "Content/Effects/";

	public const string CONTENT_PATH = "Content/";

	public const string TEXTURE_EXCEPTION_0 = "TEXTURESTAGE";

	public static BlendState BLEND_SUBTRACT = new BlendState
	{
		ColorSourceBlend = Blend.SourceAlpha,
		ColorDestinationBlend = Blend.One,
		ColorBlendFunction = BlendFunction.ReverseSubtract,
		AlphaSourceBlend = Blend.SourceAlpha,
		AlphaDestinationBlend = Blend.One,
		AlphaBlendFunction = BlendFunction.ReverseSubtract
	};

	public static string[] RENDER_PARAMS = new string[8] { "WorldIT", "WorldVP", "World", "ViewI", "View", "Projection", "CameraPos", "focalLength" };

	public static string UI_WIDGET_NAME = "UIName";

	public static string UI_WIDGET_IDENTIFIER = "UIWidget";

	public static string[] UI_WIDGET_TYPES = new string[5] { "Color", "Vector3", "Vector4", "Float", "Slider" };

	private Matrix _temp_matrix = default(Matrix);

	private Vector2 _temp_vector2 = default(Vector2);

	private Vector3 _temp_vector3 = default(Vector3);

	private Vector4 _temp_vector4 = default(Vector4);

	public Effect effect;

	public List<RenderProc> renderProcs = new List<RenderProc>();

	public int renderProcsCount;

	public int effectPassCount;

	public string data;

	public string dataEffect;

	public string dataPath;

	public bool built;

	public bool useBones;

	public List<EffectParameter> edited = new List<EffectParameter>();

	public EffectParameter effect_Bones;

	public Material(string xMaterialData)
	{
		data = xMaterialData;
		Build();
	}

	public void Build()
	{
		SceneLibrary library = GameEngine.scene.library;
		built = true;
		string[] array = data.Replace(" ", "").Split(':');
		if (array.Length > 0)
		{
			dataEffect = array[0];
			if (dataEffect == null || dataEffect.Length == 0)
			{
				return;
			}
			Effect effect = GameEngine.SceneContent.Load<Effect>("Content/Effects/" + dataEffect);
			this.effect = effect.Clone();
			effectPassCount = this.effect.CurrentTechnique.Passes.Count;
			if (array.Length <= 1)
			{
				return;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			for (int i = 1; i < array.Length; i++)
			{
				string[] array2 = array[i].Split('=');
				if (array2.Length == 2)
				{
					dictionary.Add(array2[0], array2[1]);
				}
			}
			if (dictionary.ContainsKey("Path"))
			{
				dictionary["Path"] = dictionary["Path"].Trim();
				if (dictionary["Path"] != string.Empty)
				{
					dataPath = "Content/" + dictionary["Path"];
				}
				else
				{
					dataPath = string.Empty;
				}
			}
			{
				foreach (EffectParameter parameter in this.effect.Parameters)
				{
					if (parameter.ParameterType.ToString() == "Texture" && parameter.Semantic != null && parameter.Semantic.Length > 1 && parameter.Semantic.Substring(0, Math.Min("TEXTURESTAGE".Length, parameter.Semantic.Length)).ToLower() != "TEXTURESTAGE".ToLower())
					{
						if (library.texture2Ds.ContainsKey(parameter.Semantic))
						{
							parameter.SetValue(library.texture2Ds[parameter.Semantic]);
						}
						else if (library.texture3Ds.ContainsKey(parameter.Semantic))
						{
							parameter.SetValue(library.texture3Ds[parameter.Semantic]);
						}
						else if (library.textureCubes.ContainsKey(parameter.Semantic))
						{
							parameter.SetValue(library.textureCubes[parameter.Semantic]);
						}
						else if (dataPath != null && dataPath != string.Empty)
						{
							try
							{
								string assetName = dataPath + "/" + parameter.Semantic + "_0";
								parameter.SetValue(GameEngine.SceneContent.Load<Texture2D>(assetName));
							}
							catch
							{
								try
								{
									string assetName2 = dataPath + "/" + parameter.Semantic + "_0";
									parameter.SetValue(GameEngine.SceneContent.Load<TextureCube>(assetName2));
								}
								catch
								{
									if (parameter.Semantic != null)
									{
										Console.WriteLine("MaxMaterial Texture Load Fail : " + dataPath + "/" + parameter.Semantic + "_0");
									}
									else
									{
										Console.WriteLine("MaxMaterial Texture Load Fail (No semantic) : " + dataPath + "/");
									}
								}
							}
						}
					}
					if (dictionary.ContainsKey(parameter.Name))
					{
						string[] array3 = dictionary[parameter.Name].Split('|');
						switch (array3.Length)
						{
						case 1:
							parameter.SetValue(MathUtils.FloatSafeParse(array3[0]));
							break;
						case 3:
							_temp_vector3.X = MathUtils.FloatSafeParse(array3[0]);
							_temp_vector3.Y = MathUtils.FloatSafeParse(array3[1]);
							_temp_vector3.Z = MathUtils.FloatSafeParse(array3[2]);
							parameter.SetValue(_temp_vector3);
							break;
						case 4:
							_temp_vector4.X = MathUtils.FloatSafeParse(array3[0]);
							_temp_vector4.Y = MathUtils.FloatSafeParse(array3[1]);
							_temp_vector4.Z = MathUtils.FloatSafeParse(array3[2]);
							_temp_vector4.W = MathUtils.FloatSafeParse(array3[3]);
							parameter.SetValue(_temp_vector4);
							break;
						}
					}
					if (parameter.Name == "Bones")
					{
						useBones = true;
						effect_Bones = parameter;
					}
					if (parameter.Semantic == null)
					{
						continue;
					}
					for (int j = 0; j < RENDER_PARAMS.Length; j++)
					{
						if (RENDER_PARAMS[j] == parameter.Name)
						{
							renderProcs.Add(new RenderProc(j, parameter.Name, parameter));
							renderProcsCount++;
							break;
						}
					}
				}
				return;
			}
		}
		built = false;
	}

	public void Set(Matrix world, Camera camera)
	{
		for (int i = 0; i < renderProcsCount; i++)
		{
			switch (renderProcs[i].type)
			{
			case 0:
				Matrix.Invert(ref world, out _temp_matrix);
				Matrix.Transpose(ref _temp_matrix, out _temp_matrix);
				renderProcs[i].param.SetValue(_temp_matrix);
				break;
			case 1:
				Matrix.Multiply(ref world, ref camera.view, out _temp_matrix);
				Matrix.Multiply(ref _temp_matrix, ref camera.projection, out _temp_matrix);
				renderProcs[i].param.SetValue(_temp_matrix);
				break;
			case 2:
				renderProcs[i].param.SetValue(world);
				break;
			case 3:
				renderProcs[i].param.SetValue(Matrix.Invert(camera.view));
				break;
			case 4:
				renderProcs[i].param.SetValue(camera.view);
				break;
			case 5:
				renderProcs[i].param.SetValue(camera.projection);
				break;
			case 6:
				renderProcs[i].param.SetValue(camera.position);
				break;
			case 7:
				renderProcs[i].param.SetValue(camera.focalLength);
				break;
			}
		}
	}

	public void SetManual(Matrix world, Matrix view, Matrix projection, Vector3 cameraVector)
	{
		for (int i = 0; i < renderProcsCount; i++)
		{
			switch (renderProcs[i].type)
			{
			case 0:
				renderProcs[i].param.SetValue(Matrix.Transpose(Matrix.Invert(world)));
				break;
			case 1:
				renderProcs[i].param.SetValue(Matrix.Multiply(Matrix.Multiply(world, view), projection));
				break;
			case 2:
				renderProcs[i].param.SetValue(world);
				break;
			case 3:
				renderProcs[i].param.SetValue(Matrix.Invert(view));
				break;
			case 4:
				renderProcs[i].param.SetValue(view);
				break;
			case 5:
				renderProcs[i].param.SetValue(projection);
				break;
			case 6:
				renderProcs[i].param.SetValue(cameraVector);
				break;
			}
		}
	}

	public void Dispose()
	{
		renderProcs.Clear();
		renderProcs = null;
		edited.Clear();
		edited = null;
		effect_Bones = null;
		effect.Dispose();
		effect = null;
	}

	public List<MaterialData> ToData(int xIndex, string xPart)
	{
		List<MaterialData> list = new List<MaterialData>();
		for (int i = 0; i < edited.Count; i++)
		{
			if (edited[i] != null)
			{
				list.Add(new MaterialData(xPart, xIndex, edited[i].Name, EffectValueToString(edited[i])));
			}
		}
		return list;
	}

	public void Param_ValueFromString(string xParam, string xValue)
	{
		if (effect.Parameters[xParam] != null)
		{
			Param_ValueFromString(effect.Parameters[xParam], xValue);
		}
	}

	public void Param_ValueFromString(EffectParameter oParam, string xValue)
	{
		switch (oParam.ParameterType)
		{
		case EffectParameterType.String:
			oParam.SetValue(xValue);
			break;
		case EffectParameterType.Single:
			switch (oParam.ParameterClass)
			{
			case EffectParameterClass.Scalar:
				oParam.SetValue(MathUtils.FloatSafeParse(xValue));
				break;
			case EffectParameterClass.Vector:
			{
				xValue = xValue.Replace("{", "");
				xValue = xValue.Replace("}", "");
				xValue = xValue.Replace(" ", "");
				string[] array = xValue.Split(',');
				if (array.Length > 0)
				{
					float[] array2 = new float[array.Length];
					for (int i = 0; i < array2.Length; i++)
					{
						array2[i] = MathUtils.FloatSafeParse(array[i]);
					}
					if (array2.Length >= 2 && oParam.ColumnCount == 2)
					{
						_temp_vector2.X = array2[0];
						_temp_vector2.Y = array2[1];
						oParam.SetValue(_temp_vector2);
					}
					else if (array2.Length >= 3 && oParam.ColumnCount == 3)
					{
						_temp_vector3.X = array2[0];
						_temp_vector3.Y = array2[1];
						_temp_vector3.Z = array2[2];
						oParam.SetValue(_temp_vector3);
					}
					else if (array2.Length >= 4 && oParam.ColumnCount == 4)
					{
						_temp_vector4.X = array2[0];
						_temp_vector4.Y = array2[1];
						_temp_vector4.Z = array2[2];
						_temp_vector4.W = array2[3];
						oParam.SetValue(_temp_vector4);
					}
				}
				break;
			}
			}
			break;
		}
	}

	public void Param_Edited(EffectParameter oParam)
	{
		if (!edited.Contains(oParam))
		{
			edited.Add(oParam);
		}
	}

	public static string EffectValueToString(EffectParameter oParam)
	{
		string result = "";
		if (oParam.ParameterType == EffectParameterType.Single && oParam.ParameterClass == EffectParameterClass.Vector && oParam.ColumnCount == 3)
		{
			Vector3 valueVector = oParam.GetValueVector3();
			return valueVector.X + ", " + valueVector.Y + ", " + valueVector.Z;
		}
		if (oParam.ParameterType == EffectParameterType.Single && oParam.ParameterClass == EffectParameterClass.Vector && oParam.ColumnCount == 4)
		{
			Vector4 valueVector2 = oParam.GetValueVector4();
			return valueVector2.X + ", " + valueVector2.Y + ", " + valueVector2.Z + ", " + valueVector2.W;
		}
		if (oParam.ParameterType == EffectParameterType.Single && oParam.ParameterClass == EffectParameterClass.Vector && oParam.ColumnCount == 2)
		{
			Vector2 valueVector3 = oParam.GetValueVector2();
			return valueVector3.X + ", " + valueVector3.Y;
		}
		if (oParam.ParameterType == EffectParameterType.Single && oParam.ParameterClass == EffectParameterClass.Scalar && oParam.ColumnCount == 1)
		{
			return oParam.GetValueSingle().ToString();
		}
		if (oParam.ParameterType == EffectParameterType.String)
		{
			return oParam.GetValueString();
		}
		return result;
	}

	public static void RenderStates_Set(State oState)
	{
		GraphicsDevice graphicsDevice = GameEngine.Graphics.GraphicsDevice;
		switch (oState)
		{
		case State.Solid:
			graphicsDevice.DepthStencilState = DepthStencilState.Default;
			graphicsDevice.BlendState = BlendState.Opaque;
			graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
			break;
		case State.Alpha:
			graphicsDevice.DepthStencilState = DepthStencilState.Default;
			graphicsDevice.BlendState = BlendState.AlphaBlend;
			graphicsDevice.RasterizerState = RasterizerState.CullNone;
			break;
		case State.AlphaCulled:
			graphicsDevice.DepthStencilState = DepthStencilState.Default;
			graphicsDevice.BlendState = BlendState.AlphaBlend;
			graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
			break;
		case State.AlphaNoDepthWrite:
			graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
			graphicsDevice.BlendState = BlendState.NonPremultiplied;
			graphicsDevice.RasterizerState = RasterizerState.CullNone;
			break;
		case State.AlphaNoDepth:
			graphicsDevice.DepthStencilState = DepthStencilState.None;
			graphicsDevice.BlendState = BlendState.NonPremultiplied;
			graphicsDevice.RasterizerState = RasterizerState.CullNone;
			break;
		case State.Add:
			graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
			graphicsDevice.BlendState = BlendState.Additive;
			graphicsDevice.RasterizerState = RasterizerState.CullNone;
			break;
		case State.Subtract:
			graphicsDevice.DepthStencilState = DepthStencilState.Default;
			graphicsDevice.RasterizerState = RasterizerState.CullNone;
			graphicsDevice.BlendState = BLEND_SUBTRACT;
			break;
		}
	}

	public static void RenderStates_Reset()
	{
		GraphicsDevice graphicsDevice = GameEngine.Graphics.GraphicsDevice;
		graphicsDevice.DepthStencilState = DepthStencilState.Default;
		graphicsDevice.BlendState = BlendState.Opaque;
		graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
	}
}
