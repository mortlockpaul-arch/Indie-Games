using Microsoft.Xna.Framework;

namespace DPSF;

/// <summary>
/// The Default Animated Textured Quad Particle System to inherit from, which uses Default Animated Textured Quad Particles
/// </summary>
public class DefaultAnimatedTexturedQuadParticleSystem : DPSFDefaultAnimatedTexturedQuadParticleSystem<DefaultAnimatedTexturedQuadParticle, DefaultTexturedQuadParticleVertex>
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="cGame">Handle to the Game object being used. Pass in null for this 
	/// parameter if not using a Game object.</param>
	public DefaultAnimatedTexturedQuadParticleSystem(Game cGame)
		: base(cGame)
	{
	}
}
