using GKEngine.Entities;

namespace Game.Dialogs;

public class DialogIconMenuOption
{
	private string path;

	public Dialog.DialogDelegate action;

	public Sprite sprite;

	public DialogIconMenu menu;

	public object data;

	public bool selected;

	public DialogIconMenuOption(DialogIconMenu oMenu, string xPath, Dialog.DialogDelegate oAction)
	{
		menu = oMenu;
		path = xPath;
		action = oAction;
	}

	public DialogIconMenuOption(DialogIconMenu oMenu, string xPath, Dialog.DialogDelegate oAction, object oData)
	{
		menu = oMenu;
		path = xPath;
		action = oAction;
		data = oData;
	}

	public void Load(DialogManager oManager)
	{
		sprite = new Sprite(menu.manager.spriteManager, path);
	}

	public void Dispose()
	{
		if (sprite != null)
		{
			sprite.Dispose();
		}
		action = null;
		data = null;
		menu = null;
	}

	public void SetState(bool xSelected)
	{
	}
}
