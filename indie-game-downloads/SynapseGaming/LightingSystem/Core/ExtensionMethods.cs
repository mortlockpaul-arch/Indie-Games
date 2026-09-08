using SynapseGaming.LightingSystem.Collision;
using SynapseGaming.LightingSystem.Collision.Legacy;
using SynapseGaming.LightingSystem.Rendering;

namespace SynapseGaming.LightingSystem.Core;

/// <summary />
public static class ExtensionMethods
{
	/// <summary>
	/// Creates and adds a default set of manager services. This makes
	/// initializing the SceneInterface easier.
	/// </summary>
	/// <param name="sceneinterface"></param>
	/// <param name="renderingsystemtype">Determines if deferred or forward rendering should be used.</param>
	/// <param name="collisionsystemtype">Determines if physics or legacy collision should be used.</param>
	/// <param name="autoloadpluginmanagers">Determines if 3rd party plugins should automatically be loaded.</param>
	public static void CreateDefaultManagers(this SceneInterface sceneinterface, RenderingSystemType renderingsystemtype, CollisionSystemType collisionsystemtype, bool autoloadpluginmanagers)
	{
		sceneinterface.CreateDefaultManagers(renderingsystemtype, autoloadpluginmanagers);
		if (collisionsystemtype == CollisionSystemType.Physics)
		{
			sceneinterface.AddManager(new PhysicsManager(sceneinterface));
		}
		else
		{
			sceneinterface.AddManager(new CollisionManager(sceneinterface));
		}
	}
}
