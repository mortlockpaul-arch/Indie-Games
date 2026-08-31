using SynapseGaming.LightingSystem.Core;

namespace SynapseGaming.LightingSystem.Editor;

/// <summary>
/// Interface that provides scene entities and objects with rendering of in-editor icons and helpers.
/// </summary>
public interface IEditorRenderableObject
{
	/// <summary>
	/// Implements rendering of in-editor icons and helpers.
	///
	/// This method is called twice per-frame: once with scene depth clipping enable, and once with it disabled.
	/// </summary>
	/// <param name="scenestate">Current state used to render the scene.</param>
	/// <param name="renderhelper">Helper used to draw lines associated with the object. Only calling Submit() is
	/// supported in this method, using other methods may affect rendering of lines drawn by other objects.</param>
	/// <param name="highlighted">Indicates if the object is currently highlighted by the editor.</param>
	/// <param name="selected">Indicates if the object is currently selected by the editor.</param>
	/// <param name="sceneoccludedpass">Indicates if the current rendering pass depth clips with the scene.
	/// If so rendered icons and helpers are occluded by scene objects.</param>
	void RenderEditorIcon(ISceneState scenestate, BoundingBoxRenderHelper renderhelper, bool highlighted, bool selected, bool sceneoccludedpass);
}
