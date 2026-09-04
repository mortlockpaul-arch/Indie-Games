using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ParticleSys;

internal class GreenPowerUpCollectedParticleSystem : ParticleSystem
{
	public GreenPowerUpCollectedParticleSystem(Game game, ContentManager content)
		: base(game, content)
	{
	}

	protected override void InitializeSettings(ParticleSettings settings)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		settings.TextureName = "Textures/PUPS_Green";
		settings.MaxParticles = 100;
		settings.Duration = TimeSpan.FromSeconds(5.0);
		settings.DurationRandomness = 1f;
		settings.MinHorizontalVelocity = 20f;
		settings.MaxHorizontalVelocity = 30f;
		settings.MinVerticalVelocity = -20f;
		settings.MaxVerticalVelocity = 20f;
		settings.EndVelocity = 0f;
		settings.MinColor = Color.DarkGray;
		settings.MaxColor = Color.Gray;
		settings.MinRotateSpeed = -1f;
		settings.MaxRotateSpeed = 1f;
		settings.MinStartSize = 1000f;
		settings.MaxStartSize = 1000f;
		settings.MinEndSize = 100000f;
		settings.MaxEndSize = 200000f;
		settings.SourceBlend = (Blend)5;
		settings.DestinationBlend = (Blend)2;
	}

	public void CreatePowerupCollectedParticles(Vector3 pos)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 10; i++)
		{
			AddParticle(pos, Vector3.Zero);
		}
	}
}
