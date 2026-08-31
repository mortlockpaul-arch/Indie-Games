using Microsoft.Xna.Framework;

namespace DPSF;

/// <summary>
/// The Base Particle class from which the Default Particle classes inherit from
/// </summary>
public class DPSFDefaultBaseParticle : DPSFParticle
{
	/// <summary>
	/// The Position of the Particle in 3D space.
	/// <para>NOTE: For 2D Pixel and Sprite Particles, the Z value can still be used to
	/// determine which Particles are drawn in front of others (0.0 = front, 
	/// 1.0 = back) when SpriteBatchOptions.eSortMode = SpriteSortMode.BackToFront
	/// or SpriteSortMode.FrontToBack</para>
	/// </summary>
	public Vector3 Position;

	/// <summary>
	/// The Color of the Particle, or if using a Texture, the Color to incorporate into the Particle's Texture.
	/// <para>NOTE: This Color's alpha value controls the transparency of the Particle's Texture.</para>
	/// <para>NOTE: This should be a Non-Premultipilied color.</para>
	/// </summary>
	public Color Color;

	/// <summary>
	/// The Particle's Velocity
	/// </summary>
	public Vector3 Velocity;

	/// <summary>
	/// The Particle's Acceleration
	/// </summary>
	public Vector3 Acceleration;

	/// <summary>
	/// An External Force that may be applied to the Particle
	/// </summary>
	public Vector3 ExternalForce;

	/// <summary>
	/// The Friction to apply to the Particle
	/// </summary>
	public float Friction;

	/// <summary>
	/// The Particle's Color when it is born.
	/// <para>NOTE: This should be a Non-Premultipilied color.</para>
	/// </summary>
	public Color StartColor;

	/// <summary>
	/// The Particle's Color when it dies
	/// <para>NOTE: This should be a Non-Premultipilied color.</para>
	/// </summary>
	public Color EndColor;

	/// <summary>
	/// Get the Color as a Premultiplied color (i.e. premultiplied alpha).
	/// </summary>
	public Color ColorAsPremultiplied => Color.FromNonPremultiplied(Color.ToVector4());

	/// <summary>
	/// Get the Start Color as a Premultiplied color (i.e. premultiplied alpha).
	/// </summary>
	public Color StartColorAsPremultiplied => Color.FromNonPremultiplied(StartColor.ToVector4());

	/// <summary>
	/// Get the End Color as a Premultiplied color (i.e. premultiplied alpha).
	/// </summary>
	public Color EndColorAsPremultiplied => Color.FromNonPremultiplied(EndColor.ToVector4());

	/// <summary>
	/// Resets the Particle variables to their default values
	/// </summary>
	public override void Reset()
	{
		base.Reset();
		Position = Vector3.Zero;
		Color = Color.White;
		Velocity = (Acceleration = (ExternalForce = Vector3.Zero));
		Friction = 0f;
		StartColor = (EndColor = Color.White);
	}

	/// <summary>
	/// Deep copy all of the Particle properties
	/// </summary>
	/// <param name="ParticleToCopy">The Particle to Copy the properties from</param>
	public override void CopyFrom(DPSFParticle ParticleToCopy)
	{
		DPSFDefaultBaseParticle dPSFDefaultBaseParticle = (DPSFDefaultBaseParticle)ParticleToCopy;
		base.CopyFrom(dPSFDefaultBaseParticle);
		Position = dPSFDefaultBaseParticle.Position;
		Color = dPSFDefaultBaseParticle.Color;
		Velocity = dPSFDefaultBaseParticle.Velocity;
		Acceleration = dPSFDefaultBaseParticle.Acceleration;
		ExternalForce = dPSFDefaultBaseParticle.ExternalForce;
		Friction = dPSFDefaultBaseParticle.Friction;
		StartColor = dPSFDefaultBaseParticle.StartColor;
		EndColor = dPSFDefaultBaseParticle.EndColor;
	}
}
