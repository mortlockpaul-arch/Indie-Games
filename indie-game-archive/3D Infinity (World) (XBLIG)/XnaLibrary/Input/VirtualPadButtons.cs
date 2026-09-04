namespace XnaLibrary.Input;

public class VirtualPadButtons
{
	private InputState a;

	private InputState b;

	private InputState x;

	private InputState y;

	private InputState leftShoulder;

	private InputState rightShoulder;

	private InputState leftStick;

	private InputState rightStick;

	private InputState back;

	private InputState start;

	public InputState A => a;

	public InputState B => b;

	public InputState X => x;

	public InputState Y => y;

	public InputState LeftShoulder => leftShoulder;

	public InputState RightShoulder => rightShoulder;

	public InputState LeftStick => leftStick;

	public InputState RightStick => rightStick;

	public InputState Back => back;

	public InputState Start => start;

	public VirtualPadButtons()
	{
		a = new InputState();
		b = new InputState();
		x = new InputState();
		y = new InputState();
		leftShoulder = new InputState();
		rightShoulder = new InputState();
		leftStick = new InputState();
		rightStick = new InputState();
		back = new InputState();
		start = new InputState();
	}

	public void Update()
	{
		a.Update();
		b.Update();
		x.Update();
		y.Update();
		leftShoulder.Update();
		rightShoulder.Update();
		leftStick.Update();
		rightStick.Update();
		back.Update();
		start.Update();
	}

	public void SetPress(bool press)
	{
		a.SetPress(press);
		b.SetPress(press);
		x.SetPress(press);
		y.SetPress(press);
		leftShoulder.SetPress(press);
		rightShoulder.SetPress(press);
		leftStick.SetPress(press);
		rightStick.SetPress(press);
		back.SetPress(press);
		start.SetPress(press);
	}
}
