using System;
using System.Runtime.InteropServices;
using System.Transactions.DtcProxyShim;

internal struct _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionCloner_003EFBEFDFCFA77AE7ACF158F737F8E59B4BE001EE38D948ACDD630E7CF00FF701434__InterfaceImplementationVtable
{
	public unsafe delegate* unmanaged[MemberFunction]<void*, Guid*, void**, int> QueryInterface_0;

	public unsafe delegate* unmanaged[MemberFunction]<void*, uint> AddRef_1;

	public unsafe delegate* unmanaged[MemberFunction]<void*, uint> Release_2;

	public unsafe delegate* unmanaged[MemberFunction]<ComWrappers.ComInterfaceDispatch*, int, OletxXacttc, uint, int> Commit_3;

	public unsafe delegate* unmanaged[MemberFunction]<ComWrappers.ComInterfaceDispatch*, nint, int, int, int> Abort_4;

	public unsafe delegate* unmanaged[MemberFunction]<ComWrappers.ComInterfaceDispatch*, OletxXactTransInfo*, int> GetTransactionInfo_5;

	public unsafe delegate* unmanaged[MemberFunction]<ComWrappers.ComInterfaceDispatch*, void**, int> CloneWithCommitDisabled_6;
}
