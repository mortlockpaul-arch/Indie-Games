using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json.Reflection;
using System.Text.Json.Serialization.Metadata;

namespace System.Text.Json.Serialization.Converters;

[RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
internal sealed class IEnumerableConverterFactory : JsonConverterFactory
{
	private static readonly IDictionaryConverter<IDictionary> s_converterForIDictionary = new IDictionaryConverter<IDictionary>();

	private static readonly IEnumerableConverter<IEnumerable> s_converterForIEnumerable = new IEnumerableConverter<IEnumerable>();

	private static readonly IListConverter<IList> s_converterForIList = new IListConverter<IList>();

	[RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
	public IEnumerableConverterFactory()
	{
	}

	public override bool CanConvert(Type typeToConvert)
	{
		return typeof(IEnumerable).IsAssignableFrom(typeToConvert);
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "The ctor is marked RequiresUnreferencedCode.")]
	public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
	{
		Type type = null;
		Type type2 = null;
		Type typeFromHandle;
		Type compatibleGenericBaseClass;
		if (typeToConvert.IsArray)
		{
			if (typeToConvert.GetArrayRank() > 1)
			{
				return UnsupportedTypeConverterFactory.CreateUnsupportedConverterForType(typeToConvert);
			}
			typeFromHandle = typeof(ArrayConverter<, >);
			type = typeToConvert.GetElementType();
		}
		else if ((compatibleGenericBaseClass = typeToConvert.GetCompatibleGenericBaseClass(typeof(List<>))) != null)
		{
			typeFromHandle = typeof(ListOfTConverter<, >);
			type = compatibleGenericBaseClass.GetGenericArguments()[0];
		}
		else if ((compatibleGenericBaseClass = typeToConvert.GetCompatibleGenericBaseClass(typeof(Dictionary<, >))) != null)
		{
			Type[] genericArguments = compatibleGenericBaseClass.GetGenericArguments();
			typeFromHandle = typeof(DictionaryOfTKeyTValueConverter<, , >);
			type2 = genericArguments[0];
			type = genericArguments[1];
		}
		else if (typeToConvert.IsImmutableDictionaryType())
		{
			Type[] genericArguments2 = typeToConvert.GetGenericArguments();
			typeFromHandle = typeof(ImmutableDictionaryOfTKeyTValueConverterWithReflection<, , >);
			type2 = genericArguments2[0];
			type = genericArguments2[1];
		}
		else if ((compatibleGenericBaseClass = typeToConvert.GetCompatibleGenericInterface(typeof(IDictionary<, >))) != null)
		{
			Type[] genericArguments3 = compatibleGenericBaseClass.GetGenericArguments();
			typeFromHandle = typeof(IDictionaryOfTKeyTValueConverter<, , >);
			type2 = genericArguments3[0];
			type = genericArguments3[1];
		}
		else if ((compatibleGenericBaseClass = typeToConvert.GetCompatibleGenericInterface(typeof(IReadOnlyDictionary<, >))) != null)
		{
			Type[] genericArguments4 = compatibleGenericBaseClass.GetGenericArguments();
			typeFromHandle = typeof(IReadOnlyDictionaryOfTKeyTValueConverter<, , >);
			type2 = genericArguments4[0];
			type = genericArguments4[1];
		}
		else if (typeToConvert.IsImmutableEnumerableType())
		{
			typeFromHandle = typeof(ImmutableEnumerableOfTConverterWithReflection<, >);
			type = typeToConvert.GetGenericArguments()[0];
		}
		else if ((compatibleGenericBaseClass = typeToConvert.GetCompatibleGenericInterface(typeof(IList<>))) != null)
		{
			typeFromHandle = typeof(IListOfTConverter<, >);
			type = compatibleGenericBaseClass.GetGenericArguments()[0];
		}
		else if ((compatibleGenericBaseClass = typeToConvert.GetCompatibleGenericInterface(typeof(ISet<>))) != null)
		{
			typeFromHandle = typeof(ISetOfTConverter<, >);
			type = compatibleGenericBaseClass.GetGenericArguments()[0];
		}
		else if ((compatibleGenericBaseClass = typeToConvert.GetCompatibleGenericInterface(typeof(ICollection<>))) != null)
		{
			typeFromHandle = typeof(ICollectionOfTConverter<, >);
			type = compatibleGenericBaseClass.GetGenericArguments()[0];
		}
		else if ((compatibleGenericBaseClass = typeToConvert.GetCompatibleGenericBaseClass(typeof(Stack<>))) != null)
		{
			typeFromHandle = typeof(StackOfTConverter<, >);
			type = compatibleGenericBaseClass.GetGenericArguments()[0];
		}
		else if ((compatibleGenericBaseClass = typeToConvert.GetCompatibleGenericBaseClass(typeof(Queue<>))) != null)
		{
			typeFromHandle = typeof(QueueOfTConverter<, >);
			type = compatibleGenericBaseClass.GetGenericArguments()[0];
		}
		else if ((compatibleGenericBaseClass = typeToConvert.GetCompatibleGenericBaseClass(typeof(ConcurrentStack<>))) != null)
		{
			typeFromHandle = typeof(ConcurrentStackOfTConverter<, >);
			type = compatibleGenericBaseClass.GetGenericArguments()[0];
		}
		else if ((compatibleGenericBaseClass = typeToConvert.GetCompatibleGenericBaseClass(typeof(ConcurrentQueue<>))) != null)
		{
			typeFromHandle = typeof(ConcurrentQueueOfTConverter<, >);
			type = compatibleGenericBaseClass.GetGenericArguments()[0];
		}
		else if ((compatibleGenericBaseClass = typeToConvert.GetCompatibleGenericInterface(typeof(IEnumerable<>))) != null)
		{
			typeFromHandle = typeof(IEnumerableOfTConverter<, >);
			type = compatibleGenericBaseClass.GetGenericArguments()[0];
		}
		else if (typeof(IDictionary).IsAssignableFrom(typeToConvert))
		{
			if (typeToConvert == typeof(IDictionary))
			{
				return s_converterForIDictionary;
			}
			typeFromHandle = typeof(IDictionaryConverter<>);
		}
		else if (typeof(IList).IsAssignableFrom(typeToConvert))
		{
			if (typeToConvert == typeof(IList))
			{
				return s_converterForIList;
			}
			typeFromHandle = typeof(IListConverter<>);
		}
		else if (typeToConvert.IsNonGenericStackOrQueue())
		{
			typeFromHandle = typeof(StackOrQueueConverterWithReflection<>);
		}
		else
		{
			if (typeToConvert == typeof(IEnumerable))
			{
				return s_converterForIEnumerable;
			}
			typeFromHandle = typeof(IEnumerableConverter<>);
		}
		Type type3;
		switch (typeFromHandle.GetGenericArguments().Length)
		{
		case 1:
			type3 = typeFromHandle.MakeGenericType(typeToConvert);
			break;
		case 2:
			JsonTypeInfo.ValidateType(type);
			type3 = typeFromHandle.MakeGenericType(typeToConvert, type);
			break;
		default:
			JsonTypeInfo.ValidateType(type);
			JsonTypeInfo.ValidateType(type2);
			type3 = typeFromHandle.MakeGenericType(typeToConvert, type2, type);
			break;
		}
		return (JsonConverter)Activator.CreateInstance(type3, BindingFlags.Instance | BindingFlags.Public, null, null, null);
	}
}
