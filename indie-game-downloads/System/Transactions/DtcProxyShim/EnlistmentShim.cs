using System.Transactions.DtcProxyShim.DtcInterfaces;

namespace System.Transactions.DtcProxyShim;

internal sealed class EnlistmentShim
{
	private readonly EnlistmentNotifyShim _enlistmentNotifyShim;

	internal ITransactionEnlistmentAsync EnlistmentAsync { get; set; }

	internal EnlistmentShim(EnlistmentNotifyShim notifyShim)
	{
		_enlistmentNotifyShim = notifyShim;
	}

	public void PrepareRequestDone(OletxPrepareVoteType voteType)
	{
		int hr = 0;
		bool flag = false;
		switch (voteType)
		{
		case OletxPrepareVoteType.ReadOnly:
			_enlistmentNotifyShim.SetIgnoreSpuriousProxyNotifications();
			hr = 315394;
			break;
		case OletxPrepareVoteType.SinglePhase:
			_enlistmentNotifyShim.SetIgnoreSpuriousProxyNotifications();
			hr = 315401;
			break;
		case OletxPrepareVoteType.Prepared:
			hr = 0;
			break;
		case OletxPrepareVoteType.Failed:
			_enlistmentNotifyShim.SetIgnoreSpuriousProxyNotifications();
			hr = -2147467259;
			break;
		case OletxPrepareVoteType.InDoubt:
			flag = true;
			break;
		default:
			hr = -2147467259;
			break;
		}
		if (!flag)
		{
			EnlistmentAsync.PrepareRequestDone(hr, IntPtr.Zero, IntPtr.Zero);
		}
	}

	public void CommitRequestDone()
	{
		EnlistmentAsync.CommitRequestDone(0);
	}

	public void AbortRequestDone()
	{
		EnlistmentAsync.AbortRequestDone(0);
	}
}
