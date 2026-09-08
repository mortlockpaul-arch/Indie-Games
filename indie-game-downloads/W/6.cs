namespace W;

internal interface _6
{
	_0006 ForceUpdater { get; set; }

	bool IsDynamic { get; }

	bool IsActive { get; }

	void UpdateForForces(float dt);
}
