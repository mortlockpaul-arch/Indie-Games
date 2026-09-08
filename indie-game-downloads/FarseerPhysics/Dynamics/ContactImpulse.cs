using FarseerPhysics.Common;

namespace FarseerPhysics.Dynamics;

public struct ContactImpulse
{
	public FixedArray2<float> NormalImpulses;

	public FixedArray2<float> TangentImpulses;
}
