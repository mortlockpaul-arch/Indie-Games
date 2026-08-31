namespace BEPUphysics.OtherSpaceStages;

/// <summary>
///  Defines an object which can create deferred events.
/// </summary>
public interface IDeferredEventCreator
{
	/// <summary>
	///  Gets or sets the deferred event dispatcher that owns this creator.
	/// </summary>
	DeferredEventDispatcher DeferredEventDispatcher { get; set; }

	/// <summary>
	///  Gets or sets the activity state of this creator.
	/// </summary>
	bool IsActive { get; set; }

	/// <summary>
	/// Gets or sets the number of child deferred event creators.
	/// </summary>
	int ChildDeferredEventCreators { get; set; }

	/// <summary>
	///  Dispatches the events created by this creator.
	/// </summary>
	void DispatchEvents();
}
