using Microsoft.Xna.Framework;

namespace OluXNA;

public struct VectorPositionNormal
{
	public Vector3 position;

	public Vector3 normal;

	public VectorPositionNormal(VectorPositionNormal other)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		position = other.position;
		normal = other.normal;
	}
}
