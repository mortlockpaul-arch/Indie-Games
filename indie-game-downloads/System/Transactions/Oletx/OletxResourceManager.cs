using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;
using System.Transactions.DtcProxyShim;

namespace System.Transactions.Oletx;

internal sealed class OletxResourceManager
{
	internal Guid ResourceManagerIdentifier;

	internal ResourceManagerShim resourceManagerShim;

	internal Hashtable EnlistmentHashtable;

	internal OletxTransactionManager OletxTransactionManager;

	internal ArrayList ReenlistList;

	internal ArrayList ReenlistPendingList;

	internal Timer ReenlistThreadTimer;

	internal Thread reenlistThread;

	internal bool RecoveryCompleteCalledByApplication { get; set; }

	internal ResourceManagerShim ResourceManagerShim
	{
		get
		{
			ResourceManagerShim resourceManagerShim = null;
			if (this.resourceManagerShim == null)
			{
				lock (this)
				{
					if (this.resourceManagerShim == null)
					{
						OletxTransactionManager.DtcTransactionManagerLock.AcquireReaderLock(-1);
						try
						{
							Guid resourceManagerIdentifier = ResourceManagerIdentifier;
							OletxTransactionManager.DtcTransactionManager.ProxyShimFactory.CreateResourceManager(resourceManagerIdentifier, this, out resourceManagerShim);
						}
						catch (COMException ex)
						{
							if (ex.ErrorCode != -2147168228 && ex.ErrorCode != -2147168229)
							{
								throw;
							}
							resourceManagerShim = null;
							TransactionsEtwProvider log = TransactionsEtwProvider.Log;
							if (log.IsEnabled())
							{
								log.ExceptionConsumed(TraceSourceType.TraceSourceOleTx, ex);
							}
						}
						catch (TransactionException ex2)
						{
							if (!(ex2.InnerException is COMException ex3))
							{
								throw;
							}
							if (ex3.ErrorCode != -2147168228 && ex3.ErrorCode != -2147168229)
							{
								throw;
							}
							resourceManagerShim = null;
							TransactionsEtwProvider log2 = TransactionsEtwProvider.Log;
							if (log2.IsEnabled())
							{
								log2.ExceptionConsumed(TraceSourceType.TraceSourceOleTx, ex2);
							}
						}
						finally
						{
							OletxTransactionManager.DtcTransactionManagerLock.ReleaseReaderLock();
						}
						Thread.MemoryBarrier();
						this.resourceManagerShim = resourceManagerShim;
					}
				}
			}
			return this.resourceManagerShim;
		}
		set
		{
			resourceManagerShim = value;
		}
	}

	internal OletxResourceManager(OletxTransactionManager transactionManager, Guid resourceManagerIdentifier)
	{
		resourceManagerShim = null;
		OletxTransactionManager = transactionManager;
		ResourceManagerIdentifier = resourceManagerIdentifier;
		EnlistmentHashtable = new Hashtable();
		ReenlistList = new ArrayList();
		ReenlistPendingList = new ArrayList();
		ReenlistThreadTimer = null;
		reenlistThread = null;
		RecoveryCompleteCalledByApplication = false;
	}

	internal bool CallProxyReenlistComplete()
	{
		bool result = false;
		if (RecoveryCompleteCalledByApplication)
		{
			try
			{
				ResourceManagerShim resourceManagerShim = ResourceManagerShim;
				if (resourceManagerShim != null)
				{
					resourceManagerShim.ReenlistComplete();
					result = true;
				}
			}
			catch (COMException ex)
			{
				if (ex.ErrorCode == -2147168228 || ex.ErrorCode == -2147168229)
				{
					result = false;
					TransactionsEtwProvider log = TransactionsEtwProvider.Log;
					if (log.IsEnabled())
					{
						log.ExceptionConsumed(TraceSourceType.TraceSourceOleTx, ex);
					}
				}
				else
				{
					if (ex.ErrorCode != -2147167996)
					{
						OletxTransactionManager.ProxyException(ex);
						throw;
					}
					result = true;
				}
			}
			finally
			{
				ResourceManagerShim resourceManagerShim = null;
			}
		}
		else
		{
			result = true;
		}
		return result;
	}

	internal void TMDownFromInternalRM(OletxTransactionManager oletxTM)
	{
		ResourceManagerShim = null;
		Hashtable hashtable;
		lock (EnlistmentHashtable.SyncRoot)
		{
			hashtable = (Hashtable)EnlistmentHashtable.Clone();
		}
		IDictionaryEnumerator enumerator = hashtable.GetEnumerator();
		while (enumerator.MoveNext())
		{
			(enumerator.Value as OletxEnlistment)?.TMDownFromInternalRM(oletxTM);
		}
	}

	public void TMDown()
	{
		StartReenlistThread();
	}

	internal OletxEnlistment EnlistDurable(OletxTransaction oletxTransaction, bool canDoSinglePhase, IEnlistmentNotificationInternal enlistmentNotification, EnlistmentOptions enlistmentOptions)
	{
		_ = Guid.Empty;
		bool flag = false;
		OletxEnlistment oletxEnlistment = new OletxEnlistment(canDoSinglePhase, enlistmentNotification, oletxTransaction.RealTransaction.TxGuid, enlistmentOptions, this, oletxTransaction);
		bool flag2 = false;
		try
		{
			if ((enlistmentOptions & EnlistmentOptions.EnlistDuringPrepareRequired) != EnlistmentOptions.None)
			{
				oletxTransaction.RealTransaction.IncrementUndecidedEnlistments();
				flag = true;
			}
			lock (oletxEnlistment)
			{
				try
				{
					ResourceManagerShim obj = ResourceManagerShim ?? throw TransactionManagerCommunicationException.Create(System.SR.TraceSourceOletx, null);
					if ((enlistmentOptions & EnlistmentOptions.EnlistDuringPrepareRequired) != EnlistmentOptions.None)
					{
						oletxTransaction.RealTransaction.TransactionShim.Phase0Enlist(oletxEnlistment, out var phase0EnlistmentShim);
						oletxEnlistment.Phase0EnlistmentShim = phase0EnlistmentShim;
					}
					obj.Enlist(oletxTransaction.RealTransaction.TransactionShim, oletxEnlistment, out var enlistmentShim);
					oletxEnlistment.EnlistmentShim = enlistmentShim;
				}
				catch (COMException ex)
				{
					if (ex.ErrorCode == -2147167999)
					{
						throw TransactionException.Create(System.SR.OletxTooManyEnlistments, ex, oletxEnlistment?.DistributedTxId ?? Guid.Empty);
					}
					OletxTransactionManager.ProxyException(ex);
					throw;
				}
			}
			flag2 = true;
			return oletxEnlistment;
		}
		finally
		{
			if ((!flag2 && (enlistmentOptions & EnlistmentOptions.EnlistDuringPrepareRequired) != 0) & flag)
			{
				oletxTransaction.RealTransaction.DecrementUndecidedEnlistments();
			}
		}
	}

	internal OletxEnlistment Reenlist(byte[] prepareInfo, IEnlistmentNotificationInternal enlistmentNotification)
	{
		OletxTransactionOutcome outcome = OletxTransactionOutcome.NotKnownYet;
		OletxTransactionStatus xactStatus = OletxTransactionStatus.OLETX_TRANSACTION_STATUS_NONE;
		if (prepareInfo == null)
		{
			throw new ArgumentException(System.SR.InvalidArgument, "prepareInfo");
		}
		if (new Guid(prepareInfo.AsSpan(16, 16)) != ResourceManagerIdentifier)
		{
			throw TransactionException.Create(System.SR.ResourceManagerIdDoesNotMatchRecoveryInformation, null);
		}
		try
		{
			(ResourceManagerShim ?? throw new COMException(System.SR.DtcTransactionManagerUnavailable, -2147168228)).Reenlist(prepareInfo, out outcome);
			if (OletxTransactionOutcome.Committed == outcome)
			{
				xactStatus = OletxTransactionStatus.OLETX_TRANSACTION_STATUS_COMMITTED;
			}
			else if (OletxTransactionOutcome.Aborted == outcome)
			{
				xactStatus = OletxTransactionStatus.OLETX_TRANSACTION_STATUS_ABORTED;
			}
			else
			{
				xactStatus = OletxTransactionStatus.OLETX_TRANSACTION_STATUS_PREPARED;
				StartReenlistThread();
			}
		}
		catch (COMException ex) when (ex.ErrorCode == -2147168228)
		{
			xactStatus = OletxTransactionStatus.OLETX_TRANSACTION_STATUS_PREPARED;
			ResourceManagerShim = null;
			StartReenlistThread();
			TransactionsEtwProvider log = TransactionsEtwProvider.Log;
			if (log.IsEnabled())
			{
				log.ExceptionConsumed(TraceSourceType.TraceSourceOleTx, ex);
			}
		}
		finally
		{
		}
		return new OletxEnlistment(enlistmentNotification, xactStatus, prepareInfo, this);
	}

	internal void RecoveryComplete()
	{
		Timer timer = null;
		RecoveryCompleteCalledByApplication = true;
		try
		{
			lock (ReenlistList)
			{
				lock (this)
				{
					if (ReenlistList.Count == 0 && ReenlistPendingList.Count == 0)
					{
						if (ReenlistThreadTimer != null)
						{
							timer = ReenlistThreadTimer;
							ReenlistThreadTimer = null;
						}
						if (!CallProxyReenlistComplete())
						{
							StartReenlistThread();
						}
					}
					else
					{
						StartReenlistThread();
					}
				}
			}
		}
		finally
		{
			timer?.Dispose();
		}
	}

	internal void StartReenlistThread()
	{
		lock (this)
		{
			if (ReenlistThreadTimer == null && reenlistThread == null)
			{
				ReenlistThreadTimer = new Timer(ReenlistThread, this, 10, -1);
			}
		}
	}

	internal void RemoveFromReenlistPending(OletxEnlistment enlistment)
	{
		lock (ReenlistList)
		{
			ReenlistPendingList.Remove(enlistment);
			lock (this)
			{
				if (ReenlistThreadTimer != null && ReenlistList.Count == 0 && ReenlistPendingList.Count == 0 && !ReenlistThreadTimer.Change(0, -1))
				{
					throw TransactionException.CreateInvalidOperationException(TraceSourceType.TraceSourceOleTx, System.SR.UnexpectedTimerFailure, null);
				}
			}
		}
	}

	internal void ReenlistThread(object state)
	{
		Timer timer = null;
		bool flag = false;
		OletxResourceManager oletxResourceManager = (OletxResourceManager)state;
		try
		{
			TransactionsEtwProvider log = TransactionsEtwProvider.Log;
			if (log.IsEnabled())
			{
				log.MethodEnter(TraceSourceType.TraceSourceOleTx, this, "OletxResourceManager.ReenlistThread");
			}
			ResourceManagerShim resourceManagerShim;
			lock (oletxResourceManager)
			{
				resourceManagerShim = oletxResourceManager.ResourceManagerShim;
				timer = oletxResourceManager.ReenlistThreadTimer;
				oletxResourceManager.ReenlistThreadTimer = null;
				oletxResourceManager.reenlistThread = Thread.CurrentThread;
			}
			if (resourceManagerShim != null)
			{
				int num;
				lock (oletxResourceManager.ReenlistList)
				{
					num = oletxResourceManager.ReenlistList.Count;
				}
				bool flag2 = false;
				while (!flag2 && num > 0 && resourceManagerShim != null)
				{
					OletxEnlistment oletxEnlistment;
					lock (oletxResourceManager.ReenlistList)
					{
						oletxEnlistment = null;
						num--;
						if (oletxResourceManager.ReenlistList.Count == 0)
						{
							flag2 = true;
						}
						else
						{
							oletxEnlistment = oletxResourceManager.ReenlistList[0] as OletxEnlistment;
							if (oletxEnlistment == null)
							{
								if (log.IsEnabled())
								{
									log.InternalError();
								}
								throw TransactionException.Create(System.SR.InternalError, null);
							}
							oletxResourceManager.ReenlistList.RemoveAt(0);
							lock (oletxEnlistment)
							{
								if (OletxEnlistment.OletxEnlistmentState.Done == oletxEnlistment.State)
								{
									oletxEnlistment = null;
								}
								else if (OletxEnlistment.OletxEnlistmentState.Prepared != oletxEnlistment.State)
								{
									oletxResourceManager.ReenlistList.Add(oletxEnlistment);
									oletxEnlistment = null;
								}
							}
						}
					}
					if (oletxEnlistment == null)
					{
						continue;
					}
					OletxTransactionOutcome outcome = OletxTransactionOutcome.NotKnownYet;
					try
					{
						if (oletxEnlistment.ProxyPrepareInfoByteArray == null)
						{
							if (log.IsEnabled())
							{
								log.InternalError();
							}
							throw TransactionException.Create(System.SR.InternalError, null);
						}
						resourceManagerShim.Reenlist(oletxEnlistment.ProxyPrepareInfoByteArray, out outcome);
						if (outcome == OletxTransactionOutcome.NotKnownYet)
						{
							lock (oletxEnlistment)
							{
								if (OletxEnlistment.OletxEnlistmentState.Done == oletxEnlistment.State)
								{
									oletxEnlistment = null;
								}
								else
								{
									lock (oletxResourceManager.ReenlistList)
									{
										oletxResourceManager.ReenlistList.Add(oletxEnlistment);
										oletxEnlistment = null;
									}
								}
							}
						}
					}
					catch (COMException ex) when (ex.ErrorCode == -2147168228)
					{
						if (log.IsEnabled())
						{
							log.ExceptionConsumed(TraceSourceType.TraceSourceOleTx, ex);
						}
						oletxResourceManager.ResourceManagerShim = null;
						resourceManagerShim = oletxResourceManager.ResourceManagerShim;
					}
					if (oletxEnlistment == null)
					{
						continue;
					}
					lock (oletxEnlistment)
					{
						if (OletxEnlistment.OletxEnlistmentState.Done == oletxEnlistment.State)
						{
							oletxEnlistment = null;
							continue;
						}
						lock (oletxResourceManager.ReenlistList)
						{
							oletxResourceManager.ReenlistPendingList.Add(oletxEnlistment);
						}
						switch (outcome)
						{
						case OletxTransactionOutcome.Committed:
							oletxEnlistment.State = OletxEnlistment.OletxEnlistmentState.Committing;
							if (log.IsEnabled())
							{
								log.EnlistmentStatus(TraceSourceType.TraceSourceOleTx, oletxEnlistment.EnlistmentTraceId, NotificationCall.Commit);
							}
							oletxEnlistment.EnlistmentNotification.Commit(oletxEnlistment);
							break;
						case OletxTransactionOutcome.Aborted:
							oletxEnlistment.State = OletxEnlistment.OletxEnlistmentState.Aborting;
							if (log.IsEnabled())
							{
								log.EnlistmentStatus(TraceSourceType.TraceSourceOleTx, oletxEnlistment.EnlistmentTraceId, NotificationCall.Rollback);
							}
							oletxEnlistment.EnlistmentNotification.Rollback(oletxEnlistment);
							break;
						default:
							if (log.IsEnabled())
							{
								log.InternalError();
							}
							throw TransactionException.Create(System.SR.InternalError, null);
						}
					}
				}
			}
			resourceManagerShim = null;
			lock (oletxResourceManager.ReenlistList)
			{
				lock (oletxResourceManager)
				{
					int num = oletxResourceManager.ReenlistList.Count;
					if (num <= 0 && oletxResourceManager.ReenlistPendingList.Count <= 0)
					{
						if (oletxResourceManager.CallProxyReenlistComplete())
						{
							flag = true;
						}
						else
						{
							oletxResourceManager.ReenlistThreadTimer = timer;
							if (!timer.Change(10000, -1))
							{
								throw TransactionException.CreateInvalidOperationException(TraceSourceType.TraceSourceLtm, System.SR.UnexpectedTimerFailure, null);
							}
						}
					}
					else
					{
						oletxResourceManager.ReenlistThreadTimer = timer;
						if (!timer.Change(10000, -1))
						{
							throw TransactionException.CreateInvalidOperationException(TraceSourceType.TraceSourceLtm, System.SR.UnexpectedTimerFailure, null);
						}
					}
					oletxResourceManager.reenlistThread = null;
				}
				if (log.IsEnabled())
				{
					log.MethodExit(TraceSourceType.TraceSourceOleTx, this, "OletxResourceManager.ReenlistThread");
				}
			}
		}
		finally
		{
			ResourceManagerShim resourceManagerShim = null;
			if (flag)
			{
				timer?.Dispose();
			}
		}
	}
}
