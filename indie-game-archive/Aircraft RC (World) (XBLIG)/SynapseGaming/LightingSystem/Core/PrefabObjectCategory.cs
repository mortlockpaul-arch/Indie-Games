namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Defines the type of a prefab object.
/// </summary>
public enum PrefabObjectCategory
{
	/// <summary>
	/// Unknown object type.
	/// </summary>
	Unknown,
	/// <summary>
	/// Object is of the type ISceneEntity or ISceneObject.
	/// </summary>
	Entity,
	/// <summary>
	/// Object is of the type ISceneEntityGroup.
	/// </summary>
	EntityGroup,
	/// <summary>
	/// Object is of the type ILight.
	/// </summary>
	Light,
	/// <summary>
	/// Object is of the type ILightGroup.
	/// </summary>
	LightGroup
}
