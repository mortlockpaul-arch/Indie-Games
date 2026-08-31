using BEPUphysics.Entities;

namespace BEPUphysics.Collidables;

/// <summary>
/// Handles any special logic to perform when an entity begins being contained by a detector volume.
/// Runs within an update loop for updateables; modifying the updateable listing during the event is disallowed.
/// </summary>
/// <param name="volume">DetectorVolume containing the entry.</param>
/// <param name="entity">Entity contained by the volume.</param>
public delegate void VolumeBeginsContainingEntityEventHandler(DetectorVolume volume, Entity entity);
