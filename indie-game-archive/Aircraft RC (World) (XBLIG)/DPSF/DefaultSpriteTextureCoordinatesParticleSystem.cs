using Microsoft.Xna.Framework;

namespace DPSF;

/// <summary>
/// The Default Sprite with Texture Coordinates Particle System to inherit from, which uses Default Sprite with Texture Coordinates Particles
/// </summary>
public class DefaultSpriteTextureCoordinatesParticleSystem : DPSFDefaultSpriteTextureCoordinatesParticleSystem<DefaultSpriteTextureCoordinatesParticle, DefaultSpriteParticleVertex>
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="cGame">Handle to the Game object being used. Pass in null for this 
	/// parameter if not using a Game object.</param>
	public DefaultSpriteTextureCoordinatesParticleSystem(Game cGame)
		: base(cGame)
	{
	}
}
