using AvatarFarmOnline.Scores;
using AvatarFarmOnline.Template.Controls;
using Microsoft.Xna.Framework;
using Quasar;
using Quasar.Audios;
using Quasar.GUI;
using Quasar.GUI.Controls;
using Quasar.GUI.Controls.GroupControls;
using Quasar.GUI.Elements;
using Quasar.GameUtils.Template;
using Quasar.GameUtils.Template.Controls;
using Quasar.GameUtils.XBLIG.CrossPromotion.Template;
using Quasar.Global;
using Quasar.Meshes.Text;

namespace AvatarFarmOnline.Template;

internal class ExtendedGameTemplate : GameTemplate
{
	public static string Template = "Game";

	private static BitmapFont menuFont = null;

	private static BitmapFont hudTitleFont = null;

	private static BitmapFont hudFont = null;

	public static Vector3 HUDTitleColor = GameMath.RGBToVector(200, 20, 20);

	public static Vector3 HUDChangedColor = GameMath.RGBToVector(200, 200, 20);

	public static Vector3 HUDColor = GameMath.RGBToVector(byte.MaxValue, byte.MaxValue, byte.MaxValue);

	public static BitmapFont MenuFont
	{
		get
		{
			if (menuFont == null)
			{
				menuFont = BitmapFontManager.Fonts["Menu"];
			}
			return menuFont;
		}
	}

	public static BitmapFont HUDTitleFont
	{
		get
		{
			if (hudTitleFont == null)
			{
				hudTitleFont = BitmapFontManager.Fonts["Menu"];
			}
			return hudTitleFont;
		}
	}

	public static BitmapFont HUDFont
	{
		get
		{
			if (hudFont == null)
			{
				hudFont = BitmapFontManager.Fonts["HUD"];
			}
			return hudFont;
		}
	}

	public override Vector2 GetButtonSize(Button button)
	{
		return new Vector2(420f, 64f);
	}

	public ExtendedGameTemplate()
	{
		GameTemplate.standardFont = "Standard";
		GameTemplate.HUDRectangleMargins[3] = 17f;
		GameTemplate.HUDRectanglePatternSize[0] = (GameTemplate.HUDRectanglePatternSize[1] = 128f);
		GameTemplate.interactionAudio = new PooledAudio(SoundEffectManager.SoundEffects[GameTemplate.interactAudioName], 3);
		GameTemplate.selectorAudio = new PooledAudio(SoundEffectManager.SoundEffects[GameTemplate.selectorAudioName], 5);
		GameTemplate.cancelAudio = new PooledAudio(SoundEffectManager.SoundEffects["MenuBack"], 3);
		PooledAudio pooledAudio = new PooledAudio(SoundEffectManager.SoundEffects["MenuMove"], 3);
		GameTemplate.moveAudio = pooledAudio;
		GameTemplate.DialogTitleColor = GameMath.RGBToVector(byte.MaxValue, 186, 20);
		GameTemplate.DialogTitleColorDisabled = GameMath.RGBToVector(60, 205, 60);
		GroupConfiguration gc = new GroupConfiguration
		{
			Loop = true,
			NavigateMode = GroupNavigateMode.TopDown,
			Position = new Vector2(Engine.GUIWidth * 0.385f, Engine.GUIHeight * 0.2f),
			Margin = -5f,
			HorizontalMode = GroupHorizontalLayoutMode.Right
		};
		GroupConfigurationManager.Config.AddConfig("MainMenuGroup", gc);
		gc = new GroupConfiguration
		{
			Loop = true,
			NavigateMode = GroupNavigateMode.TopDown,
			Position = new Vector2(Engine.GUIWidth * 0.375f, Engine.GUIHeight * 0.25f),
			Margin = -5f,
			HorizontalMode = GroupHorizontalLayoutMode.Right
		};
		GroupConfigurationManager.Config.AddConfig("SettingsGroup", gc);
		gc = new GroupConfiguration
		{
			Loop = true,
			NavigateMode = GroupNavigateMode.TopDown,
			Margin = 10f
		};
		GroupConfigurationManager.Config.AddConfig("MainGroup", gc);
	}

	public override GroupControlElement CreateGroupControl(GroupControl control, Group group)
	{
		return control.ControlType switch
		{
			"Selector" => new MenuSelectorItem((Selector)control), 
			"Button" => new MenuButtonItem((Button)control), 
			_ => base.CreateGroupControl(control, group), 
		};
	}

	public override DialogNode CreateDialog(Layout layout, Dialog dialog)
	{
		return new GameDialogNode(dialog);
	}

	public override GroupNode CreateGroup(Group group)
	{
		GroupNode groupNode = base.CreateGroup(group);
		if (group.Id == "MainMenuGroup" || group.Id == "SettingsGroup")
		{
			groupNode.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(-5f));
		}
		return groupNode;
	}

	public override Element CreateControl(Control control)
	{
		return control.ControlType switch
		{
			"StageLoader" => null, 
			"FarmList" => new AvatarFarmOnline.Template.Controls.FarmListItem((AvatarFarmOnline.Template.Controls.FarmList)control), 
			"FindGames" => new AvatarFarmOnline.Template.Controls.FindGamesItem((AvatarFarmOnline.Template.Controls.FindGames)control), 
			"Shop" => new AvatarFarmOnline.Template.Controls.ShopItem((AvatarFarmOnline.Template.Controls.Shop)control), 
			"HighScores" => new AvatarFarmOnline.Template.Controls.ProHighScoresNode((ProHighscores<AvatarFarmOnline.Scores.GameHighscore>)control), 
			"CrossPromotionList" => new CrossPromotionListNode((CrossPromotionList)(object)control), 
			"JoinInvited" => new AvatarFarmOnline.Template.Controls.JoinGameStatusItem((AvatarFarmOnline.Template.Controls.JoinGameStatus)control), 
			"PressAToStart" => new AvatarFarmOnline.Template.Controls.PressAToStartItem((PressAToStart)control), 
			"HowToPlay" => new AvatarFarmOnline.Template.Controls.HowToPlayItem((HowToPlay)control), 
			_ => base.CreateControl(control), 
		};
	}

	protected override void CreateElement(Layout layout, LayoutElement element)
	{
		element.addChild(new LayoutTitle(layout));
	}
}
