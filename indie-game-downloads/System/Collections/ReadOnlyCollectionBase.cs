using System.Runtime.CompilerServices;

namespace System.Collections;

public abstract class ReadOnlyCollectionBase : ICollection, IEnumerable
{
	[CompilerGenerated]
	private ArrayList _003CInnerList_003Ek__BackingField;

	protected ArrayList InnerList => _003CInnerList_003Ek__BackingField ?? (_003CInnerList_003Ek__BackingField = new ArrayList());

	public virtual int Count => InnerList.Count;

	bool ICollection.IsSynchronized => InnerList.IsSynchronized;

	object ICollection.SyncRoot => InnerList.SyncRoot;

	void ICollection.CopyTo(Array array, int index)
	{
		InnerList.CopyTo(array, index);
	}

	public virtual IEnumerator GetEnumerator()
	{
		return InnerList.GetEnumerator();
	}
}
