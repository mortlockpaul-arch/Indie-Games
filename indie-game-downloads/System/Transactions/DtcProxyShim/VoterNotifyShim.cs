using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Transactions.DtcProxyShim.DtcInterfaces;

namespace System.Transactions.DtcProxyShim;

[GeneratedComClass]
[ComExposedClass<_003CSystem_Transactions_DtcProxyShim_VoterNotifyShim_003EFA18C5EDCD51F4A664DC19CC0FA94E2E0E150CD525B2D4C7D4BA0830115345656__ComClassInformation>]
internal sealed class VoterNotifyShim : NotificationShimBase, ITransactionVoterNotifyAsync2
{
	internal VoterNotifyShim(DtcProxyShimFactory shimFactory, object enlistmentIdentifier)
		: base(shimFactory, enlistmentIdentifier)
	{
	}

	public void VoteRequest()
	{
		NotificationType = ShimNotificationType.VoteRequestNotify;
		ShimFactory.NewNotification(this);
	}

	public void Committed([MarshalAs(UnmanagedType.Bool)] bool fRetaining, nint pNewUOW, uint hresult)
	{
		NotificationType = ShimNotificationType.CommittedNotify;
		ShimFactory.NewNotification(this);
	}

	public void Aborted(nint pboidReason, [MarshalAs(UnmanagedType.Bool)] bool fRetaining, nint pNewUOW, uint hresult)
	{
		NotificationType = ShimNotificationType.AbortedNotify;
		ShimFactory.NewNotification(this);
	}

	public void HeuristicDecision([MarshalAs(UnmanagedType.U4)] OletxTransactionHeuristic dwDecision, nint pboidReason, uint hresult)
	{
		NotificationType = dwDecision switch
		{
			OletxTransactionHeuristic.XACTHEURISTIC_ABORT => ShimNotificationType.AbortedNotify, 
			OletxTransactionHeuristic.XACTHEURISTIC_COMMIT => ShimNotificationType.CommittedNotify, 
			_ => ShimNotificationType.InDoubtNotify, 
		};
		ShimFactory.NewNotification(this);
	}

	public void Indoubt()
	{
		NotificationType = ShimNotificationType.InDoubtNotify;
		ShimFactory.NewNotification(this);
	}
}
