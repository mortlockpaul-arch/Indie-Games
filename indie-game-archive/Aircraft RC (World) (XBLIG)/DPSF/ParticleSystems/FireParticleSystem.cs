using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DPSF.ParticleSystems;

internal class FireParticleSystem : DefaultTexturedQuadParticleSystem
{
	private bool mbUseAdditiveBlending;

	public float diametre;

	public FireParticleSystem(Game cGame)
		: base(cGame)
	{
	}

	protected override void InitializeRenderProperties()
	{
		base.InitializeRenderProperties();
		mbUseAdditiveBlending = false;
		ToggleAdditiveBlending();
	}

	public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
	{
		InitializeTexturedQuadParticleSystem(cGraphicsDevice, cContentManager, 10, 1000, UpdateVertexProperties, "Textures/Fire");
		base.Name = "Fire and Smoke";
		LoadFireRingEvents();
		base.Emitter.ParticlesPerSecond = 400f;
	}

	public void LoadFireRingEvents()
	{
		base.ParticleInitializationFunction = InitializeParticleFireOnVerticalRing;
		base.ParticleEvents.RemoveAllEvents();
		base.ParticleEvents.AddEveryTimeEvent(base.UpdateParticlePositionAndVelocityUsingAcceleration);
		base.ParticleEvents.AddEveryTimeEvent(base.UpdateParticleRotationUsingRotationalVelocity);
		base.ParticleEvents.AddEveryTimeEvent(base.UpdateParticleTransparencyWithQuickFadeInAndSlowFadeOut, 100);
		base.ParticleEvents.AddEveryTimeEvent(ReduceSizeBasedOnLifetime);
		base.ParticleEvents.AddEveryTimeEvent(base.UpdateParticleToFaceTheCamera, 200);
		base.Emitter.PositionData.Position = new Vector3(0f, 50f, 0f);
		base.InitialProperties.LifetimeMin = 0.005f;
		base.InitialProperties.LifetimeMax = 0.005f;
		base.InitialProperties.PositionMin = Vector3.Zero;
		base.InitialProperties.PositionMax = Vector3.Zero;
		base.InitialProperties.StartSizeMin = 0.9f;
		base.InitialProperties.StartSizeMax = 1.7f;
		base.InitialProperties.EndSizeMin = 0.9f;
		base.InitialProperties.EndSizeMax = 1.7f;
		base.InitialProperties.StartColorMin = Color.White;
		base.InitialProperties.StartColorMax = Color.White;
		base.InitialProperties.EndColorMin = Color.White;
		base.InitialProperties.EndColorMax = Color.White;
		base.InitialProperties.InterpolateBetweenMinAndMaxColors = false;
		base.InitialProperties.RotationMin = Vector3.Zero;
		base.InitialProperties.RotationMax.Z = (float)Math.PI * 2f;
		base.InitialProperties.VelocityMin = new Vector3(0f, 0f, 0f);
		base.InitialProperties.VelocityMax = new Vector3(0f, 0f, 0f);
		base.InitialProperties.AccelerationMin = Vector3.Zero;
		base.InitialProperties.AccelerationMax = Vector3.Zero;
		base.InitialProperties.RotationalVelocityMin.Z = (float)Math.PI * -2f;
		base.InitialProperties.RotationalVelocityMax.Z = (float)Math.PI * 2f;
	}

	public void InitializeParticleFireOnVerticalRing(DefaultTexturedQuadParticle cParticle)
	{
		Quaternion orientation = base.Emitter.OrientationData.Orientation;
		base.Emitter.OrientationData.Orientation = Quaternion.Identity;
		InitializeParticleUsingInitialProperties(cParticle);
		base.Emitter.OrientationData.Orientation = orientation;
		cParticle.Position = DPSFHelper.PointOnSphere((float)Math.PI / 2f, base.RandomNumber.Between(0f, 2f), diametre);
		cParticle.Position = Vector3.Transform(cParticle.Position, base.Emitter.OrientationData.Orientation);
		cParticle.Position += base.Emitter.PositionData.Position;
	}

	protected void ReduceSizeBasedOnLifetime(DefaultTexturedQuadParticle cParticle, float fElapsedTimeInSeconds)
	{
		cParticle.Size = (1f - cParticle.NormalizedElapsedTime) / 1f * cParticle.StartSize;
	}

	public void ToggleAdditiveBlending()
	{
		mbUseAdditiveBlending = !mbUseAdditiveBlending;
		if (mbUseAdditiveBlending)
		{
			base.RenderProperties.BlendState = BlendState.Additive;
		}
		else
		{
			base.RenderProperties.BlendState = BlendState.AlphaBlend;
		}
	}

	public void SetAmountOfSmokeToRelease(float fNormalizedAmount)
	{
		if (fNormalizedAmount < 0f)
		{
			fNormalizedAmount = 0f;
		}
		else if (fNormalizedAmount > 1f)
		{
			fNormalizedAmount = 1f;
		}
	}
}
