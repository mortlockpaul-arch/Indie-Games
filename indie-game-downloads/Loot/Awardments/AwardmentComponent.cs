using System;
using System.Collections.Generic;
using Eyehook.Framework;
using Loot.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Awardments;

public class AwardmentComponent : DrawableGameComponent
{
	private enum AwardmentState
	{
		Enter,
		Wait,
		Exit
	}

	private class AwardWrapper
	{
		public Awardment Award;

		public AwardmentState State;

		public TimeSpan Timer;

		public AwardWrapper(Awardment award)
		{
			Award = award;
			State = AwardmentState.Enter;
			Timer = enterTime;
		}
	}

	private Game game;

	private SpriteBatch spriteBatch;

	private ContentManager content;

	private Sprite unlocked;

	private static readonly TimeSpan enterTime = TimeSpan.FromSeconds(1.0);

	private static readonly TimeSpan waitTime = TimeSpan.FromSeconds(3.0);

	private static readonly TimeSpan exitTime = TimeSpan.FromSeconds(1.0);

	private List<AwardWrapper> awardQueue = new List<AwardWrapper>();

	private Vector2 awardPos = new Vector2(640f, 600f);

	private Vector2 iconOffset = new Vector2(-224f, -12f);

	private Vector2 textOffset = new Vector2(-160f, 0f);

	private Vector2 offset = new Vector2(0f, 176f);

	private Vector2 shadowOffset = new Vector2(2f, 2f);

	private static Color textColor = new Color(151, 100, 0);

	private static Color textShadowColor = new Color(255, 255, 0);

	public AwardmentComponent(Game game)
		: base(game)
	{
		this.game = game;
	}

	protected override void LoadContent()
	{
		spriteBatch = new SpriteBatch(base.GraphicsDevice);
		content = new ContentManager(game.Services, "Content");
		unlocked = new StillSprite(content.Load<Texture2D>("Sprites\\Awardment\\Unlocked"));
	}

	protected override void UnloadContent()
	{
		spriteBatch.Dispose();
		content.Dispose();
	}

	public void Display(Awardment award)
	{
		if (awardQueue.Count == 0)
		{
			PlaySound.Awardment();
		}
		awardQueue.Add(new AwardWrapper(award));
	}

	public override void Update(GameTime gameTime)
	{
		if (awardQueue.Count == 0)
		{
			base.Update(gameTime);
			return;
		}
		AwardWrapper awardWrapper = awardQueue[0];
		awardWrapper.Timer -= gameTime.ElapsedGameTime;
		switch (awardWrapper.State)
		{
		case AwardmentState.Enter:
			if (awardWrapper.Timer <= TimeSpan.Zero)
			{
				awardWrapper.State = AwardmentState.Wait;
				awardWrapper.Timer = waitTime;
			}
			break;
		case AwardmentState.Wait:
			if (awardWrapper.Timer <= TimeSpan.Zero)
			{
				awardWrapper.State = AwardmentState.Exit;
				awardWrapper.Timer = exitTime;
			}
			break;
		case AwardmentState.Exit:
			if (awardWrapper.Timer <= TimeSpan.Zero)
			{
				awardQueue.Remove(awardWrapper);
				if (awardQueue.Count > 0)
				{
					PlaySound.Awardment();
				}
			}
			break;
		}
		base.Update(gameTime);
	}

	public override void Draw(GameTime gameTime)
	{
		if (awardQueue.Count == 0)
		{
			base.Draw(gameTime);
			return;
		}
		AwardWrapper awardWrapper = awardQueue[0];
		Vector2 vector = awardWrapper.State switch
		{
			AwardmentState.Enter => awardPos + offset * (float)(awardWrapper.Timer.TotalSeconds / enterTime.TotalSeconds), 
			AwardmentState.Wait => awardPos, 
			_ => awardPos + offset * (float)(1.0 - awardWrapper.Timer.TotalSeconds / enterTime.TotalSeconds), 
		};
		Vector2 vector2 = vector + textOffset;
		spriteBatch.Begin();
		unlocked.Draw(spriteBatch, vector);
		if (awardWrapper.Award.Icon != null)
		{
			awardWrapper.Award.Icon.Draw(spriteBatch, vector + iconOffset);
		}
		Text.Draw(spriteBatch, vector2 + shadowOffset, awardWrapper.Award.Name, textShadowColor);
		Text.Draw(spriteBatch, vector2, awardWrapper.Award.Name, textColor);
		spriteBatch.End();
		base.Draw(gameTime);
	}
}
