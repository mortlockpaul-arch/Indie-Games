using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Serialization;
using SynapseGaming.LightingSystem.Shadows;

namespace SynapseGaming.LightingSystem.Lights;

/// <summary>
/// Provides ambient light information for rendering lighting.
/// </summary>
[Serializable]
public class AmbientLight : BaseLight, IAmbientSource
{
	private float HCB = 0.15f;

	private static BoundingBox HC_0002 = new BoundingBox(new Vector3(float.MinValue, float.MinValue, float.MinValue), new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));

	private static BoundingSphere HC_0012 = new BoundingSphere(default(Vector3), float.MaxValue);

	/// <summary>
	/// Determines if the lighting is real-time or bake-down.
	/// </summary>
	public override LightingType LightingType
	{
		get
		{
			return LightingType.RealTime;
		}
		set
		{
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
			return null;
		}
		set
		{
		}
	}

	/// <summary>
	/// World space transform of the light.
	/// </summary>
	public override Matrix World
	{
		get
		{
			return Matrix.Identity;
		}
		set
		{
		}
	}

	/// <summary>
	/// Indicates the current move. This value increments each time the object
	/// is moved (when the World transform changes).
	/// </summary>
	public override int MoveId => 0;

	/// <summary>
	/// Increases the detail of normal mapped surfaces during the ambient lighting pass (deferred rendering only).
	/// </summary>
	public float Depth
	{
		get
		{
			return HCB;
		}
		set
		{
			HCB = MathHelper.Clamp(value, 0f, 0.5f);
		}
	}

	/// <summary>
	/// Creates a new AmbientLight instance.
	/// </summary>
	public AmbientLight()
	{
		base.WorldBoundingBox = HC_0002;
		base.WorldBoundingSphere = HC_0012;
	}

	/// <summary>
	/// Deserializes object data from the provided SerializationInfo.
	/// </summary>
	/// <param name="info">Contains the serialized object data.</param>
	/// <param name="context"></param>
	public override void SetObjectData(SerializationInfo info, StreamingContext context)
	{
		Depth = SerializationHelper.DeserializeField<float>(info, "Depth");
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
		info.AddValue("Depth", Depth);
		base.GetObjectData(info, context);
	}
}
