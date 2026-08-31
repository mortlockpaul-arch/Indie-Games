namespace DPSF;

/// <summary>
/// Particle used by the Default Animated Sprite Particle System
/// </summary>
public class DefaultAnimatedSpriteParticle : DefaultSpriteTextureCoordinatesParticle
{
	/// <summary>
	/// Class to hold this Particle's Animation information
	/// </summary>
	public Animations Animation;

	/// <summary>
	/// Resets the Particle variables to their default values
	/// </summary>
	public override void Reset()
	{
		base.Reset();
		Animation = new Animations();
	}

	/// <summary>
	/// Deep copy all of the Particle properties
	/// </summary>
	/// <param name="ParticleToCopy">The Particle to Copy the properties from</param>
	public override void CopyFrom(DPSFParticle ParticleToCopy)
	{
		DefaultAnimatedSpriteParticle defaultAnimatedSpriteParticle = (DefaultAnimatedSpriteParticle)ParticleToCopy;
		base.CopyFrom((DPSFParticle)defaultAnimatedSpriteParticle);
		Animation.CopyFrom(defaultAnimatedSpriteParticle.Animation);
	}
}
