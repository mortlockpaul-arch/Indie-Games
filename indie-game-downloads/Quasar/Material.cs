using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;
using Quasar.Textures;

namespace Quasar;

public class Material
{
	public enum Priority
	{
		Low = 50,
		Medium = 100,
		High = 200
	}

	public Vector3 Specular = new Vector3(1f, 1f, 1f);

	public float Shininess = 4f;

	public Vector3 Ambient = new Vector3(0.2f, 0.2f, 0.2f);

	public Vector3 Diffuse = new Vector3(1f, 1f, 1f);

	public float Alpha = 1f;

	public float AlphaTest;

	private Priority renderPriority = Priority.Medium;

	private bool forceAlpha;

	private bool forcedAlpha;

	protected List<float> floatParameters;

	protected List<int> intParameters;

	protected List<Vector4> vector4Parameters;

	protected List<Matrix> matrixParameters;

	protected List<Vector4[]> vector4ArrayParameters;

	protected List<Matrix[]> matrixArrayParameters;

	public string Technique;

	protected List<Texture> textures = new List<Texture>(1);

	public Priority RenderPriority
	{
		get
		{
			return renderPriority;
		}
		set
		{
			renderPriority = value;
		}
	}

	public bool IsForcedAlpha => forceAlpha;

	public bool ForcedAlphaValue => forcedAlpha;

	public Vector4 DiffuseWithAlpha
	{
		get
		{
			return new Vector4(Diffuse, Alpha);
		}
		set
		{
			Diffuse = new Vector3(value.X, value.Y, value.Z);
			Alpha = value.W;
		}
	}

	public bool HasAlpha
	{
		get
		{
			if (forceAlpha)
			{
				return forcedAlpha;
			}
			if (Alpha != 1f)
			{
				return true;
			}
			if (textures.Count == 0)
			{
				return false;
			}
			Texture texture = textures[0];
			if (texture != null)
			{
				return texture.Format == SurfaceFormat.Dxt5;
			}
			return false;
		}
	}

	public Vector4 SpecularWithShininess => new Vector4(Specular, Shininess);

	public int FloatParameterCount
	{
		get
		{
			if (floatParameters == null)
			{
				return 0;
			}
			return floatParameters.Count;
		}
	}

	public int IntParameterCount
	{
		get
		{
			if (intParameters == null)
			{
				return 0;
			}
			return intParameters.Count;
		}
	}

	public int Vector4ParameterCount
	{
		get
		{
			if (vector4Parameters == null)
			{
				return 0;
			}
			return vector4Parameters.Count;
		}
	}

	public int MatrixParameterCount
	{
		get
		{
			if (matrixParameters == null)
			{
				return 0;
			}
			return matrixParameters.Count;
		}
	}

	public int Vector4ArrayParameterCount
	{
		get
		{
			if (vector4ArrayParameters == null)
			{
				return 0;
			}
			return vector4ArrayParameters.Count;
		}
	}

	public int MatrixArrayParameterCount
	{
		get
		{
			if (matrixArrayParameters == null)
			{
				return 0;
			}
			return matrixArrayParameters.Count;
		}
	}

	public List<Texture> Textures => textures;

	public Texture Texture
	{
		get
		{
			if (textures.Count > 0)
			{
				return textures[0];
			}
			return null;
		}
		set
		{
			if (textures.Count == 0)
			{
				textures.Add(value);
			}
			else
			{
				textures[0] = value;
			}
		}
	}

	public void SetForcedAlpha(bool alpha)
	{
		forceAlpha = true;
		forcedAlpha = alpha;
	}

	public void ClearForcedAlpha()
	{
		forceAlpha = false;
	}

	public void AddFloatParameter(float v)
	{
		if (floatParameters == null)
		{
			floatParameters = new List<float>(1);
		}
		floatParameters.Add(v);
	}

	public void SetFloatParameter(int index, float v)
	{
		if (floatParameters == null)
		{
			floatParameters = new List<float>(1);
		}
		for (int i = floatParameters.Count; i <= index; i++)
		{
			floatParameters.Add(0f);
		}
		floatParameters[index] = v;
	}

	public void RemoveFloatParameter(int index)
	{
		if (floatParameters != null)
		{
			floatParameters.RemoveAt(index);
		}
	}

	public float GetFloatParameter(int index)
	{
		if (floatParameters == null)
		{
			return 0f;
		}
		return floatParameters[index];
	}

	public void SetFloatShaderParameter(EffectParameter parameter, int index)
	{
		if (floatParameters != null && floatParameters.Count > index)
		{
			parameter.SetValue(floatParameters[index]);
		}
	}

	public void AddIntParameter(int v)
	{
		if (intParameters == null)
		{
			intParameters = new List<int>(1);
		}
		intParameters.Add(v);
	}

	public void SetIntParameter(int index, int v)
	{
		if (intParameters == null)
		{
			intParameters = new List<int>(1);
		}
		for (int i = intParameters.Count; i <= index; i++)
		{
			intParameters.Add(0);
		}
		intParameters[index] = v;
	}

	public void RemoveIntParameter(int index)
	{
		if (intParameters != null)
		{
			intParameters.RemoveAt(index);
		}
	}

	public int GetIntParameter(int index)
	{
		if (intParameters == null)
		{
			return 0;
		}
		return intParameters[index];
	}

	public void SetIntShaderParameter(EffectParameter parameter, int index)
	{
		if (intParameters != null && intParameters.Count > index)
		{
			parameter.SetValue(intParameters[index]);
		}
	}

	public void AddVector4Parameter(Vector4 v)
	{
		if (vector4Parameters == null)
		{
			vector4Parameters = new List<Vector4>(1);
		}
		vector4Parameters.Add(v);
	}

	public void SetVector4Parameter(int index, Vector4 v)
	{
		if (vector4Parameters == null)
		{
			vector4Parameters = new List<Vector4>(1);
		}
		for (int i = vector4Parameters.Count; i <= index; i++)
		{
			vector4Parameters.Add(Vector4.Zero);
		}
		vector4Parameters[index] = v;
	}

	public void RemoveVector4Parameter(int index)
	{
		if (vector4Parameters != null)
		{
			vector4Parameters.RemoveAt(index);
		}
	}

	public Vector4 GetVector4Parameter(int index)
	{
		if (vector4Parameters == null)
		{
			return Vector4.Zero;
		}
		return vector4Parameters[index];
	}

	public void SetVector4ShaderParameter(EffectParameter parameter, int index)
	{
		if (vector4Parameters != null && vector4Parameters.Count > index)
		{
			parameter.SetValue(vector4Parameters[index]);
		}
	}

	public void AddMatrixParameter(Matrix m)
	{
		if (matrixParameters == null)
		{
			matrixParameters = new List<Matrix>(1);
		}
		matrixParameters.Add(m);
	}

	public void SetMatrixParameter(int index, Matrix m)
	{
		if (matrixParameters == null)
		{
			matrixParameters = new List<Matrix>(1);
		}
		for (int i = matrixParameters.Count; i <= index; i++)
		{
			matrixParameters.Add(Matrix.Identity);
		}
		matrixParameters[index] = m;
	}

	public Matrix GetMatrixParameter(int index)
	{
		if (matrixParameters == null)
		{
			return Matrix.Identity;
		}
		return matrixParameters[index];
	}

	public void SetMatrixShaderParameter(EffectParameter parameter, int index)
	{
		if (matrixParameters != null && matrixParameters.Count > index)
		{
			parameter.SetValue(matrixParameters[index]);
		}
	}

	public void AddVector4ArrayParameter(Vector4[] v)
	{
		if (vector4ArrayParameters == null)
		{
			vector4ArrayParameters = new List<Vector4[]>(1);
		}
		vector4ArrayParameters.Add(v);
	}

	public void SetVector4ArrayParameter(int index, Vector4[] v)
	{
		if (vector4ArrayParameters == null)
		{
			vector4ArrayParameters = new List<Vector4[]>(1);
		}
		for (int i = vector4ArrayParameters.Count; i <= index; i++)
		{
			vector4ArrayParameters.Add(null);
		}
		vector4ArrayParameters[index] = v;
	}

	public Vector4[] GetVector4ArrayParameter(int index)
	{
		if (vector4ArrayParameters == null)
		{
			return null;
		}
		return vector4ArrayParameters[index];
	}

	public void SetVector4ArrayShaderParameter(EffectParameter parameter, int index)
	{
		if (vector4ArrayParameters != null && vector4ArrayParameters.Count > index)
		{
			parameter.SetValue(vector4ArrayParameters[index]);
		}
	}

	public void AddMatrixArrayParameter(Matrix[] m)
	{
		if (matrixArrayParameters == null)
		{
			matrixArrayParameters = new List<Matrix[]>(1);
		}
		matrixArrayParameters.Add(m);
	}

	public void SetMatrixArrayParameter(int index, Matrix[] m)
	{
		if (matrixArrayParameters == null)
		{
			matrixArrayParameters = new List<Matrix[]>(1);
		}
		for (int i = matrixArrayParameters.Count; i <= index; i++)
		{
			matrixArrayParameters.Add(null);
		}
		matrixArrayParameters[index] = m;
	}

	public Matrix[] GetMatrixArrayParameter(int index)
	{
		if (matrixArrayParameters == null)
		{
			return null;
		}
		return matrixArrayParameters[index];
	}

	public void SetMatrixArrayShaderParameter(EffectParameter parameter, int index)
	{
		if (matrixArrayParameters != null && matrixArrayParameters.Count > index)
		{
			parameter.SetValue(matrixArrayParameters[index]);
		}
	}

	public void SetTexture(int index, Texture texture)
	{
		for (int i = textures.Count; i <= index; i++)
		{
			textures.Add(null);
		}
		textures[index] = texture;
	}

	public Material()
	{
	}

	public Material(ModelMeshPart mmp)
	{
		if ((object)mmp.Effect.GetType() == typeof(BasicEffect))
		{
			BasicEffect basicEffect = (BasicEffect)mmp.Effect;
			Ambient = basicEffect.AmbientLightColor;
			Diffuse = basicEffect.DiffuseColor;
			Alpha = basicEffect.Alpha;
			Specular = basicEffect.SpecularColor;
			Shininess = basicEffect.SpecularPower;
			if (basicEffect.Texture != null)
			{
				textures.Add(basicEffect.Texture);
			}
		}
	}

	public void FromXml(XElement xe, string basePath)
	{
		Alpha = XDocHelper.ParseFloatAttribute(xe, "alpha", Alpha);
		AlphaTest = XDocHelper.ParseFloatAttribute(xe, "alphaTest", AlphaTest);
		Ambient = XDocHelper.ParseColor3Attribute(xe, "ambient", Ambient);
		Diffuse = XDocHelper.ParseColor3Attribute(xe, "diffuse", Diffuse);
		Specular = XDocHelper.ParseColor3Attribute(xe, "specular", Specular);
		Shininess = XDocHelper.ParseFloatAttribute(xe, "shininess", Shininess);
		int num = 0;
		foreach (XElement item in xe.Elements("FloatParameter"))
		{
			if (FloatParameterCount > num)
			{
				if (XDocHelper.HasAttribute(item, "value"))
				{
					SetFloatParameter(num, XDocHelper.ParseFloatAttribute(item, "value"));
				}
			}
			else
			{
				AddFloatParameter(XDocHelper.ParseFloatAttribute(item, "value"));
			}
			num++;
		}
		forceAlpha = XDocHelper.ParseBoolAttribute(xe, "forceAlpha", defaultValue: false);
		forcedAlpha = XDocHelper.ParseBoolAttribute(xe, "forcedAlphaValue", defaultValue: true);
		num = 0;
		foreach (XElement item2 in xe.Elements("IntParameter"))
		{
			if (IntParameterCount > num)
			{
				if (XDocHelper.HasAttribute(item2, "value"))
				{
					SetIntParameter(num, XDocHelper.ParseIntAttribute(item2, "value"));
				}
			}
			else
			{
				AddIntParameter(XDocHelper.ParseIntAttribute(item2, "value"));
			}
			num++;
		}
		renderPriority = (Priority)XDocHelper.ParseIntAttribute(xe, "priority", 100);
		Technique = XDocHelper.GetAttribute(xe, "technique", null);
		num = 0;
		foreach (XElement item3 in xe.Elements("Texture"))
		{
			bool flag = XDocHelper.ParseBoolAttribute(item3, "isCubemap");
			if (XDocHelper.HasAttribute(item3, "source"))
			{
				if (flag)
				{
					if (textures.Count > num)
					{
						textures[num] = CubeTextureManager.LoadTexture(Engine.ProcessPath(basePath, XDocHelper.GetAttribute(item3, "source")));
					}
					else
					{
						textures.Add(CubeTextureManager.LoadTexture(Engine.ProcessPath(basePath, XDocHelper.GetAttribute(item3, "source"))));
					}
				}
				else if (textures.Count > num)
				{
					textures[num] = TextureManager.LoadTexture(Engine.ProcessPath(basePath, XDocHelper.GetAttribute(item3, "source")));
				}
				else
				{
					textures.Add(TextureManager.LoadTexture(Engine.ProcessPath(basePath, XDocHelper.GetAttribute(item3, "source"))));
				}
			}
			num++;
		}
		num = 0;
		foreach (XElement item4 in xe.Elements("Vector4Parameter"))
		{
			if (Vector4ParameterCount > num)
			{
				if (XDocHelper.HasAttribute(item4, "source"))
				{
					SetVector4Parameter(num, XDocHelper.ParseVector4Attribute(item4, "value"));
				}
			}
			else
			{
				AddVector4Parameter(XDocHelper.ParseVector4Attribute(item4, "value"));
			}
			num++;
		}
	}

	public Material Clone()
	{
		Material material = new Material();
		material.Alpha = Alpha;
		material.AlphaTest = AlphaTest;
		material.Ambient = Ambient;
		material.Diffuse = Diffuse;
		if (floatParameters != null)
		{
			material.floatParameters = new List<float>(floatParameters);
		}
		material.forceAlpha = forceAlpha;
		material.forcedAlpha = forcedAlpha;
		if (intParameters != null)
		{
			material.intParameters = new List<int>(intParameters);
		}
		if (matrixArrayParameters != null)
		{
			material.matrixArrayParameters = new List<Matrix[]>(matrixArrayParameters);
		}
		if (matrixParameters != null)
		{
			material.matrixParameters = new List<Matrix>(matrixParameters);
		}
		material.renderPriority = renderPriority;
		material.Shininess = Shininess;
		material.Specular = Specular;
		material.Technique = Technique;
		material.textures.AddRange(textures);
		if (vector4ArrayParameters != null)
		{
			material.vector4ArrayParameters = new List<Vector4[]>(vector4ArrayParameters);
		}
		if (vector4Parameters != null)
		{
			material.vector4Parameters = new List<Vector4>(vector4Parameters);
		}
		return material;
	}
}
