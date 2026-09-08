using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Xml.Schema;

namespace System.Xml.Serialization;

internal sealed class SchemaObjectCache
{
	internal Hashtable looks = new Hashtable();

	[CompilerGenerated]
	private Hashtable _003CGraph_003Ek__BackingField;

	[CompilerGenerated]
	private Hashtable _003CHash_003Ek__BackingField;

	[CompilerGenerated]
	private Hashtable _003CObjectCache_003Ek__BackingField;

	[CompilerGenerated]
	private StringCollection _003CWarnings_003Ek__BackingField;

	private Hashtable Graph => _003CGraph_003Ek__BackingField ?? (_003CGraph_003Ek__BackingField = new Hashtable());

	private Hashtable Hash => _003CHash_003Ek__BackingField ?? (_003CHash_003Ek__BackingField = new Hashtable());

	private Hashtable ObjectCache => _003CObjectCache_003Ek__BackingField ?? (_003CObjectCache_003Ek__BackingField = new Hashtable());

	internal StringCollection Warnings => _003CWarnings_003Ek__BackingField ?? (_003CWarnings_003Ek__BackingField = new StringCollection());

	internal XmlSchemaObject AddItem(XmlSchemaObject item, XmlQualifiedName qname)
	{
		if (item == null)
		{
			return null;
		}
		if (qname == null || qname.IsEmpty)
		{
			return null;
		}
		string key = $"{item.GetType().Name}:{qname}";
		ArrayList arrayList = (ArrayList)ObjectCache[key];
		if (arrayList == null)
		{
			arrayList = new ArrayList();
			ObjectCache[key] = arrayList;
		}
		for (int i = 0; i < arrayList.Count; i++)
		{
			XmlSchemaObject xmlSchemaObject = (XmlSchemaObject)arrayList[i];
			if (xmlSchemaObject == item)
			{
				return xmlSchemaObject;
			}
			if (Match(xmlSchemaObject, item, shareTypes: true))
			{
				return xmlSchemaObject;
			}
			Warnings.Add(System.SR.Format(System.SR.XmlMismatchSchemaObjects, item.GetType().Name, qname.Name, qname.Namespace));
			Warnings.Add("DEBUG:Cached item key:\r\n" + (string)looks[xmlSchemaObject] + "\r\nnew item key:\r\n" + (string)looks[item]);
		}
		arrayList.Add(item);
		return item;
	}

	internal bool Match(XmlSchemaObject o1, XmlSchemaObject o2, bool shareTypes)
	{
		if (o1 == o2)
		{
			return true;
		}
		if (o1.GetType() != o2.GetType())
		{
			return false;
		}
		Hashtable hash = Hash;
		if (hash[o1] == null)
		{
			object obj = (hash[o1] = GetHash(o1));
		}
		int num = (int)Hash[o1];
		int hash2 = GetHash(o2);
		if (num != hash2)
		{
			return false;
		}
		if (shareTypes)
		{
			return CompositeHash(o1) == CompositeHash(o2);
		}
		return true;
	}

	private ArrayList GetDependencies(XmlSchemaObject o, ArrayList deps, Hashtable refs)
	{
		if (refs[o] == null)
		{
			refs[o] = o;
			deps.Add(o);
			if (Graph[o] is ArrayList arrayList)
			{
				for (int i = 0; i < arrayList.Count; i++)
				{
					GetDependencies((XmlSchemaObject)arrayList[i], deps, refs);
				}
			}
		}
		return deps;
	}

	private int CompositeHash(XmlSchemaObject o)
	{
		ArrayList dependencies = GetDependencies(o, new ArrayList(), new Hashtable());
		double num = 0.0;
		for (int i = 0; i < dependencies.Count; i++)
		{
			object obj = Hash[dependencies[i]];
			if (obj is int)
			{
				num += (double)((int)obj / dependencies.Count);
			}
		}
		return (int)num;
	}

	[RequiresUnreferencedCode("creates SchemaGraph")]
	[RequiresDynamicCode("XML serializer relies on dynamic code generation which is not available with Ahead of Time compilation")]
	internal void GenerateSchemaGraph(XmlSchemas schemas)
	{
		ArrayList items = new SchemaGraph(Graph, schemas).GetItems();
		for (int i = 0; i < items.Count; i++)
		{
			GetHash((XmlSchemaObject)items[i]);
		}
	}

	private int GetHash(XmlSchemaObject o)
	{
		object obj = Hash[o];
		if (obj != null && !(obj is XmlSchemaObject))
		{
			return (int)obj;
		}
		string text = ToString(o, new SchemaObjectWriter());
		looks[o] = text;
		int hashCode = text.GetHashCode();
		Hash[o] = hashCode;
		return hashCode;
	}

	private static string ToString(XmlSchemaObject o, SchemaObjectWriter writer)
	{
		return writer.WriteXmlSchemaObject(o);
	}
}
