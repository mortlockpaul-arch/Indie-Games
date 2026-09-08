using System;
using Microsoft.Xna.Framework;
using Quasar.Global;
using Quasar.Input;

namespace Quasar.GUI;

public class Dialog : IDisposable
{
	private DialogOptions options;

	private string title;

	private string text;

	private Layout layout;

	private long activeTime;

	public DialogOptions Options => options;

	public string Title => title;

	public string Text => text;

	public Layout Layout => layout;

	public event Action<Dialog, DialogResult> OnInteraction;

	private event DialogHandler handler;

	public Dialog(string title, string message, DialogOptions options, DialogHandler handler, Layout layout)
		: this(title, message, options, handler, 0, layout)
	{
	}

	public Dialog(string title, string message, DialogOptions options, DialogHandler handler, int delay, Layout layout)
	{
		this.title = title;
		text = message;
		this.options = options;
		this.layout = layout;
		activeTime = Timer.DefaultTimer.TotalTime + delay;
		if (handler != null)
		{
			this.handler += handler;
		}
	}

	public void Update()
	{
		if (Timer.DefaultTimer.TotalTime < activeTime)
		{
			return;
		}
		PlayerIndex whoPressed = PlayerIndex.One;
		switch (options)
		{
		case DialogOptions.Ok:
			if (InputManager.MenuInteract(ref whoPressed))
			{
				EndDialog(DialogResult.OkYes, whoPressed);
			}
			break;
		case DialogOptions.YesNo:
			if (InputManager.MenuInteract(ref whoPressed))
			{
				EndDialog(DialogResult.OkYes, whoPressed);
			}
			if (InputManager.MenuCancel(ref whoPressed))
			{
				EndDialog(DialogResult.No, whoPressed);
			}
			break;
		}
	}

	private void EndDialog(DialogResult result, PlayerIndex whoPressed)
	{
		layout.HideDialog();
		if (OnInteraction != null)
		{
			OnInteraction(this, result);
		}
		if (handler != null)
		{
			handler(result, whoPressed);
		}
		Dispose();
	}

	~Dialog()
	{
		Dispose();
	}

	public void Dispose()
	{
		handler = null;
	}
}
