using System;
using System.Collections.Generic;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Interface used by objects containing and providing manager services
/// to other managers and game code.
/// </summary>
public interface IManagerServiceProvider
{
	/// <summary>
	/// Adds a manager service to the provider.
	/// </summary>
	/// <param name="manager"></param>
	void AddManager(IManagerService manager);

	/// <summary>
	/// Removes a manager service from the provider.
	/// </summary>
	/// <param name="manager"></param>
	void RemoveManager(IManagerService manager);

	/// <summary>
	/// Retrieves a manager service by type from the provider.
	/// </summary>
	/// <param name="managertype">Type used by the manager as a unique
	/// identifying key (IManagerService.ManagerType).</param>
	/// <param name="required">Determines whether an exception should
	/// be thrown if the manager is not found.</param>
	/// <returns></returns>
	IManagerService GetManager(Type managertype, bool required);

	/// <summary>
	/// Retrieves a manager service by type from the provider.
	/// </summary>
	/// <typeparam name="T">Type used by the manager as a unique
	/// identifying key (IManagerService.ManagerType).</typeparam>
	/// <param name="required">Determines whether an exception should
	/// be thrown if the manager is not found.</param>
	/// <returns></returns>
	T GetManager<T>(bool required) where T : class;

	/// <summary>
	/// Retrieves all manager services from the provider.
	/// </summary>
	/// <param name="managers">List used to store manager services.</param>
	void GetManagers(List<IManagerService> managers);

	/// <summary>
	/// Resorts the contained manager services.
	///
	/// Providers should automatically resort when manager services
	/// are added and removed, however manual resorting is necessary
	/// if a manager service's ManagerProcessOrder property changes
	/// after being added to the provider.
	/// </summary>
	void ResortServices();
}
