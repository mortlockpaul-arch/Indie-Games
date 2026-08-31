using Microsoft.Xna.Framework;

namespace DPSF;

/// <summary>
/// The Default Textured Quad Particle System to inherit from, which uses Default Textured Quad Particles
/// </summary>
public class DefaultTexturedQuadParticleSystem : DPSFDefaultTexturedQuadParticleSystem<DefaultTexturedQuadParticle, DefaultTexturedQuadParticleVertex>
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="cGame">Handle to the Game object being used. Pass in null for this 
	/// parameter if not using a Game object.</param>
	public DefaultTexturedQuadParticleSystem(Game cGame)
		: base(cGame)
	{
	}
}
