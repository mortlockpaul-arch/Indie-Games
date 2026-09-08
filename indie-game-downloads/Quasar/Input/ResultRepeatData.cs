using Quasar.Global;

namespace Quasar.Input;

internal class ResultRepeatData
{
	public uint LastCheckedFrame;

	public long FirstPressTime;

	public uint RepeatsSent;

	public int RepeatStart;

	public int RepeatInterval;

	public ResultRepeatData()
	{
		RepeatStart = InputManager.REPEAT_START;
		RepeatInterval = InputManager.REPEAT_INTERVAL;
	}

	public void SetRepeatConfig(int repeatStart, int repeatInterval)
	{
		RepeatStart = repeatStart;
		RepeatInterval = repeatInterval;
	}

	public bool Check(bool status, bool justPressed)
	{
		if (status)
		{
			if (justPressed || LastCheckedFrame < Engine.Instance.ElapsedFrames - 1)
			{
				LastCheckedFrame = Engine.Instance.ElapsedFrames;
				FirstPressTime = Timer.DefaultTimer.TotalTime;
				RepeatsSent = 0u;
				return true;
			}
			LastCheckedFrame = Engine.Instance.ElapsedFrames;
			long num = Timer.DefaultTimer.TotalTime - FirstPressTime;
			if (num > RepeatStart && (num - RepeatStart) / RepeatInterval + 1 != RepeatsSent)
			{
				RepeatsSent++;
				return true;
			}
		}
		return false;
	}
}
