namespace XnaLibrary.Input;

public class VirtualPadState
{
	private VirtualPadButtons buttons;

	private VirtualPadDPad dPad;

	private VirtualPadThumbSticks thumbSticks;

	private VirtualPadTriggers triggers;

	public VirtualPadButtons Buttons => buttons;

	public VirtualPadDPad DPad => dPad;

	public VirtualPadThumbSticks ThumbSticks => thumbSticks;

	public VirtualPadTriggers Triggers => triggers;

	public VirtualPadState()
	{
		buttons = new VirtualPadButtons();
		dPad = new VirtualPadDPad();
		thumbSticks = new VirtualPadThumbSticks();
		triggers = new VirtualPadTriggers();
	}

	public void Update()
	{
		buttons.Update();
		dPad.Update();
		thumbSticks.Update();
		triggers.Update();
	}

	public void SetPress(bool press)
	{
		buttons.SetPress(press);
		dPad.SetPress(press);
		thumbSticks.SetPress(press);
		triggers.SetPress(press);
	}
}
