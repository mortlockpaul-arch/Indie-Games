namespace Quasar.GameUtils.Logic.Mode.Campaign;

public abstract class BaseCampaignGameSetup : GameSetup
{
	private int level;

	public int Level
	{
		get
		{
			return level;
		}
		set
		{
			level = value;
		}
	}

	public BaseCampaignGameSetup(int gameMode)
		: base(gameMode)
	{
	}
}
