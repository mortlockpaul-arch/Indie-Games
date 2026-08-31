using Microsoft.Xna.Framework;

namespace DPSF;

/// <summary>
/// The Default Quad Particle System to inherit from, which uses Default Quad Particles
/// </summary>
public class DefaultQuadParticleSystem : DPSFDefaultQuadParticleSystem<DefaultQuadParticle, DefaultQuadParticleVertex>
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="cGame">Handle to the Game object being used. Pass in null for this 
	/// parameter if not using a Game object.</param>
	public DefaultQuadParticleSystem(Game cGame)
		: base(cGame)
	{
	}
}
