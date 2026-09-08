namespace Quasar.GameUtils.Logic.Mode.Campaign;

public interface ICampaignGameDataCreator<T, U> where T : BaseCampaignGameSetup where U : class, IStage
{
	BaseCampaignGameData<T, U> CreateCampaignGameData(BaseCampaignPersistentGameData<T, U> persistentData);
}
