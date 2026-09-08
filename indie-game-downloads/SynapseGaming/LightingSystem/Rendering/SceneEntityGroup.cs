using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using _0003;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Serialization;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Light group object used to help organizing scene lights within a rig.
/// </summary>
[Serializable]
[EditorObject(true)]
public class SceneEntityGroup : IFullSerializable, ISerializable, ISceneEntityGroup, IGroup<ISceneEntity>, IEditorCreatedObject<ISceneEntityGroup>, IEditorObject, INamedObject
{
	private List<ISceneEntity> _3A_0018 = new List<ISceneEntity>(16);

	private IList<ISceneEntity> _3AL;

	[CompilerGenerated]
	private string _3A_0019;

	[CompilerGenerated]
	private bool _3A3;

	/// <summary>
	/// Readonly list of the contained scene objects.
	/// </summary>
	public IList<ISceneEntity> Entities => _3AL;

	/// <summary>
	/// The object's current name.
	/// </summary>
	[EditorProperty(true, Description = "Name", HorizontalAlignment = true, MajorGrouping = 1, MinorGrouping = 1, ToolTipText = "")]
	public string Name
	{
		[CompilerGenerated]
		get
		{
			return _3A_0019;
		}
		[CompilerGenerated]
		set
		{
			_3A_0019 = value;
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
			return _3A3;
		}
		[CompilerGenerated]
		set
		{
			_3A3 = value;
		}
	}

	/// <summary>
	/// Adds an object to the group.
	/// </summary>
	/// <param name="obj"></param>
	public void Add(ISceneEntity obj)
	{
		_3A_0018.Add(obj);
	}

	/// <summary>
	/// Removes an object from the group.
	/// </summary>
	/// <param name="obj"></param>
	public void Remove(ISceneEntity obj)
	{
		_3A_0018.Remove(obj);
	}

	/// <summary>
	/// Removes the object at a specific index.
	/// </summary>
	/// <param name="index"></param>
	public void RemoveAt(int index)
	{
		Remove(_3A_0018[index]);
	}

	/// <summary>
	/// Removes all objects from the group.
	/// </summary>
	public void Clear()
	{
		_3A_0018.Clear();
	}

	/// <summary>
	/// Creates a LightGroup instance.
	/// </summary>
	public SceneEntityGroup()
	{
		Name = "Group";
		_0018();
	}

	private void _0018()
	{
		_3AL = _3A_0018.AsReadOnly();
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
		_0003._6.L_0003(this, sceneEntityGroup);
		foreach (ISceneEntity item in _3A_0018)
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
		_0018();
		foreach (SerializationEntry item in info)
		{
			switch (item.Name)
			{
			case "Entities":
				_3A_0018.AddRange((List<ISceneEntity>)info.GetValue("Entities", typeof(List<ISceneEntity>)));
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
		info.AddValue("Entities", _3A_0018);
		info.AddValue("Name", Name);
	}
}
