namespace r;

internal interface h
{
	a Space { get; set; }

	object Tag { get; set; }

	void OnAdditionToSpace(a newSpace);

	void OnRemovalFromSpace(a oldSpace);
}
