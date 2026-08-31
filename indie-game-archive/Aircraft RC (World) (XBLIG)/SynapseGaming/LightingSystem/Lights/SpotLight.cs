using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Serialization;
using SynapseGaming.LightingSystem.Shadows;

namespace SynapseGaming.LightingSystem.Lights;

/// <summary>
/// Provides spotlight information for rendering lighting and shadows.
/// </summary>
[Serializable]
public class SpotLight : PointLight, ISpotSource, IPointSource, IDirectionalSource, IShadowSource
{
	private float HCB = 45f;

	private float HC_0002;

	/// <summary>
	/// Direction in world space of the light's influence.
	/// </summary>
	public Vector3 Direction
	{
		get
		{
			return World.Forward;
		}
		set
		{
			Matrix world = Matrix.Identity;
			if (value != Vector3.Zero)
			{
				world = CoreHelper.CreateMatrixFromNormalizedVectors(Vector3.Forward, Vector3.Normalize(value));
			}
			world.Translation = World.Translation;
			World = world;
		}
	}

	/// <summary>
	/// Angle in degrees of the light's influence.
	/// </summary>
	public float Angle
	{
		get
		{
			return HCB;
		}
		set
		{
			HCB = value;
			UpdateBounds();
		}
	}

	/// <summary>
	/// Intensity of the light's 3D light beam.
	/// </summary>
	public float Volume
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
	/// Creates a new SpotLight instance.
	/// </summary>
	public SpotLight()
	{
	}

	/// <summary />
	protected override void UpdateBounds()
	{
		float degrees = MathHelper.Clamp(HCB, 0.001f, 179.99f) * 0.5f;
		degrees = MathHelper.ToRadians(degrees);
		degrees = (float)Math.Tanh(degrees);
		degrees *= base.Radius;
		BoundingBox boundingbox = new BoundingBox(new Vector3(0f - degrees, 0f - degrees, 0f - base.Radius), new Vector3(degrees, degrees, 0f));
		base.WorldBoundingBox = CoreHelper.TransformBoundingBox(boundingbox, World);
		base.WorldBoundingSphere = BoundingSphere.CreateFromBoundingBox(base.WorldBoundingBox);
	}

	/// <summary>
	/// Deserializes object data from the provided SerializationInfo.
	/// </summary>
	/// <param name="info">Contains the serialized object data.</param>
	/// <param name="context"></param>
	public override void SetObjectData(SerializationInfo info, StreamingContext context)
	{
		Direction = SerializationHelper.DeserializeField<Vector3>(info, "Direction");
		SerializationHelper.DeserializeField(ref HCB, info, "Angle", usedefault: true);
		SerializationHelper.DeserializeField(ref HC_0002, info, "Volume", usedefault: true);
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
		info.AddValue("Direction", Direction);
		info.AddValue("Angle", Angle);
		info.AddValue("Volume", Volume);
		base.GetObjectData(info, context);
	}
}
