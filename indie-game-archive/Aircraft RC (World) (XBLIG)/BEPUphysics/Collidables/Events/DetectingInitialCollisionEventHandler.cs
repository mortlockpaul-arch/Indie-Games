using BEPUphysics.NarrowPhaseSystems.Pairs;

namespace BEPUphysics.Collidables.Events;

/// <summary>
/// Handles any special logic when two bodies initally collide and generate a contact point.
/// Unlike InitialCollisionDetectedEventHandler, this will trigger at the time of contact creation instead of at the end of the space's update.
/// Performing operations outside of the scope of the pair is unsafe.
/// </summary>
/// <param name="sender">Entry sending the event.</param>
/// <param name="other">Other entry within the pair opposing the monitored entry.</param>
/// <param name="pair">Pair presiding over the interaction of the two involved bodies.
/// This reference cannot be safely kept outside of the scope of the handler; pairs can quickly return to the resource pool.</param>
public delegate void DetectingInitialCollisionEventHandler<T>(T sender, Collidable other, CollidablePairHandler pair);
