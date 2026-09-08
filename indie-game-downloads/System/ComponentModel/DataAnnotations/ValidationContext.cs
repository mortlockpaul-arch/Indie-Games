using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.ComponentModel.DataAnnotations;

public sealed class ValidationContext : IServiceProvider
{
	private readonly Dictionary<object, object> _items;

	private string _displayName;

	private Func<Type, object> _serviceProvider;

	public object ObjectInstance { get; }

	public Type ObjectType => ObjectInstance.GetType();

	public string DisplayName
	{
		get
		{
			if (string.IsNullOrEmpty(_displayName))
			{
				_displayName = GetDisplayName();
				if (string.IsNullOrEmpty(_displayName))
				{
					_displayName = ObjectType.Name;
				}
			}
			return _displayName;
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentNullException("value");
			}
			_displayName = value;
		}
	}

	public string? MemberName { get; set; }

	public IDictionary<object, object?> Items => _items;

	[RequiresUnreferencedCode("Constructing a ValidationContext without a display name is not trim-safe because it uses reflection to discover the type of the instance being validated in order to resolve the DisplayNameAttribute when a display name is not provided.")]
	public ValidationContext(object instance)
		: this(instance, null, null)
	{
	}

	[RequiresUnreferencedCode("Constructing a ValidationContext without a display name is not trim-safe because it uses reflection to discover the type of the instance being validated in order to resolve the DisplayNameAttribute when a display name is not provided.")]
	public ValidationContext(object instance, IDictionary<object, object?>? items)
		: this(instance, null, items)
	{
	}

	[RequiresUnreferencedCode("Constructing a ValidationContext without a display name is not trim-safe because it uses reflection to discover the type of the instance being validated in order to resolve the DisplayNameAttribute when a display name is not provided.")]
	public ValidationContext(object instance, IServiceProvider? serviceProvider, IDictionary<object, object?>? items)
	{
		ArgumentNullException.ThrowIfNull(instance, "instance");
		if (serviceProvider != null)
		{
			InitializeServiceProvider(serviceProvider.GetService);
		}
		_items = ((items != null) ? new Dictionary<object, object>(items) : new Dictionary<object, object>());
		ObjectInstance = instance;
	}

	public ValidationContext(object instance, string displayName, IServiceProvider? serviceProvider, IDictionary<object, object?>? items)
	{
		ArgumentException.ThrowIfNullOrEmpty(displayName, "displayName");
		ArgumentNullException.ThrowIfNull(instance, "instance");
		if (serviceProvider != null)
		{
			InitializeServiceProvider(serviceProvider.GetService);
		}
		_items = ((items != null) ? new Dictionary<object, object>(items) : new Dictionary<object, object>());
		ObjectInstance = instance;
		DisplayName = displayName;
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "Constructors that trigger this codepath are marked with RequiresUnreferencedCode. Constructor that takes the display name as an argument is trim-safe.")]
	private string GetDisplayName()
	{
		string text = null;
		ValidationAttributeStore instance = ValidationAttributeStore.Instance;
		DisplayAttribute displayAttribute = null;
		if (string.IsNullOrEmpty(MemberName))
		{
			displayAttribute = instance.GetTypeDisplayAttribute(this);
		}
		else if (instance.IsPropertyContext(this))
		{
			displayAttribute = instance.GetPropertyDisplayAttribute(this);
		}
		if (displayAttribute != null)
		{
			text = displayAttribute.GetName();
		}
		return text ?? MemberName;
	}

	public void InitializeServiceProvider(Func<Type, object?> serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public object? GetService(Type serviceType)
	{
		return _serviceProvider?.Invoke(serviceType);
	}
}
