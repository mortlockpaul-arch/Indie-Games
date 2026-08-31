using BEPUphysics.NarrowPhaseSystems.Pairs;

namespace BEPUphysics.Collidables.Events;

/// <summary>
/// Handles any special logic when two bodies go from a touching state to a separated state.
/// Unlike CollisionEndedEventHandler, this will trigger at the time of contact removal instead of at the end of the space's update.
/// Performing operations outside of the scope of the controller is unsafe.
/// </summary>
/// <param name="sender">Entry sending the event.</param>
/// <param name="other">Other entry within the pair opposing the monitored entry.</param>
/// <param name="pair">Pair presiding over the interaction of the two involved bodies.
/// This reference cannot be safely kept outside of the scope of the handler; pairs can quickly return to the resource pool.</param>
public delegate void CollisionEndingEventHandler<T>(T sender, Collidable other, CollidablePairHandler pair);
