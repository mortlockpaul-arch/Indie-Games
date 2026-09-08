using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Quasar.ContentPipeline;
using Quasar.Global;

namespace Quasar.Xml;

public static class ElementLoader
{
	private static Dictionary<string, BaseElementLoader> factories;

	private static Quasar.Xml.XmlElementLoader baseLoader;

	static ElementLoader()
	{
		factories = new Dictionary<string, BaseElementLoader>(4);
		baseLoader = new Quasar.Xml.XmlElementLoader();
		factories.Add("Element", baseLoader);
		factories.Add("Skybox", new Quasar.Xml.SkyboxElementLoader());
		factories.Add("ParticleSystem", new Quasar.Xml.ParticleSystemElementLoader());
		factories.Add("Model", new Quasar.Xml.ModelElementLoader());
	}

	public static BaseElementLoader GetLoader(string name)
	{
		BaseElementLoader value = null;
		factories.TryGetValue(name, out value);
		return value;
	}

	public static void Load(string filename, Element destElement)
	{
		try
		{
			XmlSource xmlSource = Engine.ContentManager.Load<XmlSource>(filename);
			XDocument xDocument = XDocument.Parse(xmlSource.XmlCode);
			string directoryName = Path.GetDirectoryName(filename);
			baseLoader.Load(xDocument.Root, destElement, directoryName);
		}
		catch (Exception)
		{
		}
	}

	public static Element Load(string filename)
	{
		try
		{
			XmlSource xmlSource = Engine.ContentManager.Load<XmlSource>(filename);
			XDocument xDocument = XDocument.Parse(xmlSource.XmlCode);
			string currentPath = "/" + Path.GetDirectoryName(filename) + "/";
			return baseLoader.Load(xDocument.Root, currentPath);
		}
		catch (Exception)
		{
		}
		return new Element();
	}
}
