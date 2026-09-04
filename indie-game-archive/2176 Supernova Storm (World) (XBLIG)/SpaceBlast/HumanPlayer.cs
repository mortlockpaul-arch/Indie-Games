using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using SpaceBlast.Weapons;

namespace SpaceBlast;

internal class HumanPlayer : LocalPlayer
{
	private GamePadState m_LastPadState;

	private KeyboardState m_LastKeyState;

	private double m_VibrationTimeout;

	private bool m_PrimaryPlayer;

	public HumanPlayer(byte playerid, Vector3 pos, Gamer gamer, ShipColor colour, ETeam team, bool primary)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		m_LastPadState = default(GamePadState);
		m_LastKeyState = default(KeyboardState);
		m_PrimaryPlayer = true;
		base._002Ector(playerid, pos, gamer, colour, team);
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
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Invalid comparison between Unknown and I4
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Invalid comparison between Unknown and I4
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Invalid comparison between Unknown and I4
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Invalid comparison between Unknown and I4
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Invalid comparison between Unknown and I4
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		if (m_VibrationTimeout > 0.1 && TimeManager.TotalSeconds > m_VibrationTimeout)
		{
			StopControllerVibration();
		}
		GamePadState val;
		KeyboardState val2;
		if (IsActive)
		{
			if (MainGame.Instance.IsPaused)
			{
				val = default(GamePadState);
				val2 = default(KeyboardState);
			}
			else
			{
				val = (m_PrimaryPlayer ? InputManager.GetPlayer1Input() : InputManager.GetPlayer2Input());
				val2 = default(KeyboardState);
				if (!((GamePadState)(ref val)).IsConnected)
				{
					MainGame.Instance.ShowControllerDisconnectedScreen(m_PrimaryPlayer);
				}
			}
			TheShip.UpdateHumanShip(val, val2);
			GamePadButtons buttons = ((GamePadState)(ref val)).Buttons;
			if ((int)((GamePadButtons)(ref buttons)).A == 1 || ((KeyboardState)(ref val2)).IsKeyDown((Keys)32))
			{
				Weapon activeFrontWeapon = TheShip.Weapons.ActiveFrontWeapon;
				if (TimeManager.TotalSeconds >= activeFrontWeapon.EarliestNextShot && activeFrontWeapon.Ammo > 0 && !m_PowerCut)
				{
					TheShip.Weapons.FireFrontWeapon();
				}
			}
			GamePadButtons buttons2 = ((GamePadState)(ref val)).Buttons;
			if ((int)((GamePadButtons)(ref buttons2)).B == 1 || ((KeyboardState)(ref val2)).IsKeyDown((Keys)40))
			{
				Weapon activeFrontWeapon2 = TheShip.Weapons.ActiveFrontWeapon;
				if (TimeManager.TotalSeconds >= activeFrontWeapon2.EarliestNextShot && activeFrontWeapon2.Ammo > 0 && !m_PowerCut)
				{
					TheShip.Weapons.FireRearWeapon();
				}
			}
			GamePadButtons buttons3 = ((GamePadState)(ref val)).Buttons;
			if (((int)((GamePadButtons)(ref buttons3)).X == 1 || ((KeyboardState)(ref val2)).IsKeyDown((Keys)88)) && !m_PowerCut)
			{
				TheShip.Weapons.FireSpecialWeapon();
			}
			GamePadButtons buttons4 = ((GamePadState)(ref val)).Buttons;
			if ((int)((GamePadButtons)(ref buttons4)).RightShoulder == 1)
			{
				GamePadButtons buttons5 = ((GamePadState)(ref m_LastPadState)).Buttons;
				if ((int)((GamePadButtons)(ref buttons5)).RightShoulder == 0)
				{
					goto IL_01c3;
				}
			}
			if (((KeyboardState)(ref val2)).IsKeyDown((Keys)88) && !((KeyboardState)(ref m_LastKeyState)).IsKeyDown((Keys)88))
			{
				goto IL_01c3;
			}
			GamePadButtons buttons6 = ((GamePadState)(ref val)).Buttons;
			if ((int)((GamePadButtons)(ref buttons6)).LeftShoulder == 1)
			{
				GamePadButtons buttons7 = ((GamePadState)(ref m_LastPadState)).Buttons;
				if ((int)((GamePadButtons)(ref buttons7)).LeftShoulder == 0)
				{
					goto IL_0218;
				}
			}
			if (((KeyboardState)(ref val2)).IsKeyDown((Keys)90) && !((KeyboardState)(ref m_LastKeyState)).IsKeyDown((Keys)90))
			{
				goto IL_0218;
			}
			goto IL_0228;
		}
		goto IL_0236;
		IL_01c3:
		TheShip.Weapons.CycleMainWeaponRight();
		goto IL_0228;
		IL_0236:
		base.Update();
		return;
		IL_0228:
		m_LastKeyState = val2;
		m_LastPadState = val;
		goto IL_0236;
		IL_0218:
		TheShip.Weapons.CycleMainWeaponLeft();
		goto IL_0228;
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
