using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.VisualBasic;

[EditorBrowsable(EditorBrowsableState.Never)]
internal sealed class ForEachEnum : IEnumerator, IDisposable
{
	internal enum AdjustIndexType
	{
		Insert,
		Remove
	}

	private bool mDisposed;

	private Collection mCollectionObject;

	private Collection.Node mCurrent;

	private Collection.Node mNext;

	private bool mAtBeginning;

	internal WeakReference WeakRef;

	public object Current
	{
		get
		{
			if (mCurrent == null)
			{
				return null;
			}
			return mCurrent.m_Value;
		}
	}

	private void Dispose()
	{
		if (!mDisposed)
		{
			mCollectionObject.RemoveIterator(WeakRef);
			mDisposed = true;
		}
		mCurrent = null;
		mNext = null;
	}

	void IDisposable.Dispose()
	{
		//ILSpy generated this explicit interface implementation from .override directive in Dispose
		this.Dispose();
	}

	public ForEachEnum(Collection coll)
	{
		mCollectionObject = coll;
		Reset();
	}

	public bool MoveNext()
	{
		if (mDisposed)
		{
			return false;
		}
		if (mAtBeginning)
		{
			mAtBeginning = false;
			mNext = mCollectionObject.GetFirstListNode();
		}
		if (mNext == null)
		{
			Dispose();
			return false;
		}
		mCurrent = mNext;
		if (mCurrent != null)
		{
			mNext = mCurrent.m_Next;
			return true;
		}
		Dispose();
		return false;
	}

	bool IEnumerator.MoveNext()
	{
		//ILSpy generated this explicit interface implementation from .override directive in MoveNext
		return this.MoveNext();
	}

	public void Reset()
	{
		if (mDisposed)
		{
			mCollectionObject.AddIterator(WeakRef);
			mDisposed = false;
		}
		mCurrent = null;
		mNext = null;
		mAtBeginning = true;
	}

	void IEnumerator.Reset()
	{
		//ILSpy generated this explicit interface implementation from .override directive in Reset
		this.Reset();
	}

	public void Adjust(Collection.Node Node, AdjustIndexType Type)
	{
		if (Node == null || mDisposed)
		{
			return;
		}
		switch (Type)
		{
		case AdjustIndexType.Insert:
			if (mCurrent != null && Node == mCurrent.m_Next)
			{
				mNext = Node;
			}
			break;
		case AdjustIndexType.Remove:
			if (Node != mCurrent && Node == mNext)
			{
				mNext = mNext.m_Next;
			}
			break;
		}
	}

	internal void AdjustOnListCleared()
	{
		mNext = null;
	}
}
