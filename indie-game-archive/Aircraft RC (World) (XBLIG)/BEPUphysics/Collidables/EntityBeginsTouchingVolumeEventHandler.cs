using BEPUphysics.Entities;

namespace BEPUphysics.Collidables;

/// <summary>
/// Handles any special logic to perform when an entry begins touching a detector volume.
/// Runs within an update loop for updateables; modifying the updateable listing during the event is disallowed.
/// </summary>
/// <param name="volume">DetectorVolume being touched.</param>
/// <param name="toucher">Entry touching the volume.</param>
public delegate void EntityBeginsTouchingVolumeEventHandler(DetectorVolume volume, Entity toucher);
