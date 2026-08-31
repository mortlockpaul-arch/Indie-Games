using BEPUphysics.BroadPhaseEntries;

namespace BEPUphysics.Collidables.Events;

/// <summary>
/// Handles any special logic when two objects' bounding boxes cease to overlap as determined by the broadphase system.
/// Unlike PairRemovedEventHandler, this will trigger at the time of pair removal instead of at the end of the space's update.
/// </summary>
/// <param name="sender">Entry sending the event.</param>
/// <param name="other">The entry formerly interacting with the sender via the deleted pair.</param>
public delegate void RemovingPairEventHandler<T>(T sender, BroadPhaseEntry other);
