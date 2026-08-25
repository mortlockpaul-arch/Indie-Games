namespace JamSouls;

public class BotInputEvent
{
	public float duration;

	public bool bLeft;

	public bool bRight;

	public bool bDown;

	public bool bJump;

	public bool bRun;

	public BotInputEvent(bool left, bool right, bool down, bool jump, bool run)
	{
		bLeft = left;
		bRight = right;
		bDown = down;
		bJump = jump;
		bRun = run;
	}
}
