namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Generic interface used by container objects that implement submitting
/// and removing other objects.
/// </summary>
/// <typeparam name="T">Type of objects that will be submitted and removed.</typeparam>
public interface ISubmit<T>
{
	/// <summary>
	/// Adds an object to the container. This does not transfer ownership, disposable
	/// objects should be maintained and disposed separately.
	/// </summary>
	/// <param name="obj"></param>
	void Submit(T obj);

	/// <summary>
	/// Repositions an object within the container. This method is used when the container
	/// implements a tree or graph, and relocates an object within that structure
	/// often due to a change in object world position.
	/// </summary>
	/// <param name="obj"></param>
	void Move(T obj);

	/// <summary>
	/// Removes an object from the container.
	/// </summary>
	/// <param name="obj"></param>
	void Remove(T obj);

	/// <summary>
	/// Removes all objects from the container. Commonly used while clearing the scene.
	/// </summary>
	void Clear();
}
