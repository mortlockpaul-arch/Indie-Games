using System.Collections.Generic;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Interface used for scene entity groups, which help organizing scene entities within a scene.
/// </summary>
public interface ISceneEntityGroup : IGroup<ISceneEntity>, IEditorCreatedObject<ISceneEntityGroup>, IEditorObject, INamedObject
{
	/// <summary>
	/// Readonly list of the contained scene objects.
	/// </summary>
	IList<ISceneEntity> Entities { get; }
}
