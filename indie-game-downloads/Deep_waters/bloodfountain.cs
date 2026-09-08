using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Deep_waters;

public class bloodfountain : ParticleSystem
{
	public bloodfountain(GraphicsDevice gdev, Texture2D tex, Effect eff, Vector3 pos)
		: base(gdev, tex, eff, pos)
	{
	}

	protected override void InitializeSettings(ParticleSettings settings)
	{
		settings.TextureName = "Splat";
		settings.MaxParticles = 50;
		settings.totalparticles = 50;
		settings.emitterlifetime = 0f;
		settings.Duration = TimeSpan.FromSeconds(2.0);
		settings.DurationRandomness = 1f;
		settings.Gravity = new Vector3(0f, -9f, 0f);
		settings.MinHorizontalVelocity = 0f;
		settings.MaxHorizontalVelocity = 0.4f;
		settings.MinVerticalVelocity = 0.2f;
		settings.MaxVerticalVelocity = 0.3f;
		settings.EndVelocity = 0f;
		settings.MinColor = Color.Gray;
		settings.MaxColor = Color.White;
		settings.MinRotateSpeed = 0.1f;
		settings.MaxRotateSpeed = 0.5f;
		settings.MinStartSize = 0.5f;
		settings.MaxStartSize = 1f;
		settings.MinEndSize = 0.01f;
		settings.MaxEndSize = 0.05f;
		settings.BlendState = BlendState.NonPremultiplied;
	}
}
