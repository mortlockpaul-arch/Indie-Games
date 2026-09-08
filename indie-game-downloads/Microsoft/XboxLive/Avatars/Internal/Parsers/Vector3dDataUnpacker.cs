using Microsoft.XboxLive.MathUtilities;

namespace Microsoft.XboxLive.Avatars.Internal.Parsers;

public class Vector3dDataUnpacker : DataUnpackerGeneric<Vector3>
{
	public struct QuantizedVector
	{
		public int x;

		public int y;

		public int z;
	}

	public const float QUANTRADIUS_TO_DELTA_MUL0 = 2f;

	public const float QUANTRADIUS_TO_DELTA_MUL1 = 1.6329931f;

	public const float QUANTRADIUS_TO_DELTA_MUL2 = 1.7320508f;

	public const float DELTA_TO_INSET_X_MUL = 0.5f;

	public const float DELTA_TO_INSET_Z_MUL = 0.57735026f;

	public int m_BitCountX;

	public int m_BitCountY;

	public int m_BitCountZ;

	public float m_MinX;

	public float m_MinY;

	public float m_MinZ;

	public float m_QuantRadius;

	public float m_Delta0;

	public float m_Delta1;

	public float m_Delta2;

	public override int GetHeaderBitCount()
	{
		return 146;
	}

	public override int GetPerDataBitCount()
	{
		return m_BitCountX + m_BitCountY + m_BitCountZ;
	}

	public override void UnpackHeader(BitStream bitStream)
	{
		m_QuantRadius = bitStream.ReadFloat();
		m_MinX = bitStream.ReadFloat();
		m_MinY = bitStream.ReadFloat();
		m_MinZ = bitStream.ReadFloat();
		m_BitCountX = bitStream.ReadInt(6);
		m_BitCountY = bitStream.ReadInt(6);
		m_BitCountZ = bitStream.ReadInt(6);
		m_Delta0 = m_QuantRadius * 2f;
		m_Delta1 = m_QuantRadius * 1.6329931f;
		m_Delta2 = m_QuantRadius * 1.7320508f;
	}

	public override void UnpackData(BitStream bitStream, out Vector3 data)
	{
		QuantizedVector quantizedVector = new QuantizedVector
		{
			x = bitStream.ReadInt(m_BitCountX),
			y = bitStream.ReadInt(m_BitCountY),
			z = bitStream.ReadInt(m_BitCountZ)
		};
		data = default(Vector3);
		data.Y = m_MinY + m_Delta1 * (float)quantizedVector.y;
		data.Z = m_MinZ + m_Delta2 * (float)quantizedVector.z;
		if ((quantizedVector.y & 1) != 0)
		{
			data.Z += 0.57735026f * m_Delta2;
		}
		data.X = m_MinX + m_Delta0 * (float)quantizedVector.x;
		if ((quantizedVector.y & 1) != (quantizedVector.z & 1))
		{
			data.X += 0.5f * m_Delta0;
		}
	}

	public void InvertCoordinateSystem()
	{
		m_MinZ = 0f - m_MinZ;
		m_Delta2 = 0f - m_Delta2;
	}
}
