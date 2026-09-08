using System.Diagnostics;

namespace System.Threading.Tasks.Dataflow;

/// <summary>Provides options used to configure a link between dataflow blocks.</summary>
[DebuggerDisplay("PropagateCompletion = {PropagateCompletion}, MaxMessages = {MaxMessages}, Append = {Append}")]
public class DataflowLinkOptions
{
	private bool _propagateCompletion;

	private int _maxNumberOfMessages = -1;

	private bool _append = true;

	internal static readonly DataflowLinkOptions Default = new DataflowLinkOptions();

	internal static readonly DataflowLinkOptions UnlinkAfterOneAndPropagateCompletion = new DataflowLinkOptions
	{
		MaxMessages = 1,
		PropagateCompletion = true
	};

	/// <summary>Gets or sets whether the linked target will have completion and faulting notification propagated to it automatically.</summary>
	/// <returns>Returns <see cref="T:System.Boolean" />.</returns>
	public bool PropagateCompletion
	{
		get
		{
			return _propagateCompletion;
		}
		set
		{
			_propagateCompletion = value;
		}
	}

	/// <summary>Gets or sets the maximum number of messages that may be consumed across the link.</summary>
	/// <returns>Returns <see cref="T:System.Int32" />.</returns>
	public int MaxMessages
	{
		get
		{
			return _maxNumberOfMessages;
		}
		set
		{
			if (value < 1 && value != -1)
			{
				throw new ArgumentOutOfRangeException("value");
			}
			_maxNumberOfMessages = value;
		}
	}

	/// <summary>Gets or sets whether the link should be appended to the source’s list of links, or whether it should be prepended.</summary>
	/// <returns>Returns <see cref="T:System.Boolean" />.</returns>
	public bool Append
	{
		get
		{
			return _append;
		}
		set
		{
			_append = value;
		}
	}

	/// <summary>Initializes the <see cref="T:System.Threading.Tasks.Dataflow.DataflowLinkOptions" />.</summary>
	public DataflowLinkOptions()
	{
	}
}
