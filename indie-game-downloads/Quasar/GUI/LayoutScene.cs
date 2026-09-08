using Quasar.Elements.Cameras;
using Quasar.GUI.Elements;

namespace Quasar.GUI;

public class LayoutScene : Scene
{
	private Layout layout;

	private Template template;

	private DialogNode dialogNode;

	private TextInputNode textInputNode;

	public LayoutScene(Layout layout)
	{
		base.Camera = new StaticCamera2D();
		Add(base.Camera);
		this.layout = layout;
		layout.DialogCreated += OnDialogCreated;
		layout.DialogRemoved += OnDialogRemoved;
		layout.TextInputCreated += OnTextInputCreated;
		layout.TextInputRemoved += OnTextInputRemoved;
		template = TemplateManager.Templates[layout.Template];
		Add(template.CreateElement(layout));
		if (layout.IsDialogShown)
		{
			if (layout.ActiveDialog != null)
			{
				OnDialogCreated(layout, layout.ActiveDialog);
			}
			else if (layout.ActiveTextInput != null)
			{
				OnTextInputCreated(layout, layout.ActiveTextInput);
			}
		}
	}

	private void OnDialogRemoved(Layout layout)
	{
		Remove(dialogNode);
		dialogNode = null;
	}

	private void OnDialogCreated(Layout layout, Dialog dialog)
	{
		if (dialogNode != null)
		{
			Remove(dialogNode);
		}
		dialogNode = template.CreateDialog(layout, dialog);
		Add(dialogNode);
	}

	private void OnTextInputRemoved(Layout layout)
	{
		Remove(textInputNode);
		textInputNode = null;
	}

	private void OnTextInputCreated(Layout layout, TextInputDialog dialog)
	{
		if (textInputNode != null)
		{
			Remove(textInputNode);
		}
		textInputNode = template.CreateTextInputDialog(layout, dialog);
		Add(textInputNode);
	}

	public override void Update()
	{
		base.Update();
	}
}
