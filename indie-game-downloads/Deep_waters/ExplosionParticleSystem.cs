using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Deep_waters;

public class ExplosionParticleSystem : ParticleSystem
{
	public ExplosionParticleSystem(GraphicsDevice gdev, Texture2D tex, Effect eff, Vector3 pos)
		: base(gdev, tex, eff, pos)
	{
	}

	protected override void InitializeSettings(ParticleSettings settings)
	{
		settings.TextureName = "Fire";
		settings.MaxParticles = 10;
		settings.totalparticles = 10;
		settings.emitterlifetime = 0f;
		settings.Duration = TimeSpan.FromSeconds(1.0);
		settings.DurationRandomness = 1f;
		settings.MinHorizontalVelocity = -25f;
		settings.MaxHorizontalVelocity = 25f;
		settings.MinVerticalVelocity = -8f;
		settings.MaxVerticalVelocity = 8f;
		settings.EndVelocity = 0f;
		settings.MinColor = Color.Gray;
		settings.MaxColor = Color.White;
		settings.MinRotateSpeed = -2f;
		settings.MaxRotateSpeed = 2f;
		settings.MinStartSize = 30f;
		settings.MaxStartSize = 30f;
		settings.MinEndSize = 80f;
		settings.MaxEndSize = 100f;
		settings.BlendState = BlendState.Additive;
	}
}
