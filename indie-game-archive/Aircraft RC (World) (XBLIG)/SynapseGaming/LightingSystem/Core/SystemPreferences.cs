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
	private SamplingPreference HCB = SamplingPreference.Trilinear;

	private int HC_0002 = 4;

	private DetailPreference HC_0012 = DetailPreference.Medium;

	private float HCH = 1f;

	private DetailPreference HC7;

	private DetailPreference HC_0001;

	private DetailPreference HCw;

	/// <summary>
	/// Sets the user preferred balance of texture sampling quality and performance.
	/// </summary>
	public SamplingPreference TextureSampling
	{
		get
		{
			return HCB;
		}
		set
		{
			HCB = value;
		}
	}

	/// <summary>
	/// Sets the maximum anisotropy level when TextureSampling is set to Anisotropic.
	/// </summary>
	public int MaxAnisotropy
	{
		get
		{
			return HC_0002;
		}
		set
		{
			HC_0002 = value;
		}
	}

	/// <summary>
	/// Sets the user preferred balance of shadow filtering quality and performance.
	/// </summary>
	public DetailPreference ShadowDetail
	{
		get
		{
			return HC_0012;
		}
		set
		{
			HC_0012 = value;
		}
	}

	/// <summary>
	/// Sets the user preferred balance of shadow resolution and performance.
	/// </summary>
	public float ShadowQuality
	{
		get
		{
			return HCH;
		}
		set
		{
			HCH = value;
		}
	}

	/// <summary>
	/// Sets the user preferred balance of LightingEffect detail and performance.
	/// </summary>
	public DetailPreference EffectDetail
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
	/// Sets the user preferred balance of lighting detail and performance.
	/// </summary>
	public DetailPreference LightingDetail
	{
		get
		{
			return HC_0001;
		}
		set
		{
			HC_0001 = value;
		}
	}

	/// <summary>
	/// Sets the user preferred balance of post-processing effect detail and performance.
	/// </summary>
	public DetailPreference PostProcessingDetail
	{
		get
		{
			return HCw;
		}
		set
		{
			HCw = value;
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
		SerializationHelper.DeserializeEnum(ref HCB, info, "TextureSampling", isflag: false);
		SerializationHelper.DeserializeField(ref HC_0002, info, "MaxAnisotropy", usedefault: true);
		SerializationHelper.DeserializeEnum(ref HC_0012, info, "ShadowDetail", isflag: false);
		SerializationHelper.DeserializeField(ref HCH, info, "ShadowQuality", usedefault: true);
		SerializationHelper.DeserializeEnum(ref HC7, info, "EffectDetail", isflag: false);
		SerializationHelper.DeserializeEnum(ref HC_0001, info, "LightingDetail", isflag: false);
		SerializationHelper.DeserializeEnum(ref HCw, info, "PostProcessingDetail", isflag: false);
	}

	/// <summary>
	/// Serializes object data to the provided SerializationInfo.
	/// </summary>
	/// <param name="info">SerializationInfo to store the serialized data.</param>
	/// <param name="context"></param>
	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("TextureSampling", HCB);
		info.AddValue("MaxAnisotropy", HC_0002);
		info.AddValue("ShadowDetail", HC_0012);
		info.AddValue("ShadowQuality", HCH);
		info.AddValue("EffectDetail", HC7);
		info.AddValue("LightingDetail", HC_0001);
		info.AddValue("PostProcessingDetail", HCw);
	}
}
