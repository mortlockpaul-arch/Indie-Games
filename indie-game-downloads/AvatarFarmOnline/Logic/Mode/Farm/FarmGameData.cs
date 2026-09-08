using AvatarFarmOnline.Logic.Mode.Farm.Online;
using AvatarFarmOnline.Logic.Stage;
using AvatarFarmOnline.Scores;
using Quasar.GameUtils.Logic.Mode;
using Quasar.GameUtils.Network;

namespace AvatarFarmOnline.Logic.Mode.Farm;

internal class FarmGameData : BaseGameData<AvatarFarmOnline.Logic.Stage.Stage>
{
	public delegate void WaveHandler(int wave, int time, int parTime, int timeScore, int flawlessScore, int waveEndScore, float scoreFactor, int totalScore);

	protected new AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData persistentData;

	private long tickTime;

	public AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData PersistentData => persistentData;

	public FarmGameData(AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData persistentData)
		: base((PersistentGameData)persistentData, (IStageLoader)AvatarFarmOnline.Logic.Stage.StageLoader.Instance)
	{
		this.persistentData = persistentData;
		modeDescription = "";
		modeShortDescription = "";
		modeTitle = "";
	}

	public override void PrepareNextStage()
	{
		AvatarFarmOnline.Logic.Stage.StageParameters stageParameters = new AvatarFarmOnline.Logic.Stage.StageParameters();
		stageParameters.FarmHeader = persistentData.CurrentFarm;
		if (!persistentData.IsOnline)
		{
			if (stageParameters.FarmHeader is AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader localFarmHeader)
			{
				stageParameters.AddSelection(new AvatarFarmOnline.Logic.Stage.PlayerSpawnData(new AvatarFarmOnline.Logic.Stage.LocalPlayerSelection(persistentData.Setup.PlayerIndex, persistentData.FarmManager.PlayerExperience, 0), localFarmHeader.PlayerPosition, persistentData.FarmManager.PlayerExperience.Level));
			}
		}
		else
		{
			ILocalNetworkGamer localGamer = persistentData.Online.LocalGamer;
			if (persistentData.Online.IsHost)
			{
				if (stageParameters.FarmHeader is AvatarFarmOnline.Logic.Mode.Farm.LocalFarmHeader localFarmHeader2)
				{
					stageParameters.AddSelection(new AvatarFarmOnline.Logic.Stage.PlayerSpawnData(new AvatarFarmOnline.Logic.Stage.LocalPlayerSelection(localGamer.SignedInGamer.PlayerIndex, persistentData.FarmManager.PlayerExperience, localGamer.Id), localFarmHeader2.PlayerPosition, persistentData.FarmManager.PlayerExperience.Level));
				}
			}
			else if (persistentData.CurrentFarm is AvatarFarmOnline.Logic.Mode.Farm.RemoteFarmHeader remoteFarmHeader)
			{
				stageParameters.AddSelection(new AvatarFarmOnline.Logic.Stage.PlayerSpawnData(new AvatarFarmOnline.Logic.Stage.LocalPlayerSelection(localGamer.SignedInGamer.PlayerIndex, persistentData.FarmManager.PlayerExperience, localGamer.Id), remoteFarmHeader.FarmData.CenterPosition, persistentData.FarmManager.PlayerExperience.Level));
				foreach (AvatarFarmOnline.Logic.Mode.Farm.Online.RemotePlayerSpawnData remotePlayer in remoteFarmHeader.RemotePlayers)
				{
					INetworkGamer gamer = persistentData.Online.GetGamer(remotePlayer.PlayerId);
					stageParameters.AddSelection(new AvatarFarmOnline.Logic.Stage.PlayerSpawnData(new AvatarFarmOnline.Logic.Stage.NetworkPlayerSelection(gamer), remotePlayer.Position, remotePlayer.Level));
				}
			}
		}
		AvatarFarmOnline.Logic.Stage.StageLoader.Instance.StageParameters = stageParameters;
	}

	public override void StartGame()
	{
		base.StartGame();
		if (persistentData.IsOnline)
		{
			persistentData.Online.StageLoaded();
		}
		base.Stage.StartGame();
	}

	public override void Update()
	{
		base.Update();
		if (gameState != GameState.Playing || (persistentData.IsOnline && !persistentData.Online.IsHost))
		{
			return;
		}
		tickTime += base.Stage.Timer.LastInterval;
		if (tickTime >= 1000)
		{
			tickTime -= 1000L;
			base.Stage.Tick();
			if (persistentData.IsOnline)
			{
				persistentData.Online.SendTick();
			}
		}
	}

	public override void FinishStage()
	{
		base.Stage.FinishStage();
		if (persistentData.Setup.GameMode == AvatarFarmOnline.Logic.Mode.GameMode.Farm)
		{
			AvatarFarmOnline.Logic.Mode.Farm.FarmGameSetup setup = persistentData.Setup;
			if (base.Stage.FarmData.IsLocal)
			{
				AvatarFarmOnline.Scores.GameScoreManager.Instance.SetNewScore(setup.PlayerIndex, (uint)base.Stage.FarmData.PlayerData.Level, base.Stage.FarmData.PlayerData.Coins, base.Stage.FarmData.PlayerData.Cash, base.Stage.FarmData.WorldTicks);
			}
		}
		base.FinishStage();
	}
}
