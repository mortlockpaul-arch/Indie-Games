using SynapseGaming.LightingSystem.Core;

namespace SynapseGaming.LightingSystem.Editor;

/// <summary>
/// Interface used by classes listed in the SunBurn editor's list of creatable object types.
/// </summary>
/// <typeparam name="T">The base interface type used by the editor tab in which the class
/// should appear. For the "Scene Objects" tab use the ISceneEntity interface. No other
/// tabs are supported at this time.</typeparam>
public interface IEditorCreatedObject<T> : IEditorObject, INamedObject
{
	/// <summary>
	/// Deep clones the object including any contained sub-objects and components.
	/// </summary>
	/// <returns></returns>
	T Clone();

	/// <summary>
	/// Called when the object is created in the SunBurn editor.
	/// </summary>
	void OnCreatedInEditor();
}
