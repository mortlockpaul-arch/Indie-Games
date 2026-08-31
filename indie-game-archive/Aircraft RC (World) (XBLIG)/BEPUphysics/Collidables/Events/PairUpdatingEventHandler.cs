using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.NarrowPhaseSystems.Pairs;

namespace BEPUphysics.Collidables.Events;

/// <summary>
/// Handles any special logic to perform at the end of a pair's UpdateContactManifold method.
/// This is called every single update regardless if the process was quit early or did not complete due to interaction rules.
/// Unlike PairUpdatedEventHandler, this is called at the time of the collision detection update rather than at the end of the space's update.
/// Other entries' information may not be up to date, and operations acting on data outside of the character controller may be unsafe.
/// </summary>
/// <param name="sender">Entry involved in the pair monitored for events.</param>
/// <param name="other">Other entry within the pair opposing the monitored entry.</param>
/// <param name="pair">Pair that was updated.</param>
public delegate void PairUpdatingEventHandler<T>(T sender, BroadPhaseEntry other, NarrowPhasePair pair);
