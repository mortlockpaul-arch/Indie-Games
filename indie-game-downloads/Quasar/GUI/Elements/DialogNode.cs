namespace Quasar.GUI.Elements;

public abstract class DialogNode : Element
{
	private Dialog dialog;

	public DialogNode(Dialog dialog)
	{
		this.dialog = dialog;
	}
}
