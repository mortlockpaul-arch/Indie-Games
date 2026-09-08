using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.Threading.Tasks.Dataflow;

/// <summary>Represents a dataflow block that supports receiving messages without linking.</summary>
/// <typeparam name="TOutput">Specifies the type of data supplied by the <see cref="T:System.Threading.Tasks.Dataflow.IReceivableSourceBlock`1" />.</typeparam>
public interface IReceivableSourceBlock<TOutput> : ISourceBlock<TOutput>, IDataflowBlock
{
	/// <summary>Attempts to synchronously receive an available output item from the<see cref="T:System.Threading.Tasks.Dataflow.IReceivableSourceBlock`1" />.</summary>
	/// <returns>true if an item could be received; otherwise, false.</returns>
	/// <param name="filter">The predicate value must successfully pass in order for it to be received. <paramref name="filter" /> may be null, in which case all items will pass.</param>
	/// <param name="item">The item received from the source.</param>
	bool TryReceive(Predicate<TOutput>? filter, [MaybeNullWhen(false)] out TOutput item);

	/// <summary>Attempts to synchronously receive all available items from the <see cref="T:System.Threading.Tasks.Dataflow.IReceivableSourceBlock`1" />.</summary>
	/// <returns>true if one or more items could be received; otherwise, false.</returns>
	/// <param name="items">The items received from the source.</param>
	bool TryReceiveAll([NotNullWhen(true)] out IList<TOutput>? items);
}
