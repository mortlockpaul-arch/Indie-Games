namespace DPSF;

/// <summary>
/// Particle used by the Default Sprite 3D Billboard Particle System
/// </summary>
public class DefaultSprite3DBillboardParticle : DefaultSpriteParticle
{
	/// <summary>
	/// The squared distance between this particle and the camera.
	/// <para>NOTE: This property is only used if you are sorting the particles based on their distance 
	/// from the camera, otherwise you can use this property for whatever you like.</para>
	/// </summary>
	public float DistanceFromCameraSquared;

	/// <summary>
	/// Resets the Particle variables to their default values
	/// </summary>
	public override void Reset()
	{
		base.Reset();
		DistanceFromCameraSquared = 0f;
	}

	/// <summary>
	/// Deep copy all of the Particle properties
	/// </summary>
	/// <param name="ParticleToCopy">The Particle to Copy the properties from</param>
	public override void CopyFrom(DPSFParticle ParticleToCopy)
	{
		DefaultSprite3DBillboardParticle defaultSprite3DBillboardParticle = (DefaultSprite3DBillboardParticle)ParticleToCopy;
		base.CopyFrom((DPSFParticle)defaultSprite3DBillboardParticle);
		DistanceFromCameraSquared = defaultSprite3DBillboardParticle.DistanceFromCameraSquared;
	}
}
