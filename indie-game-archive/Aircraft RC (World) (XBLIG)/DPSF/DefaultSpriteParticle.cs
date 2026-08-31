using Microsoft.Xna.Framework.Graphics;

namespace DPSF;

/// <summary>
/// Particle used by the Default Sprite Particle System
/// </summary>
public class DefaultSpriteParticle : DPSFDefaultBaseParticle
{
	/// <summary>
	/// How much the Particle should be Rotated
	/// </summary>
	public float Rotation;

	/// <summary>
	/// The Width of the Particle
	/// </summary>
	public float Width;

	/// <summary>
	/// The Height of the Particle
	/// </summary>
	public float Height;

	/// <summary>
	/// Tells if the Sprite should be flipped Horizontally or Vertically
	/// </summary>
	public SpriteEffects FlipMode;

	/// <summary>
	/// The Particle's Rotational Velocity
	/// </summary>
	public float RotationalVelocity;

	/// <summary>
	/// The Particle's Rotational Acceleration
	/// </summary>
	public float RotationalAcceleration;

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
		Rotation = 0f;
		Width = (Height = 10f);
		FlipMode = SpriteEffects.None;
		RotationalVelocity = (RotationalAcceleration = 0f);
		StartWidth = (StartHeight = (EndWidth = (EndHeight = 10f)));
	}

	/// <summary>
	/// Deep copy all of the Particle properties
	/// </summary>
	/// <param name="ParticleToCopy">The Particle to Copy the properties from</param>
	public override void CopyFrom(DPSFParticle ParticleToCopy)
	{
		DefaultSpriteParticle defaultSpriteParticle = (DefaultSpriteParticle)ParticleToCopy;
		base.CopyFrom((DPSFParticle)defaultSpriteParticle);
		Rotation = defaultSpriteParticle.Rotation;
		Width = defaultSpriteParticle.Width;
		Height = defaultSpriteParticle.Height;
		FlipMode = defaultSpriteParticle.FlipMode;
		RotationalVelocity = defaultSpriteParticle.RotationalVelocity;
		RotationalAcceleration = defaultSpriteParticle.RotationalAcceleration;
		StartWidth = defaultSpriteParticle.StartWidth;
		StartHeight = defaultSpriteParticle.StartHeight;
		EndWidth = defaultSpriteParticle.EndWidth;
		EndHeight = defaultSpriteParticle.EndHeight;
	}
}
