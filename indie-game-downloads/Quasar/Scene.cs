using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Quasar.Elements;

namespace Quasar;

public class Scene : IDisposable
{
	private static Scene currentInstance;

	protected List<Light> defaultLights = new List<Light>(3);

	protected float fogStart = float.MaxValue;

	protected float fogRange;

	protected Vector3 fogColor = Vector3.One;

	protected Material globalMaterial = new Material();

	private Camera camera;

	private List<Element> elements = new List<Element>();

	private bool enabled = true;

	public static Scene CurrentInstance
	{
		get
		{
			return currentInstance;
		}
		set
		{
			currentInstance = value;
		}
	}

	public Camera Camera
	{
		get
		{
			return camera;
		}
		set
		{
			camera = value;
		}
	}

	public List<Light> DefaultLights => defaultLights;

	public float FogStart
	{
		get
		{
			return fogStart;
		}
		set
		{
			fogStart = value;
		}
	}

	public float FogRange
	{
		get
		{
			return fogRange;
		}
		set
		{
			fogRange = value;
		}
	}

	public Vector3 FogColor
	{
		get
		{
			return fogColor;
		}
		set
		{
			fogColor = value;
		}
	}

	public Material GlobalMaterial => globalMaterial;

	protected List<Element> Elements => elements;

	public bool Enabled
	{
		get
		{
			return enabled;
		}
		set
		{
			enabled = value;
		}
	}

	public Element GetElementByName(string name)
	{
		foreach (Element element2 in elements)
		{
			Element element = element2.FindNodeWithName(name);
			if (element != null)
			{
				return element;
			}
		}
		return null;
	}

	public void addLight(Light l)
	{
		defaultLights.Add(l);
	}

	public virtual void Render()
	{
		CompatHooks.Mark("Scene.Render");
		if (!enabled)
		{
			return;
		}
		foreach (Element element in elements)
		{
			element.Render();
		}
	}

	public virtual void Update()
	{
		if (enabled)
		{
			CurrentInstance = this;
			int count = elements.Count;
			for (int i = 0; i < count; i++)
			{
				elements[i].Update();
			}
			CurrentInstance = null;
		}
	}

	public void Add(Element e)
	{
		elements.Add(e);
	}

	public void Insert(int index, Element e)
	{
		elements.Insert(index, e);
	}

	public void Remove(Element e)
	{
		elements.Remove(e);
	}

	public virtual void Dispose()
	{
		camera = null;
		if (elements != null)
		{
			elements.Clear();
		}
		elements = null;
		if (defaultLights != null)
		{
			defaultLights.Clear();
		}
		defaultLights = null;
		if (currentInstance == this)
		{
			currentInstance = null;
		}
		GC.SuppressFinalize(this);
	}

	~Scene()
	{
		Dispose();
	}
}
