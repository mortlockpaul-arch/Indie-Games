using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace System.Threading.Tasks.Dataflow;

/// <summary>Provides a container of data attributes for passing between dataflow blocks.</summary>
[DebuggerDisplay("Id = {Id}")]
public readonly struct DataflowMessageHeader : IEquatable<DataflowMessageHeader>
{
	private readonly long _id;

	/// <summary>Gets the validity of the message.</summary>
	/// <returns>true if the ID of the message is different from 0. false if the ID of the message is 0.</returns>
	public bool IsValid => _id != 0;

	/// <summary>Gets the ID of the message within the source.</summary>
	/// <returns>The ID contained in the <see cref="T:System.Threading.Tasks.Dataflow.DataflowMessageHeader" /> instance.</returns>
	public long Id => _id;

	/// <summary>Initializes a new <see cref="T:System.Threading.Tasks.Dataflow.DataflowMessageHeader" /> with the specified attributes.</summary>
	/// <param name="id">The ID of the message. Must be unique within the originating source block. It does not need to be globally unique.</param>
	public DataflowMessageHeader(long id)
	{
		if (id == 0L)
		{
			throw new ArgumentException(System.SR.Argument_InvalidMessageId, "id");
		}
		_id = id;
	}

	/// <summary>Checks two <see cref="T:System.Threading.Tasks.Dataflow.DataflowMessageHeader" /> instances for equality by ID without boxing.</summary>
	/// <returns>true if the instances are equal; otherwise, false.</returns>
	/// <param name="other">Another <see cref="T:System.Threading.Tasks.Dataflow.DataflowMessageHeader" /> instance.</param>
	public bool Equals(DataflowMessageHeader other)
	{
		return this == other;
	}

	/// <summary>Checks boxed <see cref="T:System.Threading.Tasks.Dataflow.DataflowMessageHeader" /> instances for equality by ID.</summary>
	/// <returns>true if the instances are equal; otherwise, false.</returns>
	/// <param name="obj">A boxed <see cref="T:System.Threading.Tasks.Dataflow.DataflowMessageHeader" /> instance.</param>
	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is DataflowMessageHeader)
		{
			return this == (DataflowMessageHeader)obj;
		}
		return false;
	}

	/// <summary>Generates a hash code for the <see cref="T:System.Threading.Tasks.Dataflow.DataflowMessageHeader" /> instance.</summary>
	/// <returns>The hash code.</returns>
	public override int GetHashCode()
	{
		return (int)Id;
	}

	/// <summary>Checks two <see cref="T:System.Threading.Tasks.Dataflow.DataflowMessageHeader" /> instances for equality by ID.</summary>
	/// <returns>true if the instances are equal; otherwise, false.</returns>
	/// <param name="left">A <see cref="T:System.Threading.Tasks.Dataflow.DataflowMessageHeader" /> instance.</param>
	/// <param name="right">A <see cref="T:System.Threading.Tasks.Dataflow.DataflowMessageHeader" /> instance.</param>
	public static bool operator ==(DataflowMessageHeader left, DataflowMessageHeader right)
	{
		return left.Id == right.Id;
	}

	/// <summary>Checks two <see cref="T:System.Threading.Tasks.Dataflow.DataflowMessageHeader" /> instances for non-equality by ID.</summary>
	/// <returns>true if the instances are not equal; otherwise, false.</returns>
	/// <param name="left">A <see cref="T:System.Threading.Tasks.Dataflow.DataflowMessageHeader" /> instance.</param>
	/// <param name="right">A <see cref="T:System.Threading.Tasks.Dataflow.DataflowMessageHeader" /> instance.</param>
	public static bool operator !=(DataflowMessageHeader left, DataflowMessageHeader right)
	{
		return left.Id != right.Id;
	}
}
