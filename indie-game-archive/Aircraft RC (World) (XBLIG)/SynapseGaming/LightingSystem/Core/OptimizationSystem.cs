using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Lights;
using SynapseGaming.LightingSystem.Rendering;
using SynapseGaming.LightingSystem.Shadows;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Static class used to control SunBurn's built-in optimizations.
/// </summary>
public class OptimizationSystem
{
	/// <summary />
	public static TypeCasters<LightTypeCaster, BaseLight> LightTypeCasters = new TypeCasters<LightTypeCaster, BaseLight>();

	/// <summary />
	public static TypeCasters<ShadowSourceTypeCaster, IShadowSource> ShadowSourceTypeCasters = new TypeCasters<ShadowSourceTypeCaster, IShadowSource>();

	/// <summary />
	public static TypeCasters<EffectTypeCaster, Effect> EffectTypeCasters = new TypeCasters<EffectTypeCaster, Effect>();

	/// <summary />
	public static TypeCasters<SceneEntityTypeCaster, SceneEntity> SceneEntityTypeCasters = new TypeCasters<SceneEntityTypeCaster, SceneEntity>();

	/// <summary>
	/// Clears the internal cache of pre-cast data types. This may become necessary when
	/// creating and removing many new objects from the scenegraph without calling Clear().
	/// </summary>
	public static void Clear()
	{
		LightTypeCasters.Clear();
		ShadowSourceTypeCasters.Clear();
		EffectTypeCasters.Clear();
		SceneEntityTypeCasters.Clear();
	}
}
