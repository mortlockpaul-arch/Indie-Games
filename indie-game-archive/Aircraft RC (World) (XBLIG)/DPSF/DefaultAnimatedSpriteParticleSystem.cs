using Microsoft.Xna.Framework;

namespace DPSF;

/// <summary>
/// The Default Animated Sprite Particle System to inherit from, which uses Default Animated Sprite Particles
/// </summary>
public class DefaultAnimatedSpriteParticleSystem : DPSFDefaultAnimatedSpriteParticleSystem<DefaultAnimatedSpriteParticle, DefaultSpriteParticleVertex>
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="cGame">Handle to the Game object being used. Pass in null for this 
	/// parameter if not using a Game object.</param>
	public DefaultAnimatedSpriteParticleSystem(Game cGame)
		: base(cGame)
	{
	}
}
