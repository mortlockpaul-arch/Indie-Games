using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace XnaLibrary.Blade;

public class MessageBoxComponent : GameComponent
{
	public class EventResult : EventArgs
	{
		public int? Result;
	}

	private IAsyncResult asyncResult;

	private int? result;

	private bool isCompleted;

	public bool IsVisible => Guide.IsVisible;

	public bool IsCompleted => isCompleted;

	public int? Result => result;

	public event EventHandler<EventResult> Selected;

	public MessageBoxComponent(Game game)
		: base(game)
	{
	}

	public override void Update(GameTime gameTime)
	{
		if (asyncResult != null && asyncResult.IsCompleted && !Guide.IsVisible)
		{
			result = Guide.EndShowMessageBox(asyncResult);
			asyncResult = null;
			isCompleted = true;
			if (Selected != null)
			{
				EventResult eventResult = new EventResult();
				eventResult.Result = result;
				EventResult e = eventResult;
				Selected(this, e);
			}
		}
		((GameComponent)this).Update(gameTime);
	}

	public void RemoveSelectedEvents()
	{
		Selected = null;
	}

	public bool ShowMessageBox(PlayerIndex player, string title, string text, IEnumerable<string> buttons, int focusButton, MessageBoxIcon icon)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Guide.IsVisible)
		{
			return false;
		}
		result = null;
		isCompleted = false;
		asyncResult = Guide.BeginShowMessageBox(player, title, text, buttons, focusButton, icon, (AsyncCallback)null, (object)null);
		return true;
	}
}
