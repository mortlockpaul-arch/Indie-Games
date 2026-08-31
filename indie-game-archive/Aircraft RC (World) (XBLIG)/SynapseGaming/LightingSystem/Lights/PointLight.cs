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
	private LightingType HCB = LightingType.RealTime;

	private bool HC_0002;

	private int HC_0012;

	private float HCH;

	private ShadowType HC7;

	private float HC_0001 = 0.5f;

	private float HCw = 1f;

	private float HCZ = 0.2f;

	private bool HC_000F = true;

	private float HCy = 10f;

	private Matrix HC6 = Matrix.Identity;

	private IShadowSource HCD;

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
	/// Provides softer indirect-like illumination without "hot-spots".
	/// </summary>
	public override bool FillLight
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
	/// Controls how quickly lighting falls off over distance (only available in deferred rendering).
	/// Value ranges from 0.0f to 1.0f.
	/// </summary>
	public override float FalloffStrength
	{
		get
		{
			return HCH;
		}
		set
		{
			HCH = MathHelper.Clamp(value, 0f, 1f);
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
			if (HCD == null)
			{
				throw new ArgumentException("ShadowSource is null. This can result in poor rendering performance.");
			}
			return HCD;
		}
		set
		{
			if (value == null)
			{
				HCD = this;
			}
			else
			{
				HCD = value;
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
			return HC7;
		}
		set
		{
			HC7 = value;
		}
	}

	/// <summary>
	/// Position in world space of the shadow source.
	/// </summary>
	public Vector3 ShadowPosition => HC6.Translation;

	/// <summary>
	/// Adjusts the visual quality of casts shadows.
	/// </summary>
	public float ShadowQuality
	{
		get
		{
			return HC_0001;
		}
		set
		{
			HC_0001 = MathHelper.Clamp(value, 0f, 1f);
		}
	}

	/// <summary>
	/// Main property used to eliminate shadow artifacts.
	/// </summary>
	public float ShadowPrimaryBias
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
	/// Additional fine-tuned property used to eliminate shadow artifacts.
	/// </summary>
	public float ShadowSecondaryBias
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
	/// Enables independent level-of-detail per cubemap face on point-based lights.
	/// </summary>
	public bool ShadowPerSurfaceLOD
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
			return HC6.Translation;
		}
		set
		{
			HC6.Translation = value;
			HC_0012++;
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
			return HCy;
		}
		set
		{
			HCy = value;
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
			return HC6;
		}
		set
		{
			HC6 = value;
			HC_0012++;
			UpdateBounds();
		}
	}

	/// <summary>
	/// Indicates the current move. This value increments each time the object
	/// is moved (when the World transform changes).
	/// </summary>
	public override int MoveId => HC_0012;

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
		HCD = this;
		UpdateBounds();
	}

	/// <summary />
	protected virtual void UpdateBounds()
	{
		Vector3 vector = new Vector3(HCy, HCy, HCy);
		Vector3 translation = HC6.Translation;
		base.WorldBoundingBox = new BoundingBox(translation - vector, translation + vector);
		base.WorldBoundingSphere = new BoundingSphere(HC6.Translation, HCy);
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
		SerializationHelper.DeserializeEnum(ref HC7, info, "ShadowType", isflag: false);
		Position = SerializationHelper.DeserializeField<Vector3>(info, "Position");
		SerializationHelper.DeserializeField(ref HCy, info, "Radius", usedefault: true);
		SerializationHelper.DeserializeField(ref HC_0001, info, "ShadowQuality", usedefault: true);
		SerializationHelper.DeserializeField(ref HCw, info, "ShadowPrimaryBias", usedefault: true);
		SerializationHelper.DeserializeField(ref HCZ, info, "ShadowSecondaryBias", usedefault: true);
		SerializationHelper.DeserializeField(ref HC_000F, info, "ShadowPerSurfaceLOD", usedefault: true);
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
