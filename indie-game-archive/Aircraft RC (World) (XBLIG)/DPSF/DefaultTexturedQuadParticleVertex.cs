using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DPSF;

/// <summary>
/// Structure used to hold a Default Textured Quad Particle's Vertex's properties used for drawing.
/// This contains a Vector3 Position, Vector2 TextureCoordinate, and Color Color.
/// </summary>
public struct DefaultTexturedQuadParticleVertex : IDPSFParticleVertex
{
	private const int miSizeInBytes = 24;

	/// <summary>
	/// The Position of the vertex in 3D space. The position of this vertex
	/// relative to the quads other three vertices determines the Particle's orientation.
	/// </summary>
	public Vector3 Position;

	/// <summary>
	/// The Coordinate of the Texture that this Vertex corresponds to
	/// </summary>
	public Vector2 TextureCoordinate;

	/// <summary>
	/// The Color to tint the Texture
	/// </summary>
	public Color Color;

	private static readonly VertexElement[] msVertexElements = new VertexElement[3]
	{
		new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
		new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
		new VertexElement(20, VertexElementFormat.Color, VertexElementUsage.Color, 0)
	};

	/// <summary>
	/// An array describing the attributes of each Vertex
	/// </summary>
	public VertexElement[] VertexElements => msVertexElements;

	/// <summary>
	/// The Size of one Vertex in Bytes
	/// </summary>
	public int SizeInBytes => 24;
}
