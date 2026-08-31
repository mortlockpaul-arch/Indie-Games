namespace BEPUphysics.PositionUpdating;

/// <summary>
///  Update modes for position updateables.
/// </summary>
public enum PositionUpdateMode : byte
{
	/// <summary>
	/// Updates position discretely regardless of its collision pairs.
	/// </summary>
	Discrete,
	/// <summary>
	/// Updates position discretely in isolation; when a Continuous object collides with it,
	/// its position update will be bounded by the time of impact.
	/// </summary>
	Passive,
	/// <summary>
	/// Updates position continuously.  Continuous objects will integrate up to their earliest collision time.
	/// </summary>
	Continuous
}
