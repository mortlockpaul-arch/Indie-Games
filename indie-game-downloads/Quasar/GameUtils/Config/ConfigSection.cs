using System.Collections.Generic;
using System.Xml.Linq;

namespace Quasar.GameUtils.Config;

public class ConfigSection : IConfigItem
{
	protected Dictionary<string, IConfigItem> items = new Dictionary<string, IConfigItem>(4);

	protected string name;

	public string Name => name;

	public ConfigSection(string name)
	{
		this.name = name;
	}

	public void AddItem(IConfigItem configItem)
	{
		items.Add(configItem.Name, configItem);
	}

	public IConfigItem GetItem(string name)
	{
		if (items.TryGetValue(name, out var value))
		{
			return value;
		}
		return null;
	}

	protected virtual void Gather()
	{
	}

	public virtual void Apply()
	{
		foreach (IConfigItem value in items.Values)
		{
			value.Apply();
		}
	}

	public ConfigSection GetSectionItem(string name)
	{
		return GetItem(name) as ConfigSection;
	}

	public ConfigParameter<T> GetItem<T>(string name)
	{
		return GetItem(name) as ConfigParameter<T>;
	}

	public virtual void FromXml(XElement xe)
	{
		XElement xElement = xe.Element(Name);
		if (xElement != null)
		{
			foreach (IConfigItem value in items.Values)
			{
				value.FromXml(xElement);
			}
			return;
		}
		SetDefaults();
	}

	public virtual void ToXml(XElement xe)
	{
		Gather();
		XElement xElement = new XElement(Name);
		foreach (IConfigItem value in items.Values)
		{
			value.ToXml(xElement);
		}
		xe.Add(xElement);
	}

	public virtual void SetDefaults()
	{
		foreach (IConfigItem value in items.Values)
		{
			value.SetDefaults();
		}
	}
}
