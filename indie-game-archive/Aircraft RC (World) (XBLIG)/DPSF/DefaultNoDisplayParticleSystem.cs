using Microsoft.Xna.Framework;

namespace DPSF;

/// <summary>
/// The Default No Display Particle System to inherit from, which uses Default Pixel Particles
/// </summary>
public class DefaultNoDisplayParticleSystem : DPSFDefaultNoDisplayParticleSystem<DefaultNoDisplayParticle, DefaultNoDisplayParticleVertex>
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="cGame">Handle to the Game object being used. Pass in null for this 
	/// parameter if not using a Game object.</param>
	public DefaultNoDisplayParticleSystem(Game cGame)
		: base(cGame)
	{
	}
}
