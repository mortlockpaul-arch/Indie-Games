using System;
using Microsoft.Xna.Framework.Graphics;

namespace Core;

public class Trigger
{
	public enum TRIGGER_STATE
	{
		IDLE,
		ACTIVE,
		OVER
	}

	public enum TRIGGER_TYPE
	{
		UNKNOWN,
		ZOOM,
		VIEW,
		USE,
		USE_SMALL,
		ZOOM_SMALL
	}

	public string m_class_name = "Trigger";

	public TRIGGER_STATE m_state;

	public TRIGGER_TYPE m_type;

	public bool m_activate_own;

	public bool m_enabled = true;

	public Trigger(TRIGGER_TYPE type)
	{
		m_type = type;
	}

	public virtual void Clear()
	{
	}

	public virtual void Activate()
	{
		m_state = TRIGGER_STATE.ACTIVE;
	}

	public virtual void Update(TimeSpan elapsed)
	{
	}

	public virtual void Draw(SpriteBatch SB)
	{
	}
}
