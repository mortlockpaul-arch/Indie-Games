using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Deep_waters;

public class WaterDrop : ParticleSystem
{
	public WaterDrop(GraphicsDevice gdev, Texture2D tex, Effect eff, Vector3 pos)
		: base(gdev, tex, eff, pos)
	{
	}

	protected override void InitializeSettings(ParticleSettings settings)
	{
		settings.TextureName = "Splat";
		settings.MaxParticles = 40;
		settings.totalparticles = 40;
		settings.isabox = true;
		settings.emitterlifetime = 0f;
		settings.Duration = TimeSpan.FromSeconds(2.0);
		settings.DurationRandomness = 1f;
		settings.Gravity = new Vector3(0f, -9f, 0f);
		settings.MinHorizontalVelocity = 0.2f;
		settings.MaxHorizontalVelocity = 1.5f;
		settings.MinVerticalVelocity = 0.1f;
		settings.MaxVerticalVelocity = 0.3f;
		settings.EndVelocity = 0f;
		settings.MinColor = Color.White;
		settings.MaxColor = Color.Transparent;
		settings.MinRotateSpeed = 1f;
		settings.MaxRotateSpeed = 10f;
		settings.MinStartSize = 1f;
		settings.MaxStartSize = 2f;
		settings.MinEndSize = 0.01f;
		settings.MaxEndSize = 0.005f;
		settings.BlendState = BlendState.Additive;
	}
}
