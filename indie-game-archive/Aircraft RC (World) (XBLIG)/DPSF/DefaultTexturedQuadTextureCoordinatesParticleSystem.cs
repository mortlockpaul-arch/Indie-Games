using Microsoft.Xna.Framework;

namespace DPSF;

/// <summary>
/// The Default Textured Quad with Texture Coordinates Particle System to inherit from, which uses Default Textured Quad Texture Coordinates Particles
/// </summary>
public class DefaultTexturedQuadTextureCoordinatesParticleSystem : DPSFDefaultTexturedQuadTextureCoordinatesParticleSystem<DefaultTextureQuadTextureCoordinatesParticle, DefaultTexturedQuadParticleVertex>
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="cGame">Handle to the Game object being used. Pass in null for this 
	/// parameter if not using a Game object.</param>
	public DefaultTexturedQuadTextureCoordinatesParticleSystem(Game cGame)
		: base(cGame)
	{
	}
}
