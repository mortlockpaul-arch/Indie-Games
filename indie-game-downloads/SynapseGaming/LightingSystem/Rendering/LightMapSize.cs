namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Determines the light map size when generating baked down lighting on an object.
/// </summary>
public enum LightMapSize
{
	/// <summary />
	Size64x64 = 0x40,
	/// <summary />
	Size128x128 = 0x80,
	/// <summary />
	Size256x256 = 0x100,
	/// <summary />
	Size512x512 = 0x200,
	/// <summary />
	Size1024x1024 = 0x400
}
