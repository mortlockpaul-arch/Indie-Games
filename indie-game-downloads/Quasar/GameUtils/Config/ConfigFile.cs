using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Linq;
using Quasar.GameUtils.Storage;

namespace Quasar.GameUtils.Config;

public class ConfigFile : ConfigSection
{
	private string filename;

	public ConfigFile(string filename, string name)
		: base(name)
	{
		this.filename = filename;
	}

	public void Load()
	{
		SetDefaults();
		StorageManager.Instance.Exec(loadData, null, StorageManager.IOType.Read);
		Apply();
	}

	public void Save()
	{
		Gather();
		StorageManager.Instance.Post(saveData, null, StorageManager.IOType.Write);
	}

	private void loadData(IsolatedStorageFile container, object parameters)
	{
		if (container == null || !container.FileExists(filename))
		{
			return;
		}
		try
		{
			XDocument xDocument = XDocument.Load((Stream)container.OpenFile(filename, FileMode.Open));
			foreach (IConfigItem value in items.Values)
			{
				value.FromXml(xDocument.Root);
			}
		}
		catch (Exception)
		{
		}
	}

	private void saveData(IsolatedStorageFile container, object parameters)
	{
		if (container == null)
		{
			return;
		}
		try
		{
			XDocument xDocument = new XDocument();
			XElement xElement = new XElement(name);
			foreach (IConfigItem value in items.Values)
			{
				value.ToXml(xElement);
			}
			xDocument.Add(xElement);
			using Stream stream = container.OpenFile(filename, FileMode.Create);
			xDocument.Save(stream);
		}
		catch
		{
		}
	}
}
