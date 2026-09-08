using SynapseGaming.LightingSystem.Core;

namespace SynapseGaming.LightingSystem.Editor;

/// <summary>
/// Interface that provides scenes the ability to display as active / inactive in the editor.
/// </summary>
public interface IEditorActiveObject : IEditorObject, INamedObject
{
	/// <summary>
	/// Notifies the editor that the object is currently used for rendering. The editor
	/// will display unused / inactive objects as grayed-out.
	/// </summary>
	bool AssetInUse { get; }
}
