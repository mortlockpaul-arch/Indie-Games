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
	private LightingType HCB = LightingType.RealTime;

	private ShadowType HC_0002 = ShadowType.AllObjects;

	private float HC_0012 = 1f;

	private float HCH = 1f;

	private float HC7 = 0.2f;

	private bool HC_0001 = true;

	private Matrix HCw = CoreHelper.CreateMatrixFromNormalizedVectors(Vector3.Forward, Vector3.Down);

	private static BoundingBox HCZ = new BoundingBox(new Vector3(float.MinValue, float.MinValue, float.MinValue), new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));

	private static BoundingSphere HC_000F = new BoundingSphere(default(Vector3), float.MaxValue);

	/// <summary>
	/// Determines if the lighting is real-time or bake-down.
	/// </summary>
	public override LightingType LightingType
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
			return HC_0002;
		}
		set
		{
			HC_0002 = value;
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
			return HC_0012;
		}
		set
		{
			HC_0012 = MathHelper.Clamp(value, 0f, 2f);
		}
	}

	/// <summary>
	/// Main property used to eliminate shadow artifacts.
	/// </summary>
	public float ShadowPrimaryBias
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
	/// Additional fine-tuned property used to eliminate shadow artifacts.
	/// </summary>
	public float ShadowSecondaryBias
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
	/// Enables independent level-of-detail per cubemap face on point-based lights.
	/// </summary>
	public bool ShadowPerSurfaceLOD
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
			return HCw.Forward;
		}
		set
		{
			if (value == Vector3.Zero)
			{
				HCw = Matrix.Identity;
			}
			else
			{
				HCw = CoreHelper.CreateMatrixFromNormalizedVectors(Vector3.Forward, Vector3.Normalize(value));
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
			return HCw;
		}
		set
		{
			HCw = value;
			HCw.Translation = Vector3.Zero;
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
		base.WorldBoundingBox = HCZ;
		base.WorldBoundingSphere = HC_000F;
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
		SerializationHelper.DeserializeEnum(ref HC_0002, info, "ShadowType", isflag: false);
		Direction = SerializationHelper.DeserializeField<Vector3>(info, "Direction");
		SerializationHelper.DeserializeField(ref HC_0012, info, "ShadowQuality", usedefault: true);
		SerializationHelper.DeserializeField(ref HCH, info, "ShadowPrimaryBias", usedefault: true);
		SerializationHelper.DeserializeField(ref HC7, info, "ShadowSecondaryBias", usedefault: true);
		SerializationHelper.DeserializeField(ref HC_0001, info, "ShadowPerSurfaceLOD", usedefault: true);
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
