using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.ComponentModel;

public sealed class TypeDescriptor
{
	private sealed class ComNativeDescriptionProvider : TypeDescriptionProvider
	{
		private sealed class ComNativeTypeDescriptor : ICustomTypeDescriptor
		{
			private readonly IComNativeDescriptorHandler _handler;

			private readonly object _instance;

			internal ComNativeTypeDescriptor(IComNativeDescriptorHandler handler, object instance)
			{
				_handler = handler;
				_instance = instance;
			}

			AttributeCollection ICustomTypeDescriptor.GetAttributes()
			{
				return _handler.GetAttributes(_instance);
			}

			string ICustomTypeDescriptor.GetClassName()
			{
				return _handler.GetClassName(_instance);
			}

			string ICustomTypeDescriptor.GetComponentName()
			{
				return null;
			}

			[RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
			TypeConverter ICustomTypeDescriptor.GetConverter()
			{
				return _handler.GetConverter(_instance);
			}

			[RequiresUnreferencedCode("The built-in EventDescriptor implementation uses Reflection which requires unreferenced code.")]
			EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
			{
				return _handler.GetDefaultEvent(_instance);
			}

			[RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered.")]
			PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
			{
				return _handler.GetDefaultProperty(_instance);
			}

			[RequiresUnreferencedCode("Design-time attributes are not preserved when trimming. Types referenced by attributes like EditorAttribute and DesignerAttribute may not be available after trimming.")]
			object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
			{
				return _handler.GetEditor(_instance, editorBaseType);
			}

			EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
			{
				return _handler.GetEvents(_instance);
			}

			[RequiresUnreferencedCode("The public parameterless constructor or the 'Default' static field may be trimmed from the Attribute's Type.")]
			EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
			{
				return _handler.GetEvents(_instance, attributes);
			}

			[RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered.")]
			PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
			{
				return _handler.GetProperties(_instance, null);
			}

			[RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered. The public parameterless constructor or the 'Default' static field may be trimmed from the Attribute's Type.")]
			PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
			{
				return _handler.GetProperties(_instance, attributes);
			}

			object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
			{
				return _instance;
			}
		}

		internal IComNativeDescriptorHandler Handler { get; set; }

		internal ComNativeDescriptionProvider(IComNativeDescriptorHandler handler)
		{
			Handler = handler;
		}

		[return: NotNullIfNotNull("instance")]
		public override ICustomTypeDescriptor GetTypeDescriptor([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)] Type objectType, object instance)
		{
			ArgumentNullException.ThrowIfNull(objectType, "objectType");
			if (instance == null)
			{
				return null;
			}
			if (!objectType.IsInstanceOfType(instance))
			{
				throw new ArgumentException(System.SR.Format(System.SR.ConvertToException, "objectType", instance.GetType()), "instance");
			}
			return new ComNativeTypeDescriptor(Handler, instance);
		}
	}

	private sealed class AttributeProvider : TypeDescriptionProvider
	{
		private sealed class AttributeTypeDescriptor : CustomTypeDescriptor
		{
			private readonly Attribute[] _attributeArray;

			internal AttributeTypeDescriptor(Attribute[] attrs, ICustomTypeDescriptor parent)
				: base(parent)
			{
				_attributeArray = attrs;
			}

			public override AttributeCollection GetAttributes()
			{
				AttributeCollection attributes = base.GetAttributes();
				Attribute[] attributeArray = _attributeArray;
				Attribute[] array = new Attribute[attributes.Count + attributeArray.Length];
				int count = attributes.Count;
				attributes.CopyTo(array, 0);
				for (int i = 0; i < attributeArray.Length; i++)
				{
					bool flag = false;
					for (int j = 0; j < attributes.Count; j++)
					{
						if (array[j].TypeId.Equals(attributeArray[i].TypeId))
						{
							flag = true;
							array[j] = attributeArray[i];
							break;
						}
					}
					if (!flag)
					{
						array[count++] = attributeArray[i];
					}
				}
				Attribute[] array2;
				if (count < array.Length)
				{
					array2 = new Attribute[count];
					Array.Copy(array, array2, count);
				}
				else
				{
					array2 = array;
				}
				return new AttributeCollection(array2);
			}
		}

		private readonly Attribute[] _attrs;

		internal AttributeProvider(TypeDescriptionProvider existingProvider, params Attribute[] attrs)
			: base(existingProvider)
		{
			_attrs = attrs;
		}

		public override ICustomTypeDescriptor GetTypeDescriptor([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)] Type objectType, object instance)
		{
			return new AttributeTypeDescriptor(_attrs, base.GetTypeDescriptor(objectType, instance));
		}
	}

	private sealed class AttributeFilterCacheItem
	{
		private readonly Attribute[] _filter;

		internal readonly ICollection FilteredMembers;

		internal AttributeFilterCacheItem(Attribute[] filter, ICollection filteredMembers)
		{
			_filter = filter;
			FilteredMembers = filteredMembers;
		}

		internal bool IsValid(Attribute[] filter)
		{
			if (_filter.Length != filter.Length)
			{
				return false;
			}
			for (int i = 0; i < filter.Length; i++)
			{
				if (_filter[i] != filter[i])
				{
					return false;
				}
			}
			return true;
		}
	}

	private sealed class FilterCacheItem
	{
		private readonly ITypeDescriptorFilterService _filterService;

		internal readonly ICollection FilteredMembers;

		internal FilterCacheItem(ITypeDescriptorFilterService filterService, ICollection filteredMembers)
		{
			_filterService = filterService;
			FilteredMembers = filteredMembers;
		}

		internal bool IsValid(ITypeDescriptorFilterService filterService)
		{
			if (_filterService != filterService)
			{
				return false;
			}
			return true;
		}
	}

	private sealed class MemberDescriptorComparer : IComparer
	{
		public static readonly MemberDescriptorComparer Instance = new MemberDescriptorComparer();

		public int Compare(object left, object right)
		{
			return CultureInfo.InvariantCulture.CompareInfo.Compare((left as MemberDescriptor)?.Name, (right as MemberDescriptor)?.Name);
		}
	}

	[TypeDescriptionProvider(typeof(ComNativeDescriptorProxy))]
	private sealed class TypeDescriptorComObject
	{
	}

	private sealed class ComNativeDescriptorProxy : TypeDescriptionProvider
	{
		private readonly TypeDescriptionProvider _comNativeDescriptor;

		public ComNativeDescriptorProxy()
		{
			if (!IsComObjectDescriptorSupported)
			{
				throw new NotSupportedException(System.SR.ComObjectDescriptorsNotSupported);
			}
			_comNativeDescriptor = (TypeDescriptionProvider)CreateComNativeDescriptor();
			[MethodImpl(MethodImplOptions.NoInlining)]
			[UnsafeAccessor(UnsafeAccessorKind.Constructor)]
			[return: UnsafeAccessorType("System.Windows.Forms.ComponentModel.Com2Interop.ComNativeDescriptor, System.Windows.Forms")]
			static extern object CreateComNativeDescriptor();
		}

		[return: NotNullIfNotNull("instance")]
		public override ICustomTypeDescriptor GetTypeDescriptor([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)] Type objectType, object instance)
		{
			return _comNativeDescriptor.GetTypeDescriptor(objectType, instance);
		}
	}

	private sealed class MergedTypeDescriptor : ICustomTypeDescriptor
	{
		private readonly ICustomTypeDescriptor _primary;

		private readonly ICustomTypeDescriptor _secondary;

		internal MergedTypeDescriptor(ICustomTypeDescriptor primary, ICustomTypeDescriptor secondary)
		{
			_primary = primary;
			_secondary = secondary;
		}

		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return _primary.GetAttributes() ?? _secondary.GetAttributes();
		}

		string ICustomTypeDescriptor.GetClassName()
		{
			return _primary.GetClassName() ?? _secondary.GetClassName();
		}

		string ICustomTypeDescriptor.GetComponentName()
		{
			return _primary.GetComponentName() ?? _secondary.GetComponentName();
		}

		[RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return _primary.GetConverter() ?? _secondary.GetConverter();
		}

		[RequiresUnreferencedCode("The built-in EventDescriptor implementation uses Reflection which requires unreferenced code.")]
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			return _primary.GetDefaultEvent() ?? _secondary.GetDefaultEvent();
		}

		[RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered.")]
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			return _primary.GetDefaultProperty() ?? _secondary.GetDefaultProperty();
		}

		[RequiresUnreferencedCode("Design-time attributes are not preserved when trimming. Types referenced by attributes like EditorAttribute and DesignerAttribute may not be available after trimming.")]
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			ArgumentNullException.ThrowIfNull(editorBaseType, "editorBaseType");
			return _primary.GetEditor(editorBaseType) ?? _secondary.GetEditor(editorBaseType);
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return _primary.GetEvents() ?? _secondary.GetEvents();
		}

		[RequiresUnreferencedCode("The public parameterless constructor or the 'Default' static field may be trimmed from the Attribute's Type.")]
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			return _primary.GetEvents(attributes) ?? _secondary.GetEvents(attributes);
		}

		[RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered.")]
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			return _primary.GetProperties() ?? _secondary.GetProperties();
		}

		[RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered. The public parameterless constructor or the 'Default' static field may be trimmed from the Attribute's Type.")]
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			return _primary.GetProperties(attributes) ?? _secondary.GetProperties(attributes);
		}

		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
		{
			return _primary.GetPropertyOwner(pd) ?? _secondary.GetPropertyOwner(pd);
		}
	}

	private sealed class TypeDescriptionNode : TypeDescriptionProvider
	{
		private readonly struct DefaultExtendedTypeDescriptor : ICustomTypeDescriptor
		{
			private readonly TypeDescriptionNode _node;

			private readonly object _instance;

			[RequiresUnreferencedCode("The Type of instance cannot be statically discovered.")]
			internal DefaultExtendedTypeDescriptor(TypeDescriptionNode node, object instance)
			{
				_node = node;
				_instance = instance;
			}

			[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "The ctor of this Type has RequiresUnreferencedCode.")]
			AttributeCollection ICustomTypeDescriptor.GetAttributes()
			{
				TypeDescriptionProvider provider = _node.Provider;
				if (provider is ReflectTypeDescriptionProvider)
				{
					return ReflectTypeDescriptionProvider.GetExtendedAttributes();
				}
				return (provider.GetExtendedTypeDescriptor(_instance) ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetExtendedTypeDescriptor"))).GetAttributes() ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetAttributes"));
			}

			[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "The ctor of this Type has RequiresUnreferencedCode.")]
			string ICustomTypeDescriptor.GetClassName()
			{
				TypeDescriptionProvider provider = _node.Provider;
				if (provider is ReflectTypeDescriptionProvider reflectTypeDescriptionProvider)
				{
					return reflectTypeDescriptionProvider.GetExtendedClassName(_instance);
				}
				return (provider.GetExtendedTypeDescriptor(_instance) ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetExtendedTypeDescriptor"))).GetClassName() ?? _instance.GetType().FullName;
			}

			[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "The ctor of this Type has RequiresUnreferencedCode.")]
			string ICustomTypeDescriptor.GetComponentName()
			{
				TypeDescriptionProvider provider = _node.Provider;
				if (provider is ReflectTypeDescriptionProvider)
				{
					return ReflectTypeDescriptionProvider.GetExtendedComponentName(_instance);
				}
				return (provider.GetExtendedTypeDescriptor(_instance) ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetExtendedTypeDescriptor"))).GetComponentName();
			}

			[RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
			TypeConverter ICustomTypeDescriptor.GetConverter()
			{
				TypeDescriptionProvider provider = _node.Provider;
				if (provider is ReflectTypeDescriptionProvider reflectTypeDescriptionProvider)
				{
					return reflectTypeDescriptionProvider.GetExtendedConverter(_instance);
				}
				return (provider.GetExtendedTypeDescriptor(_instance) ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetExtendedTypeDescriptor"))).GetConverter() ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetConverter"));
			}

			TypeConverter ICustomTypeDescriptor.GetConverterFromRegisteredType()
			{
				TypeDescriptionProvider provider = _node.Provider;
				if (provider is ReflectTypeDescriptionProvider reflectTypeDescriptionProvider)
				{
					return reflectTypeDescriptionProvider.GetConverterFromRegisteredType(_instance.GetType(), _instance);
				}
				return (provider.GetTypeDescriptorFromRegisteredType(_instance) ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetExtendedTypeDescriptor"))).GetConverterFromRegisteredType() ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetConverter"));
			}

			[RequiresUnreferencedCode("The built-in EventDescriptor implementation uses Reflection which requires unreferenced code.")]
			EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
			{
				TypeDescriptionProvider provider = _node.Provider;
				if (provider is ReflectTypeDescriptionProvider)
				{
					return null;
				}
				return (provider.GetExtendedTypeDescriptor(_instance) ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetExtendedTypeDescriptor"))).GetDefaultEvent();
			}

			[RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered.")]
			PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
			{
				TypeDescriptionProvider provider = _node.Provider;
				if (provider is ReflectTypeDescriptionProvider)
				{
					return null;
				}
				return (provider.GetExtendedTypeDescriptor(_instance) ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetExtendedTypeDescriptor"))).GetDefaultProperty();
			}

			[RequiresUnreferencedCode("Design-time attributes are not preserved when trimming. Types referenced by attributes like EditorAttribute and DesignerAttribute may not be available after trimming.")]
			object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
			{
				ArgumentNullException.ThrowIfNull(editorBaseType, "editorBaseType");
				TypeDescriptionProvider provider = _node.Provider;
				if (provider is ReflectTypeDescriptionProvider reflectTypeDescriptionProvider)
				{
					return reflectTypeDescriptionProvider.GetExtendedEditor(_instance, editorBaseType);
				}
				return (provider.GetExtendedTypeDescriptor(_instance) ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetExtendedTypeDescriptor"))).GetEditor(editorBaseType);
			}

			[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "The ctor of this Type has RequiresUnreferencedCode.")]
			EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
			{
				TypeDescriptionProvider provider = _node.Provider;
				if (provider is ReflectTypeDescriptionProvider)
				{
					return ReflectTypeDescriptionProvider.GetExtendedEvents();
				}
				return (provider.GetExtendedTypeDescriptor(_instance) ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetExtendedTypeDescriptor"))).GetEvents() ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetEvents"));
			}

			EventDescriptorCollection ICustomTypeDescriptor.GetEventsFromRegisteredType()
			{
				TypeDescriptionProvider provider = _node.Provider;
				if (provider is ReflectTypeDescriptionProvider)
				{
					return ReflectTypeDescriptionProvider.GetExtendedEvents();
				}
				return (provider.GetExtendedTypeDescriptorFromRegisteredType(_instance) ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetExtendedTypeDescriptorFromRegisteredType"))).GetEventsFromRegisteredType() ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetEventsFromRegisteredType"));
			}

			[RequiresUnreferencedCode("The public parameterless constructor or the 'Default' static field may be trimmed from the Attribute's Type.")]
			EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
			{
				TypeDescriptionProvider provider = _node.Provider;
				if (provider is ReflectTypeDescriptionProvider)
				{
					return ReflectTypeDescriptionProvider.GetExtendedEvents();
				}
				return (provider.GetExtendedTypeDescriptor(_instance) ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetExtendedTypeDescriptor"))).GetEvents(attributes) ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetEvents"));
			}

			[RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered.")]
			PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
			{
				TypeDescriptionProvider provider = _node.Provider;
				if (provider is ReflectTypeDescriptionProvider reflectTypeDescriptionProvider)
				{
					return reflectTypeDescriptionProvider.GetExtendedProperties(_instance);
				}
				return (provider.GetExtendedTypeDescriptor(_instance) ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetExtendedTypeDescriptor"))).GetProperties() ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetProperties"));
			}

			PropertyDescriptorCollection ICustomTypeDescriptor.GetPropertiesFromRegisteredType()
			{
				TypeDescriptionProvider provider = _node.Provider;
				if (provider is ReflectTypeDescriptionProvider reflectTypeDescriptionProvider)
				{
					return reflectTypeDescriptionProvider.GetExtendedPropertiesFromRegisteredType(_instance);
				}
				return (provider.GetExtendedTypeDescriptorFromRegisteredType(_instance) ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetExtendedTypeDescriptor"))).GetPropertiesFromRegisteredType() ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetProperties"));
			}

			[RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered. The public parameterless constructor or the 'Default' static field may be trimmed from the Attribute's Type.")]
			PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
			{
				TypeDescriptionProvider provider = _node.Provider;
				if (provider is ReflectTypeDescriptionProvider reflectTypeDescriptionProvider)
				{
					return reflectTypeDescriptionProvider.GetExtendedProperties(_instance);
				}
				return (provider.GetExtendedTypeDescriptor(_instance) ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetExtendedTypeDescriptor"))).GetProperties(attributes) ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetProperties"));
			}

			[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "The ctor of this Type has RequiresUnreferencedCode.")]
			object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
			{
				TypeDescriptionProvider provider = _node.Provider;
				if (provider is ReflectTypeDescriptionProvider)
				{
					return ReflectTypeDescriptionProvider.GetExtendedPropertyOwner(_instance);
				}
				return (provider.GetExtendedTypeDescriptor(_instance) ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetExtendedTypeDescriptor"))).GetPropertyOwner(pd) ?? _instance;
			}
		}

		internal TypeDescriptionNode Next;

		internal TypeDescriptionProvider Provider;

		public override bool? RequireRegisteredTypes => Provider.RequireRegisteredTypes;

		internal TypeDescriptionNode(TypeDescriptionProvider provider)
		{
			Provider = provider;
		}

		public override object CreateInstance(IServiceProvider provider, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, Type[] argTypes, object[] args)
		{
			ArgumentNullException.ThrowIfNull(objectType, "objectType");
			if (argTypes != null)
			{
				ArgumentNullException.ThrowIfNull(args, "args");
				if (argTypes.Length != args.Length)
				{
					throw new ArgumentException(System.SR.TypeDescriptorArgsCountMismatch);
				}
			}
			return Provider.CreateInstance(provider, objectType, argTypes, args);
		}

		public override IDictionary GetCache(object instance)
		{
			ArgumentNullException.ThrowIfNull(instance, "instance");
			return Provider.GetCache(instance);
		}

		[RequiresUnreferencedCode("The Type of instance cannot be statically discovered.")]
		public override ICustomTypeDescriptor GetExtendedTypeDescriptor(object instance)
		{
			ArgumentNullException.ThrowIfNull(instance, "instance");
			return new DefaultExtendedTypeDescriptor(this, instance);
		}

		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "The object is verified at run-time to be a registered type.")]
		public override ICustomTypeDescriptor GetExtendedTypeDescriptorFromRegisteredType(object instance)
		{
			ArgumentNullException.ThrowIfNull(instance, "instance");
			Type type = instance.GetType();
			if (!Provider.IsRegisteredType(type))
			{
				ThrowHelper.ThrowInvalidOperationException_RegisterTypeRequired(type);
			}
			return new DefaultExtendedTypeDescriptor(this, instance);
		}

		protected internal override IExtenderProvider[] GetExtenderProviders(object instance)
		{
			ArgumentNullException.ThrowIfNull(instance, "instance");
			return Provider.GetExtenderProviders(instance);
		}

		[RequiresUnreferencedCode("The Type of component cannot be statically discovered.")]
		public override string GetFullComponentName(object component)
		{
			ArgumentNullException.ThrowIfNull(component, "component");
			return Provider.GetFullComponentName(component);
		}

		[return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicFields)]
		public override Type GetReflectionType([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicFields)] Type objectType, object instance)
		{
			ArgumentNullException.ThrowIfNull(objectType, "objectType");
			return Provider.GetReflectionType(objectType, instance);
		}

		public override Type GetRuntimeType(Type objectType)
		{
			ArgumentNullException.ThrowIfNull(objectType, "objectType");
			return Provider.GetRuntimeType(objectType);
		}

		public override ICustomTypeDescriptor GetTypeDescriptor([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)] Type objectType, object instance)
		{
			ArgumentNullException.ThrowIfNull(objectType, "objectType");
			if (instance != null && !objectType.IsInstanceOfType(instance))
			{
				throw new ArgumentException("instance");
			}
			return new DefaultTypeDescriptor(this, objectType, instance);
		}

		public override ICustomTypeDescriptor GetTypeDescriptorFromRegisteredType(Type objectType, object instance)
		{
			ArgumentNullException.ThrowIfNull(objectType, "objectType");
			if (instance != null && !objectType.IsInstanceOfType(instance))
			{
				throw new ArgumentException("instance");
			}
			if (!IsRegisteredType(objectType))
			{
				ThrowHelper.ThrowInvalidOperationException_RegisterTypeRequired(objectType);
			}
			return Forward();
			[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2067:UnrecognizedReflectionPattern", Justification = "Forwarding from a type provider that supports registered types to one that does not is supported.")]
			ICustomTypeDescriptor Forward()
			{
				return GetTypeDescriptor(objectType, instance);
			}
		}

		internal DefaultTypeDescriptor GetDefaultTypeDescriptor([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)] Type objectType)
		{
			return new DefaultTypeDescriptor(this, objectType, null);
		}

		public override bool IsSupportedType(Type type)
		{
			ArgumentNullException.ThrowIfNull(type, "type");
			return Provider.IsSupportedType(type);
		}

		public override bool IsRegisteredType(Type type)
		{
			return Provider.IsRegisteredType(type);
		}

		public override void RegisterType<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.Interfaces)] T>()
		{
			Provider.RegisterType<T>();
		}
	}

	private readonly struct DefaultTypeDescriptor : ICustomTypeDescriptor
	{
		private readonly TypeDescriptionNode _node;

		[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)]
		private readonly Type _objectType;

		private readonly object _instance;

		internal DefaultTypeDescriptor(TypeDescriptionNode node, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)] Type objectType, object instance)
		{
			_node = node;
			_objectType = objectType;
			_instance = instance;
		}

		public AttributeCollection GetAttributes()
		{
			TypeDescriptionProvider provider = _node.Provider;
			AttributeCollection attributes;
			if (provider is ReflectTypeDescriptionProvider reflectTypeDescriptionProvider)
			{
				attributes = reflectTypeDescriptionProvider.GetAttributes(_objectType);
			}
			else
			{
				attributes = (provider.GetTypeDescriptor(_objectType, _instance) ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetTypeDescriptor"))).GetAttributes();
				if (attributes == null)
				{
					throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetAttributes"));
				}
			}
			return attributes;
		}

		public string GetClassName()
		{
			TypeDescriptionProvider provider = _node.Provider;
			if (provider is ReflectTypeDescriptionProvider reflectTypeDescriptionProvider)
			{
				return reflectTypeDescriptionProvider.GetClassName(_objectType);
			}
			return (provider.GetTypeDescriptor(_objectType, _instance) ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetTypeDescriptor"))).GetClassName() ?? _objectType.FullName;
		}

		string ICustomTypeDescriptor.GetComponentName()
		{
			TypeDescriptionProvider provider = _node.Provider;
			if (provider is ReflectTypeDescriptionProvider)
			{
				return ReflectTypeDescriptionProvider.GetComponentName(_instance);
			}
			return (provider.GetTypeDescriptor(_objectType, _instance) ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetTypeDescriptor"))).GetComponentName();
		}

		[RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
		public TypeConverter GetConverter()
		{
			TypeDescriptionProvider provider = _node.Provider;
			TypeConverter converter;
			if (provider is ReflectTypeDescriptionProvider reflectTypeDescriptionProvider)
			{
				converter = reflectTypeDescriptionProvider.GetConverter(_objectType, _instance);
			}
			else
			{
				converter = (provider.GetTypeDescriptor(_objectType, _instance) ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetTypeDescriptor"))).GetConverter();
				if (converter == null)
				{
					throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetConverter"));
				}
			}
			return converter;
		}

		public TypeConverter GetConverterFromRegisteredType()
		{
			TypeDescriptionProvider provider = _node.Provider;
			TypeConverter converterFromRegisteredType;
			if (provider is ReflectTypeDescriptionProvider reflectTypeDescriptionProvider)
			{
				converterFromRegisteredType = reflectTypeDescriptionProvider.GetConverterFromRegisteredType(_objectType, _instance);
			}
			else
			{
				converterFromRegisteredType = (provider.GetTypeDescriptorFromRegisteredType(_objectType, _instance) ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetTypeDescriptor"))).GetConverterFromRegisteredType();
				if (converterFromRegisteredType == null)
				{
					throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetConverterFromRegisteredType"));
				}
			}
			return converterFromRegisteredType;
		}

		[RequiresUnreferencedCode("The built-in EventDescriptor implementation uses Reflection which requires unreferenced code.")]
		public EventDescriptor GetDefaultEvent()
		{
			TypeDescriptionProvider provider = _node.Provider;
			if (provider is ReflectTypeDescriptionProvider reflectTypeDescriptionProvider)
			{
				return reflectTypeDescriptionProvider.GetDefaultEvent(_objectType, _instance);
			}
			return (provider.GetTypeDescriptor(_objectType, _instance) ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetTypeDescriptor"))).GetDefaultEvent();
		}

		[RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered.")]
		public PropertyDescriptor GetDefaultProperty()
		{
			TypeDescriptionProvider provider = _node.Provider;
			if (provider is ReflectTypeDescriptionProvider reflectTypeDescriptionProvider)
			{
				return reflectTypeDescriptionProvider.GetDefaultProperty(_objectType, _instance);
			}
			return (provider.GetTypeDescriptor(_objectType, _instance) ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetTypeDescriptor"))).GetDefaultProperty();
		}

		[RequiresUnreferencedCode("Design-time attributes are not preserved when trimming. Types referenced by attributes like EditorAttribute and DesignerAttribute may not be available after trimming.")]
		public object GetEditor(Type editorBaseType)
		{
			ArgumentNullException.ThrowIfNull(editorBaseType, "editorBaseType");
			TypeDescriptionProvider provider = _node.Provider;
			if (provider is ReflectTypeDescriptionProvider reflectTypeDescriptionProvider)
			{
				return reflectTypeDescriptionProvider.GetEditor(_objectType, _instance, editorBaseType);
			}
			return (provider.GetTypeDescriptor(_objectType, _instance) ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetTypeDescriptor"))).GetEditor(editorBaseType);
		}

		public EventDescriptorCollection GetEvents()
		{
			TypeDescriptionProvider provider = _node.Provider;
			EventDescriptorCollection events;
			if (provider is ReflectTypeDescriptionProvider reflectTypeDescriptionProvider)
			{
				events = reflectTypeDescriptionProvider.GetEvents(_objectType);
			}
			else
			{
				events = (provider.GetTypeDescriptor(_objectType, _instance) ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetTypeDescriptor"))).GetEvents();
				if (events == null)
				{
					throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetEvents"));
				}
			}
			return events;
		}

		public EventDescriptorCollection GetEventsFromRegisteredType()
		{
			TypeDescriptionProvider provider = _node.Provider;
			EventDescriptorCollection eventsFromRegisteredType;
			if (provider is ReflectTypeDescriptionProvider reflectTypeDescriptionProvider)
			{
				eventsFromRegisteredType = reflectTypeDescriptionProvider.GetEventsFromRegisteredType(_objectType);
			}
			else
			{
				eventsFromRegisteredType = (provider.GetTypeDescriptorFromRegisteredType(_objectType, _instance) ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetTypeDescriptorFromRegisteredType"))).GetEventsFromRegisteredType();
				if (eventsFromRegisteredType == null)
				{
					throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetEventsFromRegisteredType"));
				}
			}
			return eventsFromRegisteredType;
		}

		[RequiresUnreferencedCode("The public parameterless constructor or the 'Default' static field may be trimmed from the Attribute's Type.")]
		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			TypeDescriptionProvider provider = _node.Provider;
			EventDescriptorCollection events;
			if (provider is ReflectTypeDescriptionProvider reflectTypeDescriptionProvider)
			{
				events = reflectTypeDescriptionProvider.GetEvents(_objectType);
			}
			else
			{
				events = (provider.GetTypeDescriptor(_objectType, _instance) ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetTypeDescriptor"))).GetEvents(attributes);
				if (events == null)
				{
					throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetEvents"));
				}
			}
			return events;
		}

		[RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered.")]
		public PropertyDescriptorCollection GetProperties()
		{
			TypeDescriptionProvider provider = _node.Provider;
			PropertyDescriptorCollection properties;
			if (provider is ReflectTypeDescriptionProvider reflectTypeDescriptionProvider)
			{
				properties = reflectTypeDescriptionProvider.GetProperties(_objectType);
			}
			else
			{
				properties = (provider.GetTypeDescriptor(_objectType, _instance) ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetTypeDescriptor"))).GetProperties();
				if (properties == null)
				{
					throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetProperties"));
				}
			}
			return properties;
		}

		public PropertyDescriptorCollection GetPropertiesFromRegisteredType()
		{
			TypeDescriptionProvider provider = _node.Provider;
			PropertyDescriptorCollection propertiesFromRegisteredType;
			if (provider is ReflectTypeDescriptionProvider reflectTypeDescriptionProvider)
			{
				propertiesFromRegisteredType = reflectTypeDescriptionProvider.GetPropertiesFromRegisteredType(_objectType);
			}
			else
			{
				propertiesFromRegisteredType = (provider.GetTypeDescriptorFromRegisteredType(_objectType, _instance) ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetTypeDescriptor"))).GetPropertiesFromRegisteredType();
				if (propertiesFromRegisteredType == null)
				{
					throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetPropertiesFromRegisteredType"));
				}
			}
			return propertiesFromRegisteredType;
		}

		[RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered. The public parameterless constructor or the 'Default' static field may be trimmed from the Attribute's Type.")]
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			TypeDescriptionProvider provider = _node.Provider;
			PropertyDescriptorCollection properties;
			if (provider is ReflectTypeDescriptionProvider reflectTypeDescriptionProvider)
			{
				properties = reflectTypeDescriptionProvider.GetProperties(_objectType);
			}
			else
			{
				properties = (provider.GetTypeDescriptor(_objectType, _instance) ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetTypeDescriptor"))).GetProperties(attributes);
				if (properties == null)
				{
					throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetProperties"));
				}
			}
			return properties;
		}

		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			TypeDescriptionProvider provider = _node.Provider;
			if (provider is ReflectTypeDescriptionProvider)
			{
				return ReflectTypeDescriptionProvider.GetPropertyOwner(_objectType, _instance);
			}
			return (provider.GetTypeDescriptor(_objectType, _instance) ?? throw new InvalidOperationException(System.SR.Format(System.SR.TypeDescriptorProviderError, _node.Provider.GetType().FullName, "GetTypeDescriptor"))).GetPropertyOwner(pd) ?? _instance;
		}
	}

	private sealed class TypeDescriptorInterface
	{
	}

	internal static class ThrowHelper
	{
		[DoesNotReturn]
		internal static void ThrowNotImplementedException_CustomTypeProviderMustImplememtMember(string memberName)
		{
			throw new NotImplementedException(System.SR.Format(System.SR.CustomTypeProviderNotImplemented, memberName));
		}

		[DoesNotReturn]
		internal static void ThrowInvalidOperationException_RegisterTypeRequired(Type type)
		{
			throw new InvalidOperationException(System.SR.Format(System.SR.TypeIsNotRegistered, type.FullName));
		}
	}

	private static readonly WeakHashtable s_providerTable = new WeakHashtable();

	internal static readonly object s_commonSyncObject = new object();

	private static readonly ConcurrentDictionary<Type, TypeDescriptionNode> s_providerTypeTable = new ConcurrentDictionary<Type, TypeDescriptionNode>();

	private static readonly ConcurrentDictionary<Type, object> s_defaultProviderInitialized = new ConcurrentDictionary<Type, object>();

	private static readonly object s_initializedDefaultProvider = new object();

	private static WeakHashtable s_associationTable;

	private static int s_metadataVersion;

	private static int s_collisionIndex;

	private static readonly Guid[] s_pipelineInitializeKeys = new Guid[3]
	{
		Guid.NewGuid(),
		Guid.NewGuid(),
		Guid.NewGuid()
	};

	private static readonly Guid[] s_pipelineMergeKeys = new Guid[3]
	{
		Guid.NewGuid(),
		Guid.NewGuid(),
		Guid.NewGuid()
	};

	private static readonly Guid[] s_pipelineFilterKeys = new Guid[3]
	{
		Guid.NewGuid(),
		Guid.NewGuid(),
		Guid.NewGuid()
	};

	private static readonly Guid[] s_pipelineAttributeFilterKeys = new Guid[3]
	{
		Guid.NewGuid(),
		Guid.NewGuid(),
		Guid.NewGuid()
	};

	private static readonly bool s_requireRegisteredTypes = AppContext.TryGetSwitch("System.ComponentModel.TypeDescriptor.RequireRegisteredTypes", out var isEnabled) && isEnabled;

	[FeatureSwitchDefinition("System.ComponentModel.TypeDescriptor.IsComObjectDescriptorSupported")]
	[FeatureGuard(typeof(RequiresUnreferencedCodeAttribute))]
	internal static bool IsComObjectDescriptorSupported
	{
		get
		{
			if (!AppContext.TryGetSwitch("System.ComponentModel.TypeDescriptor.IsComObjectDescriptorSupported", out var isEnabled))
			{
				return true;
			}
			return isEnabled;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static Type InterfaceType
	{
		[return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
		get
		{
			return typeof(TypeDescriptorInterface);
		}
	}

	internal static int MetadataVersion => s_metadataVersion;

	private static WeakHashtable AssociationTable => LazyInitializer.EnsureInitialized(ref s_associationTable, () => new WeakHashtable());

	[FeatureSwitchDefinition("System.ComponentModel.TypeDescriptor.RequireRegisteredTypes")]
	internal static bool RequireRegisteredTypes => s_requireRegisteredTypes;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static Type ComObjectType
	{
		[RequiresUnreferencedCode("COM type descriptors are not trim-compatible.")]
		[return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
		get
		{
			return typeof(TypeDescriptorComObject);
		}
	}

	[Obsolete("TypeDescriptor.ComNativeDescriptorHandler has been deprecated. Use a type description provider to supply type information for COM types instead.")]
	public static IComNativeDescriptorHandler? ComNativeDescriptorHandler
	{
		[RequiresUnreferencedCode("COM type descriptors are not trim-compatible.")]
		get
		{
			TypeDescriptionNode typeDescriptionNode = NodeFor(ComObjectType);
			ComNativeDescriptionProvider comNativeDescriptionProvider;
			do
			{
				comNativeDescriptionProvider = typeDescriptionNode.Provider as ComNativeDescriptionProvider;
				typeDescriptionNode = typeDescriptionNode.Next;
			}
			while (typeDescriptionNode != null && comNativeDescriptionProvider == null);
			return comNativeDescriptionProvider?.Handler;
		}
		[RequiresUnreferencedCode("COM type descriptors are not trim-compatible.")]
		[param: DisallowNull]
		set
		{
			TypeDescriptionNode typeDescriptionNode = NodeFor(ComObjectType);
			while (typeDescriptionNode != null && !(typeDescriptionNode.Provider is ComNativeDescriptionProvider))
			{
				typeDescriptionNode = typeDescriptionNode.Next;
			}
			if (typeDescriptionNode == null)
			{
				AddProvider(new ComNativeDescriptionProvider(value), ComObjectType);
			}
			else
			{
				((ComNativeDescriptionProvider)typeDescriptionNode.Provider).Handler = value;
			}
		}
	}

	public static event RefreshEventHandler? Refreshed;

	public static void RegisterType<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.Interfaces)] T>()
	{
		NodeFor(typeof(T), createDelegator: false).Provider.RegisterType<T>();
	}

	internal static void ValidateRegisteredType(Type type)
	{
		TypeDescriptionProvider provider = GetProvider(type);
		if (provider.RequireRegisteredTypes == true && !provider.IsRegisteredType(type))
		{
			ThrowHelper.ThrowInvalidOperationException_RegisterTypeRequired(type);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static TypeDescriptionProvider AddAttributes(Type type, params Attribute[] attributes)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		ArgumentNullException.ThrowIfNull(attributes, "attributes");
		AttributeProvider attributeProvider = new AttributeProvider(GetProvider(type), attributes);
		AddProvider(attributeProvider, type);
		return attributeProvider;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static TypeDescriptionProvider AddAttributes(object instance, params Attribute[] attributes)
	{
		ArgumentNullException.ThrowIfNull(instance, "instance");
		ArgumentNullException.ThrowIfNull(attributes, "attributes");
		AttributeProvider attributeProvider = new AttributeProvider(GetProvider(instance), attributes);
		AddProvider(attributeProvider, instance);
		return attributeProvider;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[RequiresUnreferencedCode("The Types specified in table may be trimmed, or have their static constructors trimmed.")]
	public static void AddEditorTable(Type editorBaseType, Hashtable table)
	{
		ReflectTypeDescriptionProvider.AddEditorTable(editorBaseType, table);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static void AddProvider(TypeDescriptionProvider provider, Type type)
	{
		ArgumentNullException.ThrowIfNull(provider, "provider");
		ArgumentNullException.ThrowIfNull(type, "type");
		lock (s_commonSyncObject)
		{
			TypeDescriptionNode next = NodeFor(type, createDelegator: true);
			TypeDescriptionNode value = new TypeDescriptionNode(provider)
			{
				Next = next
			};
			s_providerTable[type] = value;
			s_providerTypeTable.Clear();
		}
		Refresh(type);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static void AddProvider(TypeDescriptionProvider provider, object instance)
	{
		ArgumentNullException.ThrowIfNull(provider, "provider");
		ArgumentNullException.ThrowIfNull(instance, "instance");
		bool flag;
		lock (s_commonSyncObject)
		{
			flag = s_providerTable.ContainsKey(instance);
			TypeDescriptionNode next = NodeFor(instance, createDelegator: true);
			TypeDescriptionNode value = new TypeDescriptionNode(provider)
			{
				Next = next
			};
			s_providerTable.SetWeak(instance, value);
			s_providerTypeTable.Clear();
		}
		if (flag)
		{
			Refresh(instance, refreshReflectionProvider: false);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static void AddProviderTransparent(TypeDescriptionProvider provider, Type type)
	{
		ArgumentNullException.ThrowIfNull(provider, "provider");
		ArgumentNullException.ThrowIfNull(type, "type");
		AddProvider(provider, type);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static void AddProviderTransparent(TypeDescriptionProvider provider, object instance)
	{
		ArgumentNullException.ThrowIfNull(provider, "provider");
		ArgumentNullException.ThrowIfNull(instance, "instance");
		AddProvider(provider, instance);
	}

	private static void CheckDefaultProvider(Type type)
	{
		if (s_defaultProviderInitialized.TryGetValue(type, out var value) && value == s_initializedDefaultProvider)
		{
			return;
		}
		lock (s_commonSyncObject)
		{
			AddDefaultProvider(type);
		}
	}

	private static void AddDefaultProvider(Type type)
	{
		bool flag = false;
		if (s_defaultProviderInitialized.ContainsKey(type))
		{
			return;
		}
		s_defaultProviderInitialized.TryAdd(type, null);
		object[] customAttributes = type.GetCustomAttributes(typeof(TypeDescriptionProviderAttribute), inherit: false);
		for (int num = customAttributes.Length - 1; num >= 0; num--)
		{
			Type type2 = Type.GetType(((TypeDescriptionProviderAttribute)customAttributes[num]).TypeName);
			if (type2 != null && typeof(TypeDescriptionProvider).IsAssignableFrom(type2))
			{
				AddProvider((TypeDescriptionProvider)Activator.CreateInstance(type2), type);
				flag = true;
			}
		}
		if (!flag)
		{
			Type baseType = type.BaseType;
			if (baseType != null && baseType != type)
			{
				AddDefaultProvider(baseType);
			}
		}
		s_defaultProviderInitialized[type] = s_initializedDefaultProvider;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static void CreateAssociation(object primary, object secondary)
	{
		ArgumentNullException.ThrowIfNull(primary, "primary");
		ArgumentNullException.ThrowIfNull(secondary, "secondary");
		if (primary == secondary)
		{
			throw new ArgumentException(System.SR.TypeDescriptorSameAssociation);
		}
		WeakHashtable associationTable = AssociationTable;
		IList list = (IList)associationTable[primary];
		if (list == null)
		{
			lock (associationTable)
			{
				list = (IList)associationTable[primary];
				if (list == null)
				{
					list = new ArrayList(4);
					associationTable.SetWeak(primary, list);
				}
			}
		}
		else
		{
			for (int num = list.Count - 1; num >= 0; num--)
			{
				WeakReference weakReference = (WeakReference)list[num];
				if (weakReference.IsAlive && weakReference.Target == secondary)
				{
					throw new ArgumentException(System.SR.TypeDescriptorAlreadyAssociated);
				}
			}
		}
		lock (list)
		{
			list.Add(new WeakReference(secondary));
		}
	}

	public static EventDescriptor CreateEvent([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)] Type componentType, string name, Type type, params Attribute[] attributes)
	{
		return new ReflectEventDescriptor(componentType, name, type, attributes);
	}

	public static EventDescriptor CreateEvent([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)] Type componentType, EventDescriptor oldEventDescriptor, params Attribute[] attributes)
	{
		return new ReflectEventDescriptor(componentType, oldEventDescriptor, attributes);
	}

	public static object? CreateInstance(IServiceProvider? provider, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, Type[]? argTypes, object?[]? args)
	{
		ArgumentNullException.ThrowIfNull(objectType, "objectType");
		if (argTypes != null)
		{
			ArgumentNullException.ThrowIfNull(args, "args");
			if (argTypes.Length != args.Length)
			{
				throw new ArgumentException(System.SR.TypeDescriptorArgsCountMismatch);
			}
		}
		object obj = null;
		if (provider?.GetService(typeof(TypeDescriptionProvider)) is TypeDescriptionProvider typeDescriptionProvider)
		{
			obj = typeDescriptionProvider.CreateInstance(provider, objectType, argTypes, args);
		}
		return obj ?? NodeFor(objectType).CreateInstance(provider, objectType, argTypes, args);
	}

	[RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered.")]
	public static PropertyDescriptor CreateProperty([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)] Type componentType, string name, Type type, params Attribute[] attributes)
	{
		return new ReflectPropertyDescriptor(componentType, name, type, attributes);
	}

	[RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered.")]
	public static PropertyDescriptor CreateProperty([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)] Type componentType, PropertyDescriptor oldPropertyDescriptor, params Attribute[] attributes)
	{
		if (componentType == oldPropertyDescriptor.ComponentType && ((ExtenderProvidedPropertyAttribute)oldPropertyDescriptor.Attributes[typeof(ExtenderProvidedPropertyAttribute)]).ExtenderProperty is ReflectPropertyDescriptor)
		{
			return new ExtendedPropertyDescriptor(oldPropertyDescriptor, attributes);
		}
		return new ReflectPropertyDescriptor(componentType, oldPropertyDescriptor, attributes);
	}

	[RequiresUnreferencedCode("The public parameterless constructor or the 'Default' static field may be trimmed from the Attribute's Type.")]
	private static ArrayList FilterMembers(IList members, Attribute[] attributes)
	{
		ArrayList arrayList = null;
		int count = members.Count;
		for (int i = 0; i < count; i++)
		{
			bool flag = false;
			for (int j = 0; j < attributes.Length; j++)
			{
				if (ShouldHideMember((MemberDescriptor)members[i], attributes[j]))
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				if (arrayList == null)
				{
					arrayList = new ArrayList(count);
					for (int k = 0; k < i; k++)
					{
						arrayList.Add(members[k]);
					}
				}
			}
			else
			{
				arrayList?.Add(members[i]);
			}
		}
		return arrayList;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static object GetAssociation(Type type, object primary)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		ArgumentNullException.ThrowIfNull(primary, "primary");
		object obj = primary;
		if (!type.IsInstanceOfType(primary))
		{
			IList list = (IList)(AssociationTable?[primary]);
			if (list != null)
			{
				lock (list)
				{
					for (int num = list.Count - 1; num >= 0; num--)
					{
						object target = ((WeakReference)list[num]).Target;
						if (target == null)
						{
							list.RemoveAt(num);
						}
						else if (type.IsInstanceOfType(target))
						{
							obj = target;
						}
					}
				}
			}
			if (obj == primary && primary is IComponent component)
			{
				ISite site = component.Site;
				if (site != null && site.DesignMode && site.GetService(typeof(IDesignerHost)) is IDesignerHost designerHost)
				{
					object designer = designerHost.GetDesigner(component);
					if (designer != null && type.IsInstanceOfType(designer))
					{
						obj = designer;
					}
				}
			}
		}
		return obj;
	}

	public static AttributeCollection GetAttributes([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)] Type componentType)
	{
		if (componentType == null)
		{
			return new AttributeCollection((Attribute[]?)null);
		}
		return GetDescriptor(componentType, "componentType").GetAttributes();
	}

	[RequiresUnreferencedCode("The Type of component cannot be statically discovered.")]
	public static AttributeCollection GetAttributes(object component)
	{
		return GetAttributes(component, noCustomTypeDesc: false);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[RequiresUnreferencedCode("The Type of component cannot be statically discovered.")]
	public static AttributeCollection GetAttributes(object component, bool noCustomTypeDesc)
	{
		if (component == null)
		{
			return new AttributeCollection((Attribute[]?)null);
		}
		ICollection collection = GetDescriptor(component, noCustomTypeDesc).GetAttributes();
		if (component is ICustomTypeDescriptor)
		{
			if (noCustomTypeDesc)
			{
				ICustomTypeDescriptor extendedDescriptor = GetExtendedDescriptor(component);
				if (extendedDescriptor != null)
				{
					ICollection attributes = extendedDescriptor.GetAttributes();
					collection = PipelineMerge(0, collection, attributes, null);
				}
			}
			else
			{
				collection = PipelineFilter(0, collection, component, null);
			}
		}
		else
		{
			IDictionary cache = GetCache(component);
			collection = PipelineInitialize(0, collection, cache);
			ICustomTypeDescriptor extendedDescriptor2 = GetExtendedDescriptor(component);
			if (extendedDescriptor2 != null)
			{
				ICollection attributes2 = extendedDescriptor2.GetAttributes();
				collection = PipelineMerge(0, collection, attributes2, cache);
			}
			collection = PipelineFilter(0, collection, component, cache);
		}
		AttributeCollection attributeCollection = collection as AttributeCollection;
		if (attributeCollection == null)
		{
			Attribute[] array = new Attribute[collection.Count];
			collection.CopyTo(array, 0);
			attributeCollection = new AttributeCollection(array);
		}
		return attributeCollection;
	}

	internal static IDictionary GetCache(object instance)
	{
		return NodeFor(instance).GetCache(instance);
	}

	[RequiresUnreferencedCode("The Type of component cannot be statically discovered.")]
	public static string? GetClassName(object component)
	{
		return GetClassName(component, noCustomTypeDesc: false);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[RequiresUnreferencedCode("The Type of component cannot be statically discovered.")]
	public static string? GetClassName(object component, bool noCustomTypeDesc)
	{
		return GetDescriptor(component, noCustomTypeDesc).GetClassName();
	}

	public static string? GetClassName([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)] Type componentType)
	{
		return GetDescriptor(componentType, "componentType").GetClassName();
	}

	[RequiresUnreferencedCode("The Type of component cannot be statically discovered.")]
	public static string? GetComponentName(object component)
	{
		return GetComponentName(component, noCustomTypeDesc: false);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[RequiresUnreferencedCode("The Type of component cannot be statically discovered.")]
	public static string? GetComponentName(object component, bool noCustomTypeDesc)
	{
		return GetDescriptor(component, noCustomTypeDesc).GetComponentName();
	}

	[RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All. The Type of component cannot be statically discovered.")]
	public static TypeConverter GetConverter(object component)
	{
		return GetConverter(component, noCustomTypeDesc: false);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All. The Type of component cannot be statically discovered.")]
	public static TypeConverter GetConverter(object component, bool noCustomTypeDesc)
	{
		return GetDescriptor(component, noCustomTypeDesc).GetConverter();
	}

	public static TypeConverter GetConverterFromRegisteredType(object component)
	{
		return GetDescriptorFromRegisteredType(component).GetConverterFromRegisteredType();
	}

	[RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
	public static TypeConverter GetConverter([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)] Type type)
	{
		return GetDescriptor(type, "type").GetConverter();
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "The callers of this method ensure getting the converter is trim compatible - i.e. the type is not Nullable<T>.")]
	internal static TypeConverter GetConverterTrimUnsafe([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)] Type type)
	{
		return GetConverter(type);
	}

	public static TypeConverter GetConverterFromRegisteredType(Type type)
	{
		return GetDescriptorFromRegisteredType(type, "type").GetConverterFromRegisteredType();
	}

	[RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
	private static object ConvertFromInvariantString([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)] Type type, string stringValue)
	{
		return GetConverter(type).ConvertFromInvariantString(stringValue);
	}

	[RequiresUnreferencedCode("The built-in EventDescriptor implementation uses Reflection which requires unreferenced code.")]
	public static EventDescriptor? GetDefaultEvent([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)] Type componentType)
	{
		if (componentType == null)
		{
			return null;
		}
		return GetDescriptor(componentType, "componentType").GetDefaultEvent();
	}

	[RequiresUnreferencedCode("The built-in EventDescriptor implementation uses Reflection which requires unreferenced code. The Type of component cannot be statically discovered.")]
	public static EventDescriptor? GetDefaultEvent(object component)
	{
		return GetDefaultEvent(component, noCustomTypeDesc: false);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[RequiresUnreferencedCode("The built-in EventDescriptor implementation uses Reflection which requires unreferenced code. The Type of component cannot be statically discovered.")]
	public static EventDescriptor? GetDefaultEvent(object component, bool noCustomTypeDesc)
	{
		if (component == null)
		{
			return null;
		}
		return GetDescriptor(component, noCustomTypeDesc).GetDefaultEvent();
	}

	[RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered.")]
	public static PropertyDescriptor? GetDefaultProperty([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)] Type componentType)
	{
		if (componentType == null)
		{
			return null;
		}
		return GetDescriptor(componentType, "componentType").GetDefaultProperty();
	}

	[RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered. The Type of component cannot be statically discovered.")]
	public static PropertyDescriptor? GetDefaultProperty(object component)
	{
		return GetDefaultProperty(component, noCustomTypeDesc: false);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered. The Type of component cannot be statically discovered.")]
	public static PropertyDescriptor? GetDefaultProperty(object component, bool noCustomTypeDesc)
	{
		if (component == null)
		{
			return null;
		}
		return GetDescriptor(component, noCustomTypeDesc).GetDefaultProperty();
	}

	private static DefaultTypeDescriptor GetDescriptor([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)] Type type, string typeName)
	{
		ArgumentNullException.ThrowIfNull(type, typeName);
		return NodeFor(type).GetDefaultTypeDescriptor(type);
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2067:UnrecognizedReflectionPattern", Justification = "The type will be validated to see if it was registered.")]
	private static DefaultTypeDescriptor GetDescriptorFromRegisteredType(Type type, string typeName)
	{
		ArgumentNullException.ThrowIfNull(type, typeName);
		return NodeFor(type).GetDefaultTypeDescriptor(type);
	}

	[RequiresUnreferencedCode("The Type of component cannot be statically discovered.")]
	internal static ICustomTypeDescriptor GetDescriptor(object component, bool noCustomTypeDesc)
	{
		if (component == null)
		{
			throw new ArgumentException("component");
		}
		ICustomTypeDescriptor customTypeDescriptor = NodeFor(component).GetTypeDescriptor(component);
		ICustomTypeDescriptor customTypeDescriptor2 = component as ICustomTypeDescriptor;
		if (!noCustomTypeDesc && customTypeDescriptor2 != null)
		{
			customTypeDescriptor = new MergedTypeDescriptor(customTypeDescriptor2, customTypeDescriptor);
		}
		return customTypeDescriptor;
	}

	internal static ICustomTypeDescriptor GetDescriptorFromRegisteredType(object component)
	{
		ICustomTypeDescriptor customTypeDescriptor = NodeFor(component).GetTypeDescriptorFromRegisteredType(component);
		if (component is ICustomTypeDescriptor primary)
		{
			customTypeDescriptor = new MergedTypeDescriptor(primary, customTypeDescriptor);
		}
		return customTypeDescriptor;
	}

	[RequiresUnreferencedCode("The Type of component cannot be statically discovered.")]
	internal static ICustomTypeDescriptor GetExtendedDescriptor(object component)
	{
		if (component == null)
		{
			throw new ArgumentException("component");
		}
		return NodeFor(component).GetExtendedTypeDescriptor(component);
	}

	internal static ICustomTypeDescriptor GetExtendedDescriptorFromRegisteredType(object component)
	{
		if (component == null)
		{
			throw new ArgumentException("component");
		}
		return NodeFor(component).GetExtendedTypeDescriptorFromRegisteredType(component);
	}

	[RequiresUnreferencedCode("Design-time attributes are not preserved when trimming. Types referenced by attributes like EditorAttribute and DesignerAttribute may not be available after trimming. The Type of component cannot be statically discovered.")]
	public static object? GetEditor(object component, Type editorBaseType)
	{
		return GetEditor(component, editorBaseType, noCustomTypeDesc: false);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[RequiresUnreferencedCode("Design-time attributes are not preserved when trimming. Types referenced by attributes like EditorAttribute and DesignerAttribute may not be available after trimming. The Type of component cannot be statically discovered.")]
	public static object? GetEditor(object component, Type editorBaseType, bool noCustomTypeDesc)
	{
		ArgumentNullException.ThrowIfNull(editorBaseType, "editorBaseType");
		return GetDescriptor(component, noCustomTypeDesc).GetEditor(editorBaseType);
	}

	[RequiresUnreferencedCode("Design-time attributes are not preserved when trimming. Types referenced by attributes like EditorAttribute and DesignerAttribute may not be available after trimming.")]
	public static object? GetEditor([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)] Type type, Type editorBaseType)
	{
		ArgumentNullException.ThrowIfNull(editorBaseType, "editorBaseType");
		return GetDescriptor(type, "type").GetEditor(editorBaseType);
	}

	public static EventDescriptorCollection GetEvents([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)] Type componentType)
	{
		if (componentType == null)
		{
			return new EventDescriptorCollection(null, readOnly: true);
		}
		return GetDescriptor(componentType, "componentType").GetEvents();
	}

	public static EventDescriptorCollection GetEventsFromRegisteredType(Type componentType)
	{
		ArgumentNullException.ThrowIfNull(componentType, "componentType");
		return GetDescriptorFromRegisteredType(componentType, "componentType").GetEventsFromRegisteredType();
	}

	[RequiresUnreferencedCode("The public parameterless constructor or the 'Default' static field may be trimmed from the Attribute's Type.")]
	public static EventDescriptorCollection GetEvents([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)] Type componentType, Attribute[] attributes)
	{
		if (componentType == null)
		{
			return new EventDescriptorCollection(null, readOnly: true);
		}
		EventDescriptorCollection eventDescriptorCollection = GetDescriptor(componentType, "componentType").GetEvents(attributes);
		if (attributes != null && attributes.Length != 0)
		{
			ArrayList arrayList = FilterMembers(eventDescriptorCollection, attributes);
			if (arrayList != null)
			{
				EventDescriptor[] array = new EventDescriptor[arrayList.Count];
				arrayList.CopyTo(array);
				eventDescriptorCollection = new EventDescriptorCollection(array, readOnly: true);
			}
		}
		return eventDescriptorCollection;
	}

	[RequiresUnreferencedCode("The Type of component cannot be statically discovered.")]
	public static EventDescriptorCollection GetEvents(object component)
	{
		return GetEvents(component, null, noCustomTypeDesc: false);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[RequiresUnreferencedCode("The Type of component cannot be statically discovered.")]
	public static EventDescriptorCollection GetEvents(object component, bool noCustomTypeDesc)
	{
		return GetEvents(component, null, noCustomTypeDesc);
	}

	[RequiresUnreferencedCode("The Type of component cannot be statically discovered. The public parameterless constructor or the 'Default' static field may be trimmed from the Attribute's Type.")]
	public static EventDescriptorCollection GetEvents(object component, Attribute[] attributes)
	{
		return GetEvents(component, attributes, noCustomTypeDesc: false);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[RequiresUnreferencedCode("The Type of component cannot be statically discovered. The public parameterless constructor or the 'Default' static field may be trimmed from the Attribute's Type.")]
	public static EventDescriptorCollection GetEvents(object component, Attribute[]? attributes, bool noCustomTypeDesc)
	{
		if (component == null)
		{
			return new EventDescriptorCollection(null, readOnly: true);
		}
		ICustomTypeDescriptor descriptor = GetDescriptor(component, noCustomTypeDesc);
		ICollection collection;
		if (component is ICustomTypeDescriptor)
		{
			collection = descriptor.GetEvents(attributes);
			if (noCustomTypeDesc)
			{
				ICustomTypeDescriptor extendedDescriptor = GetExtendedDescriptor(component);
				if (extendedDescriptor != null)
				{
					ICollection events = extendedDescriptor.GetEvents(attributes);
					collection = PipelineMerge(2, collection, events, null);
				}
			}
			else
			{
				collection = PipelineFilter(2, collection, component, null);
				collection = PipelineAttributeFilter(2, collection, attributes, null);
			}
		}
		else
		{
			IDictionary cache = GetCache(component);
			collection = descriptor.GetEvents(attributes);
			collection = PipelineInitialize(2, collection, cache);
			ICustomTypeDescriptor extendedDescriptor2 = GetExtendedDescriptor(component);
			if (extendedDescriptor2 != null)
			{
				ICollection events2 = extendedDescriptor2.GetEvents(attributes);
				collection = PipelineMerge(2, collection, events2, cache);
			}
			collection = PipelineFilter(2, collection, component, cache);
			collection = PipelineAttributeFilter(2, collection, attributes, cache);
		}
		EventDescriptorCollection eventDescriptorCollection = collection as EventDescriptorCollection;
		if (eventDescriptorCollection == null)
		{
			EventDescriptor[] array = new EventDescriptor[collection.Count];
			collection.CopyTo(array, 0);
			eventDescriptorCollection = new EventDescriptorCollection(array, readOnly: true);
		}
		return eventDescriptorCollection;
	}

	private static string GetExtenderCollisionSuffix(MemberDescriptor member)
	{
		string result = null;
		IExtenderProvider extenderProvider = (member.Attributes[typeof(ExtenderProvidedPropertyAttribute)] as ExtenderProvidedPropertyAttribute)?.Provider;
		if (extenderProvider != null)
		{
			string text = null;
			if (extenderProvider is IComponent { Site: not null } component)
			{
				text = component.Site.Name;
			}
			if (string.IsNullOrEmpty(text))
			{
				text = (Interlocked.Increment(ref s_collisionIndex) - 1).ToString(CultureInfo.InvariantCulture);
			}
			result = "_" + text;
		}
		return result;
	}

	[RequiresUnreferencedCode("The Type of component cannot be statically discovered.")]
	public static string? GetFullComponentName(object component)
	{
		ArgumentNullException.ThrowIfNull(component, "component");
		return GetProvider(component).GetFullComponentName(component);
	}

	private static Type GetNodeForBaseType(Type searchType)
	{
		if (searchType.IsInterface)
		{
			return InterfaceType;
		}
		if (searchType == InterfaceType)
		{
			return null;
		}
		return searchType.BaseType;
	}

	[RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered.")]
	public static PropertyDescriptorCollection GetProperties([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)] Type componentType)
	{
		if (componentType == null)
		{
			return new PropertyDescriptorCollection(null, readOnly: true);
		}
		return GetDescriptor(componentType, "componentType").GetProperties();
	}

	public static PropertyDescriptorCollection GetPropertiesFromRegisteredType(Type componentType)
	{
		ArgumentNullException.ThrowIfNull(componentType, "componentType");
		return GetDescriptorFromRegisteredType(componentType, "componentType").GetPropertiesFromRegisteredType();
	}

	[RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered. The public parameterless constructor or the 'Default' static field may be trimmed from the Attribute's Type.")]
	public static PropertyDescriptorCollection GetProperties([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllConstructors | DynamicallyAccessedMemberTypes.AllNestedTypes | DynamicallyAccessedMemberTypes.AllMethods | DynamicallyAccessedMemberTypes.AllFields | DynamicallyAccessedMemberTypes.AllProperties | DynamicallyAccessedMemberTypes.AllEvents | DynamicallyAccessedMemberTypes.Interfaces)] Type componentType, Attribute[]? attributes)
	{
		if (componentType == null)
		{
			return new PropertyDescriptorCollection(null, readOnly: true);
		}
		PropertyDescriptorCollection propertyDescriptorCollection = GetDescriptor(componentType, "componentType").GetProperties(attributes);
		if (attributes != null && attributes.Length != 0)
		{
			ArrayList arrayList = FilterMembers(propertyDescriptorCollection, attributes);
			if (arrayList != null)
			{
				PropertyDescriptor[] array = new PropertyDescriptor[arrayList.Count];
				arrayList.CopyTo(array);
				propertyDescriptorCollection = new PropertyDescriptorCollection(array, readOnly: true);
			}
		}
		return propertyDescriptorCollection;
	}

	[RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered. The Type of component cannot be statically discovered.")]
	public static PropertyDescriptorCollection GetProperties(object component)
	{
		return GetProperties(component, noCustomTypeDesc: false);
	}

	public static PropertyDescriptorCollection GetPropertiesFromRegisteredType(object component)
	{
		return GetPropertiesFromRegisteredTypeImpl(component);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered. The Type of component cannot be statically discovered.")]
	public static PropertyDescriptorCollection GetProperties(object component, bool noCustomTypeDesc)
	{
		return GetPropertiesImpl(component, null, noCustomTypeDesc, noAttributes: true);
	}

	[RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered. The Type of component cannot be statically discovered. The public parameterless constructor or the 'Default' static field may be trimmed from the Attribute's Type.")]
	public static PropertyDescriptorCollection GetProperties(object component, Attribute[]? attributes)
	{
		return GetProperties(component, attributes, noCustomTypeDesc: false);
	}

	[RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered. The Type of component cannot be statically discovered. The public parameterless constructor or the 'Default' static field may be trimmed from the Attribute's Type.")]
	public static PropertyDescriptorCollection GetProperties(object component, Attribute[]? attributes, bool noCustomTypeDesc)
	{
		return GetPropertiesImpl(component, attributes, noCustomTypeDesc, noAttributes: false);
	}

	[RequiresUnreferencedCode("PropertyDescriptor's PropertyType cannot be statically discovered. The Type of component cannot be statically discovered. The public parameterless constructor or the 'Default' static field may be trimmed from the Attribute's Type.")]
	private static PropertyDescriptorCollection GetPropertiesImpl(object component, Attribute[] attributes, bool noCustomTypeDesc, bool noAttributes)
	{
		if (component == null)
		{
			return new PropertyDescriptorCollection(null, readOnly: true);
		}
		ICustomTypeDescriptor descriptor = GetDescriptor(component, noCustomTypeDesc);
		ICollection collection;
		if (component is ICustomTypeDescriptor)
		{
			collection = (noAttributes ? descriptor.GetProperties() : descriptor.GetProperties(attributes));
			if (noCustomTypeDesc)
			{
				ICustomTypeDescriptor extendedDescriptor = GetExtendedDescriptor(component);
				if (extendedDescriptor != null)
				{
					ICollection secondary = (noAttributes ? extendedDescriptor.GetProperties() : extendedDescriptor.GetProperties(attributes));
					collection = PipelineMerge(1, collection, secondary, null);
				}
			}
			else
			{
				collection = PipelineFilter(1, collection, component, null);
				collection = PipelineAttributeFilter(1, collection, attributes, null);
			}
		}
		else
		{
			IDictionary cache = GetCache(component);
			collection = (noAttributes ? descriptor.GetProperties() : descriptor.GetProperties(attributes));
			collection = PipelineInitialize(1, collection, cache);
			ICustomTypeDescriptor extendedDescriptor2 = GetExtendedDescriptor(component);
			if (extendedDescriptor2 != null)
			{
				ICollection secondary2 = (noAttributes ? extendedDescriptor2.GetProperties() : extendedDescriptor2.GetProperties(attributes));
				collection = PipelineMerge(1, collection, secondary2, cache);
			}
			collection = PipelineFilter(1, collection, component, cache);
			collection = PipelineAttributeFilter(1, collection, attributes, cache);
		}
		PropertyDescriptorCollection propertyDescriptorCollection = collection as PropertyDescriptorCollection;
		if (propertyDescriptorCollection == null)
		{
			PropertyDescriptor[] array = new PropertyDescriptor[collection.Count];
			collection.CopyTo(array, 0);
			propertyDescriptorCollection = new PropertyDescriptorCollection(array, readOnly: true);
		}
		return propertyDescriptorCollection;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static TypeDescriptionProvider GetProvider(Type type)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return NodeFor(type, createDelegator: true);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static TypeDescriptionProvider GetProvider(object instance)
	{
		ArgumentNullException.ThrowIfNull(instance, "instance");
		return NodeFor(instance, createDelegator: true);
	}

	private static PropertyDescriptorCollection GetPropertiesFromRegisteredTypeImpl(object component)
	{
		ArgumentNullException.ThrowIfNull(component, "component");
		ICustomTypeDescriptor descriptorFromRegisteredType = GetDescriptorFromRegisteredType(component);
		ICollection propertiesFromRegisteredType;
		if (component is ICustomTypeDescriptor)
		{
			propertiesFromRegisteredType = descriptorFromRegisteredType.GetPropertiesFromRegisteredType();
			propertiesFromRegisteredType = PipelineFilter(1, propertiesFromRegisteredType, component, null);
		}
		else
		{
			IDictionary cache = GetCache(component);
			propertiesFromRegisteredType = descriptorFromRegisteredType.GetPropertiesFromRegisteredType();
			propertiesFromRegisteredType = PipelineInitialize(1, propertiesFromRegisteredType, cache);
			ICustomTypeDescriptor extendedDescriptorFromRegisteredType = GetExtendedDescriptorFromRegisteredType(component);
			if (extendedDescriptorFromRegisteredType != null)
			{
				ICollection propertiesFromRegisteredType2 = extendedDescriptorFromRegisteredType.GetPropertiesFromRegisteredType();
				propertiesFromRegisteredType = PipelineMerge(1, propertiesFromRegisteredType, propertiesFromRegisteredType2, cache);
			}
			propertiesFromRegisteredType = PipelineFilter(1, propertiesFromRegisteredType, component, cache);
		}
		PropertyDescriptorCollection propertyDescriptorCollection = propertiesFromRegisteredType as PropertyDescriptorCollection;
		if (propertyDescriptorCollection == null)
		{
			PropertyDescriptor[] array = new PropertyDescriptor[propertiesFromRegisteredType.Count];
			propertiesFromRegisteredType.CopyTo(array, 0);
			propertyDescriptorCollection = new PropertyDescriptorCollection(array, readOnly: true);
		}
		return propertyDescriptorCollection;
	}

	internal static TypeDescriptionProvider GetProviderRecursive(Type type)
	{
		return NodeFor(type, createDelegator: false);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicFields)]
	public static Type GetReflectionType([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicFields)] Type type)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		return NodeFor(type).GetReflectionType(type);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	[RequiresUnreferencedCode("GetReflectionType is not trim compatible because the Type of object cannot be statically discovered.")]
	public static Type GetReflectionType(object instance)
	{
		ArgumentNullException.ThrowIfNull(instance, "instance");
		return NodeFor(instance).GetReflectionType(instance);
	}

	private static TypeDescriptionNode NodeFor(Type type)
	{
		return NodeFor(type, createDelegator: false);
	}

	private static TypeDescriptionNode NodeFor(Type type, bool createDelegator)
	{
		CheckDefaultProvider(type);
		TypeDescriptionNode value = null;
		Type type2 = type;
		while (value == null)
		{
			if (!s_providerTypeTable.TryGetValue(type2, out value))
			{
				value = (TypeDescriptionNode)s_providerTable[type2];
			}
			if (value != null)
			{
				continue;
			}
			Type nodeForBaseType = GetNodeForBaseType(type2);
			if (type2 == typeof(object) || nodeForBaseType == null)
			{
				lock (s_commonSyncObject)
				{
					value = (TypeDescriptionNode)s_providerTable[type2];
					if (value == null)
					{
						value = new TypeDescriptionNode(new ReflectTypeDescriptionProvider());
						s_providerTable[type2] = value;
					}
				}
			}
			else if (createDelegator)
			{
				value = new TypeDescriptionNode(new DelegatingTypeDescriptionProvider(nodeForBaseType));
				lock (s_commonSyncObject)
				{
					s_providerTypeTable.TryAdd(type2, value);
				}
			}
			else
			{
				type2 = nodeForBaseType;
			}
		}
		return value;
	}

	private static TypeDescriptionNode NodeFor(object instance)
	{
		return NodeFor(instance, createDelegator: false);
	}

	private static TypeDescriptionNode NodeFor(object instance, bool createDelegator)
	{
		TypeDescriptionNode typeDescriptionNode = (TypeDescriptionNode)s_providerTable[instance];
		if (typeDescriptionNode == null)
		{
			Type type = instance.GetType();
			nint unknown;
			if (type.IsCOMObject)
			{
				if (!IsComObjectDescriptorSupported)
				{
					throw new NotSupportedException(System.SR.ComObjectDescriptorsNotSupported);
				}
				type = ComObjectType;
			}
			else if (OperatingSystem.IsWindows() && ComWrappers.TryGetComInstance(instance, out unknown))
			{
				if (!IsComObjectDescriptorSupported)
				{
					throw new NotSupportedException(System.SR.ComObjectDescriptorsNotSupported);
				}
				Marshal.Release(unknown);
				type = ComObjectType;
			}
			typeDescriptionNode = ((!createDelegator) ? NodeFor(type) : new TypeDescriptionNode(new DelegatingTypeDescriptionProvider(type)));
		}
		return typeDescriptionNode;
	}

	private static void NodeRemove(object key, TypeDescriptionProvider provider)
	{
		lock (s_commonSyncObject)
		{
			TypeDescriptionNode typeDescriptionNode = (TypeDescriptionNode)s_providerTable[key];
			TypeDescriptionNode typeDescriptionNode2 = typeDescriptionNode;
			while (typeDescriptionNode2 != null && typeDescriptionNode2.Provider != provider)
			{
				typeDescriptionNode2 = typeDescriptionNode2.Next;
			}
			if (typeDescriptionNode2 == null)
			{
				return;
			}
			if (typeDescriptionNode2.Next != null)
			{
				typeDescriptionNode2.Provider = typeDescriptionNode2.Next.Provider;
				typeDescriptionNode2.Next = typeDescriptionNode2.Next.Next;
				if (typeDescriptionNode2 == typeDescriptionNode && typeDescriptionNode2.Provider is DelegatingTypeDescriptionProvider)
				{
					s_providerTable.Remove(key);
				}
			}
			else if (typeDescriptionNode2 != typeDescriptionNode)
			{
				Type type = (key as Type) ?? key.GetType();
				typeDescriptionNode2.Provider = new DelegatingTypeDescriptionProvider(type.BaseType);
			}
			else
			{
				s_providerTable.Remove(key);
			}
			s_providerTypeTable.Clear();
		}
	}

	[RequiresUnreferencedCode("The public parameterless constructor or the 'Default' static field may be trimmed from the Attribute's Type.")]
	private static ICollection PipelineAttributeFilter(int pipelineType, ICollection members, Attribute[] filter, IDictionary cache)
	{
		IList list = members as ArrayList;
		if (filter == null || filter.Length == 0)
		{
			return members;
		}
		if (cache != null && (list == null || list.IsReadOnly) && cache[s_pipelineAttributeFilterKeys[pipelineType]] is AttributeFilterCacheItem attributeFilterCacheItem && attributeFilterCacheItem.IsValid(filter))
		{
			return attributeFilterCacheItem.FilteredMembers;
		}
		if (list == null || list.IsReadOnly)
		{
			list = new ArrayList(members);
		}
		ArrayList arrayList = FilterMembers(list, filter);
		if (arrayList != null)
		{
			list = arrayList;
		}
		if (cache != null)
		{
			ICollection filteredMembers;
			switch (pipelineType)
			{
			case 1:
			{
				PropertyDescriptor[] array2 = new PropertyDescriptor[list.Count];
				list.CopyTo(array2, 0);
				filteredMembers = new PropertyDescriptorCollection(array2, readOnly: true);
				break;
			}
			case 2:
			{
				EventDescriptor[] array = new EventDescriptor[list.Count];
				list.CopyTo(array, 0);
				filteredMembers = new EventDescriptorCollection(array, readOnly: true);
				break;
			}
			default:
				filteredMembers = null;
				break;
			}
			AttributeFilterCacheItem value = new AttributeFilterCacheItem(filter, filteredMembers);
			cache[s_pipelineAttributeFilterKeys[pipelineType]] = value;
		}
		return list;
	}

	private static ICollection PipelineFilter(int pipelineType, ICollection members, object instance, IDictionary cache)
	{
		IComponent component = instance as IComponent;
		ITypeDescriptorFilterService typeDescriptorFilterService = null;
		ISite site = component?.Site;
		if (site != null)
		{
			typeDescriptorFilterService = site.GetService(typeof(ITypeDescriptorFilterService)) as ITypeDescriptorFilterService;
		}
		IList list = members as ArrayList;
		if (typeDescriptorFilterService == null)
		{
			return members;
		}
		if (cache != null && (list == null || list.IsReadOnly) && cache[s_pipelineFilterKeys[pipelineType]] is FilterCacheItem filterCacheItem && filterCacheItem.IsValid(typeDescriptorFilterService))
		{
			return filterCacheItem.FilteredMembers;
		}
		OrderedDictionary orderedDictionary = new OrderedDictionary(members.Count);
		bool flag;
		switch (pipelineType)
		{
		case 0:
			foreach (Attribute member in members)
			{
				orderedDictionary[member.TypeId] = member;
			}
			flag = typeDescriptorFilterService.FilterAttributes(component, orderedDictionary);
			break;
		case 1:
		case 2:
			foreach (MemberDescriptor member2 in members)
			{
				string name = member2.Name;
				if (orderedDictionary.Contains(name))
				{
					string extenderCollisionSuffix = GetExtenderCollisionSuffix(member2);
					if (extenderCollisionSuffix != null)
					{
						orderedDictionary[name + extenderCollisionSuffix] = member2;
					}
					MemberDescriptor memberDescriptor2 = (MemberDescriptor)orderedDictionary[name];
					extenderCollisionSuffix = GetExtenderCollisionSuffix(memberDescriptor2);
					if (extenderCollisionSuffix != null)
					{
						orderedDictionary.Remove(name);
						orderedDictionary[memberDescriptor2.Name + extenderCollisionSuffix] = memberDescriptor2;
					}
				}
				else
				{
					orderedDictionary[name] = member2;
				}
			}
			flag = ((pipelineType != 1) ? typeDescriptorFilterService.FilterEvents(component, orderedDictionary) : typeDescriptorFilterService.FilterProperties(component, orderedDictionary));
			break;
		default:
			flag = false;
			break;
		}
		if (list == null || list.IsReadOnly)
		{
			list = new ArrayList(orderedDictionary.Values);
		}
		else
		{
			list.Clear();
			foreach (object value2 in orderedDictionary.Values)
			{
				list.Add(value2);
			}
		}
		if (flag && cache != null)
		{
			ICollection filteredMembers;
			switch (pipelineType)
			{
			case 0:
			{
				Attribute[] array2 = new Attribute[list.Count];
				try
				{
					list.CopyTo(array2, 0);
				}
				catch (InvalidCastException)
				{
					throw new ArgumentException(System.SR.Format(System.SR.TypeDescriptorExpectedElementType, typeof(Attribute).FullName));
				}
				filteredMembers = new AttributeCollection(array2);
				break;
			}
			case 1:
			{
				PropertyDescriptor[] array3 = new PropertyDescriptor[list.Count];
				try
				{
					list.CopyTo(array3, 0);
				}
				catch (InvalidCastException)
				{
					throw new ArgumentException(System.SR.Format(System.SR.TypeDescriptorExpectedElementType, typeof(PropertyDescriptor).FullName));
				}
				filteredMembers = new PropertyDescriptorCollection(array3, readOnly: true);
				break;
			}
			case 2:
			{
				EventDescriptor[] array = new EventDescriptor[list.Count];
				try
				{
					list.CopyTo(array, 0);
				}
				catch (InvalidCastException)
				{
					throw new ArgumentException(System.SR.Format(System.SR.TypeDescriptorExpectedElementType, typeof(EventDescriptor).FullName));
				}
				filteredMembers = new EventDescriptorCollection(array, readOnly: true);
				break;
			}
			default:
				filteredMembers = null;
				break;
			}
			FilterCacheItem value = new FilterCacheItem(typeDescriptorFilterService, filteredMembers);
			cache[s_pipelineFilterKeys[pipelineType]] = value;
			cache.Remove(s_pipelineAttributeFilterKeys[pipelineType]);
		}
		return list;
	}

	private static ICollection PipelineInitialize(int pipelineType, ICollection members, IDictionary cache)
	{
		if (cache != null)
		{
			bool flag = true;
			if (cache[s_pipelineInitializeKeys[pipelineType]] is ICollection collection && collection.Count == members.Count)
			{
				IEnumerator enumerator = collection.GetEnumerator();
				IEnumerator enumerator2 = members.GetEnumerator();
				while (enumerator.MoveNext() && enumerator2.MoveNext())
				{
					if (enumerator.Current != enumerator2.Current)
					{
						flag = false;
						break;
					}
				}
			}
			if (!flag)
			{
				cache.Remove(s_pipelineMergeKeys[pipelineType]);
				cache.Remove(s_pipelineFilterKeys[pipelineType]);
				cache.Remove(s_pipelineAttributeFilterKeys[pipelineType]);
				cache[s_pipelineInitializeKeys[pipelineType]] = members;
			}
		}
		return members;
	}

	private static ICollection PipelineMerge(int pipelineType, ICollection primary, ICollection secondary, IDictionary cache)
	{
		if (secondary == null || secondary.Count == 0)
		{
			return primary;
		}
		if (cache?[s_pipelineMergeKeys[pipelineType]] is ICollection collection && collection.Count == primary.Count + secondary.Count)
		{
			IEnumerator enumerator = collection.GetEnumerator();
			IEnumerator enumerator2 = primary.GetEnumerator();
			bool flag = true;
			while (enumerator2.MoveNext() && enumerator.MoveNext())
			{
				if (enumerator2.Current != enumerator.Current)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				IEnumerator enumerator3 = secondary.GetEnumerator();
				while (enumerator3.MoveNext() && enumerator.MoveNext())
				{
					if (enumerator3.Current != enumerator.Current)
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				return collection;
			}
		}
		ArrayList arrayList = new ArrayList(primary.Count + secondary.Count);
		foreach (object item in primary)
		{
			arrayList.Add(item);
		}
		foreach (object item2 in secondary)
		{
			arrayList.Add(item2);
		}
		if (cache != null)
		{
			ICollection value;
			switch (pipelineType)
			{
			case 0:
			{
				Attribute[] array3 = new Attribute[arrayList.Count];
				arrayList.CopyTo(array3, 0);
				value = new AttributeCollection(array3);
				break;
			}
			case 1:
			{
				PropertyDescriptor[] array2 = new PropertyDescriptor[arrayList.Count];
				arrayList.CopyTo(array2, 0);
				value = new PropertyDescriptorCollection(array2, readOnly: true);
				break;
			}
			case 2:
			{
				EventDescriptor[] array = new EventDescriptor[arrayList.Count];
				arrayList.CopyTo(array, 0);
				value = new EventDescriptorCollection(array, readOnly: true);
				break;
			}
			default:
				value = null;
				break;
			}
			cache[s_pipelineMergeKeys[pipelineType]] = value;
			cache.Remove(s_pipelineFilterKeys[pipelineType]);
			cache.Remove(s_pipelineAttributeFilterKeys[pipelineType]);
		}
		return arrayList;
	}

	private static void RaiseRefresh(object component)
	{
		Volatile.Read(in Refreshed)?.Invoke(new RefreshEventArgs(component));
	}

	private static void RaiseRefresh(Type type)
	{
		Volatile.Read(in Refreshed)?.Invoke(new RefreshEventArgs(type));
	}

	public static void Refresh(object component)
	{
		Refresh(component, refreshReflectionProvider: true);
	}

	private static void Refresh(object component, bool refreshReflectionProvider)
	{
		if (component == null)
		{
			return;
		}
		bool flag = false;
		if (refreshReflectionProvider)
		{
			Type type = component.GetType();
			lock (s_commonSyncObject)
			{
				IDictionaryEnumerator enumerator = s_providerTable.GetEnumerator();
				while (enumerator.MoveNext())
				{
					DictionaryEntry entry = enumerator.Entry;
					Type type2 = entry.Key as Type;
					if ((!(type2 != null) || !type.IsAssignableFrom(type2)) && !(type2 == typeof(object)))
					{
						continue;
					}
					TypeDescriptionNode typeDescriptionNode = (TypeDescriptionNode)entry.Value;
					while (typeDescriptionNode != null && !(typeDescriptionNode.Provider is ReflectTypeDescriptionProvider))
					{
						flag = true;
						typeDescriptionNode = typeDescriptionNode.Next;
					}
					if (typeDescriptionNode != null)
					{
						ReflectTypeDescriptionProvider reflectTypeDescriptionProvider = (ReflectTypeDescriptionProvider)typeDescriptionNode.Provider;
						if (reflectTypeDescriptionProvider.IsPopulated(type))
						{
							flag = true;
							reflectTypeDescriptionProvider.Refresh(type);
						}
					}
				}
			}
		}
		IDictionary cache = GetCache(component);
		if (!flag && cache == null)
		{
			return;
		}
		if (cache != null)
		{
			for (int i = 0; i < s_pipelineFilterKeys.Length; i++)
			{
				cache.Remove(s_pipelineFilterKeys[i]);
				cache.Remove(s_pipelineMergeKeys[i]);
				cache.Remove(s_pipelineAttributeFilterKeys[i]);
			}
		}
		Interlocked.Increment(ref s_metadataVersion);
		RaiseRefresh(component);
	}

	public static void Refresh(Type type)
	{
		if (type == null)
		{
			return;
		}
		bool flag = false;
		lock (s_commonSyncObject)
		{
			IDictionaryEnumerator enumerator = s_providerTable.GetEnumerator();
			while (enumerator.MoveNext())
			{
				DictionaryEntry entry = enumerator.Entry;
				Type type2 = entry.Key as Type;
				if ((!(type2 != null) || !type.IsAssignableFrom(type2)) && !(type2 == typeof(object)))
				{
					continue;
				}
				TypeDescriptionNode typeDescriptionNode = (TypeDescriptionNode)entry.Value;
				while (typeDescriptionNode != null && !(typeDescriptionNode.Provider is ReflectTypeDescriptionProvider))
				{
					flag = true;
					typeDescriptionNode = typeDescriptionNode.Next;
				}
				if (typeDescriptionNode != null)
				{
					ReflectTypeDescriptionProvider reflectTypeDescriptionProvider = (ReflectTypeDescriptionProvider)typeDescriptionNode.Provider;
					if (reflectTypeDescriptionProvider.IsPopulated(type))
					{
						flag = true;
						reflectTypeDescriptionProvider.Refresh(type);
					}
				}
			}
		}
		if (flag)
		{
			Interlocked.Increment(ref s_metadataVersion);
			RaiseRefresh(type);
		}
	}

	public static void Refresh(Module module)
	{
		if (module == null)
		{
			return;
		}
		Hashtable hashtable = null;
		lock (s_commonSyncObject)
		{
			IDictionaryEnumerator enumerator = s_providerTable.GetEnumerator();
			while (enumerator.MoveNext())
			{
				DictionaryEntry entry = enumerator.Entry;
				Type type = entry.Key as Type;
				if ((!(type != null) || !type.Module.Equals(module)) && !(type == typeof(object)))
				{
					continue;
				}
				TypeDescriptionNode typeDescriptionNode = (TypeDescriptionNode)entry.Value;
				while (typeDescriptionNode != null && !(typeDescriptionNode.Provider is ReflectTypeDescriptionProvider))
				{
					if (hashtable == null)
					{
						hashtable = new Hashtable();
					}
					hashtable[type] = type;
					typeDescriptionNode = typeDescriptionNode.Next;
				}
				if (typeDescriptionNode == null)
				{
					continue;
				}
				ReflectTypeDescriptionProvider reflectTypeDescriptionProvider = (ReflectTypeDescriptionProvider)typeDescriptionNode.Provider;
				Type[] populatedTypes = reflectTypeDescriptionProvider.GetPopulatedTypes(module);
				foreach (Type type2 in populatedTypes)
				{
					reflectTypeDescriptionProvider.Refresh(type2);
					if (hashtable == null)
					{
						hashtable = new Hashtable();
					}
					hashtable[type2] = type2;
				}
			}
		}
		if (hashtable == null || Refreshed == null)
		{
			return;
		}
		foreach (Type key in hashtable.Keys)
		{
			RaiseRefresh(key);
		}
	}

	public static void Refresh(Assembly assembly)
	{
		if (!(assembly == null))
		{
			Module[] modules = assembly.GetModules();
			for (int i = 0; i < modules.Length; i++)
			{
				Refresh(modules[i]);
			}
		}
	}

	[RequiresUnreferencedCode("Design-time attributes are not preserved when trimming. Types referenced by attributes like EditorAttribute and DesignerAttribute may not be available after trimming.")]
	public static IDesigner? CreateDesigner(IComponent component, Type designerBaseType)
	{
		Type type = null;
		IDesigner result = null;
		AttributeCollection attributes = GetAttributes(component);
		for (int i = 0; i < attributes.Count; i++)
		{
			if (!(attributes[i] is DesignerAttribute designerAttribute))
			{
				continue;
			}
			Type type2 = Type.GetType(designerAttribute.DesignerBaseTypeName);
			if (type2 != null && type2 == designerBaseType)
			{
				ISite? site = component.Site;
				bool flag = false;
				ITypeResolutionService typeResolutionService = (ITypeResolutionService)(site?.GetService(typeof(ITypeResolutionService)));
				if (typeResolutionService != null)
				{
					flag = true;
					type = typeResolutionService.GetType(designerAttribute.DesignerTypeName);
				}
				if (!flag)
				{
					type = Type.GetType(designerAttribute.DesignerTypeName);
				}
				if (type != null)
				{
					break;
				}
			}
		}
		if (type != null)
		{
			result = (IDesigner)Activator.CreateInstance(type);
		}
		return result;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static void RemoveAssociation(object primary, object secondary)
	{
		ArgumentNullException.ThrowIfNull(primary, "primary");
		ArgumentNullException.ThrowIfNull(secondary, "secondary");
		IList list = (IList)(AssociationTable?[primary]);
		if (list == null)
		{
			return;
		}
		lock (list)
		{
			for (int num = list.Count - 1; num >= 0; num--)
			{
				object target = ((WeakReference)list[num]).Target;
				if (target == null || target == secondary)
				{
					list.RemoveAt(num);
				}
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static void RemoveAssociations(object primary)
	{
		ArgumentNullException.ThrowIfNull(primary, "primary");
		AssociationTable?.Remove(primary);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static void RemoveProvider(TypeDescriptionProvider provider, Type type)
	{
		ArgumentNullException.ThrowIfNull(provider, "provider");
		ArgumentNullException.ThrowIfNull(type, "type");
		NodeRemove(type, provider);
		RaiseRefresh(type);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static void RemoveProvider(TypeDescriptionProvider provider, object instance)
	{
		ArgumentNullException.ThrowIfNull(provider, "provider");
		ArgumentNullException.ThrowIfNull(instance, "instance");
		NodeRemove(instance, provider);
		RaiseRefresh(instance);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static void RemoveProviderTransparent(TypeDescriptionProvider provider, Type type)
	{
		ArgumentNullException.ThrowIfNull(provider, "provider");
		ArgumentNullException.ThrowIfNull(type, "type");
		RemoveProvider(provider, type);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static void RemoveProviderTransparent(TypeDescriptionProvider provider, object instance)
	{
		ArgumentNullException.ThrowIfNull(provider, "provider");
		ArgumentNullException.ThrowIfNull(instance, "instance");
		RemoveProvider(provider, instance);
	}

	[RequiresUnreferencedCode("The public parameterless constructor or the 'Default' static field may be trimmed from the Attribute's Type.")]
	private static bool ShouldHideMember(MemberDescriptor member, Attribute attribute)
	{
		if (member == null || attribute == null)
		{
			return true;
		}
		Attribute attribute2 = member.Attributes[attribute.GetType()];
		if (attribute2 == null)
		{
			return !attribute.IsDefaultAttribute();
		}
		return !attribute.Match(attribute2);
	}

	public static void SortDescriptorArray(IList infos)
	{
		ArgumentNullException.ThrowIfNull(infos, "infos");
		ArrayList.Adapter(infos).Sort(MemberDescriptorComparer.Instance);
	}
}
