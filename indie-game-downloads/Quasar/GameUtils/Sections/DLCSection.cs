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

public class DLCSection : GUISection
{
	private const string MainGroup = "MainGroup";

	private ButtonInstructions instructions;

	protected DLC dlc;

	private int previousSectionId;

	public DLCSection(int previousSectionId, string template)
		: base(10, new Layout("", LanguageManager.Texts["DLC_TITLE"], template))
	{
		this.previousSectionId = previousSectionId;
		base.Layout.OnCancel += LayoutCancel;
		dlc = new DLC(3, base.Layout);
		dlc.OnCancel += CreditsCancel;
		dlc.OnChange += OnChange;
		base.Layout.AddControl(dlc);
		instructions = new ButtonInstructions(base.Layout);
		base.Layout.AddControl(instructions);
		if (DirectoryManager.FileExists("DLC"))
		{
			XmlSource xmlSource = Engine.ContentManager.Load<XmlSource>("DLC");
			XDocument xDocument = XDocument.Parse(xmlSource.XmlCode);
			foreach (XElement item2 in xDocument.Root.Elements())
			{
				DLC.DLCItem item = ParseDLCItem(item2);
				dlc.AddItem(item);
			}
		}
		OnChange();
	}

	private DLC.DLCItem ParseDLCItem(XElement xe)
	{
		string attribute = xe.GetAttribute("title");
		string attribute2 = xe.GetAttribute("description");
		int targetSales = xe.ParseIntAttribute("target_sales");
		bool unlocked = xe.ParseBoolAttribute("unlocked");
		return new DLC.DLCItem(attribute, attribute2, targetSales, unlocked);
	}

	protected void OnChange()
	{
		instructions.ClearInstructions();
		if (dlc.CurrentPage < dlc.NumPages - 1)
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
