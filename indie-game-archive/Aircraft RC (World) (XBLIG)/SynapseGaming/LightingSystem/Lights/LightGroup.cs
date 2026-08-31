using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Serialization;
using SynapseGaming.LightingSystem.Shadows;
using Z;

namespace SynapseGaming.LightingSystem.Lights;

/// <summary>
/// Light group object used to help organizing scene lights within a rig.
/// </summary>
[Serializable]
public class LightGroup : ShadowSource, IFullSerializable, ISerializable, ILightGroup, IGroup<ILight>, IShadowSource, IEditorCreatedObject<ILightGroup>, IEditorObject, INamedObject
{
	private bool HCB;

	private List<ILight> HC_0002 = new List<ILight>(16);

	private IList<ILight> HC_0012;

	/// <summary>
	/// Readonly list of the contained lights.
	/// </summary>
	public IList<ILight> Lights => HC_0012;

	/// <summary>
	/// Determines if the group acts as a shared shadow source for all contained
	/// lights. This allows a considerable performance increase over per-light shadows.
	/// </summary>
	public bool ShadowGroup
	{
		get
		{
			return HCB;
		}
		set
		{
			HCB = value;
			IShadowSource shadowSource = null;
			if (value)
			{
				shadowSource = this;
			}
			foreach (ILight item in HC_0002)
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
		HC_0002.Add(light);
		if (HCB)
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
		HC_0002.Remove(light);
		light.ShadowSource = null;
	}

	/// <summary>
	/// Removes the light at a specific index.
	/// </summary>
	/// <param name="index"></param>
	public void RemoveAt(int index)
	{
		Remove(HC_0002[index]);
	}

	/// <summary>
	/// Removes all lights from the group.
	/// </summary>
	public void Clear()
	{
		foreach (ILight item in HC_0002)
		{
			item.ShadowSource = null;
		}
		HC_0002.Clear();
	}

	/// <summary>
	/// Creates a LightGroup instance.
	/// </summary>
	public LightGroup()
	{
		base.Name = "Group";
		B();
	}

	private void B()
	{
		HC_0012 = HC_0002.AsReadOnly();
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
		Z._7._0002w(this, lightGroup);
		foreach (ILight item in HC_0002)
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
		B();
		foreach (SerializationEntry item in info)
		{
			switch (item.Name)
			{
			case "Lights":
				HC_0002.AddRange((List<ILight>)info.GetValue("Lights", typeof(List<ILight>)));
				break;
			case "ShadowGroup":
				HCB = (bool)info.GetValue("ShadowGroup", typeof(bool));
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
		info.AddValue("ShadowGroup", HCB);
		info.AddValue("Lights", HC_0002);
	}
}
