using FarseerPhysics.Common;

namespace FarseerPhysics.Collision;

public struct DistanceInput
{
	public DistanceProxy ProxyA;

	public DistanceProxy ProxyB;

	public Transform TransformA;

	public Transform TransformB;

	public bool UseRadii;
}
