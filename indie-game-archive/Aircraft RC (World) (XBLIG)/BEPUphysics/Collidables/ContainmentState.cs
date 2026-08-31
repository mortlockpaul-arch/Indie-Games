namespace BEPUphysics.Collidables;

/// <summary>
/// Stores flags regarding an object's degree of inclusion in a volume.
/// </summary>
public struct ContainmentState
{
	/// <summary>
	/// Whether or not the object is fully contained.
	/// </summary>
	public bool IsContained;

	/// <summary>
	/// Whether or not the object is partially or fully contained.
	/// </summary>
	public bool IsTouching;

	/// <summary>
	/// Whether or not the entity associated with this state has been refreshed during the last update.
	/// </summary>
	internal bool StaleState;

	/// <summary>
	/// Constructs a new ContainmentState.
	/// </summary>
	/// <param name="touching">Whether or not the object is partially or fully contained.</param>
	/// <param name="contained">Whether or not the object is fully contained.</param>
	public ContainmentState(bool touching, bool contained)
	{
		IsTouching = touching;
		IsContained = contained;
		StaleState = false;
	}

	/// <summary>
	/// Constructs a new ContainmentState.
	/// </summary>
	/// <param name="touching">Whether or not the object is partially or fully contained.</param>
	/// <param name="contained">Whether or not the object is fully contained.</param>
	/// <param name="stale">Whether or not the entity associated with this state has been refreshed in the previous update.</param>
	internal ContainmentState(bool touching, bool contained, bool stale)
	{
		IsTouching = touching;
		IsContained = contained;
		StaleState = stale;
	}
}
