using System.Threading;

namespace System.Transactions;

internal abstract class TransactionStateEnded : TransactionState
{
	internal override void EnterState(InternalTransaction tx)
	{
		if (tx._needPulse)
		{
			Monitor.Pulse(tx);
		}
	}

	internal override void AddOutcomeRegistrant(InternalTransaction tx, TransactionCompletedEventHandler transactionCompletedDelegate)
	{
		if (transactionCompletedDelegate != null)
		{
			TransactionEventArgs e = new TransactionEventArgs();
			e._transaction = tx._outcomeSource.InternalClone();
			transactionCompletedDelegate(e._transaction, e);
		}
	}

	internal override bool IsCompleted(InternalTransaction tx)
	{
		return true;
	}
}
