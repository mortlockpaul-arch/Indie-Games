using SynapseGaming.LightingSystem.Core;

namespace SynapseGaming.LightingSystem.Editor;

/// <summary>
/// Interface used by game objects available through the editor.
/// </summary>
public interface IEditorObject : INamedObject
{
	/// <summary>
	/// Notifies the editor that the object is partially controlled via code. The editor
	/// will display information to the user indicating some property values are
	/// overridden in code and changes may not take effect.
	/// </summary>
	bool AffectedInCode { get; set; }
}
