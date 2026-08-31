using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.NarrowPhaseSystems.Pairs;

namespace BEPUphysics.Collidables.Events;

/// <summary>
/// Handles any special logic when two objects' bounding boxes overlap as determined by the broadphase system.
/// Unlike PairCreatedEventHandler, this will be called as soon as a pair is created instead of at the end of the frame.
/// This allows the pair's data to be adjusted prior to any usage, but some actions are not supported due to the execution stage.
/// </summary>
/// <param name="sender">Entry sending the event.</param>
/// <param name="other">Other entry within the pair opposing the monitored entry.</param>
/// <param name="pair">Pair presiding over the interaction of the two involved bodies.
/// This reference cannot be safely kept outside of the scope of the handler; pairs can quickly return to the resource pool.</param>
public delegate void CreatingPairEventHandler<T>(T sender, BroadPhaseEntry other, NarrowPhasePair pair);
