namespace Quasar.GameUtils.Logic.Mode.Campaign;

public class BaseCampaignGameData<T, U> : BaseGameData<U> where T : BaseCampaignGameSetup where U : class, IStage
{
	protected new ICampaignStageLoader loader;

	protected new BaseCampaignPersistentGameData<T, U> persistentData;

	public int CurrentLevel
	{
		get
		{
			T setup = persistentData.Setup;
			return setup.Level;
		}
	}

	public BaseCampaignGameData(BaseCampaignPersistentGameData<T, U> persistentData, ICampaignStageLoader loader)
		: base((PersistentGameData)persistentData, (IStageLoader)loader)
	{
		this.loader = loader;
		this.persistentData = persistentData;
	}

	public override void PrepareNextStage()
	{
		ICampaignStageLoader campaignStageLoader = loader;
		T setup = persistentData.Setup;
		campaignStageLoader.Level = setup.Level;
	}
}
