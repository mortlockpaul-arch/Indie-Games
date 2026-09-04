using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ParticleSys;

internal class ExplosionParticleSystem : ParticleSystem
{
	public ExplosionParticleSystem(Game game, ContentManager content)
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
		settings.TextureName = "Textures/explosion";
		settings.MaxParticles = 100;
		settings.Duration = TimeSpan.FromSeconds(2.0);
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
		settings.MinStartSize = 2000f;
		settings.MaxStartSize = 2000f;
		settings.MinEndSize = 20000f;
		settings.MaxEndSize = 30000f;
		settings.SourceBlend = (Blend)5;
		settings.DestinationBlend = (Blend)2;
	}

	public void CreateExplosion(Vector3 pos, Vector3 velocity)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 30; i++)
		{
			AddParticle(pos, velocity);
		}
	}
}
