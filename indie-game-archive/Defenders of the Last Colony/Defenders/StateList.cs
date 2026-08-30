namespace Defenders;

internal class StateList
{
	public EnemState state = EnemState.normal;

	public ushort next = 0;

	public StateList(EnemState state, ushort duration)
	{
		this.state = state;
		next = duration;
	}
}
