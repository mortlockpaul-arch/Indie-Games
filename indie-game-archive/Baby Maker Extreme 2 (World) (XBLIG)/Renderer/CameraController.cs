using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Renderer;

public class CameraController
{
	private const int rateOfChange = 50;

	private Effect m_defaultEffect;

	private Vector2 m_vOffset;

	private Vector2 m_vPosition;

	private float m_fZoom;

	private bool m_bIsShaking;

	private int offsetTimer;

	private int camShakeDuration;

	private int curCamShakeTime;

	private float m_fShakeStrength;

	private Vector2 m_vBumpOffset;

	private int m_iBumpTimer;

	private bool m_bIsBumped;

	private float m_fRotation;

	public Vector2 Position => m_vPosition;

	public float Zoom => m_fZoom;

	public CameraController(Effect e)
	{
		m_defaultEffect = e;
		m_vOffset = default(Vector2);
		m_vPosition = default(Vector2);
		m_fZoom = 1f;
		m_bIsShaking = false;
		offsetTimer = 0;
		m_vBumpOffset = new Vector2(0f, 0f);
		m_iBumpTimer = 0;
		m_bIsBumped = false;
		curCamShakeTime = 0;
		camShakeDuration = 3000;
		m_fShakeStrength = 10f;
		m_fRotation = 0f;
	}

	public float GetRotation()
	{
		return m_fRotation;
	}

	public void ZoomCamera(float val)
	{
		MoveCamera(m_vPosition, m_fRotation, m_fZoom + val);
	}

	public void RotateCamera(float val)
	{
		MoveCamera(m_vPosition, m_fRotation + val, m_fZoom);
	}

	public float GetZoom()
	{
		return m_fZoom;
	}

	public void PushCamera(Vector2 pos)
	{
		MoveCamera(m_vPosition + pos, m_fRotation, m_fZoom);
	}

	public void MoveCamera(Vector2 pos, float rotation, float zoom)
	{
		m_fZoom = zoom;
		m_fRotation = rotation;
		m_vPosition = pos;
		SceneRenderer.World = Matrix.Identity * Matrix.CreateTranslation(0f - m_vOffset.X, m_vOffset.Y, 0f) * Matrix.CreateTranslation(0f - (m_vPosition.X + m_vBumpOffset.X), m_vPosition.Y + m_vBumpOffset.Y, 0f) * Matrix.CreateScale(m_fZoom) * Matrix.CreateRotationZ(m_fRotation);
	}

	public void ShakeCamera(int i, float shakeStrength)
	{
		m_bIsShaking = true;
		curCamShakeTime = 0;
		camShakeDuration = i;
		offsetTimer = 0;
		m_fShakeStrength = shakeStrength;
	}

	public void BumpCampera(Vector2 bumpOffset, int revertTime)
	{
		m_bIsBumped = true;
		m_vBumpOffset += bumpOffset;
		m_iBumpTimer = revertTime;
	}

	public void Update(TimeTracker gameTime)
	{
		if (m_bIsShaking)
		{
			offsetTimer += gameTime.ElapsedMilli;
			curCamShakeTime += gameTime.ElapsedMilli;
			if (camShakeDuration > 0 && curCamShakeTime > camShakeDuration)
			{
				m_vOffset = default(Vector2);
				MoveCamera(m_vPosition, m_fRotation, m_fZoom);
				offsetTimer = 0;
				curCamShakeTime = 0;
				m_bIsShaking = false;
			}
			else
			{
				while (offsetTimer > 50)
				{
					float num = m_fShakeStrength;
					if (camShakeDuration > 0)
					{
						num *= 1f - Math.Abs(((float)curCamShakeTime - (float)camShakeDuration / 2f) / ((float)camShakeDuration / 2f));
					}
					float rand = SceneRenderer.GetRand(0f, (float)Math.PI * 2f);
					m_vOffset = num * new Vector2((float)Math.Sin(rand), (float)Math.Cos(rand));
					offsetTimer -= 50;
				}
			}
		}
		if (m_bIsBumped)
		{
			m_iBumpTimer -= gameTime.ElapsedMilli;
			if (m_iBumpTimer < 0)
			{
				m_vBumpOffset = default(Vector2);
				MoveCamera(m_vPosition, m_fRotation, m_fZoom);
				m_iBumpTimer = 0;
				m_bIsBumped = false;
			}
			else
			{
				Vector2 vector = m_vBumpOffset / (m_iBumpTimer + gameTime.ElapsedMilli);
				m_vBumpOffset -= vector * gameTime.ElapsedMilli;
			}
		}
		if (m_bIsBumped || m_bIsShaking)
		{
			MoveCamera(m_vPosition, m_fRotation, m_fZoom);
		}
	}
}
