using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AircraftRC;

public class ManetteConfig
{
	public bool Acceleration;

	public bool Deceleration;

	public bool Ypress;

	public bool Xpress;

	public bool Apress;

	public bool ApressBis;

	public bool Bpress;

	public bool StartP;

	public bool BHPlus;

	public bool BHMoins;

	public bool ZoomPlus;

	public bool ZoomMoins;

	private InputAction RightTrigger;

	private InputAction LeftTrigger;

	private InputAction A;

	private InputAction ABis;

	private InputAction B;

	private InputAction Y;

	private InputAction X;

	private InputAction Start;

	private InputAction HPlus;

	private InputAction HMoins;

	private InputAction Zoomplus;

	private InputAction Zoommoins;

	public InputState input;

	public PlayerIndex playerIndex;

	public ManetteConfig(CustomPhysicsGame game)
	{
		RightTrigger = new InputAction(new Buttons[1] { Buttons.RightTrigger }, newPressOnly: false);
		LeftTrigger = new InputAction(new Buttons[1] { Buttons.LeftTrigger }, newPressOnly: false);
		Zoomplus = new InputAction(new Buttons[1] { Buttons.RightShoulder }, newPressOnly: false);
		Zoommoins = new InputAction(new Buttons[1] { Buttons.LeftShoulder }, newPressOnly: false);
		HPlus = new InputAction(new Buttons[2]
		{
			Buttons.DPadRight,
			Buttons.DPadUp
		}, newPressOnly: true);
		HMoins = new InputAction(new Buttons[2]
		{
			Buttons.DPadLeft,
			Buttons.DPadDown
		}, newPressOnly: true);
		A = new InputAction(new Buttons[1] { Buttons.A }, newPressOnly: false);
		ABis = new InputAction(new Buttons[1] { Buttons.A }, newPressOnly: true);
		B = new InputAction(new Buttons[1] { Buttons.B }, newPressOnly: true);
		Y = new InputAction(new Buttons[1] { Buttons.Y }, newPressOnly: true);
		X = new InputAction(new Buttons[1] { Buttons.X }, newPressOnly: true);
		Start = new InputAction(new Buttons[1] { Buttons.Start }, newPressOnly: true);
		input = new InputState();
	}

	public void Update(CustomPhysicsGame game)
	{
		if (Zoomplus.Evaluate(input, game.menu.player, out playerIndex))
		{
			ZoomPlus = true;
		}
		else
		{
			ZoomPlus = false;
		}
		if (Zoommoins.Evaluate(input, game.menu.player, out playerIndex))
		{
			ZoomMoins = true;
		}
		else
		{
			ZoomMoins = false;
		}
		if (HPlus.Evaluate(input, game.menu.player, out playerIndex))
		{
			BHPlus = true;
		}
		else
		{
			BHPlus = false;
		}
		if (HMoins.Evaluate(input, game.menu.player, out playerIndex))
		{
			BHMoins = true;
		}
		else
		{
			BHMoins = false;
		}
		if (A.Evaluate(input, game.menu.player, out playerIndex))
		{
			Apress = true;
		}
		else
		{
			Apress = false;
		}
		if (ABis.Evaluate(input, game.menu.player, out playerIndex))
		{
			ApressBis = true;
		}
		else
		{
			ApressBis = false;
		}
		if (B.Evaluate(input, game.menu.player, out playerIndex))
		{
			Bpress = true;
		}
		else
		{
			Bpress = false;
		}
		if (Y.Evaluate(input, game.menu.player, out playerIndex))
		{
			Ypress = true;
		}
		else
		{
			Ypress = false;
		}
		if (X.Evaluate(input, game.menu.player, out playerIndex))
		{
			Xpress = true;
		}
		else
		{
			Xpress = false;
		}
		if (Start.Evaluate(input, game.menu.player, out playerIndex))
		{
			StartP = true;
		}
		else
		{
			StartP = false;
		}
		if (game.manetteChoix == CustomPhysicsGame.ManetteChoix.M5)
		{
			if (RightTrigger.Evaluate(input, game.menu.player, out playerIndex))
			{
				Acceleration = true;
			}
			else
			{
				Acceleration = false;
			}
			if (LeftTrigger.Evaluate(input, game.menu.player, out playerIndex))
			{
				Deceleration = true;
			}
			else
			{
				Deceleration = false;
			}
		}
		if (game.manetteChoix == CustomPhysicsGame.ManetteChoix.M6)
		{
			if (RightTrigger.Evaluate(input, game.menu.player, out playerIndex))
			{
				Acceleration = true;
			}
			else
			{
				Acceleration = false;
			}
			if (LeftTrigger.Evaluate(input, game.menu.player, out playerIndex))
			{
				Deceleration = true;
			}
			else
			{
				Deceleration = false;
			}
		}
		input.Update();
	}
}
