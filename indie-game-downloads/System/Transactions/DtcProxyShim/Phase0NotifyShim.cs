using System.Runtime.InteropServices.Marshalling;
using System.Transactions.DtcProxyShim.DtcInterfaces;

namespace System.Transactions.DtcProxyShim;

[GeneratedComClass]
[ComExposedClass<_003CSystem_Transactions_DtcProxyShim_Phase0NotifyShim_003EFCAF6D6532B6DEAD97BAF81D6C20E91901C9925D7D5BEB7FC9E4ABC65DCB5F23D__ComClassInformation>]
internal sealed class Phase0NotifyShim : NotificationShimBase, ITransactionPhase0NotifyAsync
{
	internal Phase0NotifyShim(DtcProxyShimFactory shimFactory, object enlistmentIdentifier)
		: base(shimFactory, enlistmentIdentifier)
	{
	}

	public void Phase0Request(bool fAbortHint)
	{
		AbortingHint = fAbortHint;
		NotificationType = ShimNotificationType.Phase0RequestNotify;
		ShimFactory.NewNotification(this);
	}

	public void EnlistCompleted(int status)
	{
	}
}
