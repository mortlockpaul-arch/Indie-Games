using System;
using System.Runtime.InteropServices;
using System.Transactions.DtcProxyShim;

internal struct _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionDispenser_003EF4636BE28F486AD166242976B62D2137F68C9EF0A003FD5347E9743D9211D8E44__InterfaceImplementationVtable
{
	public unsafe delegate* unmanaged[MemberFunction]<void*, Guid*, void**, int> QueryInterface_0;

	public unsafe delegate* unmanaged[MemberFunction]<void*, uint> AddRef_1;

	public unsafe delegate* unmanaged[MemberFunction]<void*, uint> Release_2;

	public unsafe delegate* unmanaged[MemberFunction]<ComWrappers.ComInterfaceDispatch*, void**, int> GetOptionsObject_3;

	public unsafe delegate* unmanaged[MemberFunction]<ComWrappers.ComInterfaceDispatch*, nint, OletxTransactionIsolationLevel, OletxTransactionIsoFlags, void*, void**, int> BeginTransaction_4;
}
