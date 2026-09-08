using FarseerPhysics.Dynamics;

namespace FarseerPhysics.Common.PhysicsLogic;

public class FilterPhysicsLogicData : FilterData
{
	private PhysicsLogicType _type;

	public FilterPhysicsLogicData(PhysicsLogicType type)
	{
		_type = type;
	}

	public override bool IsActiveOn(Body body)
	{
		if (body.PhysicsLogicFilter.IsPhysicsLogicIgnored(_type))
		{
			return false;
		}
		return base.IsActiveOn(body);
	}
}
