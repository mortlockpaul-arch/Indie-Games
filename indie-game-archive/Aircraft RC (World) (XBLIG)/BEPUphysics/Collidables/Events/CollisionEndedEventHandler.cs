using BEPUphysics.NarrowPhaseSystems.Pairs;

namespace BEPUphysics.Collidables.Events;

/// <summary>
/// Handles any special logic when two bodies go from a touching state to a separated state.
/// </summary>
/// <param name="sender">Entry sending the event.</param>
/// <param name="other">Other entry within the pair opposing the monitored entry.</param>
/// <param name="pair">Pair overseeing the collision.  Note that this instance may be invalid if the entries' bounding boxes no longer overlap.</param>
public delegate void CollisionEndedEventHandler<T>(T sender, Collidable other, CollidablePairHandler pair);
