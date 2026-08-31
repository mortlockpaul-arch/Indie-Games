using Microsoft.Xna.Framework;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Delegate used when updating movable objects.
/// </summary>
/// <param name="obj">Object to update.</param>
/// <param name="gametime">Time to up date the object to.</param>
public delegate void UpdateDelegate(IMovableObject obj, GameTime gametime);
