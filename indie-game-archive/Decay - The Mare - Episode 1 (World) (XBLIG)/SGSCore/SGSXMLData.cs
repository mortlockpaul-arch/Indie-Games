using System.Collections.Generic;

namespace SGSCore;

public class SGSXMLData
{
	public string ID = "";

	public List<object> FIELDS = new List<object>();

	public void Clear()
	{
		FIELDS.Clear();
		FIELDS = null;
	}

	public void Add(object o)
	{
		FIELDS.Add(o);
	}

	public object GetField(int index)
	{
		if (index < 0 || index >= FIELDS.Count)
		{
			return null;
		}
		return FIELDS[index];
	}
}
