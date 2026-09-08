using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Serialization;
using SynapseGaming.LightingSystem.Shadows;

namespace SynapseGaming.LightingSystem.Lights;

/// <summary>
/// Provides directional light (sunlight) information for rendering lighting and shadows.
/// </summary>
[Serializable]
public class DirectionalLight : BaseLight, IDirectionalSource, IShadowSource
{
	private LightingType _3A_0018 = LightingType.RealTime;

	private ShadowType _3AL = ShadowType.AllObjects;

	private float _3A_0019 = 1f;

	private float _3A3 = 1f;

	private float _3A6 = 0.2f;

	private bool _3AD = true;

	private Matrix _3A_0017 = CoreHelper.CreateMatrixFromNormalizedVectors(Vector3.Forward, Vector3.Down);

	private static BoundingBox _3A_0003 = new BoundingBox(new Vector3(float.MinValue, float.MinValue, float.MinValue), new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));

	private static BoundingSphere _3Al = new BoundingSphere(default(Vector3), float.MaxValue);

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
	/// Unused.
	/// </summary>
	public override bool FillLight
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	/// <summary>
	/// Controls how quickly lighting falls off over distance (unused in this light type).
	/// </summary>
	public override float FalloffStrength
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	/// <summary>
	/// Indicates the object bounding area spans the entire world and
	/// the object is always visible.
	/// </summary>
	public override bool InfiniteBounds => true;

	/// <summary>
	/// Shadow source the light's shadows are generated from.
	/// </summary>
	public override IShadowSource ShadowSource
	{
		get
		{
			return this;
		}
		set
		{
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
			return _3AL;
		}
		set
		{
			_3AL = value;
		}
	}

	/// <summary>
	/// Position in world space of the shadow source.
	/// </summary>
	public Vector3 ShadowPosition => Direction * -1000000f;

	/// <summary>
	/// Adjusts the visual quality of casts shadows.
	/// </summary>
	public float ShadowQuality
	{
		get
		{
			return _3A_0019;
		}
		set
		{
			_3A_0019 = MathHelper.Clamp(value, 0f, 2f);
		}
	}

	/// <summary>
	/// Main property used to eliminate shadow artifacts.
	/// </summary>
	public float ShadowPrimaryBias
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
	/// Additional fine-tuned property used to eliminate shadow artifacts.
	/// </summary>
	public float ShadowSecondaryBias
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
	/// Enables independent level-of-detail per cubemap face on point-based lights.
	/// </summary>
	public bool ShadowPerSurfaceLOD
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
	/// Unused.
	/// </summary>
	public bool ShadowRenderLightsTogether => false;

	/// <summary>
	/// Direction in world space of the light's influence.
	/// </summary>
	public Vector3 Direction
	{
		get
		{
			return _3A_0017.Forward;
		}
		set
		{
			if (value == Vector3.Zero)
			{
				_3A_0017 = Matrix.Identity;
			}
			else
			{
				_3A_0017 = CoreHelper.CreateMatrixFromNormalizedVectors(Vector3.Forward, Vector3.Normalize(value));
			}
		}
	}

	/// <summary>
	/// World space transform of the light.
	/// </summary>
	public override Matrix World
	{
		get
		{
			return _3A_0017;
		}
		set
		{
			_3A_0017 = value;
			_3A_0017.Translation = Vector3.Zero;
		}
	}

	/// <summary>
	/// Indicates the current move. This value increments each time the object
	/// is moved (when the World transform changes).
	/// </summary>
	public override int MoveId => 0;

	/// <summary>
	/// Creates a new DirectionalLight instance.
	/// </summary>
	public DirectionalLight()
	{
		base.WorldBoundingBox = _3A_0003;
		base.WorldBoundingSphere = _3Al;
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
		SerializationHelper.DeserializeEnum(ref _3AL, info, "ShadowType", isflag: false);
		Direction = SerializationHelper.DeserializeField<Vector3>(info, "Direction");
		SerializationHelper.DeserializeField(ref _3A_0019, info, "ShadowQuality", usedefault: true);
		SerializationHelper.DeserializeField(ref _3A3, info, "ShadowPrimaryBias", usedefault: true);
		SerializationHelper.DeserializeField(ref _3A6, info, "ShadowSecondaryBias", usedefault: true);
		SerializationHelper.DeserializeField(ref _3AD, info, "ShadowPerSurfaceLOD", usedefault: true);
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
		info.AddValue("Direction", Direction);
		info.AddValue("ShadowQuality", ShadowQuality);
		info.AddValue("ShadowPrimaryBias", ShadowPrimaryBias);
		info.AddValue("ShadowSecondaryBias", ShadowSecondaryBias);
		info.AddValue("ShadowPerSurfaceLOD", ShadowPerSurfaceLOD);
		base.GetObjectData(info, context);
	}
}
