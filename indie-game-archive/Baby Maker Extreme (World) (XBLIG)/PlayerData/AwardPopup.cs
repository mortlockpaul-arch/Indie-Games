using System;
using Microsoft.Xna.Framework;
using Renderer;
using Scene;

namespace PlayerData;

public class AwardPopup
{
	private const int QUIET_TIME = 1000;

	private const int ENTER_TIME = 200;

	private const int EXIT_TIME = 200;

	private RenderSprite m_spr;

	private int m_iTimer;

	private Vector2 m_vStartScale;

	private static int sm_iNumSpawned;

	public AwardPopup(PropType type)
	{
		switch (type)
		{
		case PropType.CRAB_MEAL:
			m_spr = SpriteManager.GetSprite("images/trophies/gotCrabs", default(Vector2), 5000 + sm_iNumSpawned);
			break;
		case PropType.DOCTOR:
		case PropType.SURGEON:
			m_spr = SpriteManager.GetSprite("images/trophies/docBlock", default(Vector2), 5000 + sm_iNumSpawned);
			break;
		case PropType.LUNCHLADY:
			m_spr = SpriteManager.GetSprite("images/trophies/eatingOut", default(Vector2), 5000 + sm_iNumSpawned);
			break;
		case PropType.GURNEY:
			m_spr = SpriteManager.GetSprite("images/trophies/onBottom", default(Vector2), 5000 + sm_iNumSpawned);
			break;
		case PropType.CRUTCHES:
			m_spr = SpriteManager.GetSprite("images/trophies/tripod", default(Vector2), 5000 + sm_iNumSpawned);
			break;
		case PropType.RECEPTION_DESK:
			m_spr = SpriteManager.GetSprite("images/trophies/blonds", default(Vector2), 5000 + sm_iNumSpawned);
			break;
		case PropType.LAB_MICROSCOPE_TABLE:
			m_spr = SpriteManager.GetSprite("images/trophies/enlargement", default(Vector2), 5000 + sm_iNumSpawned);
			break;
		case PropType.BEDSIDE_TABLE_FLOWER:
			m_spr = SpriteManager.GetSprite("images/trophies/deflower", default(Vector2), 5000 + sm_iNumSpawned);
			break;
		case PropType.SURGERY_LIGHT:
			m_spr = SpriteManager.GetSprite("images/trophies/lightsout", default(Vector2), 5000 + sm_iNumSpawned);
			break;
		case PropType.BYPASS_MACHINE:
			m_spr = SpriteManager.GetSprite("images/trophies/pumpIt", default(Vector2), 5000 + sm_iNumSpawned);
			break;
		case PropType.LAB_PILL_TABLE:
			m_spr = SpriteManager.GetSprite("images/trophies/pill", default(Vector2), 5000 + sm_iNumSpawned);
			break;
		case PropType.SKELETON:
			m_spr = SpriteManager.GetSprite("images/trophies/ridebones", default(Vector2), 5000 + sm_iNumSpawned);
			break;
		case PropType.LIMB_TABLE:
			m_spr = SpriteManager.GetSprite("images/trophies/strapon", default(Vector2), 5000 + sm_iNumSpawned);
			break;
		case PropType.DEAD_BODY:
			m_spr = SpriteManager.GetSprite("images/trophies/stiffy", default(Vector2), 5000 + sm_iNumSpawned);
			break;
		case PropType.XRAY:
			m_spr = SpriteManager.GetSprite("images/trophies/seeinside", default(Vector2), 5000 + sm_iNumSpawned);
			break;
		case PropType.WALL:
			m_spr = SpriteManager.GetSprite("images/trophies/brokenprotect", default(Vector2), 5000 + sm_iNumSpawned);
			break;
		}
		m_vStartScale = m_spr.SurfaceScale;
		sm_iNumSpawned++;
	}

	public void Update(TimeTracker gameTime)
	{
		m_iTimer += gameTime.ElapsedMilli;
		if (m_iTimer < 200)
		{
			m_spr.SurfaceScale = m_vStartScale * (float)Math.Sin(2f * ((float)m_iTimer / 200f));
		}
		else if (m_iTimer < 1200)
		{
			m_spr.SurfaceScale = m_vStartScale;
		}
		else
		{
			m_spr.SurfaceScale = m_vStartScale * (float)Math.Cos(2f * ((float)(m_iTimer - 1200) / 200f));
		}
		m_spr.Position = SceneRenderer.GetCameraPosition() + new Vector2(0f, 200f) + new Vector2(SceneRenderer.GetRand(-5f, 5f), SceneRenderer.GetRand(-5f, 5f));
		m_spr.Rotation = SceneRenderer.GetRand(-0.05f, 0.05f);
	}

	public void Draw(TimeTracker gameTime)
	{
		m_spr.Draw(gameTime);
	}

	public void ForceExit()
	{
		if (m_iTimer < 1200)
		{
			m_iTimer = 1200;
		}
	}

	public bool IsActive()
	{
		return m_iTimer < 1400;
	}
}
