using System;
using System.Runtime.CompilerServices;
using Kobingo.Xna.Library.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kobingo.Xna.Library.Graphics.Particles;

public class ParticleSystem<T> : Actor where T : Particle, new()
{
	private int m_MinParticleSpawnCount = 1;

	private int m_MaxSpawnParticles = 3;

	private int m_MinParticleLifespan = 1000;

	private int m_MaxParticleLifespan = 1000;

	private float m_MinParticleScaling = 1f;

	private float m_MaxParticleScaling = 1f;

	private float m_MinParticleAngle;

	private float m_MaxParticleAngle = (float)Math.PI * 2f;

	private float m_MinParticleSpeed = 1f;

	private float m_MaxParticleSpeed = 2f;

	private float m_MinParticleAcceleration;

	private float m_MaxParticleAcceleration;

	private float m_MinParticleRotation;

	private float m_MaxParticleRotation;

	private float m_MinParticleSpinning;

	private float m_MaxParticleSpinning;

	private float m_MinParticleSpawnRadius;

	private float m_MaxParticleSpawnRadius;

	public Manager<T> Particles { get; protected set; }

	public Texture2D ParticleTexture { get; private set; }

	public SpriteBlendMode BlendMode
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CBlendMode_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CBlendMode_003Ek__BackingField = value;
		}
	}

	public SpriteBatch SpriteBatch { get; private set; }

	public int MinParticleSpawnCount
	{
		get
		{
			return m_MinParticleSpawnCount;
		}
		set
		{
			m_MinParticleSpawnCount = value;
		}
	}

	public int MaxSpawnParticles
	{
		get
		{
			return m_MaxSpawnParticles;
		}
		set
		{
			m_MaxSpawnParticles = value;
		}
	}

	public int MinParticleLifespan
	{
		get
		{
			return m_MinParticleLifespan;
		}
		set
		{
			m_MinParticleLifespan = value;
		}
	}

	public int MaxParticleLifespan
	{
		get
		{
			return m_MaxParticleLifespan;
		}
		set
		{
			m_MaxParticleLifespan = value;
		}
	}

	public float MinParticleScaling
	{
		get
		{
			return m_MinParticleScaling;
		}
		set
		{
			m_MinParticleScaling = value;
		}
	}

	public float MaxParticleScaling
	{
		get
		{
			return m_MaxParticleScaling;
		}
		set
		{
			m_MaxParticleScaling = value;
		}
	}

	public float MinParticleAngle
	{
		get
		{
			return m_MinParticleAngle;
		}
		set
		{
			m_MinParticleAngle = value;
		}
	}

	public float MaxParticleAngle
	{
		get
		{
			return m_MaxParticleAngle;
		}
		set
		{
			m_MaxParticleAngle = value;
		}
	}

	public float MinParticleSpeed
	{
		get
		{
			return m_MinParticleSpeed;
		}
		set
		{
			m_MinParticleSpeed = value;
		}
	}

	public float MaxParticleSpeed
	{
		get
		{
			return m_MaxParticleSpeed;
		}
		set
		{
			m_MaxParticleSpeed = value;
		}
	}

	public float MinParticleAcceleration
	{
		get
		{
			return m_MinParticleAcceleration;
		}
		set
		{
			m_MinParticleAcceleration = value;
		}
	}

	public float MaxParticleAcceleration
	{
		get
		{
			return m_MaxParticleAcceleration;
		}
		set
		{
			m_MaxParticleAcceleration = value;
		}
	}

	public float MinParticleRotation
	{
		get
		{
			return m_MinParticleRotation;
		}
		set
		{
			m_MinParticleRotation = value;
		}
	}

	public float MaxParticleRotation
	{
		get
		{
			return m_MaxParticleRotation;
		}
		set
		{
			m_MaxParticleRotation = value;
		}
	}

	public float MinParticleSpinning
	{
		get
		{
			return m_MinParticleSpinning;
		}
		set
		{
			m_MinParticleSpinning = value;
		}
	}

	public float MaxParticleSpinning
	{
		get
		{
			return m_MaxParticleSpinning;
		}
		set
		{
			m_MaxParticleSpinning = value;
		}
	}

	public float MinParticleSpawnRadius
	{
		get
		{
			return m_MinParticleSpawnRadius;
		}
		set
		{
			m_MinParticleSpawnRadius = value;
		}
	}

	public float MaxParticleSpawnRadius
	{
		get
		{
			return m_MaxParticleSpawnRadius;
		}
		set
		{
			m_MaxParticleSpawnRadius = value;
		}
	}

	public ParticleSystem(SpriteBatch spriteBatch, Texture2D texture, int maxParticles)
	{
		if (spriteBatch == null)
		{
			throw new ArgumentNullException("spriteBatch");
		}
		if (texture == null)
		{
			throw new ArgumentNullException("texture");
		}
		if (maxParticles <= 0)
		{
			throw new ArgumentOutOfRangeException("maxParticles");
		}
		Particles = Manager.Create<T>(maxParticles);
		SpriteBatch = spriteBatch;
		ParticleTexture = texture;
		BlendMode = (SpriteBlendMode)1;
	}

	public virtual void SpawnParticles(Vector2 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		SpawnParticles(position, Color.White);
	}

	public virtual void SpawnParticles(Vector2 position, Color color)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		int @int = RandomHelper.GetInt32(m_MinParticleSpawnCount, m_MaxSpawnParticles);
		for (int i = 0; i < @int; i++)
		{
			T val = Particles.GetObject();
			if (val != null)
			{
				InitilizeParticle(val, position, color);
				continue;
			}
			break;
		}
	}

	protected virtual void InitilizeParticle(T particle, Vector2 position, Color color)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		int @int = RandomHelper.GetInt32(m_MinParticleLifespan, m_MaxParticleLifespan);
		float single = RandomHelper.GetSingle(m_MinParticleScaling, m_MaxParticleScaling);
		Vector2 direction = VectorHelper.GetDirection(RandomHelper.GetSingle(m_MinParticleAngle, m_MaxParticleAngle));
		float single2 = RandomHelper.GetSingle(m_MinParticleSpeed, m_MaxParticleSpeed);
		float single3 = RandomHelper.GetSingle(m_MinParticleAcceleration, m_MaxParticleAcceleration);
		float single4 = RandomHelper.GetSingle(m_MinParticleRotation, m_MaxParticleRotation);
		RandomHelper.GetSingle(m_MinParticleSpinning, m_MaxParticleSpinning);
		Vector2 val = VectorHelper.GetDirection(RandomHelper.GetAngle()) * RandomHelper.GetSingle(m_MinParticleSpawnRadius, m_MaxParticleSpawnRadius);
		particle.Position = (position + val).GetVector3();
		particle.Velocity = (direction * single2).GetVector3();
		particle.Acceleration = (direction * single3).GetVector3();
		particle.Scaling = new Vector3(single, single, single);
		particle.Rotation.Z = single4;
		particle.Lifespan = @int;
		particle.Color = color;
		particle.Activate();
	}

	protected override void DoUpdate(GameTime gameTime)
	{
		Particles.Update(gameTime);
	}

	protected override void DoDraw(GameTime gameTime)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector((float)(ParticleTexture.Width / 2), (float)(ParticleTexture.Height / 2));
		SpriteBatch.Begin(BlendMode);
		Color val2 = default(Color);
		foreach (T item in Particles.Active)
		{
			if (item.IsVisible)
			{
				float normalizedLifetime = item.NormalizedLifetime;
				float num = 4f * normalizedLifetime * (1f - normalizedLifetime);
				((Color)(ref val2))._002Ector(item.Color, num);
				SpriteBatch.Draw(ParticleTexture, item.Position.GetVector2(), (Rectangle?)null, val2, item.Rotation.Z, val, item.Scaling.GetVector2(), (SpriteEffects)0, 0f);
			}
		}
		SpriteBatch.End();
	}
}
public class ParticleSystem : ParticleSystem<Particle>
{
	public ParticleSystem(SpriteBatch spriteBatch, Texture2D texture, int maxParticles)
		: base(spriteBatch, texture, maxParticles)
	{
	}
}
