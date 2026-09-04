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

	public static readonly VertexElement[] VertexElements;

	static ParticleVertex()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		VertexElements = (VertexElement[])(object)new VertexElement[4]
		{
			new VertexElement((short)0, (short)0, (VertexElementFormat)2, (VertexElementMethod)0, (VertexElementUsage)0, (byte)0),
			new VertexElement((short)0, (short)12, (VertexElementFormat)2, (VertexElementMethod)0, (VertexElementUsage)3, (byte)0),
			new VertexElement((short)0, (short)24, (VertexElementFormat)4, (VertexElementMethod)0, (VertexElementUsage)10, (byte)0),
			new VertexElement((short)0, (short)28, (VertexElementFormat)0, (VertexElementMethod)0, (VertexElementUsage)5, (byte)0)
		};
	}
}
