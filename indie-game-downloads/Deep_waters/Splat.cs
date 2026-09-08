using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Deep_waters;

public class Splat : ParticleSystem
{
	public Splat(GraphicsDevice gdev, Texture2D tex, Effect eff, Vector3 pos)
		: base(gdev, tex, eff, pos)
	{
	}

	protected override void InitializeSettings(ParticleSettings settings)
	{
		settings.TextureName = "Splat";
		settings.MaxParticles = 10;
		settings.totalparticles = 10;
		settings.emitterlifetime = 0f;
		settings.Duration = TimeSpan.FromSeconds(20.0);
		settings.DurationRandomness = 5f;
		settings.Gravity = new Vector3(0f, -1f, 0f);
		settings.MinHorizontalVelocity = 0f;
		settings.MaxHorizontalVelocity = 0.4f;
		settings.MinVerticalVelocity = 0f;
		settings.MaxVerticalVelocity = 0f;
		settings.EndVelocity = 0f;
		settings.MinColor = Color.Gray;
		settings.MaxColor = Color.White;
		settings.MinRotateSpeed = 0.1f;
		settings.MaxRotateSpeed = 0.5f;
		settings.MinStartSize = 2f;
		settings.MaxStartSize = 3f;
		settings.MinEndSize = 5f;
		settings.MaxEndSize = 10f;
		settings.BlendState = BlendState.NonPremultiplied;
	}
}
