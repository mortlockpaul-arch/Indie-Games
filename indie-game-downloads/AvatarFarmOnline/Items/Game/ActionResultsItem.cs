using AvatarFarmOnline.Logic;
using AvatarFarmOnline.Logic.Stage;
using Microsoft.Xna.Framework;
using Quasar;
using Quasar.Behaviors;
using Quasar.Global;
using Quasar.Language;
using Quasar.Meshes.Text;

namespace AvatarFarmOnline.Items.Game;

internal class ActionResultsItem : RenderItem
{
	private class TextData
	{
		public TextMesh text;

		public RenderItem item;

		public long showTime;

		public Vector3 startPoint;
	}

	private TextData[] texts;

	private AvatarFarmOnline.Logic.Stage.Stage stage;

	private int nextText;

	public ActionResultsItem(AvatarFarmOnline.Logic.Stage.Stage stage)
	{
		this.stage = stage;
		stage.FarmData.OnActionPerformed += OnActionPerformed;
		stage.LocalPlayer.OnFailedAction += stage_OnFailedAction;
		texts = new TextData[9];
		for (int i = 0; i < 9; i++)
		{
			texts[i] = new TextData();
			texts[i].text = new TextMesh(BitmapFontManager.Fonts["Menu"], new TextDrawProperties(0.01f, HorizontalAlignment.Center), 25, useStringBuilder: true);
			texts[i].item = new RenderItem(texts[i].text);
			texts[i].item.addBehavior(new SphericalBillboardBehavior());
			texts[i].item.Visible = false;
			texts[i].showTime = 0L;
			addChild(texts[i].item);
		}
	}

	private void stage_OnFailedAction(AvatarFarmOnline.Logic.FailedActionReason obj)
	{
		string text;
		switch (obj)
		{
		default:
			return;
		case AvatarFarmOnline.Logic.FailedActionReason.ToolIsFull:
			text = "FAILED_TOOL_ALREADY_FULL".Translate();
			break;
		case AvatarFarmOnline.Logic.FailedActionReason.NotEnoughMoney:
			text = "FAILED_NOT_ENOUGH_MONEY".Translate();
			break;
		case AvatarFarmOnline.Logic.FailedActionReason.BuildingNotReady:
			text = "FAILED_BUILDING_NOT_READY".Translate();
			break;
		case AvatarFarmOnline.Logic.FailedActionReason.TreeNotReady:
			text = "FAILED_TREE_NOT_READY".Translate();
			break;
		case AvatarFarmOnline.Logic.FailedActionReason.LandNotPlowed:
			text = "FAILED_LAND_NOT_PLOWED".Translate();
			break;
		case AvatarFarmOnline.Logic.FailedActionReason.LandNotEmpty:
			text = "FAILED_LAND_NOT_EMPTY".Translate();
			break;
		case AvatarFarmOnline.Logic.FailedActionReason.PlantNotReady:
			text = "FAILED_PLANT_NOT_READY".Translate();
			break;
		case AvatarFarmOnline.Logic.FailedActionReason.AnimalHasEnoughFood:
			text = "FAILED_ANIMAL_HAS_ENOUGH_FOOD".Translate();
			break;
		case AvatarFarmOnline.Logic.FailedActionReason.CantPlow:
			text = "FAILED_CANT_PLOW".Translate();
			break;
		case AvatarFarmOnline.Logic.FailedActionReason.CantWater:
			text = "FAILED_CANT_WATER".Translate();
			break;
		case AvatarFarmOnline.Logic.FailedActionReason.NotEnoughSpace:
			text = "FAILED_NOT_ENOUGH_SPACE".Translate();
			break;
		case AvatarFarmOnline.Logic.FailedActionReason.AnimalNotReady:
			text = "FAILED_ANIMAL_NOT_READY".Translate();
			break;
		case AvatarFarmOnline.Logic.FailedActionReason.IncorrectSeason:
			text = "FAILED_INCORRECT_SEASON".Translate();
			break;
		case AvatarFarmOnline.Logic.FailedActionReason.BuildingNeeded:
			text = "FAILED_BUILDING_NEEDED".Translate();
			break;
		case AvatarFarmOnline.Logic.FailedActionReason.XpLevelNeeded:
			text = "FAILED_XPLEVEL_NEEDED".Translate();
			break;
		case AvatarFarmOnline.Logic.FailedActionReason.TileNotEmpty:
			text = "FAILED_TILE_NOT_EMPTY".Translate();
			break;
		case AvatarFarmOnline.Logic.FailedActionReason.NotPermitted:
			text = "FAILED_NOT_PERMITTED".Translate();
			break;
		case AvatarFarmOnline.Logic.FailedActionReason.Trialmode:
			text = "FAILED_TRIAL_MODE".Translate();
			break;
		}
		TextData textData = texts[nextText];
		TextMesh text2 = textData.text;
		nextText = (nextText + 1) % texts.Length;
		text2.Text = text;
		Vector3 startPoint = AvatarFarmOnline.Logic.GameGlobals.WorldPosition(stage.LocalPlayer.Position) + new Vector3(0f, 1.5f, 0f);
		textData.startPoint = startPoint;
		textData.showTime = Timer.DefaultTimer.TotalTime;
		textData.item.Visible = true;
	}

	private void OnActionPerformed(Vector2 tilePosition, int xp, AvatarFarmOnline.Logic.Money money)
	{
		TextData textData = texts[nextText];
		TextMesh text = textData.text;
		nextText = (nextText + 1) % texts.Length;
		text.StringBuilder.Length = 0;
		bool flag = false;
		if (xp != 0)
		{
			text.StringBuilder.AppendNumber(xp, AppendNumberOptions.PositiveSign | AppendNumberOptions.NumberGroup);
			text.StringBuilder.Append('\u0bbd');
			flag = true;
		}
		if (money.Amount != 0)
		{
			if (flag)
			{
				text.StringBuilder.Append("  ");
			}
			text.StringBuilder.AppendNumber(money.Amount, AppendNumberOptions.PositiveSign | AppendNumberOptions.NumberGroup);
			text.StringBuilder.Append(money.MoneyChar);
			flag = true;
		}
		Vector3 startPoint = AvatarFarmOnline.Logic.GameGlobals.WorldPosition(tilePosition) + new Vector3(0f, 1.5f, 0f);
		textData.startPoint = startPoint;
		textData.showTime = Timer.DefaultTimer.TotalTime;
		textData.item.Visible = true;
	}

	protected override void DoUpdate()
	{
		for (int i = 0; i < texts.Length; i++)
		{
			TextData textData = texts[i];
			if (!textData.item.Visible)
			{
				continue;
			}
			int num = (int)(Timer.DefaultTimer.TotalTime - textData.showTime);
			if (num > 2500)
			{
				textData.item.Visible = false;
				continue;
			}
			textData.item.Transform.Translation = textData.startPoint + new Vector3(0f, (float)num * 0.001f * 0.25f, 0f);
			float num2 = Vector3.Distance(textData.item.Transform.WorldTranslation, Scene.CurrentInstance.Camera.Transform.WorldTranslation);
			if (num < 1500)
			{
				textData.text.Alpha = GameMath.Damping(textData.text.Alpha, 1f, 0.96f);
			}
			else
			{
				textData.text.Alpha = GameMath.Damping(textData.text.Alpha, 0f, 0.94f);
			}
			switch (stage.LocalPlayer.CameraState)
			{
			default:
				textData.text.Scale = num2 * 0.001f;
				textData.item.Visible = true;
				break;
			case AvatarFarmOnline.Logic.Stage.LocalPlayer.CameraStates.World:
				textData.item.Visible = false;
				break;
			}
		}
		base.DoUpdate();
	}
}
