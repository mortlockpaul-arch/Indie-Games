using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace System.ComponentModel;

public abstract class PropertyDescriptor : MemberDescriptor
{
	private TypeConverter _converter;

	private ConcurrentDictionary<object, EventHandler> _valueChangedHandlers;

	private object[] _editors;

	private Type[] _editorTypes;

	private int _editorCount;

	private object _syncObject;

	private object SyncObject => LazyInitializer.EnsureInitialized(ref _syncObject);

	public abstract Type ComponentType { get; }

	public virtual TypeConverter Converter
	{
		[RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered.")]
		get
		{
			AttributeCollection attributes = Attributes;
			if (_converter == null)
			{
				TypeConverterAttribute typeConverterAttribute = (TypeConverterAttribute)attributes[typeof(TypeConverterAttribute)];
				if (typeConverterAttribute.ConverterTypeName != null && typeConverterAttribute.ConverterTypeName.Length > 0)
				{
					Type typeFromName = GetTypeFromName(typeConverterAttribute.ConverterTypeName);
					if (typeFromName != null && typeof(TypeConverter).IsAssignableFrom(typeFromName))
					{
						_converter = (TypeConverter)CreateInstance(typeFromName);
					}
				}
				if (_converter == null)
				{
					_converter = TypeDescriptor.GetConverter(PropertyType);
				}
			}
			return _converter;
		}
	}

	public virtual TypeConverter ConverterFromRegisteredType
	{
		get
		{
			AttributeCollection attributes = Attributes;
			if (_converter == null)
			{
				TypeConverterAttribute typeConverterAttribute = (TypeConverterAttribute)attributes[typeof(TypeConverterAttribute)];
				if (typeConverterAttribute.ConverterTypeName != null && typeConverterAttribute.ConverterTypeName.Length > 0)
				{
					_converter = CreateConverterFromTypeName(typeConverterAttribute);
				}
				if (_converter == null)
				{
					_converter = TypeDescriptor.GetConverterFromRegisteredType(PropertyType);
				}
			}
			return _converter;
			[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:UnrecognizedReflectionPattern", Justification = "GetTypeFromName() can be called on a registered type.")]
			TypeConverter CreateConverterFromTypeName(TypeConverterAttribute attr)
			{
				Type typeFromName = GetTypeFromName(attr.ConverterTypeName);
				if (typeFromName != null && typeof(TypeConverter).IsAssignableFrom(typeFromName))
				{
					return (TypeConverter)CreateInstance(typeFromName);
				}
				return null;
			}
		}
	}

	public virtual bool IsLocalizable => LocalizableAttribute.Yes.Equals(Attributes[typeof(LocalizableAttribute)]);

	public abstract bool IsReadOnly { get; }

	public DesignerSerializationVisibility SerializationVisibility => ((DesignerSerializationVisibilityAttribute)Attributes[typeof(DesignerSerializationVisibilityAttribute)]).Visibility;

	public abstract Type PropertyType { get; }

	public virtual bool SupportsChangeEvents => false;

	protected PropertyDescriptor(string name, Attribute[]? attrs)
		: base(name, attrs)
	{
	}

	protected PropertyDescriptor(MemberDescriptor descr)
		: base(descr)
	{
	}

	protected PropertyDescriptor(MemberDescriptor descr, Attribute[]? attrs)
		: base(descr, attrs)
	{
	}

	public virtual void AddValueChanged(object component, EventHandler handler)
	{
		ArgumentNullException.ThrowIfNull(component, "component");
		ArgumentNullException.ThrowIfNull(handler, "handler");
		lock (SyncObject)
		{
			if (_valueChangedHandlers == null)
			{
				_valueChangedHandlers = new ConcurrentDictionary<object, EventHandler>(1, 0);
			}
			_valueChangedHandlers.AddOrUpdate(component, handler, (object k, EventHandler v) => (EventHandler)Delegate.Combine(v, handler));
		}
	}

	public abstract bool CanResetValue(object component);

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		try
		{
			if (obj == this)
			{
				return true;
			}
			if (obj == null)
			{
				return false;
			}
			if (obj is PropertyDescriptor propertyDescriptor && propertyDescriptor.NameHashCode == NameHashCode && propertyDescriptor.PropertyType == PropertyType && propertyDescriptor.Name.Equals(Name))
			{
				return true;
			}
		}
		catch
		{
		}
		return false;
	}

	protected object? CreateInstance([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type type)
	{
		Type[] array = new Type[1] { typeof(Type) };
		if (type.GetConstructor(array) != null)
		{
			return TypeDescriptor.CreateInstance(null, type, array, new object[1] { PropertyType });
		}
		return TypeDescriptor.CreateInstance(null, type, null, null);
	}

	protected override void FillAttributes(IList attributeList)
	{
		_converter = null;
		_editors = null;
		_editorTypes = null;
		_editorCount = 0;
		base.FillAttributes(attributeList);
	}

	[RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered.")]
	public PropertyDescriptorCollection GetChildProperties()
	{
		return GetChildProperties(null, null);
	}

	[RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered. The public parameterless constructor or the 'Default' static field may be trimmed from the Attribute's Type.")]
	public PropertyDescriptorCollection GetChildProperties(Attribute[] filter)
	{
		return GetChildProperties(null, filter);
	}

	[RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered. The Type of instance cannot be statically discovered.")]
	public PropertyDescriptorCollection GetChildProperties(object instance)
	{
		return GetChildProperties(instance, null);
	}

	[RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered. The Type of instance cannot be statically discovered. The public parameterless constructor or the 'Default' static field may be trimmed from the Attribute's Type.")]
	public virtual PropertyDescriptorCollection GetChildProperties(object? instance, Attribute[]? filter)
	{
		if (instance == null)
		{
			return TypeDescriptor.GetProperties(PropertyType, filter);
		}
		return TypeDescriptor.GetProperties(instance, filter);
	}

	[RequiresUnreferencedCode("Design-time attributes are not preserved when trimming. Types referenced by attributes like EditorAttribute and DesignerAttribute may not be available after trimming. PropertyDescriptor's PropertyType cannot be statically discovered.")]
	public virtual object? GetEditor(Type editorBaseType)
	{
		object obj = null;
		AttributeCollection attributes = Attributes;
		if (_editorTypes != null)
		{
			for (int i = 0; i < _editorCount; i++)
			{
				if (_editorTypes[i] == editorBaseType)
				{
					return _editors[i];
				}
			}
		}
		if (obj == null)
		{
			for (int j = 0; j < attributes.Count; j++)
			{
				if (!(attributes[j] is EditorAttribute editorAttribute))
				{
					continue;
				}
				Type typeFromName = GetTypeFromName(editorAttribute.EditorBaseTypeName);
				if (editorBaseType == typeFromName)
				{
					Type typeFromName2 = GetTypeFromName(editorAttribute.EditorTypeName);
					if (typeFromName2 != null)
					{
						obj = CreateInstance(typeFromName2);
						break;
					}
				}
			}
			if (obj == null)
			{
				obj = TypeDescriptor.GetEditor(PropertyType, editorBaseType);
			}
			if (_editorTypes == null)
			{
				_editorTypes = new Type[5];
				_editors = new object[5];
			}
			if (_editorCount >= _editorTypes.Length)
			{
				Type[] array = new Type[_editorTypes.Length * 2];
				object[] array2 = new object[_editors.Length * 2];
				Array.Copy(_editorTypes, array, _editorTypes.Length);
				Array.Copy(_editors, array2, _editors.Length);
				_editorTypes = array;
				_editors = array2;
			}
			_editorTypes[_editorCount] = editorBaseType;
			_editors[_editorCount++] = obj;
		}
		return obj;
	}

	public override int GetHashCode()
	{
		return NameHashCode ^ PropertyType.GetHashCode();
	}

	protected override object? GetInvocationTarget(Type type, object instance)
	{
		object obj = base.GetInvocationTarget(type, instance);
		if (obj is ICustomTypeDescriptor customTypeDescriptor)
		{
			obj = customTypeDescriptor.GetPropertyOwner(this);
		}
		return obj;
	}

	[RequiresUnreferencedCode("Calls ComponentType.Assembly.GetType on the non-fully qualified typeName, which the trimmer cannot recognize.")]
	[return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
	protected Type? GetTypeFromName([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] string? typeName)
	{
		if (string.IsNullOrEmpty(typeName))
		{
			return null;
		}
		Type type = Type.GetType(typeName);
		Type type2 = null;
		if (ComponentType != null && (type == null || ComponentType.Assembly.FullName.Equals(type.Assembly.FullName)))
		{
			int num = typeName.IndexOf(',');
			if (num != -1)
			{
				typeName = typeName.Substring(0, num);
			}
			type2 = ComponentType.Assembly.GetType(typeName);
		}
		return type2 ?? type;
	}

	public abstract object? GetValue(object? component);

	protected virtual void OnValueChanged(object? component, EventArgs e)
	{
		if (component != null)
		{
			_valueChangedHandlers?.GetValueOrDefault(component, null)?.Invoke(component, e);
		}
	}

	public virtual void RemoveValueChanged(object component, EventHandler handler)
	{
		ArgumentNullException.ThrowIfNull(component, "component");
		ArgumentNullException.ThrowIfNull(handler, "handler");
		if (_valueChangedHandlers == null)
		{
			return;
		}
		lock (SyncObject)
		{
			EventHandler valueOrDefault = _valueChangedHandlers.GetValueOrDefault(component, null);
			valueOrDefault = (EventHandler)Delegate.Remove(valueOrDefault, handler);
			if (valueOrDefault != null)
			{
				_valueChangedHandlers[component] = valueOrDefault;
			}
			else
			{
				_valueChangedHandlers.Remove(component, out var _);
			}
		}
	}

	protected internal EventHandler? GetValueChangedHandler(object component)
	{
		if (component != null && _valueChangedHandlers != null)
		{
			return _valueChangedHandlers.GetValueOrDefault(component, null);
		}
		return null;
	}

	public abstract void ResetValue(object component);

	public abstract void SetValue(object? component, object? value);

	public abstract bool ShouldSerializeValue(object component);
}
