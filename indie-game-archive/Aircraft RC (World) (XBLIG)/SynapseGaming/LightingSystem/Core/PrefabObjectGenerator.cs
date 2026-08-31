using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;
using SynapseGaming.LightingSystem.Serialization;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Used to generate prefabricated objects from a source object including all
/// serialized members, sub-objects, and components.
///
/// The source object type must include the [Serialize] attribute and if a custom
/// class should ideally implement the IFullSerializable interface.
///
/// Note: objects can only be serialized in Windows. To generate objects on
/// Xbox or Windows Phone 7 the source xml must be saved to a file on Windows
/// and deployed to the Xbox or Windows Phone 7.
/// </summary>
[Serializable]
public class PrefabObjectGenerator : IFullSerializable, ISerializable
{
	private PrefabObjectCategory HCB;

	private string HC_0002 = "";

	private object[] HC_0012 = new object[0];

	private Type[] HCH = new Type[0];

	/// <summary>
	/// Category type of the contained prefab object.
	/// </summary>
	public PrefabObjectCategory Category => HCB;

	/// <summary>
	/// Xml containing the serialized source object.
	/// </summary>
	public string Xml => HC_0002;

	/// <summary>
	/// Creates a PrefabObjectGenerator instance.
	/// </summary>
	public PrefabObjectGenerator()
	{
	}

	/// <summary>
	/// Creates a PrefabObjectGenerator instance from source xml.
	/// </summary>
	/// <param name="xml">Xml containing the serialized source object.</param>
	public PrefabObjectGenerator(string xml)
	{
		HC_0002 = xml;
	}

	internal PrefabObjectGenerator(string P_0, PrefabObjectCategory P_1)
	{
		HC_0002 = P_0;
		HCB = P_1;
	}

	/// <summary>
	/// Creates a new instance of the contained prefab template object.
	/// </summary>
	/// <returns></returns>
	public object CreateObject()
	{
		return CreateObject<object>();
	}

	/// <summary>
	/// Creates a new instance of the contained prefab template object.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public T CreateObject<T>() where T : class
	{
		T val = SerializationHelper.LoadFromXml<T>(HC_0002);
		if (val == null)
		{
			return val;
		}
		Type type = val.GetType();
		MethodInfo method = type.GetMethod("Clone", HCH);
		if ((object)method != null)
		{
			try
			{
				if (method.Invoke(val, HC_0012) is T val2)
				{
					val = val2;
				}
			}
			catch
			{
			}
		}
		return val;
	}

	/// <summary>
	/// Deserializes object data from the provided SerializationInfo.
	/// </summary>
	/// <param name="info">Contains the serialized object data.</param>
	/// <param name="context"></param>
	public virtual void SetObjectData(SerializationInfo info, StreamingContext context)
	{
		SerializationHelper.DeserializeEnum(ref HCB, info, "Category", isflag: false);
		SerializationHelper.DeserializeField(ref HC_0002, info, "Data", usedefault: true);
	}

	/// <summary>
	/// Serializes object data to the provided SerializationInfo.
	/// </summary>
	/// <param name="info">SerializationInfo to store the serialized data.</param>
	/// <param name="context"></param>
	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		SerializationHelper.SerializeFieldOrEnum(ref HCB, info, "Category");
		SerializationHelper.SerializeFieldOrEnum(ref HC_0002, info, "Data");
	}
}
