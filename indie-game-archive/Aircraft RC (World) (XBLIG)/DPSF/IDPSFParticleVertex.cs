using Microsoft.Xna.Framework.Graphics;

namespace DPSF;

/// <summary>
/// Interface that must be implemented by all Particle Vertex's
/// </summary>
public interface IDPSFParticleVertex
{
	/// <summary>
	/// An array describing the Elements of each Vertex
	/// </summary>
	VertexElement[] VertexElements { get; }

	/// <summary>
	/// The Size of one Vertex Element in Bytes
	/// </summary>
	int SizeInBytes { get; }
}
