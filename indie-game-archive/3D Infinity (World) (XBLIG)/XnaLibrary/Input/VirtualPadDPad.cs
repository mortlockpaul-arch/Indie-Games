namespace XnaLibrary.Input;

public class VirtualPadDPad
{
	private InputState up;

	private InputState down;

	private InputState left;

	private InputState right;

	public InputState Up => up;

	public InputState Down => down;

	public InputState Left => left;

	public InputState Right => right;

	public VirtualPadDPad()
	{
		up = new InputState();
		down = new InputState();
		left = new InputState();
		right = new InputState();
	}

	public void Update()
	{
		up.Update();
		down.Update();
		left.Update();
		right.Update();
	}

	public void SetPress(bool press)
	{
		up.SetPress(press);
		down.SetPress(press);
		left.SetPress(press);
		right.SetPress(press);
	}
}
