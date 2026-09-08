using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.Serialization;

namespace System;

[ClassInterface(ClassInterfaceType.None)]
[ComVisible(true)]
public abstract class Delegate : ICloneable, ISerializable
{
	public struct InvocationListEnumerator<TDelegate> where TDelegate : Delegate
	{
		private readonly MulticastDelegate _delegate;

		private int _index;

		private TDelegate _current;

		public TDelegate Current => _current;

		internal InvocationListEnumerator(MulticastDelegate d)
		{
			_current = null;
			_delegate = d;
			_index = -1;
		}

		public bool MoveNext()
		{
			int index = _index + 1;
			if ((_current = Unsafe.As<TDelegate>(_delegate?.TryGetAt(index))) == null)
			{
				return false;
			}
			_index = index;
			return true;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public InvocationListEnumerator<TDelegate> GetEnumerator()
		{
			return this;
		}
	}

	internal object _target;

	internal object _methodBase;

	internal nint _methodPtr;

	internal nint _methodPtrAux;

	public object? Target => GetTarget();

	public bool HasSingleTarget => Unsafe.As<MulticastDelegate>(this).HasSingleTarget;

	public MethodInfo Method => GetMethodImpl();

	[RequiresUnreferencedCode("The target method might be removed")]
	protected Delegate(object target, string method)
	{
		ArgumentNullException.ThrowIfNull(target, "target");
		ArgumentNullException.ThrowIfNull(method, "method");
		if (!BindToMethodName(target, (RuntimeType)target.GetType(), method, (DelegateBindingFlags)10))
		{
			throw new ArgumentException(SR.Arg_DlgtTargMeth);
		}
	}

	protected Delegate([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllMethods)] Type target, string method)
	{
		ArgumentNullException.ThrowIfNull(target, "target");
		ArgumentNullException.ThrowIfNull(method, "method");
		if (target.ContainsGenericParameters)
		{
			throw new ArgumentException(SR.Arg_UnboundGenParam, "target");
		}
		if (!(target is RuntimeType methodType))
		{
			throw new ArgumentException(SR.Argument_MustBeRuntimeType, "target");
		}
		BindToMethodName(null, methodType, method, (DelegateBindingFlags)37);
	}

	protected virtual object? DynamicInvokeImpl(object?[]? args)
	{
		return ((RuntimeMethodInfo)RuntimeType.GetMethodBase(methodHandle: new RuntimeMethodHandleInternal(GetInvokeMethod()), reflectedType: (RuntimeType)GetType())).Invoke(this, BindingFlags.Default, null, args, null);
	}

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj == null || !InternalEqualTypes(this, obj))
		{
			return false;
		}
		Delegate obj2 = (Delegate)obj;
		if (_target == obj2._target && _methodPtr == obj2._methodPtr && _methodPtrAux == obj2._methodPtrAux)
		{
			return true;
		}
		if (_methodPtrAux == IntPtr.Zero)
		{
			if (obj2._methodPtrAux != IntPtr.Zero)
			{
				return false;
			}
			if (_target != obj2._target)
			{
				return false;
			}
		}
		else
		{
			if (obj2._methodPtrAux == IntPtr.Zero)
			{
				return false;
			}
			if (_methodPtrAux == obj2._methodPtrAux)
			{
				return true;
			}
		}
		if (_methodBase is MethodInfo && obj2._methodBase is MethodInfo)
		{
			return _methodBase.Equals(obj2._methodBase);
		}
		return InternalEqualMethodHandles(this, obj2);
	}

	public override int GetHashCode()
	{
		if (_methodPtrAux == IntPtr.Zero)
		{
			return ((_target != null) ? (RuntimeHelpers.GetHashCode(_target) * 33) : 0) + GetType().GetHashCode();
		}
		return GetType().GetHashCode();
	}

	protected virtual MethodInfo GetMethodImpl()
	{
		if (_methodBase is MethodInfo result)
		{
			return result;
		}
		IRuntimeMethodInfo runtimeMethodInfo = FindMethodHandle();
		RuntimeType runtimeType = RuntimeMethodHandle.GetDeclaringType(runtimeMethodInfo);
		if (runtimeType.IsGenericType && (RuntimeMethodHandle.GetAttributes(runtimeMethodInfo) & MethodAttributes.Static) == 0)
		{
			if (_methodPtrAux == IntPtr.Zero)
			{
				Type genericTypeDefinition = runtimeType.GetGenericTypeDefinition();
				Type type = _target.GetType();
				while (type != null)
				{
					if (type.IsGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition)
					{
						runtimeType = type as RuntimeType;
						break;
					}
					type = type.BaseType;
				}
			}
			else
			{
				runtimeType = (RuntimeType)GetType().GetMethod("Invoke").GetParametersAsSpan()[0].ParameterType;
			}
		}
		_methodBase = (MethodInfo)RuntimeType.GetMethodBase(runtimeType, runtimeMethodInfo);
		return (MethodInfo)_methodBase;
	}

	[RequiresUnreferencedCode("The target method might be removed")]
	public static Delegate? CreateDelegate(Type type, object target, string method, bool ignoreCase, bool throwOnBindFailure)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		ArgumentNullException.ThrowIfNull(target, "target");
		ArgumentNullException.ThrowIfNull(method, "method");
		RuntimeType obj = (type as RuntimeType) ?? throw new ArgumentException(SR.Argument_MustBeRuntimeType, "type");
		if (!obj.IsDelegate())
		{
			throw new ArgumentException(SR.Arg_MustBeDelegate, "type");
		}
		Delegate obj2 = InternalAlloc(obj);
		if (!obj2.BindToMethodName(target, (RuntimeType)target.GetType(), method, (DelegateBindingFlags)(0x1A | (ignoreCase ? 32 : 0))))
		{
			if (throwOnBindFailure)
			{
				throw new ArgumentException(SR.Arg_DlgtTargMeth);
			}
			return null;
		}
		return obj2;
	}

	public static Delegate? CreateDelegate(Type type, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllMethods)] Type target, string method, bool ignoreCase, bool throwOnBindFailure)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		ArgumentNullException.ThrowIfNull(target, "target");
		ArgumentNullException.ThrowIfNull(method, "method");
		if (target.ContainsGenericParameters)
		{
			throw new ArgumentException(SR.Arg_UnboundGenParam, "target");
		}
		RuntimeType obj = (type as RuntimeType) ?? throw new ArgumentException(SR.Argument_MustBeRuntimeType, "type");
		if (!(target is RuntimeType methodType))
		{
			throw new ArgumentException(SR.Argument_MustBeRuntimeType, "target");
		}
		if (!obj.IsDelegate())
		{
			throw new ArgumentException(SR.Arg_MustBeDelegate, "type");
		}
		Delegate obj2 = InternalAlloc(obj);
		if (!obj2.BindToMethodName(null, methodType, method, (DelegateBindingFlags)(5 | (ignoreCase ? 32 : 0))))
		{
			if (throwOnBindFailure)
			{
				throw new ArgumentException(SR.Arg_DlgtTargMeth);
			}
			return null;
		}
		return obj2;
	}

	public static Delegate? CreateDelegate(Type type, MethodInfo method, bool throwOnBindFailure)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		ArgumentNullException.ThrowIfNull(method, "method");
		RuntimeType obj = (type as RuntimeType) ?? throw new ArgumentException(SR.Argument_MustBeRuntimeType, "type");
		if (!(method is RuntimeMethodInfo rtMethod))
		{
			throw new ArgumentException(SR.Argument_MustBeRuntimeMethodInfo, "method");
		}
		if (!obj.IsDelegate())
		{
			throw new ArgumentException(SR.Arg_MustBeDelegate, "type");
		}
		Delegate obj2 = CreateDelegateInternal(obj, rtMethod, null, (DelegateBindingFlags)68);
		if (((object)obj2 == null) & throwOnBindFailure)
		{
			throw new ArgumentException(SR.Arg_DlgtTargMeth);
		}
		return obj2;
	}

	public static Delegate? CreateDelegate(Type type, object? firstArgument, MethodInfo method, bool throwOnBindFailure)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		ArgumentNullException.ThrowIfNull(method, "method");
		RuntimeType obj = (type as RuntimeType) ?? throw new ArgumentException(SR.Argument_MustBeRuntimeType, "type");
		if (!(method is RuntimeMethodInfo rtMethod))
		{
			throw new ArgumentException(SR.Argument_MustBeRuntimeMethodInfo, "method");
		}
		if (!obj.IsDelegate())
		{
			throw new ArgumentException(SR.Arg_MustBeDelegate, "type");
		}
		Delegate obj2 = CreateDelegateInternal(obj, rtMethod, firstArgument, DelegateBindingFlags.RelaxedSignature);
		if (((object)obj2 == null) & throwOnBindFailure)
		{
			throw new ArgumentException(SR.Arg_DlgtTargMeth);
		}
		return obj2;
	}

	internal static Delegate CreateDelegateNoSecurityCheck(Type type, object target, RuntimeMethodHandle method)
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		if (method.IsNullHandle())
		{
			throw new ArgumentNullException("method");
		}
		RuntimeType obj = (type as RuntimeType) ?? throw new ArgumentException(SR.Argument_MustBeRuntimeType, "type");
		if (!obj.IsDelegate())
		{
			throw new ArgumentException(SR.Arg_MustBeDelegate, "type");
		}
		MulticastDelegate multicastDelegate = InternalAlloc(obj);
		if (!multicastDelegate.BindToMethodInfo(target, method.GetMethodInfo(), RuntimeMethodHandle.GetDeclaringType(method.GetMethodInfo()), DelegateBindingFlags.RelaxedSignature))
		{
			throw new ArgumentException(SR.Arg_DlgtTargMeth);
		}
		return multicastDelegate;
	}

	internal static Delegate CreateDelegateInternal(RuntimeType rtType, RuntimeMethodInfo rtMethod, object firstArgument, DelegateBindingFlags flags)
	{
		Delegate obj = InternalAlloc(rtType);
		if (obj.BindToMethodInfo(firstArgument, rtMethod, rtMethod.GetDeclaringTypeInternal(), flags))
		{
			return obj;
		}
		return null;
	}

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2067:ParameterDoesntMeetParameterRequirements", Justification = "The parameter 'methodType' is passed by ref to QCallTypeHandle")]
	private bool BindToMethodName(object target, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllMethods)] RuntimeType methodType, string method, DelegateBindingFlags flags)
	{
		Delegate o = this;
		return BindToMethodName(ObjectHandleOnStack.Create(ref o), ObjectHandleOnStack.Create(ref target), new QCallTypeHandle(ref methodType), method, flags);
	}

	[LibraryImport("QCall", EntryPoint = "Delegate_BindToMethodName", StringMarshalling = StringMarshalling.Utf8)]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private unsafe static bool BindToMethodName(ObjectHandleOnStack d, ObjectHandleOnStack target, QCallTypeHandle methodType, string method, DelegateBindingFlags flags)
	{
		byte* ptr = default(byte*);
		bool flag = false;
		Utf8StringMarshaller.ManagedToUnmanagedIn managedToUnmanagedIn = default(Utf8StringMarshaller.ManagedToUnmanagedIn);
		try
		{
			Span<byte> buffer = stackalloc byte[256];
			managedToUnmanagedIn.FromManaged(method, buffer);
			ptr = managedToUnmanagedIn.ToUnmanaged();
			return __PInvoke(d, target, methodType, ptr, flags) != 0;
		}
		finally
		{
			managedToUnmanagedIn.Free();
		}
		[DllImport("QCall", EntryPoint = "Delegate_BindToMethodName", ExactSpelling = true)]
		unsafe static extern int __PInvoke(ObjectHandleOnStack __d_native, ObjectHandleOnStack __target_native, QCallTypeHandle __methodType_native, byte* __method_native, DelegateBindingFlags __flags_native);
	}

	private bool BindToMethodInfo(object target, IRuntimeMethodInfo method, RuntimeType methodType, DelegateBindingFlags flags)
	{
		Delegate o = this;
		bool result = BindToMethodInfo(ObjectHandleOnStack.Create(ref o), ObjectHandleOnStack.Create(ref target), method.Value, new QCallTypeHandle(ref methodType), flags);
		GC.KeepAlive(method);
		return result;
	}

	[LibraryImport("QCall", EntryPoint = "Delegate_BindToMethodInfo")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static bool BindToMethodInfo(ObjectHandleOnStack d, ObjectHandleOnStack target, RuntimeMethodHandleInternal method, QCallTypeHandle methodType, DelegateBindingFlags flags)
	{
		return __PInvoke(d, target, method, methodType, flags) != 0;
		[DllImport("QCall", EntryPoint = "Delegate_BindToMethodInfo", ExactSpelling = true)]
		static extern int __PInvoke(ObjectHandleOnStack __d_native, ObjectHandleOnStack __target_native, RuntimeMethodHandleInternal __method_native, QCallTypeHandle __methodType_native, DelegateBindingFlags __flags_native);
	}

	private static MulticastDelegate InternalAlloc(RuntimeType type)
	{
		return Unsafe.As<MulticastDelegate>(RuntimeTypeHandle.InternalAlloc(type));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static bool InternalEqualTypes(object a, object b)
	{
		if (a.GetType() == b.GetType())
		{
			return true;
		}
		MethodTable* methodTable = RuntimeHelpers.GetMethodTable(a);
		MethodTable* methodTable2 = RuntimeHelpers.GetMethodTable(b);
		bool result = methodTable->HasTypeEquivalence && methodTable2->HasTypeEquivalence && RuntimeHelpers.AreTypesEquivalent(methodTable, methodTable2);
		GC.KeepAlive(a);
		GC.KeepAlive(b);
		return result;
	}

	private void DelegateConstruct(object target, nint method)
	{
		if (method == IntPtr.Zero)
		{
			throw new ArgumentNullException("method");
		}
		Delegate o = this;
		Construct(ObjectHandleOnStack.Create(ref o), ObjectHandleOnStack.Create(ref target), method);
	}

	[DllImport("QCall", EntryPoint = "Delegate_Construct", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "Delegate_Construct")]
	private static extern void Construct(ObjectHandleOnStack _this, ObjectHandleOnStack target, nint method);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void* GetMulticastInvoke(MethodTable* pMT);

	[DllImport("QCall", EntryPoint = "Delegate_GetMulticastInvokeSlow", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "Delegate_GetMulticastInvokeSlow")]
	private unsafe static extern void* GetMulticastInvokeSlow(MethodTable* pMT);

	internal unsafe nint GetMulticastInvoke()
	{
		MethodTable* methodTable = RuntimeHelpers.GetMethodTable(this);
		void* ptr = GetMulticastInvoke(methodTable);
		if (ptr == null)
		{
			ptr = GetMulticastInvokeSlow(methodTable);
		}
		return (nint)ptr;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void* GetInvokeMethod(MethodTable* pMT);

	internal unsafe nint GetInvokeMethod()
	{
		return (nint)GetInvokeMethod(RuntimeHelpers.GetMethodTable(this));
	}

	internal IRuntimeMethodInfo FindMethodHandle()
	{
		Delegate o = this;
		IRuntimeMethodInfo o2 = null;
		FindMethodHandle(ObjectHandleOnStack.Create(ref o), ObjectHandleOnStack.Create(ref o2));
		return o2;
	}

	[DllImport("QCall", EntryPoint = "Delegate_FindMethodHandle", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "Delegate_FindMethodHandle")]
	private static extern void FindMethodHandle(ObjectHandleOnStack d, ObjectHandleOnStack retMethodInfo);

	private static bool InternalEqualMethodHandles(Delegate left, Delegate right)
	{
		return InternalEqualMethodHandles(ObjectHandleOnStack.Create(ref left), ObjectHandleOnStack.Create(ref right));
	}

	[LibraryImport("QCall", EntryPoint = "Delegate_InternalEqualMethodHandles")]
	[GeneratedCode("Microsoft.Interop.LibraryImportGenerator", "10.0.14.37416")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static bool InternalEqualMethodHandles(ObjectHandleOnStack left, ObjectHandleOnStack right)
	{
		return __PInvoke(left, right) != 0;
		[DllImport("QCall", EntryPoint = "Delegate_InternalEqualMethodHandles", ExactSpelling = true)]
		static extern int __PInvoke(ObjectHandleOnStack __left_native, ObjectHandleOnStack __right_native);
	}

	internal static nint AdjustTarget(object target, nint methodPtr)
	{
		return AdjustTarget(ObjectHandleOnStack.Create(ref target), methodPtr);
	}

	[DllImport("QCall", EntryPoint = "Delegate_AdjustTarget", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "Delegate_AdjustTarget")]
	private static extern nint AdjustTarget(ObjectHandleOnStack target, nint methodPtr);

	internal void InitializeVirtualCallStub(nint methodPtr)
	{
		Delegate o = this;
		InitializeVirtualCallStub(ObjectHandleOnStack.Create(ref o), methodPtr);
	}

	[DllImport("QCall", EntryPoint = "Delegate_InitializeVirtualCallStub", ExactSpelling = true)]
	[LibraryImport("QCall", EntryPoint = "Delegate_InitializeVirtualCallStub")]
	private static extern void InitializeVirtualCallStub(ObjectHandleOnStack d, nint methodPtr);

	internal virtual object GetTarget()
	{
		if (_methodPtrAux != IntPtr.Zero)
		{
			return null;
		}
		return _target;
	}

	public virtual object Clone()
	{
		return MemberwiseClone();
	}

	[return: NotNullIfNotNull("a")]
	[return: NotNullIfNotNull("b")]
	public static Delegate? Combine(Delegate? a, Delegate? b)
	{
		if ((object)a == null)
		{
			return b;
		}
		return a.CombineImpl(b);
	}

	public static Delegate? Combine(params Delegate?[]? delegates)
	{
		return Combine((ReadOnlySpan<Delegate?>)delegates);
	}

	public static Delegate? Combine(params ReadOnlySpan<Delegate?> delegates)
	{
		Delegate obj = null;
		if (!delegates.IsEmpty)
		{
			obj = delegates[0];
			for (int i = 1; i < delegates.Length; i++)
			{
				obj = Combine(obj, delegates[i]);
			}
		}
		return obj;
	}

	public static Delegate CreateDelegate(Type type, object? firstArgument, MethodInfo method)
	{
		return CreateDelegate(type, firstArgument, method, throwOnBindFailure: true);
	}

	public static Delegate CreateDelegate(Type type, MethodInfo method)
	{
		return CreateDelegate(type, method, throwOnBindFailure: true);
	}

	[RequiresUnreferencedCode("The target method might be removed")]
	public static Delegate CreateDelegate(Type type, object target, string method)
	{
		return CreateDelegate(type, target, method, ignoreCase: false, throwOnBindFailure: true);
	}

	[RequiresUnreferencedCode("The target method might be removed")]
	public static Delegate CreateDelegate(Type type, object target, string method, bool ignoreCase)
	{
		return CreateDelegate(type, target, method, ignoreCase, throwOnBindFailure: true);
	}

	public static Delegate CreateDelegate(Type type, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllMethods)] Type target, string method)
	{
		return CreateDelegate(type, target, method, ignoreCase: false, throwOnBindFailure: true);
	}

	public static Delegate CreateDelegate(Type type, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.AllMethods)] Type target, string method, bool ignoreCase)
	{
		return CreateDelegate(type, target, method, ignoreCase, throwOnBindFailure: true);
	}

	protected virtual Delegate CombineImpl(Delegate? d)
	{
		throw new MulticastNotSupportedException(SR.Multicast_Combine);
	}

	protected virtual Delegate? RemoveImpl(Delegate d)
	{
		if (!d.Equals(this))
		{
			return this;
		}
		return null;
	}

	public virtual Delegate[] GetInvocationList()
	{
		return new Delegate[1] { this };
	}

	public static InvocationListEnumerator<TDelegate> EnumerateInvocationList<TDelegate>(TDelegate? d) where TDelegate : Delegate
	{
		return new InvocationListEnumerator<TDelegate>(Unsafe.As<MulticastDelegate>(d));
	}

	public object? DynamicInvoke(params object?[]? args)
	{
		return DynamicInvokeImpl(args);
	}

	[Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		throw new PlatformNotSupportedException();
	}

	public static Delegate? Remove(Delegate? source, Delegate? value)
	{
		if ((object)source == null)
		{
			return null;
		}
		if ((object)value == null)
		{
			return source;
		}
		if (!InternalEqualTypes(source, value))
		{
			throw new ArgumentException(SR.Arg_DlgtTypeMis);
		}
		return source.RemoveImpl(value);
	}

	public static Delegate? RemoveAll(Delegate? source, Delegate? value)
	{
		Delegate obj;
		do
		{
			obj = source;
			source = Remove(source, value);
		}
		while (obj != source);
		return obj;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(Delegate? d1, Delegate? d2)
	{
		if ((object)d2 == null)
		{
			return (object)d1 == null;
		}
		if ((object)d2 != d1)
		{
			return d2.Equals(d1);
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(Delegate? d1, Delegate? d2)
	{
		if ((object)d2 == null)
		{
			return (object)d1 != null;
		}
		if ((object)d2 != d1)
		{
			return !d2.Equals(d1);
		}
		return false;
	}
}
