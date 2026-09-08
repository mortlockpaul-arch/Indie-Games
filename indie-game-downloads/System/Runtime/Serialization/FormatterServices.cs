using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters;

namespace System.Runtime.Serialization;

[Obsolete("Formatter-based serialization is obsolete and should not be used.", DiagnosticId = "SYSLIB0050", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
public static class FormatterServices
{
	private static readonly ConcurrentDictionary<MemberHolder, MemberInfo[]> s_memberInfoTable = new ConcurrentDictionary<MemberHolder, MemberInfo[]>();

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2065:UnrecognizedReflectionPattern", Justification = "The parentType is read from an array which currently can't be annotated,but the input type is annotated with All, so all of its base types are also All.")]
	private static FieldInfo[] InternalGetSerializableMembers([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type)
	{
		if (type.IsInterface)
		{
			return Array.Empty<FieldInfo>();
		}
		if (!type.IsSerializable)
		{
			throw new SerializationException(System.SR.Format(System.SR.Serialization_NonSerType, type.FullName, type.Assembly.FullName));
		}
		FieldInfo[] array = GetSerializableFields(type);
		Type baseType = type.BaseType;
		if (baseType != null && baseType != typeof(object))
		{
			bool parentTypes = GetParentTypes(baseType, out var parentTypes2, out var parentTypeCount);
			if (parentTypeCount > 0)
			{
				List<FieldInfo> list = new List<FieldInfo>();
				for (int i = 0; i < parentTypeCount; i++)
				{
					baseType = parentTypes2[i];
					if (!baseType.IsSerializable)
					{
						throw new SerializationException(System.SR.Format(System.SR.Serialization_NonSerType, baseType.FullName, baseType.Module.Assembly.FullName));
					}
					FieldInfo[] fields = baseType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
					string namePrefix = (parentTypes ? baseType.Name : baseType.FullName);
					FieldInfo[] array2 = fields;
					foreach (FieldInfo fieldInfo in array2)
					{
						if (!fieldInfo.IsNotSerialized)
						{
							list.Add(new SerializationFieldInfo(fieldInfo, namePrefix));
						}
					}
				}
				if (list != null && list.Count > 0)
				{
					FieldInfo[] array3 = new FieldInfo[list.Count + array.Length];
					Array.Copy(array, array3, array.Length);
					list.CopyTo(array3, array.Length);
					array = array3;
				}
			}
		}
		return array;
	}

	private static FieldInfo[] GetSerializableFields([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields)] Type type)
	{
		FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		int num = 0;
		for (int i = 0; i < fields.Length; i++)
		{
			if ((fields[i].Attributes & FieldAttributes.NotSerialized) != FieldAttributes.NotSerialized)
			{
				num++;
			}
		}
		if (num != fields.Length)
		{
			FieldInfo[] array = new FieldInfo[num];
			num = 0;
			for (int j = 0; j < fields.Length; j++)
			{
				if ((fields[j].Attributes & FieldAttributes.NotSerialized) != FieldAttributes.NotSerialized)
				{
					array[num] = fields[j];
					num++;
				}
			}
			return array;
		}
		return fields;
	}

	private static bool GetParentTypes(Type parentType, out Type[] parentTypes, out int parentTypeCount)
	{
		parentTypes = null;
		parentTypeCount = 0;
		bool flag = true;
		Type typeFromHandle = typeof(object);
		Type type = parentType;
		while (type != typeFromHandle)
		{
			if (!type.IsInterface)
			{
				string name = type.Name;
				int num = 0;
				while (flag && num < parentTypeCount)
				{
					string name2 = parentTypes[num].Name;
					if (name2.Length == name.Length && name2[0] == name[0] && name == name2)
					{
						flag = false;
						break;
					}
					num++;
				}
				if (parentTypes == null || parentTypeCount == parentTypes.Length)
				{
					Array.Resize(ref parentTypes, Math.Max(parentTypeCount * 2, 12));
				}
				parentTypes[parentTypeCount++] = type;
			}
			type = type.BaseType;
		}
		return flag;
	}

	public static MemberInfo[] GetSerializableMembers([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type)
	{
		return GetSerializableMembers(type, new StreamingContext(StreamingContextStates.All));
	}

	public static MemberInfo[] GetSerializableMembers([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type, StreamingContext context)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return s_memberInfoTable.GetOrAdd(new MemberHolder(type, context), (MemberHolder mh) => InternalGetSerializableMembers(mh._memberType));
	}

	public static void CheckTypeSecurity(Type t, TypeFilterLevel securityLevel)
	{
	}

	public static object GetUninitializedObject([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type type)
	{
		return RuntimeHelpers.GetUninitializedObject(type);
	}

	public static object GetSafeUninitializedObject([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)] Type type)
	{
		return RuntimeHelpers.GetUninitializedObject(type);
	}

	internal static void SerializationSetValue(MemberInfo fi, object target, object value)
	{
		if (fi is FieldInfo fieldInfo)
		{
			fieldInfo.SetValue(target, value);
			return;
		}
		throw new ArgumentException(System.SR.Argument_InvalidFieldInfo);
	}

	public static object PopulateObjectMembers(object obj, MemberInfo[] members, object?[] data)
	{
		ArgumentNullException.ThrowIfNull(obj, "obj");
		ArgumentNullException.ThrowIfNull(members, "members");
		ArgumentNullException.ThrowIfNull(data, "data");
		if (members.Length != data.Length)
		{
			throw new ArgumentException(System.SR.Argument_DataLengthDifferent);
		}
		for (int i = 0; i < members.Length; i++)
		{
			MemberInfo memberInfo = members[i];
			if (memberInfo == null)
			{
				throw new ArgumentNullException("members", System.SR.Format(System.SR.ArgumentNull_NullMember, i));
			}
			if (data[i] != null)
			{
				if (!(memberInfo is FieldInfo fieldInfo))
				{
					throw new SerializationException(System.SR.Serialization_UnknownMemberInfo);
				}
				fieldInfo.SetValue(obj, data[i]);
			}
		}
		return obj;
	}

	public static object?[] GetObjectData(object obj, MemberInfo[] members)
	{
		ArgumentNullException.ThrowIfNull(obj, "obj");
		ArgumentNullException.ThrowIfNull(members, "members");
		object[] array = new object[members.Length];
		for (int i = 0; i < members.Length; i++)
		{
			MemberInfo obj2 = members[i];
			if (obj2 == null)
			{
				throw new ArgumentNullException("members", System.SR.Format(System.SR.ArgumentNull_NullMember, i));
			}
			FieldInfo fieldInfo = obj2 as FieldInfo;
			if (fieldInfo == null)
			{
				throw new SerializationException(System.SR.Serialization_UnknownMemberInfo);
			}
			array[i] = fieldInfo.GetValue(obj);
		}
		return array;
	}

	public static ISerializationSurrogate GetSurrogateForCyclicalReference(ISerializationSurrogate innerSurrogate)
	{
		ArgumentNullException.ThrowIfNull(innerSurrogate, "innerSurrogate");
		return new SurrogateForCyclicalReference(innerSurrogate);
	}

	[RequiresUnreferencedCode("Types might be removed")]
	public static Type? GetTypeFromAssembly(Assembly assem, string name)
	{
		ArgumentNullException.ThrowIfNull(assem, "assem");
		return assem.GetType(name, throwOnError: false, ignoreCase: false);
	}
}
