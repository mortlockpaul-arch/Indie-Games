using BEPUphysics.Collidables;
using BEPUphysics.Entities;
using BEPUphysics.NarrowPhaseSystems.Pairs;

namespace BEPUphysics.Settings;

/// <summary>
/// Delegate which determines if a given pair should be allowed to run continuous collision detection.
/// This is only called for entities which are continuous and colliding with other objects.
/// </summary>
public delegate bool CCDFilter(Entity entity, Collidable other, CollidablePairHandler pair);
