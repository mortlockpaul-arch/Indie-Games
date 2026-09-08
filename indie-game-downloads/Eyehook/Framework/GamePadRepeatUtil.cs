using System;
using Microsoft.Xna.Framework;

namespace Eyehook.Framework;

public class GamePadRepeatUtil
{
	private GamePadManager gamePad;

	private TimeSpan timer;

	private TimeSpan newRepeatDelay;

	private TimeSpan repeatDelay;

	public GamePadRepeatUtil(GamePadManager gamePad, TimeSpan newRepeatDelay, TimeSpan repeatDelay)
	{
		this.gamePad = gamePad;
		this.newRepeatDelay = newRepeatDelay;
		this.repeatDelay = repeatDelay;
	}

	private bool canRepeat()
	{
		return timer <= TimeSpan.Zero;
	}

	public void Update(GameTime gameTime)
	{
		if (timer > TimeSpan.Zero)
		{
			timer -= gameTime.ElapsedGameTime;
		}
	}

	public bool Right()
	{
		if (gamePad.isNewDirRight())
		{
			timer = newRepeatDelay;
			return true;
		}
		if (!canRepeat() || !gamePad.isDirRight())
		{
			return false;
		}
		timer = repeatDelay;
		return true;
	}

	public bool Left()
	{
		if (gamePad.isNewDirLeft())
		{
			timer = newRepeatDelay;
			return true;
		}
		if (!canRepeat() || !gamePad.isDirLeft())
		{
			return false;
		}
		timer = repeatDelay;
		return true;
	}

	public bool Up()
	{
		if (gamePad.isNewDirUp())
		{
			timer = newRepeatDelay;
			return true;
		}
		if (!canRepeat() || !gamePad.isDirUp())
		{
			return false;
		}
		timer = repeatDelay;
		return true;
	}

	public bool Down()
	{
		if (gamePad.isNewDirDown())
		{
			timer = newRepeatDelay;
			return true;
		}
		if (!canRepeat() || !gamePad.isDirDown())
		{
			return false;
		}
		timer = repeatDelay;
		return true;
	}
}
