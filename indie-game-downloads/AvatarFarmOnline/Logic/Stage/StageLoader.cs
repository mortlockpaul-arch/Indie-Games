using Quasar.GameUtils.Logic.Mode;

namespace AvatarFarmOnline.Logic.Stage;

internal class StageLoader : IStageLoader
{
	private static AvatarFarmOnline.Logic.Stage.StageLoader instance;

	private AvatarFarmOnline.Logic.Stage.StageParameters stageParameters;

	public static AvatarFarmOnline.Logic.Stage.StageLoader Instance => instance;

	public AvatarFarmOnline.Logic.Stage.StageParameters StageParameters
	{
		get
		{
			return stageParameters;
		}
		set
		{
			stageParameters = value;
		}
	}

	static StageLoader()
	{
		instance = new AvatarFarmOnline.Logic.Stage.StageLoader();
	}

	public IStage LoadStage()
	{
		AvatarFarmOnline.Logic.Stage.FarmData farmData = stageParameters.FarmHeader.LoadFarm();
		return new AvatarFarmOnline.Logic.Stage.Stage(stageParameters, farmData);
	}
}
