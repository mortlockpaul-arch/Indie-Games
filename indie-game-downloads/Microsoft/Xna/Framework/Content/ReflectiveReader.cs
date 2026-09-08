using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Xna.Framework.Content;

internal class ReflectiveReader<T> : ContentTypeReader
{
	private delegate void ReadElement(ContentReader input, object parent);

	private List<ReadElement> readers;

	private ConstructorInfo constructor;

	private ContentTypeReader baseTypeReader;

	public override bool CanDeserializeIntoExistingObject => base.TargetType.IsClass;

	internal ReflectiveReader()
		: base(typeof(T))
	{
	}

	protected internal override void Initialize(ContentTypeReaderManager manager)
	{
		base.Initialize(manager);
		Type baseType = base.TargetType.BaseType;
		if ((object)baseType != null && baseType != typeof(object))
		{
			baseTypeReader = manager.GetTypeReader(baseType);
		}
		constructor = base.TargetType.GetDefaultConstructor();
		PropertyInfo[] properties = base.TargetType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		FieldInfo[] fields = base.TargetType.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		readers = new List<ReadElement>(fields.Length + properties.Length);
		PropertyInfo[] array = properties;
		foreach (PropertyInfo propertyInfo in array)
		{
			MethodInfo getMethod = propertyInfo.GetGetMethod(nonPublic: true);
			if ((object)getMethod != null && !(getMethod != getMethod.GetBaseDefinition()))
			{
				ReadElement elementReader = GetElementReader(manager, propertyInfo);
				if (elementReader != null)
				{
					readers.Add(elementReader);
				}
			}
		}
		FieldInfo[] array2 = fields;
		foreach (FieldInfo member in array2)
		{
			ReadElement elementReader2 = GetElementReader(manager, member);
			if (elementReader2 != null)
			{
				readers.Add(elementReader2);
			}
		}
	}

	protected internal override object Read(ContentReader input, object existingInstance)
	{
		T val = ((existingInstance != null) ? ((T)existingInstance) : (((object)constructor != null) ? ((T)constructor.Invoke(null)) : ((T)Activator.CreateInstance(typeof(T)))));
		if (baseTypeReader != null)
		{
			baseTypeReader.Read(input, val);
		}
		object obj = val;
		foreach (ReadElement reader in readers)
		{
			reader(input, obj);
		}
		val = (T)obj;
		return val;
	}

	private static ReadElement GetElementReader(ContentTypeReaderManager manager, MemberInfo member)
	{
		PropertyInfo property = member as PropertyInfo;
		FieldInfo fieldInfo = member as FieldInfo;
		if ((object)property != null)
		{
			if (!property.CanRead)
			{
				return null;
			}
			if (property.GetIndexParameters().Length != 0)
			{
				return null;
			}
		}
		Attribute customAttribute = Attribute.GetCustomAttribute(member, typeof(ContentSerializerIgnoreAttribute));
		if (customAttribute != null)
		{
			return null;
		}
		ContentSerializerAttribute contentSerializerAttribute = Attribute.GetCustomAttribute(member, typeof(ContentSerializerAttribute)) as ContentSerializerAttribute;
		if (contentSerializerAttribute == null)
		{
			if ((object)property != null)
			{
				MethodInfo getMethod = property.GetGetMethod(nonPublic: true);
				if ((object)getMethod != null && !getMethod.IsPublic)
				{
					return null;
				}
				MethodInfo setMethod = property.GetSetMethod(nonPublic: true);
				if ((object)setMethod != null && !setMethod.IsPublic)
				{
					return null;
				}
				if (!property.CanWrite)
				{
					ContentTypeReader typeReader = manager.GetTypeReader(property.PropertyType);
					if (typeReader == null || !typeReader.CanDeserializeIntoExistingObject)
					{
						return null;
					}
				}
			}
			else
			{
				if (!fieldInfo.IsPublic)
				{
					return null;
				}
				if (fieldInfo.IsInitOnly)
				{
					return null;
				}
			}
		}
		Type type;
		Action<object, object> setter;
		if ((object)property != null)
		{
			type = property.PropertyType;
			if (property.CanWrite)
			{
				setter = delegate(object o, object v)
				{
					property.SetValue(o, v, null);
				};
			}
			else
			{
				setter = delegate
				{
				};
			}
		}
		else
		{
			type = fieldInfo.FieldType;
			setter = fieldInfo.SetValue;
		}
		if (contentSerializerAttribute != null && contentSerializerAttribute.SharedResource)
		{
			return delegate(ContentReader input, object parent)
			{
				Action<object> fixup = delegate(object value)
				{
					setter(parent, value);
				};
				input.ReadSharedResource(fixup);
			};
		}
		ContentTypeReader reader = manager.GetTypeReader(type);
		if (reader == null)
		{
			throw new ContentLoadException($"Content reader could not be found for {type.FullName} type.");
		}
		Func<object, object> construct = (object parent) => (object)null;
		if ((object)property != null && !property.CanWrite)
		{
			construct = (object parent) => property.GetValue(parent, null);
		}
		return delegate(ContentReader input, object parent)
		{
			object existingInstance = construct(parent);
			object arg = input.ReadObject(reader, existingInstance);
			setter(parent, arg);
		};
	}
}
