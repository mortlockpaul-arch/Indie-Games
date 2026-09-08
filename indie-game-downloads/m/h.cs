namespace m;

internal interface h
{
	bool IsActive { get; }

	_6 PositionUpdater { get; set; }

	void PreUpdatePosition(float dt);
}
