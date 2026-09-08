using FarseerPhysics.Common;

namespace FarseerPhysics.Collision;

public struct TOIInput
{
	public DistanceProxy ProxyA;

	public DistanceProxy ProxyB;

	public Sweep SweepA;

	public Sweep SweepB;

	public float TMax;
}
