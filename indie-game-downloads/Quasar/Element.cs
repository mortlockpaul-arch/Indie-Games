using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Quasar.Elements;
using Quasar.Render;

namespace Quasar;

public class Element : IDisposable
{
	private bool active = true;

	private bool visible = true;

	protected Transform transform = new Transform();

	protected Element parent;

	public Color DebugColor = Color.Purple;

	public string Name;

	protected List<Element> children = new List<Element>(2);

	protected List<Behavior> behaviors;

	protected List<Light> affectLights;

	private Dictionary<string, Quasar.ElementAttribute> attributes;

	protected List<string> layers;

	protected readonly bool IsPureElement;

	protected readonly bool IsPureItem;

	public bool Active
	{
		get
		{
			return active;
		}
		set
		{
			active = value;
		}
	}

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

	public Transform Transform => transform;

	public Element Parent
	{
		get
		{
			return parent;
		}
		set
		{
			parent = value;
		}
	}

	public List<Element> Children => children;

	public override string ToString()
	{
		if (Name != null && Name.Length > 0)
		{
			return Name;
		}
		return base.ToString();
	}

	public Element()
	{
		IsPureElement = (object)GetType() == typeof(Element);
		IsPureItem = (object)GetType() == typeof(RenderItem);
	}

	public void addChild(Element child)
	{
		child.Parent = this;
		children.Add(child);
	}

	public void insertChild(int index, Element child)
	{
		child.Parent = this;
		children.Insert(index, child);
	}

	public void removeChild(Element child)
	{
		if (children.Remove(child))
		{
			child.Parent = null;
		}
	}

	public void clearChildren()
	{
		foreach (Element child in children)
		{
			child.Parent = null;
		}
		children.Clear();
	}

	public void addBehavior(Behavior behavior)
	{
		if (behavior.Compatible(this))
		{
			behavior.PrepareElement(this);
			if (behaviors == null)
			{
				behaviors = new List<Behavior>(1);
			}
			behaviors.Add(behavior);
		}
	}

	public void insertBehavior(int index, Behavior behavior)
	{
		behavior.PrepareElement(this);
		if (behaviors == null)
		{
			behaviors = new List<Behavior>(1);
		}
		behaviors.Insert(index, behavior);
	}

	public void removeBehavior(Behavior behavior)
	{
		if (behaviors != null && behaviors.Remove(behavior))
		{
			behavior.ClearElement(this);
		}
	}

	public void clearBehaviors()
	{
		if (behaviors == null)
		{
			return;
		}
		foreach (Behavior behavior in behaviors)
		{
			behavior.ClearElement(this);
		}
		behaviors.Clear();
	}

	public void setAttribute(string key, object attribute)
	{
		setAttribute(key, new Quasar.ElementAttribute(attribute));
	}

	public void setAttribute(string key, float attribute)
	{
		setAttribute(key, new Quasar.ElementAttribute(attribute));
	}

	public void setAttribute(string key, int attribute)
	{
		setAttribute(key, new Quasar.ElementAttribute(attribute));
	}

	public void setAttribute(string key, long attribute)
	{
		setAttribute(key, new Quasar.ElementAttribute(attribute));
	}

	public void setAttribute(string key, Vector4 attribute)
	{
		setAttribute(key, new Quasar.ElementAttribute(attribute));
	}

	private void setAttribute(string key, Quasar.ElementAttribute attribute)
	{
		if (attributes == null)
		{
			attributes = new Dictionary<string, Quasar.ElementAttribute>(1);
		}
		if (!attributes.TryGetValue(key, out var _))
		{
			attributes.Add(key, attribute);
		}
		else
		{
			attributes[key] = attribute;
		}
	}

	public object getAttributeObject(string key)
	{
		Quasar.ElementAttribute? attribute = getAttribute(key);
		if (!attribute.HasValue)
		{
			return null;
		}
		return attribute.Value.ObjectValue;
	}

	public float? getAttributeFloat(string key)
	{
		Quasar.ElementAttribute? attribute = getAttribute(key);
		if (!attribute.HasValue)
		{
			return null;
		}
		return attribute.Value.FloatValue;
	}

	public int? getAttributeInt(string key)
	{
		Quasar.ElementAttribute? attribute = getAttribute(key);
		if (!attribute.HasValue)
		{
			return null;
		}
		return attribute.Value.IntValue;
	}

	public long? getAttributeLong(string key)
	{
		Quasar.ElementAttribute? attribute = getAttribute(key);
		if (!attribute.HasValue)
		{
			return null;
		}
		return attribute.Value.LongValue;
	}

	public Vector4? getAttributeVector4(string key)
	{
		Quasar.ElementAttribute? attribute = getAttribute(key);
		if (!attribute.HasValue)
		{
			return null;
		}
		return attribute.Value.Vector4Value;
	}

	private Quasar.ElementAttribute? getAttribute(string key)
	{
		if (attributes == null)
		{
			return null;
		}
		if (attributes.TryGetValue(key, out var value))
		{
			return value;
		}
		return null;
	}

	public void clearAttribute(string key)
	{
		if (attributes != null)
		{
			attributes.Remove(key);
		}
	}

	public void setLayer(string layer)
	{
		if (layers == null)
		{
			layers = new List<string>(1);
		}
		if (!layers.Contains(layer))
		{
			layers.Add(layer);
		}
	}

	public void clearLayer(string layer)
	{
		if (layers != null)
		{
			layers.Remove(layer);
		}
	}

	public bool hasLayer(string layer)
	{
		if (layers == null)
		{
			return false;
		}
		return layers.Contains(layer);
	}

	public void addLight(Light light)
	{
		if (affectLights == null)
		{
			affectLights = new List<Light>(1);
		}
		affectLights.Add(light);
	}

	public void removeLight(Light light)
	{
		if (affectLights != null)
		{
			affectLights.Remove(light);
		}
	}

	public void clearLights()
	{
		if (affectLights != null)
		{
			affectLights.Clear();
		}
	}

	public void Render()
	{
		if (!visible)
		{
			return;
		}
		if (layers != null)
		{
			foreach (string layer in layers)
			{
				SceneRenderData.CurrentRenderData.ActiveLayers.Add(layer);
			}
		}
		if (SceneRenderData.CurrentRenderData.CanRenderLayers())
		{
			if (affectLights != null)
			{
				foreach (Light affectLight in affectLights)
				{
					SceneRenderData.CurrentRenderData.CurrentLights.Add(affectLight);
				}
			}
			if (!IsPureElement)
			{
				DoRender();
			}
			int count = children.Count;
			for (int i = 0; i < count; i++)
			{
				children[i].Render();
			}
			if (affectLights != null && affectLights.Count > 0)
			{
				int index = SceneRenderData.CurrentRenderData.CurrentLights.Count - affectLights.Count;
				SceneRenderData.CurrentRenderData.CurrentLights.RemoveRange(index, affectLights.Count);
			}
		}
		if (layers != null)
		{
			int index2 = SceneRenderData.CurrentRenderData.ActiveLayers.Count - layers.Count;
			SceneRenderData.CurrentRenderData.ActiveLayers.RemoveRange(index2, layers.Count);
		}
	}

	public void Update()
	{
		if (!active)
		{
			return;
		}
		int count;
		if (behaviors != null)
		{
			count = behaviors.Count;
			for (int i = 0; i < count; i++)
			{
				behaviors[i].Update(this);
			}
		}
		if (!IsPureElement && !IsPureItem)
		{
			DoUpdate();
		}
		count = children.Count;
		for (int j = 0; j < count; j++)
		{
			Element element = children[j];
			element.transform.SetParent(transform);
			element.Update();
		}
	}

	protected virtual void DoUpdate()
	{
	}

	protected virtual void DoRender()
	{
	}

	public Element FindNodeWithName(string name, bool recursive)
	{
		if (Name == name)
		{
			return this;
		}
		if (recursive)
		{
			Element element = null;
			foreach (Element child in children)
			{
				element = child.FindNodeWithName(name, recursive);
				if (element != null)
				{
					return element;
				}
			}
		}
		return null;
	}

	public Element FindNodeWithName(string name)
	{
		return FindNodeWithName(name, recursive: true);
	}

	public virtual void Dispose()
	{
		if (children != null)
		{
			clearChildren();
		}
		children = null;
		if (behaviors != null)
		{
			clearBehaviors();
		}
		behaviors = null;
		if (attributes != null)
		{
			attributes.Clear();
		}
		attributes = null;
		if (affectLights != null)
		{
			affectLights.Clear();
		}
		affectLights = null;
		parent = null;
		GC.SuppressFinalize(this);
	}

	~Element()
	{
		Dispose();
	}
}
