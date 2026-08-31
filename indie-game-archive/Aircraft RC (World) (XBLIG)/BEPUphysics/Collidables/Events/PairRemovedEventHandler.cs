using BEPUphysics.BroadPhaseEntries;

namespace BEPUphysics.Collidables.Events;

/// <summary>
/// Handles any special logic when two objects' bounding boxes cease to overlap as determined by the broadphase system.
/// </summary>
/// <param name="sender">Entry sending the event.</param>
/// <param name="other">The entry formerly interacting with the sender via the deleted pair.</param>
public delegate void PairRemovedEventHandler<T>(T sender, BroadPhaseEntry other);
