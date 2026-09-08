using AvatarFarmOnline.Items.Game.HUD;
using AvatarFarmOnline.Logic.Mode;
using AvatarFarmOnline.Logic.Mode.Farm;
using AvatarFarmOnline.Logic.Stage;
using Quasar.Elements;
using Quasar.GameUtils.Logic.Mode;
using Quasar.Scenes;

namespace AvatarFarmOnline.Scenes;

internal class HUDScene : Scene2D
{
	public enum EFinishAction
	{
		Retry,
		Exit
	}

	public delegate void FinishConfirmation(EFinishAction action);

	public const uint GameEndLingerTime = 4000u;

	protected AvatarFarmOnline.Logic.Stage.Stage stage;

	private AvatarFarmOnline.Items.Game.HUD.LevelUpItem levelUp;

	public Camera StageCamera
	{
		set
		{
		}
	}

	public event FinishConfirmation OnFinishConfirm;

	public HUDScene(AvatarFarmOnline.Logic.Stage.Stage stage)
	{
		this.stage = stage;
		if (GameManager.GameMode == 0)
		{
			AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData farmPersistentGameData = GameManager.PersistentData as AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData;
			Add(new AvatarFarmOnline.Items.Game.HUD.FarmHUDItem(stage));
			Add(new AvatarFarmOnline.Items.Game.HUD.PlayerHUDItem(stage));
			Add(new AvatarFarmOnline.Items.Game.HUD.SeasonHUDItem(stage));
			Add(new AvatarFarmOnline.Items.Game.HUD.PlantHUDItem(stage));
			Add(new AvatarFarmOnline.Items.Game.HUD.ButtonsHUDItem(stage));
			Add(new AvatarFarmOnline.Items.Game.HUD.ActionLog(stage));
			Add(new AvatarFarmOnline.Items.Game.HUD.SavingMessage(stage));
			levelUp = new AvatarFarmOnline.Items.Game.HUD.LevelUpItem(stage);
			Add(levelUp);
			if (farmPersistentGameData.IsOnline)
			{
				Add(new AvatarFarmOnline.Items.Game.HUD.PlayerListItem(stage));
			}
		}
	}

	protected void InvokeFinishConfirm(EFinishAction action)
	{
		if (OnFinishConfirm != null)
		{
			OnFinishConfirm(action);
		}
	}

	private void OnConfirm(EFinishAction action)
	{
		InvokeFinishConfirm(action);
	}

	public void ShowResults()
	{
		AvatarFarmOnline.Logic.Mode.GameMode gameMode = (AvatarFarmOnline.Logic.Mode.GameMode)GameManager.GameMode;
		_ = 0;
	}

	public override void Dispose()
	{
		if (levelUp != null)
		{
			levelUp.Dispose();
			levelUp = null;
		}
		base.Dispose();
	}
}
