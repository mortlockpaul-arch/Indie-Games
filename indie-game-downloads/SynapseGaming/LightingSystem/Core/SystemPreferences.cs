using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using SynapseGaming.LightingSystem.Serialization;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Provides user and hardware specific preferences to the Lighting System.
/// </summary>
[Serializable]
public class SystemPreferences : ISystemPreferences, IPreferences, IFullSerializable, ISerializable
{
	private SamplingPreference _3A_0018 = SamplingPreference.Trilinear;

	private int _3AL = 4;

	private DetailPreference _3A_0019 = DetailPreference.Medium;

	private float _3A3 = 1f;

	private DetailPreference _3A6;

	private DetailPreference _3AD;

	private DetailPreference _3A_0017;

	/// <summary>
	/// Sets the user preferred balance of texture sampling quality and performance.
	/// </summary>
	public SamplingPreference TextureSampling
	{
		get
		{
			return _3A_0018;
		}
		set
		{
			_3A_0018 = value;
		}
	}

	/// <summary>
	/// Sets the maximum anisotropy level when TextureSampling is set to Anisotropic.
	/// </summary>
	public int MaxAnisotropy
	{
		get
		{
			return _3AL;
		}
		set
		{
			_3AL = value;
		}
	}

	/// <summary>
	/// Sets the user preferred balance of shadow filtering quality and performance.
	/// </summary>
	public DetailPreference ShadowDetail
	{
		get
		{
			return _3A_0019;
		}
		set
		{
			_3A_0019 = value;
		}
	}

	/// <summary>
	/// Sets the user preferred balance of shadow resolution and performance.
	/// </summary>
	public float ShadowQuality
	{
		get
		{
			return _3A3;
		}
		set
		{
			_3A3 = value;
		}
	}

	/// <summary>
	/// Sets the user preferred balance of LightingEffect detail and performance.
	/// </summary>
	public DetailPreference EffectDetail
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
	/// Sets the user preferred balance of lighting detail and performance.
	/// </summary>
	public DetailPreference LightingDetail
	{
		get
		{
			return _3AD;
		}
		set
		{
			_3AD = value;
		}
	}

	/// <summary>
	/// Sets the user preferred balance of post-processing effect detail and performance.
	/// </summary>
	public DetailPreference PostProcessingDetail
	{
		get
		{
			return _3A_0017;
		}
		set
		{
			_3A_0017 = value;
		}
	}

	/// <summary>
	/// Creates a new SystemPreferences object.
	/// </summary>
	public SystemPreferences()
	{
	}

	/// <summary>
	/// Deserializes object data from the provided SerializationInfo.
	/// </summary>
	/// <param name="info">Contains the serialized object data.</param>
	/// <param name="context"></param>
	public void SetObjectData(SerializationInfo info, StreamingContext context)
	{
		SerializationHelper.DeserializeEnum(ref _3A_0018, info, "TextureSampling", isflag: false);
		SerializationHelper.DeserializeField(ref _3AL, info, "MaxAnisotropy", usedefault: true);
		SerializationHelper.DeserializeEnum(ref _3A_0019, info, "ShadowDetail", isflag: false);
		SerializationHelper.DeserializeField(ref _3A3, info, "ShadowQuality", usedefault: true);
		SerializationHelper.DeserializeEnum(ref _3A6, info, "EffectDetail", isflag: false);
		SerializationHelper.DeserializeEnum(ref _3AD, info, "LightingDetail", isflag: false);
		SerializationHelper.DeserializeEnum(ref _3A_0017, info, "PostProcessingDetail", isflag: false);
	}

	/// <summary>
	/// Serializes object data to the provided SerializationInfo.
	/// </summary>
	/// <param name="info">SerializationInfo to store the serialized data.</param>
	/// <param name="context"></param>
	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("TextureSampling", _3A_0018);
		info.AddValue("MaxAnisotropy", _3AL);
		info.AddValue("ShadowDetail", _3A_0019);
		info.AddValue("ShadowQuality", _3A3);
		info.AddValue("EffectDetail", _3A6);
		info.AddValue("LightingDetail", _3AD);
		info.AddValue("PostProcessingDetail", _3A_0017);
	}
}
