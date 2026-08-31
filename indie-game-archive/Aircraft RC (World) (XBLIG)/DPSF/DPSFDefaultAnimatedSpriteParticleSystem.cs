using Microsoft.Xna.Framework;

namespace DPSF;

/// <summary>
/// The Default Animated Sprite Particle System class
/// </summary>
/// <typeparam name="Particle">The Particle class to use</typeparam>
/// <typeparam name="Vertex">The Vertex Format to use</typeparam>
public class DPSFDefaultAnimatedSpriteParticleSystem<Particle, Vertex> : DPSFDefaultSpriteTextureCoordinatesParticleSystem<Particle, Vertex> where Particle : DPSFParticle, new() where Vertex : struct, IDPSFParticleVertex
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="cGame">Handle to the Game object being used. Pass in null for this 
	/// parameter if not using a Game object.</param>
	public DPSFDefaultAnimatedSpriteParticleSystem(Game cGame)
		: base(cGame)
	{
	}

	/// <summary>
	/// Updates the Animation, as well as the Particle's Texture Coordinates to match the Animation
	/// </summary>
	/// <param name="cParticle">The Particle to update</param>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticleAnimationAndTextureCoordinates(DefaultAnimatedSpriteParticle cParticle, float fElapsedTimeInSeconds)
	{
		cParticle.Animation.Update(fElapsedTimeInSeconds);
		cParticle.TextureCoordinates = cParticle.Animation.CurrentPicturesTextureCoordinates;
	}

	/// <summary>
	/// Updates the Particle to be removed from the Particle System once the Animation finishes Playing
	/// </summary>
	/// <param name="cParticle">The Particle to update</param>
	/// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
	protected void UpdateParticleToDieOnceAnimationFinishesPlaying(DefaultAnimatedSpriteParticle cParticle, float fElapsedTimeInSeconds)
	{
		if (cParticle.Animation.CurrentAnimationIsDonePlaying)
		{
			cParticle.Lifetime = 1E-06f;
			cParticle.NormalizedElapsedTime = 1f;
		}
	}
}
