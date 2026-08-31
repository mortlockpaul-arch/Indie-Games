using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Lights;
using SynapseGaming.LightingSystem.Serialization;
using Z;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Container object used for storing, sharing, and organizing scene entities, objects, and lights.
/// </summary>
[Serializable]
[EditorObject(true)]
public class Scene : BaseScene
{
	/// <summary>
	/// Creates a Scene instance.
	/// </summary>
	public Scene()
	{
	}

	/// <summary>
	/// Deep clones the object including any contained sub-objects and components.
	/// </summary>
	/// <returns></returns>
	public virtual IScene Clone()
	{
		Scene scene = new Scene();
		Z._7._0002w(this, scene);
		scene.FileName = string.Empty;
		scene.Name = string.Empty;
		foreach (ISceneEntityGroup entityGroup in _EntityGroups)
		{
			scene.EntityGroups.Add(entityGroup.Clone());
		}
		foreach (ILightGroup lightGroup in _LightGroups)
		{
			scene.LightGroups.Add(lightGroup.Clone());
		}
		return scene;
	}

	internal static Scene _0002F(string P_0)
	{
		return SerializationHelper.LoadFromXml<Scene>(P_0);
	}

	/// <summary>
	/// Deserializes object data from the provided SerializationInfo.
	/// </summary>
	/// <param name="info">Contains the serialized object data.</param>
	/// <param name="context"></param>
	public override void SetObjectData(SerializationInfo info, StreamingContext context)
	{
		SerializationHelper.DeserializeField(ref _EntityGroups, info, "EntityGroups", usedefault: false);
		SerializationHelper.DeserializeField(ref _LightGroups, info, "LightGroups", usedefault: false);
	}

	/// <summary>
	/// Serializes object data to the provided SerializationInfo.
	/// </summary>
	/// <param name="info">SerializationInfo to store the serialized data.</param>
	/// <param name="context"></param>
	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("EntityGroups", _EntityGroups);
		info.AddValue("LightGroups", _LightGroups);
	}
}
