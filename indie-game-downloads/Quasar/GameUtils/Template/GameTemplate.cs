using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Quasar.Audios;
using Quasar.GUI;
using Quasar.GUI.Controls;
using Quasar.GUI.Controls.GroupControls;
using Quasar.GUI.Elements;
using Quasar.GameUtils.Awards;
using Quasar.GameUtils.Template.Controls;
using Quasar.Global;
using Quasar.Meshes.Text;

namespace Quasar.GameUtils.Template;

public abstract class GameTemplate : Quasar.GUI.Template
{
	public static Vector3 MenuTextFocused;

	public static Vector3 MenuTextNormal;

	public static float[] HUDRectangleMargins;

	public static float[] HUDRectanglePatternSize;

	public static float[] HUDDetailRectangleMargins;

	public static Vector3 DialogTitleColor;

	public static Vector3 DialogTitleColorDisabled;

	public static Vector3 TitleColor;

	protected static string standardFont;

	private static BitmapFont font;

	protected static string titleFont;

	private static BitmapFont tFont;

	protected static string moveAudioName;

	protected static Quasar.Audio moveAudio;

	protected static string interactAudioName;

	protected static Quasar.Audio interactionAudio;

	protected static string selectorAudioName;

	protected static Quasar.Audio selectorAudio;

	protected static string layoutCancelAudioName;

	protected static Quasar.Audio cancelAudio;

	protected static GameTemplate instance;

	public static BitmapFont StandardFont
	{
		get
		{
			if (font == null)
			{
				font = BitmapFontManager.Fonts[standardFont];
			}
			return font;
		}
	}

	public static BitmapFont TitleFont
	{
		get
		{
			if (tFont == null)
			{
				tFont = BitmapFontManager.Fonts[titleFont];
			}
			return tFont;
		}
	}

	public static Quasar.Audio MoveAudio
	{
		get
		{
			if (moveAudio == null)
			{
				moveAudio = new PooledAudio(Engine.ContentManager.Load<SoundEffect>("Sounds/" + moveAudioName), 5);
			}
			return moveAudio;
		}
	}

	public static Quasar.Audio InteractionAudio
	{
		get
		{
			if (interactionAudio == null)
			{
				interactionAudio = new PooledAudio(Engine.ContentManager.Load<SoundEffect>("Sounds/" + interactAudioName), 3);
			}
			return interactionAudio;
		}
	}

	public static Quasar.Audio SelectorAudio
	{
		get
		{
			if (selectorAudio == null)
			{
				selectorAudio = new PooledAudio(Engine.ContentManager.Load<SoundEffect>("Sounds/" + selectorAudioName), 5);
			}
			return selectorAudio;
		}
	}

	public static Quasar.Audio LayoutCancelAudio
	{
		get
		{
			if (cancelAudio == null)
			{
				cancelAudio = new PooledAudio(Engine.ContentManager.Load<SoundEffect>("Sounds/" + layoutCancelAudioName), 3);
			}
			return cancelAudio;
		}
	}

	public static GameTemplate Instance => instance;

	static GameTemplate()
	{
		MenuTextFocused = new Vector3(0f, 0.565f, 1f);
		MenuTextNormal = new Vector3(1f, 1f, 1f);
		HUDRectangleMargins = new float[4] { 17f, 17f, 17f, 22f };
		HUDRectanglePatternSize = new float[2] { 80f, 80f };
		HUDDetailRectangleMargins = new float[4] { 17f, 17f, 17f, 22f };
		DialogTitleColor = new Vector3(0.6f, 1f, 0.26f);
		DialogTitleColorDisabled = new Vector3(1f, 0.48f, 0.195f);
		TitleColor = Vector3.One;
		standardFont = "Negotiate";
		font = null;
		titleFont = "Title";
		tFont = null;
		moveAudioName = "MenuMove";
		moveAudio = null;
		interactAudioName = "MenuInteraction";
		interactionAudio = null;
		selectorAudioName = "MenuMove";
		selectorAudio = null;
		layoutCancelAudioName = "MenuInteraction";
		cancelAudio = null;
		instance = null;
		Engine.RegisterDisposeHandler(OnEngineDispose);
	}

	protected GameTemplate()
	{
		instance = this;
		ControlCreatorManager.Instance.addFactory("PlayerSelect", PlayerSelectCreator.Instance);
	}

	public override GroupControlElement CreateGroupControl(GroupControl control, Group group)
	{
		return control.ControlType switch
		{
			"Button" => new ButtonItem((Button)control), 
			"Selector" => new SelectorItem((Selector)control), 
			_ => null, 
		};
	}

	public override DialogNode CreateDialog(Layout layout, Dialog dialog)
	{
		return new GameDialogNode(dialog);
	}

	public override TextInputNode CreateTextInputDialog(Layout layout, TextInputDialog dialog)
	{
		return new GameTextInputNode(dialog);
	}

	public override GroupNode CreateGroup(Group group)
	{
		GroupNode groupNode = new GameGroupNode(group);
		group.OnCancelled += OnGroupCancelled;
		foreach (GroupControl control in group.Controls)
		{
			groupNode.addChild(CreateGroupControl(control, group));
		}
		string id;
		if ((id = group.Id) != null && id == "MainGroup")
		{
			groupNode.Transform.Translation = new Vector3(0f, Engine.GUIHeight * 0.175f, 0f);
		}
		return groupNode;
	}

	protected void OnGroupCancelled(Group group, PlayerIndex whoPressed)
	{
		LayoutCancelAudio.Start();
	}

	public override Element CreateControl(Control control)
	{
		switch (control.ControlType)
		{
		case "Image":
			return new Quasar.GameUtils.Template.Controls.ImageNode((Image)control);
		case "Credits":
			((Credits)control).OnCancelled += OnControlCancelled;
			return new Quasar.GameUtils.Template.Controls.CreditsNode((Credits)control);
		case "GameInstructions":
			((GameInstructions)control).OnCancelled += OnControlCancelled;
			return new Quasar.GameUtils.Template.Controls.GameInstructionsNode((GameInstructions)control);
		case "Description":
			return new Quasar.GameUtils.Template.DescriptionItem((Description)control);
		case "Message":
			return new Quasar.GameUtils.Template.MessageItem((Message)control);
		case "ButtonInstructions":
			return new ButtonInstructionsNode((ButtonInstructions)control);
		case "Awards":
			((Quasar.GameUtils.Template.Controls.Awards)control).OnCancelled += OnControlCancelled;
			return new Quasar.GameUtils.Template.Controls.AwardsNode((Quasar.GameUtils.Template.Controls.Awards)control);
		case "Stats":
			((Quasar.GameUtils.Template.Controls.Stats)control).OnCancelled += OnControlCancelled;
			return new Quasar.GameUtils.Template.Controls.StatsNode((Quasar.GameUtils.Template.Controls.Stats)control);
		case "PlayerSelect":
			((PlayerSelect)control).OnCancelled += OnControlCancelled;
			((PlayerSelect)control).OnAccepted += OnControlCancelled;
			return new PlayerSelectNode((PlayerSelect)control);
		case "DLC":
			((DLC)control).OnCancelled += OnControlCancelled;
			return new Quasar.GameUtils.Template.Controls.DLCNode((DLC)control);
		case "ClickButton":
			return new ClickButtonNode((ClickButton)control);
		case "Label":
			return new LabelNode((Label)control);
		case "HowToPlay":
			((HowToPlay)control).OnCancelled += OnControlCancelled;
			return new Quasar.GameUtils.Template.Controls.HowToPlayNode((HowToPlay)control);
		case "PressAToStart":
			return new PressAToStartItem((PressAToStart)control);
		default:
			return base.CreateControl(control);
		}
	}

	protected void OnControlCancelled()
	{
		LayoutCancelAudio.Start();
	}

	protected virtual void CreateElement(Layout layout, LayoutElement element)
	{
		element.insertChild(0, new Quasar.GameUtils.Template.LayoutBackground());
		element.addChild(new LayoutTitle(layout));
	}

	public override LayoutElement CreateElement(Layout layout)
	{
		layout.OnCancelled += OnLayoutCancelled;
		LayoutElement layoutElement = base.CreateElement(layout);
		CreateElement(layout, layoutElement);
		return layoutElement;
	}

	private void OnLayoutCancelled(Layout layout, PlayerIndex whoPressed)
	{
		LayoutCancelAudio.Start();
	}

	public virtual AwardNotificationNode CreateAwardNode()
	{
		return new Quasar.GameUtils.Awards.BaseAwardNotificationNode();
	}

	private static void OnEngineDispose()
	{
		if (cancelAudio != null)
		{
			cancelAudio.Dispose();
			cancelAudio = null;
		}
		if (selectorAudio != null)
		{
			selectorAudio.Dispose();
			selectorAudio = null;
		}
		if (interactionAudio != null)
		{
			interactionAudio.Dispose();
			interactionAudio = null;
		}
		if (moveAudio != null)
		{
			moveAudio.Dispose();
			moveAudio = null;
		}
	}
}
