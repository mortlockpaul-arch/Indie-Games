using System;

namespace SynapseGaming.LightingSystem.Lights;

/// <summary>
/// Determines if lighting is real-time or bake-down.
/// </summary>
[Flags]
public enum LightingType
{
	/// <summary>
	/// Lighting is calculated in real-time via shaders.
	/// </summary>
	RealTime = 1,
	/// <summary>
	/// Lighting is calculated in-editor and stored in light map textures.
	/// </summary>
	BakedDown = 2
}
