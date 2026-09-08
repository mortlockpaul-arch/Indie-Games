using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Deep_waters;

public class ProjectileTrailParticleSystem : ParticleSystem
{
	public ProjectileTrailParticleSystem(GraphicsDevice gdev, Texture2D tex, Effect eff, Vector3 position)
		: base(gdev, tex, eff, position)
	{
	}

	protected override void InitializeSettings(ParticleSettings settings)
	{
		settings.TextureName = "smoke";
		settings.MaxParticles = 1000;
		settings.Duration = TimeSpan.FromSeconds(3.0);
		settings.DurationRandomness = 1.5f;
		settings.EmitterVelocitySensitivity = 0.1f;
		settings.MinHorizontalVelocity = 0f;
		settings.MaxHorizontalVelocity = 1f;
		settings.MinVerticalVelocity = -1f;
		settings.MaxVerticalVelocity = 1f;
		settings.MinColor = new Color(64, 96, 128, 255);
		settings.MaxColor = new Color(255, 255, 255, 128);
		settings.MinRotateSpeed = -4f;
		settings.MaxRotateSpeed = 4f;
		settings.MinStartSize = 1f;
		settings.MaxStartSize = 3f;
		settings.MinEndSize = 4f;
		settings.MaxEndSize = 11f;
	}
}
