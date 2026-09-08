using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.DataContracts;

namespace System.Runtime.Serialization;

internal static class FastInvokerBuilder
{
	public delegate void Setter(ref object obj, object value);

	public delegate object Getter(object obj);

	private delegate void StructSetDelegate<T, TArg>(ref T obj, TArg value);

	private delegate TResult StructGetDelegate<T, out TResult>(ref T obj);

	private static readonly MethodInfo s_createGetterInternal = typeof(FastInvokerBuilder).GetMethod("CreateGetterInternal", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

	private static readonly MethodInfo s_createSetterInternal = typeof(FastInvokerBuilder).GetMethod("CreateSetterInternal", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

	private static readonly MethodInfo s_make = typeof(FastInvokerBuilder).GetMethod("Make", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

	[RequiresDynamicCode("Data Contract Serialization and Deserialization might require types that cannot be statically analyzed.")]
	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2060:MakeGenericMethod", Justification = "The call to MakeGenericMethod is safe due to the fact that we are preserving the constructors of type which is what Make() is doing.")]
	public static Func<object> GetMakeNewInstanceFunc([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type type)
	{
		return s_make.MakeGenericMethod(type).CreateDelegate<Func<object>>();
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2060:MakeGenericMethod", Justification = "The call to MakeGenericMethod is safe due to the fact that FastInvokerBuilder.CreateGetterInternal<T, T1> is not annotated.")]
	public static Getter CreateGetter(MemberInfo memberInfo)
	{
		if (memberInfo is PropertyInfo propertyInfo)
		{
			Type declaringType = propertyInfo.DeclaringType;
			Type propertyType = propertyInfo.PropertyType;
			if (declaringType.IsGenericType && declaringType.GetGenericTypeDefinition() == typeof(KeyValue<, >))
			{
				if (propertyInfo.Name == "Key")
				{
					return (object obj) => ((IKeyValue)obj).Key;
				}
				return (object obj) => ((IKeyValue)obj).Value;
			}
			if (RuntimeFeature.IsDynamicCodeSupported || (!declaringType.IsValueType && !propertyType.IsValueType))
			{
				return s_createGetterInternal.MakeGenericMethod(declaringType, propertyType).CreateDelegate<Func<PropertyInfo, Getter>>()(propertyInfo);
			}
			return propertyInfo.GetValue;
		}
		FieldInfo fieldInfo = memberInfo as FieldInfo;
		if ((object)fieldInfo != null)
		{
			return (object obj) => fieldInfo.GetValue(obj);
		}
		throw new InvalidOperationException(System.SR.Format(System.SR.InvalidMember, DataContract.GetClrTypeFullName(memberInfo.DeclaringType), memberInfo.Name));
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2060:MakeGenericMethod", Justification = "The call to MakeGenericMethod is safe due to the fact that FastInvokerBuilder.CreateSetterInternal<T, T1> is not annotated.")]
	public static Setter CreateSetter(MemberInfo memberInfo)
	{
		PropertyInfo propInfo = memberInfo as PropertyInfo;
		if ((object)propInfo != null)
		{
			if (propInfo.CanWrite)
			{
				Type declaringType = propInfo.DeclaringType;
				Type propertyType = propInfo.PropertyType;
				if (declaringType.IsGenericType && declaringType.GetGenericTypeDefinition() == typeof(KeyValue<, >))
				{
					if (propInfo.Name == "Key")
					{
						return delegate(ref object obj, object val)
						{
							((IKeyValue)obj).Key = val;
						};
					}
					return delegate(ref object obj, object val)
					{
						((IKeyValue)obj).Value = val;
					};
				}
				if (RuntimeFeature.IsDynamicCodeSupported || (!declaringType.IsValueType && !propertyType.IsValueType))
				{
					return s_createSetterInternal.MakeGenericMethod(propInfo.DeclaringType, propInfo.PropertyType).CreateDelegate<Func<PropertyInfo, Setter>>()(propInfo);
				}
				return delegate(ref object obj, object val)
				{
					propInfo.SetValue(obj, val);
				};
			}
			throw new InvalidOperationException(System.SR.Format(System.SR.NoSetMethodForProperty, propInfo.DeclaringType, propInfo.Name));
		}
		FieldInfo fieldInfo = memberInfo as FieldInfo;
		if ((object)fieldInfo != null)
		{
			return delegate(ref object obj, object val)
			{
				fieldInfo.SetValue(obj, val);
			};
		}
		throw new InvalidOperationException(System.SR.Format(System.SR.InvalidMember, DataContract.GetClrTypeFullName(memberInfo.DeclaringType), memberInfo.Name));
	}

	private static object Make<T>() where T : new()
	{
		return new T();
	}

	private static Getter CreateGetterInternal<DeclaringType, PropertyType>(PropertyInfo propInfo)
	{
		if (typeof(DeclaringType).IsValueType)
		{
			StructGetDelegate<DeclaringType, PropertyType> getMethod = propInfo.GetMethod.CreateDelegate<StructGetDelegate<DeclaringType, PropertyType>>();
			return delegate(object obj)
			{
				DeclaringType obj2 = (DeclaringType)obj;
				return getMethod(ref obj2);
			};
		}
		Func<DeclaringType, PropertyType> getMethod2 = propInfo.GetMethod.CreateDelegate<Func<DeclaringType, PropertyType>>();
		return (object obj) => getMethod2((DeclaringType)obj);
	}

	private static Setter CreateSetterInternal<DeclaringType, PropertyType>(PropertyInfo propInfo)
	{
		if (typeof(DeclaringType).IsValueType)
		{
			StructSetDelegate<DeclaringType, PropertyType> setMethod = propInfo.SetMethod.CreateDelegate<StructSetDelegate<DeclaringType, PropertyType>>();
			return delegate(ref object obj, object val)
			{
				DeclaringType obj2 = (DeclaringType)obj;
				setMethod(ref obj2, (PropertyType)val);
				obj = obj2;
			};
		}
		Action<DeclaringType, PropertyType> setMethod2 = propInfo.SetMethod.CreateDelegate<Action<DeclaringType, PropertyType>>();
		return delegate(ref object obj, object val)
		{
			setMethod2((DeclaringType)obj, (PropertyType)val);
		};
	}
}
