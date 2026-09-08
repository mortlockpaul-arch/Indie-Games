using Quasar.Meshes.Text;

namespace Quasar.Items._2D;

public class BasicText : RenderItem
{
	private TextMesh textMesh;

	public TextMesh TextMesh => textMesh;

	public string Text
	{
		set
		{
			textMesh.Text = value;
		}
	}

	public BasicText(BitmapFont font)
	{
		textMesh = new TextMesh(font);
		addMesh(textMesh);
	}

	public BasicText(BitmapFont font, TextDrawProperties properties)
	{
		textMesh = new TextMesh(font, properties);
		addMesh(textMesh);
	}

	public BasicText(BitmapFont font, TextDrawProperties properties, int maxBufferSize)
	{
		textMesh = new TextMesh(font, properties, maxBufferSize, useStringBuilder: false);
		addMesh(textMesh);
	}
}
