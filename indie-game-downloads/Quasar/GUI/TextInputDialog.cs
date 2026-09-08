using System;
using Microsoft.Xna.Framework;
using Quasar.Input;

namespace Quasar.GUI;

public class TextInputDialog : IDisposable
{
	private string title;

	private string text;

	private string currentText;

	private Layout layout;

	public string Title => title;

	public string Text => text;

	public string CurrentText => currentText;

	public Layout Layout => layout;

	public event Action<TextInputDialog, TextInputDialogResult> OnInteraction;

	private event TextInputHandler handler;

	public TextInputDialog(string title, string message, string defaultText, TextInputHandler handler, Layout layout)
	{
		this.title = title;
		text = message;
		currentText = defaultText;
		this.layout = layout;
		if (handler != null)
		{
			this.handler += handler;
		}
	}

	public void Update()
	{
		PlayerIndex whoPressed = PlayerIndex.One;
		if (InputManager.MenuInteract(ref whoPressed))
		{
			EndDialog(TextInputDialogResult.OkYes, whoPressed);
		}
		else if (InputManager.MenuCancel(ref whoPressed))
		{
			EndDialog(TextInputDialogResult.No, PlayerIndex.One);
		}
		else
		{
			Keyboard.Instance.TextInput(ref currentText);
		}
	}

	private void EndDialog(TextInputDialogResult result, PlayerIndex whoPressed)
	{
		layout.HideDialog();
		if (OnInteraction != null)
		{
			OnInteraction(this, result);
		}
		if (handler != null)
		{
			handler(result, whoPressed, currentText);
		}
		Dispose();
	}

	~TextInputDialog()
	{
		Dispose();
	}

	public void Dispose()
	{
		handler = null;
	}
}
