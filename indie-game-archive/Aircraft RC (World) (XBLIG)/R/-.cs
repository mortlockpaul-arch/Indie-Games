using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using _7;
using SynapseGaming.LightingSystem.Serialization;

namespace R;

internal class _0002
{
	internal static MemberInfo[] _7G(Type P_0)
	{
		object[] customAttributes = P_0.GetCustomAttributes(typeof(SerializationInclusionModelAttribute), inherit: true);
		if (customAttributes == null || customAttributes.Length < 1)
		{
			return global::_7._0002.GetSerializableMembers(P_0);
		}
		return _7_0010(P_0);
	}

	private static MemberInfo[] _7_0010(Type P_0)
	{
		MemberInfo[] members = P_0.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		List<MemberInfo> list = new List<MemberInfo>(members.Length);
		MemberInfo[] array = members;
		foreach (MemberInfo memberInfo in array)
		{
			PropertyInfo propertyInfo = memberInfo as PropertyInfo;
			FieldInfo fieldInfo = memberInfo as FieldInfo;
			if ((object)propertyInfo != null && propertyInfo.CanRead && propertyInfo.CanWrite)
			{
				list.Add(memberInfo);
			}
			else if ((object)fieldInfo != null)
			{
				list.Add(memberInfo);
			}
		}
		return list.ToArray();
	}
}
internal class _0012
{
	private Dictionary<Type, string> HCB = new Dictionary<Type, string>(16);

	private Dictionary<Type, MemberInfo[]> HC_0002 = new Dictionary<Type, MemberInfo[]>(16);

	private List<MemberInfo> HC_0012 = new List<MemberInfo>(16);

	internal Dictionary<Type, string> TypeNames => HCB;

	internal void G()
	{
		HC_0002.Clear();
		HCB.Clear();
	}

	internal void _7L(Type P_0, out string P_1, out MemberInfo[] P_2)
	{
		if (HCB.ContainsKey(P_0))
		{
			P_1 = HCB[P_0];
			P_2 = HC_0002[P_0];
			return;
		}
		int num = 0;
		string text = (P_1 = XmlConvert.EncodeLocalName(P_0.Name));
		while (HCB.ContainsValue(P_1))
		{
			P_1 = text + "_" + num++;
		}
		P_2 = _0002._7G(P_0);
		object[] customAttributes = P_0.GetCustomAttributes(typeof(SerializationInclusionModelAttribute), inherit: true);
		if (customAttributes != null && customAttributes.Length > 0)
		{
			HC_0012.Clear();
			MemberInfo[] array = P_2;
			foreach (MemberInfo memberInfo in array)
			{
				customAttributes = memberInfo.GetCustomAttributes(typeof(SerializeMemberAttribute), inherit: true);
				if (customAttributes != null && customAttributes.Length > 0)
				{
					HC_0012.Add(memberInfo);
				}
			}
			P_2 = HC_0012.ToArray();
		}
		HCB.Add(P_0, P_1);
		HC_0002.Add(P_0, P_2);
	}
}
