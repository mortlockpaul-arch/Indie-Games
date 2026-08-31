using System;
using BEPUphysics.Collidables.MobileCollidables;
using BEPUphysics.OtherSpaceStages;

namespace BEPUphysics.Collidables.Events;

/// <summary>
/// Event manager for use with the CompoundCollidable.
/// It's possible to use the ContactEventManager directly with a compound,
/// but without using this class, any child event managers will fail to dispatch
/// deferred events.
/// </summary>
public class CompoundEventManager : ContactEventManager<EntityCollidable>
{
	protected override void DispatchEvents()
	{
		if (owner is CompoundCollidable compoundCollidable)
		{
			foreach (CompoundChild child in compoundCollidable.children)
			{
				IDeferredEventCreator events = child.CollisionInformation.events;
				if (events.IsActive)
				{
					events.DispatchEvents();
				}
			}
			base.DispatchEvents();
			return;
		}
		throw new Exception("Cannot use a CompoundEventManager with anything but a CompoundCollidable.");
	}
}
