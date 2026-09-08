using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.Reflection;

internal sealed class FieldAccessor
{
	private enum FieldAccessorType
	{
		InstanceReferenceType,
		InstanceValueType,
		InstanceValueTypeSize1,
		InstanceValueTypeSize2,
		InstanceValueTypeSize4,
		InstanceValueTypeSize8,
		InstancePointerType,
		StaticReferenceType,
		StaticValueType,
		StaticValueTypeSize1,
		StaticValueTypeSize2,
		StaticValueTypeSize4,
		StaticValueTypeSize8,
		StaticValueTypeBoxed,
		StaticPointerType,
		SlowPathUntilClassInitialized,
		SlowPath,
		NoInvoke
	}

	private readonly RtFieldInfo _fieldInfo;

	private nint _addressOrOffset;

	private unsafe MethodTable* _methodTable;

	private volatile FieldAccessorType _fieldAccessType;

	internal FieldAccessor(FieldInfo fieldInfo)
	{
		_fieldInfo = (RtFieldInfo)fieldInfo;
		if (_fieldInfo.m_declaringType.ContainsGenericParameters || _fieldInfo.m_declaringType.IsNullableOfT)
		{
			_fieldAccessType = FieldAccessorType.NoInvoke;
		}
		else
		{
			_fieldAccessType = FieldAccessorType.SlowPathUntilClassInitialized;
		}
	}

	private unsafe void Initialize()
	{
		if (!RuntimeFieldHandle.IsFastPathSupported(_fieldInfo))
		{
			_fieldAccessType = FieldAccessorType.SlowPath;
			return;
		}
		RuntimeType runtimeType = (RuntimeType)_fieldInfo.FieldType;
		if (_fieldInfo.IsStatic)
		{
			_addressOrOffset = RuntimeFieldHandle.GetStaticFieldAddress(_fieldInfo);
			if ((_fieldInfo.Attributes & FieldAttributes.HasFieldRVA) != FieldAttributes.PrivateScope)
			{
				_methodTable = (MethodTable*)runtimeType.TypeHandle.Value;
				_fieldAccessType = FieldAccessorType.StaticValueType;
			}
			else if (runtimeType.IsValueType)
			{
				if (runtimeType.IsEnum)
				{
					_methodTable = (MethodTable*)runtimeType.TypeHandle.Value;
					_fieldAccessType = GetPrimitiveAccessorTypeForStatic(runtimeType.GetEnumUnderlyingType());
				}
				else if (runtimeType.GetCorElementType() == CorElementType.ELEMENT_TYPE_VALUETYPE)
				{
					_methodTable = (MethodTable*)runtimeType.TypeHandle.Value;
					_fieldAccessType = FieldAccessorType.StaticValueTypeBoxed;
				}
				else
				{
					_methodTable = (MethodTable*)runtimeType.TypeHandle.Value;
					_fieldAccessType = GetPrimitiveAccessorTypeForStatic(runtimeType);
				}
			}
			else if (runtimeType.IsPointer)
			{
				_fieldAccessType = FieldAccessorType.StaticPointerType;
			}
			else if (runtimeType.IsFunctionPointer)
			{
				_methodTable = (MethodTable*)typeof(nint).TypeHandle.Value;
				_fieldAccessType = FieldAccessorType.StaticValueTypeSize8;
			}
			else
			{
				_fieldAccessType = FieldAccessorType.StaticReferenceType;
			}
		}
		else
		{
			_addressOrOffset = RuntimeFieldHandle.GetInstanceFieldOffset(_fieldInfo);
			if (runtimeType.IsEnum)
			{
				_methodTable = (MethodTable*)runtimeType.TypeHandle.Value;
				_fieldAccessType = GetPrimitiveAccessorTypeForInstance(runtimeType.GetEnumUnderlyingType());
			}
			else if (runtimeType.IsValueType)
			{
				_methodTable = (MethodTable*)runtimeType.TypeHandle.Value;
				_fieldAccessType = GetPrimitiveAccessorTypeForInstance(runtimeType);
			}
			else if (runtimeType.IsPointer)
			{
				_fieldAccessType = FieldAccessorType.InstancePointerType;
			}
			else if (runtimeType.IsFunctionPointer)
			{
				_methodTable = (MethodTable*)typeof(nint).TypeHandle.Value;
				_fieldAccessType = FieldAccessorType.InstanceValueTypeSize8;
			}
			else
			{
				_fieldAccessType = FieldAccessorType.InstanceReferenceType;
			}
		}
	}

	public unsafe object GetValue(object obj)
	{
		switch (_fieldAccessType)
		{
		case FieldAccessorType.InstanceReferenceType:
			VerifyTarget(obj);
			return Volatile.Read(in Unsafe.As<byte, object>(ref Unsafe.AddByteOffset(ref obj.GetRawData(), _addressOrOffset)));
		case FieldAccessorType.InstanceValueType:
		case FieldAccessorType.InstanceValueTypeSize1:
		case FieldAccessorType.InstanceValueTypeSize2:
		case FieldAccessorType.InstanceValueTypeSize4:
		case FieldAccessorType.InstanceValueTypeSize8:
			VerifyTarget(obj);
			return RuntimeHelpers.Box(_methodTable, ref Unsafe.AddByteOffset(ref obj.GetRawData(), _addressOrOffset));
		case FieldAccessorType.InstancePointerType:
			VerifyTarget(obj);
			return Pointer.Box((void*)Unsafe.As<byte, nint>(ref Unsafe.AddByteOffset(ref obj.GetRawData(), _addressOrOffset)), _fieldInfo.FieldType);
		case FieldAccessorType.StaticReferenceType:
			return Volatile.Read(in Unsafe.As<nint, object>(ref *(nint*)_addressOrOffset));
		case FieldAccessorType.StaticValueType:
		case FieldAccessorType.StaticValueTypeSize1:
		case FieldAccessorType.StaticValueTypeSize2:
		case FieldAccessorType.StaticValueTypeSize4:
		case FieldAccessorType.StaticValueTypeSize8:
			return RuntimeHelpers.Box(_methodTable, ref Unsafe.AsRef<byte>(((IntPtr)_addressOrOffset).ToPointer()));
		case FieldAccessorType.StaticValueTypeBoxed:
			return RuntimeHelpers.Box(_methodTable, ref Unsafe.As<nint, object>(ref *(nint*)_addressOrOffset).GetRawData());
		case FieldAccessorType.StaticPointerType:
			return Pointer.Box((void*)Unsafe.As<byte, nint>(ref Unsafe.AsRef<byte>(((IntPtr)_addressOrOffset).ToPointer())), _fieldInfo.FieldType);
		case FieldAccessorType.SlowPathUntilClassInitialized:
		{
			if (!IsStatic())
			{
				VerifyTarget(obj);
			}
			bool isClassInitialized = false;
			object value = RuntimeFieldHandle.GetValue(_fieldInfo, obj, (RuntimeType)_fieldInfo.FieldType, _fieldInfo.m_declaringType, ref isClassInitialized);
			if (isClassInitialized)
			{
				Initialize();
			}
			return value;
		}
		case FieldAccessorType.SlowPath:
		{
			if (!IsStatic())
			{
				VerifyTarget(obj);
			}
			bool isClassInitialized = true;
			return RuntimeFieldHandle.GetValue(_fieldInfo, obj, (RuntimeType)_fieldInfo.FieldType, _fieldInfo.m_declaringType, ref isClassInitialized);
		}
		case FieldAccessorType.NoInvoke:
			if ((object)_fieldInfo.DeclaringType != null && _fieldInfo.DeclaringType.ContainsGenericParameters)
			{
				throw new InvalidOperationException(SR.Arg_UnboundGenField);
			}
			if ((object)_fieldInfo.DeclaringType != null && ((RuntimeType)_fieldInfo.FieldType).IsNullableOfT)
			{
				throw new NotSupportedException();
			}
			throw new FieldAccessException();
		default:
			return null;
		}
	}

	public unsafe void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture)
	{
		switch (_fieldAccessType)
		{
		case FieldAccessorType.InstanceReferenceType:
			VerifyInstanceField(obj, ref value, invokeAttr, binder, culture);
			Volatile.Write(ref Unsafe.As<byte, object>(ref Unsafe.AddByteOffset(ref obj.GetRawData(), _addressOrOffset)), value);
			break;
		case FieldAccessorType.InstanceValueTypeSize1:
			VerifyInstanceField(obj, ref value, invokeAttr, binder, culture);
			Volatile.Write(ref Unsafe.AddByteOffset(ref obj.GetRawData(), _addressOrOffset), value.GetRawData());
			break;
		case FieldAccessorType.InstanceValueTypeSize2:
			VerifyInstanceField(obj, ref value, invokeAttr, binder, culture);
			Volatile.Write(ref Unsafe.As<byte, short>(ref Unsafe.AddByteOffset(ref obj.GetRawData(), _addressOrOffset)), Unsafe.As<byte, short>(ref value.GetRawData()));
			break;
		case FieldAccessorType.InstanceValueTypeSize4:
			VerifyInstanceField(obj, ref value, invokeAttr, binder, culture);
			Volatile.Write(ref Unsafe.As<byte, int>(ref Unsafe.AddByteOffset(ref obj.GetRawData(), _addressOrOffset)), Unsafe.As<byte, int>(ref value.GetRawData()));
			break;
		case FieldAccessorType.InstanceValueTypeSize8:
			VerifyInstanceField(obj, ref value, invokeAttr, binder, culture);
			Volatile.Write(ref Unsafe.As<byte, long>(ref Unsafe.AddByteOffset(ref obj.GetRawData(), _addressOrOffset)), Unsafe.As<byte, long>(ref value.GetRawData()));
			break;
		case FieldAccessorType.StaticReferenceType:
			VerifyStaticField(ref value, invokeAttr, binder, culture);
			Volatile.Write(ref Unsafe.As<nint, object>(ref *(nint*)_addressOrOffset), value);
			break;
		case FieldAccessorType.StaticValueTypeSize1:
			VerifyStaticField(ref value, invokeAttr, binder, culture);
			Volatile.Write(ref Unsafe.AsRef<byte>(((IntPtr)_addressOrOffset).ToPointer()), value.GetRawData());
			break;
		case FieldAccessorType.StaticValueTypeSize2:
			VerifyStaticField(ref value, invokeAttr, binder, culture);
			Volatile.Write(ref Unsafe.AsRef<short>(((IntPtr)_addressOrOffset).ToPointer()), Unsafe.As<byte, short>(ref value.GetRawData()));
			break;
		case FieldAccessorType.StaticValueTypeSize4:
			VerifyStaticField(ref value, invokeAttr, binder, culture);
			Volatile.Write(ref Unsafe.AsRef<int>(((IntPtr)_addressOrOffset).ToPointer()), Unsafe.As<byte, int>(ref value.GetRawData()));
			break;
		case FieldAccessorType.StaticValueTypeSize8:
			VerifyStaticField(ref value, invokeAttr, binder, culture);
			Volatile.Write(ref Unsafe.AsRef<long>(((IntPtr)_addressOrOffset).ToPointer()), Unsafe.As<byte, long>(ref value.GetRawData()));
			break;
		case FieldAccessorType.SlowPathUntilClassInitialized:
		{
			if (IsStatic())
			{
				VerifyStaticField(ref value, invokeAttr, binder, culture);
			}
			else
			{
				VerifyInstanceField(obj, ref value, invokeAttr, binder, culture);
			}
			bool isClassInitialized = false;
			RuntimeFieldHandle.SetValue(_fieldInfo, obj, value, (RuntimeType)_fieldInfo.FieldType, _fieldInfo.m_declaringType, ref isClassInitialized);
			if (isClassInitialized)
			{
				Initialize();
			}
			break;
		}
		case FieldAccessorType.NoInvoke:
			if ((object)_fieldInfo.DeclaringType != null && _fieldInfo.DeclaringType.ContainsGenericParameters)
			{
				throw new InvalidOperationException(SR.Arg_UnboundGenField);
			}
			throw new FieldAccessException();
		default:
		{
			if (IsStatic())
			{
				VerifyStaticField(ref value, invokeAttr, binder, culture);
			}
			else
			{
				VerifyInstanceField(obj, ref value, invokeAttr, binder, culture);
			}
			bool isClassInitialized = true;
			RuntimeFieldHandle.SetValue(_fieldInfo, obj, value, (RuntimeType)_fieldInfo.FieldType, _fieldInfo.m_declaringType, ref isClassInitialized);
			break;
		}
		}
	}

	private bool IsStatic()
	{
		return (_fieldInfo.Attributes & FieldAttributes.Static) == FieldAttributes.Static;
	}

	private void VerifyStaticField(ref object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture)
	{
		VerifyInitOnly();
		CheckValue(ref value, invokeAttr, binder, culture);
	}

	private void VerifyInstanceField(object obj, ref object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture)
	{
		VerifyTarget(obj);
		CheckValue(ref value, invokeAttr, binder, culture);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void VerifyTarget(object target)
	{
		if (!_fieldInfo.m_declaringType.IsInstanceOfType(target))
		{
			if (target == null)
			{
				ThrowHelperTargetException();
			}
			else
			{
				ThrowHelperArgumentException(target, _fieldInfo);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CheckValue(ref object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture)
	{
		if (value == null)
		{
			if (((RuntimeType)_fieldInfo.FieldType).IsActualValueType)
			{
				((RuntimeType)_fieldInfo.FieldType).CheckValue(ref value, binder, culture, invokeAttr);
			}
		}
		else if ((object)value.GetType() != _fieldInfo.FieldType)
		{
			((RuntimeType)_fieldInfo.FieldType).CheckValue(ref value, binder, culture, invokeAttr);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void VerifyInitOnly()
	{
		if ((_fieldInfo.Attributes & FieldAttributes.InitOnly) == FieldAttributes.InitOnly && _fieldAccessType != FieldAccessorType.SlowPathUntilClassInitialized)
		{
			ThrowHelperFieldAccessException(_fieldInfo);
		}
	}

	private static FieldAccessorType GetPrimitiveAccessorTypeForInstance(Type fieldType)
	{
		FieldAccessorType result = FieldAccessorType.InstanceValueType;
		if (fieldType == typeof(byte) || fieldType == typeof(sbyte) || fieldType == typeof(bool))
		{
			result = FieldAccessorType.InstanceValueTypeSize1;
		}
		else if (fieldType == typeof(short) || fieldType == typeof(ushort) || fieldType == typeof(char))
		{
			result = FieldAccessorType.InstanceValueTypeSize2;
		}
		else if (fieldType == typeof(int) || fieldType == typeof(uint) || fieldType == typeof(float))
		{
			result = FieldAccessorType.InstanceValueTypeSize4;
		}
		else if (fieldType == typeof(long) || fieldType == typeof(ulong) || fieldType == typeof(double))
		{
			result = FieldAccessorType.InstanceValueTypeSize8;
		}
		else if (fieldType == typeof(nint) || fieldType == typeof(nuint))
		{
			result = FieldAccessorType.InstanceValueTypeSize8;
		}
		return result;
	}

	private static FieldAccessorType GetPrimitiveAccessorTypeForStatic(Type fieldType)
	{
		FieldAccessorType result = FieldAccessorType.StaticValueType;
		if (fieldType == typeof(byte) || fieldType == typeof(sbyte) || fieldType == typeof(bool))
		{
			result = FieldAccessorType.StaticValueTypeSize1;
		}
		else if (fieldType == typeof(short) || fieldType == typeof(ushort) || fieldType == typeof(char))
		{
			result = FieldAccessorType.StaticValueTypeSize2;
		}
		else if (fieldType == typeof(int) || fieldType == typeof(uint) || fieldType == typeof(float))
		{
			result = FieldAccessorType.StaticValueTypeSize4;
		}
		else if (fieldType == typeof(long) || fieldType == typeof(ulong) || fieldType == typeof(double))
		{
			result = FieldAccessorType.StaticValueTypeSize8;
		}
		else if (fieldType == typeof(nint) || fieldType == typeof(nuint))
		{
			result = FieldAccessorType.StaticValueTypeSize8;
		}
		return result;
	}

	private static void ThrowHelperTargetException()
	{
		throw new TargetException(SR.RFLCT_Targ_StatFldReqTarg);
	}

	private static void ThrowHelperArgumentException(object target, FieldInfo fieldInfo)
	{
		throw new ArgumentException(SR.Format(SR.Arg_FieldDeclTarget, fieldInfo.Name, fieldInfo.DeclaringType, target.GetType()));
	}

	private static void ThrowHelperFieldAccessException(FieldInfo fieldInfo)
	{
		throw new FieldAccessException(SR.Format(SR.RFLCT_CannotSetInitonlyStaticField, fieldInfo.Name, fieldInfo.DeclaringType));
	}
}
