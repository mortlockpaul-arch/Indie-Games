using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Renderer;

public class CameraManager
{
	private const int rateOfChange = 50;

	private BasicEffect m_defaultEffect;

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

	public Vector2 Position
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_vPosition;
		}
	}

	public float Zoom => m_fZoom;

	public CameraManager(BasicEffect e)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		m_defaultEffect = e;
		m_vOffset = default(Vector2);
		m_vPosition = default(Vector2);
		m_vPosition = SceneRenderer.GetScreenDim() / 2f;
		m_fZoom = 1f;
		m_bIsShaking = true;
		offsetTimer = 0;
		m_vBumpOffset = new Vector2(300f, 0f);
		m_iBumpTimer = 10000;
		m_bIsBumped = true;
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
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		MoveCamera(m_vPosition, m_fRotation, m_fZoom + val);
	}

	public float GetZoom()
	{
		return m_fZoom;
	}

	public void PushCamera(Vector2 pos)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		MoveCamera(m_vPosition + pos, m_fRotation, m_fZoom);
	}

	public void MoveCamera(Vector2 pos, float rotation, float zoom)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		m_fZoom = zoom;
		m_fRotation = rotation;
		m_vPosition = pos;
		Vector2 screenDim = SceneRenderer.GetScreenDim();
		screenDim *= m_fZoom;
		Rectangle projectionRectSpace = default(Rectangle);
		((Rectangle)(ref projectionRectSpace))._002Ector((int)(m_vPosition.X + m_vOffset.X + m_vBumpOffset.X - screenDim.X / 2f), (int)(m_vPosition.Y + m_vOffset.Y + m_vBumpOffset.Y - screenDim.Y / 2f), (int)screenDim.X, (int)screenDim.Y);
		SceneRenderer.SetProjectionRectSpace(projectionRectSpace);
		Vector2 val = m_vPosition + m_vOffset + m_vBumpOffset - screenDim / 2f;
		Vector2 val2 = val + screenDim;
		Matrix val3 = Matrix.CreateOrthographicOffCenter(val.X, val2.X, val2.Y, val.Y, 1f, 1000f);
		if (m_fRotation != 0f)
		{
			m_defaultEffect.Projection = Matrix.CreateRotationZ(m_fRotation) * val3;
		}
		else
		{
			m_defaultEffect.Projection = val3;
		}
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
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		m_bIsBumped = true;
		m_vBumpOffset += bumpOffset;
		m_iBumpTimer = revertTime;
	}

	public void Update(TimeTracker gameTime)
	{
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
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
				Vector2 val = m_vBumpOffset / (float)(m_iBumpTimer + gameTime.ElapsedMilli);
				m_vBumpOffset -= val * (float)gameTime.ElapsedMilli;
			}
		}
		if (m_bIsBumped || m_bIsShaking)
		{
			MoveCamera(m_vPosition, m_fRotation, m_fZoom);
		}
	}
}
