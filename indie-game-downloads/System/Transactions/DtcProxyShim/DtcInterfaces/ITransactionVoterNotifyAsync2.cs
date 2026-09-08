using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace System.Transactions.DtcProxyShim.DtcInterfaces;

[GeneratedComInterface]
[Guid("5433376B-414D-11d3-B206-00C04FC2F3EF")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[IUnknownDerived<_003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionVoterNotifyAsync2_003EF5DAB68CCF462F421106F6576DEDA647A9A8BF7C30EC1987A89F37CE0D05F1C24__InterfaceInformation, _003CSystem_Transactions_DtcProxyShim_DtcInterfaces_ITransactionVoterNotifyAsync2_003EF5DAB68CCF462F421106F6576DEDA647A9A8BF7C30EC1987A89F37CE0D05F1C24__InterfaceImplementation>]
internal interface ITransactionVoterNotifyAsync2
{
	void Committed([MarshalAs(UnmanagedType.Bool)] bool fRetaining, nint pNewUOW, uint hresult);

	void Aborted(nint pboidReason, [MarshalAs(UnmanagedType.Bool)] bool fRetaining, nint pNewUOW, uint hresult);

	void HeuristicDecision(OletxTransactionHeuristic dwDecision, nint pboidReason, uint hresult);

	void Indoubt();

	void VoteRequest();
}
