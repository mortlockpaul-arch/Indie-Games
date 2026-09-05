using FarseerPhysics.Dynamics;

namespace FarseerPhysics.Controllers;

public abstract class Controller
{
	public bool Enabled;

	public FilterControllerData FilterData;

	public World World;

	public Controller(ControllerType controllerType)
	{
		FilterData = new FilterControllerData(controllerType);
	}

	public abstract void Update(float dt);
}
