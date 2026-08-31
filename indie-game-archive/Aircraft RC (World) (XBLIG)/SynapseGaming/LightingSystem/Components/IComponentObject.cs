namespace SynapseGaming.LightingSystem.Components;

/// <summary>
/// Interface used by objects that contain components.
/// </summary>
/// <typeparam name="T">Type of the parent class or interface
/// that contains the components. This strongly types the
/// components ensuring only components of the correct
/// type can be assigned.
///
/// For instance all classes and objects that derive from SceneEntity
/// use the ISceneEntity type allowing them to share components.
///
/// However lights use the ILight type to ensure entity components
/// cannot accidently be assigned to them, and vice versa.</typeparam>
public interface IComponentObject<T> where T : class
{
	/// <summary>
	/// Container that stores, manages, and updates the object's components.
	/// </summary>
	ComponentCollection<T> Components { get; }
}
