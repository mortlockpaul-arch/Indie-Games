namespace XnaLibrary.Input;

public class VirtualPadThumbSticks
{
	private VirtualPadDPad left = new VirtualPadDPad();

	private VirtualPadDPad right = new VirtualPadDPad();

	public VirtualPadDPad Left => left;

	public VirtualPadDPad Right => right;

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
