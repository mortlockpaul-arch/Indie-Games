using Microsoft.XboxLive.MathUtilities;

namespace Microsoft.XboxLive.Avatars.Internal.Parsers;

public class QuaternionDataUnpacker : Vector3dDataUnpacker
{
	public void UnpackData(BitStream bitStream, out Quaternion data)
	{
		UnpackData(bitStream, out Vector3 data2);
		data = QuaternionMath.Exp(data2);
	}

	public new void InvertCoordinateSystem()
	{
		m_MinX = 0f - m_MinX;
		m_Delta0 = 0f - m_Delta0;
		m_MinY = 0f - m_MinY;
		m_Delta1 = 0f - m_Delta1;
	}
}
