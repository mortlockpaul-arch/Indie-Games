namespace Quasar.Script.Events;

internal class OneShotEvent : ScriptEvent
{
	private bool executed;

	public OneShotEvent(ScriptInstance instance)
		: base(instance)
	{
	}

	public override void Update()
	{
		if (!executed)
		{
			Execute();
			executed = true;
		}
	}
}
