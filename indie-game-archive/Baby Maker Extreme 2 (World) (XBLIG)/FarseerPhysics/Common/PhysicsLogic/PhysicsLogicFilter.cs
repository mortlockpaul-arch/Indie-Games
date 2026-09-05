namespace FarseerPhysics.Common.PhysicsLogic;

public class PhysicsLogicFilter
{
	public PhysicsLogicType ControllerIgnores;

	public void IgnorePhysicsLogic(PhysicsLogicType type)
	{
		ControllerIgnores |= type;
	}

	public void RestorePhysicsLogic(PhysicsLogicType type)
	{
		ControllerIgnores &= ~type;
	}

	public bool IsPhysicsLogicIgnored(PhysicsLogicType type)
	{
		return (ControllerIgnores & type) == type;
	}
}
