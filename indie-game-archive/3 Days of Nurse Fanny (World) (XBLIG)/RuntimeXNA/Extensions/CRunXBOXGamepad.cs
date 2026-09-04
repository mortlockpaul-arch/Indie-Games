using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RuntimeXNA.Actions;
using RuntimeXNA.Conditions;
using RuntimeXNA.Expressions;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;

namespace RuntimeXNA.Extensions;

internal class CRunXBOXGamepad : CRunExtension
{
	private const int CND_ISCONNECTED = 0;

	private const int CND_BUTTONA = 1;

	private const int CND_BUTTONB = 2;

	private const int CND_BUTTONX = 3;

	private const int CND_BUTTONY = 4;

	private const int CND_BUTTONBACK = 5;

	private const int CND_BUTTONBIGBUTTON = 6;

	private const int CND_BUTTONLEFTSHOULDER = 7;

	private const int CND_BUTTONLEFTSTICK = 8;

	private const int CND_BUTTONRIGHTSHOULDER = 9;

	private const int CND_BUTTONRIGHTSTICK = 10;

	private const int CND_BUTTONSTART = 11;

	private const int CND_DPADUP = 12;

	private const int CND_DPADDOWN = 13;

	private const int CND_DPADLEFT = 14;

	private const int CND_DPADRIGHT = 15;

	private const int CND_BUTTONS = 16;

	private const int CND_ANYBUTTON = 17;

	private const int CND_LAST = 18;

	private const int ACT_VIBRATE = 0;

	private const int EXP_STICKLEFTH = 0;

	private const int EXP_STICKLEFTV = 1;

	private const int EXP_STICKRIGHTH = 2;

	private const int EXP_STICKRIGHTV = 3;

	private const int EXP_TRIGGERLEFT = 4;

	private const int EXP_TRIGGERRIGHT = 5;

	private const int EXP_BUTTONA = 6;

	private const int EXP_BUTTONB = 7;

	private const int EXP_BUTTONX = 8;

	private const int EXP_BUTTONY = 9;

	private const int EXP_BUTTONBACK = 10;

	private const int EXP_BUTTONBIGBUTTON = 11;

	private const int EXP_BUTTONLEFTSHOULDER = 12;

	private const int EXP_BUTTONLEFTSTICK = 13;

	private const int EXP_BUTTONRIGHTSHOULDER = 14;

	private const int EXP_BUTTONRIGHTSTICK = 15;

	private const int EXP_BUTTONSTART = 16;

	private const int EXP_DPADUP = 17;

	private const int EXP_DPADDOWN = 18;

	private const int EXP_DPADLEFT = 19;

	private const int EXP_DPADRIGHT = 20;

	private const int EXP_BUTTONS = 21;

	private const int EXP_BUTTON = 22;

	private const int GPFLAG_NODEADZONE = 1;

	private int flags;

	private long[] timers;

	private GamePadState[] states;

	public override int getNumberOfConditions()
	{
		return 18;
	}

	public override bool createRunObject(CFile file, CCreateObjectInfo cob, int version)
	{
		flags = file.readAInt();
		timers = new long[4];
		for (int i = 0; i < 4; i++)
		{
			timers[i] = 0L;
		}
		states = new GamePadState[4];
		return true;
	}

	public override void destroyRunObject(bool bFast)
	{
		for (int i = 0; i < 4; i++)
		{
			GamePad.SetVibration(getPlayer(i), 0f, 0f);
		}
	}

	public override int handleRunObject()
	{
		long timer = ho.hoAdRunHeader.rhApp.timer;
		for (int i = 0; i < 4; i++)
		{
			if (timers[i] != 0 && timer > timers[i])
			{
				GamePad.SetVibration(getPlayer(i), 0f, 0f);
				timers[i] = 0L;
			}
		}
		GamePadDeadZone deadZoneMode = GamePadDeadZone.IndependentAxes;
		if ((flags & 1) == 0)
		{
			deadZoneMode = GamePadDeadZone.None;
		}
		ref GamePadState reference = ref states[0];
		reference = GamePad.GetState(PlayerIndex.One, deadZoneMode);
		ref GamePadState reference2 = ref states[1];
		reference2 = GamePad.GetState(PlayerIndex.Two, deadZoneMode);
		ref GamePadState reference3 = ref states[2];
		reference3 = GamePad.GetState(PlayerIndex.Three, deadZoneMode);
		ref GamePadState reference4 = ref states[3];
		reference4 = GamePad.GetState(PlayerIndex.Four, deadZoneMode);
		return 0;
	}

	public override void pauseRunObject()
	{
		for (int i = 0; i < 4; i++)
		{
			GamePad.SetVibration(getPlayer(i), 0f, 0f);
		}
	}

	public override bool condition(int num, CCndExtension cnd)
	{
		return num switch
		{
			0 => RCND_ISCONNECTED(cnd), 
			1 => RCND_BUTTONA(cnd), 
			2 => RCND_BUTTONB(cnd), 
			3 => RCND_BUTTONX(cnd), 
			4 => RCND_BUTTONY(cnd), 
			5 => RCND_BUTTONBACK(cnd), 
			6 => RCND_BUTTONBIGBUTTON(cnd), 
			7 => RCND_BUTTONLEFTSHOULDER(cnd), 
			8 => RCND_BUTTONLEFTSTICK(cnd), 
			9 => RCND_BUTTONRIGHTSHOULDER(cnd), 
			10 => RCND_BUTTONRIGHTSTICK(cnd), 
			11 => RCND_BUTTONSTART(cnd), 
			12 => RCND_DPADUP(cnd), 
			13 => RCND_DPADDOWN(cnd), 
			14 => RCND_DPADLEFT(cnd), 
			15 => RCND_DPADRIGHT(cnd), 
			16 => RCND_BUTTONS(cnd), 
			17 => RCND_ANYBUTTON(cnd), 
			_ => false, 
		};
	}

	private PlayerIndex getPlayer(int num)
	{
		return num switch
		{
			0 => PlayerIndex.One, 
			1 => PlayerIndex.Two, 
			2 => PlayerIndex.Three, 
			3 => PlayerIndex.Four, 
			_ => PlayerIndex.One, 
		};
	}

	private bool RCND_ISCONNECTED(CCndExtension cnd)
	{
		int paramExpression = cnd.getParamExpression(rh, 0);
		if (paramExpression >= 0 && paramExpression <= 3)
		{
			return GamePad.GetState(getPlayer(paramExpression)).IsConnected;
		}
		return false;
	}

	private bool RCND_BUTTONS(CCndExtension cnd)
	{
		int paramExpression = cnd.getParamExpression(rh, 0);
		int paramExpression2 = cnd.getParamExpression(rh, 1);
		return CndButtons(paramExpression, paramExpression2);
	}

	private bool RCND_ANYBUTTON(CCndExtension cnd)
	{
		int paramExpression = cnd.getParamExpression(rh, 0);
		for (int i = 0; i <= 13; i++)
		{
			if (CndButtons(paramExpression, i))
			{
				return true;
			}
		}
		return false;
	}

	private bool CndButtons(int player, int button)
	{
		for (int i = 0; i < 4; i++)
		{
			if ((player & (1 << i)) == 0)
			{
				continue;
			}
			switch (button)
			{
			case 0:
				if (states[i].Buttons.A == ButtonState.Pressed)
				{
					return true;
				}
				break;
			case 1:
				if (states[i].Buttons.B == ButtonState.Pressed)
				{
					return true;
				}
				break;
			case 2:
				if (states[i].Buttons.X == ButtonState.Pressed)
				{
					return true;
				}
				break;
			case 3:
				if (states[i].Buttons.Y == ButtonState.Pressed)
				{
					return true;
				}
				break;
			case 4:
				if (states[i].Buttons.LeftShoulder == ButtonState.Pressed)
				{
					return true;
				}
				break;
			case 5:
				if (states[i].Buttons.RightShoulder == ButtonState.Pressed)
				{
					return true;
				}
				break;
			case 6:
				if (states[i].Buttons.Back == ButtonState.Pressed)
				{
					return true;
				}
				break;
			case 7:
				if (states[i].Buttons.Start == ButtonState.Pressed)
				{
					return true;
				}
				break;
			case 8:
				if (states[i].Buttons.LeftStick == ButtonState.Pressed)
				{
					return true;
				}
				break;
			case 9:
				if (states[i].Buttons.RightStick == ButtonState.Pressed)
				{
					return true;
				}
				break;
			case 10:
				if (states[i].DPad.Up == ButtonState.Pressed)
				{
					return true;
				}
				break;
			case 11:
				if (states[i].DPad.Down == ButtonState.Pressed)
				{
					return true;
				}
				break;
			case 12:
				if (states[i].DPad.Left == ButtonState.Pressed)
				{
					return true;
				}
				break;
			case 13:
				if (states[i].DPad.Right == ButtonState.Pressed)
				{
					return true;
				}
				break;
			}
		}
		return false;
	}

	private bool RCND_BUTTONA(CCndExtension cnd)
	{
		int paramExpression = cnd.getParamExpression(rh, 0);
		for (int i = 0; i < 4; i++)
		{
			if ((paramExpression & (1 << i)) != 0 && states[i].Buttons.A == ButtonState.Pressed)
			{
				return true;
			}
		}
		return false;
	}

	private bool RCND_BUTTONB(CCndExtension cnd)
	{
		int paramExpression = cnd.getParamExpression(rh, 0);
		for (int i = 0; i < 4; i++)
		{
			if ((paramExpression & (1 << i)) != 0 && states[i].Buttons.B == ButtonState.Pressed)
			{
				return true;
			}
		}
		return false;
	}

	private bool RCND_BUTTONX(CCndExtension cnd)
	{
		int paramExpression = cnd.getParamExpression(rh, 0);
		for (int i = 0; i < 4; i++)
		{
			if ((paramExpression & (1 << i)) != 0 && states[i].Buttons.X == ButtonState.Pressed)
			{
				return true;
			}
		}
		return false;
	}

	private bool RCND_BUTTONY(CCndExtension cnd)
	{
		int paramExpression = cnd.getParamExpression(rh, 0);
		for (int i = 0; i < 4; i++)
		{
			if ((paramExpression & (1 << i)) != 0 && states[i].Buttons.Y == ButtonState.Pressed)
			{
				return true;
			}
		}
		return false;
	}

	private bool RCND_BUTTONBACK(CCndExtension cnd)
	{
		int paramExpression = cnd.getParamExpression(rh, 0);
		for (int i = 0; i < 4; i++)
		{
			if ((paramExpression & (1 << i)) != 0 && states[i].Buttons.Back == ButtonState.Pressed)
			{
				return true;
			}
		}
		return false;
	}

	private bool RCND_BUTTONBIGBUTTON(CCndExtension cnd)
	{
		int paramExpression = cnd.getParamExpression(rh, 0);
		for (int i = 0; i < 4; i++)
		{
			if ((paramExpression & (1 << i)) != 0 && states[i].Buttons.BigButton == ButtonState.Pressed)
			{
				return true;
			}
		}
		return false;
	}

	private bool RCND_BUTTONLEFTSHOULDER(CCndExtension cnd)
	{
		int paramExpression = cnd.getParamExpression(rh, 0);
		for (int i = 0; i < 4; i++)
		{
			if ((paramExpression & (1 << i)) != 0 && states[i].Buttons.LeftShoulder == ButtonState.Pressed)
			{
				return true;
			}
		}
		return false;
	}

	private bool RCND_BUTTONRIGHTSHOULDER(CCndExtension cnd)
	{
		int paramExpression = cnd.getParamExpression(rh, 0);
		for (int i = 0; i < 4; i++)
		{
			if ((paramExpression & (1 << i)) != 0 && states[i].Buttons.RightShoulder == ButtonState.Pressed)
			{
				return true;
			}
		}
		return false;
	}

	private bool RCND_BUTTONLEFTSTICK(CCndExtension cnd)
	{
		int paramExpression = cnd.getParamExpression(rh, 0);
		for (int i = 0; i < 4; i++)
		{
			if ((paramExpression & (1 << i)) != 0 && states[i].Buttons.LeftStick == ButtonState.Pressed)
			{
				return true;
			}
		}
		return false;
	}

	private bool RCND_BUTTONRIGHTSTICK(CCndExtension cnd)
	{
		int paramExpression = cnd.getParamExpression(rh, 0);
		for (int i = 0; i < 4; i++)
		{
			if ((paramExpression & (1 << i)) != 0 && states[i].Buttons.RightStick == ButtonState.Pressed)
			{
				return true;
			}
		}
		return false;
	}

	private bool RCND_BUTTONSTART(CCndExtension cnd)
	{
		int paramExpression = cnd.getParamExpression(rh, 0);
		for (int i = 0; i < 4; i++)
		{
			if ((paramExpression & (1 << i)) != 0 && states[i].Buttons.Start == ButtonState.Pressed)
			{
				return true;
			}
		}
		return false;
	}

	private bool RCND_DPADUP(CCndExtension cnd)
	{
		int paramExpression = cnd.getParamExpression(rh, 0);
		for (int i = 0; i < 4; i++)
		{
			if ((paramExpression & (1 << i)) != 0 && states[i].DPad.Up == ButtonState.Pressed)
			{
				return true;
			}
		}
		return false;
	}

	private bool RCND_DPADDOWN(CCndExtension cnd)
	{
		int paramExpression = cnd.getParamExpression(rh, 0);
		for (int i = 0; i < 4; i++)
		{
			if ((paramExpression & (1 << i)) != 0 && states[i].DPad.Down == ButtonState.Pressed)
			{
				return true;
			}
		}
		return false;
	}

	private bool RCND_DPADLEFT(CCndExtension cnd)
	{
		int paramExpression = cnd.getParamExpression(rh, 0);
		for (int i = 0; i < 4; i++)
		{
			if ((paramExpression & (1 << i)) != 0 && states[i].DPad.Left == ButtonState.Pressed)
			{
				return true;
			}
		}
		return false;
	}

	private bool RCND_DPADRIGHT(CCndExtension cnd)
	{
		int paramExpression = cnd.getParamExpression(rh, 0);
		for (int i = 0; i < 4; i++)
		{
			if ((paramExpression & (1 << i)) != 0 && states[i].DPad.Right == ButtonState.Pressed)
			{
				return true;
			}
		}
		return false;
	}

	public override void action(int num, CActExtension act)
	{
		if (num == 0)
		{
			RACT_VIBRATE(act);
		}
	}

	private void RACT_VIBRATE(CActExtension act)
	{
		int paramExpression = act.getParamExpression(rh, 0);
		float leftMotor = (float)((double)act.getParamExpression(rh, 1) / 100.0);
		float rightMotor = (float)((double)act.getParamExpression(rh, 2) / 100.0);
		int paramExpression2 = act.getParamExpression(rh, 3);
		for (int i = 0; i < 4; i++)
		{
			if ((paramExpression & (1 << i)) != 0)
			{
				PlayerIndex player = getPlayer(i);
				timers[i] = ho.hoAdRunHeader.rhApp.timer + paramExpression2;
				GamePad.SetVibration(player, leftMotor, rightMotor);
			}
		}
	}

	public override CValue expression(int num)
	{
		return num switch
		{
			0 => REXP_STICKLEFTH(), 
			1 => REXP_STICKLEFTV(), 
			2 => REXP_STICKRIGHTH(), 
			3 => REXP_STICKRIGHTV(), 
			4 => REXP_TRIGGERLEFT(), 
			5 => REXP_TRIGGERRIGHT(), 
			6 => REXP_BUTTONA(), 
			7 => REXP_BUTTONB(), 
			8 => REXP_BUTTONX(), 
			9 => REXP_BUTTONY(), 
			10 => REXP_BUTTONBACK(), 
			16 => REXP_BUTTONSTART(), 
			11 => REXP_BUTTONBIGBUTTON(), 
			12 => REXP_BUTTONLEFTSHOULDER(), 
			14 => REXP_BUTTONRIGHTSHOULDER(), 
			17 => REXP_BUTTONDPADUP(), 
			18 => REXP_BUTTONDPADDOWN(), 
			19 => REXP_BUTTONDPADLEFT(), 
			20 => REXP_BUTTONDPADRIGHT(), 
			13 => REXP_BUTTONLEFTSTICK(), 
			15 => REXP_BUTTONRIGHTSTICK(), 
			22 => REXP_BUTTON(), 
			_ => new CValue(0), 
		};
	}

	private CValue REXP_STICKLEFTH()
	{
		int num = ho.getExpParam().getInt();
		int num2 = 0;
		for (int i = 0; i < 4; i++)
		{
			if ((num & (1 << i)) != 0)
			{
				num2 = (int)(states[i].ThumbSticks.Left.X * 100f);
				if (num2 != 0)
				{
					break;
				}
			}
		}
		return new CValue(num2);
	}

	private CValue REXP_STICKLEFTV()
	{
		int num = ho.getExpParam().getInt();
		int num2 = 0;
		for (int i = 0; i < 4; i++)
		{
			if ((num & (1 << i)) != 0)
			{
				num2 = -(int)(states[i].ThumbSticks.Left.Y * 100f);
				if (num2 != 0)
				{
					break;
				}
			}
		}
		return new CValue(num2);
	}

	private CValue REXP_STICKRIGHTH()
	{
		int num = ho.getExpParam().getInt();
		int num2 = 0;
		for (int i = 0; i < 4; i++)
		{
			if ((num & (1 << i)) != 0)
			{
				num2 = (int)(states[i].ThumbSticks.Right.X * 100f);
				if (num2 != 0)
				{
					break;
				}
			}
		}
		return new CValue(num2);
	}

	private CValue REXP_STICKRIGHTV()
	{
		int num = ho.getExpParam().getInt();
		int num2 = 0;
		for (int i = 0; i < 4; i++)
		{
			if ((num & (1 << i)) != 0)
			{
				num2 = -(int)(states[i].ThumbSticks.Right.Y * 100f);
				if (num2 != 0)
				{
					break;
				}
			}
		}
		return new CValue(num2);
	}

	private CValue REXP_TRIGGERLEFT()
	{
		int num = ho.getExpParam().getInt();
		int num2 = 0;
		for (int i = 0; i < 4; i++)
		{
			if ((num & (1 << i)) != 0)
			{
				num2 = (int)(states[i].Triggers.Left * 100f);
				if (num2 != 0)
				{
					break;
				}
			}
		}
		return new CValue(num2);
	}

	private CValue REXP_TRIGGERRIGHT()
	{
		int num = ho.getExpParam().getInt();
		int num2 = 0;
		for (int i = 0; i < 4; i++)
		{
			if ((num & (1 << i)) != 0)
			{
				num2 = (int)(states[i].Triggers.Right * 100f);
				if (num2 != 0)
				{
					break;
				}
			}
		}
		return new CValue(num2);
	}

	private CValue REXP_BUTTONS()
	{
		int player = ho.getExpParam().getInt();
		int button = ho.getExpParam().getInt();
		return new CValue(ExpButtons(player, button));
	}

	private CValue REXP_BUTTON()
	{
		int player = ho.getExpParam().getInt();
		for (int i = 0; i <= 13; i++)
		{
			if (ExpButtons(player, i) != 0)
			{
				return new CValue(i);
			}
		}
		return new CValue(-1);
	}

	private int ExpButtons(int player, int button)
	{
		int num = 0;
		for (int i = 0; i < 4; i++)
		{
			if ((player & (1 << i)) != 0)
			{
				switch (button)
				{
				case 0:
					if (states[i].Buttons.A == ButtonState.Pressed)
					{
						num = 1;
					}
					break;
				case 1:
					if (states[i].Buttons.B == ButtonState.Pressed)
					{
						num = 1;
					}
					break;
				case 2:
					if (states[i].Buttons.X == ButtonState.Pressed)
					{
						num = 1;
					}
					break;
				case 3:
					if (states[i].Buttons.Y == ButtonState.Pressed)
					{
						num = 1;
					}
					break;
				case 4:
					if (states[i].Buttons.LeftShoulder == ButtonState.Pressed)
					{
						num = 1;
					}
					break;
				case 5:
					if (states[i].Buttons.RightShoulder == ButtonState.Pressed)
					{
						num = 1;
					}
					break;
				case 6:
					if (states[i].Buttons.Back == ButtonState.Pressed)
					{
						num = 1;
					}
					break;
				case 7:
					if (states[i].Buttons.Start == ButtonState.Pressed)
					{
						num = 1;
					}
					break;
				case 8:
					if (states[i].Buttons.LeftStick == ButtonState.Pressed)
					{
						num = 1;
					}
					break;
				case 9:
					if (states[i].Buttons.RightStick == ButtonState.Pressed)
					{
						num = 1;
					}
					break;
				case 10:
					if (states[i].DPad.Up == ButtonState.Pressed)
					{
						num = 1;
					}
					break;
				case 11:
					if (states[i].DPad.Down == ButtonState.Pressed)
					{
						num = 1;
					}
					break;
				case 12:
					if (states[i].DPad.Left == ButtonState.Pressed)
					{
						num = 1;
					}
					break;
				case 13:
					if (states[i].DPad.Right == ButtonState.Pressed)
					{
						num = 1;
					}
					break;
				}
			}
			if (num != 0)
			{
				break;
			}
		}
		return num;
	}

	private CValue REXP_BUTTONA()
	{
		int num = ho.getExpParam().getInt();
		int i = 0;
		for (int j = 0; j < 4; j++)
		{
			if ((num & (1 << j)) != 0 && states[j].Buttons.A == ButtonState.Pressed)
			{
				i = 1;
				break;
			}
		}
		return new CValue(i);
	}

	private CValue REXP_BUTTONB()
	{
		int num = ho.getExpParam().getInt();
		int i = 0;
		for (int j = 0; j < 4; j++)
		{
			if ((num & (1 << j)) != 0 && states[j].Buttons.B == ButtonState.Pressed)
			{
				i = 1;
				break;
			}
		}
		return new CValue(i);
	}

	private CValue REXP_BUTTONX()
	{
		int num = ho.getExpParam().getInt();
		int i = 0;
		for (int j = 0; j < 4; j++)
		{
			if ((num & (1 << j)) != 0 && states[j].Buttons.X == ButtonState.Pressed)
			{
				i = 1;
				break;
			}
		}
		return new CValue(i);
	}

	private CValue REXP_BUTTONY()
	{
		int num = ho.getExpParam().getInt();
		int i = 0;
		for (int j = 0; j < 4; j++)
		{
			if ((num & (1 << j)) != 0 && states[j].Buttons.Y == ButtonState.Pressed)
			{
				i = 1;
				break;
			}
		}
		return new CValue(i);
	}

	private CValue REXP_BUTTONBIGBUTTON()
	{
		int num = ho.getExpParam().getInt();
		int i = 0;
		for (int j = 0; j < 4; j++)
		{
			if ((num & (1 << j)) != 0 && states[j].Buttons.BigButton == ButtonState.Pressed)
			{
				i = 1;
				break;
			}
		}
		return new CValue(i);
	}

	private CValue REXP_BUTTONSTART()
	{
		int num = ho.getExpParam().getInt();
		int i = 0;
		for (int j = 0; j < 4; j++)
		{
			if ((num & (1 << j)) != 0 && states[j].Buttons.Start == ButtonState.Pressed)
			{
				i = 1;
				break;
			}
		}
		return new CValue(i);
	}

	private CValue REXP_BUTTONBACK()
	{
		int num = ho.getExpParam().getInt();
		int i = 0;
		for (int j = 0; j < 4; j++)
		{
			if ((num & (1 << j)) != 0 && states[j].Buttons.Back == ButtonState.Pressed)
			{
				i = 1;
				break;
			}
		}
		return new CValue(i);
	}

	private CValue REXP_BUTTONLEFTSHOULDER()
	{
		int num = ho.getExpParam().getInt();
		int i = 0;
		for (int j = 0; j < 4; j++)
		{
			if ((num & (1 << j)) != 0 && states[j].Buttons.LeftShoulder == ButtonState.Pressed)
			{
				i = 1;
				break;
			}
		}
		return new CValue(i);
	}

	private CValue REXP_BUTTONRIGHTSHOULDER()
	{
		int num = ho.getExpParam().getInt();
		int i = 0;
		for (int j = 0; j < 4; j++)
		{
			if ((num & (1 << j)) != 0 && states[j].Buttons.RightShoulder == ButtonState.Pressed)
			{
				i = 1;
				break;
			}
		}
		return new CValue(i);
	}

	private CValue REXP_BUTTONDPADUP()
	{
		int num = ho.getExpParam().getInt();
		int i = 0;
		for (int j = 0; j < 4; j++)
		{
			if ((num & (1 << j)) != 0 && states[j].DPad.Up == ButtonState.Pressed)
			{
				i = 1;
				break;
			}
		}
		return new CValue(i);
	}

	private CValue REXP_BUTTONDPADDOWN()
	{
		int num = ho.getExpParam().getInt();
		int i = 0;
		for (int j = 0; j < 4; j++)
		{
			if ((num & (1 << j)) != 0 && states[j].DPad.Down == ButtonState.Pressed)
			{
				i = 1;
				break;
			}
		}
		return new CValue(i);
	}

	private CValue REXP_BUTTONDPADLEFT()
	{
		int num = ho.getExpParam().getInt();
		int i = 0;
		for (int j = 0; j < 4; j++)
		{
			if ((num & (1 << j)) != 0 && states[j].DPad.Left == ButtonState.Pressed)
			{
				i = 1;
				break;
			}
		}
		return new CValue(i);
	}

	private CValue REXP_BUTTONDPADRIGHT()
	{
		int num = ho.getExpParam().getInt();
		int i = 0;
		for (int j = 0; j < 4; j++)
		{
			if ((num & (1 << j)) != 0 && states[j].DPad.Right == ButtonState.Pressed)
			{
				i = 1;
				break;
			}
		}
		return new CValue(i);
	}

	private CValue REXP_BUTTONLEFTSTICK()
	{
		int num = ho.getExpParam().getInt();
		int i = 0;
		for (int j = 0; j < 4; j++)
		{
			if ((num & (1 << j)) != 0 && states[j].Buttons.LeftStick == ButtonState.Pressed)
			{
				i = 1;
				break;
			}
		}
		return new CValue(i);
	}

	private CValue REXP_BUTTONRIGHTSTICK()
	{
		int num = ho.getExpParam().getInt();
		int i = 0;
		for (int j = 0; j < 4; j++)
		{
			if ((num & (1 << j)) != 0 && states[j].Buttons.RightStick == ButtonState.Pressed)
			{
				i = 1;
				break;
			}
		}
		return new CValue(i);
	}
}
