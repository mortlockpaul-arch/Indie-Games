using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public abstract class ParticleSystem2D : DrawableGameComponent
{
	public const int AlphaBlendDrawOrder = 100;

	public const int AdditiveDrawOrder = 200;

	private static bool UseOffsetPositions = false;

	private static Vector2 Offset_ScreenSize;

	private static Vector2 Offset_Center;

	protected Game game;

	protected SpriteBatch SB;

	protected bool UseBaseDraw = true;

	protected bool DontUseGameTime;

	private bool dontBeAComponent;

	private Texture2D texture;

	private Vector2 origin;

	private int howManyEffects;

	private Particle2D[] particles;

	protected Queue<Particle2D> freeParticles;

	private int number;

	private static int totalCount = 0;

	protected float depthMin;

	protected float depthMax = 1f;

	protected int minNumParticles;

	protected int maxNumParticles;

	protected bool PixelMode;

	protected float minInitialSpeed;

	protected float maxInitialSpeed;

	protected float minAcceleration;

	protected float maxAcceleration;

	protected float minRotationSpeed;

	protected float maxRotationSpeed;

	protected float minLifetime;

	protected float maxLifetime;

	protected float minScale;

	protected float maxScale;

	protected float minGravity;

	protected float maxGravity;

	protected BlendState blendState;

	private string texFileName;

	protected Particle2D[] GetParticles => particles;

	public bool AnyFreeParticle => freeParticles.Count > 0;

	public int Number => number;

	public static void SetOffsetPositions(Vector2 ScreenSize, Vector2 Center)
	{
		UseOffsetPositions = true;
		Offset_ScreenSize = ScreenSize;
		UpdateOffset_Center(Center);
	}

	public static void UpdateOffset_Center(Vector2 Center)
	{
		Offset_Center = Center;
	}

	public static Vector2 PositionWithOffset(Vector2 pos)
	{
		if (!UseOffsetPositions)
		{
			return pos;
		}
		return new Vector2(Offset_ScreenSize.X / 2f - Offset_Center.X + pos.X, Offset_ScreenSize.Y / 2f - Offset_Center.Y + pos.Y);
	}

	protected Particle2D DequeueFreePart()
	{
		return freeParticles.Dequeue();
	}

	protected void EnqueueFreePart(Particle2D p)
	{
		freeParticles.Enqueue(p);
	}

	protected ParticleSystem2D(Game game, SpriteBatch sb, string texFileName, int howManyEffects)
		: this(game, sb, texFileName, howManyEffects, dontUseGameTime: false, dontBeAComponent: false)
	{
	}

	protected ParticleSystem2D(Game game, SpriteBatch sb, string texFileName, int howManyEffects, bool dontUseGameTime, bool dontBeAComponent)
		: base(game)
	{
		this.game = game;
		SB = sb;
		this.howManyEffects = howManyEffects;
		this.texFileName = texFileName;
		DontUseGameTime = dontUseGameTime;
		number = totalCount++;
		this.dontBeAComponent = dontBeAComponent;
		if (!dontBeAComponent)
		{
			game.Components.Add(this);
		}
	}

	public override void Initialize()
	{
		InitializeConstants();
		particles = new Particle2D[howManyEffects * maxNumParticles];
		freeParticles = new Queue<Particle2D>(howManyEffects * maxNumParticles);
		for (int i = 0; i < particles.Length; i++)
		{
			particles[i] = new Particle2D();
			EnqueueFreePart(particles[i]);
		}
		base.Initialize();
	}

	protected abstract void InitializeConstants();

	protected override void LoadContent()
	{
		if (!PixelMode)
		{
			if (string.IsNullOrEmpty(texFileName))
			{
				string message = "textureFilename wasn't set properly, so the particle system doesn't know what texture to load. Make sure your particle system's InitializeConstants function properly sets textureFilename.";
				throw new InvalidOperationException(message);
			}
			texture = game.Content.Load<Texture2D>(texFileName);
			origin.X = texture.Width / 2;
			origin.Y = texture.Height / 2;
			base.LoadContent();
		}
	}

	public void AddParticles(Vector2 where)
	{
		int numParticles = Utils.Random.Next(minNumParticles, maxNumParticles);
		AddParticles(where, numParticles);
	}

	public void AddParticles(Vector2 where, int numParticles)
	{
		for (int i = 0; i < numParticles; i++)
		{
			if (!AnyFreeParticle)
			{
				break;
			}
			Particle2D p = DequeueFreePart();
			InitializeParticle(p, where);
		}
	}

	protected virtual void InitializeParticle(Particle2D p, Vector2 where)
	{
		Vector2 vector = PickRandomDirection();
		float num = Utils.RandomBetween(minInitialSpeed, maxInitialSpeed);
		float num2 = Utils.RandomBetween(minAcceleration, maxAcceleration);
		float lifetime = Utils.RandomBetween(minLifetime, maxLifetime);
		float scale = Utils.RandomBetween(minScale, maxScale);
		float rotationSpeed = Utils.RandomBetween(minRotationSpeed, maxRotationSpeed);
		p.Initialize(where, num * vector, num2 * vector, lifetime, scale, rotationSpeed);
	}

	protected virtual Vector2 PickRandomDirection()
	{
		float num = Utils.RandomBetween(0f, (float)Math.PI * 2f);
		return new Vector2((float)Math.Cos(num), (float)Math.Sin(num));
	}

	public override void Update(GameTime gameTime)
	{
		float dt = (DontUseGameTime ? (1f / 60f) : ((float)gameTime.ElapsedGameTime.TotalSeconds));
		for (int i = 0; i < particles.Length; i++)
		{
			Particle2D particle2D = particles[i];
			if (particle2D.Active)
			{
				particle2D.Update(dt, MathHelper.Lerp(minGravity, maxGravity, Utils.RandomRatio));
				if (!particle2D.Active)
				{
					EnqueueFreePart(particle2D);
				}
			}
		}
	}

	public void Reset()
	{
		Particle2D[] getParticles = GetParticles;
		foreach (Particle2D particle2D in getParticles)
		{
			if (particle2D.Active)
			{
				particle2D.TimeSinceStart = particle2D.Lifetime - 0.01f;
			}
		}
	}

	public override void Draw(GameTime gameTime)
	{
		if (UseBaseDraw)
		{
			DrawManual(gameTime);
		}
	}

	public virtual void DrawManual(GameTime gameTime)
	{
		if (UseBaseDraw)
		{
			SB.Begin(SpriteSortMode.BackToFront, blendState);
		}
		Particle2D[] array = particles;
		foreach (Particle2D particle2D in array)
		{
			if (particle2D.Active)
			{
				float num = particle2D.TimeSinceStart / particle2D.Lifetime;
				float num2 = 4f * num * (1f - num);
				Color color = particle2D.color * num2;
				float scale = particle2D.Scale * (0.75f + 0.25f * num);
				Vector2 vector = PositionWithOffset(particle2D.Position);
				if (Utils.IsVisible(vector, scale, num2, texture.Width, texture.Height, canRotate: true, new Point(game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height)))
				{
					SB.Draw(texture, vector, null, color, particle2D.Rotation, origin, scale, SpriteEffects.None, MathHelper.Lerp(depthMin, depthMax, Utils.clampRatio(num)));
				}
			}
		}
		if (UseBaseDraw)
		{
			SB.End();
		}
		base.Draw(gameTime);
	}
}
