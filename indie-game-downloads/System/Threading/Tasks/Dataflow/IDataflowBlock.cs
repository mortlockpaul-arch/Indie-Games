namespace System.Threading.Tasks.Dataflow;

/// <summary>Represents a dataflow block.</summary>
public interface IDataflowBlock
{
	/// <summary>Gets a <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation and completion of the dataflow block.</summary>
	/// <returns>The task.</returns>
	Task Completion { get; }

	/// <summary>Signals to the <see cref="T:System.Threading.Tasks.Dataflow.IDataflowBlock" /> that it should not accept nor produce any more messages nor consume any more postponed messages.</summary>
	void Complete();

	/// <summary>Causes the <see cref="T:System.Threading.Tasks.Dataflow.IDataflowBlock" /> to complete in a <see cref="F:System.Threading.Tasks.TaskStatus.Faulted" /> state.</summary>
	/// <param name="exception">The <see cref="T:System.Exception" /> that caused the faulting.</param>
	/// <exception cref="T:System.ArgumentNullException">The <paramref name="exception" /> is null.</exception>
	void Fault(Exception exception);
}
