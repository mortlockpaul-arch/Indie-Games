namespace DPSF;

/// <summary>
/// Particle used by the Default Animated Sprite 3D Billboard Particle System
/// </summary>
public class DefaultAnimatedSprite3DBillboardParticle : DefaultSprite3DBillboardTextureCoordinatesParticle
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
		DefaultAnimatedSprite3DBillboardParticle defaultAnimatedSprite3DBillboardParticle = (DefaultAnimatedSprite3DBillboardParticle)ParticleToCopy;
		base.CopyFrom((DPSFParticle)defaultAnimatedSprite3DBillboardParticle);
		Animation.CopyFrom(defaultAnimatedSprite3DBillboardParticle.Animation);
	}
}
