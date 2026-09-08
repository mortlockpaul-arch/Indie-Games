using System.Runtime.InteropServices.Marshalling;
using System.Threading;
using System.Transactions.DtcProxyShim.DtcInterfaces;
using System.Transactions.Oletx;

namespace System.Transactions.DtcProxyShim;

[GeneratedComClass]
[ComExposedClass<_003CSystem_Transactions_DtcProxyShim_EnlistmentNotifyShim_003EF47E6FA6FFD0D268549B3DCBFD7D41032D7F1ADAC633BBE13987164886905529F__ComClassInformation>]
internal sealed class EnlistmentNotifyShim : NotificationShimBase, ITransactionResourceAsync
{
	internal ITransactionEnlistmentAsync EnlistmentAsync;

	private bool _ignoreSpuriousProxyNotifications;

	internal EnlistmentNotifyShim(DtcProxyShimFactory shimFactory, OletxEnlistment enlistmentIdentifier)
		: base(shimFactory, enlistmentIdentifier)
	{
		_ignoreSpuriousProxyNotifications = false;
	}

	internal void SetIgnoreSpuriousProxyNotifications()
	{
		_ignoreSpuriousProxyNotifications = true;
	}

	public void PrepareRequest(bool fRetaining, OletxXactRm grfRM, bool fWantMoniker, bool fSinglePhase)
	{
		IPrepareInfo obj = (IPrepareInfo)(Interlocked.Exchange(ref EnlistmentAsync, null) ?? throw new InvalidOperationException("Unexpected null in pEnlistmentAsync"));
		obj.GetPrepareInfoSize(out var pcbPrepInfo);
		byte[] array = new byte[pcbPrepInfo];
		obj.GetPrepareInfo(array);
		PrepareInfo = array;
		IsSinglePhase = fSinglePhase;
		NotificationType = ShimNotificationType.PrepareRequestNotify;
		ShimFactory.NewNotification(this);
	}

	public void CommitRequest(OletxXactRm grfRM, nint pNewUOW)
	{
		NotificationType = ShimNotificationType.CommitRequestNotify;
		ShimFactory.NewNotification(this);
	}

	public void AbortRequest(nint pboidReason, bool fRetaining, nint pNewUOW)
	{
		if (!_ignoreSpuriousProxyNotifications)
		{
			NotificationType = ShimNotificationType.AbortRequestNotify;
			ShimFactory.NewNotification(this);
		}
	}

	public void TMDown()
	{
		NotificationType = ShimNotificationType.ResourceManagerTmDownNotify;
		ShimFactory.NewNotification(this);
	}
}
