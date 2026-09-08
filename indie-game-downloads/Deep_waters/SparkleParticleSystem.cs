using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Deep_waters;

public class SparkleParticleSystem : ParticleSystem
{
	public SparkleParticleSystem(GraphicsDevice gdev, Texture2D tex, Effect eff, Vector3 position)
		: base(gdev, tex, eff, position)
	{
	}

	protected override void InitializeSettings(ParticleSettings settings)
	{
		settings.TextureName = "smoke";
		settings.MaxParticles = 10;
		settings.totalparticles = 10;
		settings.Duration = TimeSpan.FromSeconds(1.0);
		settings.DurationRandomness = 1f;
		settings.MinHorizontalVelocity = -10f;
		settings.MaxHorizontalVelocity = 10f;
		settings.MinVerticalVelocity = -10f;
		settings.MaxVerticalVelocity = 10f;
		settings.Gravity = new Vector3(0f, -50f, 0f);
		settings.EndVelocity = 0f;
		settings.MinColor = Color.White;
		settings.MaxColor = Color.White;
		settings.MinRotateSpeed = -2f;
		settings.MaxRotateSpeed = 2f;
		settings.MinStartSize = 20f;
		settings.MaxStartSize = 20f;
		settings.MinEndSize = 5f;
		settings.MaxEndSize = 5f;
		settings.BlendState = BlendState.Additive;
	}
}
