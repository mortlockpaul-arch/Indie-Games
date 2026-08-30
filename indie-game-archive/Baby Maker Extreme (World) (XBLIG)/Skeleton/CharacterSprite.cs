using System.Collections.Generic;
using MathTools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Renderer;

namespace Skeleton;

public class CharacterSprite
{
	private RenderSprite m_sprite;

	private Joint m_child;

	private Joint m_parent;

	private Vector2 m_offset;

	private float m_fRotation;

	private Vector2 m_vScale;

	private float m_depth;

	private int m_type;

	private List<RenderSprite> m_typedSprites;

	private List<List<TypedAnimFrame>> m_typedAnims;

	private List<TypedAnimFrame> m_ActiveTypedAnim;

	private int m_iTypeAnimTimer;

	private RenderSprite m_eyeSprite;

	private Vector2 m_vEyeAim;

	public int Type => m_type;

	public float Depth
	{
		get
		{
			return m_depth;
		}
		set
		{
			if (IsMouth())
			{
				foreach (RenderSprite typedSprite in m_typedSprites)
				{
					typedSprite.Depth = value + (typedSprite.Depth - m_depth);
				}
			}
			else
			{
				m_sprite.Depth = value + (m_sprite.Depth - m_depth);
			}
			m_depth = value;
			if (IsEye())
			{
				m_eyeSprite.Depth = m_sprite.Depth + 1E-06f;
			}
		}
	}

	public float Rotation
	{
		get
		{
			return m_fRotation;
		}
		set
		{
			m_fRotation = value;
		}
	}

	public Vector2 SurfaceScale
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_vScale;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_vScale = value;
			if (m_type == 2)
			{
				m_eyeSprite.WidthScale = m_vScale.X;
			}
		}
	}

	public Vector2 Offset
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return m_offset;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			m_offset = value;
		}
	}

	public Joint Child => m_child;

	public CharacterSprite(RenderSprite spr, Joint child, int type)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		m_iTypeAnimTimer = 0;
		m_type = type;
		m_vEyeAim = default(Vector2);
		if (m_type == 1)
		{
			m_typedSprites = new List<RenderSprite>();
			m_typedSprites.Add(spr);
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					m_typedSprites.Add(SpriteManager.GetSprite("mouths", new Rectangle(i * 64, j * 32, 64, 32), default(Vector2), spr.Depth));
				}
			}
			m_typedAnims = new List<List<TypedAnimFrame>>();
			CharacterResourceManager.GetMouthData(m_typedAnims);
			m_ActiveTypedAnim = m_typedAnims[0];
		}
		else if (m_type == 2)
		{
			m_eyeSprite = SpriteManager.GetSprite("circleinner", default(Vector2), spr.Depth + 1E-05f);
			m_eyeSprite.WidthScale = spr.WidthScale / 2f;
			m_eyeSprite.Color = Color.Black;
		}
		m_sprite = spr;
		m_child = child;
		m_parent = child.GetParent();
		m_offset = -child.GetRelativePosition() / 2f;
		m_fRotation = 0f;
		m_vScale = m_sprite.SurfaceScale;
		m_depth = 1f;
	}

	public void SetColor(Color c)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		m_sprite.Color = c;
	}

	public bool IsMouth()
	{
		return m_type == 1;
	}

	public bool IsEye()
	{
		return m_type == 2;
	}

	public void SetTypedPose(int i)
	{
		if (m_type == 1)
		{
			while (i >= m_typedAnims.Count)
			{
				i -= m_typedAnims.Count;
			}
			while (i < 0)
			{
				i += m_typedAnims.Count;
			}
			if (m_ActiveTypedAnim != m_typedAnims[i])
			{
				m_iTypeAnimTimer = 0;
				m_ActiveTypedAnim = m_typedAnims[i];
			}
		}
	}

	public int GetTypedPose()
	{
		if (m_type == 1)
		{
			return m_typedAnims.IndexOf(m_ActiveTypedAnim);
		}
		return 0;
	}

	public void SetEyePosition(Vector2 v)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		m_vEyeAim = v;
		v -= m_child.GetPosition();
		((Vector2)(ref v)).Normalize();
		m_eyeSprite.Position = m_child.GetPosition() + 0.4f * (v * m_child.BoneScale * m_sprite.SurfaceScale / 2f);
	}

	public Vector2 GetEyePosition()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return m_vEyeAim;
	}

	public void Update(TimeTracker gameTime, float xMirrorPos)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		Vector2 boneScale = m_child.BoneScale;
		if (m_type == 1)
		{
			m_iTypeAnimTimer += gameTime.ElapsedMilli;
			int num = 0;
			while (m_ActiveTypedAnim[num].Time < m_iTypeAnimTimer && m_ActiveTypedAnim[num].Time != -1)
			{
				num++;
				if (num >= m_ActiveTypedAnim.Count)
				{
					m_iTypeAnimTimer -= m_ActiveTypedAnim[num - 1].Time;
					num = 0;
				}
			}
			m_sprite = m_typedSprites[m_ActiveTypedAnim[num].Index];
		}
		m_sprite.MirrorPos = xMirrorPos;
		m_sprite.Position = m_parent.GetPosition();
		m_sprite.Rotation = m_parent.GetCumulativeRotation() + m_fRotation;
		float angleFromVector = VectorTools.GetAngleFromVector(m_child.GetPosition() - m_parent.GetPosition());
		m_sprite.Origin = m_offset * boneScale;
		m_sprite.SurfaceScale = m_vScale * boneScale;
		m_sprite.Update(gameTime);
		if (IsEye())
		{
			SetEyePosition(m_vEyeAim);
			m_eyeSprite.SetSecondarySurfaceScale(0f - angleFromVector, boneScale);
			m_eyeSprite.Update(gameTime);
		}
	}

	public void Draw(TimeTracker gameTime)
	{
		m_sprite.Draw(gameTime);
		if (m_type == 2)
		{
			m_eyeSprite.Draw(gameTime);
		}
	}

	public RenderSprite GetSprite()
	{
		return m_sprite;
	}
}
