using System;
using SynapseGaming.LightingSystem.Serialization;

namespace SynapseGaming.LightingSystem.Components;

/// <summary>
/// Base class used to create components with auto serialization support.
///
/// To serialize a field add the [SerializeMember] attribute to it. Please note:
/// for the data to be visible in-editor it must be provided via a property.
/// This requires all serialized editor visible members to implement a
/// property with a backing field where the property is editor visible and
/// the backing field serialized by adding the [SerializeMember] attribute.
///
/// Provides built-in support for sending inter-component messages, calling
/// OnInitialize() when the parent object changes, and automatic ComponentType
/// using the derived class type.
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
[Serializable]
[SerializationInclusionModel]
public class BaseComponentAutoSerialization<T> : BaseComponent<T> where T : class, IComponentObject<T>
{
}
