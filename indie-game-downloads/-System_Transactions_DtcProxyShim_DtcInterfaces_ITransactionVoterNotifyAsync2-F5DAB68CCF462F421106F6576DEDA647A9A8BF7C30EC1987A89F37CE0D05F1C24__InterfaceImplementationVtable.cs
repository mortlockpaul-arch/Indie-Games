using System;
using System.Runtime.InteropServices;
using System.Transactions.DtcProxyShim;

internal struct _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionVoterNotifyAsync2_003EF5DAB68CCF462F421106F6576DEDA647A9A8BF7C30EC1987A89F37CE0D05F1C24__InterfaceImplementationVtable
{
	public unsafe delegate* unmanaged[MemberFunction]<void*, Guid*, void**, int> QueryInterface_0;

	public unsafe delegate* unmanaged[MemberFunction]<void*, uint> AddRef_1;

	public unsafe delegate* unmanaged[MemberFunction]<void*, uint> Release_2;

	public unsafe delegate* unmanaged[MemberFunction]<ComWrappers.ComInterfaceDispatch*, int, nint, uint, int> Committed_3;

	public unsafe delegate* unmanaged[MemberFunction]<ComWrappers.ComInterfaceDispatch*, nint, int, nint, uint, int> Aborted_4;

	public unsafe delegate* unmanaged[MemberFunction]<ComWrappers.ComInterfaceDispatch*, OletxTransactionHeuristic, nint, uint, int> HeuristicDecision_5;

	public unsafe delegate* unmanaged[MemberFunction]<ComWrappers.ComInterfaceDispatch*, int> Indoubt_6;

	public unsafe delegate* unmanaged[MemberFunction]<ComWrappers.ComInterfaceDispatch*, int> VoteRequest_7;
}
