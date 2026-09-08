using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Deep_waters;

public class CustomParticleSystem : ParticleSystem
{
	private ParticleSettings msettings;

	public CustomParticleSystem(GraphicsDevice gdev, Texture2D tex, Effect eff, Vector3 pos)
		: base(gdev, tex, eff, pos)
	{
		msettings = new ParticleSettings();
	}

	protected override void InitializeSettings(ParticleSettings settings)
	{
		settings.TextureName = "Fire";
		settings.MaxParticles = 40;
		settings.totalparticles = 40;
		settings.emitterlifetime = 0f;
		settings.Duration = TimeSpan.FromSeconds(1.0);
		settings.DurationRandomness = 1f;
		settings.MinHorizontalVelocity = -5f;
		settings.MaxHorizontalVelocity = 5f;
		settings.MinVerticalVelocity = -5f;
		settings.MaxVerticalVelocity = 5f;
		settings.EndVelocity = 0f;
		settings.MinColor = Color.Gray;
		settings.MaxColor = Color.White;
		settings.MinRotateSpeed = -1f;
		settings.MaxRotateSpeed = 1f;
		settings.MinStartSize = 20f;
		settings.MaxStartSize = 20f;
		settings.MinEndSize = 100f;
		settings.MaxEndSize = 200f;
		settings.BlendState = BlendState.Additive;
	}
}
