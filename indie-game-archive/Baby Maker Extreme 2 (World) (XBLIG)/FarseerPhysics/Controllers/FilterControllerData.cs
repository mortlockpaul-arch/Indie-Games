using FarseerPhysics.Dynamics;

namespace FarseerPhysics.Controllers;

public class FilterControllerData : FilterData
{
	private ControllerType _type;

	public FilterControllerData(ControllerType type)
	{
		_type = type;
	}

	public override bool IsActiveOn(Body body)
	{
		if (body.ControllerFilter.IsControllerIgnored(_type))
		{
			return false;
		}
		return base.IsActiveOn(body);
	}
}
