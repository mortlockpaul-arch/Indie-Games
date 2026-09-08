using SynapseGaming.LightingSystem.Core;

namespace SynapseGaming.LightingSystem.Collision;

/// <summary>
/// Delegate used when an object passes through or overlaps a trigger.
/// </summary>
/// <param name="collider">The moving object.</param>
/// <param name="trigger">The trigger hit by the moving object.</param>
public delegate void CollisionTriggerDelegate(IMovableObject collider, IMovableObject trigger);
