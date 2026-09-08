using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectMercury;
using ProjectMercury.Emitters;
using ProjectMercury.Modifiers;
using ProjectMercury.Renderers;

namespace JamSouls;

public class MercuryParticle : ScenaricEntitie
{
	public ParticleEffect m_pe;

	public bool m_bUseBlending;

	public Vector2 m_Location;

	public GameState m_GameStateInstance;

	public SpriteBatch m_Batch;

	public MercurySpriteBatchRenderer m_Renderer;

	public bool m_bAutoTrigger;

	public bool m_bNeverDies;

	public List<RectangleConstraintDeflector> m_RectangleConstraintList = new List<RectangleConstraintDeflector>();

	public List<RadialGravityModifier> m_RadialGravityModifier = new List<RadialGravityModifier>();

	public List<Vector2> m_RadialGravityModifierOffsets = new List<Vector2>();

	public Vector2 m_oldPos;

	public MercuryParticle(GameState GameStateInstance, int x, int y, ParticleEffect pe, string name, float zOrder, bool bUseBlending)
	{
		TypeId = SCENARIC.TYPE_PARTICLE;
		Name = name;
		m_GameStateInstance = GameStateInstance;
		m_bUseBlending = bUseBlending;
		m_bNeverDies = false;
		m_pe = pe;
		m_pe.LoadContent(m_GameStateInstance.content);
		m_pe.Initialise();
		m_Location = new Vector2(x, y);
		m_zOrder = zOrder;
		m_bUseBlending = bUseBlending;
		m_Batch = m_GameStateInstance.ScreenManager.SpriteBatch;
		m_Renderer = m_GameStateInstance.m_Renderer;
		m_bAutoTrigger = true;
		m_bVisible = true;
		m_oldPos = Vector2.Zero;
		InitConstraint();
	}

	public void InitConstraint()
	{
		for (int i = 0; i < m_pe.Count; i++)
		{
			for (int j = 0; j < m_pe[i].Modifiers.Count; j++)
			{
				switch (m_pe[i].Modifiers[j].GetType().ToString())
				{
				case "ProjectMercury.Modifiers.RectangleConstraintDeflector":
					m_RectangleConstraintList.Add((RectangleConstraintDeflector)m_pe[i].Modifiers[j]);
					break;
				case "ProjectMercury.Modifiers.RadialGravityModifier":
					m_RadialGravityModifier.Add((RadialGravityModifier)m_pe[i].Modifiers[j]);
					m_RadialGravityModifierOffsets.Add(m_RadialGravityModifier[m_RadialGravityModifier.Count - 1].Position);
					break;
				}
				if (m_pe[i].Term == 60f)
				{
					m_bNeverDies = true;
				}
			}
		}
	}

	public void SetParticleColor(Color color, Vector3 Variation)
	{
		for (int i = 0; i < m_pe.Count; i++)
		{
			m_pe[i].ReleaseColour.Value = color.ToVector3();
			m_pe[i].ReleaseColour.Variation = Variation;
		}
	}

	public void SetParticleColor(Color color)
	{
		for (int i = 0; i < m_pe.Count; i++)
		{
			m_pe[i].ReleaseColour.Value = color.ToVector3();
		}
	}

	public void SetAutoTrigger(bool bAutoTrigger)
	{
		m_bAutoTrigger = bAutoTrigger;
	}

	public override void SetPosition(Vector2 pos)
	{
		m_Location = pos;
	}

	public override Vector2 GetPosition()
	{
		return m_Location;
	}

	public override void Update(GameTime gameTime)
	{
		float num = (float)gameTime.ElapsedGameTime.Milliseconds * 0.001f;
		if (m_Location != m_oldPos)
		{
			for (int i = 0; i < m_RectangleConstraintList.Count; i++)
			{
				m_RectangleConstraintList[i].Position = m_Location;
			}
			if (m_RadialGravityModifier.Count > 0)
			{
				foreach (Emitter item in m_pe)
				{
					item.Initialise();
				}
				for (int j = 0; j < m_RadialGravityModifier.Count; j++)
				{
					m_RadialGravityModifier[j].Position = m_Location + m_RadialGravityModifierOffsets[j];
				}
			}
		}
		if (m_bNeverDies)
		{
			foreach (Emitter item2 in m_pe)
			{
				for (int k = 0; k < item2.Particles.Length; k++)
				{
					item2.Particles[k].Inception += num;
				}
			}
		}
		m_oldPos = m_Location;
		if (m_bAutoTrigger)
		{
			m_pe.Trigger(m_Location);
		}
		m_pe.Update(num);
	}

	public void Trigger(Vector2 location)
	{
		m_pe.Trigger(location);
	}

	public override void Draw()
	{
		if (!m_bUseBlending && m_bVisible)
		{
			for (int i = 0; i < m_pe.Count; i++)
			{
				RenderEffect(m_pe[i]);
			}
		}
	}

	public void DrawEffect()
	{
		if (m_bUseBlending && m_bVisible)
		{
			for (int i = 0; i < m_pe.Count; i++)
			{
				RenderEffectBlend(m_pe[i]);
			}
		}
	}

	public void RenderEffectBlend(Emitter emitter)
	{
		if (emitter.BlendMode != EmitterBlendMode.None && emitter.ParticleTexture != null && emitter.ActiveParticlesCount > 0)
		{
			Rectangle value = new Rectangle(0, 0, emitter.ParticleTexture.Width, emitter.ParticleTexture.Height);
			Vector2 origin = new Vector2((float)value.Width / 2f, (float)value.Height / 2f);
			BlendState blendState = m_Renderer.GetBlendState(emitter.BlendMode);
			m_Batch.Begin(SpriteSortMode.Immediate, blendState);
			for (int i = 0; i < emitter.ActiveParticlesCount; i++)
			{
				Particle particle = emitter.Particles[i];
				float scale = particle.Scale / (float)emitter.ParticleTexture.Width;
				m_Batch.Draw(emitter.ParticleTexture, particle.Position, value, new Color(particle.Colour), particle.Rotation, origin, scale, SpriteEffects.None, m_zOrder);
			}
			m_Batch.End();
		}
	}

	public void RenderEffect(Emitter emitter)
	{
		if (emitter.ParticleTexture != null && emitter.ActiveParticlesCount > 0)
		{
			Rectangle value = new Rectangle(0, 0, emitter.ParticleTexture.Width, emitter.ParticleTexture.Height);
			Vector2 origin = new Vector2((float)value.Width / 2f, (float)value.Height / 2f);
			for (int i = 0; i < emitter.ActiveParticlesCount; i++)
			{
				Particle particle = emitter.Particles[i];
				float scale = particle.Scale / (float)emitter.ParticleTexture.Width;
				m_Batch.Draw(emitter.ParticleTexture, particle.Position, value, new Color(particle.Colour), particle.Rotation, origin, scale, m_SpriteEffect, m_zOrder);
			}
		}
	}
}
