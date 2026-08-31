using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.NarrowPhaseSystems.Pairs;

namespace BEPUphysics.Collidables.Events;

/// <summary>
/// Handles any special logic to perform at the end of a pair's UpdateContactManifold method.
/// This is called every single update regardless if the process was quit early or did not complete due to interaction rules.
/// </summary>
/// <param name="sender">Entry involved in the pair monitored for events.</param>
/// <param name="other">Other entry within the pair opposing the monitored entry.</param>
/// <param name="pair">Pair that was updated.</param>
public delegate void PairUpdatedEventHandler<T>(T sender, BroadPhaseEntry other, NarrowPhasePair pair);
