namespace Quasar.GUI;

public class LayoutElement : Element
{
	private Layout layout;

	public Layout Layout => layout;

	public LayoutElement(Layout layout)
	{
		this.layout = layout;
	}
}
