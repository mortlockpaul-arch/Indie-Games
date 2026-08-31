using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace BureauNewPDA;

internal class PDAVideoControl
{
	public enum State
	{
		Starting,
		Waiting,
		Closing,
		Closed,
		NA
	}

	private bool firstTimeCalled = true;

	public string loadVideo = "";

	private bool UIMainActive;

	private bool UIMainClosing;

	private int currentPosition = 1;

	private Color color1 = Color.White;

	private Color color2 = Color.Black;

	private Color color3 = Color.Black;

	private Color color4 = Color.Black;

	public CoreDisplayElements myCoreDisplayElements = new CoreDisplayElements();

	public State currentState = State.NA;

	public PDAGameComponent.PDAState pendingVideoState;

	public int showErrorCode = -1;

	public void resetVideoControl()
	{
		currentState = State.NA;
		pendingVideoState = PDAGameComponent.PDAState.StartUp;
		currentPosition = 1;
	}

	public void update(VideoPlayer myVideoPlayer, GamePadControl myGamePad, PDAGameComponent.PDAState currentPDAState, List<string> playSimpleSound)
	{
		switch (currentPDAState)
		{
		case PDAGameComponent.PDAState.StartUp:
			startUpVideo(myVideoPlayer, myGamePad, currentPDAState, playSimpleSound);
			break;
		case PDAGameComponent.PDAState.Exit:
			if (UIMainClosing)
			{
				UIMainClosing = false;
				loadVideo = "PhoneTurnedOffMainScreen";
				currentPosition = 1;
			}
			break;
		case PDAGameComponent.PDAState.CurrentCase:
			if (UIMainClosing)
			{
				UIMainClosing = false;
				currentPosition = 1;
			}
			videoTurnedSideways(myVideoPlayer, myGamePad);
			break;
		case PDAGameComponent.PDAState.Load:
			if (UIMainClosing)
			{
				UIMainClosing = false;
				currentPosition = 1;
			}
			videoTurnedSideways(myVideoPlayer, myGamePad);
			break;
		case PDAGameComponent.PDAState.VideoPuzzleSelect:
			if (UIMainClosing)
			{
				UIMainClosing = false;
				currentPosition = 1;
			}
			videoTurnedSideways(myVideoPlayer, myGamePad);
			break;
		}
	}

	private void videoTurnedSideways(VideoPlayer myVideoPlayer, GamePadControl myGamePad)
	{
		switch (currentState)
		{
		case State.NA:
			loadVideo = "PhoneTurnedSelectSideways";
			currentPosition = 1;
			currentState = State.Starting;
			break;
		case State.Starting:
			if (myVideoPlayer.State == MediaState.Stopped)
			{
				currentState = State.Waiting;
			}
			break;
		}
	}

	private void startUpVideo(VideoPlayer myVideoPlayer, GamePadControl myGamePad, PDAGameComponent.PDAState currentPDAState, List<string> playSimpleSound)
	{
		if (currentState == State.NA)
		{
			currentState = State.Starting;
			loadVideo = "PDATurnedOnA";
			UIMainActive = false;
			currentPosition = 1;
		}
		else if (currentState == State.Starting)
		{
			if (myVideoPlayer.State == MediaState.Stopped)
			{
				currentState = State.Waiting;
			}
		}
		else if (currentState == State.Waiting)
		{
			if (UIMainActive)
			{
				UIControlsMain(myGamePad, playSimpleSound);
			}
			UIMainActive = true;
		}
		else if (currentState == State.Closing)
		{
			currentPosition = 1;
			adjustColor(ref color1, 5, -1);
			adjustColor(ref color2, 5, -1);
			adjustColor(ref color3, 5, -1);
			adjustColor(ref color4, 5, -1);
			if (color1.A < 150)
			{
				color1 = new Color(0, 0, 0, 0);
				color2 = new Color(0, 0, 0, 0);
				color3 = new Color(0, 0, 0, 0);
				color4 = new Color(0, 0, 0, 0);
				loadVideo = "PDATurnedOnA";
				currentState = State.Closed;
			}
		}
		else if (currentState == State.Closed)
		{
			UIMainClosing = true;
			UIMainActive = false;
			currentState = State.NA;
			currentPDAState = PDAGameComponent.PDAState.CurrentCase;
		}
	}

	private void UIControlsMain(GamePadControl myGamePad, List<string> playSimpleSound)
	{
		if (myGamePad.anyDirection != GamePadControl.direction.NotSet)
		{
			switch (myGamePad.anyDirection)
			{
			case GamePadControl.direction.NE:
				currentPosition = 2;
				break;
			case GamePadControl.direction.NW:
				currentPosition = 1;
				break;
			case GamePadControl.direction.SE:
				currentPosition = 4;
				break;
			case GamePadControl.direction.SW:
				currentPosition = 3;
				break;
			case GamePadControl.direction.E:
				if (currentPosition == 1)
				{
					currentPosition = 2;
				}
				else if (currentPosition == 3)
				{
					currentPosition = 4;
				}
				break;
			case GamePadControl.direction.W:
				if (currentPosition == 2)
				{
					currentPosition = 1;
				}
				else if (currentPosition == 4)
				{
					currentPosition = 3;
				}
				break;
			case GamePadControl.direction.N:
				if (currentPosition == 3)
				{
					currentPosition = 1;
				}
				else if (currentPosition == 4)
				{
					currentPosition = 2;
				}
				break;
			case GamePadControl.direction.S:
				if (currentPosition == 1)
				{
					currentPosition = 3;
				}
				else if (currentPosition == 2)
				{
					currentPosition = 4;
				}
				break;
			}
		}
		if (myGamePad.padAPressed)
		{
			if (currentPosition == 1)
			{
				playSimpleSound.Add("Arcade Beep 02");
				pendingVideoState = PDAGameComponent.PDAState.CurrentCase;
				currentState = State.Closing;
			}
			else if ((currentPosition != 1) & (showErrorCode == -1))
			{
				playSimpleSound.Add("UI_Misc16");
				showErrorCode = 1;
			}
		}
		adjustColor(ref color1, 5, 1);
		adjustColor(ref color2, 5, 2);
		adjustColor(ref color3, 5, 3);
		adjustColor(ref color4, 5, 4);
	}

	private void adjustColor(ref Color myColor, byte amount, int colorId)
	{
		if (colorId == currentPosition)
		{
			addToColor(ref myColor, amount);
		}
		else
		{
			subtractColor(ref myColor, amount);
		}
	}

	private void addToColor(ref Color myColor, byte amount)
	{
		if (myColor.R + amount > myColor.R)
		{
			myColor.A = byte.MaxValue;
			myColor.R = byte.MaxValue;
			myColor.G = byte.MaxValue;
			myColor.B = byte.MaxValue;
		}
		else
		{
			myColor.A += amount;
			myColor.R += amount;
			myColor.G += amount;
			myColor.B += amount;
		}
	}

	private void subtractColor(ref Color myColor, byte amount)
	{
		if (myColor.R < 20)
		{
			myColor.A = 0;
			myColor.R = 0;
			myColor.G = 0;
			myColor.B = 0;
		}
		else
		{
			myColor.A -= amount;
			myColor.R -= amount;
			myColor.G -= amount;
			myColor.B -= amount;
		}
	}

	public void drawUI(SpriteBatch spriteBatch)
	{
		if (UIMainActive)
		{
			spriteBatch.Draw(myCoreDisplayElements.getTexture("PDA"), new Vector2(573f, 203f), myCoreDisplayElements.spriteRDM.getSpriteRectangle("PDA", "UIIconGlow", 0), color1, 0f, new Vector2(64f, 64f), 1f, SpriteEffects.None, 0.9f);
			spriteBatch.Draw(myCoreDisplayElements.getTexture("PDA"), new Vector2(701f, 203f), myCoreDisplayElements.spriteRDM.getSpriteRectangle("PDA", "UIIconGlow", 0), color2, 0f, new Vector2(64f, 64f), 1f, SpriteEffects.None, 0.9f);
			spriteBatch.Draw(myCoreDisplayElements.getTexture("PDA"), new Vector2(573f, 387f), myCoreDisplayElements.spriteRDM.getSpriteRectangle("PDA", "UIIconGlow", 0), color3, 0f, new Vector2(64f, 64f), 1f, SpriteEffects.None, 0.9f);
			spriteBatch.Draw(myCoreDisplayElements.getTexture("PDA"), new Vector2(701f, 387f), myCoreDisplayElements.spriteRDM.getSpriteRectangle("PDA", "UIIconGlow", 0), color4, 0f, new Vector2(64f, 64f), 1f, SpriteEffects.None, 0.9f);
		}
	}
}
