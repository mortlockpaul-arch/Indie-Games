using System.Collections.Generic;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Lights;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Interface used by objects that store, share, and organize scene entities, objects, and lights.
/// </summary>
public interface IScene : IEditorActiveObject, IEditorObject, INamedObject
{
	/// <summary>
	/// Light groups contained by the scene.
	/// </summary>
	List<ILightGroup> LightGroups { get; }

	/// <summary>
	/// Scene object groups contained by the scene.
	/// </summary>
	List<ISceneEntityGroup> EntityGroups { get; }

	/// <summary>
	/// Sets the single manager of the specified type that contains this scene.
	///
	/// Scenes can only be contained by a single manager of a specific type.
	/// </summary>
	/// <typeparam name="T">Type of the specified manager. This is often the
	/// manager interface type, not the class type.</typeparam>
	/// <param name="manager">Manager object that contains the scene.</param>
	void SetContainingManager<T>(IManager manager);

	/// <summary>
	/// Applies changes made to contained objects and groups.  This must be called after
	/// making changes and before rendering the scene.
	/// </summary>
	void Apply();

	/// <summary>
	/// Removes all objects and groups.
	/// </summary>
	void Clear();
}
