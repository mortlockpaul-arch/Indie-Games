using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Quasar.Global;

namespace Quasar.GUI;

public abstract class Control : IDisposable
{
	private string id;

	private Dictionary<string, string> parameters = new Dictionary<string, string>(2);

	private bool visible = true;

	private Layout layout;

	private object tag;

	public string Id => id;

	public abstract string ControlType { get; }

	public bool Visible
	{
		get
		{
			return visible;
		}
		set
		{
			visible = value;
		}
	}

	public Layout Layout => layout;

	public object Tag
	{
		get
		{
			return tag;
		}
		set
		{
			tag = value;
		}
	}

	public event Action<Control> OnRemoved;

	public bool GetParameter(string name, out string value)
	{
		return parameters.TryGetValue(name, out value);
	}

	public void SetParameter(string name, string value)
	{
		if (parameters.ContainsKey(name))
		{
			parameters[name] = value;
		}
		else
		{
			parameters.Add(name, value);
		}
	}

	public Control(Layout layout)
	{
		this.layout = layout;
	}

	public virtual void SetId(string id)
	{
		this.id = id;
	}

	public virtual void parseXml(XElement xe)
	{
		id = XDocHelper.GetAttribute(xe, "id");
		foreach (XElement item in xe.Elements())
		{
			if (XDocHelper.HasAttribute(item, "value"))
			{
				parameters.Add(item.Name.LocalName, XDocHelper.GetAttribute(item, "value"));
			}
		}
	}

	public void Removed()
	{
		if (OnRemoved != null)
		{
			OnRemoved(this);
		}
	}

	public virtual void Dispose()
	{
		OnRemoved = null;
		layout = null;
	}
}
