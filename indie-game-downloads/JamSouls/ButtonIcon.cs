using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JamSouls;

public class ButtonIcon
{
	public enum STATE
	{
		NORMAL,
		OVER,
		PUSHED,
		DISABLED
	}

	public enum NEIGHTBOUR
	{
		RIGHT,
		LEFT,
		DOWN,
		UP
	}

	public delegate void OnEvent(ButtonIcon bt, STATE state, PlayerIndex pIndex, int KeyPressed);

	public const float FLASH_TIMER = 10f;

	public OnEvent OnEventCallBack;

	public int[] m_TexTab = new int[3];

	public int m_ColoredSelection;

	public ButtonIcon[] m_Neightbour = new ButtonIcon[4];

	public int m_nId;

	public SpriteBatch spritebatch;

	public string m_Name;

	public string m_RealName;

	public STATE m_state;

	public int m_PlayerControllerIdx;

	public bool m_bSelected;

	public float m_FlashTimer;

	public bool m_bSelectable = true;

	public bool m_bLocked;

	private Atlas m_Atlas;

	private BackgroundLayer m_MainLayer;

	private Sprite m_LockLayer;

	public Color m_UsedColor = Color.White;

	public int m_UserData;

	public bool m_bDrawLock = true;

	public ButtonIcon(BackgroundLayer layer, int baseTex, int OverTex, int DownTex, int nId, OnEvent EvtCallBack, Atlas atlas)
	{
		m_Atlas = atlas;
		m_TexTab[0] = baseTex;
		m_TexTab[1] = OverTex;
		m_TexTab[2] = DownTex;
		m_Neightbour[0] = null;
		m_Neightbour[1] = null;
		m_Neightbour[3] = null;
		m_Neightbour[2] = null;
		m_nId = nId;
		OnEventCallBack = (OnEvent)Delegate.Combine(OnEventCallBack, EvtCallBack);
		m_Name = layer.Name;
		m_MainLayer = layer;
		m_state = STATE.NORMAL;
		m_PlayerControllerIdx = -1;
		m_bSelected = false;
	}

	public void SetLocked(Sprite LockSprite)
	{
		m_bLocked = true;
		m_bSelectable = false;
		m_LockLayer = LockSprite;
	}

	public void SetNeightBour(NEIGHTBOUR neightbour, ButtonIcon NeightBourHood)
	{
		m_Neightbour[(int)neightbour] = NeightBourHood;
		switch (neightbour)
		{
		case NEIGHTBOUR.DOWN:
			if (NeightBourHood.m_Neightbour[3] == null)
			{
				NeightBourHood.SetNeightBour(NEIGHTBOUR.UP, this);
			}
			break;
		case NEIGHTBOUR.UP:
			if (NeightBourHood.m_Neightbour[2] == null)
			{
				NeightBourHood.SetNeightBour(NEIGHTBOUR.DOWN, this);
			}
			break;
		case NEIGHTBOUR.LEFT:
			if (NeightBourHood.m_Neightbour[0] == null)
			{
				NeightBourHood.SetNeightBour(NEIGHTBOUR.RIGHT, this);
			}
			break;
		case NEIGHTBOUR.RIGHT:
			if (NeightBourHood.m_Neightbour[1] == null)
			{
				NeightBourHood.SetNeightBour(NEIGHTBOUR.LEFT, this);
			}
			break;
		}
	}

	public bool ManageInput(GameTime gametime)
	{
		PlayerIndex playerControllerIdx = (PlayerIndex)m_PlayerControllerIdx;
		if (m_FlashTimer > 0f)
		{
			m_FlashTimer -= gametime.ElapsedGameTime.Milliseconds;
		}
		if (m_PlayerControllerIdx != -1)
		{
			ButtonIcon buttonIcon = null;
			if (!m_bSelected)
			{
				if (InputManager.GetKeyState(playerControllerIdx, 1) == ButtonState.Pressed)
				{
					buttonIcon = m_Neightbour[1];
					if (buttonIcon != null)
					{
						while (buttonIcon.GetState() == STATE.OVER || buttonIcon.GetState() == STATE.PUSHED)
						{
							buttonIcon = buttonIcon.m_Neightbour[1];
							if (buttonIcon == this)
							{
								buttonIcon = null;
								break;
							}
						}
					}
				}
				else if (InputManager.GetKeyState(playerControllerIdx, 3) == ButtonState.Pressed)
				{
					buttonIcon = m_Neightbour[0];
					if (buttonIcon != null)
					{
						while (buttonIcon.GetState() == STATE.OVER || buttonIcon.GetState() == STATE.PUSHED)
						{
							buttonIcon = buttonIcon.m_Neightbour[0];
							if (buttonIcon == this)
							{
								buttonIcon = null;
								break;
							}
						}
					}
				}
				else if (InputManager.GetKeyState(playerControllerIdx, 0) == ButtonState.Pressed)
				{
					buttonIcon = m_Neightbour[3];
				}
				else if (InputManager.GetKeyState(playerControllerIdx, 2) == ButtonState.Pressed)
				{
					buttonIcon = m_Neightbour[2];
				}
				else if (InputManager.GetKeyState(playerControllerIdx, 4) == ButtonState.Pressed && m_FlashTimer <= 0f && !m_bSelected && m_bSelectable)
				{
					m_FlashTimer = 10f;
					SetState(STATE.PUSHED);
					m_bSelected = true;
					OnEventCallBack(this, STATE.PUSHED, playerControllerIdx, 4);
					return true;
				}
			}
			if (InputManager.GetKeyState(playerControllerIdx, 5) == ButtonState.Pressed)
			{
				OnEventCallBack(this, STATE.PUSHED, playerControllerIdx, 5);
				if (m_bSelected)
				{
					SetFocus(playerControllerIdx);
				}
				return true;
			}
			if (buttonIcon != null && buttonIcon.GetState() != STATE.OVER && buttonIcon.GetState() != STATE.PUSHED)
			{
				buttonIcon.SetFocus(playerControllerIdx);
				m_PlayerControllerIdx = -1;
				SetState(STATE.NORMAL);
				OnEventCallBack(buttonIcon, STATE.OVER, playerControllerIdx, 3);
				return true;
			}
		}
		return false;
	}

	public string GetName()
	{
		return m_Name;
	}

	public string GetRealName()
	{
		return m_RealName;
	}

	public void SetFocus(PlayerIndex pIndex)
	{
		m_bSelected = false;
		m_PlayerControllerIdx = (int)pIndex;
		m_state = STATE.OVER;
	}

	public bool IsFocused()
	{
		return m_bSelected;
	}

	public void Unfocus()
	{
		m_bSelected = false;
		m_state = STATE.NORMAL;
		m_PlayerControllerIdx = -1;
	}

	public void SetState(STATE state)
	{
		m_state = state;
	}

	public STATE GetState()
	{
		return m_state;
	}

	public Vector2 GetPosition()
	{
		return m_MainLayer.GetPosition();
	}

	public Vector2 GetMiddle()
	{
		Vector2 position = m_MainLayer.GetPosition();
		return new Vector2(position.X + (float)(m_Atlas.GetSprite(m_TexTab[(int)m_state]).rect.Width / 2), position.X + (float)(m_Atlas.GetSprite(m_TexTab[(int)m_state]).rect.Height / 2));
	}

	public void Draw()
	{
		int num = m_TexTab[(int)m_state];
		m_Atlas.GetSprite(num);
		if (m_FlashTimer > 0f)
		{
			num = m_ColoredSelection;
		}
		m_Atlas.Draw(num, m_MainLayer.GetPosition(), m_MainLayer.GetSpriteEffect(), m_MainLayer.m_zOrder, m_UsedColor);
		if (m_bLocked && m_bDrawLock)
		{
			Vector2 middle = GetMiddle();
			middle.X -= m_LockLayer.Width / 2;
			middle.Y = 400f;
			m_LockLayer.Draw(middle, m_UsedColor);
		}
	}
}
