using System.Collections.Generic;

namespace ProjectMercury.Controllers;

public class ControllerCollection : List<Controller>
{
	public ParticleEffect Owner { get; internal set; }

	public new void Add(Controller controller)
	{
		if (!Contains(controller))
		{
			controller.ParticleEffect = Owner;
			base.Add(controller);
		}
	}

	public new void Remove(Controller controller)
	{
		if (Contains(controller))
		{
			controller.ParticleEffect = null;
			base.Remove(controller);
		}
	}
}
