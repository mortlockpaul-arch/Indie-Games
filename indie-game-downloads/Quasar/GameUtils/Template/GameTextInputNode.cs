using Microsoft.Xna.Framework;
using Quasar.GUI;
using Quasar.GUI.Elements;
using Quasar.GameUtils.Meshes;
using Quasar.Input;
using Quasar.Language;
using Quasar.Meshes.Text;

namespace Quasar.GameUtils.Template;

public class GameTextInputNode : TextInputNode
{
	private const float WIDTH = 550f;

	private const float INFO_HEIGHT = 200f;

	private const float HEIGHT = 400f;

	private const float INPUT_HEIGHT = 80f;

	private const float TOP_HEIGHT = 48f;

	private const float BOTTOM_HEIGHT = 64f;

	private const float TOP_MARGIN = 0f;

	private const float BORDER = 94f;

	private const float MARGIN = 22f;

	private RenderItem item;

	private TextBoxMesh currentText;

	public GameTextInputNode(TextInputDialog dialog)
		: base(dialog)
	{
		item = new RenderItem();
		HUDRectangle m = new HUDRectangle(new Vector2(550f, 400f));
		item.addMesh(m);
		TextBoxMesh textBoxMesh = new TextBoxMesh(GameTemplate.StandardFont, new TextBoxDrawProperties(new Vector2(0f, 154f), new Vector2(506f, 48f), 1f, HorizontalAlignment.Left, VerticalAlignment.Top), 256, useStringBuilder: false);
		textBoxMesh.Text = dialog.Title;
		textBoxMesh.FirstMaterial.Diffuse = GameTemplate.DialogTitleColor;
		item.addMesh(textBoxMesh);
		HUDDetailRectangle hUDDetailRectangle = new HUDDetailRectangle(new Vector2(506f, 80f), new Vector2(0f, textBoxMesh.Offset.Y - 24f - 0f - 40f));
		item.addMesh(hUDDetailRectangle);
		currentText = new TextBoxMesh(GameTemplate.StandardFont, new TextBoxDrawProperties(new Vector2(0f, hUDDetailRectangle.Offset.Y), new Vector2(462f, 80f), 1f, HorizontalAlignment.Left, VerticalAlignment.Center));
		currentText.Text = dialog.CurrentText;
		item.addMesh(currentText);
		textBoxMesh = new TextBoxMesh(GameTemplate.StandardFont, new TextBoxDrawProperties(new Vector2(0f, -72f), new Vector2(506f, 178f), 1f, HorizontalAlignment.Left, VerticalAlignment.Top))
		{
			Text = dialog.Text
		};
		item.addMesh(textBoxMesh);
		TextMesh m2 = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(253f, -146f), HorizontalAlignment.Right), 40, useStringBuilder: false)
		{
			Text = InputManager.GetInputGlyph(InputManager.MenuInputCodes.Interact) + " " + LanguageManager.Texts["OK"] + "  " + InputManager.GetInputGlyph(InputManager.MenuInputCodes.Cancel) + " " + LanguageManager.Texts["CANCEL"]
		};
		item.addMesh(m2);
		addChild(item);
	}

	protected override void DoUpdate()
	{
		currentText.Text = dialog.CurrentText;
		base.DoUpdate();
	}
}
