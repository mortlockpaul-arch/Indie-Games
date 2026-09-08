namespace Quasar.GameUtils.Logic.Mode.Campaign;

public class BaseCampaignPersistentGameData<T, U> : SimplePersistentGameData<T> where T : BaseCampaignGameSetup where U : class, IStage
{
	private ICampaignGameDataCreator<T, U> creator;

	public BaseCampaignPersistentGameData(T setup, ICampaignGameDataCreator<T, U> creator)
		: base(setup)
	{
		this.creator = creator;
	}

	protected override GameData DoPrepareNextRound()
	{
		return creator.CreateCampaignGameData(this);
	}
}
