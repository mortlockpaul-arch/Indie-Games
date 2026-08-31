using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using SynapseGaming.LightingSystem.Serialization;

namespace SynapseGaming.LightingSystem.Components;

/// <summary>
/// Base class used to create components with manual serialization support.
///
/// To serialize a member override the GetObjectData() and SetObjectData() methods
/// and use the SerializationHelper class to read / write the member to the
/// provided SerializationInfo object.
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
public class BaseComponentManualSerialization<T> : BaseComponent<T>, IFullSerializable, ISerializable where T : class, IComponentObject<T>
{
	/// <summary>
	/// Deserializes object data from the provided SerializationInfo.
	/// </summary>
	/// <param name="info">Contains the serialized object data.</param>
	/// <param name="context"></param>
	public virtual void SetObjectData(SerializationInfo info, StreamingContext context)
	{
	}

	/// <summary>
	/// Serializes object data to the provided SerializationInfo.
	/// </summary>
	/// <param name="info">SerializationInfo to store the serialized data.</param>
	/// <param name="context"></param>
	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
	{
	}
}
