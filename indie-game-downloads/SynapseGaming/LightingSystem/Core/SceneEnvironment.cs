using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using _0003;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Serialization;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Provides scene environmental information to the lighting system.
/// </summary>
[Serializable]
public class SceneEnvironment : ISceneEnvironment, IEditorObject, INamedObject, IFullSerializable, ISerializable, _0003._0017, IDisposable
{
	private float _3A_0018 = 300f;

	private bool _3AL = true;

	private float _3A_0019 = 200f;

	private float _3A3 = 300f;

	private Vector3 _3A6 = Vector3.One * 0.75f;

	private float _3AD = 300f;

	private float _3A_0017 = 300f;

	private float _3A_0003 = 300f;

	private float _3Al = 3f;

	private float _3At = 0.9f;

	private bool _3AF = true;

	private float _3Ac = 1f;

	private float _3Ag = 0.5f;

	private float _3AI = 0.1f;

	private float _3A8 = 0.5f;

	private float _3AZ = 0.5f;

	private float _3Ax = 100f;

	private float _3Aq = 0.01f;

	private float _3Ab = 1f;

	private string _3AT = "";

	private string _3Ay = "";

	private string _3A_0015 = "";

	private bool _3A_0001;

	[CompilerGenerated]
	private bool _3A7;

	/// <summary>
	/// Maximum world space distance objects are visible.
	/// </summary>
	public float VisibleDistance
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
	/// Enables scene fog.
	/// </summary>
	public bool FogEnabled
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
	/// World space distance that fog begins.
	/// </summary>
	public float FogStartDistance
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
	/// World space distance that fog fully obscures objects.
	/// </summary>
	public float FogEndDistance
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
	/// Color applied to scene fog.
	/// </summary>
	public Vector3 FogColor
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
	/// World space distance that directional shadows begin fading.
	/// </summary>
	public float ShadowFadeStartDistance
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
	/// World space distance that directional shadows completely disappear.
	/// </summary>
	public float ShadowFadeEndDistance
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
	/// World space distance used to include shadow casters. This allows including shadows
	/// from objects further away than the shadow fade area, for instance shadows from
	/// distant mountains.
	/// </summary>
	public float ShadowCasterDistance
	{
		get
		{
			return _3A_0003;
		}
		set
		{
			_3A_0003 = value;
		}
	}

	/// <summary>
	/// Strength of bloom applied to the scene.
	/// </summary>
	public float BloomAmount
	{
		get
		{
			return _3Al;
		}
		set
		{
			_3Al = value;
		}
	}

	/// <summary>
	/// Minimum pixel intensity required for bloom to occur.
	/// </summary>
	public float BloomThreshold
	{
		get
		{
			return _3At;
		}
		set
		{
			_3At = value;
		}
	}

	/// <summary>
	/// Enables High Dynamic Range.
	/// </summary>
	public bool DynamicRangeEnabled
	{
		get
		{
			return _3AF;
		}
		set
		{
			_3AF = value;
		}
	}

	/// <summary>
	/// Intensity of the scene exposure.
	/// </summary>
	public float ExposureAmount
	{
		get
		{
			return _3Ac;
		}
		set
		{
			_3Ac = value;
		}
	}

	/// <summary>
	/// Intensity of scene colors when using High Dynamic Range.
	/// </summary>
	public float DynamicRangeSaturationAmount
	{
		get
		{
			return _3Ag;
		}
		set
		{
			_3Ag = value;
		}
	}

	/// <summary>
	/// Intensity of scene contrast when using High Dynamic Range.
	/// </summary>
	public float DynamicRangeDarkenAmount
	{
		get
		{
			return _3AI;
		}
		set
		{
			_3AI = value;
		}
	}

	/// <summary>
	/// Intensity of High Dynamic Range color correction and simulated film exposure effect.
	/// </summary>
	public float DynamicRangeCinematicAmount
	{
		get
		{
			return _3A8;
		}
		set
		{
			_3A8 = value;
		}
	}

	/// <summary>
	/// Time required to fully adjust High Dynamic Range to lighting changes.
	/// </summary>
	public float DynamicRangeTransitionTime
	{
		get
		{
			return _3AZ;
		}
		set
		{
			_3AZ = value;
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
			return _3Ax;
		}
		set
		{
			_3Ax = value;
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
			return _3Aq;
		}
		set
		{
			_3Aq = value;
		}
	}

	/// <summary>
	/// Amount of gravity applied to dynamic collide-able objects in the scene.
	/// </summary>
	public float Gravity
	{
		get
		{
			return _3Ab;
		}
		set
		{
			_3Ab = value;
		}
	}

	/// <summary>
	/// The object's current name.
	/// </summary>
	public string Name
	{
		get
		{
			return _3AT;
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
			return _3A7;
		}
		[CompilerGenerated]
		set
		{
			_3A7 = value;
		}
	}

	internal string FileName
	{
		get
		{
			return _3Ay;
		}
		set
		{
			_3Ay = text;
		}
	}

	internal string ProjectFile
	{
		get
		{
			return _3A_0015;
		}
		set
		{
			_3A_0015 = text;
		}
	}

	string _0003._0017.ProjectFile => _3A_0015;

	internal void Lk(string P_0)
	{
		_3AT = P_0;
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
		if (!_3A_0001)
		{
			_3A_0001 = true;
			SunBurnEditor.OnDisposeResource(this);
		}
	}

	internal static SceneEnvironment Lp(string P_0)
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
		SerializationHelper.DeserializeField(ref _3A_0018, info, "VisibleDistance", usedefault: false);
		SerializationHelper.DeserializeField(ref _3AL, info, "FogEnabled", usedefault: false);
		SerializationHelper.DeserializeField(ref _3A6, info, "FogColor", usedefault: false);
		SerializationHelper.DeserializeField(ref _3A_0019, info, "FogStartDistance", usedefault: false);
		SerializationHelper.DeserializeField(ref _3A3, info, "FogEndDistance", usedefault: false);
		SerializationHelper.DeserializeField(ref _3AD, info, "ShadowFadeStartDistance", usedefault: false);
		SerializationHelper.DeserializeField(ref _3A_0017, info, "ShadowFadeEndDistance", usedefault: false);
		SerializationHelper.DeserializeField(ref _3A_0003, info, "ShadowCasterDistance", usedefault: false);
		SerializationHelper.DeserializeField(ref _3Al, info, "BloomAmount", usedefault: false);
		SerializationHelper.DeserializeField(ref _3At, info, "BloomThreshold", usedefault: false);
		SerializationHelper.DeserializeField(ref _3AF, info, "DynamicRangeEnabled", usedefault: false);
		SerializationHelper.DeserializeField(ref _3Ac, info, "ExposureAmount", usedefault: false);
		SerializationHelper.DeserializeField(ref _3Ax, info, "DynamicRangeTransitionMaxScale", usedefault: false);
		SerializationHelper.DeserializeField(ref _3Aq, info, "DynamicRangeTransitionMinScale", usedefault: false);
		SerializationHelper.DeserializeField(ref _3AZ, info, "DynamicRangeTransitionTime", usedefault: false);
		SerializationHelper.DeserializeField(ref _3Ag, info, "DynamicRangeSaturationAmount", usedefault: false);
		SerializationHelper.DeserializeField(ref _3AI, info, "DynamicRangeDarkenAmount", usedefault: false);
		SerializationHelper.DeserializeField(ref _3A8, info, "DynamicRangeCinematicAmount", usedefault: false);
		SerializationHelper.DeserializeField(ref _3Ab, info, "Gravity", usedefault: false);
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
