namespace System.Linq;

internal sealed class SingleLinkedNode<TSource>
{
	public TSource Item { get; }

	public SingleLinkedNode<TSource> Linked { get; }

	public SingleLinkedNode(TSource item)
	{
		Item = item;
	}

	private SingleLinkedNode(SingleLinkedNode<TSource> linked, TSource item)
	{
		Linked = linked;
		Item = item;
	}

	public SingleLinkedNode<TSource> Add(TSource item)
	{
		return new SingleLinkedNode<TSource>(this, item);
	}

	public SingleLinkedNode<TSource> GetNode(int index)
	{
		SingleLinkedNode<TSource> singleLinkedNode = this;
		while (index > 0)
		{
			singleLinkedNode = singleLinkedNode.Linked;
			index--;
		}
		return singleLinkedNode;
	}

	public TSource[] ToArray(int count)
	{
		TSource[] array = new TSource[count];
		FillReversed(array);
		return array;
	}

	public void Fill(Span<TSource> span)
	{
		int num = 0;
		for (SingleLinkedNode<TSource> singleLinkedNode = this; singleLinkedNode != null; singleLinkedNode = singleLinkedNode.Linked)
		{
			span[num] = singleLinkedNode.Item;
			num++;
		}
	}

	public void FillReversed(Span<TSource> span)
	{
		int num = span.Length;
		for (SingleLinkedNode<TSource> singleLinkedNode = this; singleLinkedNode != null; singleLinkedNode = singleLinkedNode.Linked)
		{
			num--;
			span[num] = singleLinkedNode.Item;
		}
	}
}
