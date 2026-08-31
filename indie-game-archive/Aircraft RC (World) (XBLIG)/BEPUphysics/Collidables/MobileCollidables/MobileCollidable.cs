namespace BEPUphysics.Collidables.MobileCollidables;

/// <summary>
///  Superclass of all collidables which are capable of movement, and thus need bounding box updates every frame.
/// </summary>
public abstract class MobileCollidable : Collidable
{
	/// <summary>
	///  Updates the bounding box of the mobile collidable.
	/// </summary>
	/// <param name="dt">Timestep with which to update the bounding box.</param>
	public abstract void UpdateBoundingBox(float dt);
}
