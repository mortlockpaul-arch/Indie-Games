using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Deep_waters;

internal class FireParticleSystem : ParticleSystem
{
	public FireParticleSystem(GraphicsDevice gdev, Texture2D tex, Effect eff, Vector3 position)
		: base(gdev, tex, eff, position)
	{
	}

	protected override void InitializeSettings(ParticleSettings settings)
	{
		settings.TextureName = "fire";
		settings.isVisible = false;
		settings.isUpdatable = false;
		settings.MaxParticles = 100;
		settings.totalparticles = -1;
		settings.Duration = TimeSpan.FromSeconds(0.5);
		settings.DurationRandomness = 0.5f;
		settings.MinHorizontalVelocity = -0.02f;
		settings.MaxHorizontalVelocity = 0.02f;
		settings.MinVerticalVelocity = 0f;
		settings.MaxVerticalVelocity = 0.01f;
		settings.Gravity = new Vector3(0f, 0.02f, 0f);
		settings.MinColor = new Color(255, 255, 255, 50);
		settings.MaxColor = new Color(255, 255, 255, 100);
		settings.MinStartSize = 3f;
		settings.MaxStartSize = 2f;
		settings.MinEndSize = 1f;
		settings.MaxEndSize = 0.5f;
		settings.BlendState = BlendState.Additive;
	}
}
