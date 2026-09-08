using System;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Quasar.GUI;
using Quasar.GUI.Controls;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Player;
using Quasar.GameUtils.Template;
using Quasar.GameUtils.Template.Controls;
using Quasar.Global;
using Quasar.Input;
using Quasar.Language;

namespace Quasar.GameUtils.Sections;

public class HowToPlaySection : GUISection
{
	private const string MainGroup = "MainGroup";

	private ButtonInstructions instructions;

	protected HowToPlay howToPlay;

	private int previousSectionId;

	public event Action OnInstructionChange;

	public HowToPlaySection(int previousSectionId, string template)
		: base(8, new Layout("", LanguageManager.Texts["HOW_TO_PLAY_TITLE"], template))
	{
		this.previousSectionId = previousSectionId;
		base.Layout.OnCancel += LayoutCancel;
		howToPlay = new HowToPlay(base.Layout);
		howToPlay.Size = new Vector2(Engine.GUIWidth * 0.6f + 192f, Engine.GUIHeight * 0.5f);
		howToPlay.OnCancelled += OnCancel;
		howToPlay.OnItemChanged += OnChange;
		base.Layout.AddControl(howToPlay);
		instructions = new ButtonInstructions(base.Layout);
		base.Layout.AddControl(instructions);
		bool flag = true;
		int num = 0;
		while (flag)
		{
			string name = "HOW_TO_PLAY_" + num;
			if (LanguageManager.Texts.TryGetValue(name, out var text))
			{
				flag = true;
				string text2 = num.ToString();
				if (!DirectoryManager.FileExists("Textures/HowToPlay/" + text2))
				{
					text2 = "";
				}
				howToPlay.AddItem(text, text2);
				num++;
			}
			else
			{
				flag = false;
			}
		}
		OnChange(playSound: false);
	}

	private HowToPlay.HowToPlayItem ParseHowToPlayItem(XElement xe)
	{
		string text = xe.GetAttribute("description").Translate();
		string attribute = xe.GetAttribute("image");
		return new HowToPlay.HowToPlayItem(text, attribute);
	}

	protected void OnChange()
	{
		OnChange(playSound: true);
	}

	protected void OnChange(bool playSound)
	{
		instructions.ClearInstructions();
		if (howToPlay.CurrentIndex < howToPlay.ItemsCount - 1)
		{
			instructions.AddInstructions(InputManager.MenuInputCodes.Interact, "NEXT".Translate());
		}
		instructions.AddInstructions(InputManager.MenuInputCodes.Cancel, "BACK".Translate());
		if (playSound)
		{
			GameTemplate.MoveAudio.Start();
		}
		if (OnInstructionChange != null)
		{
			OnInstructionChange();
		}
	}

	private void OnCancel()
	{
		Back();
	}

	public bool LayoutCancel(Layout layout, PlayerIndex whoPressed)
	{
		Back();
		return true;
	}

	private void Back()
	{
		((BaseGame)Engine.Game).NextGameSectionId = previousSectionId;
	}

	public override void MainLoop()
	{
		Quasar.GameUtils.Player.Player.SetPresence(GamerPresenceMode.TutorialMode);
		base.MainLoop();
	}
}
