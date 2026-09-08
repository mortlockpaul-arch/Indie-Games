using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Deep_waters;

public class ExplosionSmokeParticleSystem : ParticleSystem
{
	public ExplosionSmokeParticleSystem(GraphicsDevice gdev, Texture2D tex, Effect eff, Vector3 position)
		: base(gdev, tex, eff, position)
	{
	}

	protected override void InitializeSettings(ParticleSettings settings)
	{
		settings.TextureName = "smoke";
		settings.MaxParticles = 10;
		settings.totalparticles = 10;
		settings.emitterlifetime = 1f;
		settings.Duration = TimeSpan.FromSeconds(1.0);
		settings.DurationRandomness = 1f;
		settings.MinHorizontalVelocity = -15f;
		settings.MaxHorizontalVelocity = 15f;
		settings.MinVerticalVelocity = 5f;
		settings.MaxVerticalVelocity = 15f;
		settings.Gravity = new Vector3(0f, 0f, 0f);
		settings.EndVelocity = 0f;
		settings.MinColor = Color.DarkGray;
		settings.MaxColor = Color.LightGray;
		settings.MinRotateSpeed = -2f;
		settings.MaxRotateSpeed = 2f;
		settings.MinStartSize = 20f;
		settings.MaxStartSize = 20f;
		settings.MinEndSize = 40f;
		settings.MaxEndSize = 80f;
	}
}
