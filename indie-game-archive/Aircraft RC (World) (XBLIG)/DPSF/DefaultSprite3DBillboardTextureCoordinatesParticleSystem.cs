using Microsoft.Xna.Framework;

namespace DPSF;

/// <summary>
/// The Default Sprite Particle System to inherit from, which uses Default Sprite Particles.
/// </summary>
public class DefaultSprite3DBillboardTextureCoordinatesParticleSystem : DPSFDefaultSprite3DBillboardTextureCoordinates<DefaultSprite3DBillboardTextureCoordinatesParticle, DefaultSpriteParticleVertex>
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="cGame">Handle to the Game object being used. Pass in null for this.
	/// parameter if not using a Game object.</param>
	public DefaultSprite3DBillboardTextureCoordinatesParticleSystem(Game cGame)
		: base(cGame)
	{
	}
}
