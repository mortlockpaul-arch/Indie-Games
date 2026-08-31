using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Serialization;
using Z;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Light group object used to help organizing scene lights within a rig.
/// </summary>
[Serializable]
[EditorObject(true)]
public class SceneEntityGroup : IFullSerializable, ISerializable, ISceneEntityGroup, IGroup<ISceneEntity>, IEditorCreatedObject<ISceneEntityGroup>, IEditorObject, INamedObject
{
	private List<ISceneEntity> HCB = new List<ISceneEntity>(16);

	private IList<ISceneEntity> HC_0002;

	[CompilerGenerated]
	private string HC_0012;

	[CompilerGenerated]
	private bool HCH;

	/// <summary>
	/// Readonly list of the contained scene objects.
	/// </summary>
	public IList<ISceneEntity> Entities => HC_0002;

	/// <summary>
	/// The object's current name.
	/// </summary>
	[EditorProperty(true, Description = "Name", HorizontalAlignment = true, MajorGrouping = 1, MinorGrouping = 1, ToolTipText = "")]
	public string Name
	{
		[CompilerGenerated]
		get
		{
			return HC_0012;
		}
		[CompilerGenerated]
		set
		{
			HC_0012 = value;
		}
	}

	/// <summary>
	/// Notifies the editor that this object is partially controlled via code. The editor
	/// will display information to the user indicating some property values are
	/// overridden in code and changes may not take effect.
	/// </summary>
	public bool AffectedInCode
	{
		[CompilerGenerated]
		get
		{
			return HCH;
		}
		[CompilerGenerated]
		set
		{
			HCH = value;
		}
	}

	/// <summary>
	/// Adds an object to the group.
	/// </summary>
	/// <param name="obj"></param>
	public void Add(ISceneEntity obj)
	{
		HCB.Add(obj);
	}

	/// <summary>
	/// Removes an object from the group.
	/// </summary>
	/// <param name="obj"></param>
	public void Remove(ISceneEntity obj)
	{
		HCB.Remove(obj);
	}

	/// <summary>
	/// Removes the object at a specific index.
	/// </summary>
	/// <param name="index"></param>
	public void RemoveAt(int index)
	{
		Remove(HCB[index]);
	}

	/// <summary>
	/// Removes all objects from the group.
	/// </summary>
	public void Clear()
	{
		HCB.Clear();
	}

	/// <summary>
	/// Creates a LightGroup instance.
	/// </summary>
	public SceneEntityGroup()
	{
		Name = "Group";
		B();
	}

	private void B()
	{
		HC_0002 = HCB.AsReadOnly();
	}

	/// <summary>
	/// Called when the object is created in the SunBurn editor.
	/// </summary>
	public virtual void OnCreatedInEditor()
	{
	}

	/// <summary>
	/// Deep clones the object including any contained sub-objects and components.
	/// </summary>
	/// <returns></returns>
	public virtual ISceneEntityGroup Clone()
	{
		SceneEntityGroup sceneEntityGroup = new SceneEntityGroup();
		Z._7._0002w(this, sceneEntityGroup);
		foreach (ISceneEntity item in HCB)
		{
			sceneEntityGroup.Add(item.Clone());
		}
		return sceneEntityGroup;
	}

	/// <summary>
	/// Deserializes object data from the provided SerializationInfo.
	/// </summary>
	/// <param name="info">Contains the serialized object data.</param>
	/// <param name="context"></param>
	public void SetObjectData(SerializationInfo info, StreamingContext context)
	{
		B();
		foreach (SerializationEntry item in info)
		{
			switch (item.Name)
			{
			case "Entities":
				HCB.AddRange((List<ISceneEntity>)info.GetValue("Entities", typeof(List<ISceneEntity>)));
				break;
			case "Name":
				Name = (string)info.GetValue("Name", typeof(string));
				break;
			}
		}
	}

	/// <summary>
	/// Serializes object data to the provided SerializationInfo.
	/// </summary>
	/// <param name="info">SerializationInfo to store the serialized data.</param>
	/// <param name="context"></param>
	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("Entities", HCB);
		info.AddValue("Name", Name);
	}
}
