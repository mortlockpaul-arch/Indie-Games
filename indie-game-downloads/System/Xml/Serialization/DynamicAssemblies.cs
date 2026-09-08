using System.Collections;
using System.Reflection;

namespace System.Xml.Serialization;

internal static class DynamicAssemblies
{
	private static readonly Hashtable s_nameToAssemblyMap = new Hashtable();

	private static readonly Hashtable s_assemblyToNameMap = new Hashtable();

	private static readonly ContextAwareTables<object> s_tableIsTypeDynamic = new ContextAwareTables<object>();

	internal static bool IsTypeDynamic(Type type)
	{
		return (bool)s_tableIsTypeDynamic.GetOrCreateValue(type, delegate(Type type2)
		{
			bool flag = type2.Assembly.IsDynamic;
			if (!flag)
			{
				if (type2.IsArray)
				{
					flag = IsTypeDynamic(type2.GetElementType());
				}
				else if (type2.IsGenericType)
				{
					Type[] genericArguments = type2.GetGenericArguments();
					if (genericArguments != null)
					{
						foreach (Type type3 in genericArguments)
						{
							if (!(type3 == null) && !type3.IsGenericParameter)
							{
								flag = IsTypeDynamic(type3);
								if (flag)
								{
									break;
								}
							}
						}
					}
				}
			}
			return flag;
		});
	}

	internal static bool IsTypeDynamic(Type[] arguments)
	{
		for (int i = 0; i < arguments.Length; i++)
		{
			if (IsTypeDynamic(arguments[i]))
			{
				return true;
			}
		}
		return false;
	}

	internal static void Add(Assembly a)
	{
		lock (s_nameToAssemblyMap)
		{
			if (s_assemblyToNameMap[a] == null)
			{
				Assembly assembly = s_nameToAssemblyMap[a.FullName] as Assembly;
				string text = null;
				if (assembly == null)
				{
					text = a.FullName;
				}
				else if (assembly != a)
				{
					text = $"{a.FullName}, {s_nameToAssemblyMap.Count}";
				}
				if (text != null)
				{
					s_nameToAssemblyMap.Add(text, a);
					s_assemblyToNameMap.Add(a, text);
				}
			}
		}
	}

	internal static Assembly Get(string fullName)
	{
		if (s_nameToAssemblyMap == null)
		{
			return null;
		}
		return (Assembly)s_nameToAssemblyMap[fullName];
	}

	internal static string GetName(Assembly a)
	{
		if (s_assemblyToNameMap == null)
		{
			return null;
		}
		return (string)s_assemblyToNameMap[a];
	}
}
