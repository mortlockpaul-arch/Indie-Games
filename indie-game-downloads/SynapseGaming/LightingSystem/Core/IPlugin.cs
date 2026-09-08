namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Interface used by plugins to make it possible to easily package, share, and use
/// a number of custom managers, components, and other classes.
///
/// The plugin class receives a call to Initialize() for each
/// SceneInterface the game utilizes, and can use this event to add custom
/// managers to the SceneInterface, register editor context menu items,
/// run custom startup code, and more.
///
/// Several plugins can be contained within the same assembly, and once
/// initialized other "discoverable" classes contained in the library
/// (like custom SunBurn components) will automatically become available
/// to the engine and editor.
///
/// Custom forms and dialogs can be added to the SunBurn editor by
/// registering a new context menu item using:
///
///     SunBurnEditor.AddCustomContextMenuItem(),
///
/// ..and showing the form / dialog in the item OnClick event handler.
///
/// Plugins that are packaged and installed using a SunBurn "*.sbpack"
/// installer and added to a project with the Plugin Manager tool
/// are automatically registered and loaded by the engine.
///
/// Plugins that do not use the SunBurn Packager and Plugin Manager tools
/// can be manually registered using:
///
///     SunBurnCoreSystem.ManuallyLoadPlugin{PluginClass}();
///
/// </summary>
public interface IPlugin
{
	/// <summary>
	/// The plugins receive a call to Initialize() for each
	/// SceneInterface the game utilizes, and can use this event to add custom
	/// managers to the SceneInterface, register editor context menu items,
	/// run custom startup code, and more.
	/// </summary>
	void Initialize(IManagerServiceProvider sceneinterface);

	/// <summary>
	/// Called when the game's graphics and disposable resources are no longer
	/// used or are invalid (due to exiting the game or the graphics device
	/// resetting).  All plugin resources should be disposed before exiting this method.
	/// </summary>
	void Unload();
}
