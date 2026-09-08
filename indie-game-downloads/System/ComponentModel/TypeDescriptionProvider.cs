using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace System.ComponentModel;

public abstract class TypeDescriptionProvider
{
	private sealed class EmptyCustomTypeDescriptor : CustomTypeDescriptor
	{
	}

	private readonly TypeDescriptionProvider _parent;

	private EmptyCustomTypeDescriptor _emptyDescriptor;

	public virtual bool? RequireRegisteredTypes
	{
		get
		{
			if (_parent != null)
			{
				return _parent.RequireRegisteredTypes;
			}
			return null;
		}
	}

	protected TypeDescriptionProvider()
	{
	}

	protected TypeDescriptionProvider(TypeDescriptionProvider parent)
	{
		_parent = parent;
	}

	public virtual void RegisterType<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.Interfaces)] T>()
	{
	}

	public virtual object? CreateInstance(IServiceProvider? provider, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, Type[]? argTypes, object?[]? args)
	{
		if (_parent != null)
		{
			return _parent.CreateInstance(provider, objectType, argTypes, args);
		}
		ArgumentNullException.ThrowIfNull(objectType, "objectType");
		return Activator.CreateInstance(objectType, args);
	}

	public virtual IDictionary? GetCache(object instance)
	{
		return _parent?.GetCache(instance);
	}

	[RequiresUnreferencedCode("The Type of instance cannot be statically discovered.")]
	public virtual ICustomTypeDescriptor GetExtendedTypeDescriptor(object instance)
	{
		if (_parent != null)
		{
			return _parent.GetExtendedTypeDescriptor(instance);
		}
		return _emptyDescriptor ?? (_emptyDescriptor = new EmptyCustomTypeDescriptor());
	}

	public virtual ICustomTypeDescriptor GetExtendedTypeDescriptorFromRegisteredType(object instance)
	{
		if (_parent != null)
		{
			return _parent.GetExtendedTypeDescriptorFromRegisteredType(instance);
		}
		return _emptyDescriptor ?? (_emptyDescriptor = new EmptyCustomTypeDescriptor());
	}

	protected internal virtual IExtenderProvider[] GetExtenderProviders(object instance)
	{
		if (_parent != null)
		{
			return _parent.GetExtenderProviders(instance);
		}
		ArgumentNullException.ThrowIfNull(instance, "instance");
		return Array.Empty<IExtenderProvider>();
	}

	[RequiresUnreferencedCode("The Type of component cannot be statically discovered.")]
	public virtual string? GetFullComponentName(object component)
	{
		if (_parent != null)
		{
			return _parent.GetFullComponentName(component);
		}
		return GetTypeDescriptor(component)?.GetComponentName();
	}

	[return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicFields)]
	public Type GetReflectionType([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicFields)] Type objectType)
	{
		return GetReflectionType(objectType, null);
	}

	[RequiresUnreferencedCode("GetReflectionType is not trim compatible because the Type of object cannot be statically discovered.")]
	public Type GetReflectionType(object instance)
	{
		ArgumentNullException.ThrowIfNull(instance, "instance");
		return GetReflectionType(instance.GetType(), instance);
	}

	[return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicFields)]
	public virtual Type GetReflectionType([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicFields)] Type objectType, object? instance)
	{
		if (_parent != null)
		{
			return _parent.GetReflectionType(objectType, instance);
		}
		return objectType;
	}

	public virtual Type GetRuntimeType(Type reflectionType)
	{
		if (_parent != null)
		{
			return _parent.GetRuntimeType(reflectionType);
		}
		ArgumentNullException.ThrowIfNull(reflectionType, "reflectionType");
		if (reflectionType.GetType().Assembly == typeof(object).Assembly)
		{
			return reflectionType;
		}
		return reflectionType.UnderlyingSystemType;
	}

	public ICustomTypeDescriptor? GetTypeDescriptor([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)] Type objectType)
	{
		return GetTypeDescriptor(objectType, null);
	}

	[RequiresUnreferencedCode("The Type of instance cannot be statically discovered.")]
	public ICustomTypeDescriptor? GetTypeDescriptor(object instance)
	{
		ArgumentNullException.ThrowIfNull(instance, "instance");
		return GetTypeDescriptor(instance.GetType(), instance);
	}

	public virtual ICustomTypeDescriptor? GetTypeDescriptor([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)] Type objectType, object? instance)
	{
		if (_parent != null)
		{
			return _parent.GetTypeDescriptor(objectType, instance);
		}
		return _emptyDescriptor ?? (_emptyDescriptor = new EmptyCustomTypeDescriptor());
	}

	public ICustomTypeDescriptor? GetTypeDescriptorFromRegisteredType(Type objectType)
	{
		ArgumentNullException.ThrowIfNull(objectType, "objectType");
		if (_parent != null)
		{
			return _parent.GetTypeDescriptorFromRegisteredType(objectType);
		}
		return GetTypeDescriptorFromRegisteredType(objectType, null);
	}

	public ICustomTypeDescriptor? GetTypeDescriptorFromRegisteredType(object instance)
	{
		ArgumentNullException.ThrowIfNull(instance, "instance");
		if (_parent != null)
		{
			return _parent.GetTypeDescriptorFromRegisteredType(instance);
		}
		return GetTypeDescriptorFromRegisteredType(instance.GetType(), instance);
	}

	public virtual ICustomTypeDescriptor? GetTypeDescriptorFromRegisteredType(Type objectType, object? instance)
	{
		ArgumentNullException.ThrowIfNull(objectType, "objectType");
		if (_parent != null)
		{
			return _parent.GetTypeDescriptorFromRegisteredType(objectType, instance);
		}
		return Forward();
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2067:UnrecognizedReflectionPattern", Justification = "Forwarding from a type provider that supports registered types to one that does not is supported.")]
		ICustomTypeDescriptor Forward()
		{
			return GetTypeDescriptor(objectType, instance);
		}
	}

	public virtual bool IsRegisteredType(Type type)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		if (_parent != null)
		{
			return _parent.IsRegisteredType(type);
		}
		return false;
	}

	public virtual bool IsSupportedType(Type type)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		if (_parent != null)
		{
			return _parent.IsSupportedType(type);
		}
		return true;
	}
}
