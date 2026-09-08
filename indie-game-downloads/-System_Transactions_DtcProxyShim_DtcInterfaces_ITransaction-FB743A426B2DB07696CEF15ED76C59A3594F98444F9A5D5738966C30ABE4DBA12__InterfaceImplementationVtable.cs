using System;
using System.Runtime.InteropServices;
using System.Transactions.DtcProxyShim;

internal struct _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransaction_003EFB743A426B2DB07696CEF15ED76C59A3594F98444F9A5D5738966C30ABE4DBA12__InterfaceImplementationVtable
{
	public unsafe delegate* unmanaged[MemberFunction]<void*, Guid*, void**, int> QueryInterface_0;

	public unsafe delegate* unmanaged[MemberFunction]<void*, uint> AddRef_1;

	public unsafe delegate* unmanaged[MemberFunction]<void*, uint> Release_2;

	public unsafe delegate* unmanaged[MemberFunction]<ComWrappers.ComInterfaceDispatch*, int, OletxXacttc, uint, int> Commit_3;

	public unsafe delegate* unmanaged[MemberFunction]<ComWrappers.ComInterfaceDispatch*, nint, int, int, int> Abort_4;

	public unsafe delegate* unmanaged[MemberFunction]<ComWrappers.ComInterfaceDispatch*, OletxXactTransInfo*, int> GetTransactionInfo_5;
}
