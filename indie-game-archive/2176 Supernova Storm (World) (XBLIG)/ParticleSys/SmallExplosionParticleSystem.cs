using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ParticleSys;

internal class SmallExplosionParticleSystem : ParticleSystem
{
	public SmallExplosionParticleSystem(Game game, ContentManager content)
		: base(game, content)
	{
	}

	protected override void InitializeSettings(ParticleSettings settings)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		settings.TextureName = "Textures/explosion";
		settings.MaxParticles = 200;
		settings.Duration = TimeSpan.FromSeconds(1.5);
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
		settings.MinStartSize = 400f;
		settings.MaxStartSize = 400f;
		settings.MinEndSize = 3000f;
		settings.MaxEndSize = 4000f;
		settings.SourceBlend = (Blend)5;
		settings.DestinationBlend = (Blend)2;
	}

	public void CreateExplosion(Vector3 pos, Vector3 velocity)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 5; i++)
		{
			AddParticle(pos, velocity);
		}
	}
}
