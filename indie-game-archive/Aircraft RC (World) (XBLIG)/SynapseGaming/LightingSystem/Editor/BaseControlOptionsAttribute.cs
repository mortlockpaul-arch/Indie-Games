using System;

namespace SynapseGaming.LightingSystem.Editor;

/// <summary>
/// Abstract base class for all property control options attributes.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public abstract class BaseControlOptionsAttribute : Attribute
{
}
