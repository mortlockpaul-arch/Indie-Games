using System;
using Microsoft.Xna.Framework;
using Quasar.GUI;
using Quasar.GUI.Elements;
using Quasar.GameUtils.Meshes;
using Quasar.Global;
using Quasar.Input;
using Quasar.Language;
using Quasar.Meshes.Text;

namespace Quasar.GameUtils.Template;

public class GameDialogNode : DialogNode
{
	private const float WIDTH = 550f;

	private const float HEIGHT = 400f;

	private const float TOP_HEIGHT = 48f;

	private const float BOTTOM_HEIGHT = 64f;

	private const float TOP_MARGIN = 32f;

	private const float BORDER = 94f;

	private const float MARGIN = 22f;

	private RenderItem item;

	public GameDialogNode(Dialog dialog)
		: base(dialog)
	{
		item = new RenderItem();
		HUDRectangle m = new HUDRectangle(new Vector2(550f, 400f));
		item.addMesh(m);
		Layout2D.LayoutData layout = new Layout2D.LayoutData(new Vector2(0f, 154f), new Vector2(506f, 48f));
		Layout2D.KeepPixelAlign(ref layout);
		TextBoxMesh textBoxMesh = new TextBoxMesh(GameTemplate.StandardFont, new TextBoxDrawProperties(layout, 1f, HorizontalAlignment.Left, VerticalAlignment.Top), 256, useStringBuilder: false);
		textBoxMesh.Text = dialog.Title;
		textBoxMesh.FirstMaterial.Diffuse = GameTemplate.DialogTitleColor;
		item.addMesh(textBoxMesh);
		layout = new Layout2D.LayoutData(new Vector2(0f, -8f), new Vector2(506f, 260f));
		Layout2D.KeepPixelAlign(ref layout);
		textBoxMesh = new TextBoxMesh(GameTemplate.StandardFont, new TextBoxDrawProperties(layout, 1f, HorizontalAlignment.Left, VerticalAlignment.Top))
		{
			Text = dialog.Text
		};
		item.addMesh(textBoxMesh);
		Vector2 offset = new Vector2(253f, -146f);
		offset.X = (float)Math.Floor(offset.X) + 0.5f;
		offset.Y = (float)Math.Floor(offset.Y) + 0.5f;
		TextMesh textMesh = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(offset, HorizontalAlignment.Right), 40, useStringBuilder: false);
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
	}
}
