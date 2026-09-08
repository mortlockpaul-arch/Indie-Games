using System;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Interface used by manager objects that provide services to other managers and
/// game code via IManagerServiceProvider.
/// </summary>
public interface IManagerService : IManager, IUnloadable
{
	/// <summary>
	/// Gets the manager specific Type used as a unique key for storing and
	/// requesting the manager from the IManagerServiceProvider.
	/// </summary>
	Type ManagerType { get; }

	/// <summary>
	/// Sets the order this manager is processed relative to other managers
	/// in the IManagerServiceProvider. Managers with lower processing order
	/// values are processed first.
	///
	/// In the case of BeginFrameRendering and EndFrameRendering, BeginFrameRendering
	/// is processed in the normal order (lowest order value to highest), however
	/// EndFrameRendering is processed in reverse order (highest to lowest) to ensure
	/// the first manager begun is the last one ended (FILO).
	/// </summary>
	int ManagerProcessOrder { get; set; }
}
