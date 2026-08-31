using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DPSF;

/// <summary>
/// Structure used to hold a Default Quad Particle's Vertex's properties used for drawing.
/// This contains a Vector3 Position and a Color Color.
/// </summary>
public struct DefaultQuadParticleVertex : IDPSFParticleVertex
{
	private const int miSizeInBytes = 16;

	/// <summary>
	/// The Position of the vertex in 3D space. The position of this vertex
	/// relative to the quads other three vertices determines the Particle's orientation.
	/// </summary>
	public Vector3 Position;

	/// <summary>
	/// The Color of the vertex
	/// </summary>
	public Color Color;

	private static readonly VertexElement[] msVertexElements = new VertexElement[2]
	{
		new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
		new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0)
	};

	/// <summary>
	/// An array describing the attributes of each Vertex
	/// </summary>
	public VertexElement[] VertexElements => msVertexElements;

	/// <summary>
	/// The Size of one Vertex in Bytes
	/// </summary>
	public int SizeInBytes => 16;
}
