using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Collision;
using SynapseGaming.LightingSystem.Core;
using Z;

namespace SynapseGaming.LightingSystem.Effects;

/// <summary>
/// Base class for SAS, XSI, and other effects with shader driven properties.
/// </summary>
public abstract class ParameteredEffect : Effect, ITransparentEffect, ICollisionMaterial
{
	internal class _0001CB
	{
		internal SystemStatistic HCB = SystemConsole.GetStatistic("Effect_TechniqueChanges", SystemStatisticCategory.Rendering);

		internal SystemStatistic HC_0002 = SystemConsole.GetStatistic("Effect_LightSourceChanges", SystemStatisticCategory.Rendering);
	}

	private Texture HCB;

	private int HC_0002;

	private float HC_0012;

	private float HCH;

	private bool HC7;

	private TransparencyMode HC_0001;

	private float HCw = 1f;

	private Dictionary<string, object> HCZ = new Dictionary<string, object>();

	private Dictionary<string, Texture> HC_000F = new Dictionary<string, Texture>();

	internal _0001CB HCy = new _0001CB();

	/// <summary>
	/// Amount material absorbs impact force.
	/// </summary>
	public float Elasticity
	{
		get
		{
			return HC_0012;
		}
		set
		{
			HC_0012 = value;
			HC_0002++;
		}
	}

	/// <summary>
	/// Amount material resists objects moving across its surface.
	/// </summary>
	public float Friction
	{
		get
		{
			return HCH;
		}
		set
		{
			HCH = value;
			HC_0002++;
		}
	}

	/// <summary>
	/// Indicates if collision related properties changed. This value increments each time the object
	/// collision properties change.
	/// </summary>
	public int CollisionId => HC_0002;

	/// <summary>
	/// Surfaces rendered with the effect should be visible from both sides.
	/// </summary>
	public bool DoubleSided
	{
		get
		{
			return HC7;
		}
		set
		{
			HC7 = value;
		}
	}

	/// <summary>
	/// The transparency style used when rendering the effect.
	/// </summary>
	public TransparencyMode TransparencyMode
	{
		get
		{
			return HC_0001;
		}
		set
		{
			HC_0001 = value;
			SyncTransparency();
		}
	}

	/// <summary>
	/// Used with TransparencyMode to determine the effect clipped transparency.
	///   -For Clip mode this value is a comparison value, where all TransparencyMap
	///    alpha values below the value are *not* rendered.
	///   -For Blend and Additive mode this value is a comparison value for the *shadow*
	///    transparency, where all TransparencyMap alpha values below the value are
	///    *not* rendered.
	/// </summary>
	public float TransparencyThreshold
	{
		get
		{
			return HCw;
		}
		set
		{
			HCw = value;
			SyncTransparency();
		}
	}

	/// <summary>
	/// The texture map used for transparency (values are pulled from the alpha channel).
	/// </summary>
	public Texture TransparencyMap
	{
		get
		{
			return HCB;
		}
		set
		{
		}
	}

	internal Dictionary<string, object> Properties => HCZ;

	internal Dictionary<string, Texture> Textures => HC_000F;

	/// <summary>
	/// Sets all transparency information at once.  Used to improve performance
	/// by avoiding multiple effect technique changes.
	/// </summary>
	/// <param name="mode">The transparency style used when rendering the effect.</param>
	/// <param name="threshold">Used with TransparencyMode to determine the effect transparency.
	///   -For Clip mode this value is a comparison value, where all TransparencyMap
	///    alpha values below the value are *not* rendered.
	///   -For Blend and Additive mode this value is a comparison value for the shadow
	///    transparency, where all TransparencyMap alpha values below the value are
	///    *not* rendered.</param>
	/// <param name="map">The texture map used for transparency (values are pulled from the alpha channel).</param>
	public void SetTransparencyModeAndMap(TransparencyMode mode, float threshold, Texture map)
	{
		HC_0001 = mode;
		HCw = threshold;
		HCB = map;
		SyncTransparency();
	}

	/// <summary>
	/// Applies the object's transparency information to its effect parameters.
	/// </summary>
	protected virtual void SyncTransparency()
	{
	}

	/// <summary>
	/// Sets the effect technique by name.
	/// </summary>
	public void SetTechnique(string techniquename)
	{
		HCy.HCB.AccumulationValue++;
		EffectTechnique effectTechnique = base.Techniques[techniquename];
		if (effectTechnique != null)
		{
			base.CurrentTechnique = effectTechnique;
		}
	}

	/// <summary>
	/// Sets the effect texture by name.
	/// </summary>
	public void SetTexture(string name, Texture texture)
	{
		EffectParameter effectParameter = base.Parameters[name];
		if (effectParameter != null)
		{
			if (HC_000F.ContainsKey(name))
			{
				HC_000F[name] = texture;
			}
			else
			{
				HC_000F.Add(name, texture);
			}
			if (effectParameter.ParameterType == EffectParameterType.Texture2D && texture is Texture2D)
			{
				effectParameter.SetValue((Texture2D)texture);
			}
			else if (effectParameter.ParameterType == EffectParameterType.Texture3D && texture is Texture3D)
			{
				effectParameter.SetValue((Texture3D)texture);
			}
			else if (effectParameter.ParameterType == EffectParameterType.TextureCube && texture is TextureCube)
			{
				effectParameter.SetValue((TextureCube)texture);
			}
			else if (effectParameter.ParameterType == EffectParameterType.Texture)
			{
				effectParameter.SetValue(texture);
			}
		}
	}

	internal void _0012_0014()
	{
		foreach (KeyValuePair<string, object> item in HCZ)
		{
			if (item.Key == "EffectFile")
			{
				continue;
			}
			if (item.Key == "Technique")
			{
				SetTechnique((string)item.Value);
				continue;
			}
			if (item.Key == "Elasticity")
			{
				Elasticity = (float)item.Value;
				continue;
			}
			if (item.Key == "Friction")
			{
				Friction = (float)item.Value;
				continue;
			}
			if (item.Key == "DoubleSided")
			{
				HC7 = (bool)item.Value;
				continue;
			}
			if (item.Key == "TransparencyMode")
			{
				HC_0001 = (TransparencyMode)item.Value;
				continue;
			}
			if (item.Key == "Transparency" || item.Key == "TransparencyThreshold")
			{
				HCw = (float)item.Value;
				continue;
			}
			if (item.Key == "TransparencyMapParameterName")
			{
				EffectParameter effectParameter = base.Parameters[(string)item.Value];
				if (effectParameter != null)
				{
					if (effectParameter.ParameterType == EffectParameterType.Texture2D)
					{
						HCB = effectParameter.GetValueTexture2D();
					}
					else if (effectParameter.ParameterType == EffectParameterType.Texture3D)
					{
						HCB = effectParameter.GetValueTexture3D();
					}
				}
				continue;
			}
			EffectParameter effectParameter2 = base.Parameters[item.Key];
			if (effectParameter2 == null || (effectParameter2.ParameterType != EffectParameterType.Single && effectParameter2.ParameterType != EffectParameterType.Int32))
			{
				continue;
			}
			if (effectParameter2.ParameterType == EffectParameterType.Single)
			{
				if (effectParameter2.ColumnCount == 1)
				{
					effectParameter2.SetValue(_0012J(item.Value));
				}
				else if (effectParameter2.ColumnCount == 3)
				{
					effectParameter2.SetValue(_0012r(item.Value));
				}
				else if (effectParameter2.ColumnCount == 4)
				{
					effectParameter2.SetValue(_0012S(item.Value));
				}
			}
			else if (effectParameter2.ParameterType == EffectParameterType.Int32)
			{
				effectParameter2.SetValue((int)_0012J(item.Value));
			}
		}
		SyncTransparency();
	}

	internal void _0012L(Dictionary<string, Texture> P_0)
	{
		foreach (KeyValuePair<string, Texture> item in P_0)
		{
			SetTexture(item.Key, item.Value);
		}
	}

	internal void _0012h(Dictionary<string, object> P_0)
	{
		foreach (KeyValuePair<string, object> item in P_0)
		{
			Type type = item.Value.GetType();
			if ((item.Key == "EffectFile" || item.Key == "Technique" || item.Key == "DepthTechnique" || item.Key == "GBufferTechnique" || item.Key == "FinalTechnique" || item.Key == "ShadowGenerationTechnique" || item.Key == "DoubleSided" || item.Key == "TransparencyMode" || item.Key == "TransparencyMapParameterName") && (object)type == typeof(string))
			{
				object obj = item.Value;
				if (item.Key == "DoubleSided")
				{
					obj = _0012a(obj);
				}
				else if (item.Key == "TransparencyMode")
				{
					obj = Z.H._0002_0002<TransparencyMode>((string)obj);
				}
				if (HCZ.ContainsKey(item.Key))
				{
					HCZ[item.Key] = obj;
				}
				else
				{
					HCZ.Add(item.Key, obj);
				}
			}
			else if ((object)type == typeof(float) && (item.Key == "Transparency" || item.Key == "TransparencyThreshold" || item.Key == "Elasticity" || item.Key == "Friction"))
			{
				if (HCZ.ContainsKey(item.Key))
				{
					HCZ[item.Key] = (float)item.Value;
				}
				else
				{
					HCZ.Add(item.Key, (float)item.Value);
				}
			}
			else if (HCZ.ContainsKey(item.Key))
			{
				object obj2 = HCZ[item.Key];
				Type type2 = obj2.GetType();
				if ((object)type2 == type)
				{
					obj2 = item.Value;
				}
				else if ((object)type2 == typeof(float))
				{
					obj2 = _0012J(item.Value);
				}
				else if ((object)type2 == typeof(Vector3))
				{
					obj2 = _0012r(item.Value);
				}
				else if ((object)type2 == typeof(Vector4))
				{
					obj2 = _0012S(item.Value);
				}
				HCZ[item.Key] = obj2;
			}
		}
		if (!HCZ.ContainsKey("Technique"))
		{
			HCZ.Add("Technique", base.CurrentTechnique.Name);
		}
		if (!HCZ.ContainsKey("DepthTechnique"))
		{
			HCZ.Add("DepthTechnique", _0012T("DepthTechnique"));
		}
		if (!HCZ.ContainsKey("GBufferTechnique"))
		{
			HCZ.Add("GBufferTechnique", _0012T("GBufferTechnique"));
		}
		if (!HCZ.ContainsKey("FinalTechnique"))
		{
			HCZ.Add("FinalTechnique", _0012T("FinalTechnique"));
		}
		if (!HCZ.ContainsKey("ShadowGenerationTechnique"))
		{
			HCZ.Add("ShadowGenerationTechnique", _0012T("ShadowGenerationTechnique"));
		}
		if (!HCZ.ContainsKey("Elasticity"))
		{
			HCZ.Add("Elasticity", 0.25f);
		}
		if (!HCZ.ContainsKey("Friction"))
		{
			HCZ.Add("Friction", 0.25f);
		}
		if (!HCZ.ContainsKey("DoubleSided"))
		{
			HCZ.Add("DoubleSided", false);
		}
		if (!HCZ.ContainsKey("TransparencyMode"))
		{
			HCZ.Add("TransparencyMode", TransparencyMode.None);
		}
		if (!HCZ.ContainsKey("Transparency") && !HCZ.ContainsKey("TransparencyThreshold"))
		{
			HCZ.Add("TransparencyThreshold", 0.5f);
		}
		if (HCZ.ContainsKey("TransparencyMapParameterName"))
		{
			return;
		}
		string value = "";
		for (int i = 0; i < base.Parameters.Count; i++)
		{
			EffectParameter effectParameter = base.Parameters[i];
			if ((effectParameter.ParameterType == EffectParameterType.Texture || effectParameter.ParameterType == EffectParameterType.Texture2D || effectParameter.ParameterType == EffectParameterType.Texture3D) && !string.IsNullOrEmpty(effectParameter.Name))
			{
				value = effectParameter.Name;
				break;
			}
		}
		HCZ.Add("TransparencyMapParameterName", value);
	}

	private string _0012T(string P_0)
	{
		if (base.Techniques[P_0] == null)
		{
			return "";
		}
		return P_0;
	}

	private bool _0012a(object P_0)
	{
		if (P_0 is bool)
		{
			return (bool)P_0;
		}
		if (P_0 is string)
		{
			try
			{
				return bool.Parse((string)P_0);
			}
			catch
			{
			}
		}
		return false;
	}

	private Vector4 _0012S(object P_0)
	{
		if (P_0 is Vector4)
		{
			return (Vector4)P_0;
		}
		if (P_0 is Vector3)
		{
			return new Vector4((Vector3)P_0, 1f);
		}
		if (P_0 is float)
		{
			return new Vector4((float)P_0);
		}
		return default(Vector4);
	}

	private Vector3 _0012r(object P_0)
	{
		if (P_0 is Vector4 vector)
		{
			return new Vector3(vector.X, vector.Y, vector.Z);
		}
		if (P_0 is Vector3)
		{
			return (Vector3)P_0;
		}
		if (P_0 is float)
		{
			return new Vector3((float)P_0);
		}
		return default(Vector3);
	}

	private float _0012J(object P_0)
	{
		if (P_0 is Vector4)
		{
			return ((Vector4)P_0).X;
		}
		if (P_0 is Vector3)
		{
			return ((Vector3)P_0).X;
		}
		if (P_0 is float)
		{
			return (float)P_0;
		}
		return 0f;
	}

	internal ParameteredEffect(GraphicsDevice P_0, byte[] P_1)
		: base(P_0, P_1)
	{
		for (int i = 0; i < base.Parameters.Count; i++)
		{
			EffectParameter effectParameter = base.Parameters[i];
			if (HCZ.ContainsKey(effectParameter.Name) || effectParameter.RowCount > 1 || effectParameter.Elements.Count > 0 || effectParameter.Annotations["SasBindAddress"] != null || !string.IsNullOrEmpty(effectParameter.Semantic))
			{
				continue;
			}
			if (effectParameter.ParameterType == EffectParameterType.Single)
			{
				if (effectParameter.ColumnCount == 0)
				{
					HCZ.Add(effectParameter.Name, effectParameter.GetValueSingle());
				}
				else if (effectParameter.ColumnCount == 1 && effectParameter.ParameterClass == EffectParameterClass.Scalar)
				{
					HCZ.Add(effectParameter.Name, effectParameter.GetValueSingle());
				}
				else if (effectParameter.ColumnCount == 3)
				{
					HCZ.Add(effectParameter.Name, effectParameter.GetValueVector3());
				}
				else if (effectParameter.ColumnCount == 4)
				{
					HCZ.Add(effectParameter.Name, effectParameter.GetValueVector4());
				}
			}
			else if (effectParameter.ParameterType == EffectParameterType.Int32 && effectParameter.ColumnCount == 0)
			{
				HCZ.Add(effectParameter.Name, effectParameter.GetValueInt32());
			}
			else if (effectParameter.ParameterType == EffectParameterType.Texture2D && effectParameter.ColumnCount == 0)
			{
				HCZ.Add(effectParameter.Name, "");
			}
			else if (effectParameter.ParameterType == EffectParameterType.Texture3D && effectParameter.ColumnCount == 0)
			{
				HCZ.Add(effectParameter.Name, "");
			}
			else if (effectParameter.ParameterType == EffectParameterType.TextureCube && effectParameter.ColumnCount == 0)
			{
				HCZ.Add(effectParameter.Name, "");
			}
			else if (effectParameter.ParameterType == EffectParameterType.Texture && effectParameter.ColumnCount == 0)
			{
				HCZ.Add(effectParameter.Name, "");
			}
		}
	}

	/// <summary>
	/// Creates a new effect of the same class type, with the same property values, and using the same effect file as this object.
	/// </summary>
	/// <returns></returns>
	public override Effect Clone()
	{
		Effect effect = Create();
		Z._7._0002w(this, effect);
		if (effect is ParameteredEffect parameteredEffect)
		{
			parameteredEffect._0012L(HC_000F);
			parameteredEffect._0012h(HCZ);
			parameteredEffect._0012_0014();
		}
		return effect;
	}

	/// <summary>
	/// Creates a new empty effect of the same class type and using the same effect file as this object.
	/// </summary>
	/// <returns></returns>
	protected abstract Effect Create();
}
