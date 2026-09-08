using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System;

internal sealed class Signature
{
	private RuntimeType[] _arguments;

	private RuntimeType _declaringType;

	private RuntimeType _returnTypeORfieldType;

	private object _keepAlive;

	private unsafe void* _sig;

	private int _csig;

	private int _managedCallingConventionAndArgIteratorFlags;

	private int _nSizeOfArgStack;

	private RuntimeMethodHandleInternal _pMethod;

	internal CallingConventions CallingConvention => (CallingConventions)(_managedCallingConventionAndArgIteratorFlags & 0xFF);

	internal RuntimeType[] Arguments => _arguments;

	internal RuntimeType ReturnType => _returnTypeORfieldType;

	internal RuntimeType FieldType => _returnTypeORfieldType;

	[DllImport("QCall", EntryPoint = "Signature_Init", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "Signature_Init")]
	private unsafe static extern void Init(ObjectHandleOnStack _this, void* pCorSig, int cCorSig, RuntimeFieldHandleInternal fieldHandle, RuntimeMethodHandleInternal methodHandle);

	[MemberNotNull("_returnTypeORfieldType")]
	private unsafe void Init(void* pCorSig, int cCorSig, RuntimeFieldHandleInternal fieldHandle, RuntimeMethodHandleInternal methodHandle)
	{
		Signature o = this;
		Init(ObjectHandleOnStack.Create(ref o), pCorSig, cCorSig, fieldHandle, methodHandle);
	}

	public unsafe Signature(IRuntimeMethodInfo methodHandle, RuntimeType[] arguments, RuntimeType returnType, CallingConventions callingConvention)
	{
		_arguments = arguments;
		_returnTypeORfieldType = returnType;
		_managedCallingConventionAndArgIteratorFlags = (int)callingConvention;
		_pMethod = methodHandle.Value;
		_declaringType = RuntimeMethodHandle.GetDeclaringType(_pMethod);
		Init(null, 0, default(RuntimeFieldHandleInternal), _pMethod);
		GC.KeepAlive(methodHandle);
	}

	public unsafe Signature(IRuntimeMethodInfo methodHandle, RuntimeType declaringType)
	{
		_declaringType = declaringType;
		Init(null, 0, default(RuntimeFieldHandleInternal), methodHandle.Value);
		GC.KeepAlive(methodHandle);
	}

	public unsafe Signature(IRuntimeFieldInfo fieldHandle, RuntimeType declaringType)
	{
		_declaringType = declaringType;
		Init(null, 0, fieldHandle.Value, default(RuntimeMethodHandleInternal));
		GC.KeepAlive(fieldHandle);
	}

	public unsafe Signature(void* pCorSig, int cCorSig, RuntimeType declaringType)
	{
		_declaringType = declaringType;
		Init(pCorSig, cCorSig, default(RuntimeFieldHandleInternal), default(RuntimeMethodHandleInternal));
	}

	[DllImport("QCall", EntryPoint = "Signature_AreEqual", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "Signature_AreEqual")]
	private unsafe static extern Interop.BOOL AreEqual(void* sig1, int csig1, QCallTypeHandle type1, void* sig2, int csig2, QCallTypeHandle type2);

	internal unsafe static bool AreEqual(Signature sig1, Signature sig2)
	{
		return AreEqual(sig1._sig, sig1._csig, new QCallTypeHandle(ref sig1._declaringType), sig2._sig, sig2._csig, new QCallTypeHandle(ref sig2._declaringType)) != Interop.BOOL.FALSE;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern int GetParameterOffsetInternal(void* sig, int csig, int parameterIndex);

	internal unsafe int GetParameterOffset(int parameterIndex)
	{
		int parameterOffsetInternal = GetParameterOffsetInternal(_sig, _csig, parameterIndex);
		if (parameterOffsetInternal < 0)
		{
			Marshal.ThrowExceptionForHR(parameterOffsetInternal, new IntPtr(-1));
		}
		return parameterOffsetInternal;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern int GetTypeParameterOffsetInternal(void* sig, int csig, int offset, int index);

	internal unsafe int GetTypeParameterOffset(int offset, int index)
	{
		if (offset < 0)
		{
			return offset;
		}
		int typeParameterOffsetInternal = GetTypeParameterOffsetInternal(_sig, _csig, offset, index);
		if (typeParameterOffsetInternal < 0 && typeParameterOffsetInternal != -1)
		{
			Marshal.ThrowExceptionForHR(typeParameterOffsetInternal, new IntPtr(-1));
		}
		return typeParameterOffsetInternal;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern int GetCallingConventionFromFunctionPointerAtOffsetInternal(void* sig, int csig, int offset);

	internal unsafe SignatureCallingConvention GetCallingConventionFromFunctionPointerAtOffset(int offset)
	{
		if (offset < 0)
		{
			return SignatureCallingConvention.Default;
		}
		int callingConventionFromFunctionPointerAtOffsetInternal = GetCallingConventionFromFunctionPointerAtOffsetInternal(_sig, _csig, offset);
		if (callingConventionFromFunctionPointerAtOffsetInternal < 0)
		{
			Marshal.ThrowExceptionForHR(callingConventionFromFunctionPointerAtOffsetInternal, new IntPtr(-1));
		}
		return (SignatureCallingConvention)callingConventionFromFunctionPointerAtOffsetInternal;
	}

	internal Type[] GetCustomModifiers(int parameterIndex, bool required)
	{
		return GetCustomModifiersAtOffset(GetParameterOffset(parameterIndex), required);
	}

	[DllImport("QCall", EntryPoint = "Signature_GetCustomModifiersAtOffset", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "Signature_GetCustomModifiersAtOffset")]
	private static extern void GetCustomModifiersAtOffset(ObjectHandleOnStack sigObj, int offset, Interop.BOOL required, ObjectHandleOnStack result);

	internal Type[] GetCustomModifiersAtOffset(int offset, bool required)
	{
		Signature o = this;
		Type[] o2 = null;
		GetCustomModifiersAtOffset(ObjectHandleOnStack.Create(ref o), offset, required ? Interop.BOOL.TRUE : Interop.BOOL.FALSE, ObjectHandleOnStack.Create(ref o2));
		return o2;
	}
}
