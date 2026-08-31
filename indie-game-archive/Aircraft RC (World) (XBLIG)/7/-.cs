using System;
using System.Collections.Generic;
using System.Reflection;

namespace _7;

internal class _0002
{
	public static MemberInfo[] GetSerializableMembers(Type type)
	{
		List<MemberInfo> list = new List<MemberInfo>(32);
		list.AddRange(type.GetProperties(BindingFlags.Instance | BindingFlags.Public));
		list.AddRange(type.GetFields(BindingFlags.Instance | BindingFlags.Public));
		return list.ToArray();
	}

	public static void PopulateObjectMembers(object obj, MemberInfo[] members, object[] data)
	{
		int num = 0;
		foreach (MemberInfo memberInfo in members)
		{
			try
			{
				if (memberInfo is PropertyInfo)
				{
					(memberInfo as PropertyInfo).SetValue(obj, data[num], null);
				}
				if (memberInfo is FieldInfo)
				{
					(memberInfo as FieldInfo).SetValue(obj, data[num]);
				}
			}
			catch
			{
			}
			num++;
		}
	}
}
