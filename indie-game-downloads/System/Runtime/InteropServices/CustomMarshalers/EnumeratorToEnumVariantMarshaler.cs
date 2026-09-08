using System.Collections;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Versioning;

namespace System.Runtime.InteropServices.CustomMarshalers;

[SupportedOSPlatform("windows")]
internal sealed class EnumeratorToEnumVariantMarshaler : ICustomMarshaler
{
	private static readonly EnumeratorToEnumVariantMarshaler s_enumeratorToEnumVariantMarshaler = new EnumeratorToEnumVariantMarshaler();

	public static ICustomMarshaler GetInstance(string cookie)
	{
		return s_enumeratorToEnumVariantMarshaler;
	}

	private EnumeratorToEnumVariantMarshaler()
	{
	}

	public void CleanUpManagedData(object ManagedObj)
	{
	}

	public void CleanUpNativeData(nint pNativeData)
	{
		Marshal.Release(pNativeData);
	}

	public int GetNativeDataSize()
	{
		return -1;
	}

	public nint MarshalManagedToNative(object ManagedObj)
	{
		ArgumentNullException.ThrowIfNull(ManagedObj, "ManagedObj");
		if (ManagedObj is EnumeratorViewOfEnumVariant enumeratorViewOfEnumVariant)
		{
			return Marshal.GetComInterfaceForObject<object, IEnumVARIANT>(enumeratorViewOfEnumVariant.GetUnderlyingObject());
		}
		return Marshal.GetComInterfaceForObject<EnumVariantViewOfEnumerator, IEnumVARIANT>(new EnumVariantViewOfEnumerator((IEnumerator)ManagedObj));
	}

	internal static object InternalMarshalNativeToManaged(nint pNativeData)
	{
		return GetInstance(null).MarshalNativeToManaged(pNativeData);
	}

	public object MarshalNativeToManaged(nint pNativeData)
	{
		ArgumentNullException.ThrowIfNull(pNativeData, "pNativeData");
		object objectForIUnknown = Marshal.GetObjectForIUnknown(pNativeData);
		if (!objectForIUnknown.GetType().IsCOMObject)
		{
			if (objectForIUnknown is EnumVariantViewOfEnumerator enumVariantViewOfEnumerator)
			{
				return enumVariantViewOfEnumerator.Enumerator;
			}
			return objectForIUnknown as IEnumerator;
		}
		return ComDataHelpers.GetOrCreateManagedViewFromComData(objectForIUnknown, (IEnumVARIANT var) => new EnumeratorViewOfEnumVariant(var));
	}
}
