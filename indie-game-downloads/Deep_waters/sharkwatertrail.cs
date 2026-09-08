using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Deep_waters;

public class sharkwatertrail : ParticleSystem
{
	public sharkwatertrail(GraphicsDevice gdev, Texture2D tex, Effect eff, Vector3 pos)
		: base(gdev, tex, eff, pos)
	{
	}

	protected override void InitializeSettings(ParticleSettings settings)
	{
		settings.TextureName = "Splat";
		settings.MaxParticles = 100;
		settings.totalparticles = 0;
		settings.isabox = true;
		settings.emitterlifetime = 0f;
		settings.Duration = TimeSpan.FromSeconds(1.0);
		settings.DurationRandomness = 0f;
		settings.Gravity = new Vector3(0f, -1f, 0f);
		settings.MinHorizontalVelocity = 0.4f;
		settings.MaxHorizontalVelocity = 0.8f;
		settings.MinVerticalVelocity = 0f;
		settings.MaxVerticalVelocity = 0f;
		settings.EndVelocity = 1f;
		settings.MinColor = Color.White;
		settings.MaxColor = Color.Transparent;
		settings.MinRotateSpeed = 1f;
		settings.MaxRotateSpeed = 10f;
		settings.MinStartSize = 8f;
		settings.MaxStartSize = 6f;
		settings.MinEndSize = 0.01f;
		settings.MaxEndSize = 0.005f;
		settings.BlendState = BlendState.Additive;
	}
}
