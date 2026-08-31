namespace DPSF;

/// <summary>
/// The possible Modes the Automatic Memory Manager can be in
/// </summary>
public enum AutoMemoryManagerModes
{
	/// <summary>
	/// Do not use the Automatic Memory Manager. The Number Of Particles Allocated In Memory will not be changed dynamically at run-time.
	/// This is the best option if performance is critical, as it can be expensive to allocate and release large chunks of memory at 
	/// run-time. If using this mode you should be sure that the Number Of Particles Allocated In Memory is large enough to accommodate 
	/// the particle system, but small enough that it does not waste large amounts of memory.
	/// </summary>
	Disabled,
	/// <summary>
	/// Allow the Automatic Memory Manager to allocate more memory when needed, and reduce it when not needed.
	/// </summary>
	IncreaseAndDecrease,
	/// <summary>
	/// Only allow the Automatic Memory Manager to allocate more memory when needed (cannot reduce space).
	/// </summary>
	IncreaseOnly,
	/// <summary>
	/// Only allow the Automatic Memory Manager to reduce the amount of memory allocated when it is not needed (cannot increase space).
	/// </summary>
	DecreaseOnly
}
