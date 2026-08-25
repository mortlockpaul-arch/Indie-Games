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
		settings.SourceBlend = Blend.SourceAlpha;
		settings.DestinationBlend = Blend.One;
	}

	public void CreateExplosion(Vector3 pos, Vector3 velocity)
	{
		for (int i = 0; i < 30; i++)
		{
			AddParticle(pos, velocity);
		}
	}
}
