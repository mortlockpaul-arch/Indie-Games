namespace XnaLibrary.Input;

public class VirtualPadTriggers
{
	private InputState left = new InputState();

	private InputState right = new InputState();

	public InputState Left => left;

	public InputState Right => right;

	public void Update()
	{
		left.Update();
		right.Update();
	}

	public void SetPress(bool press)
	{
		left.SetPress(press);
		right.SetPress(press);
	}
}
