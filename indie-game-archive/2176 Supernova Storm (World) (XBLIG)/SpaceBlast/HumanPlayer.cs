using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using SpaceBlast.Weapons;

namespace SpaceBlast;

internal class HumanPlayer : LocalPlayer
{
	private GamePadState m_LastPadState = default(GamePadState);

	private KeyboardState m_LastKeyState = default(KeyboardState);

	private double m_VibrationTimeout;

	private bool m_PrimaryPlayer = true;

	public HumanPlayer(byte playerid, Vector3 pos, Gamer gamer, ShipColor colour, ETeam team, bool primary)
		: base(playerid, pos, gamer, colour, team)
	{
		m_PrimaryPlayer = primary;
	}

	public override void Terminate()
	{
	}

	protected override void Reset(bool newGame)
	{
		StopControllerVibration();
		base.Reset(newGame);
	}

	public override void Update()
	{
		if (m_VibrationTimeout > 0.1 && TimeManager.TotalSeconds > m_VibrationTimeout)
		{
			StopControllerVibration();
		}
		if (IsActive)
		{
			GamePadState gamePadState;
			KeyboardState keyboardState;
			if (MainGame.Instance.IsPaused)
			{
				gamePadState = default(GamePadState);
				keyboardState = default(KeyboardState);
			}
			else
			{
				gamePadState = (m_PrimaryPlayer ? InputManager.GetPlayer1Input() : InputManager.GetPlayer2Input());
				keyboardState = default(KeyboardState);
				if (!gamePadState.IsConnected)
				{
					MainGame.Instance.ShowControllerDisconnectedScreen(m_PrimaryPlayer);
				}
			}
			TheShip.UpdateHumanShip(gamePadState, keyboardState);
			if (gamePadState.Buttons.A == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Space))
			{
				Weapon activeFrontWeapon = TheShip.Weapons.ActiveFrontWeapon;
				if (TimeManager.TotalSeconds >= activeFrontWeapon.EarliestNextShot && activeFrontWeapon.Ammo > 0 && !m_PowerCut)
				{
					TheShip.Weapons.FireFrontWeapon();
				}
			}
			if (gamePadState.Buttons.B == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Down))
			{
				Weapon activeFrontWeapon2 = TheShip.Weapons.ActiveFrontWeapon;
				if (TimeManager.TotalSeconds >= activeFrontWeapon2.EarliestNextShot && activeFrontWeapon2.Ammo > 0 && !m_PowerCut)
				{
					TheShip.Weapons.FireRearWeapon();
				}
			}
			if ((gamePadState.Buttons.X == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.X)) && !m_PowerCut)
			{
				TheShip.Weapons.FireSpecialWeapon();
			}
			if ((gamePadState.Buttons.RightShoulder == ButtonState.Pressed && m_LastPadState.Buttons.RightShoulder == ButtonState.Released) || (keyboardState.IsKeyDown(Keys.X) && !m_LastKeyState.IsKeyDown(Keys.X)))
			{
				TheShip.Weapons.CycleMainWeaponRight();
			}
			else if ((gamePadState.Buttons.LeftShoulder == ButtonState.Pressed && m_LastPadState.Buttons.LeftShoulder == ButtonState.Released) || (keyboardState.IsKeyDown(Keys.Z) && !m_LastKeyState.IsKeyDown(Keys.Z)))
			{
				TheShip.Weapons.CycleMainWeaponLeft();
			}
			m_LastKeyState = keyboardState;
			m_LastPadState = gamePadState;
		}
		base.Update();
	}

	public void SetControllerVibration(float duration, float leftMotor, float rightMotor)
	{
		if (m_PrimaryPlayer)
		{
			InputManager.SetPlayer1Vibration(leftMotor, rightMotor);
		}
		else
		{
			InputManager.SetPlayer2Vibration(leftMotor, rightMotor);
		}
		m_VibrationTimeout = TimeManager.TotalSeconds + (double)duration;
	}

	public void StopControllerVibration()
	{
		if (m_PrimaryPlayer)
		{
			InputManager.SetPlayer1Vibration(0f, 0f);
		}
		else
		{
			InputManager.SetPlayer2Vibration(0f, 0f);
		}
		m_VibrationTimeout = 0.0;
	}
}
