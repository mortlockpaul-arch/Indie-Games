using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;

namespace DPSF;

/// <summary>
/// Dummy structure used for the vertices of a No Display particle system.
/// Since the particles are not drawn, they do not have vertices, so this structure is empty.
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct DefaultNoDisplayParticleVertex : IDPSFParticleVertex
{
	/// <summary>
	/// An array describing the attributes of each Vertex
	/// </summary>
	public VertexElement[] VertexElements => null;

	/// <summary>
	/// The Size of one Vertex in Bytes
	/// </summary>
	public int SizeInBytes => 0;
}
