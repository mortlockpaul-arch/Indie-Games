using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Runtime.Versioning;

namespace Internal.Runtime.InteropServices;

[SupportedOSPlatform("windows")]
internal static class ComActivator
{
	[ComVisible(true)]
	[RequiresUnreferencedCode("Built-in COM support is not trim compatible", Url = "https://aka.ms/dotnet-illink/com")]
	private sealed class BasicClassFactory : IClassFactory
	{
		public enum ValidatedInterfaceKind
		{
			IUnknown,
			IDispatch,
			ManagedType
		}

		public struct ValidatedInterfaceType
		{
			public ValidatedInterfaceKind Kind { get; init; }

			public Type ManagedType { get; init; }
		}

		private readonly Guid _classId;

		[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
		private readonly Type _classType;

		public BasicClassFactory(Guid clsid, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] Type classType)
		{
			_classId = clsid;
			_classType = classType;
		}

		public static ValidatedInterfaceType CreateValidatedInterfaceType([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] Type classType, ref Guid riid, object outer)
		{
			if (riid == Marshal.IID_IUnknown)
			{
				return new ValidatedInterfaceType
				{
					Kind = ValidatedInterfaceKind.IUnknown,
					ManagedType = null
				};
			}
			if (riid == Marshal.IID_IDispatch)
			{
				ClassInterfaceAttribute classInterfaceAttribute = classType.GetCustomAttribute<ClassInterfaceAttribute>() ?? classType.Assembly.GetCustomAttribute<ClassInterfaceAttribute>();
				bool flag = classInterfaceAttribute == null;
				if (!flag)
				{
					ClassInterfaceType value = classInterfaceAttribute.Value;
					bool flag2 = (uint)(value - 1) <= 1u;
					flag = flag2;
				}
				if (flag)
				{
					return new ValidatedInterfaceType
					{
						Kind = ValidatedInterfaceKind.IDispatch,
						ManagedType = null
					};
				}
			}
			if (outer != null)
			{
				throw new COMException(string.Empty, -2147221232);
			}
			Type[] interfaces = classType.GetInterfaces();
			foreach (Type type in interfaces)
			{
				if (type.GUID == riid)
				{
					return new ValidatedInterfaceType
					{
						Kind = ValidatedInterfaceKind.ManagedType,
						ManagedType = type
					};
				}
			}
			throw new InvalidCastException();
		}

		public static nint GetObjectAsInterface(object obj, ValidatedInterfaceType interfaceType)
		{
			if (interfaceType.Kind == ValidatedInterfaceKind.IUnknown)
			{
				return Marshal.GetIUnknownForObject(obj);
			}
			if (interfaceType.Kind == ValidatedInterfaceKind.IDispatch)
			{
				return Marshal.GetIDispatchForObject(obj);
			}
			nint comInterfaceForObject = Marshal.GetComInterfaceForObject(obj, interfaceType.ManagedType, CustomQueryInterfaceMode.Ignore);
			if (comInterfaceForObject == IntPtr.Zero)
			{
				throw new InvalidCastException();
			}
			return comInterfaceForObject;
		}

		public static object CreateAggregatedObject(object pUnkOuter, object comObject)
		{
			nint iUnknownForObject = Marshal.GetIUnknownForObject(pUnkOuter);
			try
			{
				return Marshal.GetObjectForIUnknown(Marshal.CreateAggregatedObject(iUnknownForObject, comObject));
			}
			finally
			{
				Marshal.Release(iUnknownForObject);
			}
		}

		public void CreateInstance([MarshalAs(UnmanagedType.Interface)] object pUnkOuter, ref Guid riid, out nint ppvObject)
		{
			ValidatedInterfaceType interfaceType = CreateValidatedInterfaceType(_classType, ref riid, pUnkOuter);
			object obj = Activator.CreateInstance(_classType);
			if (pUnkOuter != null)
			{
				obj = CreateAggregatedObject(pUnkOuter, obj);
			}
			ppvObject = GetObjectAsInterface(obj, interfaceType);
		}

		public void LockServer([MarshalAs(UnmanagedType.Bool)] bool fLock)
		{
		}
	}

	[ComVisible(true)]
	[RequiresUnreferencedCode("Built-in COM support is not trim compatible", Url = "https://aka.ms/dotnet-illink/com")]
	private sealed class LicenseClassFactory : IClassFactory2, IClassFactory
	{
		private readonly Guid _classId;

		[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.Interfaces)]
		private readonly Type _classType;

		public LicenseClassFactory(Guid clsid, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.Interfaces)] Type classType)
		{
			_classId = clsid;
			_classType = classType;
		}

		public void CreateInstance([MarshalAs(UnmanagedType.Interface)] object pUnkOuter, ref Guid riid, out nint ppvObject)
		{
			CreateInstanceInner(pUnkOuter, ref riid, null, isDesignTime: true, out ppvObject);
		}

		public void LockServer([MarshalAs(UnmanagedType.Bool)] bool fLock)
		{
		}

		public void GetLicInfo(ref LICINFO licInfo)
		{
			LicenseInteropProxy.GetLicInfo(_classType, out var runtimeKeyAvail, out var licVerified);
			licInfo.cbLicInfo = 12;
			licInfo.fRuntimeKeyAvail = runtimeKeyAvail;
			licInfo.fLicVerified = licVerified;
		}

		public void RequestLicKey(int dwReserved, [MarshalAs(UnmanagedType.BStr)] out string pBstrKey)
		{
			pBstrKey = LicenseInteropProxy.RequestLicKey(_classType);
		}

		public void CreateInstanceLic([MarshalAs(UnmanagedType.Interface)] object pUnkOuter, [MarshalAs(UnmanagedType.Interface)] object pUnkReserved, ref Guid riid, [MarshalAs(UnmanagedType.BStr)] string bstrKey, out nint ppvObject)
		{
			CreateInstanceInner(pUnkOuter, ref riid, bstrKey, isDesignTime: false, out ppvObject);
		}

		private void CreateInstanceInner(object pUnkOuter, ref Guid riid, string key, bool isDesignTime, out nint ppvObject)
		{
			BasicClassFactory.ValidatedInterfaceType interfaceType = BasicClassFactory.CreateValidatedInterfaceType(_classType, ref riid, pUnkOuter);
			object obj = LicenseInteropProxy.AllocateAndValidateLicense(_classType, key, isDesignTime);
			if (pUnkOuter != null)
			{
				obj = BasicClassFactory.CreateAggregatedObject(pUnkOuter, obj);
			}
			ppvObject = BasicClassFactory.GetObjectAsInterface(obj, interfaceType);
		}
	}

	private static readonly Dictionary<string, AssemblyLoadContext> s_assemblyLoadContexts = new Dictionary<string, AssemblyLoadContext>(StringComparer.InvariantCultureIgnoreCase);

	private static readonly HashSet<string> s_loadedInDefaultContext = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

	[RequiresUnreferencedCode("Built-in COM support is not trim compatible", Url = "https://aka.ms/dotnet-illink/com")]
	private static object GetClassFactoryForType(ComActivationContext cxt)
	{
		if (!Marshal.IsBuiltInComSupported)
		{
			throw new NotSupportedException(SR.NotSupported_COM);
		}
		if (cxt.InterfaceId != Marshal.IID_IUnknown && cxt.InterfaceId != typeof(IClassFactory).GUID && cxt.InterfaceId != typeof(IClassFactory2).GUID)
		{
			throw new NotSupportedException();
		}
		if (!Path.IsPathRooted(cxt.AssemblyPath))
		{
			throw new ArgumentException(null, "cxt");
		}
		Type type = FindClassType(cxt);
		if (LicenseInteropProxy.HasLicense(type))
		{
			return new LicenseClassFactory(cxt.ClassId, type);
		}
		return new BasicClassFactory(cxt.ClassId, type);
	}

	[RequiresUnreferencedCode("Built-in COM support is not trim compatible", Url = "https://aka.ms/dotnet-illink/com")]
	private static void ClassRegistrationScenarioForType(ComActivationContext cxt, bool register)
	{
		if (!Marshal.IsBuiltInComSupported)
		{
			throw new NotSupportedException(SR.NotSupported_COM);
		}
		string text = (register ? "ComRegisterFunctionAttribute" : "ComUnregisterFunctionAttribute");
		Type type = Type.GetType("System.Runtime.InteropServices." + text + ", System.Runtime.InteropServices", throwOnError: false);
		if (type == null)
		{
			return;
		}
		if (!Path.IsPathRooted(cxt.AssemblyPath))
		{
			throw new ArgumentException(null, "cxt");
		}
		Type type2 = FindClassType(cxt);
		Type type3 = type2;
		bool flag = false;
		while (type3 != null && !flag)
		{
			MethodInfo[] methods = type3.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (MethodInfo methodInfo in methods)
			{
				if (methodInfo.GetCustomAttributes(type, inherit: true).Length != 0)
				{
					if (!methodInfo.IsStatic)
					{
						throw new InvalidOperationException(SR.Format(register ? SR.InvalidOperation_NonStaticComRegFunction : SR.InvalidOperation_NonStaticComUnRegFunction));
					}
					ReadOnlySpan<ParameterInfo> parametersAsSpan = methodInfo.GetParametersAsSpan();
					if (methodInfo.ReturnType != typeof(void) || parametersAsSpan.Length != 1 || (parametersAsSpan[0].ParameterType != typeof(string) && parametersAsSpan[0].ParameterType != typeof(Type)))
					{
						throw new InvalidOperationException(SR.Format(register ? SR.InvalidOperation_InvalidComRegFunctionSig : SR.InvalidOperation_InvalidComUnRegFunctionSig));
					}
					if (flag)
					{
						throw new InvalidOperationException(SR.Format(register ? SR.InvalidOperation_MultipleComRegFunctions : SR.InvalidOperation_MultipleComUnRegFunctions));
					}
					object[] array = new object[1];
					if (parametersAsSpan[0].ParameterType == typeof(string))
					{
						array[0] = $"HKEY_LOCAL_MACHINE\\SOFTWARE\\Classes\\CLSID\\{cxt.ClassId:B}";
					}
					else
					{
						array[0] = type2;
					}
					methodInfo.Invoke(null, array);
					flag = true;
				}
			}
			type3 = type3.BaseType;
		}
	}

	[UnmanagedCallersOnly]
	private unsafe static int GetClassFactoryForTypeInternal(ComActivationContextInternal* pCxtInt)
	{
		if (!Marshal.IsBuiltInComSupported)
		{
			throw new NotSupportedException(SR.NotSupported_COM);
		}
		return GetClassFactoryForTypeImpl(pCxtInt, isolatedContext: true);
	}

	[UnmanagedCallersOnly]
	private unsafe static int GetClassFactoryForTypeInContext(ComActivationContextInternal* pCxtInt, nint loadContext)
	{
		if (!Marshal.IsBuiltInComSupported)
		{
			throw new NotSupportedException(SR.NotSupported_COM);
		}
		if (loadContext != IntPtr.Zero && loadContext != -1)
		{
			throw new ArgumentOutOfRangeException("loadContext");
		}
		return GetClassFactoryForTypeLocal(pCxtInt, loadContext != IntPtr.Zero);
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "The same feature switch applies to GetClassFactoryForTypeInternal and this function. We rely on the warning from GetClassFactoryForTypeInternal.")]
		unsafe static int GetClassFactoryForTypeLocal(ComActivationContextInternal* pCxtInt2, bool isolatedContext)
		{
			return GetClassFactoryForTypeImpl(pCxtInt2, isolatedContext);
		}
	}

	[RequiresUnreferencedCode("Built-in COM support is not trim compatible", Url = "https://aka.ms/dotnet-illink/com")]
	private unsafe static int GetClassFactoryForTypeImpl(ComActivationContextInternal* pCxtInt, bool isolatedContext)
	{
		ref ComActivationContextInternal reference = ref *pCxtInt;
		try
		{
			nint iUnknownForObject = Marshal.GetIUnknownForObject(GetClassFactoryForType(ComActivationContext.Create(ref reference, isolatedContext)));
			Marshal.WriteIntPtr(reference.ClassFactoryDest, iUnknownForObject);
		}
		catch (Exception ex)
		{
			return ex.HResult;
		}
		return 0;
	}

	[UnmanagedCallersOnly]
	private unsafe static int RegisterClassForTypeInternal(ComActivationContextInternal* pCxtInt)
	{
		if (!Marshal.IsBuiltInComSupported)
		{
			throw new NotSupportedException(SR.NotSupported_COM);
		}
		return RegisterClassForTypeImpl(pCxtInt, isolatedContext: true);
	}

	[UnmanagedCallersOnly]
	private unsafe static int RegisterClassForTypeInContext(ComActivationContextInternal* pCxtInt, nint loadContext)
	{
		if (!Marshal.IsBuiltInComSupported)
		{
			throw new NotSupportedException(SR.NotSupported_COM);
		}
		if (loadContext != IntPtr.Zero && loadContext != -1)
		{
			throw new ArgumentOutOfRangeException("loadContext");
		}
		return RegisterClassForTypeImpl(pCxtInt, loadContext != IntPtr.Zero);
	}

	private unsafe static int RegisterClassForTypeImpl(ComActivationContextInternal* pCxtInt, bool isolatedContext)
	{
		ref ComActivationContextInternal reference = ref *pCxtInt;
		if (reference.InterfaceId != Guid.Empty || reference.ClassFactoryDest != IntPtr.Zero)
		{
			throw new ArgumentException(null, "pCxtInt");
		}
		try
		{
			ClassRegistrationScenarioForTypeLocal(ComActivationContext.Create(ref reference, isolatedContext), register: true);
		}
		catch (Exception ex)
		{
			return ex.HResult;
		}
		return 0;
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "The same feature switch applies to GetClassFactoryForTypeInternal and this function. We rely on the warning from GetClassFactoryForTypeInternal.")]
		static void ClassRegistrationScenarioForTypeLocal(ComActivationContext cxt, bool register)
		{
			ClassRegistrationScenarioForType(cxt, register);
		}
	}

	[UnmanagedCallersOnly]
	private unsafe static int UnregisterClassForTypeInternal(ComActivationContextInternal* pCxtInt)
	{
		if (!Marshal.IsBuiltInComSupported)
		{
			throw new NotSupportedException(SR.NotSupported_COM);
		}
		return UnregisterClassForTypeImpl(pCxtInt, isolatedContext: true);
	}

	[UnmanagedCallersOnly]
	private unsafe static int UnregisterClassForTypeInContext(ComActivationContextInternal* pCxtInt, nint loadContext)
	{
		if (!Marshal.IsBuiltInComSupported)
		{
			throw new NotSupportedException(SR.NotSupported_COM);
		}
		if (loadContext != IntPtr.Zero && loadContext != -1)
		{
			throw new ArgumentOutOfRangeException("loadContext");
		}
		return UnregisterClassForTypeImpl(pCxtInt, loadContext != IntPtr.Zero);
	}

	private unsafe static int UnregisterClassForTypeImpl(ComActivationContextInternal* pCxtInt, bool isolatedContext)
	{
		ref ComActivationContextInternal reference = ref *pCxtInt;
		if (reference.InterfaceId != Guid.Empty || reference.ClassFactoryDest != IntPtr.Zero)
		{
			throw new ArgumentException(null, "pCxtInt");
		}
		try
		{
			ClassRegistrationScenarioForTypeLocal(ComActivationContext.Create(ref reference, isolatedContext), register: false);
		}
		catch (Exception ex)
		{
			return ex.HResult;
		}
		return 0;
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "The same feature switch applies to GetClassFactoryForTypeInternal and this function. We rely on the warning from GetClassFactoryForTypeInternal.")]
		static void ClassRegistrationScenarioForTypeLocal(ComActivationContext cxt, bool register)
		{
			ClassRegistrationScenarioForType(cxt, register);
		}
	}

	[RequiresUnreferencedCode("Built-in COM support is not trim compatible", Url = "https://aka.ms/dotnet-illink/com")]
	private static Type FindClassType(ComActivationContext cxt)
	{
		try
		{
			AssemblyLoadContext aLC = GetALC(cxt.AssemblyPath, cxt.IsolatedContext);
			AssemblyName assemblyName = new AssemblyName(cxt.AssemblyName);
			Type type = aLC.LoadFromAssemblyName(assemblyName).GetType(cxt.TypeName);
			if (type != null)
			{
				return type;
			}
		}
		catch (Exception)
		{
		}
		throw new COMException(string.Empty, -2147221231);
	}

	[RequiresUnreferencedCode("The trimmer might remove types which are needed by the assemblies loaded in this method.")]
	private static AssemblyLoadContext GetALC(string assemblyPath, bool isolatedContext)
	{
		AssemblyLoadContext value;
		if (isolatedContext)
		{
			lock (s_assemblyLoadContexts)
			{
				if (!s_assemblyLoadContexts.TryGetValue(assemblyPath, out value))
				{
					value = new IsolatedComponentLoadContext(assemblyPath);
					s_assemblyLoadContexts.Add(assemblyPath, value);
				}
			}
		}
		else
		{
			value = AssemblyLoadContext.Default;
			lock (s_loadedInDefaultContext)
			{
				if (!s_loadedInDefaultContext.Contains(assemblyPath))
				{
					AssemblyDependencyResolver resolver = new AssemblyDependencyResolver(assemblyPath);
					AssemblyLoadContext.Default.Resolving += delegate(AssemblyLoadContext context, AssemblyName assemblyName)
					{
						string text = resolver.ResolveAssemblyToPath(assemblyName);
						return (text == null) ? null : context.LoadFromAssemblyPath(text);
					};
					s_loadedInDefaultContext.Add(assemblyPath);
				}
			}
		}
		return value;
	}
}
