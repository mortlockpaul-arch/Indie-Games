using System.Diagnostics;

namespace System.Threading.Tasks.Dataflow;

/// <summary>Provides options used to configure the processing performed by dataflow blocks.</summary>
[DebuggerDisplay("TaskScheduler = {TaskScheduler}, MaxMessagesPerTask = {MaxMessagesPerTask}, BoundedCapacity = {BoundedCapacity}")]
public class DataflowBlockOptions
{
	/// <summary>A constant used to specify an unlimited quantity for <see cref="T:System.Threading.Tasks.Dataflow.DataflowBlockOptions" /> members that provide an upper bound. This field is constant.</summary>
	public const int Unbounded = -1;

	private TaskScheduler _taskScheduler = System.Threading.Tasks.TaskScheduler.Default;

	private CancellationToken _cancellationToken = CancellationToken.None;

	private int _maxMessagesPerTask = -1;

	private int _boundedCapacity = -1;

	private string _nameFormat = "{0} Id={1}";

	private bool _ensureOrdered = true;

	internal static readonly DataflowBlockOptions Default = new DataflowBlockOptions();

	/// <summary>Gets or sets the <see cref="T:System.Threading.Tasks.TaskScheduler" /> to use for scheduling tasks.</summary>
	/// <returns>The task scheduler.</returns>
	public TaskScheduler TaskScheduler
	{
		get
		{
			return _taskScheduler;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			_taskScheduler = value;
		}
	}

	/// <summary>Gets or sets the <see cref="T:System.Threading.CancellationToken" /> to monitor for cancellation requests.</summary>
	/// <returns>The token.</returns>
	public CancellationToken CancellationToken
	{
		get
		{
			return _cancellationToken;
		}
		set
		{
			_cancellationToken = value;
		}
	}

	/// <summary>Gets or sets the maximum number of messages that may be processed per task.</summary>
	/// <returns>The maximum number of messages.</returns>
	public int MaxMessagesPerTask
	{
		get
		{
			return _maxMessagesPerTask;
		}
		set
		{
			if (value < 1 && value != -1)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			_maxMessagesPerTask = value;
		}
	}

	internal int ActualMaxMessagesPerTask
	{
		get
		{
			if (_maxMessagesPerTask != -1)
			{
				return _maxMessagesPerTask;
			}
			return int.MaxValue;
		}
	}

	/// <summary>Gets or sets the maximum number of messages that may be buffered by the block.</summary>
	/// <returns>The maximum number of messages.</returns>
	public int BoundedCapacity
	{
		get
		{
			return _boundedCapacity;
		}
		set
		{
			if (value < 1 && value != -1)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			_boundedCapacity = value;
		}
	}

	/// <summary>Gets or sets the format string to use when a block is queried for its name.</summary>
	/// <returns>The format string to use when a block is queried for its name.</returns>
	public string NameFormat
	{
		get
		{
			return _nameFormat;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			_nameFormat = value;
		}
	}

	public bool EnsureOrdered
	{
		get
		{
			return _ensureOrdered;
		}
		set
		{
			_ensureOrdered = value;
		}
	}

	internal DataflowBlockOptions DefaultOrClone()
	{
		if (this != Default)
		{
			return new DataflowBlockOptions
			{
				TaskScheduler = TaskScheduler,
				CancellationToken = CancellationToken,
				MaxMessagesPerTask = MaxMessagesPerTask,
				BoundedCapacity = BoundedCapacity,
				NameFormat = NameFormat,
				EnsureOrdered = EnsureOrdered
			};
		}
		return this;
	}

	/// <summary>Initializes a new <see cref="T:System.Threading.Tasks.Dataflow.DataflowBlockOptions" />.</summary>
	public DataflowBlockOptions()
	{
	}
}
