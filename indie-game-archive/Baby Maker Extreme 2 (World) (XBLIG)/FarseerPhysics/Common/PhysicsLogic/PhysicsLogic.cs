using FarseerPhysics.Dynamics;

namespace FarseerPhysics.Common.PhysicsLogic;

public abstract class PhysicsLogic
{
	public FilterPhysicsLogicData FilterData;

	public World World;

	public PhysicsLogic(World world, PhysicsLogicType type)
	{
		FilterData = new FilterPhysicsLogicData(type);
		World = world;
	}
}
