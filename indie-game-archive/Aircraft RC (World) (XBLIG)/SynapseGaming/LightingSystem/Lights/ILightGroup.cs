using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Shadows;

namespace SynapseGaming.LightingSystem.Lights;

/// <summary>
/// Interface used for light groups, which help organizing scene lights within a rig.
/// </summary>
public interface ILightGroup : IGroup<ILight>, IShadowSource, IEditorCreatedObject<ILightGroup>, IEditorObject, INamedObject
{
	/// <summary>
	/// Determines if the group acts as a shared shadow source for all contained
	/// lights. This allows a considerable performance increase over per-light shadows.
	/// </summary>
	bool ShadowGroup { get; set; }

	/// <summary>
	/// Shadow source location when used as a shadow group.
	/// </summary>
	Vector3 Position { get; set; }

	/// <summary>
	/// Readonly list of the contained lights.
	/// </summary>
	IList<ILight> Lights { get; }
}
