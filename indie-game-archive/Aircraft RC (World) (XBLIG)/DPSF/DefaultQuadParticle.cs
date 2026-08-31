using Microsoft.Xna.Framework;

namespace DPSF;

/// <summary>
/// Particle used by the Default Quad Particle System
/// </summary>
public class DefaultQuadParticle : DPSFDefaultBaseParticle
{
	/// <summary>
	/// The Orientation of the Particle
	/// </summary>
	public Quaternion Orientation;

	/// <summary>
	/// The Rotational Velocity of the Particle.
	/// X = Pitch Velocity, Y = Yaw Velocity, Z = Roll Velocity in radians
	/// </summary>
	public Vector3 RotationalVelocity;

	/// <summary>
	/// The Rotational Acceleration of the Particle.
	/// X = Pitch Acceleration, Y = Yaw Acceleration, Z = Roll Acceleration in radians
	/// </summary>
	public Vector3 RotationalAcceleration;

	/// <summary>
	/// The Width of the Particle
	/// </summary>
	public float Width;

	/// <summary>
	/// The Height of the Particle
	/// </summary>
	public float Height;

	/// <summary>
	/// The Width of the Particle when it is born
	/// </summary>
	public float StartWidth;

	/// <summary>
	/// The Height of the Particle when it is born
	/// </summary>
	public float StartHeight;

	/// <summary>
	/// The Width of the Particle when it dies
	/// </summary>
	public float EndWidth;

	/// <summary>
	/// The Height of the Particle when it dies
	/// </summary>
	public float EndHeight;

	/// <summary>
	/// The squared distance between this particle and the camera.
	/// <para>NOTE: This property is only used if you are sorting the particles based on their distance 
	/// from the camera, otherwise you can use this property for whatever you like.</para>
	/// </summary>
	public float DistanceFromCameraSquared;

	/// <summary>
	/// Sets the Width and Height properties to the given value.
	/// Gets the Width value, ignoring whether the Height value is the same or not.
	/// </summary>
	public float Size
	{
		get
		{
			return Width;
		}
		set
		{
			Width = value;
			Height = value;
		}
	}

	/// <summary>
	/// Sets the StartWidth and StartHeight properties to the given value.
	/// Gets the StartWidth value, ignoring whether the StartHeight value is the same or not.
	/// </summary>
	public float StartSize
	{
		get
		{
			return StartWidth;
		}
		set
		{
			StartWidth = value;
			StartHeight = value;
		}
	}

	/// <summary>
	/// Sets the EndWidth and EndHeight properties to the given value.
	/// Gets the EndWidth value, ignoring whether the EndHeight value is the same or not.
	/// </summary>
	public float EndSize
	{
		get
		{
			return EndWidth;
		}
		set
		{
			EndWidth = value;
			EndHeight = value;
		}
	}

	/// <summary>
	/// Get / Set the Normal (forward) direction of the Particle (i.e. which direction it is facing)
	/// </summary>
	public Vector3 Normal
	{
		get
		{
			return Orientation3D.GetNormalDirection(Orientation);
		}
		set
		{
			Orientation3D.SetNormalDirection(ref Orientation, value);
		}
	}

	/// <summary>
	/// Get / Set the Up direction of the Particle
	/// </summary>
	public Vector3 Up
	{
		get
		{
			return Orientation3D.GetUpDirection(Orientation);
		}
		set
		{
			Orientation3D.SetUpDirection(ref Orientation, value);
		}
	}

	/// <summary>
	/// Get / Set the Right direction of the Particle
	/// </summary>
	public Vector3 Right
	{
		get
		{
			return Orientation3D.GetRightDirection(Orientation);
		}
		set
		{
			Orientation3D.SetRightDirection(ref Orientation, value);
		}
	}

	/// <summary>
	/// Scales the Width and Height by the given amount.
	/// </summary>
	/// <param name="scale">The amount to scale the Width and Height by.</param>
	public void Scale(float scale)
	{
		Width *= scale;
		Height *= scale;
	}

	/// <summary>
	/// Updates the Width to the given value and uniformly scales the Height to maintain the width-to-height ratio.
	/// </summary>
	/// <param name="newWidth">The Width the particle should have.</param>
	public void ScaleToWidth(float newWidth)
	{
		if (Width != 0f)
		{
			float num = newWidth / Width;
			Height *= num;
			Width = newWidth;
		}
	}

	/// <summary>
	/// Updates the Height to the given value and uniformly scales the Width to maintain the width-to-height ratio.
	/// </summary>
	/// <param name="newHeight">The Height the particle should have.</param>
	public void ScaleToHeight(float newHeight)
	{
		if (Height != 0f)
		{
			float num = newHeight / Height;
			Width *= num;
			Height = newHeight;
		}
	}

	/// <summary>
	/// Resets the Particle variables to their default values
	/// </summary>
	public override void Reset()
	{
		base.Reset();
		Orientation = Quaternion.Identity;
		RotationalVelocity = (RotationalAcceleration = Vector3.Zero);
		Width = (Height = 10f);
		StartWidth = (StartHeight = (EndWidth = (EndHeight = 10f)));
		DistanceFromCameraSquared = 0f;
	}

	/// <summary>
	/// Deep copy all of the Particle properties
	/// </summary>
	/// <param name="ParticleToCopy">The Particle to Copy the properties from</param>
	public override void CopyFrom(DPSFParticle ParticleToCopy)
	{
		DefaultQuadParticle defaultQuadParticle = (DefaultQuadParticle)ParticleToCopy;
		base.CopyFrom((DPSFParticle)defaultQuadParticle);
		Orientation = defaultQuadParticle.Orientation;
		RotationalVelocity = defaultQuadParticle.RotationalVelocity;
		RotationalAcceleration = defaultQuadParticle.RotationalAcceleration;
		Width = defaultQuadParticle.Width;
		Height = defaultQuadParticle.Height;
		StartHeight = defaultQuadParticle.StartHeight;
		StartWidth = defaultQuadParticle.StartWidth;
		EndHeight = defaultQuadParticle.EndHeight;
		EndWidth = defaultQuadParticle.EndWidth;
		DistanceFromCameraSquared = defaultQuadParticle.DistanceFromCameraSquared;
	}
}
