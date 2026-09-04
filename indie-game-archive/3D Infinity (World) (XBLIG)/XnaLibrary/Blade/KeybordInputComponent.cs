using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace XnaLibrary.Blade;

public class KeybordInputComponent : GameComponent
{
	private IAsyncResult asyncResult;

	public string Result { get; set; }

	public KeybordInputComponent(Game game)
		: base(game)
	{
	}

	public override void Update(GameTime gameTime)
	{
		if (asyncResult != null && asyncResult.IsCompleted && !Guide.IsVisible)
		{
			Result = Guide.EndShowKeyboardInput(asyncResult);
			asyncResult = null;
		}
		((GameComponent)this).Update(gameTime);
	}

	public bool ShowKeyboardInput(PlayerIndex player, string title, string description, string defaultText)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (Guide.IsVisible)
		{
			return false;
		}
		Result = string.Empty;
		asyncResult = Guide.BeginShowKeyboardInput(player, title, description, defaultText, (AsyncCallback)null, (object)null);
		return true;
	}
}
