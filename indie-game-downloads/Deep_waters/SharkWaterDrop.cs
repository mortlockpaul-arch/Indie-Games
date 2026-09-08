using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Deep_waters;

public class SharkWaterDrop : ParticleSystem
{
	public SharkWaterDrop(GraphicsDevice gdev, Texture2D tex, Effect eff, Vector3 pos)
		: base(gdev, tex, eff, pos)
	{
	}

	protected override void InitializeSettings(ParticleSettings settings)
	{
		settings.TextureName = "Splat";
		settings.MaxParticles = 150;
		settings.totalparticles = 150;
		settings.isabox = true;
		settings.emitterlifetime = 0f;
		settings.Duration = TimeSpan.FromSeconds(1.0);
		settings.DurationRandomness = 1f;
		settings.Gravity = new Vector3(0f, -9f, 0f);
		settings.MinHorizontalVelocity = 0.2f;
		settings.MaxHorizontalVelocity = 1.5f;
		settings.MinVerticalVelocity = 0f;
		settings.MaxVerticalVelocity = 0.1f;
		settings.EndVelocity = 0f;
		settings.MinColor = Color.White;
		settings.MaxColor = Color.Transparent;
		settings.MinRotateSpeed = 1f;
		settings.MaxRotateSpeed = 10f;
		settings.MinStartSize = 4f;
		settings.MaxStartSize = 8f;
		settings.MinEndSize = 0.01f;
		settings.MaxEndSize = 0.02f;
		settings.BlendState = BlendState.Additive;
	}
}
