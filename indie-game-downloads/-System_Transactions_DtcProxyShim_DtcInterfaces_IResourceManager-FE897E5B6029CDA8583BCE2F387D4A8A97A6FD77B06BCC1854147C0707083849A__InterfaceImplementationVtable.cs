using System;
using System.Runtime.InteropServices;
using System.Transactions.DtcProxyShim;

internal struct _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_IResourceManager_003EFE897E5B6029CDA8583BCE2F387D4A8A97A6FD77B06BCC1854147C0707083849A__InterfaceImplementationVtable
{
	public unsafe delegate* unmanaged[MemberFunction]<void*, Guid*, void**, int> QueryInterface_0;

	public unsafe delegate* unmanaged[MemberFunction]<void*, uint> AddRef_1;

	public unsafe delegate* unmanaged[MemberFunction]<void*, uint> Release_2;

	public unsafe delegate* unmanaged[MemberFunction]<ComWrappers.ComInterfaceDispatch*, void*, void*, Guid*, OletxTransactionIsolationLevel*, void**, int> Enlist_3;

	public unsafe delegate* unmanaged[MemberFunction]<ComWrappers.ComInterfaceDispatch*, byte*, uint, uint, OletxXactStat*, int> Reenlist_4;

	public unsafe delegate* unmanaged[MemberFunction]<ComWrappers.ComInterfaceDispatch*, int> ReenlistmentComplete_5;

	public unsafe delegate* unmanaged[MemberFunction]<ComWrappers.ComInterfaceDispatch*, Guid*, void**, int> GetDistributedTransactionManager_6;
}
