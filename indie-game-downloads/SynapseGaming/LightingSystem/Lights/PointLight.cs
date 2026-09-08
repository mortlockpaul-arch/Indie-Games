using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Serialization;
using SynapseGaming.LightingSystem.Shadows;

namespace SynapseGaming.LightingSystem.Lights;

/// <summary>
/// Provides point light (aka: omni light) information for rendering lighting and shadows.
/// </summary>
[Serializable]
public class PointLight : BaseLight, IPointSource, IShadowSource
{
	private LightingType _3A_0018 = LightingType.RealTime;

	private bool _3AL;

	private int _3A_0019;

	private float _3A3;

	private ShadowType _3A6;

	private float _3AD = 0.5f;

	private float _3A_0017 = 1f;

	private float _3A_0003 = 0.2f;

	private bool _3Al = true;

	private float _3At = 10f;

	private Matrix _3AF = Matrix.Identity;

	private IShadowSource _3Ac;

	/// <summary>
	/// Determines if the lighting is real-time or bake-down.
	/// </summary>
	public override LightingType LightingType
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
	/// Provides softer indirect-like illumination without "hot-spots".
	/// </summary>
	public override bool FillLight
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
	/// Controls how quickly lighting falls off over distance (only available in deferred rendering).
	/// Value ranges from 0.0f to 1.0f.
	/// </summary>
	public override float FalloffStrength
	{
		get
		{
			return _3A3;
		}
		set
		{
			_3A3 = MathHelper.Clamp(value, 0f, 1f);
		}
	}

	/// <summary>
	/// Shadow source the light's shadows are generated from.
	/// Allows sharing shadows between point light sources.
	/// </summary>
	public override IShadowSource ShadowSource
	{
		get
		{
			if (_3Ac == null)
			{
				throw new ArgumentException("ShadowSource is null. This can result in poor rendering performance.");
			}
			return _3Ac;
		}
		set
		{
			if (value == null)
			{
				_3Ac = this;
			}
			else
			{
				_3Ac = value;
			}
		}
	}

	/// <summary>
	/// Defines the type of objects that cast shadows from the light.
	/// Does not affect an object's ability to receive shadows.
	/// </summary>
	public ShadowType ShadowType
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
	/// Position in world space of the shadow source.
	/// </summary>
	public Vector3 ShadowPosition => _3AF.Translation;

	/// <summary>
	/// Adjusts the visual quality of casts shadows.
	/// </summary>
	public float ShadowQuality
	{
		get
		{
			return _3AD;
		}
		set
		{
			_3AD = MathHelper.Clamp(value, 0f, 1f);
		}
	}

	/// <summary>
	/// Main property used to eliminate shadow artifacts.
	/// </summary>
	public float ShadowPrimaryBias
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
	/// Additional fine-tuned property used to eliminate shadow artifacts.
	/// </summary>
	public float ShadowSecondaryBias
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
	/// Enables independent level-of-detail per cubemap face on point-based lights.
	/// </summary>
	public bool ShadowPerSurfaceLOD
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
	/// Unused.
	/// </summary>
	public bool ShadowRenderLightsTogether => false;

	/// <summary>
	/// Position in world space of the light.
	/// </summary>
	public Vector3 Position
	{
		get
		{
			return _3AF.Translation;
		}
		set
		{
			_3AF.Translation = value;
			_3A_0019++;
			UpdateBounds();
		}
	}

	/// <summary>
	/// Maximum distance in world space of the light's influence.
	/// </summary>
	public float Radius
	{
		get
		{
			return _3At;
		}
		set
		{
			_3At = value;
			UpdateBounds();
		}
	}

	/// <summary>
	/// World space transform of the light.
	/// </summary>
	public override Matrix World
	{
		get
		{
			return _3AF;
		}
		set
		{
			_3AF = value;
			_3A_0019++;
			UpdateBounds();
		}
	}

	/// <summary>
	/// Indicates the current move. This value increments each time the object
	/// is moved (when the World transform changes).
	/// </summary>
	public override int MoveId => _3A_0019;

	/// <summary>
	/// Indicates the object bounding area spans the entire world and
	/// the object is always visible.
	/// </summary>
	public override bool InfiniteBounds => false;

	/// <summary>
	/// Creates a new PointLight instance.
	/// </summary>
	public PointLight()
	{
		_3Ac = this;
		UpdateBounds();
	}

	/// <summary />
	protected virtual void UpdateBounds()
	{
		Vector3 vector = new Vector3(_3At, _3At, _3At);
		Vector3 translation = _3AF.Translation;
		base.WorldBoundingBox = new BoundingBox(translation - vector, translation + vector);
		base.WorldBoundingSphere = new BoundingSphere(_3AF.Translation, _3At);
	}

	/// <summary>
	/// Returns a hash code that uniquely identifies the shadow source
	/// and its current state.  Changes to ShadowPosition affects the
	/// hash code, which is used to trigger updates on related shadows.
	/// </summary>
	/// <returns>Shadow hash code.</returns>
	public int GetShadowSourceHashCode()
	{
		return ShadowPosition.GetHashCode();
	}

	/// <summary>
	/// Deserializes object data from the provided SerializationInfo.
	/// </summary>
	/// <param name="info">Contains the serialized object data.</param>
	/// <param name="context"></param>
	public override void SetObjectData(SerializationInfo info, StreamingContext context)
	{
		SerializationHelper.DeserializeEnum(ref _3A6, info, "ShadowType", isflag: false);
		Position = SerializationHelper.DeserializeField<Vector3>(info, "Position");
		SerializationHelper.DeserializeField(ref _3At, info, "Radius", usedefault: true);
		SerializationHelper.DeserializeField(ref _3AD, info, "ShadowQuality", usedefault: true);
		SerializationHelper.DeserializeField(ref _3A_0017, info, "ShadowPrimaryBias", usedefault: true);
		SerializationHelper.DeserializeField(ref _3A_0003, info, "ShadowSecondaryBias", usedefault: true);
		SerializationHelper.DeserializeField(ref _3Al, info, "ShadowPerSurfaceLOD", usedefault: true);
		UpdateBounds();
		base.SetObjectData(info, context);
	}

	/// <summary>
	/// Serializes object data to the provided SerializationInfo.
	/// </summary>
	/// <param name="info">SerializationInfo to store the serialized data.</param>
	/// <param name="context"></param>
	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("ShadowType", ShadowType);
		info.AddValue("Position", Position);
		info.AddValue("Radius", Radius);
		info.AddValue("ShadowQuality", ShadowQuality);
		info.AddValue("ShadowPrimaryBias", ShadowPrimaryBias);
		info.AddValue("ShadowSecondaryBias", ShadowSecondaryBias);
		info.AddValue("ShadowPerSurfaceLOD", ShadowPerSurfaceLOD);
		base.GetObjectData(info, context);
	}
}
