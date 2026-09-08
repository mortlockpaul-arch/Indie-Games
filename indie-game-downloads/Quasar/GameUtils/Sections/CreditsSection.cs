using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Quasar.ContentPipeline;
using Quasar.GUI;
using Quasar.GUI.Controls;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Player;
using Quasar.GameUtils.Template.Controls;
using Quasar.Global;
using Quasar.Input;
using Quasar.Language;

namespace Quasar.GameUtils.Sections;

public class CreditsSection : GUISection
{
	private const string MainGroup = "MainGroup";

	private ButtonInstructions instructions;

	protected Credits credits;

	private int previousSectionId;

	public CreditsSection(int previousSectionId, string template)
		: base(4, new Layout("", LanguageManager.Texts["CREDITS_TITLE"], template))
	{
		this.previousSectionId = previousSectionId;
		base.Layout.OnCancel += LayoutCancel;
		credits = new Credits(3, base.Layout);
		credits.OnCancel += CreditsCancel;
		credits.OnChange += OnChange;
		base.Layout.AddControl(credits);
		instructions = new ButtonInstructions(base.Layout);
		base.Layout.AddControl(instructions);
		if (DirectoryManager.FileExists("Credits"))
		{
			XmlSource xmlSource = Engine.ContentManager.Load<XmlSource>("Credits");
			XDocument xDocument = XDocument.Parse(xmlSource.XmlCode);
			foreach (XElement item in xDocument.Root.Elements())
			{
				CreditsItem ci = ParseCreditsItem(item);
				credits.AddItem(ci);
			}
		}
		OnChange();
	}

	private CreditsItem ParseCreditsItem(XElement xe)
	{
		string attribute = xe.GetAttribute("name");
		string attribute2 = xe.GetAttribute("texture");
		if (xe.HasAttribute("gamertag"))
		{
			string attribute3 = xe.GetAttribute("gamertag");
			string attribute4 = xe.GetAttribute("functions");
			CreditsFunctions function = ParseFunctions(attribute4);
			return new CreditsItem(attribute, attribute3, function, attribute2);
		}
		string attribute5 = xe.GetAttribute("description");
		return new CreditsItem(attribute, attribute5, attribute2);
	}

	private CreditsFunctions ParseFunctions(string functions)
	{
		string[] array = functions.Split(',');
		CreditsFunctions creditsFunctions = (CreditsFunctions)0;
		string[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			switch (array2[i])
			{
			case "Betatest":
				creditsFunctions |= CreditsFunctions.Betatest;
				break;
			case "Gameplay":
				creditsFunctions |= CreditsFunctions.Gameplay;
				break;
			case "Graphics":
				creditsFunctions |= CreditsFunctions.Graphics;
				break;
			case "Modelling":
				creditsFunctions |= CreditsFunctions.Modelling;
				break;
			case "Music":
				creditsFunctions |= CreditsFunctions.Music;
				break;
			case "Programming":
				creditsFunctions |= CreditsFunctions.Programming;
				break;
			case "Sound":
				creditsFunctions |= CreditsFunctions.Sound;
				break;
			case "Voice":
				creditsFunctions |= CreditsFunctions.Voice;
				break;
			}
		}
		return creditsFunctions;
	}

	protected void OnChange()
	{
		instructions.ClearInstructions();
		if (credits.CurrentPage < credits.NumPages - 1)
		{
			instructions.AddInstructions(InputManager.MenuInputCodes.Interact, "NEXT_PAGE".Translate());
		}
		instructions.AddInstructions(InputManager.MenuInputCodes.Cancel, "BACK".Translate());
	}

	private bool CreditsCancel()
	{
		Back();
		return true;
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
		Quasar.GameUtils.Player.Player.SetPresence(GamerPresenceMode.WatchingCredits);
		base.MainLoop();
	}
}
