using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using _0003;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Serialization;
using SynapseGaming.LightingSystem.Shadows;

namespace SynapseGaming.LightingSystem.Lights;

/// <summary>
/// Light group object used to help organizing scene lights within a rig.
/// </summary>
[Serializable]
public class LightGroup : ShadowSource, IFullSerializable, ISerializable, ILightGroup, IGroup<ILight>, IShadowSource, IEditorCreatedObject<ILightGroup>, IEditorObject, INamedObject
{
	private bool _3A_0018;

	private List<ILight> _3AL = new List<ILight>(16);

	private IList<ILight> _3A_0019;

	/// <summary>
	/// Readonly list of the contained lights.
	/// </summary>
	public IList<ILight> Lights => _3A_0019;

	/// <summary>
	/// Determines if the group acts as a shared shadow source for all contained
	/// lights. This allows a considerable performance increase over per-light shadows.
	/// </summary>
	public bool ShadowGroup
	{
		get
		{
			return _3A_0018;
		}
		set
		{
			_3A_0018 = value;
			IShadowSource shadowSource = null;
			if (value)
			{
				shadowSource = this;
			}
			foreach (ILight item in _3AL)
			{
				item.ShadowSource = shadowSource;
			}
		}
	}

	/// <summary>
	/// Adds a light to the group.
	/// </summary>
	/// <param name="light"></param>
	public void Add(ILight light)
	{
		_3AL.Add(light);
		if (_3A_0018)
		{
			light.ShadowSource = this;
		}
		else
		{
			light.ShadowSource = null;
		}
	}

	/// <summary>
	/// Removes a light to the group.
	/// </summary>
	/// <param name="light"></param>
	public void Remove(ILight light)
	{
		_3AL.Remove(light);
		light.ShadowSource = null;
	}

	/// <summary>
	/// Removes the light at a specific index.
	/// </summary>
	/// <param name="index"></param>
	public void RemoveAt(int index)
	{
		Remove(_3AL[index]);
	}

	/// <summary>
	/// Removes all lights from the group.
	/// </summary>
	public void Clear()
	{
		foreach (ILight item in _3AL)
		{
			item.ShadowSource = null;
		}
		_3AL.Clear();
	}

	/// <summary>
	/// Creates a LightGroup instance.
	/// </summary>
	public LightGroup()
	{
		base.Name = "Group";
		_0018();
	}

	private void _0018()
	{
		_3A_0019 = _3AL.AsReadOnly();
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
	public virtual ILightGroup Clone()
	{
		LightGroup lightGroup = new LightGroup();
		_0003._6.L_0003(this, lightGroup);
		foreach (ILight item in _3AL)
		{
			lightGroup.Add(item.Clone());
		}
		return lightGroup;
	}

	/// <summary>
	/// Deserializes object data from the provided SerializationInfo.
	/// </summary>
	/// <param name="info">Contains the serialized object data.</param>
	/// <param name="context"></param>
	public override void SetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.SetObjectData(info, context);
		_0018();
		foreach (SerializationEntry item in info)
		{
			switch (item.Name)
			{
			case "Lights":
				_3AL.AddRange((List<ILight>)info.GetValue("Lights", typeof(List<ILight>)));
				break;
			case "ShadowGroup":
				_3A_0018 = (bool)info.GetValue("ShadowGroup", typeof(bool));
				break;
			}
		}
		ShadowGroup = ShadowGroup;
	}

	/// <summary>
	/// Serializes object data to the provided SerializationInfo.
	/// </summary>
	/// <param name="info">SerializationInfo to store the serialized data.</param>
	/// <param name="context"></param>
	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		info.AddValue("ShadowGroup", _3A_0018);
		info.AddValue("Lights", _3AL);
	}
}
