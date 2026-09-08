namespace Quasar.GameUtils.Logic.Mode;

public abstract class SimplePersistentGameData<T> : PersistentGameData where T : GameSetup
{
	protected new T setup;

	public new T Setup => setup;

	public SimplePersistentGameData(T setup)
		: base(setup)
	{
		this.setup = setup;
	}
}
