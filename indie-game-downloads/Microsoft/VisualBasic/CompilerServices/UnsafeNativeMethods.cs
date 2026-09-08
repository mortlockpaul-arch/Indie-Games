using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.VisualBasic.CompilerServices;

[ComVisible(false)]
internal sealed class UnsafeNativeMethods
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public enum tagSYSKIND
	{
		SYS_WIN16 = 0,
		SYS_MAC = 2
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct tagTLIBATTR
	{
		public Guid guid;

		public int lcid;

		public tagSYSKIND syskind;

		[MarshalAs(UnmanagedType.U2)]
		public short wMajorVerNum;

		[MarshalAs(UnmanagedType.U2)]
		public short wMinorVerNum;

		[MarshalAs(UnmanagedType.U2)]
		public short wLibFlags;
	}

	[ComImport]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Guid("00020403-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface ITypeComp
	{
		[Obsolete("Bad signature. Fix and verify signature before use.", true)]
		void RemoteBind([In][MarshalAs(UnmanagedType.LPWStr)] string szName, [In][MarshalAs(UnmanagedType.U4)] int lHashVal, [In][MarshalAs(UnmanagedType.U2)] short wFlags, [Out][MarshalAs(UnmanagedType.LPArray)] ITypeInfo[] ppTInfo, [Out][MarshalAs(UnmanagedType.LPArray)] DESCKIND[] pDescKind, [Out][MarshalAs(UnmanagedType.LPArray)] FUNCDESC[] ppFuncDesc, [Out][MarshalAs(UnmanagedType.LPArray)] VARDESC[] ppVarDesc, [Out][MarshalAs(UnmanagedType.LPArray)] ITypeComp[] ppTypeComp, [Out][MarshalAs(UnmanagedType.LPArray)] int[] pDummy);

		void RemoteBindType([In][MarshalAs(UnmanagedType.LPWStr)] string szName, [In][MarshalAs(UnmanagedType.U4)] int lHashVal, [Out][MarshalAs(UnmanagedType.LPArray)] ITypeInfo[] ppTInfo);
	}

	[ComImport]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Guid("00020400-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IDispatch
	{
		[PreserveSig]
		[Obsolete("Bad signature. Fix and verify signature before use.", true)]
		int GetTypeInfoCount();

		[PreserveSig]
		int GetTypeInfo([In] int index, [In] int lcid, [MarshalAs(UnmanagedType.Interface)] out ITypeInfo pTypeInfo);

		[PreserveSig]
		int GetIDsOfNames();

		[PreserveSig]
		int Invoke();
	}

	[ComImport]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Guid("00020401-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface ITypeInfo
	{
		[PreserveSig]
		int GetTypeAttr(out nint pTypeAttr);

		[PreserveSig]
		int GetTypeComp(out ITypeComp pTComp);

		[PreserveSig]
		int GetFuncDesc([In][MarshalAs(UnmanagedType.U4)] int index, out nint pFuncDesc);

		[PreserveSig]
		int GetVarDesc([In][MarshalAs(UnmanagedType.U4)] int index, out nint pVarDesc);

		[PreserveSig]
		int GetNames([In] int memid, [Out][MarshalAs(UnmanagedType.LPArray)] string[] rgBstrNames, [In][MarshalAs(UnmanagedType.U4)] int cMaxNames, [MarshalAs(UnmanagedType.U4)] out int cNames);

		[PreserveSig]
		[Obsolete("Bad signature, second param type should be Byref. Fix and verify signature before use.", true)]
		int GetRefTypeOfImplType([In][MarshalAs(UnmanagedType.U4)] int index, out int pRefType);

		[PreserveSig]
		[Obsolete("Bad signature, second param type should be Byref. Fix and verify signature before use.", true)]
		int GetImplTypeFlags([In][MarshalAs(UnmanagedType.U4)] int index, [Out] int pImplTypeFlags);

		[PreserveSig]
		int GetIDsOfNames([In] nint rgszNames, [In][MarshalAs(UnmanagedType.U4)] int cNames, out nint pMemId);

		[PreserveSig]
		[Obsolete("Bad signature. Fix and verify signature before use.", true)]
		int Invoke();

		[PreserveSig]
		int GetDocumentation([In] int memid, [MarshalAs(UnmanagedType.BStr)] out string pBstrName, [MarshalAs(UnmanagedType.BStr)] out string pBstrDocString, [MarshalAs(UnmanagedType.U4)] out int pdwHelpContext, [MarshalAs(UnmanagedType.BStr)] out string pBstrHelpFile);

		[PreserveSig]
		[Obsolete("Bad signature. Fix and verify signature before use.", true)]
		int GetDllEntry([In] int memid, [In] INVOKEKIND invkind, [Out][MarshalAs(UnmanagedType.BStr)] string pBstrDllName, [Out][MarshalAs(UnmanagedType.BStr)] string pBstrName, [Out][MarshalAs(UnmanagedType.U2)] short pwOrdinal);

		[PreserveSig]
		int GetRefTypeInfo([In] nint hreftype, out ITypeInfo pTypeInfo);

		[PreserveSig]
		[Obsolete("Bad signature. Fix and verify signature before use.", true)]
		int AddressOfMember();

		[PreserveSig]
		[Obsolete("Bad signature. Fix and verify signature before use.", true)]
		int CreateInstance([In] ref nint pUnkOuter, [In] ref Guid riid, [Out][MarshalAs(UnmanagedType.IUnknown)] object ppvObj);

		[PreserveSig]
		[Obsolete("Bad signature. Fix and verify signature before use.", true)]
		int GetMops([In] int memid, [Out][MarshalAs(UnmanagedType.BStr)] string pBstrMops);

		[PreserveSig]
		int GetContainingTypeLib([Out][MarshalAs(UnmanagedType.LPArray)] ITypeLib[] ppTLib, [Out][MarshalAs(UnmanagedType.LPArray)] int[] pIndex);

		[PreserveSig]
		void ReleaseTypeAttr(nint typeAttr);

		[PreserveSig]
		void ReleaseFuncDesc(nint funcDesc);

		[PreserveSig]
		void ReleaseVarDesc(nint varDesc);
	}

	[ComImport]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Guid("B196B283-BAB4-101A-B69C-00AA00341D07")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IProvideClassInfo
	{
		[return: MarshalAs(UnmanagedType.Interface)]
		ITypeInfo GetClassInfo();
	}

	[ComImport]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Guid("00020402-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface ITypeLib
	{
		[Obsolete("Bad signature. Fix and verify signature before use.", true)]
		void RemoteGetTypeInfoCount([Out][MarshalAs(UnmanagedType.LPArray)] int[] pcTInfo);

		void GetTypeInfo([In][MarshalAs(UnmanagedType.U4)] int index, [Out][MarshalAs(UnmanagedType.LPArray)] ITypeInfo[] ppTInfo);

		void GetTypeInfoType([In][MarshalAs(UnmanagedType.U4)] int index, [Out][MarshalAs(UnmanagedType.LPArray)] TYPEKIND[] pTKind);

		void GetTypeInfoOfGuid([In] ref Guid guid, [Out][MarshalAs(UnmanagedType.LPArray)] ITypeInfo[] ppTInfo);

		[Obsolete("Bad signature. Fix and verify signature before use.", true)]
		void RemoteGetLibAttr([Out][MarshalAs(UnmanagedType.LPArray)] tagTLIBATTR[] ppTLibAttr, [Out][MarshalAs(UnmanagedType.LPArray)] int[] pDummy);

		void GetTypeComp([Out][MarshalAs(UnmanagedType.LPArray)] ITypeComp[] ppTComp);

		[Obsolete("Bad signature. Fix and verify signature before use.", true)]
		void RemoteGetDocumentation(int index, [In][MarshalAs(UnmanagedType.U4)] int refPtrFlags, [Out][MarshalAs(UnmanagedType.LPArray)] string[] pBstrName, [Out][MarshalAs(UnmanagedType.LPArray)] string[] pBstrDocString, [Out][MarshalAs(UnmanagedType.LPArray)] int[] pdwHelpContext, [Out][MarshalAs(UnmanagedType.LPArray)] string[] pBstrHelpFile);

		[Obsolete("Bad signature. Fix and verify signature before use.", true)]
		void RemoteIsName([In][MarshalAs(UnmanagedType.LPWStr)] string szNameBuf, [In][MarshalAs(UnmanagedType.U4)] int lHashVal, [Out][MarshalAs(UnmanagedType.LPArray)] nint[] pfName, [Out][MarshalAs(UnmanagedType.LPArray)] string[] pBstrLibName);

		[Obsolete("Bad signature. Fix and verify signature before use.", true)]
		void RemoteFindName([In][MarshalAs(UnmanagedType.LPWStr)] string szNameBuf, [In][MarshalAs(UnmanagedType.U4)] int lHashVal, [Out][MarshalAs(UnmanagedType.LPArray)] ITypeInfo[] ppTInfo, [Out][MarshalAs(UnmanagedType.LPArray)] int[] rgMemId, [In][Out][MarshalAs(UnmanagedType.LPArray)] short[] pcFound, [Out][MarshalAs(UnmanagedType.LPArray)] string[] pBstrLibName);

		[Obsolete("Bad signature. Fix and verify signature before use.", true)]
		void LocalReleaseTLibAttr();
	}

	[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
	internal static extern int LCMapStringA(int Locale, int dwMapFlags, [MarshalAs(UnmanagedType.LPArray)] byte[] lpSrcStr, int cchSrc, [MarshalAs(UnmanagedType.LPArray)] byte[] lpDestStr, int cchDest);

	[DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
	internal static extern int LCMapString(int Locale, int dwMapFlags, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpSrcStr, int cchSrc, [MarshalAs(UnmanagedType.VBByRefStr)] ref string lpDestStr, int cchDest);

	[DllImport("oleaut32", CharSet = CharSet.Unicode)]
	internal static extern int VarParseNumFromStr([In][MarshalAs(UnmanagedType.LPWStr)] string str, int lcid, int dwFlags, [MarshalAs(UnmanagedType.LPArray)] byte[] numprsPtr, [MarshalAs(UnmanagedType.LPArray)] byte[] digits);

	[DllImport("oleaut32", CharSet = CharSet.Unicode, PreserveSig = false)]
	[RequiresUnreferencedCode("Marshalling COM Objects is not trim safe.")]
	internal static extern object VarNumFromParseNum([MarshalAs(UnmanagedType.LPArray)] byte[] numprsPtr, [MarshalAs(UnmanagedType.LPArray)] byte[] DigitArray, int dwVtBits);

	[DllImport("oleaut32", CharSet = CharSet.Unicode, PreserveSig = false)]
	[RequiresUnreferencedCode("Marshalling COM Objects is not trim safe.")]
	internal static extern void VariantChangeType(out object dest, [In] ref object Src, short wFlags, short vt);

	[DllImport("user32", CharSet = CharSet.Unicode)]
	internal static extern int MessageBeep(int uType);

	[DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
	internal static extern int SetLocalTime(NativeTypes.SystemTime systime);

	[DllImport("kernel32", BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true, ThrowOnUnmappableChar = true)]
	internal static extern int MoveFile([In][MarshalAs(UnmanagedType.LPTStr)] string lpExistingFileName, [In][MarshalAs(UnmanagedType.LPTStr)] string lpNewFileName);

	[DllImport("kernel32", CharSet = CharSet.Unicode)]
	internal static extern int GetLogicalDrives();
}
