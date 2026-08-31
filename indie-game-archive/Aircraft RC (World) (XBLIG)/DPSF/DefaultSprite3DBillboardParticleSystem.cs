using Microsoft.Xna.Framework;

namespace DPSF;

/// <summary>
/// The Default Sprite 3D Billboard Particle System to inherit from, which uses Default Sprite 3D Billboard Particles
/// </summary>
public class DefaultSprite3DBillboardParticleSystem : DPSFDefaultSprite3DBillboardParticleSystem<DefaultSprite3DBillboardParticle, DefaultSpriteParticleVertex>
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="cGame">Handle to the Game object being used. Pass in null for this 
	/// parameter if not using a Game object.</param>
	public DefaultSprite3DBillboardParticleSystem(Game cGame)
		: base(cGame)
	{
	}
}
