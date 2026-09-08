namespace Quasar.GameUtils.Logic.Mode;

public abstract class BaseGameData<T> : GameData where T : class, IStage
{
	protected IStageLoader loader;

	public new T Stage => base.Stage as T;

	public BaseGameData(PersistentGameData persistentData, IStageLoader stageLoader)
		: base(persistentData)
	{
		loader = stageLoader;
	}

	protected override IStage DoLoadStage()
	{
		return loader.LoadStage();
	}
}
