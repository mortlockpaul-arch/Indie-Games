using AvatarFarmOnline.Logic;
using AvatarFarmOnline.Logic.Mode.Farm;
using AvatarFarmOnline.Logic.Stage;
using Microsoft.Xna.Framework;
using Quasar;
using Quasar.Audios;
using Quasar.Global;
using Quasar.Language;
using Quasar.Meshes.Text;

namespace AvatarFarmOnline.Items.Game.HUD;

internal class LevelUpItem : RenderItem
{
	private TextMesh text;

	private SimpleAudio levelUpAudio;

	private AvatarFarmOnline.Logic.Stage.Stage stage;

	private AvatarFarmOnline.Logic.Mode.Farm.PlayerExperience playerExperience;

	private long showTime;

	public LevelUpItem(AvatarFarmOnline.Logic.Stage.Stage stage)
	{
		Visible = false;
		this.stage = stage;
		stage.FarmData.PlayerData.OnLevelUp += PlayerData_OnLevelUp;
		playerExperience = stage.LocalPlayer.PlayerSelection.PlayerExperience;
		playerExperience.OnLevelUp += PlayerExperience_OnLevelUp;
		text = new TextMesh(BitmapFontManager.Fonts["Menu"], new TextDrawProperties(1.5f, HorizontalAlignment.Center), 40, useStringBuilder: true);
		addMesh(text);
		levelUpAudio = new SimpleAudio(SoundEffectManager.SoundEffects["LevelUp"]);
	}

	private void PlayerExperience_OnLevelUp(AvatarFarmOnline.Logic.Mode.Farm.PlayerExperience obj)
	{
		showTime = Timer.DefaultTimer.TotalTime;
		text.StringBuilder.Length = 0;
		text.StringBuilder.Append("LEVEL_UP".Translate());
		levelUpAudio.Start();
		text.Alpha = 0f;
		Visible = true;
	}

	public override void Dispose()
	{
		base.Dispose();
		if (levelUpAudio != null)
		{
			levelUpAudio.Dispose();
			levelUpAudio = null;
		}
		if (playerExperience != null)
		{
			playerExperience.OnLevelUp -= PlayerExperience_OnLevelUp;
			playerExperience = null;
		}
	}

	private void PlayerData_OnLevelUp(AvatarFarmOnline.Logic.Stage.PlayerData obj, int coins, int cash)
	{
		showTime = Timer.DefaultTimer.TotalTime;
		text.StringBuilder.Length = 0;
		if (AvatarFarmOnline.Logic.GameGlobals.FarmSize(obj.Level).X > stage.FarmData.FarmSize.X)
		{
			text.StringBuilder.Append("FARM_EXTENDED".Translate());
		}
		else
		{
			text.StringBuilder.Append("FARM_LEVEL_UP".Translate());
		}
		text.StringBuilder.Append(" +");
		text.StringBuilder.AppendNumber(coins);
		text.StringBuilder.Append(AvatarFarmOnline.Logic.Money.CoinChar);
		text.StringBuilder.Append(" +");
		text.StringBuilder.AppendNumber(cash);
		text.StringBuilder.Append(AvatarFarmOnline.Logic.Money.CashChar);
		levelUpAudio.Start();
		text.Alpha = 0f;
		Visible = true;
	}

	protected override void DoUpdate()
	{
		if (Visible)
		{
			int num = (int)(Timer.DefaultTimer.TotalTime - showTime);
			if (num > 3000)
			{
				Visible = false;
			}
			else
			{
				if (num < 2000)
				{
					text.Alpha = GameMath.Damping(text.Alpha, 1f, 0.96f);
				}
				else
				{
					text.Alpha = GameMath.Damping(text.Alpha, 0f, 0.94f);
				}
				Transform.Translation = new Vector3(0f, -100f + (float)num * 0.001f * 100f, 0f);
			}
		}
		base.DoUpdate();
	}
}
