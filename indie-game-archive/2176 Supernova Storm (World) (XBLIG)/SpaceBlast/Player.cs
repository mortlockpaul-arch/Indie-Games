using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceBlast;

internal abstract class Player
{
	public readonly Ship TheShip;

	public bool IsActive = true;

	public int Kills;

	public Color LightColour = new Color(0, 0, 0, byte.MaxValue);

	protected ETeam m_Team;

	public readonly Gamer TheGamer;

	private Color m_NewLightColour = new Color(0, 0, 0, byte.MaxValue);

	private Color m_StartingLightColour = new Color(0, 0, 0, byte.MaxValue);

	private double m_ChangingColourStartTime;

	private bool m_ChangingColour;

	public double RespawnTime;

	private double m_MegaDamageTimeout;

	private double m_CloakTimeout;

	private double m_InvincibleTimeout;

	private double m_PowerCutTimeout;

	protected byte m_PlayerID;

	protected bool m_MegaDamage;

	protected bool m_Cloaked;

	protected bool m_Invincible;

	protected bool m_PowerCut;

	public readonly SoundEffectInstance EngineSound;

	public ETeam Team => m_Team;

	public double MegaDamageRemaining
	{
		get
		{
			if (!m_MegaDamage)
			{
				return 0.0;
			}
			return m_MegaDamageTimeout - TimeManager.TotalSeconds;
		}
	}

	public double CloakRemaining
	{
		get
		{
			if (!m_Cloaked)
			{
				return 0.0;
			}
			return m_CloakTimeout - TimeManager.TotalSeconds;
		}
	}

	public double InvincibilityRemaining
	{
		get
		{
			if (!m_Invincible)
			{
				return 0.0;
			}
			return m_InvincibleTimeout - TimeManager.TotalSeconds;
		}
	}

	public double PowerCutRemaining => Math.Max(0.0, m_PowerCut ? (m_PowerCutTimeout - TimeManager.TotalSeconds) : 0.0);

	public byte PlayerID => m_PlayerID;

	public bool IsMegaDamageActive => m_MegaDamage;

	public bool IsCloakActive => m_Cloaked;

	public bool IsInvincibile => m_Invincible;

	public bool IsPowerCut => m_PowerCut;

	public Player(byte playerid, Vector3 pos, Gamer gamer, ShipColor colour, ETeam team)
	{
		TheShip = new Ship(pos, this, colour);
		m_PlayerID = playerid;
		TheGamer = gamer;
		if (gamer != null)
		{
			TheGamer.Tag = this;
		}
		m_Team = team;
		EngineSound = MainGame.AudioMan.CreateEngineSound();
		Reset(newGame: true);
	}

	public abstract void Terminate();

	protected virtual void Reset(bool newGame)
	{
		if (newGame)
		{
			Kills = 0;
		}
		TheShip.Reset();
		m_MegaDamage = false;
		m_Cloaked = false;
		m_Invincible = false;
		m_PowerCut = false;
		m_MegaDamageTimeout = 0.0;
		m_CloakTimeout = 0.0;
		m_InvincibleTimeout = 0.0;
		m_PowerCutTimeout = 0.0;
		LightColour.PackedValue = 4278190080u;
		m_NewLightColour.PackedValue = 4278190080u;
		m_ChangingColourStartTime = 0.0;
		m_ChangingColour = false;
	}

	public virtual void Update()
	{
		if (!IsActive)
		{
			return;
		}
		if (m_MegaDamage && TimeManager.TotalSeconds > m_MegaDamageTimeout)
		{
			m_MegaDamage = false;
			m_MegaDamageTimeout = 0.0;
			ChangeColour();
		}
		if (m_Cloaked && TimeManager.TotalSeconds > m_CloakTimeout)
		{
			m_Cloaked = false;
			m_CloakTimeout = 0.0;
			ChangeColour();
			MainGame.AudioMan.Play(Sound.Decloak, TheShip.Position);
		}
		if (m_Invincible && TimeManager.TotalSeconds > m_InvincibleTimeout)
		{
			m_Invincible = false;
			m_InvincibleTimeout = 0.0;
			ChangeColour();
		}
		if (m_PowerCut && TimeManager.TotalSeconds > m_PowerCutTimeout)
		{
			m_PowerCut = false;
			m_PowerCutTimeout = 0.0;
		}
		if (m_ChangingColour)
		{
			double num = (TimeManager.TotalSeconds - m_ChangingColourStartTime) / GameConstants.PlayerLightChangeDuration;
			if (num >= 1.0)
			{
				LightColour.PackedValue = m_NewLightColour.PackedValue;
				m_ChangingColour = false;
				m_ChangingColourStartTime = 0.0;
			}
			else
			{
				uint num2 = (uint)((double)(m_NewLightColour.R - m_StartingLightColour.R) * num + (double)(int)m_StartingLightColour.R);
				uint num3 = (uint)((double)(m_NewLightColour.G - m_StartingLightColour.G) * num + (double)(int)m_StartingLightColour.G);
				uint num4 = (uint)((double)(m_NewLightColour.B - m_StartingLightColour.B) * num + (double)(int)m_StartingLightColour.B);
				uint num5 = (uint)((double)(m_NewLightColour.A - m_StartingLightColour.A) * num + (double)(int)m_StartingLightColour.A);
				LightColour.PackedValue = (num5 << 24) + (num2 << 16) + (num3 << 8) + num4;
			}
		}
	}

	public virtual void Respawn(RespawnLocation pos)
	{
		TheShip.Position = pos.Position;
		TheShip.Rotation = pos.Rotation;
		TheShip.Velocity = Vector3.Zero;
		Reset(newGame: false);
		IsActive = true;
		LightColour = new Color(0, 0, byte.MaxValue, byte.MaxValue);
		MakeInvincible(5f);
	}

	public void Draw(bool forceDraw)
	{
		if (IsActive)
		{
			TheShip.Draw(forceDraw);
		}
	}

	public virtual void Die(Player killedBy)
	{
		RespawnTime = TimeManager.TotalSeconds + 5.0;
		IsActive = false;
		TheShip.Die();
		if (killedBy != null)
		{
			MainGame.Players.IncreasePlayerScore(killedBy, this);
		}
	}

	public Color GetLightColour()
	{
		return LightColour;
	}

	public void ApplyMegaDamage(float duration)
	{
		m_MegaDamage = true;
		m_MegaDamageTimeout = TimeManager.TotalSeconds + (double)duration;
		ChangeColour();
	}

	public void ApplyCloak(float duration)
	{
		m_Cloaked = true;
		m_CloakTimeout = TimeManager.TotalSeconds + (double)duration;
		ChangeColour();
	}

	public void MakeInvincible(float duration)
	{
		m_Invincible = true;
		m_InvincibleTimeout = TimeManager.TotalSeconds + (double)duration;
		ChangeColour();
	}

	public void ApplyEMP(float duration)
	{
		m_PowerCut = true;
		m_PowerCutTimeout = Math.Max(m_PowerCutTimeout, TimeManager.TotalSeconds + (double)duration);
	}

	private void ChangeColour()
	{
		uint num = 0u;
		uint num2 = 0u;
		uint num3 = 0u;
		uint num4 = 255u;
		if (m_MegaDamage)
		{
			num = 255u;
		}
		if (m_Cloaked)
		{
			num2 = 255u;
			num4 = 0u;
		}
		if (m_Invincible)
		{
			num3 = 255u;
		}
		m_NewLightColour.PackedValue = (num4 << 24) + (num << 16) + (num2 << 8) + num3;
		m_StartingLightColour.PackedValue = LightColour.PackedValue;
		m_ChangingColourStartTime = TimeManager.TotalSeconds;
		m_ChangingColour = true;
	}

	public string GetGamerTag()
	{
		if (TheGamer != null)
		{
			return TheGamer.Gamertag;
		}
		if (this is HumanPlayer)
		{
			if (MainGame.Instance.RightPlayer != null)
			{
				if (this == MainGame.Instance.LeftPlayer)
				{
					return "Left Player";
				}
				return "Right Player";
			}
			return "Player";
		}
		return m_PlayerID switch
		{
			0 => "Cyborg: HAL 15k", 
			1 => "Cyborg: SAL 17k", 
			2 => "Cyborg: Super WOPR", 
			3 => "Cyborg: Extreme Deep Thought", 
			4 => "Cyborg: Blue Gene X", 
			5 => "Cyborg: Roadrunner 5", 
			6 => "Cyborg: Intrepid 4", 
			_ => "Cyborg: Jaguar XT9", 
		};
	}

	public Gamer GetGamer()
	{
		return TheGamer;
	}
}
