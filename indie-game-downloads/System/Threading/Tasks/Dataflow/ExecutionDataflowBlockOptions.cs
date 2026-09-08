using System.Diagnostics;

namespace System.Threading.Tasks.Dataflow;

/// <summary>Provides options used to configure the processing performed by dataflow blocks that process each message through the invocation of a user-provided delegate. These are dataflow blocks such as <see cref="T:System.Threading.Tasks.Dataflow.ActionBlock`1" /> and <see cref="T:System.Threading.Tasks.Dataflow.TransformBlock`2" />.</summary>
[DebuggerDisplay("TaskScheduler = {TaskScheduler}, MaxMessagesPerTask = {MaxMessagesPerTask}, BoundedCapacity = {BoundedCapacity}, MaxDegreeOfParallelism = {MaxDegreeOfParallelism}")]
public class ExecutionDataflowBlockOptions : DataflowBlockOptions
{
	internal new static readonly ExecutionDataflowBlockOptions Default = new ExecutionDataflowBlockOptions();

	private int _maxDegreeOfParallelism = 1;

	private bool _singleProducerConstrained;

	/// <summary>Gets the maximum number of messages that may be processed by the block concurrently.</summary>
	/// <returns>The maximum number of messages.</returns>
	public int MaxDegreeOfParallelism
	{
		get
		{
			return _maxDegreeOfParallelism;
		}
		set
		{
			if (value < 1 && value != -1)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			_maxDegreeOfParallelism = value;
		}
	}

	/// <summary>Gets whether code using the dataflow block is constrained to one producer at a time.</summary>
	/// <returns>Returns <see cref="T:System.Boolean" />.</returns>
	public bool SingleProducerConstrained
	{
		get
		{
			return _singleProducerConstrained;
		}
		set
		{
			_singleProducerConstrained = value;
		}
	}

	internal int ActualMaxDegreeOfParallelism
	{
		get
		{
			if (_maxDegreeOfParallelism != -1)
			{
				return _maxDegreeOfParallelism;
			}
			return int.MaxValue;
		}
	}

	internal bool SupportsParallelExecution
	{
		get
		{
			if (_maxDegreeOfParallelism != -1)
			{
				return _maxDegreeOfParallelism > 1;
			}
			return true;
		}
	}

	internal new ExecutionDataflowBlockOptions DefaultOrClone()
	{
		if (this != Default)
		{
			return new ExecutionDataflowBlockOptions
			{
				TaskScheduler = base.TaskScheduler,
				CancellationToken = base.CancellationToken,
				MaxMessagesPerTask = base.MaxMessagesPerTask,
				BoundedCapacity = base.BoundedCapacity,
				NameFormat = base.NameFormat,
				EnsureOrdered = base.EnsureOrdered,
				MaxDegreeOfParallelism = MaxDegreeOfParallelism,
				SingleProducerConstrained = SingleProducerConstrained
			};
		}
		return this;
	}

	/// <summary>Initializes a new <see cref="T:System.Threading.Tasks.Dataflow.ExecutionDataflowBlockOptions" />.</summary>
	public ExecutionDataflowBlockOptions()
	{
	}
}
