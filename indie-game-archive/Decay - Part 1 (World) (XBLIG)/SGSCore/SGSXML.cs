using System;
using System.Collections.Generic;

namespace SGSCore;

[Serializable]
public class SGSXML
{
	public string ID = "";

	public List<object> OBJECTS = new List<object>();

	public List<SGSXMLData> DATA = new List<SGSXMLData>();

	public void Clear()
	{
		for (int i = 0; i < OBJECTS.Count; i++)
		{
			OBJECTS[i] = null;
		}
		OBJECTS.Clear();
		OBJECTS = null;
		for (int j = 0; j < DATA.Count; j++)
		{
			if (DATA[j] != null)
			{
				DATA[j].Clear();
				DATA[j] = null;
			}
		}
		DATA.Clear();
		DATA = null;
	}

	public void AddObject(object o)
	{
		if (o != null)
		{
			OBJECTS.Add(o);
		}
	}

	public void AddData(SGSXMLData data)
	{
		if (data != null)
		{
			DATA.Add(data);
		}
	}

	public object GetObject(int index)
	{
		if (index < 0 || index >= OBJECTS.Count)
		{
			return null;
		}
		return OBJECTS[index];
	}

	public SGSXMLData GetData(string ID)
	{
		for (int i = 0; i < DATA.Count; i++)
		{
			if (DATA[i].ID == ID)
			{
				return DATA[i];
			}
		}
		return null;
	}
}
