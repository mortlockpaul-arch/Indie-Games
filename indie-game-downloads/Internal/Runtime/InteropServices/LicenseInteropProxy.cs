using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.Runtime.InteropServices;

internal sealed class LicenseInteropProxy
{
	private static readonly Type s_licenseAttrType = Type.GetType("System.ComponentModel.LicenseProviderAttribute, System.ComponentModel.TypeConverter", throwOnError: false);

	private static readonly Type s_licenseExceptionType = Type.GetType("System.ComponentModel.LicenseException, System.ComponentModel.TypeConverter", throwOnError: false);

	private object _licContext;

	private Type _targetRcwType;

	[UnsafeAccessor(UnsafeAccessorKind.Method)]
	private static extern void SetSavedLicenseKey([UnsafeAccessorType("System.ComponentModel.LicenseContext, System.ComponentModel.TypeConverter")] object licContext, Type type, string key);

	[UnconditionalSuppressMessage("Trimming", "IL2111", Justification = "Manually validated that the annotations are kept in sync.")]
	[UnsafeAccessor(UnsafeAccessorKind.StaticMethod)]
	private static extern object CreateWithContext([UnsafeAccessorType("System.ComponentModel.LicenseManager, System.ComponentModel.TypeConverter")] object licManager, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type type, [UnsafeAccessorType("System.ComponentModel.LicenseContext, System.ComponentModel.TypeConverter")] object licContext);

	[UnsafeAccessor(UnsafeAccessorKind.StaticMethod)]
	private static extern bool ValidateAndRetrieveLicenseDetails([UnsafeAccessorType("System.ComponentModel.LicenseManager+LicenseInteropHelper, System.ComponentModel.TypeConverter")] object licInteropHelper, [UnsafeAccessorType("System.ComponentModel.LicenseContext, System.ComponentModel.TypeConverter")] object licContext, Type type, [UnsafeAccessorType("System.ComponentModel.License&, System.ComponentModel.TypeConverter")] out object license, out string licenseKey);

	[UnsafeAccessor(UnsafeAccessorKind.StaticMethod)]
	[return: UnsafeAccessorType("System.ComponentModel.LicenseContext, System.ComponentModel.TypeConverter")]
	private static extern object GetCurrentContextInfo([UnsafeAccessorType("System.ComponentModel.LicenseManager+LicenseInteropHelper, System.ComponentModel.TypeConverter")] object licInteropHelper, Type type, out bool isDesignTime, out string key);

	[UnsafeAccessor(UnsafeAccessorKind.StaticMethod)]
	[return: UnsafeAccessorType("System.ComponentModel.LicenseManager+CLRLicenseContext, System.ComponentModel.TypeConverter")]
	private static extern object CreateDesignContext([UnsafeAccessorType("System.ComponentModel.LicenseManager+CLRLicenseContext, System.ComponentModel.TypeConverter")] object context, Type type);

	[UnsafeAccessor(UnsafeAccessorKind.StaticMethod)]
	[return: UnsafeAccessorType("System.ComponentModel.LicenseManager+CLRLicenseContext, System.ComponentModel.TypeConverter")]
	private static extern object CreateRuntimeContext([UnsafeAccessorType("System.ComponentModel.LicenseManager+CLRLicenseContext, System.ComponentModel.TypeConverter")] object context, Type type, string key);

	[UnsafeAccessor(UnsafeAccessorKind.Constructor)]
	[return: UnsafeAccessorType("System.ComponentModel.LicenseManager+LicInfoHelperLicenseContext, System.ComponentModel.TypeConverter")]
	private static extern object CreateLicInfoHelperLicenseContext();

	[UnsafeAccessor(UnsafeAccessorKind.Method)]
	private static extern bool Contains([UnsafeAccessorType("System.ComponentModel.LicenseManager+LicInfoHelperLicenseContext, System.ComponentModel.TypeConverter")] object licInfoHelperContext, string assemblyName);

	public static object Create()
	{
		return new LicenseInteropProxy();
	}

	public static bool HasLicense(Type type)
	{
		if (s_licenseAttrType == null)
		{
			return false;
		}
		return type.IsDefined(s_licenseAttrType, inherit: true);
	}

	public static void GetLicInfo(Type type, out bool runtimeKeyAvail, out bool licVerified)
	{
		runtimeKeyAvail = false;
		licVerified = false;
		object obj = CreateLicInfoHelperLicenseContext();
		if (ValidateAndRetrieveLicenseDetails(null, obj, type, out var license, out var _))
		{
			if (license is IDisposable disposable)
			{
				disposable.Dispose();
				licVerified = true;
			}
			runtimeKeyAvail = Contains(obj, type.AssemblyQualifiedName);
		}
	}

	public static string RequestLicKey(Type type)
	{
		if (!ValidateAndRetrieveLicenseDetails(null, null, type, out var license, out var licenseKey))
		{
			throw new COMException();
		}
		((IDisposable)license)?.Dispose();
		return licenseKey ?? throw new COMException();
	}

	public static object AllocateAndValidateLicense([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type type, string key, bool isDesignTime)
	{
		object licContext = ((!isDesignTime) ? CreateRuntimeContext(null, type, key) : CreateDesignContext(null, type));
		try
		{
			return CreateWithContext(null, type, licContext);
		}
		catch (Exception ex) when (ex.GetType() == s_licenseExceptionType)
		{
			throw new COMException(ex.Message, -2147221230);
		}
	}

	public void GetCurrentContextInfo(RuntimeTypeHandle rth, out bool isDesignTime, out nint bstrKey)
	{
		Type typeFromHandle = Type.GetTypeFromHandle(rth);
		_licContext = GetCurrentContextInfo(null, typeFromHandle, out isDesignTime, out var key);
		_targetRcwType = typeFromHandle;
		bstrKey = Marshal.StringToBSTR(key);
	}

	public void SaveKeyInCurrentContext(nint bstrKey)
	{
		if (bstrKey != IntPtr.Zero)
		{
			string key = Marshal.PtrToStringBSTR(bstrKey);
			SetSavedLicenseKey(_licContext, _targetRcwType, key);
		}
	}
}
