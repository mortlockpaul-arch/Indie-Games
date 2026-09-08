using System.Transactions.DtcProxyShim.DtcInterfaces;

namespace System.Transactions.DtcProxyShim;

internal sealed class VoterBallotShim
{
	private readonly VoterNotifyShim _voterNotifyShim;

	internal ITransactionVoterBallotAsync2 VoterBallotAsync2 { get; set; }

	internal VoterBallotShim(VoterNotifyShim notifyShim)
	{
		_voterNotifyShim = notifyShim;
	}

	public void Vote(bool voteYes)
	{
		int hr = 0;
		if (!voteYes)
		{
			hr = -2147467259;
		}
		VoterBallotAsync2.VoteRequestDone(hr, IntPtr.Zero);
	}
}
