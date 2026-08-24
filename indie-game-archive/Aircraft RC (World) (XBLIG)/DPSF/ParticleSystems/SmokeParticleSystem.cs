using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DPSF.ParticleSystems;

internal class SmokeParticleSystem(Game cGame) : DefaultSprite3DBillboardParticleSystem(cGame)
{
	private Color[] msaColors = new Color[9]
	{
		Color.WhiteSmoke,
		Color.Gray,
		Color.Black,
		Color.Green,
		Color.Yellow,
		Color.Red,
		Color.Pink,
		Color.Brown,
		Color.Blue
	};

	private int miCurrentColor;

	public float mfColorBlendAmount = 0.5f;

	public Vector3 mcExternalObjectPosition = Vector3.Zero;

	public float mfAttractRepelForce = 3f;

	public float mfAttractRepelRange = 50f;

	public Vector3 vitesse;

	public float nombrepar;

	public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
	{
		InitializeSpriteParticleSystem(cGraphicsDevice, cContentManager, 500, 40000, "Textures/Smoke");
		LoadSmokeEvents();
		base.Emitter.ParticlesPerSecond = 60f;
		base.Name = "Smoke";
	}

	public void LoadSmokeEvents()
	{
		base.ParticleInitializationFunction = InitializeParticleRisingSmoke;
		base.ParticleEvents.RemoveAllEvents();
		base.ParticleEvents.AddEveryTimeEvent(base.UpdateParticlePositionAndVelocityUsingAcceleration, 500);
		base.ParticleEvents.AddEveryTimeEvent(base.UpdateParticleRotationUsingRotationalVelocity);
		base.ParticleEvents.AddEveryTimeEvent(UpdateColor);
		base.ParticleEvents.AddEveryTimeEvent(base.UpdateParticleTransparencyWithQuickFadeInAndSlowFadeOut, 100);
		base.ParticleEvents.AddEveryTimeEvent(IncreaseSizeBasedOnLifetime);
	}

	public void InitializeParticleRisingSmoke(DefaultSprite3DBillboardParticle cParticle)
	{
		cParticle.Lifetime = nombrepar;
		cParticle.Position = base.Emitter.PositionData.Position;
		cParticle.Size = base.RandomNumber.Between(0.1f, 0.9f);
		cParticle.Color = msaColors[miCurrentColor];
		cParticle.Rotation = base.RandomNumber.Between(0f, (float)Math.PI * 2f);
		cParticle.Velocity = vitesse;
		cParticle.Acceleration = Vector3.Zero;
		cParticle.RotationalVelocity = base.RandomNumber.Between((float)Math.PI * -100f, (float)Math.PI);
		cParticle.StartSize = cParticle.Size;
		mfColorBlendAmount = 0.5f;
	}

	public void InitializeParticleFoggySmoke(DefaultSprite3DBillboardParticle cParticle)
	{
		cParticle.Lifetime = base.RandomNumber.Between(1f, 3f);
		cParticle.Position = base.Emitter.PositionData.Position;
		cParticle.Position += new Vector3(base.RandomNumber.Next(-500, 500), 0f, base.RandomNumber.Next(-500, 500));
		cParticle.Size = base.RandomNumber.Next(10, 25);
		cParticle.Color = msaColors[miCurrentColor];
		cParticle.Rotation = base.RandomNumber.Between(0f, (float)Math.PI * 2f);
		cParticle.Velocity = vitesse;
		cParticle.Acceleration = Vector3.Zero;
		cParticle.RotationalVelocity = base.RandomNumber.Between(-(float)Math.PI, (float)Math.PI);
		cParticle.StartSize = cParticle.Size;
		mfColorBlendAmount = 0.5f;
	}

	protected void IncreaseSizeBasedOnLifetime(DefaultSprite3DBillboardParticle cParticle, float fElapsedTimeInSeconds)
	{
		cParticle.Size = (1f + cParticle.NormalizedElapsedTime) / 1f * cParticle.StartSize;
	}

	protected void UpdateColor(DefaultSprite3DBillboardParticle cParticle, float fElapsedTimeInSeconds)
	{
		cParticle.Color = msaColors[miCurrentColor];
	}

	protected void RepelParticleFromExternalObject(DefaultSprite3DBillboardParticle cParticle, float fElapsedTimeInSeconds)
	{
		Vector3 vector = cParticle.Position - mcExternalObjectPosition;
		float num = vector.Length();
		if (num < mfAttractRepelRange)
		{
			vector.Normalize();
			cParticle.Velocity += vector * (mfAttractRepelRange - num) * mfAttractRepelForce;
			cParticle.RotationalVelocity += 0.005f;
		}
	}

	protected void AttractParticleToExternalObject(DefaultSprite3DBillboardParticle cParticle, float fElapsedTimeInSeconds)
	{
		Vector3 vector = mcExternalObjectPosition - cParticle.Position;
		float num = vector.Length();
		if (num < mfAttractRepelRange)
		{
			vector.Normalize();
			cParticle.Velocity = vector * (mfAttractRepelRange - num) * mfAttractRepelForce;
		}
	}

	public void ChangeColor()
	{
		if (checked(++miCurrentColor) >= msaColors.Length)
		{
			miCurrentColor = 0;
		}
	}

	public void MakeParticlesAttractToExternalObject()
	{
		base.ParticleEvents.RemoveEveryTimeEvents(AttractParticleToExternalObject);
		base.ParticleEvents.AddEveryTimeEvent(AttractParticleToExternalObject);
	}

	public void MakeParticlesRepelFromExternalObject()
	{
		base.ParticleEvents.RemoveEveryTimeEvents(RepelParticleFromExternalObject);
		base.ParticleEvents.AddEveryTimeEvent(RepelParticleFromExternalObject);
	}

	public void StopParticleAttractionAndRepulsionToExternalObject()
	{
		base.ParticleEvents.RemoveEveryTimeEvents(RepelParticleFromExternalObject);
		base.ParticleEvents.RemoveEveryTimeEvents(AttractParticleToExternalObject);
	}
}
