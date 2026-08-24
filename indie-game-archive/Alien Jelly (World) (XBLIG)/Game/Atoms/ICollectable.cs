namespace Game.Atoms;

public interface ICollectable
{
	bool collected { get; }

	int type { get; }

	Atom atom { get; }

	void Collect();
}
