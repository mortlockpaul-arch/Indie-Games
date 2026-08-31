using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Lights;
using SynapseGaming.LightingSystem.Serialization;

namespace SynapseGaming.LightingSystem.Shadows;

/// <summary>
/// Provides a shareable point shadow source for use with PointLight objects.
/// Any number of PointLight objects can share the same shadow source.  Shadow
/// source position and properties are independent of the lights that reference it.
/// </summary>
[Serializable]
public class ShadowSource : IPointSource, IShadowSource, INamedObject, IFullSerializable, ISerializable
{
	private string HCB = "";

	private ShadowType HC_0002 = ShadowType.AllObjects;

	private float HC_0012 = 0.5f;

	private float HCH = 1f;

	private float HC7 = 0.2f;

	private bool HC_0001 = true;

	private bool HCw;

	private Matrix HCZ = Matrix.Identity;

	[CompilerGenerated]
	private bool HC_000F;

	/// <summary>
	/// The object's current name.
	/// </summary>
	public string Name
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
	/// Notifies the editor that this object is partially controlled via code. The editor
	/// will display information to the user indicating some property values are
	/// overridden in code and changes may not take effect.
	/// </summary>
	public bool AffectedInCode
	{
		[CompilerGenerated]
		get
		{
			return HC_000F;
		}
		[CompilerGenerated]
		set
		{
			HC_000F = value;
		}
	}

	/// <summary>
	/// Defines the type of objects that cast shadows from the source.
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
	public Vector3 ShadowPosition => HCZ.Translation;

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
			HC_0012 = MathHelper.Clamp(value, 0f, 1f);
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
	/// Requests that all lights contained within the shadow source are rendered in one
	/// pass (this is only a performance hint - support depends on the rendering implementation).
	/// </summary>
	public bool ShadowRenderLightsTogether
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
	/// Position in world space of the source.
	/// </summary>
	public Vector3 Position
	{
		get
		{
			return HCZ.Translation;
		}
		set
		{
			HCZ.Translation = value;
		}
	}

	/// <summary>
	/// Maximum distance in world space of the source's influence.
	/// </summary>
	public float Radius
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
	/// World space transform of the shadow source.
	/// </summary>
	public Matrix World
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

	internal void Hh(IShadowSource P_0)
	{
		ShadowPerSurfaceLOD = P_0.ShadowPerSurfaceLOD;
		ShadowQuality = P_0.ShadowQuality;
		ShadowPrimaryBias = P_0.ShadowPrimaryBias;
		ShadowSecondaryBias = P_0.ShadowSecondaryBias;
		ShadowType = P_0.ShadowType;
		World = P_0.World;
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
	/// Creates a new ShadowSource instance.
	/// </summary>
	public ShadowSource()
	{
	}

	/// <summary>
	/// Deserializes object data from the provided SerializationInfo.
	/// </summary>
	/// <param name="info">Contains the serialized object data.</param>
	/// <param name="context"></param>
	public virtual void SetObjectData(SerializationInfo info, StreamingContext context)
	{
		Vector3 field = default(Vector3);
		SerializationHelper.DeserializeEnum(ref HC_0002, info, "ShadowType", isflag: false);
		SerializationHelper.DeserializeField(ref field, info, "Position", usedefault: true);
		Position = field;
		SerializationHelper.DeserializeField(ref HCB, info, "Name", usedefault: true);
		SerializationHelper.DeserializeField(ref HC_0012, info, "ShadowQuality", usedefault: true);
		SerializationHelper.DeserializeField(ref HCH, info, "ShadowPrimaryBias", usedefault: true);
		SerializationHelper.DeserializeField(ref HC7, info, "ShadowSecondaryBias", usedefault: true);
		SerializationHelper.DeserializeField(ref HC_0001, info, "ShadowPerSurfaceLOD", usedefault: true);
		SerializationHelper.DeserializeField(ref HCw, info, "ShadowRenderLightsTogether", usedefault: true);
	}

	/// <summary>
	/// Serializes object data to the provided SerializationInfo.
	/// </summary>
	/// <param name="info">SerializationInfo to store the serialized data.</param>
	/// <param name="context"></param>
	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("Name", Name);
		info.AddValue("ShadowType", ShadowType);
		info.AddValue("Position", Position);
		info.AddValue("Radius", Radius);
		info.AddValue("ShadowQuality", ShadowQuality);
		info.AddValue("ShadowPrimaryBias", ShadowPrimaryBias);
		info.AddValue("ShadowSecondaryBias", ShadowSecondaryBias);
		info.AddValue("ShadowPerSurfaceLOD", ShadowPerSurfaceLOD);
		info.AddValue("ShadowRenderLightsTogether", ShadowRenderLightsTogether);
	}
}
