using Quasar.Meshes.Text;

namespace Quasar.Items._2D;

public class BasicTextBox : RenderItem
{
	private TextBoxMesh textBoxMesh;

	public TextBoxMesh TextBoxMesh => textBoxMesh;

	public string Text
	{
		get
		{
			return textBoxMesh.Text;
		}
		set
		{
			textBoxMesh.Text = value;
		}
	}

	public BasicTextBox(BitmapFont font, TextBoxDrawProperties properties)
	{
		textBoxMesh = new TextBoxMesh(font, properties);
		addMesh(textBoxMesh);
	}
}
