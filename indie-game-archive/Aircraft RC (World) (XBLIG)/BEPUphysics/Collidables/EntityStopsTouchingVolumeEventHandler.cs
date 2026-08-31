using BEPUphysics.Entities;

namespace BEPUphysics.Collidables;

/// <summary>
/// Handles any special logic to perform when an entry stops touching a detector volume.
/// Runs within an update loop for updateables; modifying the updateable listing during the event is disallowed.
/// </summary>
/// <param name="volume">DetectorVolume no longer being touched.</param>
/// <param name="toucher">Entry no longer touching the volume.</param>
public delegate void EntityStopsTouchingVolumeEventHandler(DetectorVolume volume, Entity toucher);
