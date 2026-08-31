using Microsoft.Xna.Framework;

namespace DPSF;

/// <summary>
/// The Default Animated 3D Billboard Sprite Particle System to inherit from, which uses Default Animated Sprite Particles
/// </summary>
public class DefaultAnimatedSprite3DBillboardParticleSystem : DPSFDefaultAnimatedSprite3DBillboardParticleSystem<DefaultAnimatedSprite3DBillboardParticle, DefaultSpriteParticleVertex>
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="cGame">Handle to the Game object being used. Pass in null for this 
	/// parameter if not using a Game object.</param>
	public DefaultAnimatedSprite3DBillboardParticleSystem(Game cGame)
		: base(cGame)
	{
	}
}
