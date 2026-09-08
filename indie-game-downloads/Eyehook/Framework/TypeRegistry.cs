using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Eyehook.Framework;

public class TypeRegistry<ParentType> where ParentType : class, BinaryRW
{
	private const string NullID = "0";

	private Dictionary<Type, string> idMap = new Dictionary<Type, string>();

	private Dictionary<string, Type> typeMap = new Dictionary<string, Type>();

	private static Type[] LoaderParams = new Type[1] { typeof(BinaryReader) };

	private static object[] LoaderParamValues = new object[1];

	public virtual void Register<T>(string id) where T : ParentType
	{
		Type typeFromHandle = typeof(T);
		Validate(id, typeFromHandle);
		idMap.Add(typeFromHandle, id);
		typeMap.Add(id, typeFromHandle);
	}

	private void Validate(string id, Type type)
	{
		if (typeMap.ContainsKey(id) || id.Equals("0"))
		{
			throw new Exception("TypeDB: ID '" + id + "' is already registered.");
		}
		if (idMap.ContainsKey(type))
		{
			throw new Exception("TypeDB: Type '" + type?.ToString() + "' is already registered.");
		}
		if ((object)type.GetConstructor(LoaderParams) == null)
		{
			throw new Exception("TypeDB: Type '" + type?.ToString() + "' does not have a valid constructor.");
		}
	}

	public void WriteType(BinaryWriter writer, Type type)
	{
		string value = idMap[type];
		writer.Write(value);
	}

	public Type ReadType(BinaryReader reader)
	{
		return typeMap[reader.ReadString()];
	}

	public void Save(BinaryWriter writer, ParentType obj)
	{
		if (obj == null)
		{
			writer.Write("0");
			return;
		}
		string value = idMap[obj.GetType()];
		writer.Write(value);
		obj.Write(writer);
	}

	public ParentType Load(BinaryReader reader)
	{
		string text = reader.ReadString();
		if (text.Equals("0"))
		{
			return null;
		}
		ConstructorInfo constructor = typeMap[text].GetConstructor(LoaderParams);
		LoaderParamValues[0] = reader;
		return (ParentType)constructor.Invoke(LoaderParamValues);
	}
}
