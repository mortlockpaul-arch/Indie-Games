namespace Game.Atoms;

public class AtomTrigger
{
	public delegate bool TriggerDelegate(object oTriggerer);

	private Atom atom;

	public TriggerDelegate triggered;

	public AtomTrigger(Atom oAtom, TriggerDelegate oTriggered)
	{
		atom = oAtom;
		triggered = oTriggered;
	}
}
