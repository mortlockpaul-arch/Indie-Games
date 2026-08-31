using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using _0001;
using SynapseGaming.LightingSystem.Serialization;

namespace R;

internal class B
{
	private StreamingContext HCB = default(StreamingContext);

	private FormatterConverter HC_0002 = new FormatterConverter();

	private Dictionary<string, int> HC_0012 = new Dictionary<string, int>(16);

	private H HCH = new H();

	internal object _7F(global::_0001._0001 P_0)
	{
		HCH.G();
		_0001._7 documentElement = P_0.DocumentElement;
		if (documentElement.Name == "root")
		{
			_0001._0002 childNodes = documentElement.ChildNodes;
			int count = childNodes.Count;
			for (int i = 0; i < count; i++)
			{
				_0001.B b = childNodes[i];
				if (b.Name != "classes")
				{
					continue;
				}
				_0001._0002 childNodes2 = b.ChildNodes;
				int count2 = childNodes2.Count;
				for (int j = 0; j < count2; j++)
				{
					if (childNodes2[j] is _0001._7 obj)
					{
						Type type = _7f(obj);
						HCH._5(obj.Name, type);
					}
				}
			}
			HCH._7h();
			for (int k = 0; k < count; k++)
			{
				_0001.B b2 = childNodes[k];
				if (!(b2.Name == "classes") && b2 is _0001._7)
				{
					return _7F(b2 as _0001._7);
				}
			}
		}
		return null;
	}

	private Type _7f(_0001._7 P_0)
	{
		string name = P_0.Name;
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
		_0001._0012 obj = (_0001._0012)P_0.Attributes.GetNamedItem("fullname");
		_0001._0012 obj2 = (_0001._0012)P_0.Attributes.GetNamedItem("assembly");
		_0001._0012 obj3 = (_0001._0012)P_0.Attributes.GetNamedItem("fullyqualifiedname");
		if (obj == null || obj2 == null)
		{
			return null;
		}
		string innerText = obj.InnerText;
		string innerText2 = obj2.InnerText;
		string text = "";
		if (obj3 != null)
		{
			text = obj3.InnerText;
		}
		Type type = HCH._7T(innerText, innerText2, text);
		_0001._0002 childNodes = P_0.ChildNodes;
		int count = childNodes.Count;
		if ((object)type == null || count < 1)
		{
			return type;
		}
		Type[] array = new Type[count];
		for (int i = 0; i < array.Length; i++)
		{
			if (childNodes[i] is _0001._7 obj4)
			{
				array[i] = _7f(obj4);
			}
		}
		return type.MakeGenericType(array);
	}

	private object _7F(_0001._7 P_0)
	{
		if (P_0 == null || string.IsNullOrEmpty(P_0.Name))
		{
			return null;
		}
		Type type = null;
		HCH._7a(P_0.Name, out var type2, out var array);
		if (type2.IsGenericType)
		{
			type = type2.GetGenericTypeDefinition();
		}
		object obj = null;
		ConstructorInfo[] constructors = type2.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		ConstructorInfo[] array2 = constructors;
		foreach (ConstructorInfo constructorInfo in array2)
		{
			ParameterInfo[] parameters = constructorInfo.GetParameters();
			if (parameters == null || parameters.Length <= 0)
			{
				obj = constructorInfo.Invoke(null);
				break;
			}
		}
		if (obj == null)
		{
			obj = Activator.CreateInstance(type2);
		}
		if (obj == null)
		{
			throw new Exception($"Cannot create an instance of type '{type2.FullName}'.");
		}
		SerializationInfo serializationInfo = new SerializationInfo(type2, HC_0002);
		foreach (_0001.B childNode in P_0.ChildNodes)
		{
			if (!(childNode is _0001._7) || string.IsNullOrEmpty(childNode.Name))
			{
				continue;
			}
			bool flag = false;
			foreach (_0001.B childNode2 in childNode.ChildNodes)
			{
				if (childNode2 is _0001._7)
				{
					serializationInfo.AddValue(childNode.Name, _7F(childNode2 as _0001._7));
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				string name = XmlConvert.DecodeName(childNode.Name);
				serializationInfo.AddValue(name, childNode.InnerText);
			}
		}
		if ((object)type == typeof(KeyValuePair<, >))
		{
			Type[] genericArguments = type2.GetGenericArguments();
			ConstructorInfo constructor = type2.GetConstructor(genericArguments);
			if ((object)constructor != null)
			{
				object value = serializationInfo.GetValue("key", genericArguments[0]);
				object value2 = serializationInfo.GetValue("value", genericArguments[1]);
				obj = constructor.Invoke(new object[2] { value, value2 });
			}
		}
		else if (obj is ICollection)
		{
			IList list = obj as IList;
			IDictionary dictionary = obj as IDictionary;
			if (list != null)
			{
				Type type3 = list.GetType();
				Type datatype = typeof(object);
				if (type3.IsGenericType)
				{
					Type[] genericArguments2 = type3.GetGenericArguments();
					if (genericArguments2.Length > 0)
					{
						datatype = genericArguments2[0];
					}
				}
				foreach (SerializationEntry item in serializationInfo)
				{
					object value3 = serializationInfo.GetValue(item.Name, datatype);
					list.Add(value3);
				}
			}
			else if (dictionary != null)
			{
				foreach (SerializationEntry item2 in serializationInfo)
				{
					object value4 = item2.Value;
					Type type4 = value4.GetType();
					PropertyInfo property = type4.GetProperty("Key");
					PropertyInfo property2 = type4.GetProperty("Value");
					if ((object)property == null || (object)property2 == null)
					{
						throw new Exception("Object inserted into Dictionary member is not a KeyValuePair type.");
					}
					object value5 = property.GetValue(value4, null);
					object value6 = property2.GetValue(value4, null);
					dictionary.Add(value5, value6);
				}
			}
		}
		else if (obj is IFullSerializable)
		{
			(obj as IFullSerializable).SetObjectData(serializationInfo, HCB);
		}
		else if (array.Length > 0)
		{
			HC_0012.Clear();
			for (int j = 0; j < array.Length; j++)
			{
				if (!HC_0012.ContainsKey(array[j].Name))
				{
					HC_0012.Add(array[j].Name, j);
				}
			}
			foreach (SerializationEntry item3 in serializationInfo)
			{
				if (!HC_0012.ContainsKey(item3.Name))
				{
					continue;
				}
				int num = HC_0012[item3.Name];
				MemberInfo memberInfo = array[num];
				if (memberInfo is PropertyInfo)
				{
					PropertyInfo propertyInfo = memberInfo as PropertyInfo;
					MethodInfo setMethod = propertyInfo.GetSetMethod(nonPublic: true);
					if (propertyInfo.CanWrite && !setMethod.IsPrivate && !setMethod.IsFamily)
					{
						object value7 = ((!propertyInfo.PropertyType.IsEnum) ? serializationInfo.GetValue(item3.Name, propertyInfo.PropertyType) : Enum.Parse(propertyInfo.PropertyType, (string)serializationInfo.GetValue(item3.Name, typeof(string)), ignoreCase: false));
						propertyInfo.SetValue(obj, value7, null);
					}
				}
				else if (memberInfo is FieldInfo)
				{
					FieldInfo fieldInfo = memberInfo as FieldInfo;
					if (!fieldInfo.IsPrivate && !fieldInfo.IsFamily)
					{
						object value8 = ((!fieldInfo.FieldType.IsEnum) ? serializationInfo.GetValue(item3.Name, fieldInfo.FieldType) : Enum.Parse(fieldInfo.FieldType, (string)serializationInfo.GetValue(item3.Name, typeof(string)), ignoreCase: false));
						fieldInfo.SetValue(obj, value8);
					}
				}
			}
		}
		return obj;
	}
}
