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
	private float _3A_0018 = 45f;

	private float _3AL;

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
			return _3A_0018;
		}
		set
		{
			_3A_0018 = value;
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
			return _3AL;
		}
		set
		{
			_3AL = value;
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
		float degrees = MathHelper.Clamp(_3A_0018, 0.001f, 179.99f) * 0.5f;
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
		SerializationHelper.DeserializeField(ref _3A_0018, info, "Angle", usedefault: true);
		SerializationHelper.DeserializeField(ref _3AL, info, "Volume", usedefault: true);
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
