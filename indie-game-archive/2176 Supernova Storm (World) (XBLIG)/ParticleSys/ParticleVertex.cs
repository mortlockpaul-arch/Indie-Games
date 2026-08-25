using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParticleSys;

internal struct ParticleVertex
{
	public const int SizeInBytes = 32;

	public Vector3 Position;

	public Vector3 Velocity;

	public Color Random;

	public float Time;

	public static readonly VertexElement[] VertexElements = new VertexElement[4]
	{
		new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
		new VertexElement(0, 12, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Normal, 0),
		new VertexElement(0, 24, VertexElementFormat.Color, VertexElementMethod.Default, VertexElementUsage.Color, 0),
		new VertexElement(0, 28, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0)
	};
}
