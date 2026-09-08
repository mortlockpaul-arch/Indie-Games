using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace System.Text.Json;

[StructLayout(LayoutKind.Auto)]
[DebuggerDisplay("{DebuggerDisplay,nq}")]
internal struct ReadStackFrame
{
	public JsonPropertyInfo JsonPropertyInfo;

	public StackFramePropertyState PropertyState;

	public bool UseExtensionProperty;

	public byte[] JsonPropertyName;

	public string JsonPropertyNameAsString;

	public object DictionaryKey;

	public int OriginalDepth;

	public object ReturnValue;

	public JsonTypeInfo JsonTypeInfo;

	public StackFrameObjectState ObjectState;

	public bool CanContainMetadata;

	public MetadataPropertyName LatestMetadataPropertyName;

	public MetadataPropertyName MetadataPropertyNames;

	public PolymorphicSerializationState PolymorphicSerializationState;

	public JsonTypeInfo PolymorphicJsonTypeInfo;

	public int PropertyIndex;

	public PropertyRefCacheBuilder PropertyRefCacheBuilder;

	public ArgumentState CtorArgumentState;

	public JsonNumberHandling? NumberHandling;

	public BitArray AssignedProperties;

	public bool HasParentObject;

	public bool IsPopulating;

	public JsonTypeInfo BaseJsonTypeInfo
	{
		get
		{
			if (PolymorphicSerializationState != PolymorphicSerializationState.PolymorphicReEntryStarted)
			{
				return JsonTypeInfo;
			}
			return PolymorphicJsonTypeInfo;
		}
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string DebuggerDisplay => $"ConverterStrategy.{JsonTypeInfo?.Converter.ConverterStrategy}, {JsonTypeInfo?.Type.Name}";

	public void EndConstructorParameter()
	{
		CtorArgumentState.JsonParameterInfo = null;
		JsonPropertyName = null;
		PropertyState = StackFramePropertyState.None;
	}

	public void EndProperty()
	{
		JsonPropertyInfo = null;
		JsonPropertyName = null;
		JsonPropertyNameAsString = null;
		PropertyState = StackFramePropertyState.None;
	}

	public void EndElement()
	{
		JsonPropertyNameAsString = null;
		PropertyState = StackFramePropertyState.None;
	}

	public bool IsProcessingDictionary()
	{
		return JsonTypeInfo.Kind == JsonTypeInfoKind.Dictionary;
	}

	public bool IsProcessingEnumerable()
	{
		return JsonTypeInfo.Kind == JsonTypeInfoKind.Enumerable;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void MarkPropertyAsRead(JsonPropertyInfo propertyInfo)
	{
		if (AssignedProperties != null)
		{
			if (!propertyInfo.Options.AllowDuplicateProperties && AssignedProperties[propertyInfo.PropertyIndex])
			{
				ThrowHelper.ThrowJsonException_DuplicatePropertyNotAllowed(propertyInfo);
			}
			AssignedProperties[propertyInfo.PropertyIndex] = true;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void InitializePropertiesValidationState(JsonTypeInfo typeInfo)
	{
		if (typeInfo.ShouldTrackRequiredProperties || !typeInfo.Options.AllowDuplicateProperties)
		{
			AssignedProperties = new BitArray(typeInfo.Properties.Count);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void ValidateAllRequiredPropertiesAreRead(JsonTypeInfo typeInfo)
	{
		if (typeInfo.ShouldTrackRequiredProperties)
		{
			BitArray bitArray = AssignedProperties.Or(typeInfo.OptionalPropertiesMask);
			if (!bitArray.HasAllSet())
			{
				ThrowHelper.ThrowJsonException_JsonRequiredPropertyMissing(typeInfo, bitArray);
			}
		}
		AssignedProperties = null;
	}
}
