using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DPSF.ParticleSystems;

internal class ExplosionFireSmokeParticleSystem(Game game) : DefaultSprite3DBillboardTextureCoordinatesParticleSystem(game)
{
	private Rectangle _flameSmoke1TextureCoordinates = new Rectangle(0, 0, 128, 128);

	private Rectangle _flameSmoke2TextureCoordinates = new Rectangle(128, 0, 128, 128);

	private Rectangle _flameSmoke3TextureCoordinates = new Rectangle(0, 128, 128, 128);

	private Rectangle _flameSmoke4TextureCoordinates = new Rectangle(128, 128, 128, 128);

	public Color ExplosionColor { get; set; }

	public int ExplosionParticleSize { get; set; }

	public Vector3 vitesse { get; set; }

	public int ExplosionIntensity { get; set; }

	public override void SetCameraPosition(Vector3 cameraPosition)
	{
		base.CameraPosition = cameraPosition;
	}

	protected override void InitializeRenderProperties()
	{
		base.InitializeRenderProperties();
		base.RenderProperties.BlendState = BlendState.Additive;
	}

	public override void AutoInitialize(GraphicsDevice graphicsDevice, ContentManager contentManager, SpriteBatch spriteBatch)
	{
		InitializeSpriteParticleSystem(graphicsDevice, contentManager, 1000, 50000, "Textures/ExplosionParticles");
		base.Name = "Explosion - Fire Smoke";
		LoadEvents();
	}

	public void LoadEvents()
	{
		base.ParticleInitializationFunction = InitializeParticleExplosion;
		base.ParticleEvents.RemoveAllEvents();
		base.ParticleEvents.AddEveryTimeEvent(base.UpdateParticleVelocityUsingExternalForce);
		base.ParticleEvents.AddEveryTimeEvent(base.UpdateParticlePositionAndVelocityUsingAcceleration);
		base.ParticleEvents.AddEveryTimeEvent(base.UpdateParticleRotationAndRotationalVelocityUsingRotationalAcceleration);
		base.ParticleEvents.AddEveryTimeEvent(base.UpdateParticleTransparencyWithQuickFadeInAndSlowFadeOut, 100);
		base.ParticleEvents.AddEveryTimeEvent(UpdateParticleFireSmokeColor);
		base.ParticleEvents.AddEveryTimeEvent(UpdateParticleFireSmokeSize);
		base.Emitter.ParticlesPerSecond = 500f;
		base.Emitter.EmitParticlesAutomatically = false;
	}

	public void SetupToAutoExplodeEveryInterval(float intervalInSeconds)
	{
		base.ParticleSystemEvents.RemoveAllEventsInGroup(1);
		base.ParticleSystemEvents.LifetimeData.EndOfLifeOption = CParticleSystemEvents.EParticleSystemEndOfLifeOptions.Repeat;
		base.ParticleSystemEvents.LifetimeData.Lifetime = intervalInSeconds;
		base.ParticleSystemEvents.AddTimedEvent(0f, UpdateParticleSystemToExplode, 0, 1);
	}

	public void InitializeParticleExplosion(DefaultSprite3DBillboardTextureCoordinatesParticle particle)
	{
		particle.Lifetime = 0.4f;
		particle.Color = (particle.StartColor = ExplosionColor);
		particle.EndColor = Color.White;
		particle.Position = base.Emitter.PositionData.Position + new Vector3(0f, 0f, 0f);
		particle.Velocity = vitesse;
		particle.ExternalForce = new Vector3(1f, -6f, 1f);
		float size = (particle.StartSize = 0.1f);
		particle.Size = size;
		particle.EndSize = ExplosionParticleSize;
		particle.Rotation = base.RandomNumber.Between(0f, (float)Math.PI * 2f);
		particle.RotationalVelocity = base.RandomNumber.Between(-(float)Math.PI / 2f, (float)Math.PI / 2f);
		particle.SetTextureCoordinates(base.RandomNumber.Next(0, 4) switch
		{
			1 => _flameSmoke2TextureCoordinates, 
			2 => _flameSmoke3TextureCoordinates, 
			3 => _flameSmoke4TextureCoordinates, 
			_ => _flameSmoke1TextureCoordinates, 
		});
	}

	protected void UpdateParticleFireSmokeColor(DefaultSprite3DBillboardTextureCoordinatesParticle particle, float elapsedTimeInSeconds)
	{
		float num = 0.3f;
		if (particle.NormalizedElapsedTime < num)
		{
			particle.Color = particle.StartColor;
			return;
		}
		float fInterpolationAmount = (particle.NormalizedElapsedTime - num) * (1f / (1f - num));
		particle.Color = DPSFHelper.LerpColor(particle.StartColor, particle.EndColor, fInterpolationAmount);
	}

	protected void UpdateParticleFireSmokeSize(DefaultSprite3DBillboardTextureCoordinatesParticle particle, float elapsedTimeInSeconds)
	{
		if (particle.NormalizedElapsedTime < 0.1f)
		{
			particle.Size = MathHelper.Lerp(particle.StartWidth, particle.EndWidth, particle.NormalizedElapsedTime * 4f);
		}
		else
		{
			particle.Size = particle.EndWidth;
		}
	}

	protected void UpdateParticleSystemToExplode(float elapsedTimeInSeconds)
	{
		Explode();
	}

	public void Explode()
	{
		base.Emitter.BurstParticles = ExplosionIntensity;
	}

	public void ChangeExplosionColor()
	{
		ExplosionColor = DPSFHelper.RandomColor();
	}

	public void ChangeExplosionColor(Color color)
	{
		ExplosionColor = color;
	}
}
