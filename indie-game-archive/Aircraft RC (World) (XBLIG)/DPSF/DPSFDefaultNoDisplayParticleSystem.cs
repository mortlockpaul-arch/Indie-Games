using Microsoft.Xna.Framework;

namespace DPSF;

/// <summary>
/// The Default No Display Particle System class
/// </summary>
/// <typeparam name="Particle">The Particle class to use</typeparam>
/// <typeparam name="Vertex">The Vertex Format to use</typeparam>
public class DPSFDefaultNoDisplayParticleSystem<Particle, Vertex> : DPSFDefaultBaseParticleSystem<Particle, Vertex> where Particle : DPSFParticle, new() where Vertex : struct, IDPSFParticleVertex
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="cGame">Handle to the Game object being used. Pass in null for this 
	/// parameter if not using a Game object.</param>
	public DPSFDefaultNoDisplayParticleSystem(Game cGame)
		: base(cGame)
	{
	}
}
