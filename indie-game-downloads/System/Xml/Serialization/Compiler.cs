using System.Buffers.Binary;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace System.Xml.Serialization;

internal sealed class Compiler
{
	private readonly StringWriter _writer = new StringWriter(CultureInfo.InvariantCulture);

	internal TextWriter Source => _writer;

	[RequiresUnreferencedCode("Reflects against input Type DeclaringType")]
	internal static void AddImport(Type type, Hashtable types)
	{
		if (type == null || TypeScope.IsKnownType(type) || types[type] != null)
		{
			return;
		}
		types[type] = type;
		Type baseType = type.BaseType;
		if (baseType != null)
		{
			AddImport(baseType, types);
		}
		Type declaringType = type.DeclaringType;
		if (declaringType != null)
		{
			AddImport(declaringType, types);
		}
		Type[] interfaces = type.GetInterfaces();
		for (int i = 0; i < interfaces.Length; i++)
		{
			AddImport(interfaces[i], types);
		}
		ConstructorInfo[] constructors = type.GetConstructors();
		for (int j = 0; j < constructors.Length; j++)
		{
			ParameterInfo[] parameters = constructors[j].GetParameters();
			for (int k = 0; k < parameters.Length; k++)
			{
				AddImport(parameters[k].ParameterType, types);
			}
		}
		if (type.IsGenericType)
		{
			Type[] genericArguments = type.GetGenericArguments();
			for (int l = 0; l < genericArguments.Length; l++)
			{
				AddImport(genericArguments[l], types);
			}
		}
		Assembly assembly = type.Module.Assembly;
		if (DynamicAssemblies.IsTypeDynamic(type))
		{
			DynamicAssemblies.Add(assembly);
			return;
		}
		object[] customAttributes = type.GetCustomAttributes(typeof(TypeForwardedFromAttribute), inherit: false);
		if (customAttributes.Length != 0)
		{
			Assembly.Load(new AssemblyName((customAttributes[0] as TypeForwardedFromAttribute).AssemblyFullName));
		}
	}

	internal static string GetTempAssemblyName(AssemblyName parent, string ns)
	{
		if (!string.IsNullOrEmpty(ns))
		{
			return $"{parent.Name}.XmlSerializers.{GetPersistentHashCode(ns)}";
		}
		return parent.Name + ".XmlSerializers";
	}

	private static uint GetPersistentHashCode(string value)
	{
		return BinaryPrimitives.ReadUInt32BigEndian(SHA512.HashData(Encoding.UTF8.GetBytes(value)));
	}
}
