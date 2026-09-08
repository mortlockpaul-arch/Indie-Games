using System.Diagnostics.CodeAnalysis;

namespace System.ComponentModel;

public interface ICustomTypeDescriptor
{
	bool? RequireRegisteredTypes => null;

	AttributeCollection GetAttributes();

	string? GetClassName();

	string? GetComponentName();

	[RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
	TypeConverter? GetConverter();

	[RequiresUnreferencedCode("The built-in EventDescriptor implementation uses Reflection which requires unreferenced code.")]
	EventDescriptor? GetDefaultEvent();

	[RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered.")]
	PropertyDescriptor? GetDefaultProperty();

	[RequiresUnreferencedCode("Design-time attributes are not preserved when trimming. Types referenced by attributes like EditorAttribute and DesignerAttribute may not be available after trimming.")]
	object? GetEditor(Type editorBaseType);

	EventDescriptorCollection GetEvents();

	[RequiresUnreferencedCode("The public parameterless constructor or the 'Default' static field may be trimmed from the Attribute's Type.")]
	EventDescriptorCollection GetEvents(Attribute[]? attributes);

	[RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered.")]
	PropertyDescriptorCollection GetProperties();

	[RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered. The public parameterless constructor or the 'Default' static field may be trimmed from the Attribute's Type.")]
	PropertyDescriptorCollection GetProperties(Attribute[]? attributes);

	object? GetPropertyOwner(PropertyDescriptor? pd);

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "Forwarding from a type provider that supports registered types to one that does not is supported.")]
	TypeConverter? GetConverterFromRegisteredType()
	{
		if (!RequireRegisteredTypes.HasValue)
		{
			if (TypeDescriptor.RequireRegisteredTypes)
			{
				TypeDescriptor.ThrowHelper.ThrowNotImplementedException_CustomTypeProviderMustImplememtMember("GetConverterFromRegisteredType");
			}
		}
		else if (RequireRegisteredTypes == true)
		{
			TypeDescriptor.ThrowHelper.ThrowNotImplementedException_CustomTypeProviderMustImplememtMember("GetConverterFromRegisteredType");
		}
		return GetConverter();
	}

	EventDescriptorCollection GetEventsFromRegisteredType()
	{
		if (!RequireRegisteredTypes.HasValue)
		{
			if (TypeDescriptor.RequireRegisteredTypes)
			{
				TypeDescriptor.ThrowHelper.ThrowNotImplementedException_CustomTypeProviderMustImplememtMember("GetEventsFromRegisteredType");
			}
		}
		else if (RequireRegisteredTypes == true)
		{
			TypeDescriptor.ThrowHelper.ThrowNotImplementedException_CustomTypeProviderMustImplememtMember("GetEventsFromRegisteredType");
		}
		return GetEvents();
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "Forwarding from a type provider that supports registered types to one that does not is supported.")]
	PropertyDescriptorCollection GetPropertiesFromRegisteredType()
	{
		if (!RequireRegisteredTypes.HasValue)
		{
			if (TypeDescriptor.RequireRegisteredTypes)
			{
				TypeDescriptor.ThrowHelper.ThrowNotImplementedException_CustomTypeProviderMustImplememtMember("GetPropertiesFromRegisteredType");
			}
		}
		else if (RequireRegisteredTypes == true)
		{
			TypeDescriptor.ThrowHelper.ThrowNotImplementedException_CustomTypeProviderMustImplememtMember("GetPropertiesFromRegisteredType");
		}
		return GetProperties();
	}
}
