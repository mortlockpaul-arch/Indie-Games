using System.Collections.Generic;
using Kobingo.Xna.Library.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kobingo.Xna.Library.Game;

public class MessageScreen : GameScreen
{
	public List<string> Message { get; set; }

	public MessageScreen(ScreenManager screenManager, params string[] message)
		: base(screenManager)
	{
		Message = new List<string>(message);
	}

	public override void Draw(GameTime gameTime, float transition)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		base.Draw(gameTime, transition);
		if (GameManager.Font != null)
		{
			Vector2 screenCenter = base.ScreenManager.ScreenCenter;
			float num = screenCenter.Y - (float)((Message.Count - 1) * GameManager.Font.LineSpacing / 2);
			base.ScreenManager.SpriteBatch.Begin((SpriteBlendMode)1);
			for (int i = 0; i < Message.Count; i++)
			{
				base.ScreenManager.SpriteBatch.DrawAlignedString(GameManager.Font, Message[i], new Vector2(screenCenter.X, num + (float)(i * GameManager.Font.LineSpacing)), Align.Center, Color.White);
			}
			base.ScreenManager.SpriteBatch.End();
		}
	}
}
