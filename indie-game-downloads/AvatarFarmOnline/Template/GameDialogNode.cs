using Microsoft.Xna.Framework;
using Quasar;
using Quasar.GUI;
using Quasar.GUI.Elements;
using Quasar.GameUtils.Meshes;
using Quasar.GameUtils.Template;
using Quasar.Input;
using Quasar.Language;
using Quasar.Meshes.Text;

namespace AvatarFarmOnline.Template;

public class GameDialogNode : DialogNode
{
	private const float WIDTH = 550f;

	private const float HEIGHT = 400f;

	private const float TOP_HEIGHT = 40f;

	private const float BOTTOM_HEIGHT = 64f;

	private const float TOP_MARGIN = 32f;

	private const float BORDER = 94f;

	private const float MARGIN = 32f;

	private RenderItem item;

	public GameDialogNode(Dialog dialog)
		: base(dialog)
	{
		item = new RenderItem();
		transform.Scale = new Vector3(1.1f);
		HUDRectangle m = new HUDRectangle(new Vector2(550f, 400f));
		item.addMesh(m);
		TextBoxMesh textBoxMesh = new TextBoxMesh(GameTemplate.StandardFont, new TextBoxDrawProperties(new Vector2(0f, 148f), new Vector2(486f, 40f), 1f, HorizontalAlignment.Left, VerticalAlignment.Top), 256, useStringBuilder: false);
		textBoxMesh.Text = dialog.Title;
		textBoxMesh.FirstMaterial.Diffuse = GameTemplate.DialogTitleColor;
		item.addMesh(textBoxMesh);
		textBoxMesh = new TextBoxMesh(GameTemplate.StandardFont, new TextBoxDrawProperties(new Vector2(0f, -4f), new Vector2(486f, 272f), 1f, HorizontalAlignment.Left, VerticalAlignment.Top), 256, useStringBuilder: false)
		{
			Text = dialog.Text
		};
		item.addMesh(textBoxMesh);
		TextMesh textMesh = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(243f, -152f), HorizontalAlignment.Right), 40, useStringBuilder: false);
		switch (dialog.Options)
		{
		case DialogOptions.Ok:
			textMesh.Text = InputManager.GetInputGlyph(InputManager.MenuInputCodes.Interact) + " " + LanguageManager.Texts["OK"];
			break;
		case DialogOptions.YesNo:
			textMesh.Text = InputManager.GetInputGlyph(InputManager.MenuInputCodes.Interact) + " " + LanguageManager.Texts["OK"] + "  " + InputManager.GetInputGlyph(InputManager.MenuInputCodes.Cancel) + " " + LanguageManager.Texts["CANCEL"];
			break;
		}
		item.addMesh(textMesh);
		addChild(item);
		dialog.OnInteraction += dialog_OnInteraction;
	}

	protected override void DoUpdate()
	{
		transform.Scale = Vector3.Lerp(transform.Scale, Vector3.One, 0.2f);
		base.DoUpdate();
	}

	private void dialog_OnInteraction(Dialog arg1, DialogResult arg2)
	{
		GameTemplate.InteractionAudio.Start();
	}
}
