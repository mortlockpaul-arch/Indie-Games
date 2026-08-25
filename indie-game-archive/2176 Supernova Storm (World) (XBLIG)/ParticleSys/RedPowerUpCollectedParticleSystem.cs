using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ParticleSys;

internal class RedPowerUpCollectedParticleSystem : ParticleSystem
{
	public RedPowerUpCollectedParticleSystem(Game game, ContentManager content)
		: base(game, content)
	{
	}

	protected override void InitializeSettings(ParticleSettings settings)
	{
		settings.TextureName = "Textures/PUPS_Red";
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
		settings.SourceBlend = Blend.SourceAlpha;
		settings.DestinationBlend = Blend.One;
	}

	public void CreatePowerupCollectedParticles(Vector3 pos)
	{
		for (int i = 0; i < 10; i++)
		{
			AddParticle(pos, Vector3.Zero);
		}
	}
}
