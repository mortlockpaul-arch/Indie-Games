using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Serialization;
using Z;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Provides scene environmental information to the lighting system.
/// </summary>
[Serializable]
public class SceneEnvironment : ISceneEnvironment, IEditorObject, INamedObject, IFullSerializable, ISerializable, Z.w, IDisposable
{
	private float HCB = 300f;

	private bool HC_0002 = true;

	private float HC_0012 = 200f;

	private float HCH = 300f;

	private Vector3 HC7 = Vector3.One * 0.75f;

	private float HC_0001 = 300f;

	private float HCw = 300f;

	private float HCZ = 300f;

	private float HC_000F = 3f;

	private float HCy = 0.9f;

	private bool HC6 = true;

	private float HCD = 1f;

	private float HC_0011 = 0.5f;

	private float HCK = 0.1f;

	private float HC_0003 = 0.5f;

	private float HCk = 0.5f;

	private float HCs = 100f;

	private float HC_0013 = 0.01f;

	private float HCX = 1f;

	private string HCz = "";

	private string HCA = "";

	private string HCc = "";

	private bool HCY;

	[CompilerGenerated]
	private bool HCV;

	/// <summary>
	/// Maximum world space distance objects are visible.
	/// </summary>
	public float VisibleDistance
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
	/// Enables scene fog.
	/// </summary>
	public bool FogEnabled
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
	/// World space distance that fog begins.
	/// </summary>
	public float FogStartDistance
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
	/// World space distance that fog fully obscures objects.
	/// </summary>
	public float FogEndDistance
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
	/// Color applied to scene fog.
	/// </summary>
	public Vector3 FogColor
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
	/// World space distance that directional shadows begin fading.
	/// </summary>
	public float ShadowFadeStartDistance
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
	/// World space distance that directional shadows completely disappear.
	/// </summary>
	public float ShadowFadeEndDistance
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
	/// World space distance used to include shadow casters. This allows including shadows
	/// from objects further away than the shadow fade area, for instance shadows from
	/// distant mountains.
	/// </summary>
	public float ShadowCasterDistance
	{
		get
		{
			return HCZ;
		}
		set
		{
			HCZ = value;
		}
	}

	/// <summary>
	/// Strength of bloom applied to the scene.
	/// </summary>
	public float BloomAmount
	{
		get
		{
			return HC_000F;
		}
		set
		{
			HC_000F = value;
		}
	}

	/// <summary>
	/// Minimum pixel intensity required for bloom to occur.
	/// </summary>
	public float BloomThreshold
	{
		get
		{
			return HCy;
		}
		set
		{
			HCy = value;
		}
	}

	/// <summary>
	/// Enables High Dynamic Range.
	/// </summary>
	public bool DynamicRangeEnabled
	{
		get
		{
			return HC6;
		}
		set
		{
			HC6 = value;
		}
	}

	/// <summary>
	/// Intensity of the scene exposure.
	/// </summary>
	public float ExposureAmount
	{
		get
		{
			return HCD;
		}
		set
		{
			HCD = value;
		}
	}

	/// <summary>
	/// Intensity of scene colors when using High Dynamic Range.
	/// </summary>
	public float DynamicRangeSaturationAmount
	{
		get
		{
			return HC_0011;
		}
		set
		{
			HC_0011 = value;
		}
	}

	/// <summary>
	/// Intensity of scene contrast when using High Dynamic Range.
	/// </summary>
	public float DynamicRangeDarkenAmount
	{
		get
		{
			return HCK;
		}
		set
		{
			HCK = value;
		}
	}

	/// <summary>
	/// Intensity of High Dynamic Range color correction and simulated film exposure effect.
	/// </summary>
	public float DynamicRangeCinematicAmount
	{
		get
		{
			return HC_0003;
		}
		set
		{
			HC_0003 = value;
		}
	}

	/// <summary>
	/// Time required to fully adjust High Dynamic Range to lighting changes.
	/// </summary>
	public float DynamicRangeTransitionTime
	{
		get
		{
			return HCk;
		}
		set
		{
			HCk = value;
		}
	}

	/// <summary>
	/// Maximum intensity increase allowed for High Dynamic Range. Limits intensity
	/// increases, which sets the darkness-level where the scene will remain dark.
	/// </summary>
	public float DynamicRangeTransitionMaxScale
	{
		get
		{
			return HCs;
		}
		set
		{
			HCs = value;
		}
	}

	/// <summary>
	/// Maximum intensity decrease allowed for High Dynamic Range. Limits intensity
	/// decreases, which sets the brightness-level where the scene will remain overly bright.
	/// </summary>
	public float DynamicRangeTransitionMinScale
	{
		get
		{
			return HC_0013;
		}
		set
		{
			HC_0013 = value;
		}
	}

	/// <summary>
	/// Amount of gravity applied to dynamic collide-able objects in the scene.
	/// </summary>
	public float Gravity
	{
		get
		{
			return HCX;
		}
		set
		{
			HCX = value;
		}
	}

	/// <summary>
	/// The object's current name.
	/// </summary>
	public string Name
	{
		get
		{
			return HCz;
		}
		set
		{
		}
	}

	/// <summary>
	/// Notifies the editor that this object is partially controlled via code. The editor
	/// will display information to the user indicating some property values are
	/// overridden in code and changes may not take effect.
	/// </summary>
	public bool AffectedInCode
	{
		[CompilerGenerated]
		get
		{
			return HCV;
		}
		[CompilerGenerated]
		set
		{
			HCV = value;
		}
	}

	internal string FileName
	{
		get
		{
			return HCA;
		}
		set
		{
			HCA = hCA;
		}
	}

	internal string ProjectFile
	{
		get
		{
			return HCc;
		}
		set
		{
			HCc = hCc;
		}
	}

	string Z.w.ProjectFile => HCc;

	internal void _0002N(string P_0)
	{
		HCz = P_0;
	}

	/// <summary>
	/// Creates a new SceneEnvironment instance.
	/// </summary>
	public SceneEnvironment()
	{
		SunBurnEditor.OnCreateResource(this);
	}

	/// <summary>
	/// Releases resources allocated by this object.
	/// </summary>
	public void Dispose()
	{
		if (!HCY)
		{
			HCY = true;
			SunBurnEditor.OnDisposeResource(this);
		}
	}

	internal static SceneEnvironment _0002F(string P_0)
	{
		SceneEnvironment sceneEnvironment = SerializationHelper.LoadFromXml<SceneEnvironment>(P_0);
		if (sceneEnvironment == null)
		{
			sceneEnvironment = new SceneEnvironment();
		}
		return sceneEnvironment;
	}

	/// <summary>
	/// Deserializes object data from the provided SerializationInfo.
	/// </summary>
	/// <param name="info">Contains the serialized object data.</param>
	/// <param name="context"></param>
	public void SetObjectData(SerializationInfo info, StreamingContext context)
	{
		SerializationHelper.DeserializeField(ref HCB, info, "VisibleDistance", usedefault: false);
		SerializationHelper.DeserializeField(ref HC_0002, info, "FogEnabled", usedefault: false);
		SerializationHelper.DeserializeField(ref HC7, info, "FogColor", usedefault: false);
		SerializationHelper.DeserializeField(ref HC_0012, info, "FogStartDistance", usedefault: false);
		SerializationHelper.DeserializeField(ref HCH, info, "FogEndDistance", usedefault: false);
		SerializationHelper.DeserializeField(ref HC_0001, info, "ShadowFadeStartDistance", usedefault: false);
		SerializationHelper.DeserializeField(ref HCw, info, "ShadowFadeEndDistance", usedefault: false);
		SerializationHelper.DeserializeField(ref HCZ, info, "ShadowCasterDistance", usedefault: false);
		SerializationHelper.DeserializeField(ref HC_000F, info, "BloomAmount", usedefault: false);
		SerializationHelper.DeserializeField(ref HCy, info, "BloomThreshold", usedefault: false);
		SerializationHelper.DeserializeField(ref HC6, info, "DynamicRangeEnabled", usedefault: false);
		SerializationHelper.DeserializeField(ref HCD, info, "ExposureAmount", usedefault: false);
		SerializationHelper.DeserializeField(ref HCs, info, "DynamicRangeTransitionMaxScale", usedefault: false);
		SerializationHelper.DeserializeField(ref HC_0013, info, "DynamicRangeTransitionMinScale", usedefault: false);
		SerializationHelper.DeserializeField(ref HCk, info, "DynamicRangeTransitionTime", usedefault: false);
		SerializationHelper.DeserializeField(ref HC_0011, info, "DynamicRangeSaturationAmount", usedefault: false);
		SerializationHelper.DeserializeField(ref HCK, info, "DynamicRangeDarkenAmount", usedefault: false);
		SerializationHelper.DeserializeField(ref HC_0003, info, "DynamicRangeCinematicAmount", usedefault: false);
		SerializationHelper.DeserializeField(ref HCX, info, "Gravity", usedefault: false);
	}

	/// <summary>
	/// Serializes object data to the provided SerializationInfo.
	/// </summary>
	/// <param name="info">SerializationInfo to store the serialized data.</param>
	/// <param name="context"></param>
	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("VisibleDistance", VisibleDistance);
		info.AddValue("FogEnabled", FogEnabled);
		info.AddValue("FogColor", FogColor);
		info.AddValue("FogStartDistance", FogStartDistance);
		info.AddValue("FogEndDistance", FogEndDistance);
		info.AddValue("ShadowFadeStartDistance", ShadowFadeStartDistance);
		info.AddValue("ShadowFadeEndDistance", ShadowFadeEndDistance);
		info.AddValue("ShadowCasterDistance", ShadowCasterDistance);
		info.AddValue("BloomAmount", BloomAmount);
		info.AddValue("BloomThreshold", BloomThreshold);
		info.AddValue("DynamicRangeEnabled", DynamicRangeEnabled);
		info.AddValue("ExposureAmount", ExposureAmount);
		info.AddValue("DynamicRangeTransitionMaxScale", DynamicRangeTransitionMaxScale);
		info.AddValue("DynamicRangeTransitionMinScale", DynamicRangeTransitionMinScale);
		info.AddValue("DynamicRangeTransitionTime", DynamicRangeTransitionTime);
		info.AddValue("DynamicRangeSaturationAmount", DynamicRangeSaturationAmount);
		info.AddValue("DynamicRangeDarkenAmount", DynamicRangeDarkenAmount);
		info.AddValue("DynamicRangeCinematicAmount", DynamicRangeCinematicAmount);
		info.AddValue("Gravity", Gravity);
	}
}
