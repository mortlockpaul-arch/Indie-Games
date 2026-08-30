using System;
using System.Collections.Generic;
using Kobingo.Xna.Library.Common;
using Kobingo.Xna.Library.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kobingo.Xna.Library.Game;

public class CreditsScreen : GameScreen
{
	public static SpriteFont Font { get; set; }

	public List<string> Credits { get; private set; }

	private int Index { get; set; }

	private TickTimer Timer { get; set; }

	private Transition<int> Transition { get; set; }

	public CreditsScreen(ScreenManager screenManager, params string[] credits)
		: base(screenManager)
	{
		Credits = new List<string>(credits);
		Timer = new TickTimer(TimeSpan.FromSeconds(3.0));
		TickTimer timer = Timer;
		EventHandler value = delegate
		{
			if (Credits.Count > 0)
			{
				Transition.Change(++Index, TimeSpan.FromSeconds(0.5), wait: true, TimeSpan.Zero);
			}
		};
		timer.Tick += value;
		Transition = new Transition<int>();
	}

	public override void Update(GameTime gameTime, bool active)
	{
		Timer.Update(gameTime);
		Transition.Update(gameTime);
		if (Transition.Current >= Credits.Count)
		{
			Close();
		}
		base.Update(gameTime, active);
	}

	public override void Draw(GameTime gameTime, float transition)
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		base.Draw(gameTime, transition);
		if (Font == null)
		{
			return;
		}
		GameManager.ScreenManager.SpriteBatch.Begin((SpriteBlendMode)1);
		foreach (TransitionState<int> state in Transition.States)
		{
			if (state.Value < Credits.Count)
			{
				GameManager.ScreenManager.SpriteBatch.DrawAlignedString(Font, Credits[state.Value], GameManager.ScreenManager.ScreenCenter, Align.Center, new Color(Color.White, state.Transition));
			}
		}
		GameManager.ScreenManager.SpriteBatch.End();
	}

	public override void Show()
	{
		Index = 0;
		Transition.Clear();
		Timer.Reset();
		if (Credits.Count > 0)
		{
			Transition.Change(0, TimeSpan.FromSeconds(0.5), wait: true, TimeSpan.Zero);
		}
		base.Show();
	}
}
