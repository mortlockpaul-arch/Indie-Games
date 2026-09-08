namespace Quasar.GUI.Elements;

public abstract class TextInputNode : Element
{
	protected TextInputDialog dialog;

	public TextInputNode(TextInputDialog dialog)
	{
		this.dialog = dialog;
	}
}
