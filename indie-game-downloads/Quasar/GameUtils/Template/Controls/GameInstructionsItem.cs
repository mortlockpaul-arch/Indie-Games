using Microsoft.Xna.Framework.Graphics;

namespace Quasar.GameUtils.Template.Controls;

public class GameInstructionsItem
{
	private Texture2D image;

	private string text;

	public Texture2D Image => image;

	public bool HasImage => image != null;

	public string Text => text;

	public GameInstructionsItem(string text, Texture2D image)
	{
		this.image = image;
		this.text = text;
	}
}
