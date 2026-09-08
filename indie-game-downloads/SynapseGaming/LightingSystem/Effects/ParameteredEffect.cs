using System;
using System.Collections.Generic;
using _0003;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Collision;
using SynapseGaming.LightingSystem.Core;

namespace SynapseGaming.LightingSystem.Effects;

/// <summary>
/// Base class for SAS, XSI, and other effects with shader driven properties.
/// </summary>
public abstract class ParameteredEffect : Effect, ITransparentEffect, ICollisionMaterial
{
	internal class DA_0018
	{
		internal SystemStatistic _3A_0018 = SystemConsole.GetStatistic("Effect_TechniqueChanges", SystemStatisticCategory.Rendering);

		internal SystemStatistic _3AL = SystemConsole.GetStatistic("Effect_LightSourceChanges", SystemStatisticCategory.Rendering);
	}

	private Texture _3A_0018;

	private int _3AL;

	private float _3A_0019;

	private float _3A3;

	private bool _3A6;

	private TransparencyMode _3AD;

	private float _3A_0017 = 1f;

	private Dictionary<string, object> _3A_0003 = new Dictionary<string, object>();

	private Dictionary<string, Texture> _3Al = new Dictionary<string, Texture>();

	internal DA_0018 _3At = new DA_0018();

	/// <summary>
	/// Amount material absorbs impact force.
	/// </summary>
	public float Elasticity
	{
		get
		{
			return _3A_0019;
		}
		set
		{
			_3A_0019 = value;
			_3AL++;
		}
	}

	/// <summary>
	/// Amount material resists objects moving across its surface.
	/// </summary>
	public float Friction
	{
		get
		{
			return _3A3;
		}
		set
		{
			_3A3 = value;
			_3AL++;
		}
	}

	/// <summary>
	/// Indicates if collision related properties changed. This value increments each time the object
	/// collision properties change.
	/// </summary>
	public int CollisionId => _3AL;

	/// <summary>
	/// Surfaces rendered with the effect should be visible from both sides.
	/// </summary>
	public bool DoubleSided
	{
		get
		{
			return _3A6;
		}
		set
		{
			_3A6 = value;
		}
	}

	/// <summary>
	/// The transparency style used when rendering the effect.
	/// </summary>
	public TransparencyMode TransparencyMode
	{
		get
		{
			return _3AD;
		}
		set
		{
			_3AD = value;
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
			return _3A_0017;
		}
		set
		{
			_3A_0017 = value;
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
			return _3A_0018;
		}
		set
		{
		}
	}

	internal Dictionary<string, object> Properties => _3A_0003;

	internal Dictionary<string, Texture> Textures => _3Al;

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
		_3AD = mode;
		_3A_0017 = threshold;
		_3A_0018 = map;
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
		_3At._3A_0018.AccumulationValue++;
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
			if (_3Al.ContainsKey(name))
			{
				_3Al[name] = texture;
			}
			else
			{
				_3Al.Add(name, texture);
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

	internal void _0019z()
	{
		foreach (KeyValuePair<string, object> item in _3A_0003)
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
				_3A6 = (bool)item.Value;
				continue;
			}
			if (item.Key == "TransparencyMode")
			{
				_3AD = (TransparencyMode)item.Value;
				continue;
			}
			if (item.Key == "Transparency" || item.Key == "TransparencyThreshold")
			{
				_3A_0017 = (float)item.Value;
				continue;
			}
			if (item.Key == "TransparencyMapParameterName")
			{
				EffectParameter effectParameter = base.Parameters[(string)item.Value];
				if (effectParameter != null)
				{
					if (effectParameter.ParameterType == EffectParameterType.Texture2D)
					{
						_3A_0018 = effectParameter.GetValueTexture2D();
					}
					else if (effectParameter.ParameterType == EffectParameterType.Texture3D)
					{
						_3A_0018 = effectParameter.GetValueTexture3D();
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
					effectParameter2.SetValue(_0019_0012(item.Value));
				}
				else if (effectParameter2.ColumnCount == 3)
				{
					effectParameter2.SetValue(_0019r(item.Value));
				}
				else if (effectParameter2.ColumnCount == 4)
				{
					effectParameter2.SetValue(_00195(item.Value));
				}
			}
			else if (effectParameter2.ParameterType == EffectParameterType.Int32)
			{
				effectParameter2.SetValue((int)_0019_0012(item.Value));
			}
		}
		SyncTransparency();
	}

	internal void _0019_0014(Dictionary<string, Texture> P_0)
	{
		foreach (KeyValuePair<string, Texture> item in P_0)
		{
			SetTexture(item.Key, item.Value);
		}
	}

	internal void _0019v(Dictionary<string, object> P_0)
	{
		foreach (KeyValuePair<string, object> item in P_0)
		{
			Type type = item.Value.GetType();
			if ((item.Key == "EffectFile" || item.Key == "Technique" || item.Key == "DepthTechnique" || item.Key == "GBufferTechnique" || item.Key == "FinalTechnique" || item.Key == "ShadowGenerationTechnique" || item.Key == "DoubleSided" || item.Key == "TransparencyMode" || item.Key == "TransparencyMapParameterName") && (object)type == typeof(string))
			{
				object obj = item.Value;
				if (item.Key == "DoubleSided")
				{
					obj = _0019a(obj);
				}
				else if (item.Key == "TransparencyMode")
				{
					obj = _0003._3.L_0019<TransparencyMode>((string)obj);
				}
				if (_3A_0003.ContainsKey(item.Key))
				{
					_3A_0003[item.Key] = obj;
				}
				else
				{
					_3A_0003.Add(item.Key, obj);
				}
			}
			else if ((object)type == typeof(float) && (item.Key == "Transparency" || item.Key == "TransparencyThreshold" || item.Key == "Elasticity" || item.Key == "Friction"))
			{
				if (_3A_0003.ContainsKey(item.Key))
				{
					_3A_0003[item.Key] = (float)item.Value;
				}
				else
				{
					_3A_0003.Add(item.Key, (float)item.Value);
				}
			}
			else if (_3A_0003.ContainsKey(item.Key))
			{
				object obj2 = _3A_0003[item.Key];
				Type type2 = obj2.GetType();
				if ((object)type2 == type)
				{
					obj2 = item.Value;
				}
				else if ((object)type2 == typeof(float))
				{
					obj2 = _0019_0012(item.Value);
				}
				else if ((object)type2 == typeof(Vector3))
				{
					obj2 = _0019r(item.Value);
				}
				else if ((object)type2 == typeof(Vector4))
				{
					obj2 = _00195(item.Value);
				}
				_3A_0003[item.Key] = obj2;
			}
		}
		if (!_3A_0003.ContainsKey("Technique"))
		{
			_3A_0003.Add("Technique", base.CurrentTechnique.Name);
		}
		if (!_3A_0003.ContainsKey("DepthTechnique"))
		{
			_3A_0003.Add("DepthTechnique", _0019C("DepthTechnique"));
		}
		if (!_3A_0003.ContainsKey("GBufferTechnique"))
		{
			_3A_0003.Add("GBufferTechnique", _0019C("GBufferTechnique"));
		}
		if (!_3A_0003.ContainsKey("FinalTechnique"))
		{
			_3A_0003.Add("FinalTechnique", _0019C("FinalTechnique"));
		}
		if (!_3A_0003.ContainsKey("ShadowGenerationTechnique"))
		{
			_3A_0003.Add("ShadowGenerationTechnique", _0019C("ShadowGenerationTechnique"));
		}
		if (!_3A_0003.ContainsKey("Elasticity"))
		{
			_3A_0003.Add("Elasticity", 0.25f);
		}
		if (!_3A_0003.ContainsKey("Friction"))
		{
			_3A_0003.Add("Friction", 0.25f);
		}
		if (!_3A_0003.ContainsKey("DoubleSided"))
		{
			_3A_0003.Add("DoubleSided", false);
		}
		if (!_3A_0003.ContainsKey("TransparencyMode"))
		{
			_3A_0003.Add("TransparencyMode", TransparencyMode.None);
		}
		if (!_3A_0003.ContainsKey("Transparency") && !_3A_0003.ContainsKey("TransparencyThreshold"))
		{
			_3A_0003.Add("TransparencyThreshold", 0.5f);
		}
		if (_3A_0003.ContainsKey("TransparencyMapParameterName"))
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
		_3A_0003.Add("TransparencyMapParameterName", value);
	}

	private string _0019C(string P_0)
	{
		if (base.Techniques[P_0] == null)
		{
			return "";
		}
		return P_0;
	}

	private bool _0019a(object P_0)
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

	private Vector4 _00195(object P_0)
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

	private Vector3 _0019r(object P_0)
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

	private float _0019_0012(object P_0)
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
			if (_3A_0003.ContainsKey(effectParameter.Name) || effectParameter.RowCount > 1 || effectParameter.Elements.Count > 0 || effectParameter.Annotations["SasBindAddress"] != null || !string.IsNullOrEmpty(effectParameter.Semantic))
			{
				continue;
			}
			if (effectParameter.ParameterType == EffectParameterType.Single)
			{
				if (effectParameter.ColumnCount == 0)
				{
					_3A_0003.Add(effectParameter.Name, effectParameter.GetValueSingle());
				}
				else if (effectParameter.ColumnCount == 1 && effectParameter.ParameterClass == EffectParameterClass.Scalar)
				{
					_3A_0003.Add(effectParameter.Name, effectParameter.GetValueSingle());
				}
				else if (effectParameter.ColumnCount == 3)
				{
					_3A_0003.Add(effectParameter.Name, effectParameter.GetValueVector3());
				}
				else if (effectParameter.ColumnCount == 4)
				{
					_3A_0003.Add(effectParameter.Name, effectParameter.GetValueVector4());
				}
			}
			else if (effectParameter.ParameterType == EffectParameterType.Int32 && effectParameter.ColumnCount == 0)
			{
				_3A_0003.Add(effectParameter.Name, effectParameter.GetValueInt32());
			}
			else if (effectParameter.ParameterType == EffectParameterType.Texture2D && effectParameter.ColumnCount == 0)
			{
				_3A_0003.Add(effectParameter.Name, "");
			}
			else if (effectParameter.ParameterType == EffectParameterType.Texture3D && effectParameter.ColumnCount == 0)
			{
				_3A_0003.Add(effectParameter.Name, "");
			}
			else if (effectParameter.ParameterType == EffectParameterType.TextureCube && effectParameter.ColumnCount == 0)
			{
				_3A_0003.Add(effectParameter.Name, "");
			}
			else if (effectParameter.ParameterType == EffectParameterType.Texture && effectParameter.ColumnCount == 0)
			{
				_3A_0003.Add(effectParameter.Name, "");
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
		_0003._6.L_0003(this, effect);
		if (effect is ParameteredEffect parameteredEffect)
		{
			parameteredEffect._0019_0014(_3Al);
			parameteredEffect._0019v(_3A_0003);
			parameteredEffect._0019z();
		}
		return effect;
	}

	/// <summary>
	/// Creates a new empty effect of the same class type and using the same effect file as this object.
	/// </summary>
	/// <returns></returns>
	protected abstract Effect Create();
}
